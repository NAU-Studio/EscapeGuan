using DG.Tweening;

using EscapeGuan;
using EscapeGuan.UI;

using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public InventorySlotBase[] Slots;

    public RectTransform Selection;
    public Hidable SelectionHidable => Selection.GetComponent<Hidable>();

    private bool Selected;

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
            Hidable.Hide();
        }
        else
        {
            GameManager.Player.OutControl();
            Hidable.Show();
        }
        Showed = !Showed;
    }

    private void UpdateSelection(InventorySlotBase sender)
    {
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
