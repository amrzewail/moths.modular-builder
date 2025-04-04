using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.ModularBuilder.Data
{
    [CreateAssetMenu(menuName = "ModularBuilder/Rule Modules")]
    public class RuleModules : ScriptableObject
    {
        public List<RuleModule> modulingRules = new List<RuleModule>();
    }
}