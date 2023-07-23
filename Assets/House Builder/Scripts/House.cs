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
        private Dictionary<GameObject, MeshRenderer[]> _moduleMeshRenderers = new Dictionary<GameObject, MeshRenderer[]>();
        public bool hasReference => this;
        public Vector3 origin => transform.position;

        public Quaternion rotation => transform.rotation;

        [SerializeField] Vector3 _gridCellSize = Vector3.one;
        [SerializeField] int _cellsPerLevel = 3;

        public Vector3 gridSize => _gridCellSize;
        public int gridsPerLevel => _cellsPerLevel;

        private void OnValidate()
        {
            _cellsPerLevel = Mathf.Max(1, _cellsPerLevel);

            CheckUpdateModulesList();
        }

        [ContextMenu("Force Update Modules")]
        private void ForceUpdateModulesList()
        {
            _allModules.Clear();
            _moduleMeshRenderers.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform level = transform.GetChild(i);
                for (int j = 0; j < level.childCount; j++)
                {
                    Transform moduleType = level.GetChild(j);
                    for (int k = 0; k < moduleType.childCount; k++)
                    {
                        var g = moduleType.GetChild(k).gameObject;
                        _allModules.Add(g);
                        _moduleMeshRenderers[g] = g.GetComponentsInChildren<MeshRenderer>();
                    }
                }
            }
        }

        private void CheckUpdateModulesList()
        {
            if (_allModules.Count == 0)
            {
                ForceUpdateModulesList();
            }

            if (_moduleMeshRenderers.Count == 0)
            {
                for (int i = 0; i < _allModules.Count; i++)
                {
                    var g = _allModules[i];
                    _moduleMeshRenderers[g] = g.GetComponentsInChildren<MeshRenderer>();
                }
            }
        }

        public void Add(string type, int level, GameObject module)
        {
            CheckUpdateModulesList();

            Transform levelChild = transform.Find($"Level{level}");
            if (!levelChild)
            {
                levelChild = new GameObject($"Level{level}").transform;
                levelChild.SetParent(transform);
                levelChild.localPosition = Vector3.zero;
            }
            Transform child = levelChild.Find(type);
            if (child == null)
            {
                child = new GameObject(type).transform;
                child.SetParent(levelChild);
                child.localPosition = Vector3.zero;
            }
            module.transform.SetParent(child, true);

            _allModules.Add(module);
            _moduleMeshRenderers[module] = module.GetComponentsInChildren<MeshRenderer>();
        }

        public void Replace(GameObject oldExistingModule, GameObject newModule)
        {
            var parent = oldExistingModule.GetComponentInParent<IHouse>();
            if (parent != (IHouse)this) return;
            newModule.transform.SetParent(oldExistingModule.transform.parent);
            newModule.transform.position = oldExistingModule.transform.position;
            newModule.transform.rotation = oldExistingModule.transform.rotation;
            newModule.transform.localScale = oldExistingModule.transform.localScale;
            _allModules.Add(newModule);
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

        public List<GameObject> GetAtPosition(string type, int level, Vector3 worldPosition, float precision)
        {
            Transform levelChild = transform.Find($"Level{level}");
            _queryResult.Clear();
            if (levelChild)
            {
                Transform child = levelChild.Find(type);
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

        public List<GameObject> GetAllAtHeight(string type, int level, int heightIndex)
        {
            Transform levelChild = transform.Find($"Level{level}");
            List<GameObject> list = new List<GameObject>();
            if (levelChild)
            {
                Transform child = levelChild.Find(type);
                if (child)
                {
                    for (int i = 0; i < child.childCount; i++)
                    {
                        if (Mathf.Abs(child.GetChild(i).transform.localPosition.y - (gridsPerLevel * level + heightIndex * gridSize.y)) > 0.1f) continue;
                        list.Add(child.GetChild(i).gameObject);
                    }
                }
            }
            return list;
        }

        public string GetModuleType(GameObject g)
        {
            return g.transform.parent.name;
        }

        public int GetModuleLevel(Vector3 position)
        {
            var levelIndex = position.y / (gridSize.y * gridsPerLevel);
            if (1f - levelIndex % 1f < 0.01f) return Mathf.CeilToInt(levelIndex);
            else return Mathf.FloorToInt(levelIndex);
        }

        public MeshRenderer[] GetModuleRenderers(GameObject module)
        {
            if (_moduleMeshRenderers.ContainsKey(module)) return _moduleMeshRenderers[module];
            return new MeshRenderer[0];
        }

        public List<Material> GetAllModuleMaterials()
        {
            return GetModulesMaterials(_allModules);
        }

        public List<Material> GetModulesMaterials(List<GameObject> modules)
        {
            List<Material> materials = new List<Material>();
            MeshRenderer[] renderers = null;
            List<Material> rendererMaterials = new List<Material>();
            for (int i = 0; i < modules.Count; i++)
            {
                GameObject g = modules[i];

                if (!g) continue;
                if (!_moduleMeshRenderers.ContainsKey(g)) continue;

                renderers = _moduleMeshRenderers[g];
                for (int j = 0; j < renderers.Length; j++)
                {
                    renderers[j].GetSharedMaterials(rendererMaterials);
                    for(int k = 0; k < rendererMaterials.Count; k++)
                    {
                        if (materials.Contains(rendererMaterials[k])) continue;
                        materials.Add(rendererMaterials[k]);
                    }
                }
            }
            return materials;
        }

        public List<GameObject> GetAllModulesOfMaterial(Material material)
        {
            List<GameObject> modules = new List<GameObject>();
            MeshRenderer[] renderers = null;
            List<Material> rendererMaterials = new List<Material>();

            for (int i = 0; i < _allModules.Count; i++)
            {
                GameObject g = _allModules[i];
                if (!g)
                {
                    _allModules.RemoveAt(i--);
                    continue;
                }
                if (!_moduleMeshRenderers.ContainsKey(g)) continue;
                renderers = _moduleMeshRenderers[g];
                for (int j = 0; j < renderers.Length; j++)
                {
                    renderers[j].GetSharedMaterials(rendererMaterials);
                    for (int k = 0; k < rendererMaterials.Count; k++)
                    {
                        if (rendererMaterials[k] == material)
                        {
                            modules.Add(g);
                            goto NEXT_MODULE;
                        }
                    }

                }
            NEXT_MODULE:
                continue;
            }
            return modules;
        }
    }
}