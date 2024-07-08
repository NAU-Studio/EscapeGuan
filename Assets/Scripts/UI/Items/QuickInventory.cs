using System.Collections.Generic;

using DG.Tweening;

using EscapeGuan.Items;

using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeGuan.UI.Items
{
    public class QuickInventory : MonoBehaviour
    {
        public List<InventorySlot> Slots;
        ItemStack prevItem;

        public RectTransform SelectionBox;

        public int Selection;

        public ChangeSelectionEventHandler OnChangeSelection;

        public delegate void ChangeSelectionEventHandler(int id, ItemStack item);

        public ItemStack Current => Slots[Selection].Item;
        public bool CurrentEmpty => Current == null;

        private void Update()
        {
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
                prevItem?.OnPutDown();
                prevItem = Slots[Selection].Item;

                OnChangeSelection(Selection, prevItem);
                if (!CurrentEmpty)
                {
                    GameManager.Main.ItemProfile.SetText(prevItem.Base.Name, prevItem.Base.GetDescription(prevItem));
                    prevItem.OnHoldUp();
                }
                else
                    GameManager.Main.ItemProfile.HideFast();
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
