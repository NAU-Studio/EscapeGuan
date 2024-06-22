using System;

using EscapeGuan.Entities.Items;
using EscapeGuan.Items;

using UnityEngine;

namespace EscapeGuan.UI.Item.WaterBottle
{
    public class WaterBottleManager : MonoBehaviour
    {
        ItemStack Exhaler;

        Hidable Hidable => GetComponent<Hidable>();

        public int KeyLayer => Keys.UILayer + 9999;

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
            if (Keys.Down(KeyCode.X, KeyLayer))
            {
                Drink();
                return;
            }
            if (Keys.Down(KeyCode.C, KeyLayer))
            {
                Throw();
                return;
            }
        }
    }
}
