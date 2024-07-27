using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeGuan.UI
{
    public class Crosshair : CursorFollower
    {
        public TMP_Text VelocityText;

        public override bool ScreenSpace => true;

        public float Velocity
        {
            get
            {
                return (Mouse.current.position.value / ScaleFactor - new Vector2(Width / 2, Height / 2)).magnitude / (10 * Mathf.PI);
            }
        }

        public float Angle
        {
            get
            {
                if (CenterPos.x != 0)
                    return (CenterPos.x > 0 ? 90 : -90) - Mathf.Atan(CenterPos.y / CenterPos.x) * Mathf.Rad2Deg;
                else
                    return CenterPos.y < 0 ? 180 : 0;
            }
        }

        protected override void Update()
        {
            base.Update();
            float floating = -(76800 / (GameManager.Player.ThrowStability / 100 * (CenterPos.magnitude - 1281))) - 5000 / GameManager.Player.ThrowStability;
            transform.anchoredPosition += new Vector2(Random.Range(-floating / 2, floating / 2), Random.Range(-floating / 2, floating / 2));
            VelocityText.text = $"{Velocity:0.00} m/s";
        }
    }
}
