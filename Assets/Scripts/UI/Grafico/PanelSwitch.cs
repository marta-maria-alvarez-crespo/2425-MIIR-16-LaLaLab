// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelSwitcher : MonoBehaviour
{
    // Clase para cambiar entre dos paneles usando toggles
    // Permite alternar entre dos paneles y ocultar otros elementos de la UI
    public Toggle toggleA;
    public Toggle toggleB;
    public List<GameObject> panelsToHide;
    public GameObject panelToShow;

    private bool ignorarEventos = false;

    private void Start()
    {
        // Método que se ejecuta al iniciar el script
        // Configura los toggles y añade listeners para detectar cambios
        toggleA.onValueChanged.AddListener(OnToggleAChanged);
        toggleB.onValueChanged.AddListener(OnToggleBChanged);

        // Inicializa el estado al toggle B activo
        SetActiveToggle(toggleB);
    }

    private void OnToggleAChanged(bool isOn)
    {
        // Método que se ejecuta al cambiar el estado del toggle A
        // Si el toggle A se activa, muestra el panel asociado y oculta los demás
        // Si se desactiva, activa el toggle B para mantener un estado consistente
        if (ignorarEventos) return;

        if (isOn)
        {
            SetActiveToggle(toggleA);
        }
        else
        {
            ignorarEventos = true;
            toggleA.isOn = true;
            ignorarEventos = false;
        }
    }

    private void OnToggleBChanged(bool isOn)
    {
        // Método que se ejecuta al cambiar el estado del toggle B
        // Si el toggle B se activa, muestra el panel asociado y oculta los demás
        if (ignorarEventos) return;

        if (isOn)
        {
            SetActiveToggle(toggleB);
        }
        else
        {
            ignorarEventos = true;
            toggleB.isOn = true;
            ignorarEventos = false;
        }
    }

    private void SetActiveToggle(Toggle activeToggle)
    {
        // Método para establecer el toggle activo y ocultar/mostrar los paneles
        // Evita que se disparen eventos de cambio mientras se actualizan los toggles
        ignorarEventos = true;

        bool isA = activeToggle == toggleA;

        toggleA.isOn = isA;
        toggleB.isOn = !isA;

        // Oculta o muestra los paneles según el toggle activo
        foreach (GameObject panel in panelsToHide)
        {
            panel.SetActive(!isA); // Si A está activo, se ocultan
        }

        if (panelToShow != null)
        {
            panelToShow.SetActive(isA); // Si A está activo, se muestra
        }

        ignorarEventos = false;
    }
}
