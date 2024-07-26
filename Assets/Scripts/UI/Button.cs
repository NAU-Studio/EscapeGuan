using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EscapeGuan.UI
{
    public abstract class Button : RectBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public float Transition;

        public string HoverAudio, ClickAudio;

        public UnityEvent OnClick;

        public virtual void OnPointerEnter(PointerEventData e)
        {
            GameManager.Main.PlayAudio(AudioSources.UI, HoverAudio);
        }

        public virtual void OnPointerExit(PointerEventData e) { }

        public virtual void OnPointerDown(PointerEventData e) { }

        public virtual void OnPointerUp(PointerEventData e)
        {
            GameManager.Main.PlayAudio(AudioSources.UI, ClickAudio);
            OnClick.Invoke();
        }
    }
}