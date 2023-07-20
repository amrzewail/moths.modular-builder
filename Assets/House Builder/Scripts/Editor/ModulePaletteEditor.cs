using HouseBuilder.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor
{
    using Editor = UnityEditor.Editor;
    [CustomEditor(typeof(ModulePalette))]
    [CanEditMultipleObjects]
    public class ModulePaletteEditor : Editor
    {
        private SerializedProperty _typesProperty;
        private SerializedProperty _prefabsProperty;

        private List<string> _availableTypes;

        public void OnEnable()
        {

            _availableTypes = ModuleTypeUtility.GetTypes();

            _typesProperty = serializedObject.FindProperty("_type");
            _prefabsProperty = serializedObject.FindProperty("_prefabs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Type");

            if (EditorGUILayout.DropdownButton(new GUIContent(_typesProperty.stringValue), FocusType.Passive, GUILayout.ExpandWidth(true)))
            {
                GenericMenu menu = new GenericMenu();
                for(int i = 0; i < _availableTypes.Count; i++)
                {
                    var type = _availableTypes[i];
                    menu.AddItem(new GUIContent(type), _typesProperty.stringValue.Equals(type), () =>
                    {
                        _typesProperty.stringValue = type.ToString();
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(_prefabsProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}