// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropdownFixer : MonoBehaviour
{
    // Clase para personalizar el estilo de un Dropdown de TextMeshPro
    // Ajusta el tamaño, espaciado y estilo de los elementos del Dropdown
    public TMP_Dropdown dropdown;
    public Vector2 tamañoDeseado = new Vector2(250, 200);
    public float alturaItem = 40f;
    public float espaciadoEntreItems = 20f;
    public TMP_FontAsset fuente;
    public Color colorTexto = Color.white;
    public int tamañoFuente = 18;

    void Update()
    {
        // Método que se ejecuta en cada frame para aplicar el estilo al Dropdown
        // Comprueba si el Dropdown está asignado y aplica el estilo
        GameObject lista = GameObject.Find("Dropdown List");
        if (lista == null || lista.GetComponent<EstiloAplicado>() != null)
            return;

        try
        {
            RectTransform rect = lista.GetComponent<RectTransform>();
            if (rect != null)
                rect.sizeDelta = tamañoDeseado;

            Transform content = lista.transform.Find("Viewport/Content");
            if (content == null)
            {
                Debug.LogWarning("No se encontró 'Content' dentro del Dropdown List.");
                return;
            }

            float yOffset = 0f;

            foreach (Transform item in content)
            {
                if (item.name.Contains("Item") && item.GetComponent<Toggle>() != null)
                {
                    RectTransform itemRect = item.GetComponent<RectTransform>();
                    if (itemRect != null)
                    {
                        itemRect.anchorMin = new Vector2(0, 1);
                        itemRect.anchorMax = new Vector2(1, 1);
                        itemRect.pivot = new Vector2(0.5f, 1);
                        itemRect.sizeDelta = new Vector2(0, alturaItem);
                        itemRect.anchoredPosition = new Vector2(0, -yOffset);
                        yOffset += alturaItem + espaciadoEntreItems;
                    }

                    TextMeshProUGUI label = item.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        label.font = fuente;
                        label.fontSize = tamañoFuente;
                        label.color = colorTexto;
                        label.enableAutoSizing = false;
                    }
                }
            }

            lista.AddComponent<EstiloAplicado>();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error al personalizar Dropdown List: " + e.Message);
        }
    }
}

public class EstiloAplicado : MonoBehaviour { } // Marca para evitar aplicar el estilo varias veces
