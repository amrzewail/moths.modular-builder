using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HouseBuilder 
{
    [CreateAssetMenu(menuName = "HouseBuilder/Module Palette")]
    public class ModulePalette : ScriptableObject
    {
        [SerializeField] string _type;
        [SerializeField] GameObject[] _prefabs;

        public string Type => _type;

        public GameObject[] Prefabs => _prefabs;

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