using EscapeGuan;
using EscapeGuan.UI;

using UnityEngine;

public class ControlledEntityAttributeImageSetter : MonoBehaviour
{
    public string AttributeName, MaxAttributeName;
    public SlicedFilledImage Image;

    public bool HideOnFull;

    private Hidable Hidable => GetComponent<Hidable>(); // 仅 HideOnFull 为 true 时允许使用
    private bool Shown = false;

    public void Update()
    {
        float value = 0;
        if (GameManager.Player != null)
            value = GameManager.Player.GetAttribute<float>(AttributeName) / GameManager.Player.GetAttribute<float>(MaxAttributeName);
        Image.SetFillAmount(value);
        if (HideOnFull && Shown && Image.fillAmount >= 1)
        {
            Shown = false;
            Hidable.Hide();
        }
        if (HideOnFull && !Shown && Image.fillAmount < 1)
        {
            Shown = true;
            Hidable.Show();
        }
    }
}
