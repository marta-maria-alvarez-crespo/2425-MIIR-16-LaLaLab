// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;

public class ToggleMenuController : MonoBehaviour
{
    // Clase para controlar el estado de los toggles de un menú
    // Permite alternar entre un modo manual y automático, mostrando diferentes menús según el estado de los toggles
    // Utiliza dos toggles: uno para el modo manual y otro para el modo automático
    public Toggle manualToggle;
    public Toggle autoToggle;
    public bool ignorarEventos = false;
    public GameObject manualMenu;
    public GameObject autoMenu;

    void Start()
    {
        // Método que se ejecuta al iniciar el script
        // Configura los toggles para que controlen el estado del menú
        // Inicializa el estado del menú en modo manual
        manualToggle.onValueChanged.AddListener(OnManualToggleChanged);
        autoToggle.onValueChanged.AddListener(OnAutoToggleChanged);
        SetMenuMode(false);
    }

    public void OnManualToggleChanged(bool isOn)
    {
        // Método que se ejecuta al cambiar el estado del toggle manual
        // Actualiza el estado del menú según el estado del toggle manual
        Debug.Log($"[ToggleMenuController] OnManualToggleChanged: {isOn}, ignorarEventos: {ignorarEventos}");
        if (ignorarEventos) return;

        if (isOn)
        {
            SetMenuMode(true);
        }
        else
        {
            ignorarEventos = true;
            manualToggle.isOn = true;
            ignorarEventos = false;
        }
    }

    public void OnAutoToggleChanged(bool isOn)
    {
        // Método que se ejecuta al cambiar el estado del toggle automático
        // Actualiza el estado del menú según el estado del toggle automático
        Debug.Log($"[ToggleMenuController] OnAutoToggleChanged: {isOn}, ignorarEventos: {ignorarEventos}");
        if (ignorarEventos) return;

        if (isOn)
        {
            SetMenuMode(false);
        }
        else
        {
            ignorarEventos = true;
            autoToggle.isOn = true;
            ignorarEventos = false;
        }
    }

    public void SetMenuMode(bool manual)
    {
        // Método que establece el modo del menú según el estado del toggle manual
        // Si el modo es manual, se muestra el menú manual; si es automático, se muestra el menú automático
        Debug.Log($"[ToggleMenuController] SetMenuMode({manual}) → Llamado por: {new System.Diagnostics.StackTrace()}");
        ignorarEventos = true;
        manualToggle.isOn = manual;
        autoToggle.isOn = !manual;
        manualMenu.SetActive(manual);
        autoMenu.SetActive(!manual);
        ignorarEventos = false;
    }
}
