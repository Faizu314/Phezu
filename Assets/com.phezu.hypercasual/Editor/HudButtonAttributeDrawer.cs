using UnityEditor;
using UnityEngine;

namespace Phezu.HyperCasualTemplate.Editor {

    [CustomPropertyDrawer(typeof(HudButtonAttribute))]
    public class HudButtonAttributeDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.type.CompareTo("int") != 0) {
                Debug.LogError("HudButton attribute should be used only for integers");
                return;
            }

            if (HcInputManager.Instance != null)
                DisplayHudButtonsEnum(position, property, label);
            else
                DisplayDefaultInteger(position, property, label);
        }

        private void DisplayHudButtonsEnum(Rect position, SerializedProperty property, GUIContent label) {
            var buttons = HcInputManager.Instance.HudButtons;

            GUIContent[] options = new GUIContent[buttons.Length];

            for (int i = 0; i < options.Length; i++)
                options[i] = new(buttons[i]);

            EditorGUI.BeginProperty(position, label, property);

            property.intValue = EditorGUI.Popup(position, label, property.intValue, options);

            EditorGUI.EndProperty();
        }

        private void DisplayDefaultInteger(Rect position, SerializedProperty property, GUIContent label) {
            Rect intField = position;
            intField.height /= 2f;

            EditorGUI.BeginProperty(intField, label, property);

            property.intValue = EditorGUI.IntField(intField, label, property.intValue);

            EditorGUI.EndProperty();

            Rect logPos = intField;
            logPos.y += intField.height + 2f;

            EditorGUI.LabelField(logPos, "Could not find HcInputManager to display HUD buttons");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float defaultHeight = base.GetPropertyHeight(property, label);
            if (HcInputManager.Instance == null) {
                return (defaultHeight * 2f) + 2f;
            }
            else
                return defaultHeight;
        }
    }
}
