using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeGuan.UI
{
    public class Crosshair : RectBehaviour
    {
        public RectTransform Parent;

        public TMP_Text VelocityText;


        public float Velocity
        {
            get
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, Mouse.current.position.value, Camera.main, out Vector2 pos);
                return pos.magnitude / (10 * Mathf.PI);
            }
        }

        private void Update()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, Mouse.current.position.value, Camera.main, out Vector2 pos);
            float floating = -(1920 / (GameManager.Player.ThrowStability / 10)) * (1920 / (pos.magnitude - 1920) + 1);
            transform.anchoredPosition = pos + new Vector2(Random.Range(-floating / 2, floating / 2), Random.Range(-floating / 2, floating / 2));
            VelocityText.text = $"速度：{pos.magnitude / (10 * Mathf.PI):0.00} m*s^(-1)";
        }
    }
}
