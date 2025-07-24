// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using TMPro;

public class EscaladorCilindro : MonoBehaviour
{
    public TMP_Dropdown selectorDeModelos;
    public GameObject plantasContenedor;

    public Transform cilindroVisual;
    public float diametroMaximo = 1f; 
    public float consignaMaxima = 10f;

    private Vector3 escalaOriginal;

    void Start()
    {
        // Método que se ejecuta al iniciar el script
        // Inicializa el cilindro visual con la escala original
        if (cilindroVisual != null)
        {
            escalaOriginal = cilindroVisual.localScale;
        }
    }

    void Update()
    {
        // Método que se ejecuta en cada frame
        // Actualiza la escala del cilindro visual según la consigna de la bomba de la planta seleccionada
        // Obtiene el índice del modelo seleccionado y busca la planta correspondiente
        if (selectorDeModelos == null || plantasContenedor == null || cilindroVisual == null) return;

        int index = selectorDeModelos.value;

        if (index >= 0 && index < plantasContenedor.transform.childCount)
        {
            Transform plantaTransform = plantasContenedor.transform.GetChild(index);
            var simulador = plantaTransform.GetComponent<MonoBehaviour>();

            if (simulador != null)
            {
                var metodo = simulador.GetType().GetMethod("GetConsignaBomba");
                if (metodo != null)
                {
                    float consignaBomba = (float)metodo.Invoke(simulador, null);
                    float t = Mathf.Clamp01(consignaBomba / consignaMaxima);
                    float nuevoDiametro = Mathf.Lerp(0f, diametroMaximo, t);

                    Vector3 nuevaEscala = new Vector3(nuevoDiametro, escalaOriginal.y, nuevoDiametro);
                    cilindroVisual.localScale = nuevaEscala;
                }
            }
        }
    }
}
