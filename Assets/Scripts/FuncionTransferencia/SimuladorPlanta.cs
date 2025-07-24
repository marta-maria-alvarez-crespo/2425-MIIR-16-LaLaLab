// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Globalization;
using System.IO;
using System;

[System.Serializable] 
public class SimulacionResponse
{
    public float[] salida;
}

public class SimuladorPlanta : MonoBehaviour
{
    // Clase que identifica cada planta de nivel, permitiendo la simulación de su comportamiento
    // mediante su comunicación con el servidor local para obtener los datos de la salida (sensor)
    public PlantaNivel planta;
    private float consignaBomba = 0f;
    private float consignaValvula = 0f;
    private float tiempoMuestreo = 0.5f;
    private string urlSimular = "http://localhost:8000/simular";
    private Coroutine muestreoCoroutine;
    private float nivelSensor = 0f;

    public float GetNivelSensor()
    {
        return nivelSensor;
    }

    void Start()
    {
        // Inicializa la carpeta de plantas y lee el archivo de configuración
        // y se añade el listener para activar/desactivar la planta
        // previa comprbación de que estén iniciadas correctamente (tengan el toggle necesario)

        InicializarCarpetaPlantas();
        LeerArchivoConfiguracion();

        if (planta.toggleActiva != null)

        {
            planta.toggleActiva.onValueChanged.AddListener(OnToggleActivaChanged);

            if (planta.toggleActiva.isOn)
            {
                StartCoroutine(ActivarPlanta());
                muestreoCoroutine = StartCoroutine(Muestrear());
            }
        }
    }

    void InicializarCarpetaPlantas()
    {
        // Método que inicializa la carpeta de plantas en Documentos/lalalab/plantas
        // Si no existe, se crea y se copian las plantas base almacenadas desde StreamingAssets, para evitar errores

        string directorioDocumentos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "lalalab");
        string carpetaPlantas = Path.Combine(directorioDocumentos, "plantas");
        string origenPlantas = Path.Combine(Application.streamingAssetsPath, "plantas");

        try
        {
            if (!Directory.Exists(directorioDocumentos))
            {
                Directory.CreateDirectory(directorioDocumentos);
                // Debug.Log("Carpeta lalalab creada en Documentos.");
            }

            if (!Directory.Exists(carpetaPlantas))
            {
                CopiarEnDirectorio(origenPlantas, carpetaPlantas, true);
                // Debug.Log("Carpeta plantas copiada a Documentos/lalalab.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al inicializar carpetas: " + ex.Message);
        }
    }

