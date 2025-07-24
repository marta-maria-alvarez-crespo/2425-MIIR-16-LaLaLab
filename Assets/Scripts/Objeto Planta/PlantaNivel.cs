// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025


using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class PlantaNivel
{
    // Clase que representa una planta de nivel en la UI
    public Toggle toggleActiva;
    public Toggle toggleOnline;
    public Button botonCargarFuncion;
    public int plantaId;

    public GameObject objetoPlanta;
}
