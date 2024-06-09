using System.Collections;
using System.Collections.Generic;

using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;

using Unity.VisualScripting;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RockProp : MonoBehaviour
{
    public BoxCollider2D PathfindingObstacle;
    public ObstaclePropData Data { get; private set; }
    public BoxCollider2D Collider => GetComponent<BoxCollider2D>();
    public GameObject ParticleTemplate;

    public float BreakForce;

    public static GameObject Template;

    public static RockProp Create(ObstaclePropData data)
    {
        RockProp prop = Instantiate(Template).GetComponent<RockProp>();
        prop.Data = data;
        prop.GetComponent<SpriteRenderer>().sprite = data.Sprite;
        prop.Collider.size = data.Size;
        prop.PathfindingObstacle.size = data.PathfindingObstacleSize;
        return prop;
    }

    public void UpdateData()
    {
        GetComponent<SpriteRenderer>().sprite = Data.Sprite;
        Collider.size = Data.Size;
        PathfindingObstacle.size = Data.PathfindingObstacleSize;
    }

    public override void PickItem(ItemEntity sender)
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody.velocity.magnitude * collision.rigidbody.mass >= BreakForce)
            Destroy();
    }

    public void Destroy()
    {
        GameObject g = Instantiate(ParticleTemplate, transform.position, Quaternion.identity);
        g.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject);
    }
}

public struct ObstaclePropData
{
    public Sprite Sprite;
    public Vector2 Size, PathfindingObstacleSize;
}