using EscapeGuan.Entities;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackVfx : MonoBehaviour
{
    public Entity Sender;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Entity>() is Entity e && e != Sender)
            Sender.Attack(e);
    }
}
