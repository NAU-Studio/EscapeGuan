using System.Collections;
using System.Collections.Generic;

using EscapeGuan.Entities;
using EscapeGuan.Entities.Items;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RockProp : Entity
{
    public BoxCollider2D PathfindingObstacle;
    public ObstaclePropData Data { get; private set; }
    public BoxCollider2D Collider => GetComponent<BoxCollider2D>();
    
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
}

public struct ObstaclePropData
{
    public Sprite Sprite;
    public Vector2 Size, PathfindingObstacleSize;
}