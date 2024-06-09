using System.Collections;
using System.Collections.Generic;

using EscapeGuan.Entities.Items;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class QuickInventorySlot : MonoBehaviour
{
    public Image Image;
    public TMP_Text Count;

    public ItemStack Item;

    private void Update()
    {
        if (Item != null)
        {
            Image.sprite = Item.Icon;
            Count.text = Item.Count.ToString();
        }
    }
}
