// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025


using SFB; 
using UnityEngine;
using System.IO;
using Environment = System.Environment;
using UnityEngine.Networking;
using System.Collections;


public class CargadorFuncionTransferencia : MonoBehaviour
{
    private string rutaArchivoGuardado;
    public void CargarArchivo()
    {
        // Método que permite cargar un ficher txt con la ft de cada planta
        // Se compureba que exista la carpeta, que se crea en el script SimuladorPlanta.cs (SimualdorPlanta.Start() (InicializarCarpetaPlantas))
        // Siempre se carga de (...)/Documents/lalalab/plantas
    
        var paths = StandaloneFileBrowser.OpenFilePanel("Selecciona archivo", "", "txt", false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                string directorioDocumentos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "lalalab");
                string carpetaPlantas = Path.Combine(directorioDocumentos, "plantas");
                string destino = Path.Combine(carpetaPlantas, Path.GetFileName(paths[0]));

                if (!Directory.Exists(carpetaPlantas))
                {
                    Debug.LogWarning("La carpeta de plantas no existe");
                }
                else
                {
                    File.Copy(paths[0], destino, true);
                    rutaArchivoGuardado = destino;
                    Debug.Log("Archivo guardado en: " + rutaArchivoGuardado);

                    // Extraer el id de la planta desde el nombre del archivo
                    string nombreArchivo = Path.GetFileNameWithoutExtension(paths[0]); // plantaN (n numero de planta)
                    string plantaId = nombreArchivo.Replace("planta", "");

                    // Llamar al backend para recargar
                    StartCoroutine(RecargarPlanta(plantaId));
                }
            }
        }

        private IEnumerator RecargarPlanta(string plantaId)
        {
            // Llamada al backend (servidor_local.py) para recargar la planta con el id indicado
            string url = $"http://127.0.0.1:8000/recargar";
            WWWForm form = new WWWForm();
            form.AddField("planta_id", plantaId);

            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error al recargar la planta: " + www.error);
                }
                else
                {
                    Debug.Log("Planta recargada correctamente: " + www.downloadHandler.text);
                }
            }
        }
}
        