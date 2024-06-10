using EscapeGuan.UI;

using UnityEngine;
using UnityEngine.UI;

using static EscapeGuan.GameManager;

public class ControlledEntityAttributeImageSetter : MonoBehaviour
{
    public bool IsSlicedFillImage;
    public string AttributeName, MaxAttributeName;
    public SlicedFilledImage Image_SF;
    public Image Image_UNT;

    public void Update()
    {
        if (IsSlicedFillImage)
            Image_SF.fillAmount = Main.EntityPool[Main.ControlledEntityId].GetAttribute<float>(AttributeName) / Main.EntityPool[Main.ControlledEntityId].GetAttribute<float>(MaxAttributeName);
        else
            Image_UNT.fillAmount = Main.EntityPool[Main.ControlledEntityId].GetAttribute<float>(AttributeName) / Main.EntityPool[Main.ControlledEntityId].GetAttribute<float>(MaxAttributeName);
    }
}
