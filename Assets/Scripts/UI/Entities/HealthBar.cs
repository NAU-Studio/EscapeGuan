using EscapeGuan.Entities;
using EscapeGuan.UI;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ObjectFollower), typeof(Hidable))]
public class HealthBar : MonoBehaviour
{
    public Entity Target;
    public SlicedFilledImage Value;

    public TMP_Text ValueText, MaxText;

    public ObjectFollower ObjectFollower => GetComponent<ObjectFollower>();
    public Hidable Hidable => GetComponent<Hidable>();

    private void Start()
    {
        Hidable.Show();
    }

    private void Update()
    {
        Value.fillAmount = Target.HealthPoint / Target.MaxHealthPoint;
        ValueText.text = Target.HealthPoint.ToString("0");
        MaxText.text = Target.HealthPoint.ToString("0");
    }

    public void Initialize(Entity target)
    {
        Target = target;
        ObjectFollower.Target = target.transform;
        ObjectFollower.Offset = target.HealthBarOffset;
    }
}
