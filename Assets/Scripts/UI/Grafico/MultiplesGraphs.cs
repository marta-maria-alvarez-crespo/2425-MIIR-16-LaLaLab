// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMultiLineGraph : Graphic
{
    // Clase para graficar múltiples series de datos en un gráfico de líneas
    // Permite añadir puntos a diferentes series y visualizarlas en un gráfico
    [System.Serializable]
    public class DataSeries
    {
        public string name;
        public Color color;
        public List<Vector2> points = new List<Vector2>();
        public bool visible = true;

        private const int maxPoints = 30;

        public void AddPoint(float x, float y)
        {
            points.Add(new Vector2(x, y));
            if (points.Count > maxPoints)
            {
                points.RemoveAt(0);
            }
        }
    }

    [Header("Configuración del gráfico")]
    public float timeWindow = 10f;
    public float yMin = 0f;
    public float yMax = 10f;
    public float lineThickness = 2f;

    [Header("Configuración de líneas verticales")]
    public Color verticalLineColor = new Color(1f, 1f, 1f, 0.2f);
    public float verticalLineThickness = 1f;
    [Range(0f, 1f)] public float verticalLineHeightRatio = 1f;

    [Header("Etiquetas de tiempo")]
    public GameObject timeLabelPrefab;
    public Vector2 timeLabelOffset = new Vector2(0f, -20f);

    public List<DataSeries> seriesList = new List<DataSeries>();
    
    protected List<GameObject> activeLabels = new List<GameObject>();
    [HideInInspector] public float startTime = 0f;

    public void AddPoint(int seriesIndex, float x, float y, bool visible)
    {
        // Método para añadir un punto a una serie específica del gráfico
        // Verifica que el índice de la serie sea válido y añade el punto
        // Si la serie no es visible, no añade el punto
        if (seriesIndex < 0 || seriesIndex >= seriesList.Count) return;

        var series = seriesList[seriesIndex];
        series.visible = visible;
        series.AddPoint(x, y);

        SetVerticesDirty();
        UpdateLabels();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Método para dibujar el gráfico en el componente UI
        // Recorre las series y dibuja líneas entre los puntos de cada serie
        vh.Clear();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        for (int s = 0; s < seriesList.Count; s++)
        {
            var series = seriesList[s];
            if (series.points.Count < 2) continue;

            float xStart = series.points[0].x;
            float xEnd = series.points[series.points.Count - 1].x;
            float xRange = Mathf.Max(timeWindow, xEnd - xStart);

            Vector2? prev = null;

            for (int i = 0; i < series.points.Count; i++)
            {
                var point = series.points[i];
                if (float.IsNaN(point.y))
                {
                    prev = null;
                    continue;
                }

                Vector2 curr = TransformPoint(point, xStart, xRange, width, height);

                if (prev.HasValue)
                {
                    DrawLine(vh, prev.Value, curr, lineThickness, series.color);
                }

                prev = curr;
            }
        }

        List<Vector2> referencia = ObtenerReferencia();
        if (referencia != null && referencia.Count > 0)
        {
            // Dibuja líneas verticales en los puntos de referencia

            float xStart = referencia[0].x;
            float xEnd = referencia[referencia.Count - 1].x;
            float xRange = Mathf.Max(timeWindow, xEnd - xStart);

            foreach (var point in referencia)
            {
                if (float.IsNaN(point.y)) continue;

                Vector2 transformed = TransformPoint(point, xStart, xRange, width, height);
                float lineHeight = height * verticalLineHeightRatio;
                Vector2 bottom = new Vector2(transformed.x, 0);
                Vector2 top = new Vector2(transformed.x, lineHeight);
                DrawLine(vh, bottom, top, verticalLineThickness, verticalLineColor);
            }
        }
    }

    protected Vector2 TransformPoint(Vector2 point, float xStart, float xRange, float width, float height)
    {
        // Método para transformar un punto del gráfico a coordenadas de pantalla
        // Calcula la posición del punto en el gráfico según el rango y tamaño del rectáng
        float x = (point.x - xStart) / xRange * width;
        float y = (point.y - yMin) / (yMax - yMin) * height;
        return new Vector2(x, y);
    }

    protected void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color color)
    {
        // Método para dibujar una línea entre dos puntos en el gráfico
        // Crea un cuadrado que representa la línea y lo añade al VertexHelper
        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * thickness * 0.5f;

        UIVertex[] quad = new UIVertex[4];

        quad[0].position = start - normal;
        quad[1].position = start + normal;
        quad[2].position = end + normal;
        quad[3].position = end - normal;

        for (int i = 0; i < 4; i++)
        {
            quad[i].color = color;
        }

        vh.AddUIVertexQuad(quad);
    }

    public void UpdateLabels()
    {
        // Método para actualizar las etiquetas de tiempo en el gráfico
        // Si el prefab de etiqueta es nulo o no se ha iniciado el tiempo, no hace nada
        if (timeLabelPrefab == null || startTime == 0f) return;

        foreach (var label in activeLabels)
            Destroy(label);
        activeLabels.Clear();

        List<Vector2> referencia = ObtenerReferencia();
        if (referencia == null || referencia.Count == 0) return;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float xStart = referencia[0].x;
        float xEnd = referencia[referencia.Count - 1].x;
        float xRange = xEnd - xStart;

        if (xRange <= 0f) return;

        int numLabels = 5;
        for (int i = 0; i <= numLabels; i++)
        {
            float t = xStart + (xRange * i / numLabels);
            Vector2 pos = TransformPoint(new Vector2(t, yMin), xStart, xRange, width, height);

            GameObject label = Instantiate(timeLabelPrefab, transform);
            var text = label.GetComponent<TextMeshProUGUI>();
            if (text != null)
                text.text = FormatearTiempo(t);

            RectTransform rt = label.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(pos.x + timeLabelOffset.x, timeLabelOffset.y);

            activeLabels.Add(label);
        }
    }

    protected string FormatearTiempo(float segundos)
    {
        // Método para formatear el tiempo en un formato legible
        // Convierte los segundos a un formato de horas, minutos y segundos
        int totalSegundos = Mathf.FloorToInt(segundos);
        int horas = totalSegundos / 3600;
        int minutos = (totalSegundos % 3600) / 60;
        int seg = totalSegundos % 60;

        if (horas > 0)
            return $"{horas:D2}:{minutos:D2}:{seg:D2}";
        else
            return $"{minutos:D2}:{seg:D2}";
    }

    private List<Vector2> ObtenerReferencia()
    {
        // Método para obtener los puntos de referencia de todas las series
        // Combina los puntos de todas las series visibles en una lista
        List<Vector2> puntosCombinados = new List<Vector2>();
        float tiempoMinimo = Time.realtimeSinceStartup - startTime - timeWindow;

        foreach (var serie in seriesList)
        {
            foreach (var punto in serie.points)
            {
                if (!float.IsNaN(punto.y) && punto.x >= tiempoMinimo)
                {
                    puntosCombinados.Add(punto);
                }
            }
        }

        puntosCombinados.Sort((a, b) => a.x.CompareTo(b.x));
        return puntosCombinados.Count > 0 ? puntosCombinados : null;
    }
}
