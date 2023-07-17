using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public class HouseModule : MonoBehaviour
    {
        [field: SerializeField] public ModuleType Type { get; private set; }
    }
}