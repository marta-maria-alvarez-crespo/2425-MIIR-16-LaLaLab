// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;

public class ModoGraficasManager : MonoBehaviour
{
    // Clase para gestionar el modo de gráficas
    // Permite cambiar entre modo histórico y tiempo real mediante interacción del usuario
    public GameObject graficoHistorico;
    public GameObject graficoTiempoReal;
    public GraficadorHistorico graficadorHistorico;

    public void ActivarModoHistoricoDesdeJSON(string json)
    {
        // Método para activar el modo histórico desde un JSON
        // Desactiva el gráfico de tiempo real y activa el gráfico histórico
        if (string.IsNullOrEmpty(json)) return;

        ModoGraficas.modoHistoricoActivo = true;

        if (graficoTiempoReal != null)
            graficoTiempoReal.SetActive(false);

        if (graficoHistorico != null)
            graficoHistorico.SetActive(true);

        if (graficadorHistorico != null)
            graficadorHistorico.GraficarDesdeJSON(json);
    }

    public void ActivarModoTiempoRealDesdeToggle()
    {
        // Método para activar el modo de gráficos en tiempo real desde un toggle
        // Desactiva el gráfico histórico y activa el gráfico de tiempo real
        if (!ModoGraficas.modoHistoricoActivo) return;

        ModoGraficas.modoHistoricoActivo = false;

        if (graficadorHistorico != null)
            graficadorHistorico.LimpiarGraficoHistorico();

        if (graficoHistorico != null)
            graficoHistorico.SetActive(false);

        if (graficoTiempoReal != null)
            graficoTiempoReal.SetActive(true);

        Debug.Log("Modo tiempo real activado por interacción del usuario.");
    }
}
