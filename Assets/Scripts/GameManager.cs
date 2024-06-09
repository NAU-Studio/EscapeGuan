using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;
using EscapeGuan.Registries;

using UnityEditor;

using UnityEngine;

namespace EscapeGuan
{
    public class GameManager : MonoBehaviour
    {
        public GameObject DamageText;
        public int ControlledEntityId;
        public Dictionary<int, Entity> EntityPool = new();
        public List<int> ItemEntities = new();
        public GameObject ItemTemplate;
        public static GameManager Main = new();

        public static Dictionary<string, Sprite> ImageResources = new();

        private void Start()
        {
            Main = this;

            ImageResources.Add("water_bottle", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/water_bottle_item.png"));

            #region Initialize items
            ItemRegistry.Main.RegisterObject("water_bottle", new TestBottleItem("À¶±ê¿óÈªË®", "TEST", ImageResources["water_bottle"]));

            ItemStack ix = ItemRegistry.Main.CreateItemStack("water_bottle", new());
            ix.CreateEntity(ItemTemplate, new(-0.19f, -4.22f), 0);
            ix = ItemRegistry.Main.CreateItemStack("water_bottle", new());
            ix.CreateEntity(ItemTemplate, new(-1.19f, -4.22f), 0);
            ix = ItemRegistry.Main.CreateItemStack("water_bottle", new());
            ix.CreateEntity(ItemTemplate, new(-0.19f, -3.22f), 0);
            ix = ItemRegistry.Main.CreateItemStack("water_bottle", new());
            ix.CreateEntity(ItemTemplate, new(-1.19f, -3.22f), 0);
            #endregion
        }

        public static void Pause()
        {
            Time.timeScale = 0;
        }

        public static void Continue()
        {
            Time.timeScale = 1;
        }

        public static void DelayAction(MonoBehaviour sender, Action act, float interval)
        {
            sender.StartCoroutine(DelayActionP(act, interval));
        }

        private static IEnumerator DelayActionP(Action act, float interval)
        {
            yield return new WaitForSeconds(interval);
            act();
        }

        public static void IntervalAction(MonoBehaviour sender, IntervalActionStatementGetter statement, Action act, float interval)
        {
            sender.StartCoroutine(IntervalActionP(sender, statement, act, interval));
        }

        public static void IntervalAction(MonoBehaviour sender, IntervalActionStatementGetter statement, CoroutineAction act, float interval)
        {
            sender.StartCoroutine(IntervalActionP(sender, statement, act, interval));
        }

        private static IEnumerator IntervalActionP(MonoBehaviour sender, IntervalActionStatementGetter statement, Action act, float interval)
        {
            while (statement())
            {
                act();
                yield return new WaitForSeconds(interval);
            }
            while (!statement())
                yield return new WaitForFixedUpdate();
            sender.StartCoroutine(IntervalActionP(sender, statement, act, interval));
        }

        private static IEnumerator IntervalActionP(MonoBehaviour sender, IntervalActionStatementGetter statement, CoroutineAction act, float interval)
        {
            while (statement())
            {
                sender.StartCoroutine(act());
                yield return new WaitForSeconds(interval);
            }
            while (!statement())
                yield return new WaitForFixedUpdate();
            sender.StartCoroutine(IntervalActionP(sender, statement, act, interval));
        }

        public delegate bool IntervalActionStatementGetter();
        public delegate IEnumerator CoroutineAction();
    }

    public static class ExtensionMethods
    {
        public static int CountOf<T>(this List<T> list, T value)
        {
            return list.Where(d => d.Equals(value)).Count();
        }
    }
}
