using System.Collections;

using DG.Tweening;

using EscapeGuan.UI;

using TMPro;

using UnityEngine;

public class ItemProfileShower : HidableUI
{
    public float FadeTransition;
    public TMP_Text NameUI, DescriptionUI;

    private Tween tween;
    private Coroutine afterShowCo;

    public void SetText(string name, string description)
    {
        NameUI.text = name;
        DescriptionUI.text = description;

        transform.sizeDelta = new(Mathf.Max(NameUI.preferredWidth, DescriptionUI.preferredWidth), transform.sizeDelta.y);
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
