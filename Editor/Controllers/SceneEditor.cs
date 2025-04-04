using Moths.ModularBuilder.Editor.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Moths.ModularBuilder.Editor.Controllers
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

        private bool CheckForDuplication(Vector3 position, Vector3 eulerAngles, string moduleType, int levelIndex, GameObject prefab)
        {
            List<GameObject> currentPlacedElements = _editor.House.GetAtPosition(position);
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
            var g = _editor.House.GetAtPosition(position);
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
            GameObject module = InstantiatePrefab(_editor.Previewer.Prefab, _editor.Palettes.CurrentModuleType, _editor.Grid.CurrentLevelIndex);
            Undo.RegisterFullObjectHierarchyUndo(module, "New module object values");
            module.name = _editor.Previewer.Prefab.name;
            module.transform.position = _editor.Previewer.position;
            module.transform.eulerAngles = _editor.Previewer.eulerAngles;
            module.transform.localScale = _editor.Previewer.localScale;
            _editor.House.Add(_editor.Palettes.CurrentModuleType, _editor.Grid.CurrentLevelIndex, module);
            _editor.Logger.Log(nameof(SceneEditor), "Instantiated preview GameObject.");
            Undo.SetCurrentGroupName("Create new module");
            return module;
        }

        private GameObject InstantiatePrefab(GameObject prefab, string moduleType, int levelIndex)
        {
            GameObject module = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(module, "Created module object");
            module.name = prefab.name;
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
                        _editor.Previewer.Refresh(_editor.House);

                        RenderPreviewPivot();
                        HighlightAlignedModules();
                    }

                    view.Repaint();

                    break;

                case KeyCommand.UnselectPrefab:
                    _editor.Previewer.Clean();
                    _editor.Selector.Clear();
                    break;

                case KeyCommand.Place:
                    if (_editor.Previewer.Prefab)
                    {
                        if (!CheckForDuplication(_editor.Previewer.position, _editor.Previewer.eulerAngles, _editor.Palettes.CurrentModuleType, _editor.Grid.CurrentLevelIndex, _editor.Previewer.Prefab))
                        {
                            _editor.Selector.Clear();
                            GameObject module = InstantiatePreviewPrefab();
                        }
                    }
                    break;

                case KeyCommand.HidePreview:
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
                    if (_editor.Previewer.Prefab)
                    {
                        _previewExtraEulerAngles.y = (_previewExtraEulerAngles.y + 90 * _editor.Input.ScrollWheel) % 360;
                        _editor.Previewer.eulerAngles = (gridRotation * _editor.Previewer.Prefab.transform.rotation).eulerAngles + _previewExtraEulerAngles;
                        _editor.Logger.Log(nameof(SceneEditor), $"Rotate module prefab {_previewExtraEulerAngles}.");
                    }
                    break;

                case KeyCommand.AdjustHeight:
                    _editor.Grid.totalHeightIndex += _editor.Input.ScrollWheel;

                    _editor.Logger.Log(nameof(SceneEditor), $"Change grid height {_editor.Grid.totalHeightIndex}");

                    break;

                case KeyCommand.Frame:

                    if (_editor.IsHouseValid)
                    {
                        view.Frame(new Bounds(position, _editor.Grid.gridSize * _editor.Grid.heightPerLevel * 2), false);
                    }

                    break;

                case KeyCommand.Extrude:

                    if (_editor.IsHouseValid)
                    {
                        if (_editor.Selector.CurrentMultiple.Count > 0)
                        {
                            ExtrudeHeight();
                        }
                    }

                    return;
            }


            Handles.color = handlesColor;
        }



        public void ExtrudeHeight()
        {
            List<GameObject> gameObjects = null;
            if (_editor.Selector.CurrentMultiple.Count > 0)
            {
                gameObjects = _editor.Selector.CurrentMultiple.ToList();
            }

            _editor.Selector.Clear();
            foreach (GameObject g in gameObjects)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(g);
                Vector3 position = g.transform.position;
                position.y += _editor.Grid.gridSize.y;

                int levelIndex = _editor.House.GetModuleLevel(position);
                string moduleType = _editor.House.GetModuleType(g);

                if (CheckForDuplication(position, g.transform.eulerAngles, moduleType, levelIndex, prefab)) continue;

                MeshRenderer[] ogRenderers = g.GetComponentsInChildren<MeshRenderer>();

                prefab = InstantiatePrefab(prefab, moduleType, levelIndex);
                Undo.RegisterFullObjectHierarchyUndo(prefab, "New extruded module object values");

                prefab.transform.position = position;
                prefab.transform.rotation = g.transform.rotation;
                prefab.transform.localScale = g.transform.localScale;
                _editor.House.Add(moduleType, levelIndex, prefab);

                MeshRenderer[] prefabRenderers = prefab.GetComponentsInChildren<MeshRenderer>();

                for(int i = 0; i < ogRenderers.Length; i++)
                {
                    if (i >= prefabRenderers.Length) break;
                    prefabRenderers[i].sharedMaterials = ogRenderers[i].sharedMaterials;
                }

                _editor.Selector.Select(prefab);

            }

            Undo.SetCurrentGroupName("Extrude prefabs");

            _editor.Logger.Log(nameof(SceneEditor), "Attempted extrude height.");
        }

        public void ReplaceSelectionWith(GameObject prefab)
        {
            if (!prefab) return;

            var selections = _editor.Selector.CurrentMultiple;
            if (selections.Count == 0) return;

            var newModules = new List<GameObject>();
            foreach (var g in selections)
            {
                GameObject module = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                module.name = prefab.name;
                _editor.House.Replace(g, module);
                Undo.RegisterCreatedObjectUndo(module, "Replaced module object");
                newModules.Add(module);
                DestroyGameObject(g);
            }
            _editor.Selector.Clear();
            foreach (var g in newModules)
            {
                _editor.Selector.Select(g);
            }
            _editor.Logger.Log(nameof(SceneEditor), $"Attempted replace with {prefab.name}");
        }

        public void AddPrefabToSelection(GameObject prefab)
        {
            if (!prefab) return;

            var selections = _editor.Selector.CurrentMultiple;
            if (selections.Count == 0) return;

            var newModules = new List<GameObject>();
            foreach (var g in selections)
            {
                GameObject module = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                module.name = prefab.name;
                module.transform.position = g.transform.position;
                module.transform.rotation = g.transform.rotation;
                module.transform.localScale = g.transform.localScale;
                _editor.House.Add(_editor.House.GetModuleType(g), _editor.House.GetModuleLevel(g.transform.position), module);
                Undo.RegisterCreatedObjectUndo(module, "Replaced module object");
                newModules.Add(module);
            }
            _editor.Selector.Clear();
            foreach (var g in newModules)
            {
                _editor.Selector.Select(g);
            }
            _editor.Logger.Log(nameof(SceneEditor), $"Attempted add with {prefab.name}");
        }

        public void ReplaceSelectionWithMaterial(Material oldMaterial, Material material)
        {
            if (!_editor.IsHouseValid) return;
            if (_editor.Selector.CurrentMultiple.Count == 0) return;
            if (!oldMaterial) return;

            List<Material> rendererMaterials = new List<Material>();
            foreach(var g in _editor.Selector.CurrentMultiple)
            {
                MeshRenderer[] renderers = _editor.House.GetModuleRenderers(g);
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].GetSharedMaterials(rendererMaterials);
                    for(int k = 0; k < rendererMaterials.Count; k++)
                    {
                        if (rendererMaterials[k] != oldMaterial) continue;
                        rendererMaterials[k] = material;

                    }
                    Undo.RecordObject(renderers[i], "Material change");
                    renderers[i].sharedMaterials = rendererMaterials.ToArray();
                }
                _editor.Logger.Log(nameof(SceneEditor), $"Changed material for module {g.name}");
            }
        }

        public void SelectAllOfMaterial(Material material)
        {
            var modules = _editor.House.GetAllModulesOfMaterial(material);
            foreach(var m in modules)
            {
                _editor.Selector.Select(m);
            }
        }

    }
}