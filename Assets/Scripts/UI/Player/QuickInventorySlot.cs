using EscapeGuan.Entities.Items;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace EscapeGuan.UI.Player
{
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
                Image.sprite = Item.Base.Icon;
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
                Item?.Use(GameManager.Player);
                cd = Item.CD;
            }
        }
    }
}
