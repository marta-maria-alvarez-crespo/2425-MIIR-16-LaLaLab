// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;

public class RotadorPorAltura : MonoBehaviour
{
    // Clase para rotar un objeto según la altura de un cubo de agua
    // Permite actualziar la posicón de los sensores flotadores en función de la altura del agua
    [Header("Configuración de rotación")]
    public Transform objetoARotar;
    public float anguloInicial = 0f;
    public float anguloFinal = 45f;

    [Header("Referencia al cubo de agua")]
    public Transform cuboAgua;
    public float alturaMaxima = 5f;

    private Quaternion rotacionBase;

    void Start()
    {
        // Método que se ejecuta al iniciar el script
        // Guarda la rotación inicial del objeto a rotar
        if (objetoARotar != null)
        {
            rotacionBase = objetoARotar.localRotation;
        }
    }

    void Update()
    {
        // Método que se ejecuta en cada frame
        // Actualiza la rotación del objeto según la altura del cubo de agua
        if (objetoARotar == null || cuboAgua == null) return;

        float alturaActual = cuboAgua.localScale.y;
        float alturaNormalizada = Mathf.Clamp01(alturaActual / alturaMaxima);

        float anguloInterpolado = Mathf.Lerp(anguloInicial, anguloFinal, alturaNormalizada);
        Quaternion rotacionObjetivo = Quaternion.Euler(anguloInterpolado, -90, 90);

        objetoARotar.localRotation = Quaternion.Lerp(objetoARotar.localRotation, rotacionObjetivo, Time.deltaTime * 5f);
    }
}
