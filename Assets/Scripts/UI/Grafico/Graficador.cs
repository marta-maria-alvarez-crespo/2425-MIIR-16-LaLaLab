// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GraficadorPlantasConMQTT : MonoBehaviour
{
    // Clase para graficar las plantas con MQTT
    // Permite graficar las consignas de las plantas en tiempo real
    [System.Serializable]
    public class PlantaGraficable
    {
        public SimuladorPlanta planta;
        public Toggle togglePlanta;
        public Toggle toggleBomba;
        public Toggle toggleValvula;
        public Toggle toggleSensor;
    }

    public PlantaGraficable[] plantasGraficables;
    public ModoGraficasManager modoGraficasManager;

    public UIMultiLineGraph grafico;
    public float intervaloActualizacion = 1.0f;
    public GameObject graficoHistorico;
    public GameObject graficoTiempoReal;


    private float[] consignasMQTT = new float[6];
    private bool tiempoIniciado = false;

    void Start()
    {
        // Al iniciar la aplicación, se configura el graficador de plantas
        // Asigna los colores a las series del gráfico y comienza la actualización periódica
        AsignarColores(grafico);
        Application.runInBackground = true;
        StartCoroutine(ActualizarGraficoCoroutine());

        foreach (var pg in plantasGraficables)
        {
            if (pg.togglePlanta != null)
                pg.togglePlanta.onValueChanged.AddListener(OnToggleChanged);

            if (pg.toggleSensor != null)
                pg.toggleSensor.onValueChanged.AddListener(OnToggleChanged);

            if (pg.toggleBomba != null)
                pg.toggleBomba.onValueChanged.AddListener(OnToggleChanged);

            if (pg.toggleValvula != null)
                pg.toggleValvula.onValueChanged.AddListener(OnToggleChanged);
        }

    }


    // [ELIMINADO ]ActualizarConsignaMQTT(int plantaIndex, float valor)

    IEnumerator ActualizarGraficoCoroutine()
    {
        // Coroutine para actualizar el gráfico periódicamente
        // Permite actualizar el gráfico en intervalos regulares definidos por intervaloActualizacion
        while (true)
        {
            ActualizarGrafico();
            yield return new WaitForSeconds(intervaloActualizacion);
        }
    }



    public void OnToggleChanged(bool _)
    {
        // Método que se ejecuta al cambiar el estado de un toggle
        // Si el toggle está activo, activa el modo de gráficos en tiempo real
        modoGraficasManager.ActivarModoTiempoRealDesdeToggle();
    }


    void ActualizarGrafico()
    {
        // Método para actualizar el gráfico con los datos de las plantas
        // Recorre las plantas graficables y actualiza los puntos del gráfico
        if (grafico == null || plantasGraficables == null) return;

        if (!tiempoIniciado && HayAlgunaSerieActiva())
        {
            grafico.startTime = Time.realtimeSinceStartup;
            tiempoIniciado = true;
        }

        if (!tiempoIniciado) return;

        float currentTime = Time.realtimeSinceStartup - grafico.startTime;

        for (int i = 0; i < plantasGraficables.Length; i++)
        {
            var pg = plantasGraficables[i];
            if (pg.planta == null || pg.togglePlanta == null) continue;

            int baseIndex = i * 4;
            bool plantaActiva = pg.togglePlanta.isOn;
            bool sensorActivo = plantaActiva && pg.toggleSensor != null && pg.toggleSensor.isOn;
            bool bombaActivo = plantaActiva && pg.toggleBomba != null && pg.toggleBomba.isOn;
            bool valvulaActivo = plantaActiva && pg.toggleValvula != null && pg.toggleValvula.isOn;

            // Limitar a 10V los valores del sensor real y simulado
            float sensorReal = sensorActivo ? Mathf.Min(consignasMQTT[i], 10f) : float.NaN;
            float sensorSimulado = sensorActivo ? Mathf.Min(pg.planta.GetNivelSensor(), 10f) : float.NaN;

            grafico.AddPoint(baseIndex,     currentTime, sensorReal, sensorActivo);     // Sensor Real
            grafico.AddPoint(baseIndex + 1, currentTime, sensorSimulado, sensorActivo); // Sensor Simulado
            grafico.AddPoint(baseIndex + 2, currentTime, bombaActivo ? pg.planta.GetConsignaBomba() : float.NaN, bombaActivo); // Bomba
            grafico.AddPoint(baseIndex + 3, currentTime, valvulaActivo ? pg.planta.GetConsignaValvula() : float.NaN, valvulaActivo); // Válvula
        }
    }



    void AsignarColores(UIMultiLineGraph grafico)
    {
        // Método para asignar colores a las series del gráfico
        // Define una lista de colores base y los asigna a las series del gráfico
        Color[] baseColors = new Color[]
        {
            new Color(0.0f, 0.4f, 1.0f), // azukl
            new Color(1.0f, 0.5f, 0.0f), // naranja
            new Color(0.0f, 0.8f, 0.2f), // verde
            new Color(1.0f, 0.2f, 0.2f), // rojito
            new Color(0.6f, 0.3f, 1.0f), // moradito
            new Color(0.5f, 0.3f, 0.1f)  // marron
        };

        grafico.seriesList.Clear();

        for (int i = 0; i < baseColors.Length; i++)
        {
            Color baseColor = baseColors[i];

            grafico.seriesList.Add(new UIMultiLineGraph.DataSeries { name = $"Planta {i + 1} - Sensor Real", color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f) });
            grafico.seriesList.Add(new UIMultiLineGraph.DataSeries { name = $"Planta {i + 1} - Sensor Simulado", color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.75f) });
            grafico.seriesList.Add(new UIMultiLineGraph.DataSeries { name = $"Planta {i + 1} - Bomba", color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f) });
            grafico.seriesList.Add(new UIMultiLineGraph.DataSeries { name = $"Planta {i + 1} - Válvula", color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.3f) });
        }
    }

    bool HayAlgunaSerieActiva()
    {
        // Método para comprobar si hay alguna serie activa en el gráfico
        // Recorre las plantas graficables y verifica si al menos una serie está activa
        foreach (var pg in plantasGraficables)
        {
            if (pg.togglePlanta != null && pg.togglePlanta.isOn)
            {
                if ((pg.toggleSensor != null && pg.toggleSensor.isOn) ||
                    (pg.toggleBomba != null && pg.toggleBomba.isOn) ||
                    (pg.toggleValvula != null && pg.toggleValvula.isOn))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void ActualizarSensorRealMQTT(int plantaIndex, float valor, string unidad)
    {
        // Método para actualizar el sensor real de una planta específica
        // Permite actualizar el valor del sensor real de una planta en función del índice y las unidades
        if (plantaIndex >= 0 && plantaIndex < consignasMQTT.Length)
        {
            if (unidad == "%")
                valor /= 10f;
            consignasMQTT[plantaIndex] = valor;
        }
    } 
}
