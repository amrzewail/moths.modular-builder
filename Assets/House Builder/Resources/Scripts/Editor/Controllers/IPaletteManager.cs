using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface IPaletteManager
    {
        bool IsLoaded { get; }

        PaletteSet[] PaletteSets { get; }
        PaletteSet CurrentPaletteSet { get; set; }

        string CurrentModuleType { get; set; }

        bool LoadPaletteSets();


    }
}