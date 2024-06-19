using EscapeGuan.Entities.Items;

using TMPro;

using UnityEngine.UI;

namespace EscapeGuan.UI.Item
{
    public class ItemPickupOption : RectBehaviour
    {
        public ItemEntity Target;
        public bool Enabled = true;

        public bool Destroyed = false;

        public Image Icon;
        public TMP_Text Text;

        private void Update()
        {
            Icon.sprite = Target.item.Icon;
            Text.text = Target.item.Base.Name;
        }

        public void Pickup()
        {
            if (!Destroyed)
            {
                Target.Pickup(GameManager.Main.EntityPool[GameManager.Main.ControlledEntityId]);
                Destroy();
            }
        }

        public void Show()
        {
            GetComponent<HidableUI>().Show();
        }

        public void Destroy()
        {
            Destroyed = true;
            GetComponent<HidableUI>().Hide();
            GameManager.DelayAction(this, () => { Destroy(gameObject); }, GetComponent<HidableUI>().Transition + .1f);
        }
    }
}
