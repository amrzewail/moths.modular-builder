using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HouseBuilder 
{
    [CreateAssetMenu(menuName = "HouseBuilder/Palette Set")]
    public class PaletteSet : ScriptableObject
    {
        [field: SerializeField] public ModulePalette[] Palettes { get; private set; }

    }
}