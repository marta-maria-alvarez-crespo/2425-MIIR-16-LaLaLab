// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class ToggleVisualFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Clase para proporcionar retroalimentación visual en los toggles
    // Permite mostrar animaciones en el toggle al interactuar con él
    [Header("UI References")]
    public Image targetImage;

    [Header("Scaling")]
    public float baseScale = 0.1f;
    public float hoverScaleMultiplier = 1.1f;
    public float clickScaleMultiplier = 0.8f;

    [Header("Colors")]
    public Color hoverColor = new Color(0.302f, 0.471f, 1f, 1f);
    public Color normalColor = Color.white;

    [Header("OFF State")]
    [Range(0f, 1f)] public float offAlpha = 0.4f;

    [Header("Timing")]
    public float transitionDuration = 0.15f;

    private Toggle toggle;
    private bool isHovering = false;
    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;

    void Awake()
    {
        // Inicializar el componente Toggle
        toggle = GetComponent<Toggle>();
    }

    void Start()
    {
        // Aplicar estado visual inicial sin animaciones
        ApplyVisualsInstant();

        // Añadir listener después de la inicialización
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnDestroy()
    {
        // Limpiar listeners para evitar referencias perdidas
        toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        // Método que se ejecuta al cambiar el estado del toggle
        // Actualiza la animación y el color del toggle según su estado
        if (!gameObject.activeInHierarchy) return;
        AnimateVisuals(GetTargetScale(), GetCurrentColor(isOn));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Método que se ejecuta al pasar el cursor sobre el toggle
        // Cambia el estado de hover y actualiza la animación
        isHovering = true;
        if (!gameObject.activeInHierarchy) return;
        AnimateVisuals(GetTargetScale(), GetCurrentColor(toggle.isOn));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Método que se ejecuta al salir del cursor del toggle
        // Cambia el estado de hover y actualiza la animación
        isHovering = false;
        if (!gameObject.activeInHierarchy) return;
        AnimateVisuals(GetTargetScale(), GetCurrentColor(toggle.isOn));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Método que se ejecuta al hacer clic en el toggle
        // Si el toggle no está activo, se aplica un efecto de clic
        // y se actualiza la animación
        if (!toggle.isOn && gameObject.activeInHierarchy)
        {
            StartCoroutine(ClickEffect());
        }
    }

    private void ApplyVisualsInstant()
    {
        // Método para aplicar el estado visual inicial del toggle sin animaciones
        // Establece el tamaño y color del toggle según su estado
        if (targetImage != null)
        {
            targetImage.transform.localScale = Vector3.one * baseScale;
            targetImage.color = GetCurrentColor(toggle.isOn);
        }
    }

    private Color GetCurrentColor(bool isOn)
    {
        // Método para obtener el color actual del toggle según su estado
        // Si el toggle está activo, devuelve el color normal; si no, aplica el alpha
        float alpha = isOn ? 1f : offAlpha;
        return new Color(normalColor.r, normalColor.g, normalColor.b, alpha);
    }

    private float GetTargetScale()
    {
        // Método para obtener el tamaño objetivo del toggle según su estado
        // Si el toggle está activo, devuelve el tamaño base; si no, aplica el multiplicador de hover
        return isHovering ? baseScale * hoverScaleMultiplier : baseScale;
    }

    private void AnimateVisuals(float targetScale, Color targetColor)
    {
        // Método para animar el tamaño y color del toggle
        // Si el objeto no está activo o la imagen objetivo es nula, no hace nada
        if (!gameObject.activeInHierarchy || targetImage == null) return;

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);

        scaleCoroutine = StartCoroutine(ScaleTo(targetScale));
        colorCoroutine = StartCoroutine(ColorTo(targetColor));
    }

    private IEnumerator ScaleTo(float target)
    {
        // Método para animar el tamaño del toggle
        // Interpola entre el tamaño actual y el tamaño objetivo durante un tiempo determinado
        Vector3 start = targetImage.transform.localScale;
        Vector3 end = Vector3.one * target;
        float t = 0;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            targetImage.transform.localScale = Vector3.Lerp(start, end, t / transitionDuration);
            yield return null;
        }

        targetImage.transform.localScale = end;
    }

    private IEnumerator ColorTo(Color target)
    {
        // Método para animar el color del toggle
        // Interpola entre el color actual y el color objetivo durante un tiempo determinado
        Color start = targetImage.color;
        float t = 0;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            targetImage.color = Color.Lerp(start, target, t / transitionDuration);
            yield return null;
        }

        targetImage.color = target;
    }

    private IEnumerator ClickEffect()
    {
        // Método para aplicar un efecto de clic al toggle
        // Escala el toggle a un tamaño más pequeño y luego lo devuelve al tamaño original
        yield return ScaleTo(baseScale * clickScaleMultiplier);
        yield return new WaitForSeconds(0.05f);
        AnimateVisuals(GetTargetScale(), GetCurrentColor(toggle.isOn));
    }
}
