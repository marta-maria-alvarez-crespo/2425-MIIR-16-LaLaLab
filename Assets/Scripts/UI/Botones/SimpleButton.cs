// Autora: Marta María Álvarez Crespo
// Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
// GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

// Ultima modificación: 18/07/2025

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SimpleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Clase para un botón simple con retroalimentación visual
    // Permite cambiar su estado y aplicar animaciones al interactuar con él
    [Header("UI References")]
    public Image targetImage;

    [Header("Sprite")]
    public Sprite buttonSprite;

    [Header("Scaling")]
    public float baseScale = 0.1f;
    public float hoverScaleMultiplier = 1.1f;
    public float clickScaleMultiplier = 0.8f;

    [Header("Colors")]
    public Color hoverColor = new Color(0.302f, 0.471f, 1f, 1f);
    public Color clickColor = new Color(0f, 0.067f, 0.467f, 1f);

    [Header("Timing")]
    public float transitionDuration = 0.15f;

    private bool isHovering = false;
    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;

    void Start()
    {
        // Al iniciar, se configura el sprite del botón y se aplica el estado visual inicial
        targetImage.sprite = buttonSprite;
        ResetVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Método que se ejecuta al pasar el cursor sobre el botón
        // Cambia el tamaño y color del botón al pasar el cursor
        isHovering = true;
        AnimateVisuals(baseScale * hoverScaleMultiplier, hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Método que se ejecuta al quitar el cursor del botón
        // Cambia el tamaño y color del botón al estado normal
        isHovering = false;
        AnimateVisuals(baseScale, Color.white);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Método que se ejecuta al hacer clic en el botón
        // Cambia el color del botón al hacer clic y aplica un efecto de escala

        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        targetImage.color = clickColor;

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        StartCoroutine(ClickEffect());
    }

    private void ResetVisuals()
    {
        // Método para restablecer el estado visual del botón
        // Establece el tamaño y color del botón al estado base sin animaciones
        targetImage.transform.localScale = Vector3.one * baseScale;
        targetImage.color = Color.white;
    }

    private void AnimateVisuals(float targetScale, Color targetColor)
    {
        // Método para animar el tamaño y color del botón
        // Interpola entre el tamaño actual y el tamaño objetivo durante un tiempo determinado
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);

        scaleCoroutine = StartCoroutine(ScaleTo(targetScale));
        colorCoroutine = StartCoroutine(ColorTo(targetColor));
    }

    private IEnumerator ScaleTo(float target)
    {
        // Método para animar el tamaño del botón
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
        // Método para animar el color del botón
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
        // Método para aplicar un efecto de clic al botón
        // Escala el botón a un tamaño más pequeño y luego lo devuelve al tamaño original
        yield return ScaleTo(baseScale * clickScaleMultiplier);
        yield return new WaitForSeconds(0.05f);

        float targetScale = isHovering ? baseScale * hoverScaleMultiplier : baseScale;
        Color targetColor = isHovering ? hoverColor : Color.white;
        AnimateVisuals(targetScale, targetColor);
    }
}
