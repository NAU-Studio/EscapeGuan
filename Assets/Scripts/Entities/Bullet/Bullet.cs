using System;

using UnityEngine;

using Random = UnityEngine.Random;

namespace EscapeGuan.Entities.Bullet
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Bullet : Entity
    {
        public override bool GuanAttackable => false;
        public override int InventoryLength => throw new Exception($"{Id} has no inventory!");

        public float Highest, InitialVelocity, Direction;
        public Vector2 DropPoint => new(Mathf.Sin(Mathf.Deg2Rad * Direction) * DropPointDistance, Mathf.Cos(Mathf.Deg2Rad * Direction) * DropPointDistance);
        public Entity Thrower;
        public Transform Shadow;

        public Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();

        public const float Gravity = 9.8f;

        public override bool BulletHitable => false;

        private float DropPointDistance => 2 * InitialVelocity * Mathf.Sqrt(Highest / Gravity);

        private float ElapsedDistance;

        protected float ZCoord => -Gravity * Mathf.Pow(ElapsedDistance / InitialVelocity - Mathf.Sqrt(Highest / Gravity), 2) + Highest;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Rigidbody.velocity = new(Mathf.Sin(Mathf.Deg2Rad * Direction) * InitialVelocity, Mathf.Cos(Mathf.Deg2Rad * Direction) * InitialVelocity);
            ElapsedDistance += Rigidbody.velocity.magnitude * Time.fixedDeltaTime;
            if (ElapsedDistance >= DropPointDistance)
                Drop();

            Shadow.transform.localScale = new(ZCoord / 2, ZCoord / 2, 1);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Entity>() is Entity e && e.BulletHitable && e != Thrower)
                Hit(e);
        }

        public virtual void Drop()
        {
            Kill();
        }

        public virtual void Hit(Entity e)
        {
            Attack(e);
            Drop();
        }

        public override float GetAttackAmount() => (Random.value < Thrower.CriticalRate ? Thrower.CriticalMultiplier : 1) * Thrower.AttackValue * Rigidbody.mass * Rigidbody.velocity.magnitude;

        public void Init(Entity thrower, float initvel, float ang)
        {
            Thrower = thrower;
            InitialVelocity = initvel;
            Direction = ang;
        }
    }
}
