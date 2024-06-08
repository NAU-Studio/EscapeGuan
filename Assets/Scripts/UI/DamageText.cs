using System.Collections;

using DG.Tweening;

using TMPro;

using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float Value;
    public bool IsHealth;

    public Material DamageMaterial, HealthMaterial;
    public Color DamageColor, HealthColor;

    private void Start()
    {
        if (IsHealth)
        {
            GetComponent<TMP_Text>().color = HealthColor;
            GetComponent<TMP_Text>().material = HealthMaterial;
        }
        else
        {
            GetComponent<TMP_Text>().color = DamageColor;
            GetComponent<TMP_Text>().material = DamageMaterial;
        }
        GetComponent<TMP_Text>().text = Value.ToString("0.0");
        IEnumerator MoveAnimation()
        {
            transform.DOMoveY(transform.position.y + 3, 4).SetEase(Ease.Linear);
            transform.DOScale(2, .1f).SetEase(Ease.OutCubic);
            yield return new WaitForSecondsRealtime(.1f);
            transform.DOScale(1, .4f).SetEase(Ease.OutCubic);
            yield return new WaitForSecondsRealtime(1.6f);
            GetComponent<TMP_Text>().DOFade(0, .3f);
            Destroy(gameObject, 1);
        }
        StartCoroutine(MoveAnimation());
    }
}
