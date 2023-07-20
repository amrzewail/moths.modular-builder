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
        private Tabs<PrefabButtonVisualElement> _prefabsTabs;


        public PlacementView(IEditor editor)
        {
            _editor = editor;
            _editor.OnUpdate += Update;
            CreateGUI();

        }


        private void CreateGUI()
        {
            Label shortcuts = new Label();
            shortcuts.style.color = new Color(1, 1, 1, 0.5f);
            shortcuts.text = 
                "LMB: Place modules\n" +
                "Alt + Scroll wheel: Rotate module\n" +
                "Alt + RMB: Flip module x-axis\n" +
                "Ctrl + LMB: Select modules\n" +
                "Backspace: Delete selected\n" +
                "E: Extrude selected up\n" +
                "Esc: Clear selections\n" +
                "";
            this.Add(shortcuts);

            _transformLabel = new Label();
            _transformLabel.style.color = new Color(1, 1, 1, 0.5f);
            this.Add(_transformLabel);

            VisualElement paletteSetAndModuleTypeHorizontal = new VisualElement();
            paletteSetAndModuleTypeHorizontal.style.flexDirection = FlexDirection.Row;
            paletteSetAndModuleTypeHorizontal.style.justifyContent = Justify.SpaceBetween;

            _paletteSetSelection = new SelectionMenu<PaletteSet>("Palette Set");
            _paletteSetSelection.style.flexGrow = 1;
            _paletteSetSelection.onSelected += PaletteSetSelectCallback;
            _paletteSetSelection.isItemDisabled += (paletteSet) => paletteSet.Palettes.Length == 0;

            _moduleTypeSelection = new SelectionMenu<string>("Module Type");
            _moduleTypeSelection.style.flexGrow = 1;
            _moduleTypeSelection.onSelected += ModuleChangeCallback;


            paletteSetAndModuleTypeHorizontal.Add(_paletteSetSelection);
            paletteSetAndModuleTypeHorizontal.Add(_moduleTypeSelection);


            this.Add(paletteSetAndModuleTypeHorizontal);


            var raiseButton = new Button();
            raiseButton.text = "Extrude Up";
            raiseButton.clicked += RaiseCallback;

            this.Add(raiseButton);

            ScrollView scrollView = new ScrollView();
            _prefabsTabs = new Tabs<PrefabButtonVisualElement>();
            _prefabsTabs.AddToClassList("prefabs-tabs");
            _prefabsTabs.onTabClicked += PrefabButtonCallback;
            scrollView.Add(_prefabsTabs);
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
            _prefabsTabs.ClearTabs();
            if (!_editor.Palettes.Palette) return;
            int i = 0;
            foreach(var prefab in _editor.Palettes.Palette.Prefabs)
            {
                var prefabVE = new PrefabButtonVisualElement(prefab);
                _prefabsTabs.AddTab(prefabVE);
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
            _transformLabel.text = $"Rotation: {Mathf.RoundToInt(rotationY)} {(_editor.Previewer.localScale.x < 0 ? "Flipped" : "")}\n";
        }
    }
}