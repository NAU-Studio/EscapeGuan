using System.Collections;
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

        public enum Status
        {
            Wander,
            Attack,
            Rest 
        }

        public GuanEmotionManager EmotionManager;

        [ReadOnly]
        public Status State = Status.Wander;

        public override bool GuanAttackable => false;

        [Header("Sprites")]
        public Sprite Front, Right, Back, Left;

        private AIDestinationSetter targetDestinationSetter => GetComponent<AIDestinationSetter>();
        private AIPath targetPath => GetComponent<AIPath>();
        private SpriteRenderer sprite => GetComponent<SpriteRenderer>();
        private Entity targetAttack;

        [SerializeField, ReadOnly]
        private float Stamina, AttackTimer, DamageMultiplier = 1;
        [SerializeField, ReadOnly]
        private bool Attacking;

        public override void Start()
        {
            Stamina = MaxStamina;

            base.Start();
            targetDestinationSetter.target = Destinator;
            targetPath.maxSpeed = WanderSpeed;
            // Wander
            GameManager.IntervalAction(this, () => State == Status.Wander, () =>
            {
                targetPath.maxSpeed = WanderSpeed;
                Destinator.position = transform.position + new Vector3(Random.Range(-MaxWanderDistance, MaxWanderDistance),
                    Random.Range(-MaxWanderDistance, MaxWanderDistance));
            }, WanderInterval);
        }

        public override void FixedUpdate()
        {
            Transform nearest = null;
            float nd = float.MaxValue;
            foreach (KeyValuePair<int, Entity> e in GameManager.Main.EntityPool.Where(
                (x) => x.Value.GuanAttackable && Vector3.Distance(transform.position, x.Value.transform.position) <= NoticeDistance))
            {
                if (Vector3.Distance(transform.position, e.Value.transform.position) < nd)
                {
                    nd = Vector3.Distance(transform.position, e.Value.transform.position);
                    nearest = e.Value.transform;
                }
                targetPath.maxSpeed = AttackSpeed;
            }
            if (nearest == null)
                goto SkipNearest;
            if (State == Status.Wander)
            {
                State = Status.Attack;
                EmotionManager.ChangeEmotion(GuanEmotion.FindTarget);
                targetAttack = nearest.GetComponent<Entity>();
            }

            SkipNearest:
            if (targetAttack != null && Vector3.Distance(transform.position, targetAttack.transform.position) > LoseDistance)
            {
                EmotionManager.ChangeEmotion(GuanEmotion.LoseTarget);
                targetAttack = null;
            }

            if (targetAttack == null)
            {
                State = Status.Wander;
                Destinator.position = transform.position;
            }
            if (targetAttack != null && State == Status.Attack)
                Destinator.position = targetAttack.transform.position;

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
                if (!Attacking && Vector3.Distance(transform.position, targetAttack.transform.position) <= NoticeDistance)
                {
                    Attacking = true;
                    AttackTimer = 0;
                }
                if (Attacking && Vector3.Distance(transform.position, targetAttack.transform.position) > NoticeDistance)
                    Attacking = false;
                if (Attacking)
                {
                    AttackTimer -= Time.fixedDeltaTime;
                    if (AttackTimer <= 0 && Vector3.Distance(transform.position, targetAttack.transform.position) <= AttackRange)
                    {
                        Attack(targetAttack);
                        AttackTimer = AttackDelay;
                    }
                }
            }
            #endregion

            #region Sprite
            Vector2 v = targetPath.velocity;
            float dir = Mathf.Rad2Deg * Mathf.Atan(-(v.y / v.x));
            if (v.x > 0)
            {
                if (dir > -45 && dir < 45)
                    sprite.sprite = Right;
                if (dir > 45)
                    sprite.sprite = Front;
                if (dir < -45)
                    sprite.sprite = Back;
            }
            if (v.x < 0)
            {
                if (dir > -45 && dir < 45)
                    sprite.sprite = Left;
                if (dir > 45)
                    sprite.sprite = Back;
                if (dir < -45)
                    sprite.sprite = Front;
            }
            #endregion
            base.FixedUpdate();
        }

        private void OnDrawGizmos()
        {
            Color rc = Gizmos.color;
            if (GameManager.Main != null)
            foreach (KeyValuePair<int, Entity> e in GameManager.Main.EntityPool.Where(
                (x) => x.Value.GuanAttackable && Vector3.Distance(transform.position, x.Value.transform.position) < NoticeDistance))
            {
                Transform tr = e.Value.transform;
                float d = Vector3.Distance(transform.position, tr.position);
                if (d < NoticeDistance)
                    Gizmos.color = Color.red;
                else if (d > LoseDistance)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, tr.position);
            }
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
    }
}