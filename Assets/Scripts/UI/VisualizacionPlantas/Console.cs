// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using TMPro;

public class InGameConsoleUI : MonoBehaviour
{
    // Clase para mostrar la consola de depuración en la UI
    // Permite visualizar mensajes de log en un TextMeshProUGUI
    // Se suscribe a los eventos de log de la aplicación para capturar mensajes
    [SerializeField] private TextMeshProUGUI consoleText;

    void Awake()
    {
        // Método que se ejecuta al iniciar el script
        // Comprueba si el TextMeshProUGUI está asignado y se suscribe a los eventos de log
        if (consoleText == null)
        {
            return;
        }

        Application.logMessageReceived += HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Método que se ejecuta al recibir un mensaje de log
        // Actualiza el texto del TextMeshProUGUI con el mensaje de log
        if (consoleText != null)
        {
            consoleText.text = logString;
        }
    }

    void OnDestroy()
    {
        // Método que se ejecuta al destruir el objeto
        // Se desuscribe de los eventos de log para evitar fugas de memoria
        Application.logMessageReceived -= HandleLog;
    }
}
