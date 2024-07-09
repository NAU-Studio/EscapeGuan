using System.Collections;

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

        public bool ToggleBlockRaycasts = true, ToggleInteractable = true;

        public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();

        public virtual void Show()
        {
            CanvasGroup.DOFade(ShowAlpha, Transition);
            if (ToggleBlockRaycasts)
                CanvasGroup.blocksRaycasts = true;
            if (ToggleInteractable)
                CanvasGroup.interactable = true;
        }

        protected virtual void ShowNoTransition()
        {
            CanvasGroup.alpha = ShowAlpha;
            if (ToggleBlockRaycasts)
                CanvasGroup.blocksRaycasts = true;
            if (ToggleInteractable)
                CanvasGroup.interactable = true;
        }

        public virtual void Hide()
        {
            CanvasGroup.DOFade(0, Transition);
            if (ToggleBlockRaycasts)
                CanvasGroup.blocksRaycasts = false;
            if (ToggleInteractable)
                CanvasGroup.interactable = false;
        }

        protected virtual void HideNoTransition()
        {
            CanvasGroup.alpha = 0;
            if (ToggleBlockRaycasts)
                CanvasGroup.blocksRaycasts = false;
            if (ToggleInteractable)
                CanvasGroup.interactable = false;
        }

        public virtual void HideDestroy()
        {
            Hide();
            StartCoroutine(DestroyAfterHide());
        }

        private IEnumerator DestroyAfterHide()
        {
            yield return new WaitForSeconds(Transition);
            Destroy(gameObject);
        }
    }
}