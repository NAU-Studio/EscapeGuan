using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EscapeGuan.UI
{
    public class ColorButton : Button
    {
        public Graphic Target;
        public Color Normal, Hover, Down;

        public override void OnPointerEnter(PointerEventData e)
        {
            base.OnPointerEnter(e);
            Target.CrossFadeColor(Hover, Transition, false, true);
        }
        public override void OnPointerExit(PointerEventData e)
        {
            base.OnPointerExit(e);
            Target.CrossFadeColor(Normal, Transition, false, true);
        }
        public override void OnPointerDown(PointerEventData e)
        {
            base.OnPointerDown(e);
            Target.CrossFadeColor(Down, Transition, false, true);
        }
        public override void OnPointerUp(PointerEventData e)
        {
            base.OnPointerUp(e);
            Target.CrossFadeColor(Hover, Transition, false, true);
        }

        private void OnValidate()
        {
            Target.CrossFadeColor(Normal, Transition, false, true);
        }
    }
}