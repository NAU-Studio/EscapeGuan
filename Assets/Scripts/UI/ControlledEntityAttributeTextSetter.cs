using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace EscapeGuan.UI
{
    public class ControlledEntityAttributeTextSetter : MonoBehaviour
    {
        public List<ControlledAttributeGetterValue> Getters;

        public string Format;

        public void Update()
        {
            string t = Format;
            foreach (var getter in Getters)
                t = t.Replace(getter.KeyValue, getter.ToString());
            GetComponent<TMP_Text>().text = t;
        }
    }

    [Serializable]
    public class ControlledAttributeGetterValue
    {
        public string Name;
        public string KeyValue;

        public object Get()
        {
            return ((Delegate)GameManager.Main.EntityPool[GameManager.Main.ControlledEntityId].Attributes[Name]
                    .GetType()
                    .GetField("Getter")
                    .GetValue(GameManager.Main.EntityPool[GameManager.Main.ControlledEntityId].Attributes[Name]))
                    .DynamicInvoke();
        }

        public override string ToString()
        {
            return Get().ToString();
        }
    }

}
