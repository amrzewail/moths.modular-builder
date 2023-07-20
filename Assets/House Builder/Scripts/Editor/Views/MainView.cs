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
    public class MainView : VisualElement
    {
        private readonly IEditor _editor;

        private VisualElement _header;
        private VisualElement _body;


        private ButtonTabs _tabs;


        private ButtonTabs _heightTabs;
        private Toggle _showAllLevels;

        private VisualElement _newHouseView;
        private PlacementView _placementView;

        private int _lastHouseMaxHeightCount = 0;

        public MainView(IEditor editor)
        {
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

            _heightTabs = new ButtonTabs();
            for(int i = 0; i < 31; i++)
            {
                _heightTabs.AddTab($"{i}");
            }

            _heightTabs.label.text = "Height";
            _heightTabs.onTabClicked += HeightTabClickCallback;
            _heightTabs.tabsStyle.flexWrap = Wrap.Wrap;

            _showAllLevels = new Toggle("Show all levels");
            _showAllLevels.RegisterCallback<ChangeEvent<bool>>(ShowAllLevelsToggle);

            //_header.Add(_levelTabs);
            _header.Add(_heightTabs);
            _header.Add(_showAllLevels);


            _placementView = new PlacementView(_editor);
            _newHouseView = new VisualElement();


            _tabs = new ButtonTabs("Placement", "Editing");
            _tabs.onTabClicked += TabClickCallback;
            _tabs.Click(0);
            _header.Add(_tabs);
            
            var newHouseButton = new Button();
            newHouseButton.text = "Create New";
            newHouseButton.clicked += NewHouseClickCallback;
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

        private void HeightTabClickCallback(int index, string arg2)
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



        private void TabClickCallback(int index, string name)
        {
            if (_editor.IsHouseValid == false) return;

            _body.Clear();
            switch (index)
            {
                case 0:
                    _body.Add(_placementView);
                    _placementView.Refresh();
                    return;
            }
        }

        private void NewHouseClickCallback()
        {
            GameObject g = new GameObject("New House", typeof(House));
            Selection.activeGameObject = g;
        }

        private void UpdateVisualElements()
        {
            _body.Clear();

            if (_editor.IsHouseValid)
            {
                TabClickCallback(_tabs.CurrentTab, "");
            }
            else
            {
                _body.Add(_newHouseView);
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
                if (_heightTabs != null && _lastHouseMaxHeightCount != _editor.House.levelGridHeight)
                {
                    _heightTabs.ClearBackgroundColorOverrides();
                    for (int i = 0; i < _heightTabs.TabsCount; i++)
                    {
                        if (i % _editor.House.levelGridHeight == 0) _heightTabs.OverrideTabBackgroundColor(i, Color.red);
                    }
                    _heightTabs.UpdateTabBackgroundColors();
                    _lastHouseMaxHeightCount = _editor.House.levelGridHeight;
                }
            }

            if (_editor.Grid.CurrentHeightIndex != _heightTabs.CurrentTab)
            {
                _heightTabs.ClickNoCallback(_editor.Grid.totalHeightIndex);
                UpdateHouseVisibility();
            }

            //if (_editor.Grid.CurrentLevelIndex != _levelTabs.CurrentTab)
            //{
            //    if (_levelTabs.TabsCount > _editor.Grid.CurrentLevelIndex)
            //    {
            //        UpdateHouseVisibility();
            //        _levelTabs.ClickNoCallback(_editor.Grid.CurrentLevelIndex);
            //    }
            //}
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