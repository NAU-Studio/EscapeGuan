using DG.Tweening;

using UnityEngine;

namespace EscapeGuan.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Hidable : RectBehaviour
    {
        public float Transition;
        [Range(0, 1)]
        public float ShowAlpha = 1;

        public virtual void Show()
        {
            GetComponent<CanvasGroup>().DOFade(ShowAlpha, Transition);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
        }

        protected virtual void ShowNoTransition()
        {
            GetComponent<CanvasGroup>().alpha = ShowAlpha;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
        }

        public virtual void Hide()
        {
            GetComponent<CanvasGroup>().DOFade(0, Transition);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
        }

        protected virtual void HideNoTransition()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
        }
    }
}