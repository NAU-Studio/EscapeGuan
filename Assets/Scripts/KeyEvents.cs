using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace EscapeGuan
{
    public class KeyEvents : MonoBehaviour
    {
        public List<KeyEvent> Events;

        public void Update()
        {
            foreach (KeyEvent e in Events)
            {
                if (Input.GetKeyDown(e.Key))
                    e.Event.Invoke();
            }
        }
    }

    [Serializable]
    public class KeyEvent
    {
        public UnityEvent Event;
        public KeyCode Key;
    }
}
