using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HouseBuilder 
{
    [CreateAssetMenu(menuName = "HouseBuilder/Module Palette")]
    public class ModulePalette : ScriptableObject
    {
        [field: SerializeField] public ModuleType Type { get; private set; }

        [field: SerializeField] public GameObject[] Prefabs { get; private set; }
    }
}