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

    public new Boss Target => (Boss)base.Target;

    public Hidable Hidable => GetComponent<Hidable>();

    protected override void Update()
    {
        if (Target == null || Target.IsDestroyed())
            Hidable.HideDestroy();
        base.Update();
        Name.text = NameFormat.Replace("Name", Target.BossName).Replace("Desc", Target.BossDescription);
    }
}
