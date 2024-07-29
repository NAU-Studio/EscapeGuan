using EscapeGuan.UI;

using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : RectBehaviour
{
    protected RectTransform Parent => (RectTransform)GetComponentInParent<Canvas>().transform;

    public virtual bool ScreenSpace => false;

    protected float ScaleFactor => GetComponentInParent<Canvas>().scaleFactor;
    protected float Width => Parent.sizeDelta.x;
    protected float Height => Parent.sizeDelta.y;
    protected Vector2 CenterPos => Mouse.current.position.value / ScaleFactor - new Vector2(Width / 2, Height / 2);

    protected virtual void Update()
    {
        Vector2 pos;
        if (ScreenSpace)
            pos = Mouse.current.position.value / ScaleFactor;
        else
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, Mouse.current.position.value, Camera.main, out pos);
        transform.anchoredPosition = pos;
    }
}
