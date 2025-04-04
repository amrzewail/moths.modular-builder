using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Moths.ModularBuilder 
{
    [CreateAssetMenu(menuName = "ModularBuilder/Module Palette")]
    public class ModulePalette : ScriptableObject
    {
        [SerializeField] string _type;
        [SerializeField] GameObject[] _prefabs;

        public string Type => _type;

        public GameObject[] Prefabs => _prefabs == null ? new GameObject[0] : _prefabs;

        public void SetType(string type)
        {
            _type = type;
        }

        public void SetPrefabs(GameObject[] prefabs)
        {
            _prefabs = prefabs;
        }

    }
}