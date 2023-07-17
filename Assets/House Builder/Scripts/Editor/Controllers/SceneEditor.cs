using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class SceneEditor : ISceneEditor
    {
        private readonly IEditor _editor;
        private readonly IPrefabPreviewer _previewer;

        public SceneEditor(IEditor editor)
        {
            _editor = editor;
            _previewer = new PrefabPreviewer(_editor.Logger);

            _previewer.material = _editor.Materials.deletingModule;
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

            Undo.DestroyObjectImmediate(g[0]);
            _editor.Logger.Log(nameof(SceneEditor), "Deleted module prefab.");
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
            if (_editor.IsHouseValid == false) return;

            Vector3 position = BuilderEditorUtility.MouseToWorldPosition(view, _editor.Grid.Center);
            position = _editor.Grid.Snap(position);

            Color handlesColor = Handles.color;

            switch (_editor.Input.Command)
            {
                case KeyCommand.None:
                    _previewer.Clean();
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
                    Handles.color = Color.green;
                    Handles.DrawWireDisc(position, Vector3.up, _editor.Grid.gridSize.magnitude / 4);
                    if (_editor.Previewer.Prefab)
                    {
                        _editor.Previewer.Show();
                        _editor.Previewer.position = position;
                    }

                    view.Repaint();

                    break;

                case KeyCommand.Instantiate:
                    if (_editor.Previewer.Prefab)
                    {
                        if (!CheckForDuplication(_editor.Previewer.position, _editor.Previewer.eulerAngles, _editor.Previewer.Prefab))
                        {
                            GameObject module = InstantiatePreviewPrefab();
                        }
                    }
                    break;

                case KeyCommand.PrepareDelete:
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
                    Handles.color = Color.red;
                    Handles.DrawWireDisc(position, Vector3.up, _editor.Grid.gridSize.magnitude / 4);
                    _editor.Previewer.Hide();


                    List<GameObject> deletingElements = _editor.House.GetAtPosition(_editor.Palettes.ModuleType, _editor.Grid.CurrentLevelIndex, position, _editor.Grid.gridSize.magnitude / 4f);
                    GameObject deletingElement = deletingElements.Count > 0 ? deletingElements[0] : null;
                    if (deletingElement)
                    {
                        _previewer.SetPrefab(deletingElement);
                        _previewer.position = deletingElement.transform.position;
                        _previewer.eulerAngles = deletingElement.transform.eulerAngles;
                        _previewer.localScale = deletingElement.transform.localScale * 1.1f;
                    }
                    else
                    {
                        _previewer.Clean();
                    }


                    view.Repaint();
                    break;

                case KeyCommand.Delete:
                    DestroyAtPosition(position);

                    _editor.Logger.Log(nameof(SceneEditor), "Try delete module prefab.");

                    break;

                case KeyCommand.Flip:
                    Vector3 scale = _editor.Previewer.localScale;
                    scale.x *= -1;
                    _editor.Previewer.localScale = scale;

                    _editor.Logger.Log(nameof(SceneEditor), $"Flip module prefab {scale}.");

                    break;

                case KeyCommand.Rotate:
                    Vector3 eulerAngles = _editor.Previewer.eulerAngles;
                    eulerAngles.y = (eulerAngles.y + 90 * _editor.Input.ScrollWheel) % 360;
                    if (eulerAngles.y < 0) eulerAngles.y += 360;
                    _editor.Previewer.eulerAngles = eulerAngles;

                    _editor.Logger.Log(nameof(SceneEditor), $"Rotate module prefab {eulerAngles}.");

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