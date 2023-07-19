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
        private EnumField _enumModuleTypes;
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

            _enumModuleTypes = new EnumField("Module Type", ModuleType.Wall);
            _enumModuleTypes.RegisterCallback<ChangeEvent<Enum>>(ModuleChangeCallback);

            _transformLabel = new Label();

            this.Add(_paletteSetSelection);
            this.Add(_enumModuleTypes);
            this.Add(_transformLabel);

            var raiseButton = new Button();
            raiseButton.text = "Raise";
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
            }
            _editor.Palettes.ModuleType = (ModuleType)_enumModuleTypes.value;
            UpdateModulesGrid();
        }

        private bool PaletteSetSelectCallback(PaletteSet set)
        {
            _editor.Palettes.PaletteSet = set;
            _editor.Logger.Log(nameof(PlacementView), $"Changed palette set to {set.name}");

            _enumModuleTypes.value = _editor.Palettes.ModuleType;
            UpdateModulesGrid();
            return true;
        }

        private void RaiseCallback()
        {
            _editor.SceneEditor.RaiseHeight();
        }

        private void ModuleChangeCallback(ChangeEvent<Enum> evt)
        {
            ModuleType type = (ModuleType)evt.newValue;
            _editor.Logger.Log(nameof(PlacementView), $"Changed module type to {type}.");
            _editor.Palettes.ModuleType = type;
            UpdateModulesGrid();
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