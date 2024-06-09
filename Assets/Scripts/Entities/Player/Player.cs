using System.Collections;
using System.Collections.Generic;

using EscapeGuan.Entities.Items;
using EscapeGuan.UI;
using EscapeGuan.UI.Item;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
        public float RunStaminaCost;
        public float StaminaRestoreAmount;

        public float StaminaRestoreDelay;
        public float CameraFollowSpeed;
        public Transform Camera;
        public SlicedFilledImage HealthBar;
        public Image StaminaBar;
        public HideableUI StaminaBarHideable;

        public Tilemap Map;
        public List<TileBase> SlowdownTiles;
        public float SlowdownMultiplier;

        public Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();

        public override void Start()
        {
            base.Start();
            Attributes.Add(new Attribute<float>("Speed", () => Speed, (x) => Speed = x));
            Attributes.Add(new Attribute<float>("RunSpeedMultiplier", () => RunSpeedMultiplier, (x) => RunSpeedMultiplier = x));
            Attributes.Add(new Attribute<float>("Stamina", () => Stamina, (x) => Stamina = x));
            Attributes.Add(new Attribute<float>("MaxStamina", () => MaxStamina, (x) => MaxStamina = x));
            Attributes.Add(new Attribute<float>("RunStaminaCost", () => RunStaminaCost, (x) => RunStaminaCost = x));
            Attributes.Add(new Attribute<float>("StaminaRestoreAmount", () => StaminaRestoreAmount, (x) => StaminaRestoreAmount = x));
            GameManager.Main.ControlledEntityId = EntityId;
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
            List.Remove((ItemEntity)GameManager.Main.EntityPool[v]);
        }

        public override void PickItem(ItemEntity sender)
        {
            QuickInventory.Add(sender.item);
            RemoveNear(sender.EntityId);
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
        public override void FixedUpdate()
        {
            float h = Input.GetAxisRaw("Horizontal"), v = Input.GetAxisRaw("Vertical");
            #region Item Pickup
            for (int i = NearItems.Count - 1; i >= 0; i--)
            {
                ItemEntity e = (ItemEntity)GameManager.Main.EntityPool[NearItems[i]];
                if (Vector2.Distance(e.transform.position, transform.position) > ItemPickupRange)
                    RemoveNear(NearItems[i]);
            }
            foreach (int x in GameManager.Main.ItemEntities)
            {
                if (NearItems.Contains(x))
                    continue;
                ItemEntity e = (ItemEntity)GameManager.Main.EntityPool[x];
                if (Vector2.Distance(e.transform.position, transform.position) <= ItemPickupRange)
                {
                    NearItems.Add(x);
                    List.Add((ItemEntity)GameManager.Main.EntityPool[x]);
                }
            }
            #endregion
            #region Stamina
            if ((Abs(h) > 0 || Abs(v) > 0) && Running)
                RunStaminaCostable = CostStamina((CurrentSpeed - 1) * RunStaminaCost * Time.fixedDeltaTime);
            else
                RunStaminaCostable = true;

            if (Restorable)
                RestoreStamina(StaminaRestoreAmount * Time.fixedDeltaTime);
            if (Stamina >= MaxStamina)
                StaminaBarHideable.Hide();
            else
                StaminaBarHideable.Show();
            #endregion
            #region Movement Control
            if (Running)
                CurrentSpeed = RunSpeedMultiplier;
            else
                CurrentSpeed = 1;

            float final = Speed * CurrentSpeed;
            if (SlowdownTiles.Contains(Map.GetTile(new(RoundToInt(transform.position.x) - 1, RoundToInt(transform.position.y) - 1))))
                final *= SlowdownMultiplier;

            if (Abs(h) > 0 && Abs(v) > 0)
                Rigidbody.velocity = new(h * Sqrt2In2 * final, v * Sqrt2In2 * final);
            else
                Rigidbody.velocity = new(h * final, v * final);

            Camera.position = Vector3.Lerp(Camera.position, new(transform.position.x, transform.position.y, Camera.position.z), CameraFollowSpeed * Time.fixedDeltaTime);
            #endregion
            base.FixedUpdate();
        }

        private void Update()
        {
            HealthBar.fillAmount = HealthPoint / MaxHealthPoint;
            StaminaBar.fillAmount = Stamina / MaxStamina;

            #region Movement Control
            if (Input.GetKeyDown(KeyCode.LeftShift) && RunStaminaCostable)
                Running = true;
            if (!RunStaminaCostable || (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0))
                Running = false;
            #endregion
        }
    }
}
