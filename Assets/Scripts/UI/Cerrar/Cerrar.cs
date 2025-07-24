// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;

public class BotonCerrar : MonoBehaviour
{
    public void CerrarAplicacion()
    {
        // Método que se ejecuta al pulsar el botón de cerrar la aplicación
        // Cierra la aplicación
        Application.Quit();
        Debug.Log("Aplicación cerrada.");
    }
}
