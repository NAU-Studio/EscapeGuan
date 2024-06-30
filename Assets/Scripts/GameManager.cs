using System;
using System.Collections;
using System.Collections.Generic;

using EscapeGuan.Entities;
using EscapeGuan.Entities.Player;
using EscapeGuan.Items;
using EscapeGuan.Registries;
using EscapeGuan.UI;
using EscapeGuan.UI.Item;

using UnityEngine;

namespace EscapeGuan
{
    public class GameManager : MonoBehaviour
    {
        public GameObject DamageText;

        public ItemProfileShower ItemProfile;

        public AudioSource UIEffect;

        public Crosshair Crosshair;

        public static GameManager Main = new();

        public static Dictionary<string, Sprite> ImageResources = new();

        public static Dictionary<string, GameObject> Templates = new();

        public static int ControlledId;
        public static Dictionary<int, Entity> EntityPool = new();
        public static HashSet<int> ItemEntities = new();
        public static Player Player => (Player)EntityPool[ControlledId];
        public static PlayerAction Action;

        private void Start()
        {
            Main = this;

            #region Initialize Resources
            ImageResources.Add("water_bottle", Resources.Load<Sprite>("Sprites/Items/water_bottle_item"));
            ImageResources.Add("small_stick", Resources.Load<Sprite>("Sprites/Items/small_stick"));
            #endregion

            #region Initialize Item Registry
            ItemRegistry.Main.RegisterObject("water_bottle", new WaterBottleItem("蓝标矿泉水", "一瓶蓝标矿泉水，净含量550 mL。能扔能喝，还能往里兑水，水越多砸人越疼。", ImageResources["water_bottle"]));
            ItemRegistry.Main.RegisterObject("empty_bottle", new EmptyWaterBottleItem("空的蓝标矿泉水", "一瓶蓝标矿泉水，净含量550 mL。但是里面没有水，不过可以往里倒！", ImageResources["water_bottle"]));
            ItemRegistry.Main.RegisterObject("small_stick", new SmallStickItem("小树枝", "伤害轻微提升，但是容易断", ImageResources["small_stick"]));
            #endregion

            #region Initialize Templates
            Templates.Add("item", Resources.Load<GameObject>("Prefabs/Item"));
            Templates.Add("rock", Resources.Load<GameObject>("Prefabs/Rock"));
            Templates.Add("water_bottle_bullet", Resources.Load<GameObject>("Prefabs/Water Bottle Bullet"));
            Templates.Add("water_drop", Resources.Load<GameObject>("Prefabs/Water Drop"));

            // Particles
            Templates.Add("rock_destroy_particle", Resources.Load<GameObject>("Prefabs/Rock Destroy Particle"));
            Templates.Add("water_drop_particle", Resources.Load<GameObject>("Prefabs/Water Drop Particle"));
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
}
