using System.Collections.Generic;

using DG.Tweening;

using EscapeGuan.Entities.Items;

using Unity.VisualScripting;

using UnityEngine;

namespace EscapeGuan.UI.Item
{
    public class ItemPickupList : RectBehaviour
    {
        public List<ItemPickupOption> Options = new();
        public ItemPickupOption Template;
        public HidableUI Parent;
        public RectTransform SelectionObject;
        public int Selection = 0;

        private int prevSel = 0;
        private void Update()
        {
            if (Options.Count > 1)
            {
                float scr = Input.mouseScrollDelta.y;
                if (scr < 0)
                {
                    Selection++;
                    if (Selection >= Options.Count)
                        Selection = 0;
                }
                if (scr > 0)
                {
                    Selection--;
                    if (Selection < 0)
                        Selection = Options.Count - 1;
                }
            }
            if (Options.Count > 0)
            {
                if (Selection >= Options.Count)
                    Selection = Options.Count - 1;
                if (Selection != prevSel)
                {
                    prevSel = Selection;
                    SelectionObject.DOAnchorPosY(-Selection * Template.transform.sizeDelta.y, .2f).SetEase(Ease.OutCubic);
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && Options.Count != 0)
            {
                Options[Selection].Pickup();
            }
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

        public void RemoveDestroyed()
        {
            foreach (ItemPickupOption opt in Options)
            {
                if (opt.Target.IsDestroyed())
                    Remove(opt.Target);
            }
        }

        public void Pick(int index)
        {
            Options[index].Pickup();
        }
    }
}
