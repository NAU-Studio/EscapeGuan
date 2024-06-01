using System;
using System.Collections.Generic;

using EscapeGuan.Entities.Items;

using UnityEngine;

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
        public int EntityId;

        public List<ItemStack> Inventory = new();

        public virtual bool GuanAttackable => true;

        public virtual void Start()
        {
            EntityId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            RegisterEntity();
        }

        public virtual void RegisterEntity()
        {
            GameManager.Main.EntityPool.Add(EntityId, this);
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

        public void Attack(float amount, Vector2 direction, float knockback)
        {
            Damage(amount / (DefenseValue + 1));
            KnockbackVelocity += direction * knockback;
        }

        public virtual void FixedUpdate()
        {
            KnockbackVelocity = Vector2.Lerp(KnockbackVelocity, Vector2.zero, Drag);
            transform.Translate(KnockbackVelocity);
        }

        public virtual void Damage(float amount)
        {
            HealthPoint -= amount;
            DamageText dtx = Instantiate(GameManager.Main.DamageText, transform.position + Vector3.back + (Vector3)(Vector2.one * UnityEngine.Random.Range(-.1f, .1f)), Quaternion.identity).GetComponent<DamageText>();
            dtx.Value = amount;
            dtx.gameObject.SetActive(true);

            if (HealthPoint <= 0)
                Kill();
        }

        public virtual void Kill()
        {
            Destroy(gameObject);
        }

        public virtual void Health(float amount)
        {
            HealthPoint += amount * RecieveHealthMultiplier;
            DamageText dtx = Instantiate(GameManager.Main.DamageText, transform.position + Vector3.back + (Vector3)(Vector2.one * UnityEngine.Random.Range(-.1f, .1f)), Quaternion.identity).GetComponent<DamageText>();
            dtx.Value = amount;
            dtx.gameObject.SetActive(true);
        }

        public AttributeList Attributes = new();

        public virtual T GetAttribute<T>(string name, T value)
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

        public abstract void PickItem(ItemEntity sender);
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