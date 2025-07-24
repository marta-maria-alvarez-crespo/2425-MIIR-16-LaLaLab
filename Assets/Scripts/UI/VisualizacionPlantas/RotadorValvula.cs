// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using TMPro;

public class RotadorValvula : MonoBehaviour
{
    // Clase para rotar la válvula de una planta según el voltaje recibido
    // Permite visualizar el estado de la válvula en función del voltaje de consigna
    public TMP_Dropdown selectorDeModelos;
    public GameObject plantasContenedor;

    public float anguloMaximo = 90f;
    public float voltajeMaximo = 10f;

    public Transform valvulaVisual;

    private Quaternion rotacionInicial;

    void Start()
    {
        if (valvulaVisual != null)
        {
            rotacionInicial = valvulaVisual.rotation;
        }
    }

    void Update()
    {
        // Método que se ejecuta en cada frame
        // Actualiza la rotación de la válvula visual según el voltaje de consigna de la planta seleccionada
        // Obtiene el índice del modelo seleccionado y busca la planta correspondiente
        if (selectorDeModelos == null || plantasContenedor == null || valvulaVisual == null) return;

        int index = selectorDeModelos.value;

        if (index >= 0 && index < plantasContenedor.transform.childCount)
        {
            Transform plantaTransform = plantasContenedor.transform.GetChild(index);
            var simulador = plantaTransform.GetComponent<MonoBehaviour>();

            if (simulador != null)
            {
                var metodo = simulador.GetType().GetMethod("GetConsignaValvula");
                if (metodo != null)
                {
                    float voltajeValvula = (float)metodo.Invoke(simulador, null);
                    float t = 1f - Mathf.Clamp01(voltajeValvula / voltajeMaximo);
                    float anguloY = Mathf.Lerp(0f, anguloMaximo, t);

                    Quaternion rotacionObjetivo = rotacionInicial * Quaternion.Euler(0f, 0f, anguloY);
                    valvulaVisual.rotation = Quaternion.Lerp(valvulaVisual.rotation, rotacionObjetivo, Time.deltaTime * 5f);
                }
            }
        }
    }
}
