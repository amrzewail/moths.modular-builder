using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public class House : MonoBehaviour, IHouse
    {
        private List<GameObject> _queryResult = new List<GameObject>();
        private List<GameObject> _allModules = new List<GameObject>();

        public bool hasReference => this;
        public Vector3 origin => transform.position;

        public Quaternion rotation => transform.rotation;

        [SerializeField] Vector3 _gridSize = Vector3.one;
        [SerializeField] int _levelGridHeight = 3;

        public Vector3 gridSize => _gridSize;
        public int levelGridHeight => _levelGridHeight;

        private void OnValidate()
        {
            _levelGridHeight = Mathf.Max(1, _levelGridHeight);
        }

        private void CheckUpdateModulesList()
        {
            if (_allModules.Count == 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform level = transform.GetChild(i);
                    for(int j = 0; j < level.childCount; j++)
                    {
                        Transform moduleType = level.GetChild(j);
                        for (int k = 0; k < moduleType.childCount; k++)
                        {
                            _allModules.Add(moduleType.GetChild(k).gameObject);
                        }
                    }
                }
            }
        }

        public void Add(ModuleType type, int level, GameObject module)
        {
            CheckUpdateModulesList();

            Transform levelChild = transform.Find($"Level{level}");
            if (!levelChild)
            {
                levelChild = new GameObject($"Level{level}").transform;
                levelChild.SetParent(transform);
                levelChild.localPosition = Vector3.zero;
            }
            Transform child = levelChild.Find(type.ToString());
            if (child == null)
            {
                child = new GameObject(type.ToString()).transform;
                child.SetParent(levelChild);
                child.localPosition = Vector3.zero;
            }
            module.transform.SetParent(child, true);

            _allModules.Add(module);
        }

        public GameObject GetFirstByQuery(Func<GameObject, bool> query)
        {
            CheckUpdateModulesList();
            for (int i = 0; i < _allModules.Count; i++)
            {
                if (!_allModules[i])
                {
                    _allModules.RemoveAt(i);
                    i--;
                    continue;
                }
                if (query(_allModules[i].gameObject))
                {
                    return _allModules[i].gameObject;
                }
            }
            return null;
        }

        public List<GameObject> GetByQuery(Func<GameObject, bool> query)
        {
            CheckUpdateModulesList();
            _queryResult.Clear();
            for(int i = 0; i < _allModules.Count; i++)
            {
                if (!_allModules[i])
                {
                    _allModules.RemoveAt(i);
                    i--;
                    continue;
                }
                if (query(_allModules[i].gameObject))
                {
                    _queryResult.Add(_allModules[i].gameObject);
                }
            }
            return _queryResult;
        }

        public List<GameObject> GetAtPosition(ModuleType type, int level, Vector3 worldPosition, float precision)
        {
            Transform levelChild = transform.Find($"Level{level}");
            _queryResult.Clear();
            if (levelChild)
            {
                Transform child = levelChild.Find(type.ToString());
                if (child)
                {
                    for (int i = 0; i < child.childCount; i++)
                    {
                        if (Vector3.Distance(child.GetChild(i).position, worldPosition) < precision) _queryResult.Add(child.GetChild(i).gameObject);
                    }
                }
            }
            return _queryResult;

        }

        public void HideLevelRange(int minInclusive, int maxExlusive)
        {
            int childCount = transform.childCount;
            int levelTxtLength = "Level".Length;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                int childLevel = int.Parse(child.name.Substring(levelTxtLength, child.name.Length - levelTxtLength));
                child.gameObject.SetActive(childLevel < minInclusive || childLevel >= maxExlusive);
            }
        }

        public void ShowAllLevels()
        {
            if (!transform) return;
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
        }

        public List<GameObject> GetAllAtHeight(ModuleType type, int level, int heightIndex)
        {
            Transform levelChild = transform.Find($"Level{level}");
            List<GameObject> list = new List<GameObject>();
            if (levelChild)
            {
                Transform child = levelChild.Find(type.ToString());
                if (child)
                {
                    for (int i = 0; i < child.childCount; i++)
                    {
                        if (Mathf.Abs(child.GetChild(i).transform.localPosition.y - (levelGridHeight * level + heightIndex * gridSize.y)) > 0.1f) continue;
                        list.Add(child.GetChild(i).gameObject);
                    }
                }
            }
            return list;
        }
    }
}