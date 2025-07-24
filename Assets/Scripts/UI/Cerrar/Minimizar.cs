// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using System.Runtime.InteropServices;

public class BotonMinimizar : MonoBehaviour
{
    // Clase para minimizar la ventana de la aplicación
    // Permite minimizar la ventana actual
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(System.IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    private const int SW_MINIMIZE = 6;

    public void MinimizarVentana()
    {
        // Método que se ejecuta al pulsar el botón de minimizar la ventana
        // Minimiza la ventana actual de la aplicación
        System.IntPtr hWnd = GetActiveWindow();
        ShowWindow(hWnd, SW_MINIMIZE);
        Debug.Log("Ventana minimizada.");
    }
}
