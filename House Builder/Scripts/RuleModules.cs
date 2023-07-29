using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Data
{
    [CreateAssetMenu(menuName = "HouseBuilder/Rule Modules")]
    public class RuleModules : ScriptableObject
    {
        public List<RuleModule> modulingRules = new List<RuleModule>();
    }
}