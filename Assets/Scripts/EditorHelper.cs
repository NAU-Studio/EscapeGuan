#if UNITY_EDITOR

using UnityEditor;

using UnityEngine;

namespace EscapeGuan
{
    public static class EditorHelper
    {
        [MenuItem("Assets/Copy Asset Path")]
        public static void CopyAssetPath()
        {
            if (Selection.assetGUIDs != null && Selection.assetGUIDs.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
                GUIUtility.systemCopyBuffer = path;
            }
        }
    }
}

#endif