using EscapeGuan;
using EscapeGuan.Entities;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RockProp : PropEntity
{
    public override bool GuanAttackable => true;

    protected override void Damage(float amount)
    {
        // 自己死不了（
    }

    public override void Damage(float amount, Entity sender)
    {
        Kill();
        Attack(sender);
    }

    public override void Kill()
    {
        Instantiate(GameManager.Templates["rock_destroy_particle"], transform.position, Quaternion.identity);
        base.Kill();
    }
}
