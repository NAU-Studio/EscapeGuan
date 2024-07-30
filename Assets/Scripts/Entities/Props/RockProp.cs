using EscapeGuan;
using EscapeGuan.Entities.Props;
using EscapeGuan.Items;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RockProp : PropEntity
{
    protected override string[] GetDamageSE() => new string[] { "entity.stone_1", "entity.stone_2", "entity.stone_3", "entity.stone_4" };

    public override void Kill()
    {
        GameManager.Main.PlayAudio(AudioSources.Prop, "se.rock_break");
        Instantiate(GameManager.Templates["rock_destroy_particle"], transform.position, Quaternion.identity);
        Rigidbody2D r = ItemRegistry.Main.CreateItemStack("rock_sharp").CreateEntity().GetComponent<Rigidbody2D>();
        r.velocity = new(Random.Range(1, 3), Random.Range(1, 3));
        r.transform.position = transform.position;
        base.Kill();
    }
}
