// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ControladorNivelVisual : MonoBehaviour
{
    // Clase para controlar la visualización del nivel de agua en las plantas
    // Permite seleccionar una planta y visualizar su nivel de agua en los cubos que representan el agua
    public TMP_Dropdown selectorDeModelos;
    public GameObject plantasContenedor;
    public float alturaMaxima = 5f;
    private float valorSensorReal = 0f;


    private List<SimuladorPlanta> simuladores = new List<SimuladorPlanta>();
    private SimuladorPlanta simulador;
    public Transform cuboAgua;
    private float alturaActual = 0f;

    public float valorSensorMaximo = 10f;

    [Range(0f, 1f)]
    public float alturaMinimaRelativaPrincipal = 0.2f;

    [Range(0f, 1f)]
    public float alturaMinimaRelativaReserva = 0.2f;

    public Transform cuboReserva;
    public float alturaMaximaReserva = 2f;


    void Start()
    {
        // Método que se ejecuta al iniciar el script
        // Inicializa la lista de simuladores y el dropdown de selección de modelos
        foreach (Transform child in plantasContenedor.transform)
        {
            SimuladorPlanta sp = child.GetComponent<SimuladorPlanta>();
            if (sp != null)
                simuladores.Add(sp);
        }

        if (selectorDeModelos != null)
        {
            selectorDeModelos.ClearOptions();
            List<string> nombres = new List<string>();

            foreach (SimuladorPlanta simulador in simuladores)
            {
                nombres.Add($"Planta de Nivel {simulador.planta.plantaId}");
            }

            selectorDeModelos.AddOptions(nombres);
            selectorDeModelos.onValueChanged.AddListener(OnPlantaSeleccionada);
            OnPlantaSeleccionada(selectorDeModelos.value);
        }
    }

    void Update()
    {
        // Método que se ejecuta en cada frame
        // Actualiza la visualización del nivel de agua en los cubos según el valor del sensor de la planta seleccionada
        // Si no hay planta seleccionada, no hace nada
        simulador = ObtenerPlantaSeleccionada();

        if (simulador == null || cuboAgua == null || cuboReserva == null)
            return;

        float valorSensor;

        if (simulador.planta.toggleOnline != null && simulador.planta.toggleOnline.isOn)
        {
            valorSensor = valorSensorReal; // Valor real recibido por MQTT
        }
        else
        {
            valorSensor = simulador.GetNivelSensor(); // Valor simulado
        }

        float nivelNormalizado = Mathf.Clamp01(valorSensor / valorSensorMaximo);

        // Tanque principal
        float alturaRelativaPrincipal = Mathf.Lerp(alturaMinimaRelativaPrincipal, 1f, nivelNormalizado);
        float alturaObjetivo = alturaRelativaPrincipal * alturaMaxima;
        alturaActual = Mathf.Lerp(alturaActual, alturaObjetivo, Time.deltaTime * 10f);

        Vector3 escala = cuboAgua.localScale;
        escala.y = alturaActual;
        cuboAgua.localScale = escala;

        Vector3 posicion = cuboAgua.localPosition;
        posicion.y = alturaActual / 2f;
        cuboAgua.localPosition = posicion;

        // Tanque de reserva
        float nivelReserva = 1f - nivelNormalizado;
        float alturaRelativaReserva = Mathf.Lerp(alturaMinimaRelativaReserva, 1f, nivelReserva);
        float alturaReserva = alturaRelativaReserva * alturaMaximaReserva;

        Vector3 escalaReserva = cuboReserva.localScale;
        escalaReserva.y = alturaReserva;
        cuboReserva.localScale = escalaReserva;

        Vector3 posicionReserva = cuboReserva.localPosition;
        posicionReserva.y = alturaReserva / 2f;
        cuboReserva.localPosition = posicionReserva;
    }

    void OnEnable()
    {
        // Método que se ejecuta al habilitar el objeto
        // Se suscribe al evento de recepción de sensor real para actualizar el valor del sensor
        MqttSubscriberFromJson.OnSensorRealRecibido += RecibirSensorReal;
    }

    void OnDisable()
    {
        // Método que se ejecuta al deshabilitar el objeto
        // Se desuscribe del evento de recepción de sensor real para evitar fugas de memoria
        MqttSubscriberFromJson.OnSensorRealRecibido -= RecibirSensorReal;
    }

    void RecibirSensorReal(int plantaId, float valor)
    {
        // Método que se ejecuta al recibir un valor de sensor real por MQTT
        // Actualiza el valor del sensor real si la planta seleccionada coincide con el ID recibido
        if (simulador != null && simulador.planta.plantaId == plantaId)
        {
            valorSensorReal = valor;
        }
    }

    void OnPlantaSeleccionada(int index)
    {
        // Método que se ejecuta al seleccionar una planta en el dropdown
        // Actualiza el simulador seleccionado y muestra un mensaje de depuración
        if (index >= 0 && index < simuladores.Count)
        {
            simulador = simuladores[index];
            Debug.Log($"Planta seleccionada para visualización: {simulador.planta.plantaId}");
        }
    }
    private SimuladorPlanta ObtenerPlantaSeleccionada()
    {
        // Método para obtener el simulador de la planta seleccionada en el dropdown
        // Si el dropdown no está asignado o el índice es inválido, devuelve null
        var index = selectorDeModelos.value;
        if (index >= 0 && index < plantasContenedor.transform.childCount)
        {
            return plantasContenedor.transform.GetChild(index).GetComponent<SimuladorPlanta>();
        }
        return null;
    }

}
