using System.Collections.Generic;
using DG.Tweening;
using EscapeGuan.Entities;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    public static Dictionary<string, GameObject> VfxTemplates = new();

    public static void CreateLinearTrail(string name, Vector3 position, Vector3 destination, float duration, Ease ease = Ease.Linear)
    {
        GameObject fx = Instantiate(VfxTemplates[name], position, Quaternion.identity);
        fx.transform.DOMove(destination, duration).SetEase(ease);
    }

    public static void CreateLinearAttackTrail(string name, Vector3 position, Vector3 destination, Entity sender, float duration, Ease ease = Ease.Linear)
    {
        GameObject fx = Instantiate(VfxTemplates[name], position, Quaternion.identity);
        fx.transform.DOMove(destination, duration).SetEase(ease);
        fx.GetComponent<AttackVfx>().Sender = sender;
    }
}
