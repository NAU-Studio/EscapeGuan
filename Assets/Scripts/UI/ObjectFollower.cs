using EscapeGuan.UI;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectFollower : RectBehaviour
{
    public Transform Target;
    public Vector3 Offset;

    private void Update()
    {
        if (!Target.IsDestroyed())
            transform.anchoredPosition = Camera.main.WorldToScreenPoint(Target.position + Offset);
    }
}
