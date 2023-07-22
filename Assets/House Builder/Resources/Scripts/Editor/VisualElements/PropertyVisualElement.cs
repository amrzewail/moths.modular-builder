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
    public class PropertyVisualElement<T> : VisualElement where T : UnityEngine.Object
    {
        private T _obj;
        private ObjectField _propertyField;
        private VisualElement _preview;
        private Label _previewLabel;
        private Button _deleteBtn;

        public event Func<T, bool> propertyChanged;
        public event Action create;

        public string imageFallbackText { get => _previewLabel.text; set => _previewLabel.text = value; }

        public PropertyVisualElement(T obj)
        {
            this.AddToClassList("object-property");

            _obj = obj;


            _preview = new VisualElement();
            _preview.AddToClassList("preview");
            _preview.RegisterCallback<ClickEvent>(PreviewClickCallback);

            _preview.RegisterCallback<DragEnterEvent>(DragEnterCallback);
            _preview.RegisterCallback<DragLeaveEvent>(DragLeaveCallback);
            _preview.RegisterCallback<DragUpdatedEvent>(DragUpdatedCallback);
            _preview.RegisterCallback<DragPerformEvent>(DragPerformCallback);
            this.Add(_preview);

            _previewLabel = new Label();
            _previewLabel.AddToClassList("fallback-label");

            _deleteBtn = new Button();
            _deleteBtn.AddToClassList("delete-btn");
            _deleteBtn.text = "X";
            _deleteBtn.RegisterCallback<ClickEvent>(DeleteClickCallback);

            _propertyField = new ObjectField();
            _propertyField.objectType = typeof(T);
            _propertyField.allowSceneObjects = false;
            _propertyField.value = _obj;
            _propertyField.AddToClassList("object-field");
            _propertyField.RegisterValueChangedCallback(PropertyChangedCallback);
            this.Add(_propertyField);


            UpdatePreview();
        }

        private void DeleteClickCallback(ClickEvent evt)
        {
            evt.StopPropagation();
            Change(null);
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
            if (references[0] is T)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
        private void DragPerformCallback(DragPerformEvent evt)
        {
            var references = DragAndDrop.objectReferences;
            if (references == null || references.Length == 0) return;
            if (references[0] is T)
            {
                DragAndDrop.AcceptDrag();
                Change((T)references[0]);
            }
        }

        private void PropertyChangedCallback(ChangeEvent<UnityEngine.Object> evt)
        {
            _propertyField.SetValueWithoutNotify(null);
            Change((T)evt.newValue);
        }

        private void Change(T obj)
        {
            if (propertyChanged == null || propertyChanged(obj))
            {
                _obj = obj;
                _propertyField.SetValueWithoutNotify(_obj);
                UpdatePreview();
            }
        }

        private void PreviewClickCallback(ClickEvent evt)
        {
            if (!_obj)
            {
                create?.Invoke();
                return;
            }
            EditorGUIUtility.PingObject(_obj);
        }

        private async void UpdatePreview()
        {
            if (!_obj)
            {
                _preview.style.backgroundImage = null;
                if (_preview.Contains(_deleteBtn)) _preview.Remove(_deleteBtn);
                if (!_preview.Contains(_previewLabel)) _preview.Add(_previewLabel);
                return;
            }
            var texture = await BuilderEditorUtility.GetAssetTexturePreview(_obj);
            _preview.style.backgroundImage = texture;
            _preview.Add(_deleteBtn);
        }
    }
}