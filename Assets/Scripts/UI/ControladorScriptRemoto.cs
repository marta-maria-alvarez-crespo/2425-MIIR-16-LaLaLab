// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using SFB;

public class ControladorScriptRemoto : MonoBehaviour
{
    // Clase para controlar la subida de scripts y su ejecución en un servidor remoto
    // Permite seleccionar un script desde el sistema de archivos y subirlo a la EC2
    public Button botonSubir;
    public Toggle toggleEjecutar;
    private IpServer IpServer;
    public string servidor = IpServer.servidor;

    private string rutaScriptSeleccionado = "";

    void Start()
    {
        // Al iniciar, se configura el botón para subir un script y el toggle para controlar su ejecución
        botonSubir.onClick.AddListener(SeleccionarYSubirScript);
        toggleEjecutar.onValueChanged.AddListener(ControlarEjecucion);
    }

    void SeleccionarYSubirScript()
    {
        // Método para seleccionar un script desde el sistema de archivos y subirlo al servidor
        // Utiliza StandaloneFileBrowser para abrir un diálogo de selección de archivos
        var extensiones = new[] {
            new ExtensionFilter("Python Scripts", "py")
        };
        var rutas = StandaloneFileBrowser.OpenFilePanel("Selecciona un script", "", extensiones, false);

        if (rutas.Length > 0 && File.Exists(rutas[0]))
        {
            rutaScriptSeleccionado = rutas[0];
            StartCoroutine(SubirScript(rutaScriptSeleccionado));
        }
    }

    IEnumerator SubirScript(string ruta)
    {
        // Método para subir el script seleccionado al servidor
        // Utiliza UnityWebRequest para enviar el archivo al servidor especificado
        WWWForm form = new WWWForm();
        byte[] fileData = File.ReadAllBytes(ruta);
        form.AddBinaryData("file", fileData, "ataque.py", "text/x-python");

        UnityWebRequest www = UnityWebRequest.Post(servidor + "/upload", form);

        yield return www.SendWebRequest();

        if (www.responseCode == 200)
        {
            Debug.Log("Script subido correctamente");
            Debug.Log("Respuesta: " + www.downloadHandler.text);
        }
        else if (www.responseCode == 0)
        {
            Debug.LogWarning("No se recibió respuesta del servidor (posible respuesta vacía)");
        }
    }

    void ControlarEjecucion(bool ejecutar)
    {
        // Método que se ejecuta al cambiar el estado del toggle para ejecutar el script
        // Envía una petición al servidor para iniciar o detener la ejecución del script
        StartCoroutine(EnviarFlagEjecucion(ejecutar));
    }

    IEnumerator EnviarFlagEjecucion(bool ejecutar)
    {
        // Método para enviar el estado de ejecución del script al servidor
        // Utiliza UnityWebRequest para enviar un JSON con el estado de ejecución
        string json = "{\"ejecutar\": " + ejecutar.ToString().ToLower() + "}";
        UnityWebRequest www = new UnityWebRequest(servidor + "/control", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.responseCode == 200)
        {
            Debug.Log("Flag enviado correctamente");
            Debug.Log("Respuesta: " + www.downloadHandler.text);
        }
        else if (www.responseCode == 0)
        {
            Debug.LogWarning("No se recibió respuesta del servidor (posible respuesta vacía)");
        }
    }
}
