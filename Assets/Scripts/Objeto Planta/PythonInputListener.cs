// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class PythonScriptManager : MonoBehaviour
{
    // Clase para gestionar la ejecución de scripts Python desde Unity
    // Permite cargar un script, ejecutarlo y detenerlo, así como recibir consignas para las plantas
    // y actualizar la UI en consecuencia
    public GestorPlantas gestorPlantas;
    public Toggle toggleEjecutarScript;
    public Toggle toggleModoAutomatico;
    public List<SimuladorPlanta> simuladores;

    public GameObject panelOpciones;
    public GameObject panelModoScript;
    public ToggleMenuController toggleMenuController;

    public Button botonCargarScript;

    private Process procesoPython;
    private Thread hiloEscucha;
    private string rutaScript;

    private SliderToggle sliderToggleEjecutarScript;

    void Start()
    {
        // Al iniciar, se configura el toggle para ejecutar el script y el botón para cargar el script Python
        toggleEjecutarScript.isOn = false;
        toggleEjecutarScript.onValueChanged.AddListener(OnToggleScriptChanged);

        botonCargarScript.onClick.RemoveAllListeners();
        botonCargarScript.onClick.AddListener(CargarScriptPython);

        sliderToggleEjecutarScript = toggleEjecutarScript.GetComponent<SliderToggle>();
    }

    void CargarScriptPython()
    {
        // Método para cargar un script Python desde el sistema de archivos
        var paths = SFB.StandaloneFileBrowser.OpenFilePanel("Selecciona script Python", "", "py", false);
        if (paths.Length > 0)
        {
            rutaScript = paths[0];
            UnityEngine.Debug.Log($"Script Python cargado: {rutaScript}");
        }
    }

    void OnToggleScriptChanged(bool activo)
    {
        // Método que se ejecuta al cambiar el estado del toggle para ejecutar el script
        // Si el toggle está activo, se inicia el script Python; si no, se detiene
        if (activo)
        {
            if (string.IsNullOrEmpty(rutaScript))
            {
                UnityEngine.Debug.LogWarning("No se ha cargado ningún script.");
                toggleEjecutarScript.isOn = false;
                return;
            }

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                panelOpciones?.SetActive(false);
                panelModoScript?.SetActive(true);
            });

            hiloEscucha = new Thread(EjecutarScriptPython);
            hiloEscucha.Start();
        }
        else
        {
            DetenerScriptPython();
        }
    }

    void DetenerScriptPython()
    {
        // Método para detener la ejecución del script Python
        // Limpia el proceso Python y el hilo de escucha, y actualiza la UI
        try
        {
            if (procesoPython != null && !procesoPython.HasExited)
            {
                procesoPython.Kill();
                procesoPython.Dispose();
            }
            if (hiloEscucha != null && hiloEscucha.IsAlive)
            {
                hiloEscucha.Abort();
            }

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                panelOpciones?.SetActive(true);
                panelModoScript?.SetActive(false);

                bool esManual = toggleMenuController.manualToggle.isOn;
                toggleMenuController.SetMenuMode(esManual);

                foreach (var simulador in gestorPlantas.simulador)
                {
                    var simpleToggle = simulador.planta.toggleActiva.GetComponent<SimpleToggle>();
                    if (simpleToggle != null)
                    {
                        simpleToggle.SetState(false);
                    }
                    else
                    {
                        simulador.planta.toggleActiva.isOn = false;
                    }

                    simulador.planta.toggleActiva.interactable = true;
                }

                if (sliderToggleEjecutarScript != null)
                {
                    sliderToggleEjecutarScript.SetState(false);
                }
                else
                {
                    toggleEjecutarScript.isOn = false;
                }
            });

            UnityEngine.Debug.Log("Script Python detenido.");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error al detener el script: {ex.Message}");
        }
    }

    void EjecutarScriptPython()
    {
        // Método para ejecutar el script Python en un proceso separado
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"\"{rutaScript}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        procesoPython = new Process { StartInfo = psi };
        procesoPython.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data) && e.Data.StartsWith("SEND") && toggleEjecutarScript.isOn)
            {
                ProcesarLinea(e.Data);
            }
        };

        procesoPython.Start();
        procesoPython.BeginOutputReadLine();
        procesoPython.WaitForExit();
    }

    void ProcesarLinea(string linea)
    {
        // Método para procesar las líneas recibidas del script Python
        // Extrae los valores de la bomba, la válvula, las unidades y los ids de las plantas a simular
        UnityEngine.Debug.Log($"ProcesarLinea recibida: {linea}");

        string[] partes = linea.Split(' ');
        if (partes.Length < 6) return;

        try
        {
            float bomba = float.Parse(partes[1], CultureInfo.InvariantCulture);
            float valvula = float.Parse(partes[2], CultureInfo.InvariantCulture);

            string unidadB = partes[3];
            string unidadV = partes[4];
            int[] tanques = Array.ConvertAll(partes[5].Split(','), int.Parse);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                gestorPlantas.sliderBomba.value = bomba;
                gestorPlantas.sliderValvula.value = valvula;
                gestorPlantas.toggleBombaUnidad.isOn = unidadB == "V";
                gestorPlantas.toggleValvulaUnidad.isOn = unidadV == "V";

                List<int> idsOnline = new List<int>();

                for (int i = 0; i < gestorPlantas.simulador.Count; i++)
                {
                    var planta = gestorPlantas.simulador[i].planta;
                    bool activa = tanques.Contains(planta.plantaId);

                    var simpleToggle = planta.toggleActiva.GetComponent<SimpleToggle>();
                    if (simpleToggle != null)
                    {
                        simpleToggle.SetState(activa);
                    }
                    else
                    {
                        planta.toggleActiva.isOn = activa;
                    }

                    planta.toggleActiva.interactable = false;

                    if (activa && planta.toggleOnline.isOn)
                    {
                        idsOnline.Add(planta.plantaId);
                    }

                    if (activa && i < gestorPlantas.simulador.Count && gestorPlantas.simulador[i] != null)
                    {
                        simuladores[i].ActualizarConsigna(bomba, valvula);
                    }
                }

                gestorPlantas.EnviarDesdePython(bomba, valvula, unidadB, unidadV, idsOnline.ToArray());
            });
        }
        catch (FormatException ex)
        {
            UnityEngine.Debug.LogError($"Error de formato en la línea: {linea}. Detalles: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            UnityEngine.Debug.LogError($"Error al convertir los IDs de las plantas: {ex.Message}");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error al procesar línea: {ex.Message}");
        }
    }

    void OnApplicationQuit()
    {
        // Método que se ejecuta al cerrar la aplicación
        // Detiene el script Python si está en ejecución
        DetenerScriptPython();
    }
}
