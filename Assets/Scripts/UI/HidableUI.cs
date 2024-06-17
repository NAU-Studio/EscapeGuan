using DG.Tweening;

using UnityEngine;

namespace EscapeGuan.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HidableUI : MonoBehaviour
    {
        public float Transition;
        [Range(0, 1)]
        public float ShowAlpha = 1;
        public bool Hidden;

        public virtual void Show()
        {
            if (!Hidden)
                return;
            GetComponent<CanvasGroup>().DOFade(ShowAlpha, Transition);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            Hidden = false;
        }

        protected virtual void ShowNoTransition()
        {
            if (!Hidden)
                return;
            GetComponent<CanvasGroup>().alpha = ShowAlpha;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            Hidden = false;
        }

        public virtual void Hide()
        {
            if (Hidden)
                return;
            GetComponent<CanvasGroup>().DOFade(0, Transition);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
            Hidden = true;
        }

        protected virtual void HideNoTransition()
        {
            if (!Hidden)
                return;
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            Hidden = false;
        }
    }
}