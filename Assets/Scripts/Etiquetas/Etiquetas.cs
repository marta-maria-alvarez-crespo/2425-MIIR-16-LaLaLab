// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 17/07/2025


using UnityEngine;
using TMPro;

public class VisualizadorValoresPlanta : MonoBehaviour
{
    public ControladorNivelVisual controladorNivelVisual;
    public TextMeshProUGUI textoBomba;
    public TextMeshProUGUI textoValvula;
    public TextMeshProUGUI textoSensorSimulado;
    public TextMeshProUGUI textoSensorReal;
    private SimuladorPlanta simulador;
    private float valorSensorReal = 0f;

    void Update()
    {
        // Durante cada frame, se actualizan los valores de la planta seleccionada en la UI

        if (controladorNivelVisual == null) return;
        simulador = ObtenerPlantaSeleccionada();
        if (simulador != null)
        {
            float bomba = simulador.GetConsignaBomba(); // Consigna de la bomba
            float valvula = simulador.GetConsignaValvula(); // Consigna de la válvula
            float sensorSimulado = simulador.GetNivelSensor(); // Nivel simulado del sensor

            textoBomba.text = $"{bomba:0.00}V"; // Valor de la bomba
            textoValvula.text = $"{valvula:0.00}V"; // Valor de la válvula
            textoSensorSimulado.text = $"{sensorSimulado:0.00}V"; // Valor simulado del sensor
            textoSensorReal.text = $"{valorSensorReal:0.00}V"; // Valor real recibido por MQTT
        }
    }

    void OnEnable()
    {
        MqttSubscriberFromJson.OnSensorRealRecibido += RecibirSensorReal;
    }

    void OnDisable()
    {
        MqttSubscriberFromJson.OnSensorRealRecibido -= RecibirSensorReal;
    }

    private void RecibirSensorReal(int plantaId, float valor)
    {
        // Permite actualizar el valor del sensor real en la UI cuando se recibe un mensaje MQTT
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            var plantaActual = ObtenerPlantaSeleccionada();
            if (plantaActual != null && plantaActual.planta.plantaId == plantaId)
            {
                valorSensorReal = valor;
            }
        });
    }

    private SimuladorPlanta ObtenerPlantaSeleccionada()
    {
        // Obtener le indice de planta seleccionada en el controlador de nivel visual
        var index = controladorNivelVisual.selectorDeModelos.value;
        if (index >= 0 && index < controladorNivelVisual.plantasContenedor.transform.childCount)
        {
            return controladorNivelVisual.plantasContenedor.transform.GetChild(index).GetComponent<SimuladorPlanta>();
        }
        return null;
    }
    public void ActualizarSensorReal(float valor)
    {
        // Método para actualizar el valor del sensor real directamente desde el graficador asociado
        valorSensorReal = valor;
    }
}
