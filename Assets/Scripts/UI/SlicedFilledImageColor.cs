using EscapeGuan.UI;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SlicedFilledImage))]
public class SlicedFilledImageColor : MonoBehaviour
{
    public Color LowColor, HighColor;
    public Graphic Background;

    public SlicedFilledImage SlicedFilledImage => GetComponent<SlicedFilledImage>();

    private void Update()
    {
        Color c = Color.Lerp(LowColor, HighColor, SlicedFilledImage.fillAmount);
        SlicedFilledImage.color = c;
        Background.color = c;
    }
}
