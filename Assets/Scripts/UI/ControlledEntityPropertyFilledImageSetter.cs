using System.Reflection;
using EscapeGuan;
using EscapeGuan.Entities;
using EscapeGuan.Entities.Player;
using EscapeGuan.UI;

using UnityEngine;

public class ControlledEntityPropertyFilledImageSetter : MonoBehaviour
{
    public string ValueProperty, MaxValueProperty;
    public SlicedFilledImage Image;

    public bool HideOnFull;

    private Hidable Hidable => GetComponent<Hidable>(); // 仅 HideOnFull 为 true 时允许使用
    private bool Shown = false;

    private FieldInfo value, maxValue;

    private void Start()
    {
        value = typeof(Player).GetField(ValueProperty);
        maxValue = typeof(Player).GetField(MaxValueProperty);
    }

    public void Update()
    {
        float v = 0;
        if (GameManager.Player != null)
        {
            if (value.FieldType == typeof(float))
                v = (float)value.GetValue(GameManager.Player);
            else if (value.FieldType == typeof(Modifiable))
                v = (Modifiable)value.GetValue(GameManager.Player);

            if (maxValue.FieldType == typeof(float))
                v /= (float)maxValue.GetValue(GameManager.Player);
            else if (maxValue.FieldType == typeof(Modifiable))
                v /= (Modifiable)maxValue.GetValue(GameManager.Player);
        }
        Image.SetFillAmount(v);
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
