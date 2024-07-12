using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using EscapeGuan.Entities.Bullet;
using EscapeGuan.Entities.Items;
using EscapeGuan.Items;
using EscapeGuan.UI;
using EscapeGuan.UI.Items;

using UnityEngine;
using UnityEngine.InputSystem;
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

        public float AttackDistance;

        [Header("Other settings")]
        public Tilemap Map;
        public List<TileBase> SlowdownTiles;
        public float SlowdownMultiplier;

        public QuickInventory QuickInventory;

        public GameObject ThrowCrosshair, ThrowCrosshairVelocity;
        public float ThrowStability;

        public AttackState AttackState;

        public BloodAmountEffect BloodEffectA, BloodEffectB;

        private Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();

        public override int InventoryLength => 36;
        public override bool ShowHealthBarAtTop => false;

        public Dictionary<string, float> AttackDistanceGains = new();
        public Dictionary<string, float> AttackDamageGains = new();

        private float TotalAttackDistanceGain => AttackDistanceGains.Sum(x => x.Value) + 1;
        private float TotalAttackDamageGain => AttackDamageGains.Sum(x => x.Value) + 1;

        public override void Start()
        {
            base.Start();

            GameManager.Action = new();
            InControl();
            GameManager.Action.Enable();

            QuickInventory.OnChangeSelection += UpdateAttackState;

            Attributes.Add(new Attribute<float>("Speed", () => Speed, x => Speed = x));
            Attributes.Add(new Attribute<float>("RunSpeedMultiplier", () => RunSpeedMultiplier, x => RunSpeedMultiplier = x));
            Attributes.Add(new Attribute<float>("Stamina", () => Stamina, x => Stamina = x));
            Attributes.Add(new Attribute<float>("MaxStamina", () => MaxStamina, x => MaxStamina = x));
            GameManager.ControlledId = Id;

            StartCoroutine(SetRestorable());
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
        private IEnumerator SetRestorable()
        {
            yield return new WaitForSecondsRealtime(StaminaRestoreWaitTime);
            Restorable = true;
        }

        public bool CostStamina(float amount)
        {
            if (amount <= 0)
                return true;
            if (Stamina - amount < 0)
                return false;
            Stamina -= amount;
            Restorable = false;
            return true;
        }

        public bool RestoreStamina(float amount)
        {
            if (amount <= 0)
                return true;
            if (Stamina + amount > MaxStamina)
                if (Stamina < MaxStamina)
                    Stamina = MaxStamina;
                else
                    return false;
            Stamina += amount;
            return true;
        }
        #endregion

        private float CurrentSpeed;
        private bool Running, RunStaminaCostable;
        private bool Restorable;

        private Vector2 movement;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            #region Stamina
            if ((movement.x != 0 || movement.y != 0) && Running)
                RunStaminaCostable = CostStamina(Time.fixedDeltaTime * StaminaCostAmount);
            else
                RunStaminaCostable = true;

            if (Restorable)
                RestoreStamina(Time.fixedDeltaTime * StaminaRestoreAmount);
            else
                StartCoroutine(SetRestorable());
            #endregion
            #region Movement Control
            if (!RunStaminaCostable || (movement == Vector2.zero))
                Running = false;
            if (Running)
                CurrentSpeed = RunSpeedMultiplier;
            else
                CurrentSpeed = 1;

            float final = Speed * CurrentSpeed;
            if (SlowdownTiles.Contains(Map.GetTile(new(RoundToInt(transform.position.x) - 1, RoundToInt(transform.position.y) - 1))))
                final *= SlowdownMultiplier;
            Rigidbody.velocity = final * movement;
            #endregion
        }

        public void OutControl()
        {
            GameManager.Action.Player.Movement.performed -= PerformMovement;
            GameManager.Action.Player.RunningToggle.performed -= PerformRunningToggle;
            GameManager.Action.Player.Attack.performed -= PerformAttack;
            GameManager.Action.Player.Use.performed -= QuickInventory.Use;

            movement = Vector2.zero;
            Running = false;
        }

        public void InControl()
        {
            GameManager.Action.Player.Movement.performed += PerformMovement;
            GameManager.Action.Player.RunningToggle.performed += PerformRunningToggle;
            GameManager.Action.Player.Attack.performed += PerformAttack;
            GameManager.Action.Player.Use.performed += QuickInventory.Use;

            movement = Vector2.zero;
            Running = false;
        }

        private void PerformMovement(InputAction.CallbackContext x) => movement = x.ReadValue<Vector2>();
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
            VfxManager.CreateLinearAttackTrail("vfx_attack_trail_glow_0", transform.position, (Vector2)transform.position + Vector2.ClampMagnitude(GameManager.CursorPosition - (Vector2)Camera.main.transform.position, AttackDistance * TotalAttackDistanceGain), this, .1f, Ease.OutSine);

            if (!QuickInventory.CurrentEmpty && QuickInventory.Current.Base is IPlayerGainItem && QuickInventory.Current.Base is DurabilityItem)
                QuickInventory.Current.Use(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, AttackDistance * TotalAttackDistanceGain);
        }

        protected override void Damage(float amount)
        {
            base.Damage(amount);
            Camera.main.GetComponent<SmoothFollower>().Shake(.1f);
            Camera.main.GetComponent<SmoothFollower>().ShakeDrag = (HealthPoint / MaxHealthPoint + 0.01f) * 0.75f;

            BloodEffectA.SetAmount(0.2f - HealthPoint / MaxHealthPoint);
            BloodEffectB.SetAmount(1 - HealthPoint / MaxHealthPoint);
        }

        public override float GetAttackAmount()
        {
            return base.GetAttackAmount() * TotalAttackDamageGain;
        }
    }

    public enum AttackState
    {
        Normal,
        Throw,
        Shot
    }
}
