using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HouseBuilder.Editor
{
    public class ModulePointer
    {
        private GameObject _target;
        private GameObject _lastPrefab;

        public GameObject Target => _target;
        public GameObject Prefab => _lastPrefab;

        public void Release()
        {
            if (_target) Object.DestroyImmediate(_target);
            _target = null;
            _lastPrefab = null;
        }

        public void SetSafeCopy(GameObject prefab, Material material = null)
        {
            if (_lastPrefab == prefab) return;

            Release();

            _lastPrefab = prefab;
            _target = GameObject.Instantiate(prefab);
            StageUtility.PlaceGameObjectInCurrentStage(_target);

            if (material)
            {
                Renderer[] renderers = _target.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material = material;
                }
            }
        }
    }
}