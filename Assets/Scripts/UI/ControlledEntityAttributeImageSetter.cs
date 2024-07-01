using EscapeGuan;
using EscapeGuan.UI;

using UnityEngine;

public class ControlledEntityAttributeImageSetter : MonoBehaviour
{
    public string AttributeName, MaxAttributeName;
    public SlicedFilledImage Image;

    public void Update()
    {
        Image.fillAmount = GameManager.Player.GetAttribute<float>(AttributeName) / GameManager.Player.GetAttribute<float>(MaxAttributeName);
    }
}
