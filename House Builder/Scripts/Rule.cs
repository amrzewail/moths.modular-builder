using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Data
{
    [System.Serializable]
    public struct Rule
    {
        public RuleDirection direction;
        public RuleCondition condition;
        public string moduleType;
        public int unitDistance;

        public override bool Equals(object obj)
        {
            if (obj is Rule == false) return base.Equals(obj);
            Rule other = (Rule)obj;
            return direction == other.direction && condition == other.condition && moduleType == other.moduleType && unitDistance == other.unitDistance;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(direction, condition, moduleType, unitDistance);
        }
    }
}