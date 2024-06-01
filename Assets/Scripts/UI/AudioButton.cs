using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EscapeGuan.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioButton : Button
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            GetComponent<AudioSource>().Play();
        }
    }

}
