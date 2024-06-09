using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;

using UnityEngine;

public abstract class PropEntity : Entity
{
    public bool Breakable;
    public float BreakForce;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.velocity.magnitude * collision.rigidbody.mass >= BreakForce)
        {
            Kill();
            if (collision.gameObject.GetComponent<Entity>() != null)
                Attack(collision.gameObject.GetComponent<Entity>());
        }
    }

    public override void PickItem(ItemEntity sender)
    {
        throw new EntityCannotPickupException(EntityId);
    }
}
