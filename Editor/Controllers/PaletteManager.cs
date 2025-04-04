using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.ModularBuilder.Editor.Controllers
{
    public class PaletteManager : IPaletteManager
    {
        private string _currentModuleType;
        private PaletteSet _currentPaletteSet;
        private ILogger _logger;

        public bool IsLoaded { get; private set; }

        public PaletteSet[] PaletteSets { get; private set; }

        public string CurrentModuleType
        {
            get => _currentModuleType;
            set
            {
                if (_currentModuleType == value) return;
                _currentModuleType = value;
                _logger.Log(nameof(PaletteManager), $"Changed module type to {_currentModuleType}.");
            }
        }


        public PaletteSet CurrentPaletteSet
        {
            get => _currentPaletteSet;
            set
            {
                if (_currentPaletteSet == value) return;
                _currentPaletteSet = value;
                if (_currentPaletteSet == null) return;
                _logger.Log(nameof(PaletteManager), $"Changed palette set to {_currentPaletteSet.name}");

            }
        }

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
                if (!CurrentPaletteSet) CurrentPaletteSet = PaletteSets[0];
                IsLoaded = true;
                _logger.Log(nameof(PaletteManager), "Palette sets loaded successfully.");
                return true;
            }
            else
            {
                CurrentPaletteSet = null;
            }
            _logger.Error(nameof(PaletteManager), "No palette sets found in Resources.");
            return false;
        }
    }
}