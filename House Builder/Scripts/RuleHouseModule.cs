using HouseBuilder.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public class RuleHouseModule : HouseModule
    {
        [SerializeField] GameObject defaultPrefab;
        [SerializeField] RuleModules rules;



        private GameObject _currentPrefab;
        private GameObject _currentInstantiatedModule;

        public override GameObject previewGameObject => defaultPrefab ? defaultPrefab : this.gameObject;

        private GameObject GetCurrentModule()
        {
            if (!_currentInstantiatedModule)
            {
                if (transform.childCount > 0)
                {
                    _currentInstantiatedModule = transform.GetChild(0).gameObject;
                }
            }
            return _currentInstantiatedModule;
        }

        private GameObject InstantiatePrefab(GameObject prefab)
        {
#if UNITY_EDITOR
            if (_currentPrefab == prefab) return _currentInstantiatedModule;


            if (_currentInstantiatedModule)
            {
                DestroyImmediate(_currentInstantiatedModule);
            }
            _currentInstantiatedModule = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, this.transform);
            _currentPrefab = prefab;
            _currentInstantiatedModule.name = prefab.name;
            return _currentInstantiatedModule;
#endif
            return null;
        }

        public override void Refresh(IHouse house, bool refreshNeighbours)
        {
            transform.rotation = house.rotation;
            transform.localScale = Vector3.one;

            var results = house.GetRulesAtPosition(transform.position);

            RuleModule? module = null;
            GameObject prefab = defaultPrefab;

            for(int i = 0; i < rules.modulingRules.Count; i++)
            {
                module = rules.modulingRules[i];

                bool isModuleRulePassed = true;

                for (int j = 0; j < module.Value.rules.Count; j++)
                {
                    var rule = module.Value.rules[j];
                    bool isRulePassed = false;

                    for (int k = 0; k < results.Count; k++)
                    {
                        var resultRule = results[k];

                        if (rule.Equals(resultRule))
                        {
                            isRulePassed = true;
                            break;
                        } 
                    }

                    if (!isRulePassed)
                    {
                        isModuleRulePassed = false;
                        break;
                    }
                }

                if (isModuleRulePassed)
                {
                    prefab = module.Value.prefab;
                    break;
                }

                module = null;
            }

            GameObject g = InstantiatePrefab(prefab);
            g.transform.localPosition = Vector3.zero;
            if (module.HasValue)
            {
                g.transform.localRotation = module.Value.rotation;
                g.transform.localScale = module.Value.scale;
            }
            else
            {
                g.transform.localRotation = prefab.transform.localRotation;
                g.transform.localScale = prefab.transform.localScale;
            }

            if (refreshNeighbours)
            {
                //var modules = house.GetByQuery(x =>
                //{
                //    var h = x.GetComponent<HouseModule>();
                //    if (!h) return false;
                //    h.Refresh(house, false);
                //    return true;
                //});
            }


        }
    }
}