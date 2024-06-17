using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using EscapeGuan.Entities;
using EscapeGuan.Registries;
using EscapeGuan.UI;

using Unity.VisualScripting;

using UnityEngine;

namespace EscapeGuan
{
    public class GameManager : MonoBehaviour
    {
        public GameObject DamageText;
        public int ControlledEntityId;
        public Dictionary<int, Entity> EntityPool = new();
        public List<int> ItemEntities = new();

        public WaterDrinkUI WaterUseUI;


        public static GameManager Main = new();

        public static Dictionary<string, Sprite> ImageResources = new();

        public static Dictionary<string, GameObject> Templates = new();

        private void Start()
        {
            Main = this;

            // 一切都是为了mod和可持续的发展 T_T
            #region Initialize Resources
            ImageResources.Add("water_bottle", Resources.Load<Sprite>("Sprites/Items/water_bottle_item"));
            #endregion

            #region Initialize Item Registry
            ItemRegistry.Main.RegisterObject("water_bottle", new WaterBottleItem("蓝标矿泉水", "一瓶蓝标矿泉水，净含量550 mL，能扔也能喝！", ImageResources["water_bottle"]));
            #endregion

            #region Initialize Templates
            Templates.Add("item", Resources.Load<GameObject>("Prefabs/Item"));
            Templates.Add("rock", Resources.Load<GameObject>("Prefabs/Rock"));
            #endregion
        }

        private void Update()
        {
            foreach (KeyValuePair<int, Entity> e in EntityPool.Where((x) => x.Value.IsDestroyed()))
            {
                EntityPool.Remove(e.Key);
                int a = ItemEntities.IndexOf(e.Key);
                if (a >= 0)
                    ItemEntities.Remove(a);
            }
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
