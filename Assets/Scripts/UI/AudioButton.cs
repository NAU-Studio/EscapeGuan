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

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            GameManager.Main.PlayAudio(AudioSources.UI, "ui.button.hover");
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnClick.Invoke();
            GameManager.Main.PlayAudio(AudioSources.UI, "ui.button.click");
        }
    }
}
