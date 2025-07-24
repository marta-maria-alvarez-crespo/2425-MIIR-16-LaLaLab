// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025


using UnityEngine;
using UnityEngine.UI;

public class AsignadorBoton : MonoBehaviour
{
    // Objeto que permite la asignacion de standaloneFileBrowser al evento onlcik() del boton de la UI
    public Button miBoton;
    public CargadorFuncionTransferencia cargador;
    private SimuladorPlanta simuladorPlanta;

    void Start()
    {
        miBoton.onClick.AddListener(cargador.CargarArchivo);
    }
}
