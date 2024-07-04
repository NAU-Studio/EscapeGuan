using System.Collections;

using DG.Tweening;

using TMPro;

using UnityEngine;

namespace EscapeGuan.UI.Items
{
    public class ItemProfileShower : Hidable
    {
        public float FadeTransition;
        public TMP_Text NameUI, DescriptionUI;

        private Tween tween;
        private Coroutine afterShowCo;

        public void SetText(string name, string description)
        {
            NameUI.text = name;
            DescriptionUI.text = description;
            Show();
        }

        public override void Show()
        {
            base.Show();
            tween?.Kill();
            if (afterShowCo != null)
                StopCoroutine(afterShowCo);
            afterShowCo = StartCoroutine(AfterShow());
        }

        public IEnumerator AfterShow()
        {
            yield return new WaitForSeconds(2);
            Hide();
        }

        public override void Hide()
        {
            tween = GetComponent<CanvasGroup>().DOFade(0, FadeTransition);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
        }
    }

}
