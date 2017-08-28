using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(DroneConfig))]
public class DroneConfigDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		GUIContent newLabel = new GUIContent ("Drone " + property.FindPropertyRelative ("viz_id").intValue.ToString ());
		newLabel = EditorGUI.BeginProperty (position, newLabel , property);
		Rect contentPosition = EditorGUI.PrefixLabel (position, newLabel);
        float fullWidth = contentPosition.width;
        float fullHeight = contentPosition.height;

        EditorGUI.indentLevel = 0;

        contentPosition.width = 0.05f * fullWidth;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("is_active"), GUIContent.none);
        contentPosition.x += contentPosition.width;

        contentPosition.width = 0.70f * fullWidth;
		EditorGUI.PropertyField (contentPosition, property.FindPropertyRelative ("radio_uri"), GUIContent.none);
		contentPosition.x += contentPosition.width;

        contentPosition.width = 0.25f * fullWidth;
		EditorGUIUtility.labelWidth = 14f;
		EditorGUI.PropertyField (contentPosition, property.FindPropertyRelative ("color"), GUIContent.none);

        EditorGUI.EndProperty ();
	}
}
