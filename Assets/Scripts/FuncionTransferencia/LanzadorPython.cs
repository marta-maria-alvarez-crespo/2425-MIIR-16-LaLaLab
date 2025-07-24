// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System.Diagnostics;
using System.IO;
using UnityEngine;

public class LanzadorServidoresPython : MonoBehaviour
// Clase para lanzar múltiples servicios de Python (servidores locales) desde la aplicación
{
    void Start()
    {
        // Al iniciar la aplicación, se lanzan los servidores locales necesarios
        LanzarServidor("servidor_local.exe");
        LanzarServidor("consultas_database.exe");
    }

    void LanzarServidor(string nombreExe)
    // Método auxiliar para lanzar un exe desde la carpeta StreamingAssets
    {
        string rutaExe = Path.Combine(Application.streamingAssetsPath, nombreExe);
        if (File.Exists(rutaExe))
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = rutaExe,
                UseShellExecute = true,
                CreateNoWindow = true
            };

            Process.Start(startInfo);
            UnityEngine.Debug.Log($"{nombreExe} lanzado correctamente.");
        }
        else
        {
            UnityEngine.Debug.LogError($"No se encontró {nombreExe} en la ruta esperada: {rutaExe}");
        }
    }
}
