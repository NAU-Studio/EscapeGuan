using EscapeGuan.Entities;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RockProp : PropEntity
{
    public GameObject ParticleTemplate;

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
        GameObject g = Instantiate(ParticleTemplate, transform.position, Quaternion.identity);
        g.GetComponent<ParticleSystem>().Play();
        base.Kill();
    }
}
