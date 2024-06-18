using System;

using EscapeGuan.Entities.Items;
using EscapeGuan.UI;

using UnityEngine;

public class WaterBottleManager : MonoBehaviour, IKeyBehaviour
{
    ItemStack Exhaler;
    
    HidableUI Hidable => GetComponent<HidableUI>();

    public int KeyLayer => KeyManager.UILayer + 9999;

    public void Exhale(ItemStack sender)
    {
        Exhaler = sender;
        Hidable.Show();
    }

    public void Drink()
    {
        ((WaterBottleItem)Exhaler.Base).Drink(Exhaler);
        Exhaler = null;
        Hidable.Hide();
    }

    public void Throw()
    {
        Hidable.Hide();
        throw new NotImplementedException();
    }
    
    public void ThrowEmpty()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (KeyManager.KeyDown(KeyCode.X, KeyLayer))
        {
            Drink();
            return;
        }
        if (KeyManager.KeyDown(KeyCode.C, KeyLayer))
        {
            Throw();
            return;
        }
    }
}
