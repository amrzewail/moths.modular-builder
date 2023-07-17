using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class ModulePlacementVisualElement : VisualElement
    {

        private ModuleType SelectedModuleType => _enumModuleTypes == null ? ModuleType.None : (ModuleType)_enumModuleTypes.value;
        private GameObject SelectedPrefab
        {
            get
            {
                if (_currentPalette == null) return null;
                if (_currentSelectedPrefabIndex >= _currentPalette.Prefabs.Length) _currentSelectedPrefabIndex = 0;
                return _currentPalette.Prefabs[_currentSelectedPrefabIndex];
            }
        }

        private BuilderEditor _editor;

        private PaletteSet[] _paletteSets;
        private PaletteSet _currentPaletteSet;
        private ModulePalette[] _palettes;



        private PaletteSetList _paletteList;
        private EnumField _enumModuleTypes;
        private Label _transformLabel;

        private GridVisualElement _grid;

        private Material _materialDeleting;


        private ModuleType _lastSelectedModuleType = ModuleType.None;

        private ModulePalette _currentPalette;
        private ModulePointer _previewModule;

        private Vector3 _currentEulerAngles = new Vector3();
        private Vector3 _currentLocalScale = Vector3.one;
        private int _currentSelectedPrefabIndex;

        public ModulePlacementVisualElement(BuilderEditor editor)
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;

            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            _previewModule = new ModulePointer();

            _editor = editor;
            _editor.OnSceneGUI += SceneGUI;
            _editor.OnDestroyed += OnDestroy;
            _editor.OnBeforeSelectionChange += OnBeforeSelectionChange;
            _editor.OnSelectionChanged += OnSelectionChanged;
            _editor.OnUpdate += Update;
            _editor.OnFocused += OnFocus;
            _editor.OnDisabled += OnDisable;

            _materialDeleting = Resources.Load<Material>("HouseBuilder/Material_Deleting");



            _paletteList = new PaletteSetList();
            _paletteList.onSelected += PaletteListSelectCallback;

            _enumModuleTypes = new EnumField("Module Type", ModuleType.Wall);

            _transformLabel = new Label();

            var raiseButton = new Button();
            raiseButton.text = "Raise";
            raiseButton.clicked += RaiseCallback;

            _grid = new GridVisualElement();


            ScrollView _scrollView = new ScrollView();
            _scrollView.Add(_grid);

            this.Add(_paletteList);
            this.Add(_enumModuleTypes);
            this.Add(_transformLabel);
            this.Add(raiseButton);
            this.Add(_scrollView);


            LoadPaletteSets();
            LoadPalettes();
            UpdateCurrentPalette();
            UpdatePrefabPreview();
        }

        private void RaiseCallback()
        {
            var gameObjects = _editor.currentHouse.GetAllAtHeight(SelectedModuleType, _editor.Grid.CurrentLevelIndex, _editor.Grid.CurrentHeightIndex);

            _editor.Grid.totalHeightIndex++;

            foreach(GameObject g in gameObjects)
            {
                GameObject prefab = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(g);
                Vector3 position = g.transform.position;
                position.y = _editor.Grid.Center.y;

                if (CheckForDuplication(position, g.transform.eulerAngles, prefab)) continue;

                prefab = InstantiatePrefab(prefab);
                prefab.transform.position = position;
                prefab.transform.rotation = g.transform.rotation;
                prefab.transform.localScale = g.transform.localScale;

            }
        }

        private void CleanUp()
        {
            if (_editor.currentHouse != null) _editor.currentHouse.ShowAllLevels();

            _previewModule.Release();

        }


        private bool PaletteListSelectCallback(PaletteSet set)
        {
            _currentPaletteSet = set;

            LoadPalettes();

            return true;
        }

        private void LoadPaletteSets()
        {
            _paletteSets = Resources.LoadAll<PaletteSet>("");
            _paletteList.Refresh(_paletteSets);

            if (_paletteSets == null || _paletteSets.Length == 0) return;
            if (!_currentPaletteSet) _currentPaletteSet = _paletteSets[0];
        }

        private void LoadPalettes()
        {
            if (!_currentPaletteSet) return;

            _palettes = _currentPaletteSet.Palettes;
        }


        private void UpdateCurrentPalette()
        {
            _currentPalette = null;

            if (_palettes == null) return;

            for (int i = 0; i < _palettes.Length; i++)
            {
                if (_palettes[i].Type == SelectedModuleType)
                {
                    _currentPalette = _palettes[i];
                    break;
                }
            }

            if (_currentPalette == null) return;

            _grid.Clear();

            for (int i = 0; i < _currentPalette.Prefabs.Length; i++)
            {
                var prefabVE = new PrefabButtonVisualElement(_currentPalette.Prefabs[i]);
                int index = i;
                prefabVE.clicked += g => PrefabButtonCallback(index);
                _grid.Add(prefabVE);
            }

        }

        private void PrefabButtonCallback(int index)
        {
            _currentSelectedPrefabIndex = index;
        }

        private void UpdatePrefabPreview()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                _previewModule.Release();
                return;
            }


            if (!_currentPalette) return;

            _previewModule.SetSafeCopy(SelectedPrefab);

        }

        private void InstantiatePreviewPrefab()
        {
            GameObject module = InstantiatePrefab(_previewModule.Prefab);
            module.transform.position = _previewModule.Target.transform.position;
            module.transform.rotation = _previewModule.Target.transform.rotation;
            module.transform.localScale = _previewModule.Target.transform.localScale;
        }

        private GameObject InstantiatePrefab(GameObject prefab)
        {
            GameObject module = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            module.name = prefab.name;
            _editor.currentHouse.Add(SelectedModuleType, _editor.Grid.CurrentLevelIndex, module);
            Undo.RegisterCreatedObjectUndo(module, "Created module object");
            return module;
        }

        private void DestroyAtPosition(Vector3 position)
        {
            var g = _editor.currentHouse.GetAtPosition(SelectedModuleType, _editor.Grid.CurrentLevelIndex, position, _editor.gridSize.magnitude / 4f);
            if (!g) return;

            Undo.DestroyObjectImmediate(g);
        }

        private bool CheckForDuplication(Vector3 position, Vector3 eulerAngles, GameObject prefab)
        {
            GameObject currentPlacedElement = _editor.currentHouse.GetAtPosition(SelectedModuleType, _editor.Grid.CurrentLevelIndex, position, _editor.gridSize.magnitude / 4f);
            bool isDuplicated = false;

            if (currentPlacedElement)
            {
                GameObject placedPrefab = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(currentPlacedElement);
                if (placedPrefab == prefab)
                {
                    isDuplicated = Vector3.Dot(currentPlacedElement.transform.eulerAngles.normalized, eulerAngles.normalized) > 0.95f;
                }
            }

            return isDuplicated;
        }


        private void OnModuleTypeChanged()
        {
            UpdateCurrentPalette();
        }


        private void SceneGUIDeletion(Vector3 position)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            Color handlesColor = Handles.color;
            Handles.color = Color.red;
            Handles.DrawWireDisc(position, Vector3.up, _editor.gridSize.magnitude / 4);
            Handles.color = handlesColor;


            GameObject deletingElement = _editor.currentHouse.GetAtPosition(SelectedModuleType, _editor.Grid.CurrentLevelIndex, position, _editor.gridSize.magnitude / 4f);
            if (deletingElement)
            {
                _previewModule.SetSafeCopy(deletingElement, _materialDeleting);
            }
            else
            {
                _previewModule.Release();
            }
            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
            {
                if (Event.current.button == 0 && !Event.current.alt)
                {
                    _previewModule.Release();
                    DestroyAtPosition(position);
                }
            }

            if (_previewModule.Target && _previewModule.Prefab)
            {
                _previewModule.Target.transform.position = deletingElement.transform.position;
                _previewModule.Target.transform.rotation = deletingElement.transform.rotation;
                _previewModule.Target.transform.localScale = deletingElement.transform.localScale * 1.05f;
            }

            SceneView.RepaintAll();
        }

        private void SceneGUICreation(Vector3 position)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            Color handlesColor = Handles.color;
            Handles.color = Color.green;
            Handles.DrawWireDisc(position, Vector3.up, _editor.gridSize.magnitude / 4);
            Handles.color = handlesColor;

            UpdatePrefabPreview();


            if (Event.current.alt == true)
            {
                if (Event.current.type == EventType.ScrollWheel)
                {
                    Vector2 delta = Event.current.delta;
                    _currentEulerAngles.y = (_currentEulerAngles.y + 90 * Mathf.Sign(delta.y)) % 360;
                    if (_currentEulerAngles.y < 0) _currentEulerAngles.y += 360;
                    Event.current.Use();
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    _currentLocalScale.x *= -1;
                }
            }

            if (_previewModule.Target && _previewModule.Prefab)
            {
                _previewModule.Target.transform.position = position;

                Vector3 eulerAngles = _previewModule.Target.transform.eulerAngles;
                eulerAngles.y = _currentEulerAngles.y;
                _previewModule.Target.transform.eulerAngles = eulerAngles;

                Vector3 localScale = _previewModule.Target.transform.localScale;
                localScale.x = _currentLocalScale.x * _previewModule.Prefab.transform.localScale.x;
                _previewModule.Target.transform.localScale = localScale;

                if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                {
                    if (Event.current.button == 0 && !Event.current.alt)
                    {
                        if (!CheckForDuplication(_previewModule.Target.transform.position, _previewModule.Target.transform.eulerAngles, _previewModule.Prefab))
                        {
                            InstantiatePreviewPrefab();
                        }
                    }
                }

            }
        }


        #region Editor Event Listeners


        private void SceneGUI(SceneView view)
        {
            if (_editor.currentHouse == null || !_editor.currentHouse.hasReference || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                _previewModule.Release();
                return;
            }

            UpdateCurrentPalette();

            if (!_currentPalette) return;


            Vector3 position = BuilderEditorUtility.MouseToWorldPosition(view, _editor.Grid.Center);
            position = _editor.Grid.Snap(position);

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.control)
            {
                SceneGUIDeletion(position);
            }
            else
            {
                SceneGUICreation(position);
            }
        }


        private void Update()
        {
            if (_lastSelectedModuleType != SelectedModuleType)
            {
                _lastSelectedModuleType = SelectedModuleType;
                OnModuleTypeChanged();
            }

            _transformLabel.text = $"Rotation: {Mathf.RoundToInt(_currentEulerAngles.y)} {(_currentLocalScale.x < 0 ? "Flipped" : "")}";

        }


        private void OnPlayModeChanged(PlayModeStateChange obj)
        {
            CleanUp();
        }

        private void OnBeforeAssemblyReload()
        {
            CleanUp();

        }

        private void OnDisable()
        {
            CleanUp();
        }


        private void OnDestroy()
        {
            CleanUp();
        }

        private void OnBeforeSelectionChange()
        {
            if (_editor.currentHouse == null) return;
            if (!_editor.currentHouse.hasReference) return;
            _editor.currentHouse.ShowAllLevels();
        }

        private void OnSelectionChanged(GameObject obj)
        {
        }


        public void Refresh()
        {

        }

        private void OnFocus()
        {
            LoadPaletteSets();
            LoadPalettes();
        }


        #endregion

    }
}