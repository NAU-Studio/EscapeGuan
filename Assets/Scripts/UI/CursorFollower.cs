using EscapeGuan.UI;

using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollower : RectBehaviour
{
    protected RectTransform Parent => (RectTransform)GetComponentInParent<Canvas>().transform;

    protected virtual void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, Mouse.current.position.value, Camera.main, out Vector2 pos);
        transform.anchoredPosition = pos;
    }
}
