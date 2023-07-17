using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class HouseEditingVisualElement : VisualElement
    {
        private BuilderEditor _editor;

        private ButtonTabs _tabs;


        private ButtonTabs _levelTabs;
        private ButtonTabs _heightTabs;
        private Toggle _showAllLevels;

        private VisualElement _header;
        private VisualElement _body;
        private ModulePlacementVisualElement _modulePlacementVE;

        private bool _lastShowAllLevels;
        private bool _isInitialized = false;

        public HouseEditingVisualElement(BuilderEditor editor)
        {
            _editor = editor;
            _editor.OnUpdate += Update;
            _editor.OnSceneGUI += SceneGUI;
            _editor.OnDestroyed += OnDestroy;
            _editor.OnSelectionChanged += OnSelectionChanged;

            _modulePlacementVE = new ModulePlacementVisualElement(editor);


            _levelTabs = new ButtonTabs("0", "1", "2", "3", "4", "5", "6", "7", "8", "9");
            _levelTabs.label.text = "Level";
            _levelTabs.onTabClicked += LevelTabClickCallback;

            _heightTabs = new ButtonTabs();
            _heightTabs.label.text = "Height";
            _heightTabs.onTabClicked += HeightTabClickCallback;

            _showAllLevels = new Toggle("Show all levels");

            _header = new VisualElement();
            _header.Add(_levelTabs);
            _header.Add(_heightTabs);
            _header.Add(_showAllLevels);


            _body = new VisualElement();

            _tabs = new ButtonTabs("Placement", "Editing");
            _tabs.onTabClicked += TabClickCallback;
            _tabs.Click(0);


            this.Add(_header);
            this.Add(_tabs);
            this.Add(_body);

            _isInitialized = true;
        }


        private void TabClickCallback(int index, string name)
        {
            _body.Clear();
            switch (index)
            {
                case 0:
                    _body.Add(_modulePlacementVE);
                    _modulePlacementVE.Refresh();
                    return;
            }
        }


        private void HeightTabClickCallback(int index, string arg2)
        {
            _editor.Grid.SetHeightIndex(index);
            SceneView.RepaintAll();
        }

        private void LevelTabClickCallback(int index, string arg2)
        {
            _editor.Grid.SetLevelIndex(index);
            UpdateHouseVisibility();
            SceneView.RepaintAll();
        }

        private void UpdateHouseVisibility()
        {
            if (_showAllLevels.value)
            {
                _editor.currentHouse.ShowAllLevels();
                return;
            }
            _editor.currentHouse.HideLevelRange(_editor.Grid.CurrentLevelIndex + 1, 100);
        }


        private void SceneGUIHeightControl()
        {
            if (Event.current.control)
            {
                if (Event.current.type == EventType.ScrollWheel)
                {
                    Vector2 delta = Event.current.delta;
                    int direction = -Mathf.RoundToInt(Mathf.Sign(delta.y));
                    _editor.Grid.totalHeightIndex += direction;

                    UpdateHouseVisibility();

                    Event.current.Use();
                }
            }
        }

        private void Update()
        {
            if (!_isInitialized) return;

            if (_editor.currentHouse != null)
            {
                if (_heightTabs != null)
                {
                    int maxHeightValue = _editor.Grid.LevelHeightCount;
                    if (maxHeightValue != _heightTabs.TabsCount)
                    {
                        _heightTabs.ClearTabs();
                        for (int i = 0; i < maxHeightValue; i++)
                        {
                            _heightTabs.AddTab($"{i}");
                        }
                    }
                }

                if(_lastShowAllLevels != _showAllLevels.value)
                {
                    UpdateHouseVisibility();
                    _lastShowAllLevels = _showAllLevels.value;
                }
            }

            if (_editor.Grid.CurrentHeightIndex != _heightTabs.CurrentTab)
            {
                _heightTabs.ClickNoCallback(_editor.Grid.CurrentHeightIndex);
            }
            if (_editor.Grid.CurrentLevelIndex != _levelTabs.CurrentTab)
            {
                if (_levelTabs.TabsCount > _editor.Grid.CurrentLevelIndex)
                {
                    _levelTabs.ClickNoCallback(_editor.Grid.CurrentLevelIndex);
                }
            }
        }

        private void OnSelectionChanged(GameObject obj)
        {
            if (_editor.currentHouse == null) return;
            UpdateHouseVisibility();
        }


        private void OnDestroy()
        {
        }


        private void SceneGUI(SceneView view)
        {
            SceneGUIHeightControl();
        }

    }
}