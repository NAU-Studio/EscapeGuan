using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using EscapeGuan.Entities.Bullet;
using EscapeGuan.Entities.Items;
using EscapeGuan.Items;
using EscapeGuan.Items.Prop;
using EscapeGuan.UI;
using EscapeGuan.UI.Items;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

using static UnityEngine.Mathf;

namespace EscapeGuan.Entities.Player
{
    public class Player : Entity
    {
        [Header("Player Attributes")]
        public float Speed;
        public float RunSpeedMultiplier;

        public float Stamina;
        public float MaxStamina;
        public float StaminaRestoreWaitTime;
        public float StaminaRestoreAmount;
        public float StaminaCostAmount;

        public Modifiable AttackDistance;

        [Header("Other settings")]
        public Tilemap Map;
        public List<TileBase> SlowdownTiles;
        public float SlowdownMultiplier;

        public QuickInventory QuickInventory;

        public GameObject ThrowCrosshair, ThrowCrosshairVelocity, PlaceCursor;
        public float ThrowStability;

        public AttackState AttackState;

        public BloodAmountEffect BloodEffect;

        public Volume PostProcessing;

        public ParticleSystem BloodDropParticle;

        private Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();

        public override int InventoryLength => 36;
        public override bool ShowHealthBarAtTop => false;

        public override void Start()
        {
            base.Start();

            GameManager.Action = new();
            InControl();
            GameManager.Action.Enable();

            QuickInventory.OnChangeSelection += UpdateAttackState;

            GameManager.ControlledId = Id;
        }

        [Header("Item Pickup System")]
        public float ItemPickupRange = 1;
        public List<int> NearItems = new();
        public ItemPickupList List;

        #region Item Pickup Actions
        public void RemoveNear(int v)
        {
            NearItems.Remove(v);
            List.Remove((ItemEntity)GameManager.EntityPool[v]);
        }

        public override void PickItem(ItemEntity sender)
        {
            AddItem(sender.item);
            RemoveNear(sender.Id);
        }

        public override void AddItem(ItemStack sender)
        {
            for (int i = 0; i < InventoryLength; i++)
            {
                if (Inventory[i] == null)
                {
                    Inventory.Set(i, sender);
                    break;
                }
                if (Inventory[i].Merge(sender))
                    break;
            }
        }
        #endregion
        #region Stamina Actions
        public bool CostStamina(float amount)
        {
            if (amount <= 0)
                return true;
            if (Stamina - amount < 0)
                return false;
            Stamina -= amount;
            RestoreTimer = 0;
            return true;
        }

        public bool RestoreStamina(float amount)
        {
            if (amount <= 0)
                return true;
            if (Stamina + amount > MaxStamina)
            {
                if (Stamina != MaxStamina)
                    Stamina = MaxStamina;
                return false;
            }
            Stamina += amount;
            return true;
        }
        #endregion

        private float CurrentSpeed;
        private bool Running, RunStaminaCostable;

        private Vector2 ControlledMotion;

        private float RestoreTimer = 0;
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            #region Stamina
            if ((ControlledMotion.x != 0 || ControlledMotion.y != 0) && Running)
                RunStaminaCostable = CostStamina(Time.fixedDeltaTime * StaminaCostAmount);
            else
                RunStaminaCostable = true;

            if (RestoreTimer >= StaminaRestoreWaitTime)
                RestoreStamina(Time.fixedDeltaTime * StaminaRestoreAmount);

            #endregion
            #region Movement Control
            if (!RunStaminaCostable || (ControlledMotion == Vector2.zero))
                Running = false;
            if (Running)
                CurrentSpeed = RunSpeedMultiplier;
            else
                CurrentSpeed = 1;

            float final = Speed * CurrentSpeed;
            if (SlowdownTiles.Contains(Map.GetTile(new(RoundToInt(transform.position.x) - 1, RoundToInt(transform.position.y) - 1))))
                final *= SlowdownMultiplier;
            Rigidbody.velocity = final * ControlledMotion;
            #endregion
        }

        private void Update()
        {
            if (RestoreTimer < StaminaRestoreWaitTime)
                RestoreTimer += Time.deltaTime;
        }

