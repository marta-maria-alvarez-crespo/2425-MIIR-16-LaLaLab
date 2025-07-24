// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonSelector : MonoBehaviour
{
    public Toggle allToggle;
    public List<Toggle> otherToggles;

    void Start()
    {
        // Método que se ejecuta al iniciar el script
        // Configura el toggle "All" para que controle todos los demás toggles
        allToggle.onValueChanged.AddListener(OnAllToggleChanged);
        allToggle.isOn = false; // Inicialmente desactivado
    }

    void OnAllToggleChanged(bool isOn)
    {
        // Método que se ejecuta al cambiar el estado del toggle "All"
        // Actualiza el estado de todos los otros toggles según el estado del toggle "All
        foreach (var toggle in otherToggles)
        {
            toggle.isOn = isOn;

            // También actualiza el SimpleToggle si está presente
            SimpleToggle visual = toggle.GetComponent<SimpleToggle>();
            if (visual != null)
            {
                visual.SetState(isOn);
            }
        }
    }
}
