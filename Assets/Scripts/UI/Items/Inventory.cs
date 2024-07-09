using DG.Tweening;

using EscapeGuan;
using EscapeGuan.Items;
using EscapeGuan.UI;
using EscapeGuan.UI.Items;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public InventorySlotBase[] Slots;

    public RectTransform Selection;
    public Hidable SelectionHidable => Selection.GetComponent<Hidable>();

    private bool Selected;
    private InventorySlotBase CurrentSlot;
    private ItemStack CurrentMouseHolding;

    private void Start()
    {
        foreach (InventorySlotBase s in Slots)
            s.MouseEvent += UpdateSelection;

        GameManager.Action.Player.OpenInventory.performed += Toggle;
    }

    private bool Showed;
    private Hidable Hidable => GetComponent<Hidable>();

    private void Toggle(InputAction.CallbackContext x)
    {
        if (Showed)
        {
            GameManager.Player.InControl();
            GameManager.Action.Player.Attack.performed -= Select;
            Hidable.Hide();
        }
        else
        {
            GameManager.Player.OutControl();
            GameManager.Action.Player.Attack.performed += Select;
            Hidable.Show();
        }
        Showed = !Showed;
    }

    private void Select(InputAction.CallbackContext x)
    {
        if (CurrentMouseHolding == null)
        {
            if (!CurrentSlot.IsNull)
            {
                CurrentMouseHolding = CurrentSlot.Item;
                CurrentSlot.SetItem();
            }
        }
        else
        {
            if (CurrentSlot.IsNull)
            {
                CurrentSlot.SetItem(CurrentMouseHolding);
                CurrentMouseHolding = null;
            }
            else
            {
                // TODO: Item merging
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
