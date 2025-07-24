// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PantallaCargaController : MonoBehaviour
{
    // Clase para controlar la pantalla de carga
    // Permite mostrar un panel de carga durante un tiempo determinado al iniciar la aplicación,
    // evitando que se envíen consultas al servidor antes de que se haya iniciado
    public GameObject panelCarga;
    public float tiempoEspera = 5;

    void Start()
    {
        // Asegura que el panel esté activo al iniciar
        if (panelCarga != null)
        {
            panelCarga.SetActive(true);
            // Inicia la corrutina para ocultarlo después de n segundos
            StartCoroutine(DesactivarPanelDespuesDeTiempo(tiempoEspera));
        }
    }

    IEnumerator DesactivarPanelDespuesDeTiempo(float segundos)
    {
        // Corrutina que espera un tiempo determinado antes de desactivar el panel de carga
        yield return new WaitForSeconds(segundos);
        panelCarga.SetActive(false);
    }
}
