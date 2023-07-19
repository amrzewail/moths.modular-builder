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

        public PrefabButtonVisualElement(GameObject prefab)
        {
            _targetPrefab = prefab;

            this.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;


            Label label = new Label(prefab.name);
            this.Add(label);

            base.clicked += ClickCallback;

            RefreshPreview();
        }

        private async void RefreshPreview()
        {
            _previewTexture = null;
            while(_previewTexture == null)
            {
                _previewTexture = AssetPreview.GetAssetPreview(prefab);

                this.style.backgroundImage = _previewTexture;

                if (_previewTexture == null) await System.Threading.Tasks.Task.Delay(100);
            }
            this.style.minHeight = _previewTexture.height;
            this.style.maxWidth = _previewTexture.width;

        }

        private void ClickCallback()
        {
            this.clicked?.Invoke(prefab);
        }


    }
}