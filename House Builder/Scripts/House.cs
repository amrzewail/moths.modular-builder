using HouseBuilder.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public class House : MonoBehaviour, IHouse
    {
        [System.Serializable]
        private struct ModuleGroup
        {
            public Vector3Int center;
            public List<GameObject> modules;
        };

        private List<ModuleGroup> _positionGroups = new List<ModuleGroup>();
        private List<GameObject> _queryResult = new List<GameObject>();
        private List<GameObject> _allModules = new List<GameObject>();
        private Dictionary<GameObject, MeshRenderer[]> _moduleMeshRenderers = new Dictionary<GameObject, MeshRenderer[]>();
        private List<Rule> _rulesResult = new List<Rule>();

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
            _positionGroups.Clear();

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
                        AddModuleToGroup(g);
                    }
                }
            }
        }

        private void AddModuleToGroup(GameObject g)
        {
            var group = GetGroupByWorldPosition(g.transform.position);
            if (group != null)
            {
                group.Value.modules.Add(g);
            }
            else
            {
                group = new ModuleGroup
                {
                    center = WorldToGroupPosition(g.transform.position),
                    modules = new List<GameObject> { g }
                };
                _positionGroups.Add(group.Value);
            }
        }

        private Vector3 WorldToHousePosition(Vector3 worldPosition)
        {
            return transform.InverseTransformPoint(worldPosition);
        }

        private Vector3Int WorldToGroupPosition(Vector3 worldPosition)
        {
            Vector3 housePosition = WorldToHousePosition(worldPosition);
            Vector3 groupCellSize = _gridCellSize * 3 + Vector3.one * 0.001f;
            Vector3Int groupPosition = Vector3Int.zero;
            groupPosition.x = (int)(housePosition.x / groupCellSize.x);
            groupPosition.y = (int)(housePosition.y / groupCellSize.y);
            groupPosition.z = (int)(housePosition.z / groupCellSize.z);
            return groupPosition;
        }

        private ModuleGroup? GetGroupByWorldPosition(Vector3 worldPosition)
        {
            Vector3 groupPosition = WorldToGroupPosition(worldPosition);

            for (int i = 0; i < _positionGroups.Count; i++)
            {
                if (_positionGroups[i].center == groupPosition)
                {
                    return _positionGroups[i];
                }
            }

            return null;
        }

        private void CheckUpdateModulesList()
        {
            if (_allModules.Count == 0 || _positionGroups.Count == 0 || _moduleMeshRenderers.Count == 0)
            {
                ForceUpdateModulesList();
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
#if UNITY_EDITOR
            UnityEditor.Undo.SetTransformParent(module.transform, child, "Module set house level parent");
#endif
            if (module.TryGetComponent(out HouseModule houseModule))
            {
                houseModule.Refresh(this, true);
            }

            _allModules.Add(module);
            _moduleMeshRenderers[module] = module.GetComponentsInChildren<MeshRenderer>();
            AddModuleToGroup(module);
        }

        public void Replace(GameObject oldExistingModule, GameObject newModule)
        {
            var parent = oldExistingModule.GetComponentInParent<IHouse>();
            if (parent != (IHouse)this) return;
#if UNITY_EDITOR
            UnityEditor.Undo.SetTransformParent(newModule.transform, oldExistingModule.transform.parent, "Module set house level parent");
#endif
            newModule.transform.position = oldExistingModule.transform.position;
            newModule.transform.rotation = oldExistingModule.transform.rotation;
            newModule.transform.localScale = oldExistingModule.transform.localScale;

            _allModules.Add(newModule);
            _moduleMeshRenderers[newModule] = newModule.GetComponentsInChildren<MeshRenderer>();
            AddModuleToGroup(newModule);
        }

        public GameObject GetFirstByQuery(Func<GameObject, bool> query, Vector3? worldPosition)
        {
            CheckUpdateModulesList();

            var searchingList = _allModules;
            if (worldPosition != null)
            {
                var group = GetGroupByWorldPosition(worldPosition.Value);
                if (group != null)
                {
                    searchingList = group.Value.modules;
                }
            }

            for (int i = 0; i < searchingList.Count; i++)
            {
                if (!searchingList[i])
                {
                    searchingList.RemoveAt(i);
                    i--;
                    continue;
                }
                if (query(searchingList[i].gameObject))
                {
                    return searchingList[i].gameObject;
                }
            }
            return null;
        }

        public List<GameObject> GetByQuery(Func<GameObject, bool> query, Vector3? worldPosition)
        {
            CheckUpdateModulesList();
            _queryResult.Clear();

            var searchingList = _allModules;
            if (worldPosition != null)
            {
                var group = GetGroupByWorldPosition(worldPosition.Value);
                if (group != null)
                {
                    searchingList = group.Value.modules;
                }
            }

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
                    _queryResult.Add(_allModules[i].gameObject);
                }
            }
            return _queryResult;
        }

        public List<GameObject> GetAtPosition(Vector3 worldPosition)
        {
            CheckUpdateModulesList();

            _queryResult.Clear();
            var group = GetGroupByWorldPosition(worldPosition);
            if (group != null)
            {
                for (int i = 0; i < group.Value.modules.Count; i++)
                {
                    var module = group.Value.modules[i];
                    if (!module)
                    {
                        group.Value.modules.RemoveAt(i--);
                        continue;
                    }
                    if ((module.transform.position - worldPosition).magnitude < _gridCellSize.magnitude / 8f)
                    {
                        _queryResult.Add(module);
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
                var childName = child.name.Substring(levelTxtLength, child.name.Length - levelTxtLength);
                int childLevel = 0;
                if (!int.TryParse(childName, out childLevel))
                {
                    Debug.LogError($"House:: Selected house has invalid Level child objects");
                    continue;
                }
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

        public List<Rule> GetRulesAtPosition(Vector3 position)
        {
            _rulesResult.Clear();

            GameObject m;
            RuleDirection d;
            float cellDist = 0;

            Vector3 up = rotation * Vector3.up;
            Vector3 forward = rotation * Vector3.forward;
            Vector3 right = rotation * Vector3.right;

            for (int i = 0; i < _allModules.Count; i++)
            {
                m = _allModules[i];
                if (!m)
                {
                    _allModules.RemoveAt(i--);
                    continue;
                }
                Vector3 vector = m.transform.position - position;
                float vectorMagnitude = vector.magnitude;

                //the module is at the same position
                if (vectorMagnitude < _gridCellSize.sqrMagnitude / 8f) continue;

                var type = GetModuleType(m);

                RuleDirection ruleDirection = RuleDirection.Forward;
                float distance;
                if (Mathf.Abs(distance = Vector3.Dot(vector, forward)) >= vectorMagnitude * 0.99f)
                {
                    ruleDirection = distance > 0 ? RuleDirection.Forward : RuleDirection.Backward;
                    cellDist = _gridCellSize.z;
                }
                else if (Mathf.Abs(distance = Vector3.Dot(vector, right)) >= vectorMagnitude * 0.99f)
                {
                    ruleDirection = distance > 0 ? RuleDirection.Right : RuleDirection.Left;
                    cellDist = _gridCellSize.x;
                }
                else if (Mathf.Abs(distance = Vector3.Dot(vector, up)) >= vectorMagnitude * 0.99f)
                {
                    ruleDirection = distance > 0 ? RuleDirection.Up : RuleDirection.Down;
                    cellDist = _gridCellSize.y;
                }
                else
                {
                    continue;
                }


                _rulesResult.Add(new Rule
                {
                    condition = RuleCondition.Exists,
                    direction = ruleDirection,
                    moduleType = type,
                    unitDistance = Mathf.RoundToInt(Mathf.Abs(distance) / cellDist),
                });
            }

            return _rulesResult;
        }
    }
}