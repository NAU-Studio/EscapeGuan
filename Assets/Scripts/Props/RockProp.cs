using EscapeGuan;
using EscapeGuan.Entities.Props;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RockProp : PropEntity
{
    public override bool GuanAttackable => true;

    protected override string[] GetDamageSE() => new string[] { "entity.stone_1", "entity.stone_2", "entity.stone_3", "entity.stone_4" };

    public override void Kill()
    {
        GameManager.Main.PlayAudio(AudioSources.Prop, "se.rock_break");
        Instantiate(GameManager.Templates["rock_destroy_particle"], transform.position, Quaternion.identity);
        base.Kill();
    }
}
