using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class ModuleObjectSelector : ISelector
    {
        public bool isEnabled { get; set; } = true;
        public Color? overrideColor { get; set; } = null;

        public Func<GameObject, bool> CanSelect { get; set; }

        public GameObject Current => CurrentMultiple.Count > 0 ? CurrentMultiple[0] : null;

        public List<GameObject> CurrentMultiple { get; private set; }

        private IEditor _editor;
        private readonly Color _selectionColor = new Color(0, 0, 0.75f, 0.75f);
        private GameObject _mouseDownObject;
        private GameObject _lastDraggedGameObject;
        private GameObject _lastSelectedGameObject;

        private Color selectionColor => overrideColor != null ? overrideColor.Value : _selectionColor;

        public ModuleObjectSelector(IEditor editor)
        {
            _editor = editor;
            _editor.OnSceneGUI += SceneGUI;

            CurrentMultiple = new List<GameObject>();
        }


        private bool CanSelectObject(GameObject g)
        {
            if (!g) return false;
            bool isInHouse = _editor.House.GetFirstByQuery(x => g == x);
            if (CanSelect != null)
            {
                return CanSelect(g) && isInHouse;
            }
            return isInHouse;
        }


        private void Select(GameObject g)
        {
            if (!isEnabled) return;
            if (!_editor.IsHouseValid) return;

            if (CurrentMultiple.Contains(g)) return;
            _editor.Outliner.AddGameObject(g, selectionColor);
            CurrentMultiple.Add(g);

            _editor.Logger.Log(nameof(ModuleObjectSelector), $"Highlight gameobject {g.name}");
        }

        private void Unselect(GameObject g)
        {
            if (!CurrentMultiple.Contains(g)) return;
            _editor.Outliner.RemoveGameObject(g);
            CurrentMultiple.Remove(g);

            _editor.Logger.Log(nameof(ModuleObjectSelector), $"Unhighlight gameobject {g.name}");
        }

        private void SceneGUI(SceneView view)
        {

            for (int i = 0; i < CurrentMultiple.Count; i++)
            {
                if (!CurrentMultiple[i])
                {
                    CurrentMultiple.RemoveAt(i);
                    i--;
                }
            }

            GameObject selectedGameObject;


            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                _mouseDownObject = HandleUtility.PickGameObject(_editor.Input.MousePosition, true);
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                _lastDraggedGameObject = null;
                _lastSelectedGameObject = null;
            }

            switch (_editor.Input.Command)
            {
                case KeyCommand.HighlightDrag:

                    selectedGameObject = HandleUtility.PickGameObject(_editor.Input.MousePosition, true);
                    if (selectedGameObject && selectedGameObject != _lastDraggedGameObject)
                    {
                        if (!CurrentMultiple.Contains(selectedGameObject))
                        {
                            if (CanSelectObject(selectedGameObject))
                            {
                                Select(selectedGameObject);
                                _lastSelectedGameObject = selectedGameObject;
                            }
                        }
                        else if (_lastSelectedGameObject != selectedGameObject)
                        {
                            Unselect(selectedGameObject);
                            _lastSelectedGameObject = null;
                        }
                        _lastDraggedGameObject = selectedGameObject;
                        _mouseDownObject = null;
                    }

                    break;

                case KeyCommand.HighlightClick:

                    selectedGameObject = HandleUtility.PickGameObject(_editor.Input.MousePosition, true);
                    bool canSelect = CanSelectObject(selectedGameObject);
                    if (!selectedGameObject || !canSelect)
                    {
                        Clear();
                        break;
                    }
                    if (selectedGameObject && _mouseDownObject == selectedGameObject)
                    {
                        if (CurrentMultiple.Contains(selectedGameObject))
                        {
                            Unselect(selectedGameObject);
                        }
                        else if (canSelect)
                        {
                            Select(selectedGameObject);
                        }
                    }

                    break;

            }
        }

        public void Clear()
        {
            if (CurrentMultiple.Count <= 1 && !Current) return;

            while(CurrentMultiple.Count > 0)
            {
                _editor.Outliner.RemoveGameObject(CurrentMultiple[0]);
                CurrentMultiple.RemoveAt(0);
            }
        }
    }
}