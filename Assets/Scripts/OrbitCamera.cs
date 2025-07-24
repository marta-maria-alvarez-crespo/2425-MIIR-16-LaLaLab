// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public float orbitSpeed = 4f;
    public float zoomSpeed = 100f;
    public float minZoom = 10f;
    public float maxZoom = 500f;

    private float distance;

    void Start()
    {
        // Inicializo la posicion de la camra y la distancia al objeto
        distance = 390f;
        Vector3 dir = (transform.position - transform.parent.position).normalized;
        // Vector 3 hace referencia a las coordenadas x y y z
        transform.position = transform.parent.position + dir * distance;
        // transform.posiction lo que hace es mover la cámara a un sitio concreto del 
        // espacio 3D, en este caso, a una distancia fija del objeto central controlada desde el editor
    }

    void LateUpdate()
    {
        // ORBITAR alrededor del objeto 3D usando la rueda del ratón presionada
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X") * orbitSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * orbitSpeed;

            // Rotar la cámara alrededor del objeto central
            transform.RotateAround(transform.parent.position, Vector3.up, mouseX);
            transform.RotateAround(transform.parent.position, transform.right, mouseY);
        }

        // El zoom se controla con el giro de la rueda del ratón, sin pulsarla
        // Lo que hace realmente es desplazar la cámara hacia delante o hacia atrás, haciendo el efecto de zoom in o zoom out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // la distancia se calcula como la distancia actual menos el scroll multiplicado por la velocidad de zoom
            // y el tiempo transcurrido desde el último frame
            distance -= scroll * zoomSpeed * Time.deltaTime;
            // Se limita la distancia que se ha obtenido entre un mínimo y un máximo
            distance = Mathf.Clamp(distance, minZoom, maxZoom);

            // Determino hacia dónde mira la cámara con respecto al objeto central es decir, el modelo 3D
            // y normalizo el vector para que tenga una longitud de 1
            Vector3 dir = (transform.position - transform.parent.position).normalized;
            // Calculo la posición nueva que está a una distancia fija del objeto central, mirando a la dirección del vector
            transform.position = transform.parent.position + dir * distance;
        }

        // Mantener la cámara mirando al centro
        transform.LookAt(transform.parent.position);
    }
}
