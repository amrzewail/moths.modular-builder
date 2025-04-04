using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.ModularBuilder.Editor
{
    public class PrefabButtonVisualElement : Button
    {
        private Texture2D _previewTexture;
        private GameObject _targetPrefab;

        public GameObject prefab => _targetPrefab;

        public new event Action<GameObject> clicked;

        public event Action<GameObject> add;
        public event Action<GameObject> replace;

        public List<VisualElement> onHoverElements = new List<VisualElement>();

        public PrefabButtonVisualElement(GameObject prefab)
        {
            _targetPrefab = prefab;

            this.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;

            Label label = new Label(prefab.name);
            label.AddToClassList("prefab-label");
            this.Add(label);

            var replaceButton = new Button();
            replaceButton.text = "Replace";
            replaceButton.RegisterCallback<ClickEvent>(ReplaceClickCallback);
            replaceButton.AddToClassList("prefab-replace-btn");
            this.Add(replaceButton);
            onHoverElements.Add(replaceButton);

            var addButton = new Button();
            addButton.text = "Add";
            addButton.RegisterCallback<ClickEvent>(AddClickCallback);
            addButton.AddToClassList("prefab-add-btn");
            this.Add(addButton);
            onHoverElements.Add(addButton);

            base.clicked += ClickCallback;

            RefreshPreview();
        }

        private void ReplaceClickCallback(ClickEvent evt)
        {
            evt.StopPropagation();
            replace?.Invoke(prefab);
        }

        private void AddClickCallback(ClickEvent evt)
        {
            evt.StopPropagation();
            add?.Invoke(prefab);
        }


        private async void RefreshPreview()
        {
            _previewTexture = await BuilderEditorUtility.GetAssetTexturePreview(_targetPrefab);
            if (_previewTexture == null) return;
            this.style.backgroundImage = _previewTexture;
            //this.style.minHeight = _previewTexture.height;
            //this.style.maxWidth = _previewTexture.width;

        }

        private void ClickCallback()
        {
            this.clicked?.Invoke(prefab);
        }


    }
}