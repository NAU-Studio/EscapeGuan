using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using EscapeGuan.Entities.Items;
using EscapeGuan.Items;
using EscapeGuan.UI;

using UnityEngine;

using Random = UnityEngine.Random;

namespace EscapeGuan.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [Header("Entity Base Attributes")]
        [Header("- Health Point")]
        public float HealthPoint;
        public Modifiable MaxHealthPoint;

        [Header("- Attack")]
        public Modifiable AttackValue;
        public Modifiable CriticalMultiplier = 1;
        public Modifiable CriticalRate = 1;
        public Modifiable Knockback = 1;

        [Header("- Health")]
        public Modifiable RecieveHealthMultiplier = 1;
        public float SendHealthValue = 1;

        [Header("- Defense")]
        public Modifiable DefenseValue;

        [Header("- Knockback")]
        public Vector2 KnockbackVelocity;

        [Header("* Not Attribute *")]
        public float Drag = .5f;
        public int Id;

        public ItemStackCollection Inventory = new();

        public Action OnKill = () => { };

        public abstract int InventoryLength { get; }
        public virtual bool EverythingAttackable => true;
        public virtual bool GuanAttackable => true;
        public virtual bool BulletHitable => true;
        public virtual bool ShowHealthBarAtTop => true;
        public virtual Vector3 HealthBarOffset => Vector3.zero;

        [HideInInspector] public float IntervalRemaining = 0;

        public virtual void Start()
        {
            Id = Random.Range(int.MinValue, int.MaxValue);
            RegisterEntity();

            try
            {
                for (int _ = 0; _ < InventoryLength; _++)
                    Inventory.Add(null);
            }
            catch { }
        }

        public virtual void RegisterEntity()
        {
            GameManager.EntityPool.Add(Id, this);
        }

        public virtual void Attack(Entity target)
        {
            Attack(target, GetAttackAmount());
        }

        public virtual void Attack(Entity target, float amount)
        {
            if (!target.EverythingAttackable)
                return;
            if (amount <= 0.1f)
                return;
            if (target.IntervalRemaining > 0)
                return;
            target.Damage(amount, this);
            target.KnockbackVelocity += (Vector2)(target.transform.position - transform.position).normalized * Knockback;
        }

        public virtual void FixedUpdate()
        {
            if (KnockbackVelocity != Vector2.zero)
            {
                KnockbackVelocity -= KnockbackVelocity * Drag;
                transform.Translate(KnockbackVelocity);
            }
        }

        protected virtual void Update()
        {
            if (IntervalRemaining > 0)
                IntervalRemaining -= Time.deltaTime;
        }

        public virtual float GetAttackAmount()
        {
            return Random.value < CriticalRate ? AttackValue * CriticalMultiplier : AttackValue;
        }

        public virtual float GetDamageAmount(float basev)
        {
            return 100 * basev / (DefenseValue + 100);
        }

        private bool HealthBarCreated = false;
        private HealthBar HealthBar;
        protected virtual void Damage(float amount)
        {
            IntervalRemaining = GameManager.DamageInterval;

            GameManager.Main.PlayRandomAudio(AudioSources.Player, 1, Random.Range(.8f, 1.2f), GetDamageSE());

            HealthPoint -= GetDamageAmount(amount);
            DamageText dtx = Instantiate(GameManager.Main.DamageText, transform.position + Vector3.back + (Vector3)(Vector2.one * Random.Range(-.1f, .1f)), Quaternion.identity).GetComponent<DamageText>();
            dtx.Value = GetDamageAmount(amount);
            dtx.gameObject.SetActive(true);
            OnHealthChange();

            if (ShowHealthBarAtTop && !HealthBarCreated)
            {
                HealthBar = Instantiate(GameManager.Templates["health_bar"], GameManager.Main.ObjectHUDContainer).GetComponent<HealthBar>();
                HealthBar.Initialize(this);
                HealthBarCreated = true;
            }

            if (HealthPoint <= 0)
                Kill();
        }

        protected virtual string[] GetDamageSE() => new string[] { "entity.damage_1", "entity.damage_2", "entity.damage_3" };

        protected virtual void Damage(float amount, Entity sender)
        {
            Damage(amount);
        }

        public virtual void Kill()
        {
            GameManager.EntityPool.Remove(Id);
            if (HealthBarCreated)
                HealthBar.Hidable.HideDestroy();
            Destroy(gameObject);
            OnKill();
        }

        public virtual void Health(float amount)
        {
            HealthPoint += amount * RecieveHealthMultiplier;
            DamageText dtx = Instantiate(GameManager.Main.DamageText, transform.position + Vector3.back + (Vector3)(Vector2.one * Random.Range(-.1f, .1f)), Quaternion.identity).GetComponent<DamageText>();
            dtx.Value = amount;
            dtx.gameObject.SetActive(true);
            OnHealthChange();
        }

        public virtual void PickItem(ItemEntity sender) { }
        public virtual void AddItem(ItemStack sender) { }

        // Events
        public virtual void OnHealthChange() { }
    }
}
[Serializable]
public class EntityCannotPickupException : Exception
{
    public EntityCannotPickupException() { }
    public EntityCannotPickupException(int id) : base(id.ToString()) { }
    protected EntityCannotPickupException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}
