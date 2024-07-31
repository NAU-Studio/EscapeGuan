using System;
using System.Collections.Generic;
using System.Linq;

using EscapeGuan.Entities.Items;
using EscapeGuan.Items;

using Pathfinding;

using UnityEngine;

using Random = UnityEngine.Random;

namespace EscapeGuan.Entities.Enemy
{
    [RequireComponent(typeof(AIDestinationSetter), typeof(AIPath), typeof(SpriteRenderer))]
    public class Guan : Entity, IBoss
    {
        [Header("Guan Attributes")]
        public Transform Destinator;

        public float NoticeDistance, LoseDistance, MaxWanderDistance, AttackRange,
            WanderInterval, AttackInterval, WanderSpeed, AttackSpeed, MaxStamina, AttackStaminaCost, StaminaRestore,
            AttackDelay;

        public GuanEmotionManager EmotionManager;
        public Status State = Status.Wander;
        public ParticleSystem BloodDropParticle;

        public override bool GuanAttackable => false;
        public override int InventoryLength => 9;
        public override bool ShowHealthBarAtTop => false;
        public string BossName => "管哥";
        public string BossDescription => "我勒个臭管啊";

        private AIDestinationSetter TargetDestinationSetter => GetComponent<AIDestinationSetter>();
        private AIPath TargetPath => GetComponent<AIPath>();

        private Animator MovementAnimator => GetComponent<Animator>();

        private Entity AttackTarget;
        private float AttackTimer;
        private bool Attacking;

        public enum Status
        {
            Wander,
            Attack,
            Rest
        }

        public override void Start()
        {
            base.Start();
            TargetDestinationSetter.target = Destinator;
            TargetPath.maxSpeed = WanderSpeed;
            GameManager.IntervalAction(this, () => State == Status.Wander, () =>
            {
                TargetPath.maxSpeed = WanderSpeed;
                Destinator.position = transform.position + new Vector3(Random.Range(-MaxWanderDistance, MaxWanderDistance),
                    Random.Range(-MaxWanderDistance, MaxWanderDistance));
            }, WanderInterval);
        }

        public override void FixedUpdate()
        {
            Transform nearest = null;
            float nd = float.MaxValue;
            foreach (KeyValuePair<int, Entity> e in GameManager.EntityPool.Where((x) => x.Value.EverythingAttackable && x.Value.GuanAttackable && Vector3.Distance(transform.position, x.Value.transform.position) <= NoticeDistance))
            {
                if (Vector3.Distance(transform.position, e.Value.transform.position) < nd)
                {
                    nd = Vector3.Distance(transform.position, e.Value.transform.position);
                    nearest = e.Value.transform;
                }
                TargetPath.maxSpeed = AttackSpeed;
            }
            if (nearest == null)
                goto SkipNearest;
            if (State == Status.Wander)
            {
                State = Status.Attack;
                EmotionManager.ChangeEmotion(GuanEmotion.FindTarget);
            }
            AttackTarget = nearest.GetComponent<Entity>();

        SkipNearest:
            if (AttackTarget != null && Vector3.Distance(transform.position, AttackTarget.transform.position) > LoseDistance)
            {
                EmotionManager.ChangeEmotion(GuanEmotion.LoseTarget);
                AttackTarget = null;
            }

            if (AttackTarget == null)
            {
                State = Status.Wander;
                Destinator.position = transform.position;
            }
            if (AttackTarget != null && State == Status.Attack)
                Destinator.position = AttackTarget.transform.position;

            #region Normal Attack
            if (State == Status.Attack | State == Status.Rest)
            {
                if (!Attacking && Vector3.Distance(transform.position, AttackTarget.transform.position) <= NoticeDistance)
                {
                    Attacking = true;
                    AttackTimer = 0;
                }
                if (Attacking && Vector3.Distance(transform.position, AttackTarget.transform.position) > NoticeDistance)
                    Attacking = false;
                if (Attacking)
                {
                    AttackTimer -= Time.fixedDeltaTime;
                    if (AttackTimer <= 0 && Vector3.Distance(transform.position, AttackTarget.transform.position) <= AttackRange)
                    {
                        Attack(AttackTarget);
                        AttackTimer = AttackDelay;
                    }
                }
            }
            #endregion

            #region Motion
            Vector2 v = TargetPath.velocity;
            MovementAnimator.SetBool("Moving", v.x != 0 && v.y != 0);
            if (v.x < 0)
                transform.localScale = Vector3.one;
            if (v.x > 0)
                transform.localScale = new(-1, 1, 1);
            #endregion
            base.FixedUpdate();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new(.5f, 0, 0);
            Gizmos.DrawWireSphere(transform.position, AttackRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, NoticeDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, LoseDistance);
        }

        protected override void Damage(float amount)
        {
            base.Damage(amount);
            BloodDropParticle.Emit((int)(amount / 100));
        }

        public override float GetAttackAmount()
        {
            return (Random.value < CriticalRate ? CriticalMultiplier : 1) * AttackValue;
        }

        public override void PickItem(ItemEntity sender)
        {
            throw new EntityCannotPickupException();
        }

        public override void AddItem(ItemStack sender)
        {
            throw new NotImplementedException();
        }
    }
}