using DG.Tweening;

using UnityEngine;

namespace EscapeGuan.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HideableUI : MonoBehaviour
    {
        public float Transition;
        [Range(0, 1)]
        public float ShowAlpha = 1;
        public bool Hided;

        public virtual void Show()
        {
            if (!Hided)
                return;
            GetComponent<CanvasGroup>().DOFade(ShowAlpha, Transition);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            Hided = false;
        }

        protected virtual void ShowNoTransition()
        {
            if (!Hided)
                return;
            GetComponent<CanvasGroup>().alpha = ShowAlpha;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            Hided = false;
        }

        public virtual void Hide()
        {
            if (Hided)
                return;
            GetComponent<CanvasGroup>().DOFade(0, Transition);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
            Hided = true;
        }

        protected virtual void HideNoTransition()
        {
            if (!Hided)
                return;
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            Hided = false;
        }
    }
}