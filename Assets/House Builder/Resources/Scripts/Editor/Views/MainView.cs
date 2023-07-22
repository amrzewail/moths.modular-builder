using HouseBuilder.Editor.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor.Views
{
    public class MainView : ScrollView
    {
        private readonly IEditor _editor;

        private VisualElement _header;
        private VisualElement _body;


        private Tabs<Button> _tabs;

        private Tabs<Button> _heightTabs;
        private Toggle _showAllLevels;

        private VisualElement _newHouseView;

        private PlacementView _placementView;
        private PalettesView _palettesView;

        private int _lastHouseMaxHeightCount = 0;

        public MainView(IEditor editor)
        {
            this.AddToClassList("main-view");
            this.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;

            _editor = editor;
            _editor.OnSelectionChanged += OnSelectionChanged;
            _editor.OnBeforeSelectionChange += OnBeforeSelectionChange;
            _editor.OnUpdate += Update;

            _header = new VisualElement();
            _body = new VisualElement();

            this.Add(_header);
            this.Add(_body);


            //_levelTabs = new ButtonTabs("0", "1", "2", "3", "4", "5", "6", "7", "8", "9");
            //_levelTabs.label.text = "Level";
            //_levelTabs.onTabClicked += LevelTabClickCallback;

            _heightTabs = new Tabs<Button>();
            for(int i = 0; i < 31; i++)
            {
                Button btn = new Button();
                btn.text = $"{i}";
                _heightTabs.AddTab(btn);
            }

            _heightTabs.label = "Height";
            _heightTabs.onTabClicked += HeightTabClickCallback;
            _heightTabs.AddToClassList("height-tabs-container");

            _showAllLevels = new Toggle("Show all levels");
            _showAllLevels.RegisterCallback<ChangeEvent<bool>>(ShowAllLevelsToggle);
            _showAllLevels.AddToClassList("show-levels");

            //_header.Add(_levelTabs);
            _header.Add(_heightTabs);
            _header.Add(_showAllLevels);


            _newHouseView = new VisualElement();
            _newHouseView.AddToClassList("new-house-view");

            _placementView = new PlacementView(_editor);
            _palettesView = new PalettesView(_editor);


            _tabs = new Tabs<Button>(
                new Button { text = "Placement" },
                new Button { text = "Editing" },
                new Button { text = "Palettes" });
            _tabs.onTabClicked += TabClickCallback;
            _tabs.Click(0);
            _tabs.AddToClassList("edit-tabs");

            _header.Add(_tabs);

            var newHouseButton = new Button();
            newHouseButton.text = "Create New House";
            newHouseButton.clicked += NewHouseClickCallback;
            newHouseButton.AddToClassList("new-house-btn");
            _newHouseView.Add(newHouseButton);



            UpdateVisualElements();

        }

        private void ShowAllLevelsToggle(ChangeEvent<bool> evt)
        {
            if (!_editor.IsHouseValid)
            {
                evt.PreventDefault();
                return;
            }
            UpdateHouseVisibility();
        }

        private void HeightTabClickCallback(int index)
        {
            _editor.Grid.totalHeightIndex = index;
            UpdateHouseVisibility();
            SceneView.RepaintAll();
        }

        private void LevelTabClickCallback(int index, string arg2)
        {
            _editor.Grid.CurrentLevelIndex = index;
            UpdateHouseVisibility();
            SceneView.RepaintAll();
        }



        private void TabClickCallback(int index)
        {
            UpdateVisualElements();
        }

        private void NewHouseClickCallback()
        {
            GameObject g = new GameObject("New House", typeof(House));
            Selection.activeGameObject = g;
        }

        private void UpdateVisualElements()
        {
            _body.Clear();

            switch (_tabs.Current)
            {
                case 0:
                    if (!_editor.IsHouseValid)
                    {
                        _body.Add(_newHouseView);
                        return;
                    }

                    _body.Add(_placementView);
                    _placementView.Refresh();
                    return;

                case 1:


                    return;

                case 2:
                    _body.Add(_palettesView);
                    _palettesView.Refresh();
                    return;
            }
        }

        private void UpdateHouseVisibility()
        {
            if (!_editor.IsHouseValid) return;

            if (_showAllLevels.value)
            {
                _editor.House.ShowAllLevels();
                return;
            }
            _editor.House.HideLevelRange(_editor.Grid.CurrentLevelIndex + 1, 100);
        }

        private void Update()
        {
            if (_editor.IsHouseValid)
            {
                if (_heightTabs != null && _lastHouseMaxHeightCount != _editor.House.gridsPerLevel)
                {
                    _heightTabs.RemoveAllTabsClass("tab-level");
                    for (int i = 0; i < _heightTabs.TabsCount; i++)
                    {
                        if (i % _editor.House.gridsPerLevel == 0) _heightTabs.AddTabClass(i, "tab-level");

                    }
                    _lastHouseMaxHeightCount = _editor.House.gridsPerLevel;
                }
            }

            if (_editor.Grid.totalHeightIndex != _heightTabs.Current)
            {
                _heightTabs.ClickNoCallback(_editor.Grid.totalHeightIndex);
                UpdateHouseVisibility();
            }
        }


        private void OnBeforeSelectionChange()
        {
            if (_editor.IsHouseValid)
            {
                _editor.House.ShowAllLevels();
            }
        }


        private void OnSelectionChanged(GameObject obj)
        {
            UpdateVisualElements();

            if (_editor.IsHouseValid == false) return;
            UpdateHouseVisibility();
        }
    }
}