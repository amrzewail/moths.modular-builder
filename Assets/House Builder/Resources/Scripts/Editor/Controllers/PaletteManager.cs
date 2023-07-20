using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class PaletteManager : IPaletteManager
    {
        private string _currentModuleType;
        private PaletteSet _currentPaletteSet;
        private ILogger _logger;

        public bool IsLoaded { get; private set; }

        public PaletteSet[] PaletteSets { get; private set; }


        public PaletteSet PaletteSet
        {
            get => _currentPaletteSet;
            set
            {
                _currentPaletteSet = value;
                ModuleType = "";
                if (!PaletteSet) return;
                if (PaletteSet.Palettes.Length > 0)
                {
                    Palette = PaletteSet.Palettes[0];
                    if (Palette)
                    {
                        ModuleType = Palette.Type;
                    }
                }
            }
        }
        public string ModuleType
        {
            get => _currentModuleType;
            set
            {
                _currentModuleType = value;
                Palette = null;
                if (!PaletteSet) return;
                if (PaletteSet.Palettes == null) return;

                for (int i = 0; i < PaletteSet.Palettes.Length; i++)
                {
                    if (!PaletteSet.Palettes[i])
                    {
                        _logger.Error(nameof(PaletteManager), $"Empty palette array element in {PaletteSet.name}.");
                        continue;
                    }
                    if (PaletteSet.Palettes[i].Type == ModuleType)
                    {
                        Palette = PaletteSet.Palettes[i];
                        break;
                    }
                }
            }
        }

        public ModulePalette Palette { get; private set; }

        public PaletteManager(ILogger logger)
        {
            _logger = logger;
        }

        public bool LoadPaletteSets()
        {
            IsLoaded = false;
            PaletteSets = Resources.LoadAll<PaletteSet>("");
            if (PaletteSets.Length > 0)
            {
                if (!PaletteSet) PaletteSet = PaletteSets[0];
                if (PaletteSet.Palettes.Length > 0)
                {
                    Palette = PaletteSet.Palettes[0];
                    ModuleType = Palette.Type;
                }
                IsLoaded = true;
                _logger.Log(nameof(PaletteManager), "Palette sets loaded successfully.");
                return true;
            }
            _logger.Error(nameof(PaletteManager), "No palette sets found in Resources.");
            return false;
        }
    }
}