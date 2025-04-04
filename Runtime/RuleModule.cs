using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.ModularBuilder.Data
{
    [System.Serializable]
    public struct RuleModule
    {
        public string ruleId;
        public List<Rule> rules;
        public GameObject prefab;
        public Quaternion rotation;
        public Vector3 scale;
    }
}