using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface IPaletteManager
    {
        bool IsLoaded { get; }

        PaletteSet[] PaletteSets { get; }
        PaletteSet PaletteSet { get; set; }

        ModuleType ModuleType { get; set; }

        ModulePalette Palette { get; }

        bool LoadPaletteSets();


    }
}