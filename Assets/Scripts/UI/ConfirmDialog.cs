using UnityEngine;
using UnityEngine.Events;

namespace EscapeGuan.UI
{
    public class ConfirmDialog : HidableUI
    {
        public UnityEvent OnConfirm;
        public UnityEvent OnCancel;

        public AudioSource AudioSource;
        public AudioClip ConfirmAudio, CancelAudio;

        public void Confirm()
        {
            OnConfirm?.Invoke();
            AudioSource.clip = ConfirmAudio;
            AudioSource.Play();
            Hide();
        }

        public void Cancel()
        {
            OnCancel?.Invoke();
            AudioSource.clip = CancelAudio;
            AudioSource.Play();
            Hide();
        }
    }
}