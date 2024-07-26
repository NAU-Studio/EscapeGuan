using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EscapeGuan.UI.Starting
{
    public class StartingButton : Button
    {
        public RectTransform DecorationHighlight;
        public Color ClickDownColor;

        public Image DecorationHighlightImage => DecorationHighlight.GetComponent<Image>();

        public override void OnPointerDown(PointerEventData e)
        {
            DecorationHighlightImage.CrossFadeColor(ClickDownColor, Transition, false, true);
        }

        public override void OnPointerEnter(PointerEventData e)
        {
            base.OnPointerEnter(e);
            DecorationHighlight.DOScaleX(1, Transition).SetEase(Ease.OutCubic);
        }

        public override void OnPointerExit(PointerEventData e)
        {
            DecorationHighlight.DOScaleX(0, Transition).SetEase(Ease.InCubic);
        }

        public override void OnPointerUp(PointerEventData e)
        {
            base.OnPointerUp(e);
            DecorationHighlightImage.CrossFadeColor(Color.white, Transition, false, true);
        }
    }
}