        public void OutControl()
        {
            GameManager.Action.Player.Movement.performed -= PerformMovement;
            GameManager.Action.Player.RunningToggle.performed -= PerformRunningToggle;
            GameManager.Action.Player.Attack.performed -= PerformAttack;
            GameManager.Action.Player.Use.performed -= QuickInventory.Use;

            ControlledMotion = Vector2.zero;
            Running = false;
        }

        public void InControl()
        {
            GameManager.Action.Player.Movement.performed += PerformMovement;
            GameManager.Action.Player.RunningToggle.performed += PerformRunningToggle;
            GameManager.Action.Player.Attack.performed += PerformAttack;
            GameManager.Action.Player.Use.performed += QuickInventory.Use;

            ControlledMotion = Vector2.zero;
            Running = false;
        }

        private void PerformMovement(InputAction.CallbackContext x) => ControlledMotion = x.ReadValue<Vector2>();
        private void PerformRunningToggle(InputAction.CallbackContext x) => Running = true;
        private void PerformAttack(InputAction.CallbackContext x) => AttackSelector();

        public void UpdateAttackState(int id, ItemStack item)
        {
            ThrowCrosshair.SetActive(false);
            ThrowCrosshairVelocity.SetActive(false);

            if (item == null)
                goto def;

            if (item.Base is IThrowableItem)
            {
                AttackState = AttackState.Throw;
                ThrowCrosshair.SetActive(true);
                ThrowCrosshairVelocity.SetActive(true);
                return;
            }

            if (item.Base is IPlacableItem)
            {
                AttackState = AttackState.Normal;
                PlaceCursor.SetActive(true);
                return;
            }

        def:
            AttackState = AttackState.Normal;
        }

        public void AddNear(int i)
        {
            NearItems.Add(i);
            List.Add((ItemEntity)GameManager.EntityPool[i]);
        }

        public void AttackSelector()
        {
            switch (AttackState)
            {
                case AttackState.Normal: Attack(); return;
                case AttackState.Throw: Throw(); return;
            }
        }

        public void Throw()
        {
            WaterBottleBullet b = Instantiate(GameManager.Templates["water_bottle_bullet"], transform.position, Quaternion.identity).GetComponent<WaterBottleBullet>();

            b.Init(
                this,
                ThrowCrosshair.GetComponent<Crosshair>().Velocity,
                ThrowCrosshair.GetComponent<Crosshair>().Angle,
                QuickInventory.Slots[QuickInventory.Selection].Item);
            Inventory[QuickInventory.Selection] = null;
        }

        public void Attack()
        {
            VfxManager.CreateLinearAttackTrail("vfx_attack_trail_glow_0", transform.position, (Vector2)transform.position + Vector2.ClampMagnitude(GameManager.CursorPosition - (Vector2)Camera.main.transform.position, AttackDistance), this, .1f, Ease.OutSine);

            if (!QuickInventory.CurrentEmpty && QuickInventory.Current.Base is IPlayerModifierItem && QuickInventory.Current.Base is DurabilityItem)
                QuickInventory.Current.Use(this);
        }

        protected override void Damage(float amount)
        {
            base.Damage(amount);
            Camera.main.GetComponent<SmoothFollower>().Shake(.3f);
            Camera.main.GetComponent<SmoothFollower>().ShakeDrag = (HealthPoint / MaxHealthPoint + 0.01f) * 1f;

            BloodDropParticle.Emit((int)(amount / 100));
        }

        public override void OnHealthChange()
        {
            base.OnHealthChange();
            BloodEffect.SetAmount(.75f);

            foreach (VolumeComponent c in PostProcessing.profile.components)
            {
                if (c is Bloom b)
                    b.threshold.SetValue(new FloatParameter(Lerp(0, 2, HealthPoint / MaxHealthPoint)));
                if (c is MotionBlur m)
                    m.intensity.SetValue(new FloatParameter(Lerp(1, .5f, HealthPoint / MaxHealthPoint)));
            }
        }

        public override void Kill()
        {
            base.Kill();
            OutControl();
        }

        public Vector2 GetPlacePosition() => Camera.main.ScreenToWorldPoint(PlaceCursor.GetComponent<RectTransform>().anchoredPosition);
    }

    public enum AttackState
    {
        Normal,
        Throw,
        Shot
    }
}
