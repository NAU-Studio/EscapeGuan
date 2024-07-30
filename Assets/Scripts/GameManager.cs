using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using EscapeGuan.Entities;
using EscapeGuan.Entities.Player;
using EscapeGuan.Items;
using EscapeGuan.Items.Recipes;
using EscapeGuan.UI;
using EscapeGuan.UI.Items;

using UnityEngine;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

namespace EscapeGuan
{
    public class GameManager : MonoBehaviour
    {
        public GameObject DamageText;

        public ItemProfileShower ItemProfile;

        public Crosshair Crosshair;

        public AudioSource UISource, PlayerSource, AmbientSource, PropSource;

        public Transform ObjectHUDContainer;

        public RectTransform MainCanvas;

        public static float DamageInterval = 0.3f;

        public static GameManager Main = new();

        public static Dictionary<string, Sprite> ImageResources = new();

        public static Dictionary<string, GameObject> Templates = new();
        public static Dictionary<string, AudioClip> Audios = new();

        public static int ControlledId;
        public static Dictionary<int, Entity> EntityPool = new();
        public static HashSet<int> ItemEntities = new();
        public static Player Player
        {
            get
            {
                if (EntityPool.TryGetValue(ControlledId, out Entity e))
                    return (Player)e;
                else
                    return null;
            }
        }
        public static PlayerAction Action;

        public static Vector2 CursorPosition { get { RectTransformUtility.ScreenPointToWorldPointInRectangle(Main.MainCanvas, Mouse.current.position.value, Camera.main, out Vector3 v); return v; } }

        public static List<Recipe> Recipes = new();

