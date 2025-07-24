// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SliderSelector : MonoBehaviour
{
    // Clase para controlar un conjunto de toggles asociados a sliders
    // Permite seleccionar un toggle "All" que controla el estado de todos los demás toggles
    public Toggle allToggle;
    public List<SliderToggle> sliderToggles;

    private bool isUpdatingFromCode = false;

    void Start()
    {
        // Método que se ejecuta al iniciar el script
        // Configura el toggle "All" para que controle todos los demás toggles
        foreach (var slider in sliderToggles)
        {
            if (slider.toggle != null)
            {
                slider.toggle.onValueChanged.AddListener(delegate { StartCoroutine(DelayedUpdateAllToggleState()); });
            }
        }

        allToggle.onValueChanged.AddListener(OnAllToggleChanged);
        allToggle.isOn = false;

        StartCoroutine(DelayedUpdateAllToggleState());
    }

    void OnAllToggleChanged(bool isOn)
    {
        // Método que se ejecuta al cambiar el estado del toggle "All"
        // Actualiza el estado de todos los otros toggles según el estado del toggle "All"
        if (isUpdatingFromCode) return;

        foreach (var slider in sliderToggles)
        {
            slider.SetState(isOn);
        }
    }

    IEnumerator DelayedUpdateAllToggleState()
    {
        // Corrutina que actualiza el estado del toggle "All" después de un frame
        // necesario para evitar bucles infinitos al cambiar el estado de los toggles
        yield return null; // Esperar un frame para que no se bugee

        bool allOn = sliderToggles.TrueForAll(s => s.IsOn());

        if (allToggle.isOn != allOn)
        {
            isUpdatingFromCode = true;
            allToggle.isOn = allOn;
            isUpdatingFromCode = false;
        }
    }
}
