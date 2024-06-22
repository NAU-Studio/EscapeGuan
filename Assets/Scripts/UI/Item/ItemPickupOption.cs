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
            Icon.sprite = Target.item.Base.Icon;
            Text.text = Target.item.Base.Name;
        }

        public void Pickup()
        {
            if (!Destroyed)
            {
                Target.Pickup(GameManager.Player);
                Destroy();
            }
        }

        public void Show()
        {
            GetComponent<Hidable>().Show();
        }

        public void Destroy()
        {
            Destroyed = true;
            GetComponent<Hidable>().Hide();
            GameManager.DelayAction(this, () => { Destroy(gameObject); }, GetComponent<Hidable>().Transition + .1f);
        }
    }
}
