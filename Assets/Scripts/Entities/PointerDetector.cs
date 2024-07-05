using EscapeGuan.Entities;

using UnityEngine;

public class PointerDetector : MonoBehaviour
{
    public Entity Entity => GetComponentInParent<Entity>();

    protected virtual void OnMouseEnter()
    {
        Entity.Pointed = true;
    }

    private void OnMouseExit()
    {
        Entity.Pointed = false;
    }
}
