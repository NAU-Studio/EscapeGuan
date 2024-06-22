using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EscapeGuan.Entities.Items;
using EscapeGuan.UI.Item;
using EscapeGuan.UI.Player;
using UnityEngine;
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
        public float StaminaRestoreDelay;

        [Header("Other settings")]
        public float CameraFollowSpeed;
        public Transform Camera;

        public Tilemap Map;
        public List<TileBase> SlowdownTiles;
        public float SlowdownMultiplier;

        private Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();
        public override int InventoryLength => 36;

        public override void Start()
        {
            base.Start();
            Attributes.Add(new Attribute<float>("Speed", () => Speed, (x) => Speed = x));
            Attributes.Add(new Attribute<float>("RunSpeedMultiplier", () => RunSpeedMultiplier, (x) => RunSpeedMultiplier = x));
            Attributes.Add(new Attribute<float>("Stamina", () => Stamina, (x) => Stamina = x));
            Attributes.Add(new Attribute<float>("MaxStamina", () => MaxStamina, (x) => MaxStamina = x));
            GameManager.ControlledEntityId = Id;

            StartCoroutine(SetRestorable());
        }

        private const float Sqrt2In2 = 0.70710678118654752440084436210485f;

        [Header("Item Pickup System")]
        public float ItemPickupRange = 1;
        public List<int> NearItems = new();
        public ItemPickupList List;
        public QuickInventoryShower QuickInventory;

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

        private float Horizontal => (Keys.Press(KeyCode.A, Keys.ControlLayer) ? -1 : 0) + (Keys.Press(KeyCode.D, Keys.ControlLayer) ? 1 : 0);
        private float Vertical => (Keys.Press(KeyCode.S, Keys.ControlLayer) ? -1 : 0) + (Keys.Press(KeyCode.W, Keys.ControlLayer) ? 1 : 0);

        public override void FixedUpdate()
        {
            #region Stamina
            if ((Abs(Horizontal) > 0 || Abs(Vertical) > 0) && Running)
                RunStaminaCostable = CostStamina((CurrentSpeed - 1) * Time.fixedDeltaTime);
            else
                RunStaminaCostable = true;

            if (Restorable)
                RestoreStamina(Time.fixedDeltaTime);
            #endregion
            #region Movement Control
            if (Running)
                CurrentSpeed = RunSpeedMultiplier;
            else
                CurrentSpeed = 1;

            float final = Speed * CurrentSpeed;
            if (SlowdownTiles.Contains(Map.GetTile(new(RoundToInt(transform.position.x) - 1, RoundToInt(transform.position.y) - 1))))
                final *= SlowdownMultiplier;

            if (Abs(Horizontal) > 0 && Abs(Vertical) > 0)
                Rigidbody.velocity = new(Horizontal * Sqrt2In2 * final, Vertical * Sqrt2In2 * final);
            else
                Rigidbody.velocity = new(Horizontal * final, Vertical * final);

            Camera.position = Vector3.Lerp(Camera.position, new(transform.position.x, transform.position.y, Camera.position.z), CameraFollowSpeed * Time.fixedDeltaTime);
            #endregion
            base.FixedUpdate();
        }

        private void Update()
        {
            #region Movement Control
            if (Keys.Down(KeyCode.LeftShift, Keys.ControlLayer) && RunStaminaCostable)
                Running = true;
            if (!RunStaminaCostable || (Horizontal == 0 && Vertical == 0))
                Running = false;
            #endregion
        }

        public void AddNear(int i)
        {
            NearItems.Add(i);
            List.Add((ItemEntity)GameManager.EntityPool[i]);
        }
    }
}
