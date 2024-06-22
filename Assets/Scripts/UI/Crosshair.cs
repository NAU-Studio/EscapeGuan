using UnityEngine;

namespace EscapeGuan.UI
{
    public class Crosshair : RectBehaviour
    {
        public RectTransform Parent;

        public float FloatingAmount = 0;

        private void Update()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, Input.mousePosition, Camera.main, out Vector2 pos);
            transform.anchoredPosition = pos + new Vector2(Random.Range(-FloatingAmount, FloatingAmount), Random.Range(-FloatingAmount, FloatingAmount));
        }
    }
}
