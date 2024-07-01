using System.Collections;

using DG.Tweening;

using TMPro;

using UnityEngine;

namespace EscapeGuan.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class DamageText : MonoBehaviour
    {
        public float Value;
        public bool IsHealth;

        public Material DamageMaterial, HealthMaterial;
        public Color DamageColor, HealthColor;

        public TMP_Text Text => GetComponent<TMP_Text>();

        private void Start()
        {
            if (IsHealth)
            {
                Text.color = HealthColor;
                Text.material = HealthMaterial;
            }
            else
            {
                Text.color = DamageColor;
                Text.material = DamageMaterial;
            }
            Text.text = Value.ToString("0");
            IEnumerator MoveAnimation()
            {
                transform.DOMoveY(transform.position.y + 3, 4).SetEase(Ease.Linear);
                transform.DOScale(2, .1f).SetEase(Ease.OutCubic);
                yield return new WaitForSecondsRealtime(.1f);
                transform.DOScale(1, .4f).SetEase(Ease.OutCubic);
                yield return new WaitForSecondsRealtime(1.6f);
                Text.DOFade(0, .3f);
                Destroy(gameObject, 1);
            }
            StartCoroutine(MoveAnimation());
        }
    }
}
