using System.Collections.Generic;

using DG.Tweening;

using EscapeGuan.Entities.Items;

using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeGuan.UI.Player
{
    public class QuickInventoryShower : MonoBehaviour
    {
        [SerializeField]
        List<QuickInventorySlot> Slots;
        ItemStack prevItem;

        public RectTransform SelectionBox;

        public int Selection;

        public void Set(int index, ItemStack item)
        {
            Slots[index].Item = item;
        }

        public bool Add(ItemStack item)
        {
            foreach (QuickInventorySlot s in Slots)
            {
                if (s.Item == null)
                {
                    s.Item = item;
                    return true;
                }
                if (s.Item.Combine(item))
                    return true;
            }
            return false;
        }

        private void Update()
        {
            foreach (QuickInventorySlot s in Slots)
                s.Item = GameManager.Player.Inventory[s.Index];

            float scr = Input.mouseScrollDelta.y;
            if (scr > 0)
            {
                Selection++;
                if (Selection >= Slots.Count)
                    Selection = 0;
            }
            if (scr < 0)
            {
                Selection--;
                if (Selection < 0)
                    Selection = Slots.Count - 1;
            }
            if (scr != 0)
                SelectionBox.DOAnchorPosX(Selection * 48, .1f).SetEase(Ease.OutCubic);

            if (prevItem != Slots[Selection].Item)
            {
                prevItem = Slots[Selection].Item;
                GameManager.Main.ItemProfile.SetText(Slots[Selection].Item.Base.Name, Slots[Selection].Item.Base.GetDescription(Slots[Selection].Item));
            }
        }

        private void Start()
        {
            GameManager.Action.Player.Use.performed += Use;
        }

        public void Use(InputAction.CallbackContext cbc)
        {
            Slots[Selection].Use();
        }
    }
}
