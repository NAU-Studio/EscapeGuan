using System.Collections.Generic;

using DG.Tweening;

using EscapeGuan;
using EscapeGuan.Entities.Items;

using UnityEngine;

public class QuickInventoryShower : MonoBehaviour
{
    [SerializeField]
    List<QuickInventorySlot> Slots;

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
            s.Item = GameManager.Main.EntityPool[GameManager.Main.ControlledEntityId].Inventory[s.Index];

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

        if (Input.GetKeyDown(KeyCode.Z))
            Use();
    }

    public void Use()
    {
        Slots[Selection].Use();
    }
}
