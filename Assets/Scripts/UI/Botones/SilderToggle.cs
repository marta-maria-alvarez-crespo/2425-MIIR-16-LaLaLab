// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SliderToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Clase para un toggle con retroalimentación visual y animaciones
    // Permite cambiar su estado y aplicar animaciones al interactuar con él
    [Header("UI References")]
    public Image targetImage;
    public Toggle toggle;

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

    [Header("Movement")]
    public float moveDistance = 50f;

    [Header("Timing")]
    public float transitionDuration = 0.15f;

    private bool isOn = false;
    private bool isHovering = false;
    private bool suppressToggleEvent = false;

    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;

    private RectTransform imageTransform;

    void Awake()
    {
        // Inicializar el componente Toggle y la imagen de destino
        // Asegurarse de que el componente Toggle está asignado
        // y que la imagen de destino está configurada correctamente
        imageTransform = targetImage.GetComponent<RectTransform>();

        if (toggle == null)
            toggle = GetComponent<Toggle>();

        if (toggle == null)
        {
            Debug.LogError($"[{name}] No se encontró un componente Toggle.");
            return;
        }

        isOn = false;
        toggle.isOn = false;

        UpdateVisualsInstant();
        ResetVisuals();
    }

    void Start()
    {
        // Al iniciar, se configura el toggle y se aplica el estado visual inicial
        // Añadir listener para detectar cambios en el estado del toggle
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool value)
    {
        // Método que se ejecuta al cambiar el estado del toggle
        // Si el evento está suprimido, no se aplica el cambio visual
        if (suppressToggleEvent)
        {
            return;
        }

        // Debug.Log($"[{name}] Toggle cambiado a: {value}");
        ApplyState(value);
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
        return; // No hacer nada al hacer clic, el estado se maneja por el Toggle
    }

    private void ApplyState(bool state)
    {
        // Método para aplicar el estado del toggle
        // Actualiza el estado interno y visual del toggle
        isOn = state;
        Debug.Log($"[{name}] ApplyState: {state}");

        UpdateVisualsInstant();

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        StartCoroutine(ClickEffect());
    }

    private void UpdateVisualsInstant()
    {
    // Método para actualizar el sprite del toggle según su estado
    // Si el toggle está activo, muestra el sprite "on"; si no, muestra el sprite "off"
    if (targetImage == null || imageTransform == null)
        {
            return;
        }
    targetImage.sprite = isOn ? onSprite : offSprite;
}


    private void ResetVisuals()
    {
        // Método para restablecer el estado visual del toggle
        // Establece el tamaño y color del toggle al estado base sin animaciones
        imageTransform.localScale = Vector3.one * baseScale;
        targetImage.color = Color.white;
    }

    private void AnimateVisuals(float targetScale, Color targetColor)
    {
        // Método para animar el tamaño y color del toggle
        // Interpola entre el tamaño actual y el tamaño objetivo durante un tiempo determinado
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);

        scaleCoroutine = StartCoroutine(ScaleTo(targetScale));
        colorCoroutine = StartCoroutine(ColorTo(targetColor));
    }

    private IEnumerator ScaleTo(float target)
    {
        // Método para animar el tamaño del toggle
        // Interpola entre el tamaño actual y el tamaño objetivo durante un tiempo determinado
        Vector3 start = imageTransform.localScale;
        Vector3 end = Vector3.one * target;
        float t = 0;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            imageTransform.localScale = Vector3.Lerp(start, end, t / transitionDuration);
            yield return null;
        }

        imageTransform.localScale = end;
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

        Vector2 startPos = imageTransform.anchoredPosition;
        Vector2 endPos = isOn ? new Vector2(moveDistance, 0) : Vector2.zero;

        float t = 0;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            imageTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t / transitionDuration);
            yield return null;
        }

        imageTransform.anchoredPosition = endPos;

        float finalScale = isHovering ? baseScale * hoverScaleMultiplier : baseScale;
        Color finalColor = isHovering ? hoverColor : Color.white;
        AnimateVisuals(finalScale, finalColor);
    }

    public bool IsOn()
    {
        // Método para obtener el estado actual del toggle
        // Devuelve el estado del toggle de Unity si está asignado, o el estado interno
        return isOn;
    }

    public void SetState(bool state)
    {
        // Método para establecer el estado del toggle
        // Si el estado es el mismo que el actual, no hace nada
        if (toggle != null)
        {
            suppressToggleEvent = true;
            toggle.isOn = state;
            suppressToggleEvent = false;

            ApplyState(state);
        }
    }
}
