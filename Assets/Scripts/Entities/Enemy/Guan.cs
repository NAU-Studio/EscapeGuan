using System;
using System.Collections.Generic;
using System.Linq;

using EscapeGuan.Entities.Items;

using Pathfinding;

using UnityEngine;

using Random = UnityEngine.Random;

namespace EscapeGuan.Entities.Enemy
{
    [RequireComponent(typeof(AIDestinationSetter), typeof(AIPath), typeof(SpriteRenderer))]
    public class Guan : Entity
    {
        [Header("Guan Attributes")]
        public GameObject Emotion;
        public Transform Destinator;

        public float NoticeDistance, LoseDistance, MaxWanderDistance, AttackRange,
            WanderInterval, AttackInterval, WanderSpeed, AttackSpeed, MaxStamina, AttackStaminaCost, StaminaRestore,
            AttackDelay;

        [Header("Bottle")]
        public float ThrowBottleInterval;
        public int BottleCount = 5;
        [Header("Sprites")]
        public Sprite IdleSprite;
        public AnimationClip MovementClip;

        public GuanEmotionManager EmotionManager;
        public Status State = Status.Wander;

        public override bool GuanAttackable => false;

        public override int InventoryLength => 9;

        private AIDestinationSetter TargetDestinationSetter => GetComponent<AIDestinationSetter>();
        private AIPath TargetPath => GetComponent<AIPath>();

        private Animator MovementAnimator => GetComponent<Animator>();

        private Entity AttackTarget;



        private float Stamina, AttackTimer, DamageMultiplier = 1;
        private bool Attacking;

        public enum Status
        {
            Wander,
            Attack,
            Rest
        }

        public override void Start()
        {
            Stamina = MaxStamina;

            base.Start();
            TargetDestinationSetter.target = Destinator;
            TargetPath.maxSpeed = WanderSpeed;
            // Wander
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
            foreach (KeyValuePair<int, Entity> e in GameManager.EntityPool.Where((x) => x.Value.GuanAttackable && Vector3.Distance(transform.position, x.Value.transform.position) <= NoticeDistance))
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

            #region Rest
            if (State == Status.Attack)
            {
                Stamina -= AttackStaminaCost * Time.fixedDeltaTime;

                if (Stamina <= 0)
                {
                    Destinator.position = transform.position;
                    State = Status.Rest;
                    EmotionManager.ChangeEmotion(GuanEmotion.Rest);
                    DamageMultiplier = 0.5f;
                }
            }
            if (State != Status.Attack)
            {
                if (Stamina < MaxStamina)
                    Stamina += StaminaRestore * Time.fixedDeltaTime;
                else
                    Stamina = MaxStamina;

                if (State == Status.Rest && Stamina >= MaxStamina - Random.Range(0, MaxStamina / 5))
                {
                    State = Status.Attack;
                    DamageMultiplier = 1;
                }
            }
            #endregion

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

            #region Sprite
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
            Color rc = Gizmos.color;
            Gizmos.color = new(.5f, 0, 0);
            Gizmos.DrawWireSphere(transform.position, AttackRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, NoticeDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, LoseDistance);
            if (State != Status.Attack)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, Destinator.position);
            }
            Gizmos.DrawWireSphere(Destinator.position, .25f);
            Gizmos.color = rc;
        }

        public override float GetAttackAmount()
        {
            return Random.value < CriticalRate ? AttackValue * CriticalMultiplier * DamageMultiplier : AttackValue * DamageMultiplier;
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