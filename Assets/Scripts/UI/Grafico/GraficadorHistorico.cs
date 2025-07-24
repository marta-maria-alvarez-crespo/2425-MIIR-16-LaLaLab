// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System.Collections.Generic;
using UnityEngine;
using System;

public class GraficadorHistorico : MonoBehaviour
{
    // Clase para graficar datos históricos de plantas
    // Permite graficar los datos históricos de las plantas en un gráfico de líneas

    public UIMultiLineGraph grafico;
    public GameObject graficoTiempoReal;
    public GameObject graficoHistorico;

    [Range(5f, 500f)]
    public float ventanaTiempo = 30f;

    public float zoomVelocidad = 10f;

    [System.Serializable]
    public class PuntoHistorico
    {
        // Clase para representar un punto del dataset histórico de datos
        public string timestamp;
        public float sensor_real;
        public float bomba_real;
        public float valvula_real;
    }

    [System.Serializable]
    public class PlantaHistorica
    {
        // Clase para representar los datos históricos de una planta
        // Contiene el id de la planta y su lista de puntos
        public string plant_id;
        public List<PuntoHistorico> data_points;
    }

    [System.Serializable]
    public class RespuestaHistorica
    {
        // Clase para representar la respuesta del servidor con los datos históricos
        public List<PlantaHistorica> data;
    }

    private Color[] baseColors = new Color[]
    {
        // Lista de colores base para las series del gráfico
        new Color(0.0f, 0.4f, 1.0f),
        new Color(1.0f, 0.5f, 0.0f),
        new Color(0.0f, 0.8f, 0.2f),
        new Color(1.0f, 0.2f, 0.2f),
        new Color(0.6f, 0.3f, 1.0f),
        new Color(0.5f, 0.3f, 0.1f)
    };

    public void GraficarDesdeJSON(string json)
    {
        // Método para graficar los datos históricos a partir de un JSON
        // Deserializa el JSON y llama a GraficarDesdeEstructura para graficar los datos
        var datos = JsonUtility.FromJson<RespuestaHistorica>(json);
        GraficarDesdeEstructura(datos);
    }

    public void GraficarDesdeEstructura(RespuestaHistorica datos)
    {
        // Método para graficar los datos históricos a partir de una estructura de datos
        // Comprueba si el modo de gráficos históricos está activo y si hay datos disponibles
        if (!ModoGraficas.modoHistoricoActivo) return;
        if (datos == null || datos.data == null || datos.data.Count == 0) return;

        grafico.seriesList.Clear();
        grafico.startTime = 0f;
        DateTime timestampBase = DateTime.Parse(datos.data[0].data_points[0].timestamp);

        foreach (var plantaData in datos.data)
        {
            int plantaId = int.Parse(plantaData.plant_id);
            Color baseColor = baseColors[(plantaId - 1) % baseColors.Length];

            int indice = grafico.seriesList.Count;

            grafico.seriesList.Add(new UIMultiLineGraph.DataSeries
            {
                name = $"Planta {plantaId} - Sensor",
                color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f)
            });
            grafico.seriesList.Add(new UIMultiLineGraph.DataSeries
            {
                name = $"Planta {plantaId} - Bomba",
                color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.75f)
            });
            grafico.seriesList.Add(new UIMultiLineGraph.DataSeries
            {
                name = $"Planta {plantaId} - Válvula",
                color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f)
            });

            var puntos = plantaData.data_points;
            float? lastBomba = null;
            float? lastValvula = null;
            float? ultimoSensor = null;
            DateTime? ultimoTimestamp = null;

            for (int i = 0; i < puntos.Count; i++)
            {
                var punto = puntos[i];
                DateTime timestampActual = DateTime.Parse(punto.timestamp);

                if (ultimoTimestamp.HasValue)
                {
                    // Si hay un timestamp anterior, añade puntos intermedios para mantener la continuidad
                    for (DateTime t = ultimoTimestamp.Value.AddSeconds(1); t < timestampActual; t = t.AddSeconds(1))
                    {
                        float tiempoRealtivo = (float)(t - timestampBase).TotalSeconds;
                        if (ultimoSensor.HasValue)
                            grafico.AddPoint(indice, tiempoRealtivo, ultimoSensor.Value, true);
                        if (lastBomba.HasValue)
                            grafico.AddPoint(indice + 1, tiempoRealtivo, lastBomba.Value, true);
                        if (lastValvula.HasValue)
                            grafico.AddPoint(indice + 2, tiempoRealtivo, lastValvula.Value, true);
                    }
                }

                float tiempoActual = (float)(timestampActual - timestampBase).TotalSeconds;
                if (!float.IsNaN(punto.sensor_real)) ultimoSensor = punto.sensor_real;
                if (!float.IsNaN(punto.bomba_real)) lastBomba = punto.bomba_real;
                if (!float.IsNaN(punto.valvula_real)) lastValvula = punto.valvula_real;

                if (ultimoSensor.HasValue)
                    grafico.AddPoint(indice, tiempoActual, ultimoSensor.Value, true);
                if (lastBomba.HasValue)
                    grafico.AddPoint(indice + 1, tiempoActual, lastBomba.Value, true);
                if (lastValvula.HasValue)
                    grafico.AddPoint(indice + 2, tiempoActual, lastValvula.Value, true);

                ultimoTimestamp = timestampActual;
            }
        }

        grafico.yMin = 0f;
        grafico.yMax = 10f;

        grafico.timeWindow = ventanaTiempo;
        grafico.SetVerticesDirty();

    }

    public void LimpiarGraficoHistorico()
    {
        // Método para limpiar el gráfico histórico
        // Elimina todos los puntos de las series y actualiza las etiquetas del gráfico
        if (grafico != null)
        {
            foreach (var serie in grafico.seriesList)
                serie.points.Clear();
            grafico.UpdateLabels();
        }
    }
}
