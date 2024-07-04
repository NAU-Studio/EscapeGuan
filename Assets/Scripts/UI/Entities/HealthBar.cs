using EscapeGuan.Entities;
using EscapeGuan.UI;

using UnityEngine;

[RequireComponent(typeof(ObjectFollower), typeof(Hidable))]
public class HealthBar : HealthBarBase
{
    public ObjectFollower ObjectFollower => GetComponent<ObjectFollower>();
    public Hidable Hidable => GetComponent<Hidable>();

    private void Start()
    {
        Hidable.Show();
    }

    public override void Initialize(Entity target)
    {
        base.Initialize(target);
        ObjectFollower.Target = target.transform;
        ObjectFollower.Offset = target.HealthBarOffset;
    }
}
