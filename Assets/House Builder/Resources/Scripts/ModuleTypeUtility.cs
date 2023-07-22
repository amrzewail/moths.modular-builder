using HouseBuilder.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public static class ModuleTypeUtility
    {
        public static List<string> GetTypes()
        {
            var moduleTypesAll = Resources.LoadAll<ModuleTypes>("");
            List<string> _availableTypes = new List<string>();
            foreach (var moduleTypes in moduleTypesAll)
            {
                var types = moduleTypes.Types;
                foreach (var type in types)
                {
                    if (_availableTypes.Contains(type)) continue;
                    _availableTypes.Add(type);
                }
            }
            return _availableTypes;
        }

        public static List<string> GetTypes(PaletteSet set)
        {
            List<string> _availableTypes = new List<string>();
            foreach (var palette in set.Palettes)
            {
                if (_availableTypes.Contains(palette.Type)) continue;
                _availableTypes.Add(palette.Type);
            }
            return _availableTypes;
        }
    }
}