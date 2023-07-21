using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class PrefabPropertyVisualElement : VisualElement
    {
        private GameObject _prefab;
        private ObjectField _propertyField;
        private VisualElement _preview;
        private Button _deleteBtn;

        public event Action<GameObject> prefabChanged;

        public PrefabPropertyVisualElement(GameObject prefab)
        {
            this.AddToClassList("prefab-property");

            _prefab = prefab;


            _preview = new VisualElement();
            _preview.AddToClassList("preview");
            _preview.RegisterCallback<ClickEvent>(PreviewClickCallback);

            _preview.RegisterCallback<DragEnterEvent>(DragEnterCallback);
            _preview.RegisterCallback<DragLeaveEvent>(DragLeaveCallback);
            _preview.RegisterCallback<DragUpdatedEvent>(DragUpdatedCallback);
            _preview.RegisterCallback<DragPerformEvent>(DragPerformCallback);
            this.Add(_preview);

            _deleteBtn = new Button();
            _deleteBtn.AddToClassList("delete-btn");
            _deleteBtn.text = "X";
            _deleteBtn.RegisterCallback<ClickEvent>(DeleteClickCallback);

            _propertyField = new ObjectField();
            _propertyField.objectType = typeof(GameObject);
            _propertyField.allowSceneObjects = false;
            _propertyField.value = _prefab;
            _propertyField.AddToClassList("object-field");
            _propertyField.RegisterValueChangedCallback(PrefabChangedCallback);
            this.Add(_propertyField);


            UpdatePreview();
        }

        private void DeleteClickCallback(ClickEvent evt)
        {
            evt.StopPropagation();
            ChangePrefab(null);
        }

        private void DragLeaveCallback(DragLeaveEvent evt)
        {
            _preview.RemoveFromClassList("selected");
        }

        private void DragEnterCallback(DragEnterEvent evt)
        {
            _preview.AddToClassList("selected");
        }


        private void DragUpdatedCallback(DragUpdatedEvent evt)
        {
            var references = DragAndDrop.objectReferences;
            if (references == null || references.Length == 0) return;
            if (references[0] is GameObject)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
        private void DragPerformCallback(DragPerformEvent evt)
        {
            var references = DragAndDrop.objectReferences;
            if (references == null || references.Length == 0) return;
            if (references[0] is GameObject)
            {
                DragAndDrop.AcceptDrag();
                ChangePrefab((GameObject)references[0]);
            }
        }

        private void PrefabChangedCallback(ChangeEvent<UnityEngine.Object> evt)
        {
            ChangePrefab((GameObject)evt.newValue);
        }

        private void ChangePrefab(GameObject prefab)
        {
            _prefab = prefab;
            _propertyField.SetValueWithoutNotify(_prefab);
            prefabChanged?.Invoke(_prefab);
            UpdatePreview();
        }

        private void PreviewClickCallback(ClickEvent evt)
        {
            if (!_prefab)
            {
                return;
            }
            EditorGUIUtility.PingObject(_prefab);
        }

        private async void UpdatePreview()
        {
            if (!_prefab)
            {
                _preview.style.backgroundImage = null;
                if (_preview.Contains(_deleteBtn)) _preview.Remove(_deleteBtn);
                return;
            }
            var texture = await BuilderEditorUtility.GetAssetTexturePreview(_prefab);
            _preview.style.backgroundImage = texture;
            _preview.Add(_deleteBtn);
        }
    }
}