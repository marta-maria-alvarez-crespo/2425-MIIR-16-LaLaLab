// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Security.Cryptography.X509Certificates;

[Serializable]
public class TopicConfig
{
    // Clase para deserializar el JSON de configuración de topics MQTT
    public List<Laboratorio> laboratorios;
}

[Serializable]
public class Laboratorio
{
    // Clase que representa un laboratorio con su nombre y grupos de sistemas
    public string nombre;
    public List<Grupo> grupos;
}

[Serializable]
public class Grupo
{
    // Clase que representa un grupo de sistemas dentro de un laboratorio
    public string nombreGrupo;
    public List<int> ids;
}

[Serializable]
public class SensorPayload
{
    // Clase para deserializar el JSON de los mensajes recibidos por MQTT
    // Contiene el valor del sensor y la unidad de medida
    public float Sensor;
    public string USensor;
}

public class MqttSubscriberFromJson : MonoBehaviour
{
    private MqttClient client;

    [Header("AWS IoT Config")]

    private IpMqtt IpMqtt;
    
    public string brokerEndpoint = IpMqtt.brokerEndpoint;
    public int brokerPort = IpMqtt.brokerPort;
    private string certificateFileName = IpMqtt.certificateFileName;
    private string pfxPassword = IpMqtt.pfxPassword;

    [Header("JSON Config")]
    public string jsonFileName = "mqtt_topics.json";

    public static event Action<int, float> OnSensorRealRecibido;

    void Start()
    {
        ConnectAndSubscribe();
    }

    void ConnectAndSubscribe()
    {
        // Método para conectar al broker MQTT y suscribirse a los topics definidos en el JSON
        // Carga el archivo JSON desde StreamingAssets y deserializa los topics
        // Luego se conecta al broker MQTT y se suscribe a los topics correspondientes
        // utlizando el certificado para identigicarse en IoT Core
        try
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
            if (!File.Exists(jsonPath))
            {
                Debug.LogError("No se encontró el archivo JSON: " + jsonPath);
                return;
            }

            string json = File.ReadAllText(jsonPath);
            TopicConfig config = JsonUtility.FromJson<TopicConfig>(json);

            List<string> topics = new List<string>();
            foreach (var lab in config.laboratorios)
            {
                foreach (var grupo in lab.grupos)
                {
                    foreach (int id in grupo.ids)
                    {
                        string topic = $"{lab.nombre}/{grupo.nombreGrupo}{id}/parametros";
                        topics.Add(topic);
                    }
                }
            }

            string certPath = Path.Combine(Application.streamingAssetsPath, "certificados", certificateFileName);
            X509Certificate2 clientCert = new X509Certificate2(certPath, pfxPassword);
            client = new MqttClient(brokerEndpoint, brokerPort, true, null, clientCert, MqttSslProtocols.TLSv1_2);
            client.ProtocolVersion = MqttProtocolVersion.Version_3_1_1;

            client.MqttMsgPublishReceived += OnMessageReceived;

            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);

            byte[] qosLevels = new byte[topics.Count];
            for (int i = 0; i < qosLevels.Length; i++) qosLevels[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;

            client.Subscribe(topics.ToArray(), qosLevels);

            foreach (var topic in topics)
                Debug.Log($"Suscrito a: {topic}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al conectar o suscribirse: " + ex.Message);
        }
    }

    private GraficadorPlantasConMQTT graficador;

    void Awake()
    {
        // Busca el componente GraficadorPlantasConMQTT en la escena
        // para poder actualizar los gráficos con los datos recibidos por MQTT
        graficador = FindFirstObjectByType<GraficadorPlantasConMQTT>();
    }

    void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        // Método que se ejecuta cuando se recibe un mensaje por MQTT
        // Deserializa el mensaje JSON y extrae el valor del sensor y su unidad
        // Luego invoca el evento OnSensorRealRecibido con el índice de la planta y el valor del sensor
        // También actualiza el gráfico correspondiente en GraficadorPlantasConMQTT
        string message = Encoding.UTF8.GetString(e.Message);
        Debug.Log($"Mensaje recibido en {e.Topic}: {message}");

        string[] partes = e.Topic.Split('/');

        foreach (string parte in partes)
        {
            if (parte.StartsWith("planta_nivel"))
            {
                string numeroStr = parte.Replace("planta_nivel", "");
                if (int.TryParse(numeroStr, out int plantaIndex))
                {
                    try
                    {
                        SensorPayload payload = JsonUtility.FromJson<SensorPayload>(message);
                        float valor = payload.Sensor;
                        string unidad = payload.USensor.Trim();

                        Debug.Log($"[MQTT] Unidad recibida: '{unidad}'");

                        if (unidad == "%")
                        {
                            valor /= 10f;
                        }

                        Debug.Log($"Sensor real recibido para planta {plantaIndex}: {valor} V");

                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            OnSensorRealRecibido?.Invoke(plantaIndex, valor);

                            var graficador = FindFirstObjectByType<GraficadorPlantasConMQTT>();
                            if (graficador != null)
                                graficador.ActualizarSensorRealMQTT(plantaIndex - 1, valor, unidad);
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error al leer el JSON: {ex.Message}");
                    }
                }
                break;
            }
        }
    }
}
