// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using System.Diagnostics;
using System.Linq;

public class ExitHandlerRuntime : MonoBehaviour
{
    // Clase para gestionar el cierre de la aplicación y enviar consignas de parada a las plantas
    // Se asegura de que todos los procesos lanzados desde la carpeta StreamingAssets se cierren
    private void OnApplicationQuit()
    {
        // Método que se ejecuta al cerrar la aplicación
        // Cierra todos los procesos que se hayan lanzado desde la carpeta StreamingAssets
        CerrarProcesosDesdeStreamingAssets();
        EnviarCeroATodasLasPlantas();
    }

    private void CerrarProcesosDesdeStreamingAssets()
    {
        // Método para cerrar todos los procesos que se hayan lanzado desde la carpeta StreamingAssets
        // Recorre todos los procesos en ejecución y los cierra
        string streamingPath = Application.streamingAssetsPath.Replace("/", "\\");

        foreach (var proceso in Process.GetProcesses())
        {
            try
            {
                string ruta = proceso.MainModule?.FileName;
                if (!string.IsNullOrEmpty(ruta) && ruta.StartsWith(streamingPath, System.StringComparison.OrdinalIgnoreCase))
                {
                    proceso.Kill();
                    UnityEngine.Debug.Log($"Proceso terminado: {proceso.ProcessName}");
                }
            }
            catch { }
        }
    }

    private void EnviarCeroATodasLasPlantas()
    {
        // Método para enviar una consigna de parada a todas las plantas
        // Busca el componente MqttConsignaSender en la escena y envía una consigna de 0 a todas las plantas
        // por seguridad
        MqttConsignaSender sender = FindFirstObjectByType<MqttConsignaSender>();
        if (sender == null)
        {
            return;
        }

        SimuladorPlanta[] simuladores = FindObjectsByType<SimuladorPlanta>(FindObjectsSortMode.None);
        int[] ids = simuladores
            .Select(s => int.TryParse(s.planta.plantaId.ToString(), out int id) ? id : -1)
            .Where(id => id >= 0)
            .ToArray();

        sender.EnviarDesdeScript(0f, 0f, "V", "V", ids);
        UnityEngine.Debug.Log("Consigna 0 enviada");
    }
}
