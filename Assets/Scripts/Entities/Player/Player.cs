using System.Collections;
using System.Collections.Generic;

using EscapeGuan.Entities.Items;
using EscapeGuan.UI.Item;

using static UnityEngine.Mathf;

using UnityEngine;
using UnityEngine.Tilemaps;
using EscapeGuan.UI.Player;
using EscapeGuan.Items;
using EscapeGuan.Entities.Bullet;
using EscapeGuan.UI;

namespace EscapeGuan.Entities.Player
{
    public class Player : Entity
    {
        [Header("Player Attributes")]
        public float Speed;
        public float RunSpeedMultiplier;

        public float Stamina;
        public float MaxStamina;
        public float StaminaRestoreDelay;

        [Header("Other settings")]
        public Tilemap Map;
        public List<TileBase> SlowdownTiles;
        public float SlowdownMultiplier;

        public QuickInventory QuickInventory;

        public GameObject ThrowCrosshair, ThrowCrosshairVelocity;
        public float ThrowStability;

        public AttackState AttackState;

        private Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();
        public override int InventoryLength => 36;

        public override void Start()
        {
            base.Start();

            GameManager.Action = new();
            GameManager.Action.Player.Movement.performed += (x) => movement = x.ReadValue<Vector2>();
            GameManager.Action.Player.RunningToggle.performed += (x) => Running = true;
            GameManager.Action.Player.Attack.performed += (x) => AttackSelector();
            GameManager.Action.Enable();

            QuickInventory.OnChangeSelection += UpdateAttackState;

            Attributes.Add(new Attribute<float>("Speed", () => Speed, (x) => Speed = x));
            Attributes.Add(new Attribute<float>("RunSpeedMultiplier", () => RunSpeedMultiplier, (x) => RunSpeedMultiplier = x));
            Attributes.Add(new Attribute<float>("Stamina", () => Stamina, (x) => Stamina = x));
            Attributes.Add(new Attribute<float>("MaxStamina", () => MaxStamina, (x) => MaxStamina = x));
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
                if (Inventory[i].Combine(sender))
                    break;
            }
        }
        #endregion
        #region Stamina Actions
        private IEnumerator SetRestorable()
        {
            while (Restorable)
                yield return null;
            yield return new WaitForSecondsRealtime(StaminaRestoreDelay);
            Restorable = true;
            StartCoroutine(SetRestorable());
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
                RunStaminaCostable = CostStamina((CurrentSpeed - 1) * Time.fixedDeltaTime);
            else
                RunStaminaCostable = true;

            if (Restorable)
                RestoreStamina(Time.fixedDeltaTime);
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
                case AttackState.Normal: return;
                case AttackState.Throw: Throw(); return;
            }
        }

        public void Throw()
        {
            WaterBottleBullet go = Instantiate(GameManager.Templates["water_bottle_bullet"], transform.position, Quaternion.identity).GetComponent<WaterBottleBullet>();

            go.InitialVelocity = ThrowCrosshair.GetComponent<Crosshair>().Velocity;
            go.Base = QuickInventory.Slots[QuickInventory.Selection].Item;
            go.CloneEntityAttribute(this);
            Inventory[QuickInventory.Selection] = null;
        }
    }

    public enum AttackState
    {
        Normal,
        Throw,
        Shot
    }
}
