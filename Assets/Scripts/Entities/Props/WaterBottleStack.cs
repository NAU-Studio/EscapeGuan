using EscapeGuan.Entities.Props;
using EscapeGuan.Items;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WaterBottleStack : PropEntity
{
    protected override string[] GetDamageSE() => new string[] { "entity.plastic_1", "entity.plastic_2", "entity.plastic_3" };

    protected override void OnCollisionStay2D(Collision2D collision)
    { }

    public override void Kill()
    {
        // 5 * 7 = 35
        for (int i = 0; i < 35; i++)
        {
            ItemStack s = ItemRegistry.Main.CreateItemStack("water_bottle");
            Rigidbody2D rig = s.CreateEntity().GetComponent<Rigidbody2D>();
            rig.velocity = new(Random.Range(1, 5), Random.Range(1, 5));
            rig.transform.position = transform.position;
            rig.transform.eulerAngles = new(0, 0, Random.Range(0, 360f));
        }
        base.Kill();
    }
}
