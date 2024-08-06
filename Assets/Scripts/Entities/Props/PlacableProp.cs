using EscapeGuan.Items;
using UnityEngine;

namespace EscapeGuan.Entities.Props
{
    public class PlacableProp : PropEntity
    {
        protected override void OnCollisionStay2D(Collision2D collision)
        { }

        public override void Kill()
        {
            for (int i = 0; i < 3; i++)
            {
                ItemStack s = ItemRegistry.Main.CreateItemStack("small_stick", 1);
                Rigidbody2D rig = s.CreateEntity().GetComponent<Rigidbody2D>();
                rig.velocity = new(Random.Range(1, 10), Random.Range(1, 10));
                rig.transform.position = transform.position;
                rig.transform.eulerAngles = new(0, 0, Random.Range(0, 360f));
            }
            base.Kill();
        }

        public override void FixedUpdate()
        {
            // NO KNOCKBACK!!!!!!!!!!!!!
        }
    }
}