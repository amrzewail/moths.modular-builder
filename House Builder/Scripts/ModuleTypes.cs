using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Data
{
    [CreateAssetMenu(menuName = "HouseBuilder/ModuleTypes")]
    public class ModuleTypes : ScriptableObject
    {
        [SerializeField] string[] _moduleTypes;

        public string[] Types => _moduleTypes == null ? new string[0] : _moduleTypes;
    }
}