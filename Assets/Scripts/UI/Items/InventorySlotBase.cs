using EscapeGuan.Items;

using TMPro;

using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EscapeGuan.UI.Items
{
    public abstract class InventorySlotBase : RectBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Image Image;
        public TMP_Text Count;
        public SlicedFilledImage Durability;
        public Hidable DurabilityHidable => Durability.GetComponentInParent<Hidable>();

        public abstract ItemStack Item { get; }
        public virtual bool IsNull => Item == null;

        protected bool DurabilityShown = false;

        protected virtual void Update()
        {
            if (!IsNull)
            {
                Image.sprite = Item.Base.Icon;
                Image.color = new(1, 1, 1, 1);
                Count.text = Item.GetCountString();
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
            else
            {
                Image.sprite = null;
                Image.color = new(1, 1, 1, 0);
                Count.text = "";
                if (DurabilityShown)
                {
                    DurabilityHidable.Hide();
                    DurabilityShown = false;
                }
            }
        }

        public abstract void SetItem(ItemStack i = null);

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEvent(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEvent(null);
        }

        public MouseEventHandler MouseEvent = x => { };

        public delegate void MouseEventHandler(InventorySlotBase sender);
    }
}
