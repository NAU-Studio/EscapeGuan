using EscapeGuan.Entities;
using EscapeGuan.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class HealthBarBase : MonoBehaviour
{
    public Entity Target;
    public SlicedFilledImage Value;

    public TMP_Text ValueText, MaxText;

    protected virtual void Update()
    {
        Value.fillAmount = Target.HealthPoint / Target.MaxHealthPoint;
        ValueText.text = Target.HealthPoint.ToString("0");
        MaxText.text = Target.MaxHealthPoint.ToString("0");
    }

    public virtual void Initialize(Entity target)
    {
        Target = target;
    }
}
