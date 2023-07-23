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

        private Label _warningLabel;

        private VisualElement _modulePalettesList;
        private int _lastSelectionCount = -1;

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

            var keymap = _editor.Input.GetKeymap(InputContext.Placement);

            Label shortcuts = new Label();
            shortcuts.text = "";
            foreach(var binding in keymap.bindings)
            {
                shortcuts.text += binding.ToString();
                shortcuts.text += "\n";
            }

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

            paletteSetAndModuleTypeHorizontal.Add(_paletteSetSelection);

            this.Add(paletteSetAndModuleTypeHorizontal);

            _modulePalettesList = new VisualElement();
            _modulePalettesList.AddToClassList("placement-palette-list");
            this.Add(_modulePalettesList);


            _warningLabel = new Label();
            _warningLabel.AddToClassList("no-palette-sets-label");
        }


        public void Refresh()
        {
            if (this.Contains(_warningLabel)) this.Remove(_warningLabel);

            if (_editor.Palettes.LoadPaletteSets())
            {
                _paletteSetSelection.Refresh(_editor.Palettes.PaletteSets, x => x.name);
                _paletteSetSelection.Select(_editor.Palettes.CurrentPaletteSet);

                UpdateModuleTypeList();
            }
            else
            {
                _warningLabel.text = "No PalleteSets found in Editor/Resources.\nGo to <b>Edit Palettes</b> tab to create a new PaletteSet.";
                this.Add(_warningLabel);
            }
        }

        private void UpdateModuleTypeList()
        {
            if (this.Contains(_warningLabel)) this.Remove(_warningLabel);

            _modulePalettesList.Clear();
            _palettesElements.Clear();

            var currentSet = _editor.Palettes.CurrentPaletteSet;
            if (currentSet.Palettes == null || currentSet.Palettes.Length == 0)
            {

                _warningLabel.text = $"<i>{currentSet.name}</i> is empty.\n\nGo to <b>Edit Palettes</b> tab then click <b>Edit</b> next to this PalleteSet to add new palettes to <i>{currentSet.name}</i>.";
                this.Add(_warningLabel);
                _editor.Logger.Error(nameof(PlacementView), $"The current selected palette set is empty.");
                return;
            }

            foreach (var palette in currentSet.Palettes)
            {
                ModulePalettePlacementVisualElement modulePaletteVE = new ModulePalettePlacementVisualElement(palette);
                _modulePalettesList.Add(modulePaletteVE);
                modulePaletteVE.selected += (type, prefab) => ModuleSelectedCallback(modulePaletteVE, type, prefab);
                modulePaletteVE.replace += (type, prefab) => ModuleReplaceCallback(modulePaletteVE, type, prefab);
                modulePaletteVE.add += (type, prefab) => ModuleAddCallback(modulePaletteVE, type, prefab);

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


        private void ModuleReplaceCallback(ModulePalettePlacementVisualElement modulePaletteVE, string type, GameObject prefab)
        {
            _editor.SceneEditor.ReplaceSelectionWith(prefab);

        }
        private void ModuleAddCallback(ModulePalettePlacementVisualElement modulePaletteVE, string type, GameObject prefab)
        {
            _editor.SceneEditor.AddPrefabToSelection(prefab);

        }

        private void Update()
        {
            float rotationY = _editor.Previewer.eulerAngles.y;
            rotationY -= _editor.Grid.rotation.eulerAngles.y;
            _transformLabel.text = $"Rotation: {Mathf.RoundToInt(rotationY)} {(_editor.Previewer.localScale.x < 0 ? "Flipped" : "")}\n";

            if (_lastSelectionCount != _editor.Selector.CurrentMultiple.Count)
            {
                foreach(var element in _palettesElements)
                {
                    element.SetHoverElementsVisibility(_editor.Selector.CurrentMultiple.Count > 0);
                }
                _lastSelectionCount = _editor.Selector.CurrentMultiple.Count;
            }
        }
    }
}