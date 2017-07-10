using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(DronesManager.DroneConfig))]
public class DroneConfigDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		GUIContent newLabel = new GUIContent ("Drone " + property.FindPropertyRelative ("viz_id").intValue.ToString ());
		newLabel = EditorGUI.BeginProperty (position, newLabel , property);
		Rect contentPosition = EditorGUI.PrefixLabel (position, newLabel);
		contentPosition.width *= 0.75f;
		EditorGUI.indentLevel = 0;
		EditorGUI.PropertyField (contentPosition, property.FindPropertyRelative ("radio_uri"), GUIContent.none);
		contentPosition.x += contentPosition.width;
		contentPosition.width /= 3f;
		EditorGUIUtility.labelWidth = 14f;
		EditorGUI.PropertyField (contentPosition, property.FindPropertyRelative ("color"), GUIContent.none);
		EditorGUI.EndProperty ();
	}
}
