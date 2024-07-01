using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace EscapeGuan.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Hidable : RectBehaviour
    {
        public float Transition;
        [Range(0, 1)]
        public float ShowAlpha = 1;

        public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();

        public virtual void Show()
        {
            CanvasGroup.DOFade(ShowAlpha, Transition);
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
        }

        protected virtual void ShowNoTransition()
        {
            CanvasGroup.alpha = ShowAlpha;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
        }

        public virtual void Hide()
        {
            CanvasGroup.DOFade(0, Transition);
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;
        }

        protected virtual void HideNoTransition()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
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