using System.Collections.Generic;

using DG.Tweening;

using EscapeGuan.Entities.Items;

using UnityEngine;
using UnityEngine.InputSystem;

namespace EscapeGuan.UI.Items
{
    public class ItemPickupList : RectBehaviour
    {
        public List<ItemPickupOption> Options = new();
        public ItemPickupOption Template;
        public Hidable Parent;
        public RectTransform SelectionObject;
        public int Selection = 0;

        private void Update()
        {
            int scr = (int)Mouse.current.scroll.value.y;
            if (Options.Count > 1 && scr != 0)
            {
                Selection -= scr;
                if (Selection >= Options.Count)
                    Selection = 0;
                if (Selection < 0)
                    Selection = Options.Count - 1;
                SelectionObject.DOAnchorPosY(-Selection * Template.transform.sizeDelta.y, .2f).SetEase(Ease.OutCubic);
            }
            if (Keyboard.current.fKey.wasReleasedThisFrame && Options.Count != 0)
                Pick();
        }

        public void Refresh()
        {
            if (Options.Count <= 0)
            {
                Parent.Hide();
                return;
            }

            Parent.Show();

            float t = 0;
            foreach (ItemPickupOption opt in Options)
            {
                if (opt.transform.anchoredPosition.y != t)
                    opt.transform.DOAnchorPosY(t, .2f).SetEase(Ease.OutCubic);
                if (!opt.Destroyed)
                    t -= Template.transform.sizeDelta.y;
            }
            transform.sizeDelta = new(transform.sizeDelta.x, -t + Template.transform.sizeDelta.y);
        }

        public void Add(ItemEntity target)
        {
            GameObject o = Instantiate(Template.gameObject, transform);
            o.SetActive(true);
            ItemPickupOption opt = o.GetComponent<ItemPickupOption>();
            opt.Target = target;
            opt.Show();
            opt.transform.anchoredPosition = new(0, -Options.Count * Template.transform.sizeDelta.y);
            Options.Add(opt);
            Refresh();
        }

        public void Remove(ItemEntity target)
        {
            foreach (ItemPickupOption opt in Options)
            {
                if (opt.Target == target)
                {
                    opt.Destroy();
                    Options.Remove(opt);
                    Refresh();
                    return;
                }
            }
        }

        public void Pick()
        {
            Options[Selection].Pickup();
            GameManager.Main.PlayAudio(AudioSources.Player, "player.pickup", pitch: Random.Range(0.5f, 1.5f));
        }
    }
}
