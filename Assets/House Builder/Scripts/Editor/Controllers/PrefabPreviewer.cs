using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace HouseBuilder.Editor.Controllers
{
    public class PrefabPreviewer : IPrefabPreviewer
    {
        private readonly ILogger _logger;

        private readonly ModulePointer _pointer;

        private Vector3? _lastEulerAngles = null;
        private Vector3? _lastLocalScale = null;

        public Vector3 position
        {
            get
            {
                if (!_pointer.Target) return Vector3.zero;
                return _pointer.Target.transform.position;
            }

            set
            {
                if (!_pointer.Target) return;
                _pointer.Target.transform.position = value;
            }
        }
        public Vector3 eulerAngles
        {
            get
            {
                if (!_pointer.Target) return Vector3.zero;
                return _pointer.Target.transform.eulerAngles;
            }

            set
            {
                if (!_pointer.Target) return;
                _pointer.Target.transform.eulerAngles = value;
                _lastEulerAngles = value;
            }
        }
        public Vector3 localScale
        {
            get
            {
                if (!_pointer.Target) return Vector3.one;
                return _pointer.Target.transform.localScale;
            }

            set
            {
                if (!_pointer.Target) return;
                _pointer.Target.transform.localScale = value;
                _lastLocalScale = value;
            }
        }

        public Material material { get; set; }


        public GameObject Prefab { get; private set; }



        public PrefabPreviewer(ILogger logger)
        {
            _logger = logger;
            _pointer = new ModulePointer();
        }

        public void SetPrefab(GameObject prefab)
        {
            _pointer.SetSafeCopy(prefab, material);

            if (_lastLocalScale != null)
            {
                localScale = _lastLocalScale.Value;
            }

            if (_lastEulerAngles != null)
            {
                //Vector3 e = eulerAngles;
                //e.y = _lastEulerAngles.Value.y;
                //eulerAngles = e;
                eulerAngles = _lastEulerAngles.Value;
            }

            if (prefab != Prefab)
            {
                _logger.Log(nameof(PrefabPreviewer), $"Change preview prefab to {prefab.name}.");
            }
            Prefab = prefab;

            Show();
        }

        public void Show()
        {
            if (_pointer.Target)
            {
                if (_pointer.Target.activeSelf == false)
                {
                    _pointer.Target.gameObject.SetActive(true);
                    _logger.Log(nameof(PrefabPreviewer), "Show scene preview.");
                }
            }
        }

        public void Hide()
        {
            if (_pointer.Target)
            {
                if (_pointer.Target.activeSelf == true)
                {
                    _pointer.Target.gameObject.SetActive(false);
                    _logger.Log(nameof(PrefabPreviewer), "Hide scene preview.");
                }
            }
        }

        public void Clean()
        {
            if (!_pointer.Target) return;
            _pointer.Release();
            Prefab = null;
            _logger.Log(nameof(PrefabPreviewer), "Destroy preview.");
        }

    }
}