using System.Collections;
using System.Collections.Generic;

using EscapeGuan;
using EscapeGuan.Entities.Items;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class QuickInventorySlot : MonoBehaviour
{
    public Image Image;
    public TMP_Text Count;

    public ItemStack Item;
    public int Index;

    private float cd = 0;
    private void Update()
    {
        cd -= Time.deltaTime;
        if (Item != null)
        {
            Image.sprite = Item.Icon;
            Image.color = new(1, 1, 1, 1);
            Count.text = Item.GetCountString();
        }
        else
        {
            Image.sprite = null;
            Image.color = new(1, 1, 1, 0);
            Count.text = "";
        }
    }

    public void Use()
    {
        if (cd <= 0)
        {
            Item?.Use(GameManager.Main.EntityPool[GameManager.Main.ControlledEntityId]);
            cd = Item.CD;
        }
    }
}