    void CopiarEnDirectorio(string directorioDocumentos, string carpetaPlantas, bool copiar)
    {
        // Método que copia los archivos de plantas desde el directorio de origen a la carpeta de plantas
        DirectoryInfo dir = new DirectoryInfo(directorioDocumentos);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
        {
            // Debug.LogError("Directorio de origen no existe: " + directorioDocumentos);
            return;
        }

        Directory.CreateDirectory(carpetaPlantas);

        FileInfo[] archivos = dir.GetFiles();
        foreach (FileInfo archivo in archivos)
        {
            string rutaTemproal = Path.Combine(carpetaPlantas, archivo.Name);
            archivo.CopyTo(rutaTemproal, false);
        }

        if (copiar)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string rutaTemproal = Path.Combine(carpetaPlantas, subdir.Name);
                CopiarEnDirectorio(subdir.FullName, rutaTemproal, copiar);
            }
        }
    }

    void CopiarPlantas(string dirBase, string destDirName, bool copySubDirs)
    {
        // Método que copia los archivos de plantas desde el directorio base a un destino
        DirectoryInfo dir = new DirectoryInfo(dirBase);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
        {
            // Debug.LogError("Directorio de origen no existe: " + dirBase);
            return;
        }

        Directory.CreateDirectory(destDirName);

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                CopiarPlantas(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }

    void OnToggleActivaChanged(bool isOn)
    {
        // Método que comprueba si el toggle de la planta está activo o no
        // Si está activo, se inicia la simulación de muestreo, si no, se detiene, para ahorrar recursos
        if (isOn)
        {
            StartCoroutine(ActivarPlanta());

            if (muestreoCoroutine == null)
                muestreoCoroutine = StartCoroutine(Muestrear());
        }
        else
        {
            StartCoroutine(DesactivarPlanta());

            if (muestreoCoroutine != null)
            {
                StopCoroutine(muestreoCoroutine);
                muestreoCoroutine = null;
            }
        }
    }

    IEnumerator Muestrear()
    {
        // Método que simula el muestreo de la planta, enviando los valores de consigna de bomba y válvula
        while (true)
        {
            float bomba = consignaBomba;
            float valvula = consignaValvula;

            // Debug.Log($"Enviando a planta {planta.plantaId}: bomba={bomba}, válvula={valvula}");

            yield return SimularSalida(bomba, valvula, salida =>
            {
                nivelSensor = salida;
                string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
                // Debug.Log($"[{timestamp}] Planta {planta.plantaId} | Salida simulada: {salida:0.00}");
            });

            yield return new WaitForSeconds(tiempoMuestreo);
        }
    }

    public void LeerArchivoConfiguracion()
    {
        // Método que lee el archivo de configuración de la planta para obtener el tiempo de muestreo
        string documentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string ruta = Path.Combine(documentos, "lalalab", "plantas", $"planta{planta.plantaId}.txt");

        if (File.Exists(ruta))
        {
            string[] lines = File.ReadAllLines(ruta);
            foreach (string line in lines)
            {
                if (line.StartsWith("Ts:"))
                {
                    string valor = line.Split(':')[1].Trim();
                    if (float.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out float ts))
                    {
                        tiempoMuestreo = ts;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"Archivo de configuración no encontrado para planta {planta.plantaId} en: {ruta}");
        }
    }


    IEnumerator SimularSalida(float bomba, float valvula, System.Action<float> callback)
    {
        // Método que envía los valores de consigna de bomba y válvula al servidor local para simular la salida
        // y recibe la salida simulada
        string json = $"{{\"planta_id\":\"{planta.plantaId}\",\"entradas\":[{bomba.ToString(CultureInfo.InvariantCulture)},{valvula.ToString(CultureInfo.InvariantCulture)}]}}";
        UnityWebRequest request = new UnityWebRequest(urlSimular, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            SimulacionResponse response = JsonUtility.FromJson<SimulacionResponse>(responseText);

            if (response != null && response.salida != null && response.salida.Length > 0)
            {
                callback?.Invoke(response.salida[0]);
            }
            else
            {
                Debug.LogWarning($"Respuesta vacía {planta.plantaId}");
            }
        }
        else
        {
            Debug.LogError($"Error al simular planta {planta.plantaId}: {request.error} - Código: {request.responseCode} - Respuesta: {request.downloadHandler.text}");
        }
    }

    IEnumerator ActivarPlanta()
    {
        // Método que activa la planta enviando una solicitud al servidor local
        string urlActivar = $"http://localhost:8000/activar?planta_id={planta.plantaId}";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(urlActivar, "");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Error al activar planta {planta.plantaId}: {request.error}");
        else
            Debug.Log($"Planta {planta.plantaId} activada");
    }

    IEnumerator DesactivarPlanta()
    {
        // Análogo al anterior, pero desactiva la planta
        string urlDesactivar = $"http://localhost:8000/desactivar?planta_id={planta.plantaId}";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(urlDesactivar, "");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError($"Error al desactivar planta {planta.plantaId}: {request.error}");
        else
            Debug.Log($"Planta {planta.plantaId} desactivada ");
    }

    public void ActualizarConsigna(float bomba, float valvula)
    {
        // Método que actualiza las consignas de bomba y válvula de la planta   
        consignaBomba = bomba;
        consignaValvula = valvula;
        Debug.Log($"Consigna actualizada en planta {planta.plantaId}: bomba={bomba}, valvula={valvula}");
    }
    
    public float GetConsignaBomba()
    {
        // Método que devuelve la consigna de la bomba
        return consignaBomba;
    }

    public float GetConsignaValvula()
    {
        // Método que devuelve la consigna de la válvula
        return consignaValvula;
    }
}
