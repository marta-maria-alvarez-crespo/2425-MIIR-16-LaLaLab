// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using System;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Security.Cryptography.X509Certificates;

public class MqttPublisher : MonoBehaviour
{
    // Clase para gestionar la conexión MQTT y publicar mensajes
    // Permite conectarse a un broker MQTT y publicar mensajes en un topic específico

    private IpMqtt IpMqtt;
    
    public string brokerEndpoint = IpMqtt.brokerEndpoint;
    public int brokerPort = IpMqtt.brokerPort;
    private string certificateFileName = IpMqtt.certificateFileName;
    private string pfxPassword = IpMqtt.pfxPassword;

    private MqttClient client;

    public bool IsConnected => client != null && client.IsConnected;

    public void Connect()
    {
        // Método para conectar al broker MQTT, cargando el certificado desde la carpeta StreamingAssets
        try
        {
            string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, "certificados", certificateFileName);
            X509Certificate2 clientCert = new X509Certificate2(fullPath, pfxPassword);
            client = new MqttClient(brokerEndpoint, brokerPort, true, null, clientCert, MqttSslProtocols.TLSv1_2);
            client.ProtocolVersion = MqttProtocolVersion.Version_3_1_1;

            client.Connect(Guid.NewGuid().ToString());
            Debug.Log("MQTT conectado.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al conectar MQTT: " + ex.Message);
        }
    }

    public void Publicar(string topic, string mensaje)
    {
        // Método para publicar un mensaje en un topic, previa comprobación de conexión
        if (IsConnected)
        {
            client.Publish(topic, Encoding.UTF8.GetBytes(mensaje), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            Debug.Log($"Se ha publicado en {topic}: {mensaje}");
        }
        else
        {
            Debug.LogWarning("MQTT no está conectado.");
        }
    }
}
