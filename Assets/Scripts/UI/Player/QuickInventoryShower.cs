using System;
using System.Collections;
using System.Collections.Generic;

using EscapeGuan;
using EscapeGuan.Entities.Items;

using UnityEngine;

public class QuickInventoryShower : MonoBehaviour
{
    [SerializeField]
    List<QuickInventorySlot> Slots;

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

    public void Use()
    {
        Slots[Selection].Item?.Use(GameManager.Main.EntityPool[GameManager.Main.ControlledEntityId]);
    }
}
