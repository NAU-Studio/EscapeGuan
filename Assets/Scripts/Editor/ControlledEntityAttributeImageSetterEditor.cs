using UnityEditor;

[CustomEditor(typeof(ControlledEntityAttributeImageSetter))]
public class ControlledEntityAttributeImageSetterEditor : Editor
{
    new ControlledEntityAttributeImageSetter target => (ControlledEntityAttributeImageSetter)base.target;

    SerializedProperty IsSlicedFillImage, AttributeName, MaxAttributeName, Image_SF, Image_UNT;

    private void OnEnable()
    {
        IsSlicedFillImage = serializedObject.FindProperty("IsSlicedFillImage");
        AttributeName = serializedObject.FindProperty("AttributeName");
        MaxAttributeName = serializedObject.FindProperty("MaxAttributeName");
        Image_SF = serializedObject.FindProperty("Image_SF");
        Image_UNT = serializedObject.FindProperty("Image_UNT");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(IsSlicedFillImage);
        EditorGUILayout.PropertyField(AttributeName);
        EditorGUILayout.PropertyField(MaxAttributeName);
        if (target.IsSlicedFillImage)
            EditorGUILayout.PropertyField(Image_SF, label: new("Image"));
        else
            EditorGUILayout.PropertyField(Image_UNT, label: new("Image"));

        serializedObject.ApplyModifiedProperties();
    }
}