        private void Start()
        {
            Main = this;

            #region Initialize Resources
            ImageResources.Add("water_bottle", Resources.Load<Sprite>("Sprites/Items/water_bottle_item"));
            ImageResources.Add("small_stick", Resources.Load<Sprite>("Sprites/Items/small_stick"));
            ImageResources.Add("refined_stick", Resources.Load<Sprite>("Sprites/Items/refined_stick"));
            ImageResources.Add("rock_sharp", Resources.Load<Sprite>("Sprites/Items/rock_sharp"));
            #endregion

            #region Initialize Item Registry
            ItemRegistry.Main.RegisterObject("water_bottle", new WaterBottleItem("蓝标矿泉水", "一瓶蓝标矿泉水，净含量550 mL。", ImageResources["water_bottle"]));
            ItemRegistry.Main.RegisterObject("empty_bottle", new EmptyWaterBottleItem("空的蓝标矿泉水", "一瓶空的蓝标矿泉水，净含量550 mL，里面没有水。", ImageResources["water_bottle"]));
            ItemRegistry.Main.RegisterObject("small_stick", new SmallStickItem("小树枝", "攻击伤害和攻击距离提升，但是容易断。", ImageResources["small_stick"]));
            ItemRegistry.Main.RegisterObject("refined_stick", new("木棍", "精致加工的木棍，可以用于合成，但没有任何攻击加成。", ImageResources["refined_stick"]));
            ItemRegistry.Main.RegisterObject("rock_sharp", new RockSharpItem("石头碎片", "尖锐的石头碎片，不会提升攻击距离但会大幅提升攻击伤害。", ImageResources["rock_sharp"]));
            #endregion

            #region Initialize Templates
            Templates.Add("item", Resources.Load<GameObject>("Prefabs/Item"));
            Templates.Add("rock", Resources.Load<GameObject>("Prefabs/Rock"));
            Templates.Add("water_bottle_stack", Resources.Load<GameObject>("Prefabs/Water Bottle Stack"));
            Templates.Add("water_bottle_bullet", Resources.Load<GameObject>("Prefabs/Water Bottle Bullet"));
            Templates.Add("water_drop", Resources.Load<GameObject>("Prefabs/Water Drop"));

            // Particles
            Templates.Add("rock_destroy_particle", Resources.Load<GameObject>("Prefabs/Rock Destroy Particle"));
            Templates.Add("water_drop_particle", Resources.Load<GameObject>("Prefabs/Water Drop Particle"));
            Templates.Add("blood_drop_particle", Resources.Load<GameObject>("Prefabs/Blood Drop Particle"));

            // HUD
            Templates.Add("health_bar", Resources.Load<GameObject>("Prefabs/Health Bar"));

            // VFX
            VfxManager.VfxTemplates.Add("vfx_attack_trail_glow_0", Resources.Load<GameObject>("Prefabs/VFX Attack Trail Glow"));
            #endregion

            #region Initialize Audios
            Audios.Add("ui.button.hover", Resources.Load<AudioClip>("Audios/UI/Hover"));
            Audios.Add("ui.button.click_n2", Resources.Load<AudioClip>("Audios/UI/Click -2"));
            Audios.Add("ui.button.click_n1", Resources.Load<AudioClip>("Audios/UI/Click -1"));
            Audios.Add("ui.button.click_0", Resources.Load<AudioClip>("Audios/UI/Click 0"));
            Audios.Add("ui.button.click_1", Resources.Load<AudioClip>("Audios/UI/Click 1"));
            Audios.Add("ui.button.click_2", Resources.Load<AudioClip>("Audios/UI/Click 2"));
            Audios.Add("player.pickup", Resources.Load<AudioClip>("Audios/Player/pop"));
            Audios.Add("se.rock_break", Resources.Load<AudioClip>("Audios/SE/Rock Break"));
            Audios.Add("se.water.splash", Resources.Load<AudioClip>("Audios/SE/Water Splash"));
            Audios.Add("se.water.bottle_hit", Resources.Load<AudioClip>("Audios/SE/Water Bottle Hit"));
            Audios.Add("se.water.drip", Resources.Load<AudioClip>("Audios/SE/Water Drip"));
            Audios.Add("entity.damage_1", Resources.Load<AudioClip>("Audios/Damage/hit1"));
            Audios.Add("entity.damage_2", Resources.Load<AudioClip>("Audios/Damage/hit2"));
            Audios.Add("entity.damage_3", Resources.Load<AudioClip>("Audios/Damage/hit3"));
            Audios.Add("entity.stone_1", Resources.Load<AudioClip>("Audios/Damage/stone1"));
            Audios.Add("entity.stone_2", Resources.Load<AudioClip>("Audios/Damage/stone2"));
            Audios.Add("entity.stone_3", Resources.Load<AudioClip>("Audios/Damage/stone3"));
            Audios.Add("entity.stone_4", Resources.Load<AudioClip>("Audios/Damage/stone4"));
            Audios.Add("entity.plastic_1", Resources.Load<AudioClip>("Audios/Damage/plastic1"));
            Audios.Add("entity.plastic_2", Resources.Load<AudioClip>("Audios/Damage/plastic2"));
            Audios.Add("entity.plastic_3", Resources.Load<AudioClip>("Audios/Damage/plastic3"));
            #endregion

            #region Initialize recipes
            foreach (TextAsset asset in Resources.LoadAll<TextAsset>("Datas/Recipe"))
                Recipes.Add(Recipe.FromFile(asset.text));
            #endregion

            Cursor.visible = false;
        }

        public void PlayAudio(AudioSources src, string name, float volume = 1, float pitch = 1)
        {
            PlayAudio_(src switch
            {
                AudioSources.UI => UISource,
                AudioSources.Player => PlayerSource,
                AudioSources.Ambient => AmbientSource,
                AudioSources.Prop => PropSource,

                _ => throw new ArgumentOutOfRangeException(nameof(src))
            }, name, volume, pitch);
        }

        public void PlayRandomAudio(AudioSources src, float volume = 1, float pitch = 1, params string[] name)
        {
            PlayAudio(src, name[Random.Range(0, name.Length)], volume, pitch);
        }

        private void PlayAudio_(AudioSource src, string name, float volume, float pitch)
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
