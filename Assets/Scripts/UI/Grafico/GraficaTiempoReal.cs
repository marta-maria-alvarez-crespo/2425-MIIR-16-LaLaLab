// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineGraph : Graphic
{
    // Clase para graficar datos en tiempo real en un gráfico de líneas
    // Permite añadir puntos y graficarlos
    [Header("Configuración")]
    public float timeWindow = 10f; // segundos visibles en el eje X
    public float yMin = -1f;
    public float yMax = 1f;
    public float lineThickness = 2f;

    private List<Vector2> points = new List<Vector2>();
    private float startTime;

    protected override void OnEnable()
    {
        // Inicializa el tiempo de inicio al activar el componente
        base.OnEnable();
        startTime = Time.time;
    }

    public void AddPoint(float value)
    {
        // Método para añadir un punto al gráfico
        // Calcula el tiempo transcurrido y añade el punto a la lista
        float currentTime = Time.time - startTime;
        float x = currentTime;
        float y = Mathf.Clamp(value, yMin, yMax);
        points.Add(new Vector2(x, y));
        points.RemoveAll(p => p.x < currentTime - timeWindow);

        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Método para dibujar el gráfico en el componente UI
        // Recorre los puntos y dibuja líneas entre ellos
        vh.Clear();

        if (points.Count < 2)
            return;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float xStart = points[0].x;
        float xEnd = points[points.Count - 1].x;
        float xRange = Mathf.Max(timeWindow, xEnd - xStart);

        Vector2 prev = TransformPoint(points[0], xStart, xRange, width, height);

        for (int i = 1; i < points.Count; i++)
        {
            Vector2 curr = TransformPoint(points[i], xStart, xRange, width, height);
            DrawLine(vh, prev, curr, lineThickness, color);
            prev = curr;
        }
    }

    private Vector2 TransformPoint(Vector2 point, float xStart, float xRange, float width, float height)
    {
        // Método para transformar un punto del gráfico a coordenadas de pantalla
        // Calcula la posición del punto en el gráfico según el rango y tamaño del rectángulo
        float x = (point.x - xStart) / xRange * width;
        float y = (point.y - yMin) / (yMax - yMin) * height;
        return new Vector2(x, y);
    }

    private void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color color)
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

        int index = vh.currentVertCount;

        vh.AddUIVertexQuad(quad);
    }
}
