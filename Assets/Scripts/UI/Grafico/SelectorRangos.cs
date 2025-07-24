// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

public class SelectorRangos : MonoBehaviour
{
    // Clase para seleccionar rangos de fechas y horas en la UI
    // Permite enviar consultas al servidor y descargar datos en formato CSV
    public TMP_InputField startDateInput;
    public TMP_InputField startTimeInput;
    public TMP_InputField endDateInput;
    public TMP_InputField endTimeInput;
    public Button submitButton;
    public Button downloadCSVButton;
    public Button downloadAllButton;
    public ModoGraficasManager modoGraficasManager;

    public GameObject realTimeGraph;
    public GameObject historicalGraph;

    private string lastStart;
    private string lastEnd;

    void Start()
    {
        // Método que se ejecuta al iniciar el script
        // Configura los botones y añade listeners para detectar pulsaciones
        downloadCSVButton.onClick.AddListener(OnDownloadCSV);
        downloadAllButton.onClick.AddListener(OnDownloadAll);
    }

    public void OnSubmit()
    {
        // Método que se ejecuta al pulsar el botón de enviar consulta
        // Valida las fechas y horas introducidas, y envía una consulta al servidor
        // Si las fechas son válidas, inicia una coroutine para solicitar los datos
        try
        {
            DateTime start = DateTime.Parse($"{startDateInput.text} {startTimeInput.text}");
            DateTime end = DateTime.Parse($"{endDateInput.text} {endTimeInput.text}");

            if (start >= end)
            {
                Debug.LogError("La fecha de inicio debe ser anterior a la de fin.");
                return;
            }

            lastStart = start.ToString("yyyy-MM-dd HH:mm:ss");
            lastEnd = end.ToString("yyyy-MM-dd HH:mm:ss");
            Debug.Log($"Consulta enviada: {lastStart} → {lastEnd}");

            StartCoroutine(RequestDataFromServer(lastStart, lastEnd));
        }
        catch (FormatException)
        {
            Debug.LogError("Formato de fecha u hora inválido.");
        }
    }

    IEnumerator RequestDataFromServer(string start, string end)
    {
        // Método para solicitar datos al servidor en un rango de fechas
        // Envía una petición POST al servidor y espera la respuesta
        // Si la respuesta es exitosa, activa el modo histórico con los datos recibidos
        // si no, muestra un error en la consola
        string url = "http://localhost:8001/api/consulta";

        WWWForm form = new WWWForm();
        form.AddField("start", start);
        form.AddField("end", end);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al consultar: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log(json);

            realTimeGraph.SetActive(false);
            historicalGraph.SetActive(true);

            if (modoGraficasManager != null)
            {
                modoGraficasManager.ActivarModoHistoricoDesdeJSON(json);
            }
            else
            {
                Debug.LogWarning("No se encontró modoGraficasManager.");
            }
        }
    }

    void OnDownloadCSV()
    {
        // Método que se ejecuta al pulsar el botón de descargar CSV
        // Si las fechas de inicio y fin son válidas, descarga los datos en formato CSV
        // Si no, muestra un mensaje de advertencia
        if (string.IsNullOrEmpty(lastStart) || string.IsNullOrEmpty(lastEnd))
        {
            Debug.LogWarning("Primero realiza una consulta.");
            return;
        }

        string url = $"http://localhost:8001/api/descargar_csv?start={UnityWebRequest.EscapeURL(lastStart)}&end={UnityWebRequest.EscapeURL(lastEnd)}";
        StartCoroutine(DownloadAndSaveCSV(url, "datos_rango.csv"));
    }

    void OnDownloadAll()
    {
        // Método que se ejecuta al pulsar el botón de descargar todos los datos
        // Descarga todos los datos del servidor en formato CSV
        // Guarda el archivo en la carpeta de descargas del usuario
        string url = "http://localhost:8001/api/descargar_todo";
        StartCoroutine(DownloadAndSaveCSV(url, "dataset_completo.csv"));
    }

    IEnumerator DownloadAndSaveCSV(string url, string filename)
    {
        // Método para descargar un archivo CSV desde el servidor
        // Guarda el archivo en la carpeta de descargas del usuario
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al descargar CSV: " + www.error);
        }
        else
        {
            string downloadsPath = GetDownloadsPath();
            string fullPath = Path.Combine(downloadsPath, filename);

            try
            {
                File.WriteAllBytes(fullPath, www.downloadHandler.data);
                Debug.Log($"Archivo guardado en: {fullPath}");

#if UNITY_EDITOR
                UnityEditor.EditorUtility.RevealInFinder(fullPath);
#elif UNITY_STANDALONE_WIN
                System.Diagnostics.Process.Start("explorer.exe", "/select," + fullPath.Replace("/", "\\"));
#endif
            }
            catch (Exception e)
            {
                Debug.LogError("Error al guardar el archivo: " + e.Message);
            }
        }
    }

    string GetDownloadsPath()
    {
        // Método para obtener la ruta de la carpeta de descargas del usuario
        // Devuelve la ruta adecuada
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }
        else
        {
            return Application.persistentDataPath;
        }
    }
}
