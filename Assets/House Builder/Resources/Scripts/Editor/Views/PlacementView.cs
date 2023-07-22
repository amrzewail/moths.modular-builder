using HouseBuilder.Editor.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor.Views
{
    public class PlacementView : VisualElement
    {
        private readonly IEditor _editor;

        private List<ModulePalettePlacementVisualElement> _palettesElements = new List<ModulePalettePlacementVisualElement>();

        private SelectionMenu<PaletteSet> _paletteSetSelection;
        private Label _transformLabel;
        private VisualElement _modulePalettesList;

        public PlacementView(IEditor editor)
        {

            _editor = editor;
            _editor.OnUpdate += Update;

            _palettesElements = new List<ModulePalettePlacementVisualElement>();

            CreateGUI();

        }


        private void CreateGUI()
        {
            this.AddToClassList("placement-view");

            Label shortcuts = new Label();
            shortcuts.text = 
                "LMB: Place modules\n" +
                "Alt + Scroll wheel: Rotate module\n" +
                "Alt + RMB: Flip module x-axis\n" +
                "Ctrl + LMB: Select modules\n" +
                "Backspace: Delete selected\n" +
                "E: Extrude selected up\n" +
                "Esc: Clear selections\n" +
                "";
            shortcuts.AddToClassList("shortcuts-label");
            this.Add(shortcuts);

            _transformLabel = new Label();
            _transformLabel.AddToClassList("transforms-label");
            this.Add(_transformLabel);

            VisualElement paletteSetAndModuleTypeHorizontal = new VisualElement();
            paletteSetAndModuleTypeHorizontal.style.flexDirection = FlexDirection.Row;
            paletteSetAndModuleTypeHorizontal.style.justifyContent = Justify.SpaceBetween;

            _paletteSetSelection = new SelectionMenu<PaletteSet>("Palette Set");
            _paletteSetSelection.style.flexGrow = 1;
            _paletteSetSelection.onSelected += PaletteSetSelectCallback;
            _paletteSetSelection.isItemDisabled += (paletteSet) => paletteSet.Palettes.Length == 0;

            paletteSetAndModuleTypeHorizontal.Add(_paletteSetSelection);

            this.Add(paletteSetAndModuleTypeHorizontal);


            //var raiseButton = new Button();
            //raiseButton.text = "Extrude Up";
            //raiseButton.clicked += RaiseCallback;

            //this.Add(raiseButton);

            _modulePalettesList = new VisualElement();
            _modulePalettesList.AddToClassList("placement-palette-list");
            this.Add(_modulePalettesList);

            //_modulePalette = new VisualElement();
            //_modulePalette.AddToClassList("module-palette-element");
            //this.Add(_modulePalette);

            //_prefabsTabs = new Tabs<PrefabButtonVisualElement>();
            //_prefabsTabs.AddToClassList("prefabs-tabs");
            //_prefabsTabs.onTabClicked += PrefabButtonCallback;
            //_modulePalette.Add(_prefabsTabs);
        }


        public void Refresh()
        {
            if (_editor.Palettes.LoadPaletteSets())
            {
                _paletteSetSelection.Refresh(_editor.Palettes.PaletteSets, x => x.name);
                _paletteSetSelection.Select(_editor.Palettes.CurrentPaletteSet);

                UpdateModuleTypeList();
                
            }
        }

        private void UpdateModuleTypeList()
        {
            _modulePalettesList.Clear();
            _palettesElements.Clear();

            var currentSet = _editor.Palettes.CurrentPaletteSet;
            if (currentSet.Palettes == null) return;


            foreach (var palette in currentSet.Palettes)
            {
                ModulePalettePlacementVisualElement modulePaletteVE = new ModulePalettePlacementVisualElement(palette);
                _modulePalettesList.Add(modulePaletteVE);
                modulePaletteVE.selected += (type, prefab) => ModuleSelectedCallback(modulePaletteVE, type, prefab);
                _palettesElements.Add(modulePaletteVE);
            }

            _editor.Logger.Log(nameof(PlacementView), $"Updated modules list.");
        }

        private bool PaletteSetSelectCallback(PaletteSet set)
        {
            _editor.Palettes.CurrentPaletteSet = set;

            UpdateModuleTypeList();
            return true;
        }

        private void RaiseCallback()
        {
            _editor.SceneEditor.ExtrudeHeight();
        }

        private void ModuleSelectedCallback(ModulePalettePlacementVisualElement element, string moduleType, GameObject prefab)
        {
            if (!_editor.IsHouseValid) return;

            _editor.Palettes.CurrentModuleType = moduleType;
            _editor.Previewer.SetPrefab(prefab);

            foreach(var p in _palettesElements)
            {
                if (p == element) continue;
                p.ClearSelection();
            }
        }


        private void Update()
        {
            float rotationY = _editor.Previewer.eulerAngles.y;
            rotationY -= _editor.Grid.rotation.eulerAngles.y;
            _transformLabel.text = $"Rotation: {Mathf.RoundToInt(rotationY)} {(_editor.Previewer.localScale.x < 0 ? "Flipped" : "")}\n";
        }
    }
}