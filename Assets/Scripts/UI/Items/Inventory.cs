using DG.Tweening;

using EscapeGuan.Items;
using EscapeGuan.Items.Recipes;

using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeGuan.UI.Items
{
    public class Inventory : MonoBehaviour
    {
        public InventorySlotBase[] Slots;

        public RectTransform Selection;
        public CursorInventorySlot CursorSlot;
        public Hidable SelectionHidable => Selection.GetComponent<Hidable>();

        public InventoryOperationSlot[] CraftingIngredientSlots;
        public int CraftingIngredientWidth, CraftingIngredientHeight;
        public InventoryOperationSlot CraftingResultSlot;

        private bool Selected;
        private InventorySlotBase CurrentSlot;

        private void Start()
        {
            foreach (InventorySlotBase s in Slots)
                s.MouseEvent += UpdateSelection;

            GameManager.Action.Player.OpenInventory.performed += Toggle;

            foreach (InventoryOperationSlot i in CraftingIngredientSlots)
                i.OnItemChanged += UpdateCrafting;
            CraftingResultSlot.OnItemChanged += PickCraftingResult;
        }

        private bool Crafting = false;
        private void UpdateCrafting()
        {
            CraftingInput input = new(CraftingIngredientSlots, CraftingIngredientWidth, CraftingIngredientHeight);
            foreach (Recipe r in GameManager.Recipes)
            {
                if (r.Match(input))
                {
                    CraftingResultSlot.SetItemSilently(r.Result.CreateItemStack());
                    Crafting = true;
                    return;
                }
            }
            CraftingResultSlot.SetItem();
        }
        private void PickCraftingResult()
        {
            if (!Crafting)
                return;
            foreach (InventoryOperationSlot i in CraftingIngredientSlots)
                i.SetItemSilently();
            Crafting = false;
        }

        private bool Showed;
        private Hidable Hidable => GetComponent<Hidable>();

        private void Toggle(InputAction.CallbackContext x)
        {
            if (Showed)
            {
                GameManager.Player.InControl();
                GameManager.Action.Player.Attack.performed -= Select;
                GameManager.Action.Player.Use.performed -= Tweak;
                GameManager.Action.Player.Close.performed -= Toggle;
                Hidable.Hide();
            }
            else
            {
                GameManager.Player.OutControl();
                GameManager.Action.Player.Attack.performed += Select;
                GameManager.Action.Player.Use.performed += Tweak;
                GameManager.Action.Player.Close.performed += Toggle;
                Hidable.Show();
            }
            Showed = !Showed;
        }

        private void Select(InputAction.CallbackContext x)
        {
            if (CursorSlot.IsNull)
            {
                if (CurrentSlot != null && !CurrentSlot.IsNull)
                {
                    CursorSlot.SetItem(CurrentSlot.Item);
                    CurrentSlot.SetItem();
                }
            }
            else if (CurrentSlot != null)
            {
                if (CurrentSlot.IsNull)
                {
                    CurrentSlot.SetItem(CursorSlot.Item);
                    CursorSlot.SetItem();
                }
                else
                {
                    if (CurrentSlot.Item.Merge(CursorSlot.Item))
                        CursorSlot.SetItem();
                    else
                    {
                        ItemStack mediation = CurrentSlot.Item;
                        CurrentSlot.SetItem(CursorSlot.Item);
                        CursorSlot.SetItem(mediation);
                    }
                }
            }
        }

        private void Tweak(InputAction.CallbackContext x)
        {
            if (CursorSlot.IsNull)
            {
                if (CurrentSlot != null && !CurrentSlot.IsNull)
                {
                    int pickCount = Mathf.FloorToInt((float)CurrentSlot.Item.Count / 2);
                    if (pickCount <= 0)
                        return;
                    CursorSlot.SetItem(CurrentSlot.Item.Duplicate(pickCount));
                    CurrentSlot.Item.Count -= pickCount;
                }
            }
            else if (CurrentSlot != null)
            {
                if (CurrentSlot.IsNull)
                {
                    CurrentSlot.SetItem(CursorSlot.Item.Duplicate(1));
                    CursorSlot.Item.Count--;
                }
                else
                {
                    if (CurrentSlot.Item.Merge(CursorSlot.Item.Duplicate(1)))
                    {
                        CursorSlot.Item.Count--;
                        if (CursorSlot.Item.Count <= 0)
                            CursorSlot.SetItem();
                    }
                    else
                    {
                        ItemStack mediation = CurrentSlot.Item;
                        CurrentSlot.SetItem(CursorSlot.Item);
                        CursorSlot.SetItem(mediation);
                    }
                }
            }
        }

        private void UpdateSelection(InventorySlotBase sender)
        {
            CurrentSlot = sender;
            if (sender != null)
            {
                Selection.DOAnchorPos(GetCanvasPosition(sender.transform), .2f).SetEase(Ease.OutCubic);
                Selection.DOPivot(sender.transform.pivot, .2f).SetEase(Ease.OutCubic);
                if (!Selected)
                {
                    Selected = true;
                    SelectionHidable.Show();
                }
            }
            else
            {
                if (Selected)
                {
                    Selected = false;
                    SelectionHidable.Hide();
                }
            }
        }

        private RectTransform Parent => (RectTransform)GetComponentInParent<Canvas>().transform;

        private Vector2 GetCanvasPosition(RectTransform t)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, Camera.main.WorldToScreenPoint(t.position), Camera.main, out Vector2 ret);
            return ret;
        }
    }
}
