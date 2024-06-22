using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public static class Keys
{
    public const int ControlLayer = 10000, UILayer = 20000;

    public static HashSet<int> ObstructionKeys = new() { int.MinValue };
    public static int ObstructionKeyLayer => ObstructionKeys.Max();

    public static bool Down(KeyCode k, int layer = 0)
    {
        return Input.GetKeyDown(k) && layer >= ObstructionKeyLayer;
    }

    public static bool Press(KeyCode k, int layer = 0)
    {
        return Input.GetKey(k) && layer >= ObstructionKeyLayer;
    }

    public static bool Up(KeyCode k, int layer = 0) => Input.GetKeyUp(k) && layer >= ObstructionKeyLayer;

    public static void Obstruct(int layer) => ObstructionKeys.Add(layer);

    public static void Deobstruct(int layer) => ObstructionKeys.Remove(layer);
}
