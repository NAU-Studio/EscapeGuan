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
        public float MaxHealthPoint;

        [Header("- Attack")]
        public float AttackValue;
        public float CriticalMultiplier = 1;
        public float CriticalRate = 1;
        public float Knockback = 1;

        [Header("- Health")]
        public float RecieveHealthMultiplier = 1;
        public float SendHealthValue = 1;

        [Header("- Defense")]
        public float DefenseValue;

        [Header("- Knockback")]
        public Vector2 KnockbackVelocity;

        [Header("* Not Attribute *")]
        public float Drag = .5f;
        public int Id;

        public ItemStackCollection Inventory = new();

        public Action OnKill = () => { };

        public abstract int InventoryLength { get; }
        public virtual bool GuanAttackable => true;
        public virtual bool BulletHitable => true;
        public virtual bool ShowHealthBarAtTop => true;
        public virtual Vector3 HealthBarOffset => Vector3.zero;

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
            Attributes.Add(new Attribute<float>("HealthPoint", () => HealthPoint, (x) => HealthPoint = x));
            Attributes.Add(new Attribute<float>("MaxHealthPoint", () => MaxHealthPoint, (x) => MaxHealthPoint = x));
            Attributes.Add(new Attribute<float>("AttackValue", () => AttackValue, (x) => AttackValue = x));
            Attributes.Add(new Attribute<float>("CriticalMultiplier", () => CriticalMultiplier, (x) => CriticalMultiplier = x));
            Attributes.Add(new Attribute<float>("CriticalRate", () => CriticalRate, (x) => CriticalRate = x));
            Attributes.Add(new Attribute<float>("Knockback", () => Knockback, (x) => Knockback = x));
            Attributes.Add(new Attribute<float>("RecieveHealthMultiplier", () => RecieveHealthMultiplier, (x) => RecieveHealthMultiplier = x));
            Attributes.Add(new Attribute<float>("SendHealthValue", () => SendHealthValue, (x) => SendHealthValue = x));
            Attributes.Add(new Attribute<float>("DefenseValue", () => DefenseValue, (x) => DefenseValue = x));
        }

        public virtual void Attack(Entity target)
        {
            target.Damage(GetAttackAmount(), this);
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
            HealthPoint -= GetDamageAmount(amount);
            DamageText dtx = Instantiate(GameManager.Main.DamageText, transform.position + Vector3.back + (Vector3)(Vector2.one * Random.Range(-.1f, .1f)), Quaternion.identity).GetComponent<DamageText>();
            dtx.Value = GetDamageAmount(amount);
            dtx.gameObject.SetActive(true);

            if (ShowHealthBarAtTop && !HealthBarCreated)
            {
                HealthBar = Instantiate(GameManager.Templates["health_bar"], GameManager.Main.ObjectHUDContainer).GetComponent<HealthBar>();
                HealthBar.Initialize(this);
                HealthBarCreated = true;
            }

            if (HealthPoint <= 0)
                Kill();
        }

        public virtual void Damage(float amount, Entity sender)
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
        }

        public AttributeList Attributes = new();

        public virtual T GetAttribute<T>(string name)
        {
            foreach (Attribute attre in Attributes)
            {
                if (attre.Name.Equals(name) && attre is Attribute<T> attr)
                    return attr.Getter();
            }
            throw new Exception("Attribute not found.");
        }

        public virtual void SetAttribute<T>(string name, T value)
        {
            foreach (Attribute attre in Attributes)
            {
                if (attre.Name.Equals(name) && attre is Attribute<T> attr)
                {
                    attr.Setter(value);
                    return;
                }
            }
            throw new Exception("Attribute not found.");
        }

        public virtual void PickItem(ItemEntity sender) { }
        public virtual void AddItem(ItemStack sender) { }
    }

    public abstract class Attribute
    {
        public string Name;
        public abstract Type Type { get; }
    }

    public class Attribute<T> : Attribute
    {
        public AttributeGetter<T> Getter;
        public AttributeSetter<T> Setter;

        public override Type Type => typeof(T);

        public Attribute(string name, AttributeGetter<T> getter, AttributeSetter<T> setter)
        {
            Name = name;
            Getter = getter;
            Setter = setter;
        }
    }

    public delegate T AttributeGetter<T>();
    public delegate void AttributeSetter<T>(T x);

    public class AttributeList : List<Attribute>
    {
        public int IndexOfName(string name)
        {
            for (int i = 0; i < Count; ++i)
                if (this[i].Name.Equals(name))
                    return i;
            return -1;
        }

        public bool ContainsName(string name)
        {
            return IndexOfName(name) != -1;
        }

        public Attribute this[string n]
        {
            get => this[IndexOfName(n)];
        }
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
