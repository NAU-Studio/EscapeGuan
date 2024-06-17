using UnityEngine;

public static class KeyManager
{
    public const int ControlLayer = 10000, UILayer = 20000;


    public static int ObstructionKeyLayer = -1;

    public static bool KeyDown(KeyCode k, int layer)
    {
        return Input.GetKeyDown(k) && layer >= ObstructionKeyLayer;
    }

    public static bool KeyPress(KeyCode k, int layer)
    {
        return Input.GetKey(k) && layer >= ObstructionKeyLayer;
    }

    public static bool KeyUp(KeyCode k, int layer)
    {
        return Input.GetKeyUp(k) && layer >= ObstructionKeyLayer;
    }
}

public interface IKeyBehaviour
{
    public int KeyLayer { get; }
}