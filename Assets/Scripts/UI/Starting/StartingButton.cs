using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartingButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public RectTransform DecorationHighlight;
    public Color ClickDownColor;

    public Image DecorationHighlightImage => DecorationHighlight.GetComponent<Image>();

    public void OnPointerDown(PointerEventData eventData)
    {
        DecorationHighlightImage.CrossFadeColor(ClickDownColor, .2f, false, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DecorationHighlight.DOScaleX(1, .2f).SetEase(Ease.OutCubic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DecorationHighlight.DOScaleX(0, .2f).SetEase(Ease.InCubic);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DecorationHighlightImage.CrossFadeColor(Color.white, .2f, false, true);
    }
}
