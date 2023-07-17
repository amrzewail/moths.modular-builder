using HouseBuilder.Editor.Controllers;
using HouseBuilder.Editor.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ILogger = HouseBuilder.Editor.Controllers.ILogger;
using Logger = HouseBuilder.Editor.Controllers.Logger;

namespace HouseBuilder.Editor
{
    public class BuilderEditor : EditorWindow, IEditor
    {
        public Color gridColor { get; private set; } = new Color(0, 1f, 0, 0.3f);

        public Materials Materials { get; private set; }

        public ILogger Logger { get; private set; }
        public IInputController Input { get; private set; }
        public IGrid Grid { get; private set; }
        public IHouse House { get; private set; }
        public IPaletteManager Palettes { get; private set; }
        public IPrefabPreviewer Previewer { get; private set; }
        public ISceneEditor SceneEditor { get; private set; }

        public bool IsHouseValid => House != null && House.hasReference;


        public event Action OnUpdate;
        public event Action<SceneView> OnSceneGUI;
        public event Action OnDestroyed;
        public event Action OnBeforeSelectionChange;
        public event Action<GameObject> OnSelectionChanged;
        public event Action OnFocused;
        public event Action OnDisabled;
        public event Action OnEnabled;



        [MenuItem("House Builder/Editor")]
        public static void ShowWindow()
        {
            BuilderEditor editor = GetWindow<BuilderEditor>();
            editor.titleContent = new GUIContent("House Builder");
        }


        public void CreateGUI()
        {
            Materials = Materials.Default();

            Logger = new Logger();


            Input = new InputController(Logger);
            Grid = new SceneGrid();
            Palettes = new PaletteManager(Logger);
            Previewer = new PrefabPreviewer(Logger);
            SceneEditor = new SceneEditor(this);

            MainView mainView = new MainView(this);

            rootVisualElement.Add(mainView);

            OnSelectionChange();
        }


        private void UpdateGridValues()
        {
            if (IsHouseValid)
            {
                Grid.position = House.origin;
                Grid.gridSize = House.gridSize;
                Grid.oneLevelHeight = House.levelHeight;
            }
        }

        private void CleanUp()
        {
            if (IsHouseValid)
            {
                House.ShowAllLevels();
            }
            if (Previewer != null)
            {
                Previewer.Clean();
            }
        }

        #region Unity Events

        public void OnEnable()
        {
            SceneView.duringSceneGui -= SceneGUICallback;
            SceneView.duringSceneGui += SceneGUICallback;

            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;


            OnEnabled?.Invoke();
        }

        public void OnDisable()
        {
            SceneView.duringSceneGui -= SceneGUICallback;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;

            CleanUp();

            OnDisabled?.Invoke();
        }


        private void Update()
        {
            UpdateGridValues();

            OnUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            CleanUp();

            OnDestroyed?.Invoke();
        }

        private void OnFocus()
        {
            OnFocused?.Invoke();
        }

        private void OnSelectionChange()
        {
            OnBeforeSelectionChange?.Invoke();

            House = null;

            if (Selection.activeGameObject && Selection.activeGameObject.scene.IsValid())
            {
                House = Selection.activeGameObject.GetComponent<IHouse>();
            }

            UpdateGridValues();

            if (!IsHouseValid)
            {
                Previewer.Clean();
            }

            OnSelectionChanged(Selection.activeGameObject);
        }

        private void SceneGUICallback(SceneView view)
        {
            Input.Update();

            if (IsHouseValid)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                Grid.Draw(gridColor);
            }

            SceneEditor.OnSceneGUI(view);

            OnSceneGUI?.Invoke(view);

            Input.Clear();
        }

        private void OnPlayModeChanged(PlayModeStateChange obj)
        {
            CleanUp();
        }

        private void OnBeforeAssemblyReload()
        {
            CleanUp();

        }


        #endregion


    }
}