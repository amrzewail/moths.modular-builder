using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class PrefabButtonVisualElement : Button
    {
        private Texture2D _previewTexture;
        private GameObject _targetPrefab;

        public GameObject prefab => _targetPrefab;

        public new event Action<GameObject> clicked;
        public event Action<GameObject> replace;

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

            base.clicked += ClickCallback;

            RefreshPreview();
        }

        private void ReplaceClickCallback(ClickEvent evt)
        {
            Debug.Log($"{nameof(PrefabButtonVisualElement)} Replace click callback");
            evt.StopPropagation();
            replace?.Invoke(prefab);
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