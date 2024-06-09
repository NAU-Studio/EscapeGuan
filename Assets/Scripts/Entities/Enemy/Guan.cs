using System.Collections;
using System.Collections.Generic;
using System.Linq;

using EscapeGuan.Entities.Items;

using Pathfinding;

using UnityEngine;

using Random = UnityEngine.Random;

namespace EscapeGuan.Entities.Enemy
{
    [RequireComponent(typeof(AIDestinationSetter), typeof(AIPath))]
    public class Guan : Entity
    {
        [Header("Guan Attributes")]
        public GameObject Emotion;
        public Transform Destinator;

        public float NoticeDistance, LoseDistance, MaxWanderDistance, AttackRange,
            WanderInterval, AttackInterval, WanderSpeed, AttackSpeed, RestDuration, RestInterval;

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

        public bool CanAttack = true;

        [ReadOnly]
        public Status State = Status.Wander;

        public override bool GuanAttackable => false;

        private AIDestinationSetter targetDestinationSetter;
        private AIPath targetPath;
        private Entity targetAttack;

        public override void Start()
        {
            base.Start();
            targetDestinationSetter = GetComponent<AIDestinationSetter>();
            targetPath = GetComponent<AIPath>();
            targetDestinationSetter.target = Destinator;
            targetPath.maxSpeed = WanderSpeed;
            // Wander
            GameManager.IntervalAction(this, () => State == Status.Wander, () =>
            {
                targetPath.maxSpeed = WanderSpeed;
                Destinator.position = transform.position + new Vector3(Random.Range(-MaxWanderDistance, MaxWanderDistance),
                    Random.Range(-MaxWanderDistance, MaxWanderDistance));
            }, WanderInterval);

            // Attack
            GameManager.IntervalAction(this, () => State == Status.Attack, () =>
            {
                targetPath.maxSpeed = AttackSpeed;
                if (Vector3.Distance(transform.position, targetAttack.transform.position) < AttackRange && CanAttack)
                    Attack(targetAttack);
            }, AttackInterval);

            IEnumerator rest()
            {
                EmotionManager.ChangeEmotion(GuanEmotion.Rest);
                Status prev = State;
                State = Status.Rest;
                Destinator.position = transform.position;
                yield return new WaitForSeconds(RestDuration);
                State = prev;
                GameManager.IntervalAction(this, () => State == Status.Attack, rest, AttackInterval);
            }
        }

        public override void FixedUpdate()
        {
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
            if (targetAttack != null)
                Destinator.position = targetAttack.transform.position;
            base.FixedUpdate();
            Transform nearest = null;
            float nd = float.MaxValue;
            foreach (KeyValuePair<int, Entity> e in GameManager.Main.EntityPool.Where(
                (x) => x.Value.GuanAttackable && Vector3.Distance(transform.position, x.Value.transform.position) < NoticeDistance))
            {
                if (Vector3.Distance(transform.position, e.Value.transform.position) < nd)
                {
                    nd = Vector3.Distance(transform.position, e.Value.transform.position);
                    nearest = e.Value.transform;
                }
            }
            if (nearest == null)
                return;
            if (State == Status.Wander)
            {
                State = Status.Attack;
                EmotionManager.ChangeEmotion(GuanEmotion.FindTarget);
                targetAttack = nearest.GetComponent<Entity>();
            }
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

        public override void PickItem(ItemEntity sender)
        {
            throw new EntityCannotPickupException();
        }
    }
}