using EscapeGuan.UI;
using UnityEngine;

public class RectSmoothFollower : RectBehaviour
{
    public float FollowSpeed;

    public RectTransform Target;

    public Vector2 Offset;

    private void Update()
    {
        transform.anchoredPosition = Vector2.Lerp(transform.anchoredPosition, Target.anchoredPosition + Offset, FollowSpeed * Time.deltaTime);
    }
}
