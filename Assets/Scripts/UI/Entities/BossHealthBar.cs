using EscapeGuan.Entities.Enemy;
using EscapeGuan.UI;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;

[RequireComponent(typeof(Hidable))]
public class BossHealthBar : HealthBarBase
{
    public TMP_Text Name;
    [Multiline(3)]
    public string NameFormat;
    public Hidable Hidable => GetComponent<Hidable>();

    protected override void Update()
    {
        if (Target == null || Target.IsDestroyed())
            Hidable.HideDestroy();
        base.Update();
        if (Target is not IBoss)
            return;
        Name.text = NameFormat.Replace("Name", ((IBoss)Target).BossName).Replace("Desc", ((IBoss)Target).BossDescription);
    }
}
