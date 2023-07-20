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

        private SelectionMenu<PaletteSet> _paletteSetSelection;
        private SelectionMenu<string> _moduleTypeSelection;
        private Label _transformLabel;
        private GridVisualElement _grid;


        public PlacementView(IEditor editor)
        {
            _editor = editor;
            _editor.OnUpdate += Update;
            CreateGUI();

        }


        private void CreateGUI()
        {
            _paletteSetSelection = new SelectionMenu<PaletteSet>();
            _paletteSetSelection.onSelected += PaletteSetSelectCallback;
            _paletteSetSelection.isItemDisabled += (paletteSet) => paletteSet.Palettes.Length == 0;

            _moduleTypeSelection = new SelectionMenu<string>();
            _moduleTypeSelection.onSelected += ModuleChangeCallback;

            _transformLabel = new Label();

            this.Add(_paletteSetSelection);
            this.Add(_moduleTypeSelection);
            this.Add(_transformLabel);

            var raiseButton = new Button();
            raiseButton.text = "Extrude Up";
            raiseButton.clicked += RaiseCallback;

            this.Add(raiseButton);

            ScrollView scrollView = new ScrollView();
            _grid = new GridVisualElement();
            scrollView.Add(_grid);
            this.Add(scrollView);
        }


        public void Refresh()
        {
            if (_editor.Palettes.LoadPaletteSets())
            {
                _paletteSetSelection.Refresh(_editor.Palettes.PaletteSets, x => x.name);
                _paletteSetSelection.Select(_editor.Palettes.PaletteSet);

                UpdateModuleTypeList();
                
            }
        }

        private void UpdateModuleTypeList()
        {
            var types = ModuleTypeUtility.GetTypes(_editor.Palettes.PaletteSet);
            if (types.Count > 0)
            {
                _moduleTypeSelection.Refresh(types.ToArray(), x => x);
                _moduleTypeSelection.Select(types[0]);
            }
            else
            {
                _moduleTypeSelection.Refresh(new string[] { "None" }, x => x);
                _moduleTypeSelection.Select("None");
            }
            ModuleChangeCallback(_moduleTypeSelection.Current);
        }

        private bool PaletteSetSelectCallback(PaletteSet set)
        {
            _editor.Palettes.PaletteSet = set;
            _editor.Logger.Log(nameof(PlacementView), $"Changed palette set to {set.name}");

            UpdateModuleTypeList();
            UpdateModulesGrid();
            return true;
        }

        private void RaiseCallback()
        {
            _editor.SceneEditor.ExtrudeHeight();
        }

        private bool ModuleChangeCallback(string type)
        {
            _editor.Logger.Log(nameof(PlacementView), $"Changed module type to {type}.");
            _editor.Palettes.ModuleType = type;
            UpdateModulesGrid();

            return true;
        }

        private void UpdateModulesGrid()
        {
            _grid.Clear();
            if (!_editor.Palettes.Palette) return;
            int i = 0;
            foreach(var prefab in _editor.Palettes.Palette.Prefabs)
            {
                var prefabVE = new PrefabButtonVisualElement(prefab);
                int index = i;
                prefabVE.clicked += g => PrefabButtonCallback(index);
                _grid.Add(prefabVE);
                i++;
            }
            _editor.Logger.Log(nameof(PlacementView), $"Updated modules grid.");
        }

        private void PrefabButtonCallback(int index)
        {
            if (!_editor.IsHouseValid) return;

            _editor.Previewer.SetPrefab(_editor.Palettes.Palette.Prefabs[index]);
        }


        private void Update()
        {
            float rotationY = _editor.Previewer.eulerAngles.y;
            rotationY -= _editor.Grid.rotation.eulerAngles.y;
            _transformLabel.text = $"Rotation: {Mathf.RoundToInt(rotationY)} {(_editor.Previewer.localScale.x < 0 ? "Flipped" : "")}";
        }
    }
}