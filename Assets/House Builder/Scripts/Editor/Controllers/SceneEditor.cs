using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class SceneEditor : ISceneEditor
    {
        private readonly IEditor _editor;
        private readonly IOutliner _alignedOutliner;

        private Vector3 _previewExtraEulerAngles;

        private readonly Color _yAxisAlignColor = new Color(0, 0.75f, 0, 0.8f);

        private List<GameObject> _highlightedObjectsAligned = new List<GameObject>();

        public SceneEditor(IEditor editor)
        {
            _editor = editor;
            _alignedOutliner = new Outliner(editor);
        }

        private bool CheckForDuplication(Vector3 position, Vector3 eulerAngles, GameObject prefab)
        {
            List<GameObject> currentPlacedElements = _editor.House.GetAtPosition(_editor.Palettes.ModuleType, _editor.Grid.CurrentLevelIndex, position, _editor.Grid.gridSize.magnitude / 4f);
            bool isDuplicated;
            foreach(var currentPlacedElement in currentPlacedElements)
            {
                GameObject placedPrefab = PrefabUtility.GetCorrespondingObjectFromSource(currentPlacedElement);
                if (placedPrefab == prefab)
                {
                    isDuplicated = Vector3.Dot(currentPlacedElement.transform.eulerAngles.normalized, eulerAngles.normalized) > 0.99f;
                    if (isDuplicated) return true;
                }
            }

            return false;
        }

        private void DestroyAtPosition(Vector3 position)
        {
            var g = _editor.House.GetAtPosition(_editor.Palettes.ModuleType, _editor.Grid.CurrentLevelIndex, position, _editor.Grid.gridSize.magnitude / 4f);
            if (g.Count == 0) return;
            DestroyGameObject(g[0]);
        }

        private void DestroyGameObject(GameObject g)
        {
            if (!g) return;

            _editor.Logger.Log(nameof(SceneEditor), $"Deleted module {g.name}.");
            Undo.DestroyObjectImmediate(g);
        }


        private GameObject InstantiatePreviewPrefab()
        {
            if (_editor.Previewer.Prefab == null) return null;
            GameObject module = InstantiatePrefab(_editor.Previewer.Prefab);
            module.name = _editor.Previewer.Prefab.name;
            module.transform.position = _editor.Previewer.position;
            module.transform.eulerAngles = _editor.Previewer.eulerAngles;
            module.transform.localScale = _editor.Previewer.localScale;
            _editor.Logger.Log(nameof(SceneEditor), "Instantiated preview GameObject.");
            return module;
        }

        private GameObject InstantiatePrefab(GameObject prefab)
        {
            GameObject module = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            module.name = prefab.name;
            _editor.House.Add(_editor.Palettes.ModuleType, _editor.Grid.CurrentLevelIndex, module);
            Undo.RegisterCreatedObjectUndo(module, "Created module object");
            return module;
        }

        private void RenderPreviewPivot()
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            Matrix4x4 TRS = Matrix4x4.TRS(_editor.Previewer.position, _editor.Grid.rotation * Quaternion.Euler(_previewExtraEulerAngles), Vector3.one);
            Handles.matrix = TRS;

            Handles.color = Color.blue;
            Handles.DrawLine(Vector3.zero, Vector3.forward);

            Handles.color = Color.red;
            Handles.DrawLine(Vector3.zero, Vector3.right);

            Handles.color = Color.green;
            Handles.DrawLine(Vector3.zero, Vector3.down * _editor.Grid.totalHeightIndex * _editor.Grid.gridSize.y);

            Handles.matrix = Matrix4x4.identity;
        }

        private void HighlightAlignedModules()
        {
            Vector2 previewPosition = new Vector2(_editor.Previewer.position.x, _editor.Previewer.position.z);
            float precision = _editor.Grid.gridSize.magnitude / 4;
            List<GameObject> alignedYAxis = _editor.House.GetByQuery(g =>
            {
                return g.transform.position.y < _editor.Previewer.position.y - _editor.Grid.gridSize.y - precision && Vector2.Distance(new Vector2(g.transform.position.x, g.transform.position.z), previewPosition) < precision;
            });

            for(int i = 0; i < alignedYAxis.Count; i++)
            {
                _alignedOutliner.AddGameObject(alignedYAxis[i], _yAxisAlignColor);
            }
        }

        public void RaiseHeight()
        {
            var gameObjects = _editor.House.GetAllAtHeight(_editor.Palettes.ModuleType, _editor.Grid.CurrentLevelIndex, _editor.Grid.CurrentHeightIndex);

            _editor.Grid.totalHeightIndex++;

            foreach (GameObject g in gameObjects)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(g);
                Vector3 position = g.transform.position;
                position.y = _editor.Grid.Center.y;

                if (CheckForDuplication(position, g.transform.eulerAngles, prefab)) continue;

                prefab = InstantiatePrefab(prefab);
                prefab.transform.position = position;
                prefab.transform.rotation = g.transform.rotation;
                prefab.transform.localScale = g.transform.localScale;

            }

            _editor.Logger.Log(nameof(SceneEditor), "Attempted raise height.");
        }

        public void OnSceneGUI(SceneView view)
        {
            _alignedOutliner.Cleanup();

            if (_editor.IsHouseValid == false) return;

            Vector3 position = BuilderEditorUtility.MouseToWorldPosition(view, _editor.Grid.Center);
            position = _editor.Grid.Snap(position);

            Quaternion gridRotation = _editor.Grid.rotation;

            Color handlesColor = Handles.color;

            switch (_editor.Input.Command)
            {
                case KeyCommand.None:
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
                    Handles.color = Color.green;
                    Handles.DrawWireDisc(position, Vector3.up, _editor.Grid.gridSize.magnitude / 4);

                    if (_editor.Previewer.Prefab)
                    {
                        _editor.Previewer.Show();
                        _editor.Previewer.position = position;
                        _editor.Previewer.eulerAngles = (gridRotation * _editor.Previewer.Prefab.transform.rotation).eulerAngles + _previewExtraEulerAngles;

                        RenderPreviewPivot();
                        HighlightAlignedModules();
                    }

                    view.Repaint();

                    break;

                case KeyCommand.UnselectedPrefab:
                    _editor.Previewer.Clean();

                    break;

                case KeyCommand.LeftMouseButtonUp:
                    if (_editor.Previewer.Prefab)
                    {
                        if (!CheckForDuplication(_editor.Previewer.position, _editor.Previewer.eulerAngles, _editor.Previewer.Prefab))
                        {
                            _editor.Selector.Clear();
                            GameObject module = InstantiatePreviewPrefab();
                        }
                    }
                    break;

                case KeyCommand.PrepareHighlight:
                    _editor.Previewer.Hide();
                    break;

                case KeyCommand.Delete:

                    if (_editor.Selector.Current)
                    {
                        foreach(var g in _editor.Selector.CurrentMultiple)
                        {
                            DestroyGameObject(g);
                        }
                    }
                    else
                    {

                        DestroyAtPosition(position);
                        _editor.Logger.Log(nameof(SceneEditor), "Try delete module prefab.");
                    }
                    break;

                case KeyCommand.Flip:
                    Vector3 scale = _editor.Previewer.localScale;
                    scale.x *= -1;
                    _editor.Previewer.localScale = scale;

                    _editor.Logger.Log(nameof(SceneEditor), $"Flip module prefab {scale}.");

                    break;

                case KeyCommand.Rotate:
                    _previewExtraEulerAngles.y = (_previewExtraEulerAngles.y + 90 * _editor.Input.ScrollWheel) % 360;
                    _editor.Previewer.eulerAngles = (gridRotation * _editor.Previewer.Prefab.transform.rotation).eulerAngles + _previewExtraEulerAngles;

                    _editor.Logger.Log(nameof(SceneEditor), $"Rotate module prefab {_previewExtraEulerAngles}.");

                    break;

                case KeyCommand.ChangeHeight:
                    _editor.Grid.totalHeightIndex += _editor.Input.ScrollWheel;

                    _editor.Logger.Log(nameof(SceneEditor), $"Change grid height {_editor.Grid.totalHeightIndex}");

                    break;

                case KeyCommand.Frame:

                    if (_editor.IsHouseValid)
                    {
                        view.Frame(new Bounds(position, _editor.Grid.gridSize * _editor.Grid.LevelHeightCount * 2), false);
                    }

                    break;
            }


            Handles.color = handlesColor;
        }
    }
}