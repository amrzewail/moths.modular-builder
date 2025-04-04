using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Moths.ModularBuilder 
{
    [CreateAssetMenu(menuName = "ModularBuilder/Palette Set")]
    public class PaletteSet : ScriptableObject
    {
        [field: SerializeField] public ModulePalette[] Palettes { get; private set; }


        public void SetPalettes(ModulePalette[] palettes)
        {
            Palettes = palettes;
        }
    }
}