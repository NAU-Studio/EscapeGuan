using TMPro;
using UnityEngine;

public class BossHealthBar : HealthBar
{
    public TMP_Text Name;
    [Multiline(3)]
    public string NameFormat;

    public new Boss Target => (Boss)base.Target;

    protected override void Update()
    {
        base.Update();
        Name.text = NameFormat.Replace("Name", Target.BossName).Replace("Desc", Target.BossDescription);
    }
}
