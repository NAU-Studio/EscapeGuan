using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using EscapeGuan.Entities;
using EscapeGuan.Entities.Player;
using EscapeGuan.Items;
using EscapeGuan.Registries;
using EscapeGuan.UI;
using EscapeGuan.UI.Item;
using Unity.VisualScripting;
using UnityEngine;

namespace EscapeGuan
{
    public class GameManager : MonoBehaviour
    {
        public GameObject DamageText;

        public ItemProfileShower ItemProfile;

        public Crosshair Crosshair;

        public AudioSource UISource, PlayerSource, AmbientSource, PropSource;

        public Transform ObjectHUDContainer;

        public static GameManager Main = new();

        public static Dictionary<string, Sprite> ImageResources = new();

        public static Dictionary<string, GameObject> Templates = new();
        public static Dictionary<string, AudioClip> Audios = new();

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

            // HUD
            Templates.Add("health_bar", Resources.Load<GameObject>("Prefabs/Health Bar"));
            #endregion

            #region Initialize Audios
            Audios.Add("ui.button.hover", Resources.Load<AudioClip>("Audios/UI/Hover"));
            Audios.Add("ui.button.click", Resources.Load<AudioClip>("Audios/UI/Click"));
            Audios.Add("player.pickup", Resources.Load<AudioClip>("Audios/Player/pop"));
            Audios.Add("se.rock_break", Resources.Load<AudioClip>("Audios/SE/Rock Break"));
            Audios.Add("se.water.splash", Resources.Load<AudioClip>("Audios/SE/Water Splash"));
            Audios.Add("se.water.bottle_hit", Resources.Load<AudioClip>("Audios/SE/Water Bottle Hit"));
            Audios.Add("se.water.drip", Resources.Load<AudioClip>("Audios/SE/Water Drip"));
            #endregion
        }

        public void PlayAudio(AudioSources src, string name, float volume = 1, float pitch = 1)
        {
            Play(src switch
            {
                AudioSources.UI => UISource,
                AudioSources.Player => PlayerSource,
                AudioSources.Ambient => AmbientSource,
                AudioSources.Prop => PropSource,

                _ => throw new ArgumentOutOfRangeException(nameof(src))
            }, name, volume, pitch);
        }

        private void Play(AudioSource src, string name, float volume, float pitch)
        {
            src.volume = volume;
            src.pitch = pitch;
            src.PlayOneShot(Audios[name]);
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

public static class Extensions
{
    public static void SetFillAmount(this SlicedFilledImage img, float to)
    {
        if (img.fillAmount == to)
            return;
        DOTween.To(() => img.fillAmount, (x) => img.fillAmount = x, to, 0.2f).SetEase(Ease.OutCubic);
    }
}

public enum AudioSources
{
    UI,
    Player,
    Ambient,
    Prop,
}
