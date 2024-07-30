using System;
using System.Collections.Generic;
using System.Reflection;
using EscapeGuan.Entities.Player;
using TMPro;

using UnityEngine;

namespace EscapeGuan.UI
{
    public class ControlledEntityPropertyTextSetter : MonoBehaviour
    {
        public List<string> Properties;

        public string Format;

        private List<FieldInfo> fields = new();

        private void OnEnable()
        {
            foreach (string s in Properties)
                fields.Add(typeof(Player).GetField(s));
        }

        public void Update()
        {
            string t = Format;
            for (int i = 0; i < Properties.Count; i++)
                t = t.Replace($"{{{Properties[i]}}}", GameManager.Player == null ? "已死亡" : fields[i].GetValue(GameManager.Player).ToString());
            GetComponent<TMP_Text>().text = t;
        }
    }
}
