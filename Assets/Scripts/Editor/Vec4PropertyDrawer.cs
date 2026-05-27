using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Editor
{
	public class Vec4PropertyDrawer: PropertyDrawer
	{
        /*public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 150;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty x = property.FindPropertyRelative("_x");
            SerializedProperty y = property.FindPropertyRelative("_y");
            SerializedProperty z = property.FindPropertyRelative("_z");
            SerializedProperty w = property.FindPropertyRelative("_w");

            EditorGUI.PropertyField(new Rect(position.x, position.y + 20, 300, 20), x);
        }*/
	}
}