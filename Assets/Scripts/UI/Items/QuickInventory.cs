using System.Collections.Generic;

using DG.Tweening;

using EscapeGuan.Items;

using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeGuan.UI.Items
{
    public class QuickInventory : MonoBehaviour
    {
        public List<QuickInventorySlot> Slots;
        ItemStack prevItem;

        public RectTransform SelectionBox;

        public int Selection;

        public ChangeSelectionEventHandler OnChangeSelection;

        public delegate void ChangeSelectionEventHandler(int id, ItemStack item);


        public bool CurrentEmpty => Slots[Selection].Item == null;

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
                SelectionBox.DOAnchorPosX(Slots[Selection].transform.anchoredPosition.x, .1f).SetEase(Ease.OutCubic);

            if (prevItem != Slots[Selection].Item)
            {
                prevItem = Slots[Selection].Item;
                OnChangeSelection(Selection, Slots[Selection].Item);
                if (!CurrentEmpty)
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
