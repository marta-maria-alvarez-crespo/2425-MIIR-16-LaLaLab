// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class SimpleToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Clase para un toggle simple con retroalimentación visual
    // Permite cambiar su estado y aplicar animaciones al interactuar con él
    [Header("UI References")]
    public Image targetImage;
    public Toggle unityToggle; // Toggle real de Unity

    [Header("Sprites")]
    public Sprite onSprite;
    public Sprite offSprite;

    [Header("Scaling")]
    public float baseScale = 0.1f;
    public float hoverScaleMultiplier = 1.1f;
    public float clickScaleMultiplier = 0.8f;

    [Header("Colors")]
    public Color hoverColor = new Color(0.302f, 0.471f, 1f, 1f);
    public Color clickColor = new Color(0f, 0.067f, 0.467f, 1f);

    [Header("Timing")]
    public float transitionDuration = 0.15f;

    [Header("Events")]
    public UnityEvent<bool> onToggleChanged;

    private bool isOn = false;
    private bool isHovering = false;
    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;

    void Start()
    {
        // Al iniciar, se configura el toggle y se aplica el estado visual inicial
        if (unityToggle != null)
        {
            isOn = unityToggle.isOn;
        }

        UpdateSprite();
        ResetVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Método que se ejecuta al pasar el cursor sobre el toggle
        // Cambia el tamaño y color del toggle al pasar el cursor
        isHovering = true;
        AnimateVisuals(baseScale * hoverScaleMultiplier, hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Método que se ejecuta al salir del cursor del toggle
        // Restaura el tamaño y color del toggle al estado normal
        isHovering = false;
        AnimateVisuals(baseScale, Color.white);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Método que se ejecuta al hacer clic en el toggle
        // Si el toggle no está activo, aplica un efecto de clic y actualiza el estado
        SetState(!isOn);
    }

    private void UpdateSprite()
    {
        // Método para actualizar el sprite del toggle según su estado
        // Si el toggle está activo, muestra el sprite "on"; si no, muestra el sprite "off"
        targetImage.sprite = isOn ? onSprite : offSprite;
    }

    private void ResetVisuals()
    {
        // Método para restablecer el estado visual del toggle
        // Establece el tamaño y color del toggle al estado base sin animaciones
        targetImage.transform.localScale = Vector3.one * baseScale;
        targetImage.color = Color.white;
    }

    private void AnimateVisuals(float targetScale, Color targetColor)
    {
        // Método para animar el tamaño y color del toggle
        // Si el objeto no está activo o la imagen objetivo es nula, no hace nada
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

        float targetScale = isHovering ? baseScale * hoverScaleMultiplier : baseScale;
        Color targetColor = isHovering ? hoverColor : Color.white;
        AnimateVisuals(targetScale, targetColor);
    }

    public void SetState(bool state)
    {
        // Método para establecer el estado del toggle
        // Si el estado es el mismo que el actual, no hace nada
        if (isOn == state) return;

        isOn = state;
        if (unityToggle != null)
        {
            unityToggle.isOn = state;
        }

        UpdateSprite();
        onToggleChanged?.Invoke(state);

        // Inicia animación visual como si se hubiera hecho clic
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        StartCoroutine(ClickEffect());
    }


    public bool IsOn()
    {
        // Método para obtener el estado actual del toggle
        // Devuelve el estado del toggle de Unity si está asignado, o el estado interno
        return unityToggle != null ? unityToggle.isOn : isOn;
    }
}
