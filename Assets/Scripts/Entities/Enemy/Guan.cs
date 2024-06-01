using System;
using System.Collections;
using System.Collections.Generic;

using EscapeGuan.Entities.Items;

using Pathfinding;

using Unity.VisualScripting;

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
                Destinator.position = targetAttack.transform.position;
                targetPath.maxSpeed = AttackSpeed;
                if (Vector3.Distance(transform.position, targetAttack.transform.position) < AttackRange && CanAttack)
                    targetAttack.Attack(AttackValue, (targetAttack.transform.position - transform.position).normalized, Knockback);
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
            (float v, Transform t)? minDist = null;
            foreach (KeyValuePair<int, Entity> e in GameManager.Main.EntityPool)
            {
                if (!e.Value.GuanAttackable)
                    continue;
                Transform tr = e.Value.transform;
                float dist = Vector3.Distance(transform.position, tr.position);
                minDist = (Mathf.Min(dist), tr);
            }
            if (minDist == null)
                return;

            (float v, Transform t) = minDist.Value;
            if (v < NoticeDistance && State == Status.Wander)
            {
                State = Status.Attack;
                EmotionManager.ChangeEmotion(GuanEmotion.FindTarget);
                targetAttack = t.GetComponent<Entity>();
            }
            if (v > LoseDistance && State == Status.Attack)
            {
                State = Status.Wander;
                EmotionManager.ChangeEmotion(GuanEmotion.LoseTarget);
                Destinator.position = transform.position;
            }

            base.FixedUpdate();
        }

        private void OnDrawGizmos()
        {
            Color rc = Gizmos.color;
            if (GameManager.Main != null)
            foreach (KeyValuePair<int, Entity> e in GameManager.Main.EntityPool)
            {
                if (!e.Value.GuanAttackable)
                    continue;
                if (e.Value.IsDestroyed())
                    continue;
                Transform tr = e.Value.transform;
                (float v, Transform t) = (Vector3.Distance(transform.position, tr.position), tr);
                if (v < NoticeDistance)
                    Gizmos.color = Color.red;
                else if (v > LoseDistance)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, t.position);
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
            throw new NotImplementedException();
        }
    }
}