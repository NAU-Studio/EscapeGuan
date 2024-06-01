using EscapeGuan.Entities.Items;

namespace EscapeGuan.UI.Item
{
    public class ItemPickupOption : RectBehaviour
    {
        public ItemEntity Target;
        public bool Enabled = true;

        public bool Destroyed = false;

        public void Pickup()
        {
            if (!Destroyed)
            {
                Target.Pickup(GameManager.Main.EntityPool[GameManager.Main.ControlledEntityId]);
                Destroy();
            }
        }

        public void Show()
        {
            GetComponent<HideableUI>().Show();
        }

        public void Destroy()
        {
            Destroyed = true;
            GetComponent<HideableUI>().Hide();
            GameManager.DelayAction(this, () => { Destroy(gameObject); }, GetComponent<HideableUI>().Transition + .1f);
        }
    }
}
