using EscapeGuan.Items;

using TMPro;

using UnityEngine.UI;

namespace EscapeGuan.UI.Items
{
    public class InventorySlot : RectBehaviour
    {
        public Image Image;
        public TMP_Text Count;
        public SlicedFilledImage Durability;
        public Hidable DurabilityHidable;

        public ItemStack Item => GameManager.Player.Inventory[Index];
        public bool IsNull => Item == null;

        public int Index;

        private void Update()
        {
            if (Item != null)
            {
                Image.sprite = Item.Base.Icon;
                Image.color = new(1, 1, 1, 1);
                Count.text = Item.GetCountString();
            }
            else
            {
                Image.sprite = null;
                Image.color = new(1, 1, 1, 0);
                Count.text = "";
            }
            if (!IsNull)
            {
                float du = Item.Durability;
                if (du >= 1 && DurabilityShown)
                {
                    DurabilityHidable.Hide();
                    DurabilityShown = false;
                }
                if (du < 1 && !DurabilityShown)
                {
                    DurabilityHidable.Show();
                    DurabilityShown = true;
                }
                Durability.SetFillAmount(du);
            }
            else if (DurabilityShown)
            {
                DurabilityHidable.Hide();
                DurabilityShown = false;
            }
        }
        private bool DurabilityShown = false;

        public void Use()
        {
            Item?.Use(GameManager.Player);
        }
    }
}
