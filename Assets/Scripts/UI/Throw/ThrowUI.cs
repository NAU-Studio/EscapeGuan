using EscapeGuan.Entities.Bullet;
using UnityEngine;

namespace EscapeGuan.UI.Throw
{
    public class ThrowUI : MonoBehaviour
    {
        public string Text;
        public Bullet Template;
        public RectTransform RangeDisplayer;

        public bool Throwing = false;

        public float GetFloatingRange(float stability) => RangeDisplayer.anchoredPosition.magnitude * RangeDisplayer.anchoredPosition.magnitude / (stability * 100) + 10;

        private void Update()
        {
            if (Throwing)
            {
                float fr = GetFloatingRange(GameManager.Player.ThrowStability);
                GameManager.Main.Crosshair.FloatingAmount = fr / 2;
                RangeDisplayer.sizeDelta = new(fr, fr);
            }
        }
    }
}
