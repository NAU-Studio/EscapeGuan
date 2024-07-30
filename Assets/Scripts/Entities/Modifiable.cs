using System;
using System.Collections.Generic;

using UnityEngine;

namespace EscapeGuan.Entities
{
    [Serializable]
    public class Modifiable
    {
        public float BaseValue = default;
        [HideInInspector] public readonly Dictionary<string, Modifier> Modifiers = new();

        public float GetAppliedValue()
        {
            float ret = BaseValue;
            foreach (KeyValuePair<string, Modifier> m in Modifiers)
                ret = m.Value.GetAppliedValue(ret);
            return ret;
        }

        public static implicit operator Modifiable(float value) => new() { BaseValue = value };
        public static implicit operator float(Modifiable value) => value.GetAppliedValue();

        public void Add(string key, Modifier value) => Modifiers.Add(key, value);
        public void Remove(string key) => Modifiers.Remove(key);

        public override string ToString() => GetAppliedValue().ToString();
        public string ToString(string format) => GetAppliedValue().ToString(format);
    }

    public struct Modifier
    {
        public float Value;
        public bool IsMultiple;

        public Modifier(float value, bool isMultiple)
        {
            Value = value;
            IsMultiple = isMultiple;
        }

        public readonly float GetAppliedValue(float Base) => IsMultiple ? Base * Value : Base + Value;
    }
}