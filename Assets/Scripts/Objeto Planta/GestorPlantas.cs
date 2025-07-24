// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class GestorPlantas : MonoBehaviour
{
    // Clase para gestionar las plantas de nivel y sus consignas
    // Permite cargar funciones de transferencia, enviar consignas y evaluar el estado de las plantas
    public List<PlantaNivel> plantas;
    public MqttConsignaSender mqttSender;

    public Slider sliderBomba;
    public Slider sliderValvula;
    public Toggle toggleBombaUnidad;
    public Toggle toggleValvulaUnidad;
    public List<SimuladorPlanta> simulador;


    [HideInInspector] public float ultimaBomba;
    [HideInInspector] public float ultimaValvula;
    [HideInInspector] public string ultimaUnidadB;
    [HideInInspector] public string ultimaUnidadV;

    public int[] ultimosTanques;


    void Start()
    {
        // Al iniciar la aplicación, se configura el gestor de plantas
        // Carga las funciones de transferencia y configura los sliders y toggles
        foreach (var planta in plantas)
        {
            var p = planta;
            planta.toggleActiva.isOn = false;
        }
    }

    // [ELIMINADO] EvaluarPlantas()

    public void EnviarManual()
    {
    // Método que se ejecuta al pulsar el botón de enviar manualmente
    // Envía las consignas de la bomba y la válvula a las plantas activas
    
    float bomba = toggleBombaUnidad.isOn ? sliderBomba.value : sliderBomba.value / 10f;
    float valvula = toggleValvulaUnidad.isOn ? sliderValvula.value : sliderValvula.value / 10f;

    string unidadB = toggleBombaUnidad.isOn ? "V" : "%";
    string unidadV = toggleValvulaUnidad.isOn ? "V" : "%";

    List<int> idsOnline = new List<int>();

    for (int i = 0; i < simulador.Count; i++)
    {
        UnityEngine.Debug.Log($"caracola {i}");
        var planta = simulador[i].planta;
        if (!planta.toggleActiva.isOn) continue;

        if (i < simulador.Count && simulador[i] != null)
        {
            simulador[i].ActualizarConsigna(bomba, valvula);
        }

        if (planta.toggleOnline.isOn)
            idsOnline.Add(planta.plantaId);
    }

    if (idsOnline.Count > 0)
    {
        mqttSender.EnviarDesdeScript(bomba, valvula, unidadB, unidadV, idsOnline.ToArray());
    }

    ultimaBomba = bomba;
    ultimaValvula = valvula;
    ultimaUnidadB = unidadB;
    ultimaUnidadV = unidadV;
    ultimosTanques = idsOnline.ToArray();
}


public void EnviarDesdePython(float bomba, float valvula, string unidadB, string unidadV, int[] tanques)
    {
        // Método para enviar consignas desde un script Python
        mqttSender.EnviarDesdeScript(bomba, valvula, unidadB, unidadV, tanques);
    }
}