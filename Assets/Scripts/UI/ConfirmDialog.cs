using UnityEngine.Events;

namespace EscapeGuan.UI
{
    public class ConfirmDialog : Hidable
    {
        public UnityEvent OnConfirm;
        public UnityEvent OnCancel;

        public void Confirm()
        {
            OnConfirm?.Invoke();
            Hide();
        }

        public void Cancel()
        {
            OnCancel?.Invoke();
            Hide();
        }
    }
}