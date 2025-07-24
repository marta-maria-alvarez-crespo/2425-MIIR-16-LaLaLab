// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;

public class UIMultiLineGraphHistorico : UIMultiLineGraph
{
    // Clase para el gráfico de líneas múltiple en modo histórico
    // Hereda de UIMultiLineGraph y se encarga de dibujar las líneas sin etiquetas de tiempo
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float globalMinX = float.MaxValue;
        float globalMaxX = float.MinValue;

        foreach (var series in seriesList)
        {
            foreach (var point in series.points)
            {
                if (float.IsNaN(point.y)) continue;
                globalMinX = Mathf.Min(globalMinX, point.x);
                globalMaxX = Mathf.Max(globalMaxX, point.x);
            }
        }

        float totalDuration = globalMaxX - globalMinX;
        float scaledRange = Mathf.Max(totalDuration / 1000f, 0.001f);

        for (int s = 0; s < seriesList.Count; s++)
        {
            var series = seriesList[s];
            if (!series.visible || series.points.Count < 2) continue;

            Vector2? prev = null;

            for (int i = 0; i < series.points.Count; i++)
            {
                var point = series.points[i];
                if (float.IsNaN(point.y))
                {
                    prev = null;
                    continue;
                }

                float normalizedX = (point.x - globalMinX) / (globalMaxX - globalMinX);
                Vector2 curr = new Vector2(normalizedX * width, (point.y - yMin) / (yMax - yMin) * height);


                if (prev.HasValue)
                {
                    DrawLine(vh, prev.Value, curr, lineThickness, series.color);
                }

                prev = curr;
            }
        }
    }

    public new void UpdateLabels()
    {
        // No se generan etiquetas de tiempo para evitar sobrecarga visual
    }
}
