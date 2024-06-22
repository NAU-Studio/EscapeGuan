using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace EscapeGuan.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class Button : Selectable
    {
        public class OnClickEvent : UnityEvent
        { }

        public OnClickEvent OnClick = new();

        public AudioClip Hover, Click;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            GameManager.Main.UIEffect.clip = Hover;
            GameManager.Main.UIEffect.Play();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnClick.Invoke();
            GameManager.Main.UIEffect.clip = Click;
            GameManager.Main.UIEffect.Play();
        }
    }
}
