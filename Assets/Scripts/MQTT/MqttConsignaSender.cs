// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class MqttConsignaSender : MonoBehaviour
{
    // Clase para enviar consignas a través de MQTT a la planta de nivel
    // Permite configurar los valores de la bomba y la válvula, así como seleccionar los tanques a los que se enviarán las consignas
    [Header("MQTT")]
    public MqttPublisher mqttPublisher;

    [Header("UI Bomba")]
    public Slider sliderBomba;
    public TMP_Text textoBomba;
    public Toggle toggleBombaUnidad;

    [Header("UI Válvula")]
    public Slider sliderValvula;
    public TMP_Text textoValvula;
    public Toggle toggleValvulaUnidad;

    [Header("Tanques")]
    public List<Toggle> tanqueToggles;

    private float valorBombaPorcentaje = 0f;
    private float valorValvulaPorcentaje = 0f;

    void Start()
    {
        // Al iniciar la aplicación, se conecta al broker MQTT y se configuran los sliders y toggles
        mqttPublisher.Connect();
        ConfigurarSliders();
    }

    void ConfigurarSliders()
    {
        sliderBomba.onValueChanged.AddListener(OnSliderBombaChanged);
        sliderValvula.onValueChanged.AddListener(OnSliderValvulaChanged);
        toggleBombaUnidad.onValueChanged.AddListener(delegate { ActualizarVistaBomba(); });
        toggleValvulaUnidad.onValueChanged.AddListener(delegate { ActualizarVistaValvula(); });

        ActualizarVistaBomba();
        ActualizarVistaValvula();
    }

    void OnSliderBombaChanged(float valorVisual)
    {
        // Actualiza el valor de la bomba según la unidad seleccionada (Voltios o Porcentaje)
        // Si está en Voltios, el valor visual es de 0 a 10;
        // Si está en Porcentaje, el valor visual es de 0 a 100 
        valorBombaPorcentaje = toggleBombaUnidad.isOn ? valorVisual * 10f : valorVisual;
        ActualizarVistaBomba();
    }

    void ActualizarVistaBomba()
    {
        // Actualiza la vista de la bomba según la unidad seleccionada
        // Si está en Voltios, el valor visual es de 0 a 10;
        // Si está en Porcentaje, el valor visual es de 0 a 100
        bool usarVoltios = toggleBombaUnidad.isOn;
        float valorVisual = usarVoltios ? valorBombaPorcentaje / 10f : valorBombaPorcentaje;

        sliderBomba.maxValue = usarVoltios ? 10f : 100f;
        sliderBomba.value = valorVisual;

        textoBomba.text = usarVoltios
            ? $"{valorVisual:0.00}"
            : $"{valorVisual:0.0}";
    }

    void OnSliderValvulaChanged(float valorVisual)
    {
        // Análogamente a la bomba, el valor de la válvula según la unidad seleccionada (Voltios o Porcentaje)
        // Si está en Voltios, el valor visual es de 0 a 10;
        // Si está en Porcentaje, el valor visual es de 0 a 100
        valorValvulaPorcentaje = toggleValvulaUnidad.isOn ? valorVisual * 10f : valorVisual;
        ActualizarVistaValvula();
    }

    void ActualizarVistaValvula()
    {
        // Analógamente a la bomba, actualiza la vista de la válvula según la unidad seleccionada
        // Si está en Voltios, el valor visual es de 0 a 10;
        // Si está en Porcentaje, el valor visual es de 0 a 100
        bool usarVoltios = toggleValvulaUnidad.isOn;
        float valorVisual = usarVoltios ? valorValvulaPorcentaje / 10f : valorValvulaPorcentaje;

        sliderValvula.maxValue = usarVoltios ? 10f : 100f;
        sliderValvula.value = valorVisual;

        textoValvula.text = usarVoltios
            ? $"{valorVisual:0.00}"
            : $"{valorVisual:0.0}";
    }

    public void EnviarConsigna()
    {
        // Envía la consigna a los tanques seleccionados a través de MQTT, mediante el click en el botón de enviar
        // Verifica si el cliente MQTT está conectado antes de enviar
        if (!mqttPublisher.IsConnected)
        {
            Debug.LogWarning("MQTT no está conectado.");
            return;
        }
        string unidadBomba = toggleBombaUnidad.isOn ? "V" : "%";
        string unidadValvula = toggleValvulaUnidad.isOn ? "V" : "%";

        float bomba = valorBombaPorcentaje;
        float valvula = valorValvulaPorcentaje;

        string bombaFormateada = unidadBomba == "V"
            ? bomba.ToString("0.00", CultureInfo.InvariantCulture)
            : bomba.ToString("0.0", CultureInfo.InvariantCulture);

        string valvulaFormateada = unidadValvula == "V"
            ? valvula.ToString("0.00", CultureInfo.InvariantCulture)
            : valvula.ToString("0.0", CultureInfo.InvariantCulture);

        UnityEngine.Debug.Log($"Enviando consigna: Bomba={bombaFormateada}, Válvula={valvulaFormateada}, UnidadBomba={unidadBomba}, UnidadValvula={unidadValvula}");

        string mensaje = $"{{\"Bomba\": {bombaFormateada}, \"UBomba\": \"{unidadBomba}\", \"Valvula\": {valvulaFormateada}, \"UValvula\": \"{unidadValvula}\"}}";

        for (int i = 0; i < tanqueToggles.Count; i++)
        {
            if (tanqueToggles[i].isOn)
            {
                string topic = $"optimizacion/planta_nivel{i + 1}/consigna";
                mqttPublisher.Publicar(topic, mensaje);
            }
        }
    }

    public void EnviarDesdeScript(float bomba, float valvula, string unidadB, string unidadV, int[] tanques)
    {
        // Método para enviar consignas desde otro script por mqtt, permitiendo especificar los valores de 
        // la bomba y la válvula, las unidades y las plantas a las que se enviaran las consignas

        float bombaConvertida = unidadB == "V" ? bomba : bomba * 10f;
        float valvulaConvertida = unidadV == "V" ? valvula : valvula * 10f;

        string bombaFormateada = unidadB == "V"
            ? bombaConvertida.ToString("0.00", CultureInfo.InvariantCulture)
            : bombaConvertida.ToString("0.0", CultureInfo.InvariantCulture);

        string valvulaFormateada = unidadV == "V"
            ? valvulaConvertida.ToString("0.00", CultureInfo.InvariantCulture)
            : valvulaConvertida.ToString("0.0", CultureInfo.InvariantCulture);

        string mensaje = $"{{\"Bomba\": {bombaFormateada}, \"UBomba\": \"{unidadB}\", \"Valvula\": {valvulaFormateada}, \"UValvula\": \"{unidadV}\"}}";

        foreach (int i in tanques)
        {
            string topic = $"optimizacion/planta_nivel{i}/consigna";
            mqttPublisher.Publicar(topic, mensaje);
            Debug.Log($"[SCRIPT] Mensaje enviado a {topic}: {mensaje}");
        }
    }
}
