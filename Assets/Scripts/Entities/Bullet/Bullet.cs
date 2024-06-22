using System;

using UnityEngine;

using Random = UnityEngine.Random;

namespace EscapeGuan.Entities.Bullet
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : Entity
    {
        public override bool GuanAttackable => false;
        public override int InventoryLength => throw new Exception($"{Id} has no inventory!");

        public float Highest, InitialVelocity, Direction;

        public Vector2 DropPoint => new(Mathf.Sin(Mathf.Deg2Rad * Direction) * DropPointDistance, Mathf.Cos(Mathf.Deg2Rad * Direction) * DropPointDistance);

        public Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();

        public const float Gravity = 0.98f;

        private float DropPointDistance => 2 * InitialVelocity * Mathf.Sqrt(Highest / Gravity);

        private float ElapsedDistance;

        public override void Start()
        {
            base.Start();
            transform.eulerAngles = new(0, 0, Random.Range(0f, 360));
            Rigidbody.angularVelocity = Random.Range(45f, 135f);
            Rigidbody.velocity = new(Mathf.Sin(Mathf.Deg2Rad * Direction) * InitialVelocity, Mathf.Cos(Mathf.Deg2Rad * Direction) * InitialVelocity);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            ElapsedDistance += Rigidbody.velocity.magnitude * Time.fixedDeltaTime;
            if (ElapsedDistance >= DropPointDistance)
                Drop();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<Entity>() is Entity e)
                Hit(e);
        }

        public virtual void Drop()
        {
            Destroy(gameObject);
        }

        public virtual void Hit(Entity e)
        {
            Attack(e);
            Destroy(gameObject);
        }

        public override float GetAttackAmount() => (Random.value < CriticalRate ? CriticalMultiplier : 1) * AttackValue * Rigidbody.mass * Rigidbody.velocity.magnitude;
    }
}
