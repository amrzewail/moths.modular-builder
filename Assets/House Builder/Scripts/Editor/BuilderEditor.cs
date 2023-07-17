using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class BuilderEditor : EditorWindow
    {
        [MenuItem("House Builder/Editor")]
        public static void ShowWindow()
        {
            BuilderEditor editor = GetWindow<BuilderEditor>();
            editor.titleContent = new GUIContent("House Builder");
        }

        public Vector3 gridSize => currentHouse == null ? Vector3.one : currentHouse.gridSize;
        public Color gridColor { get; private set; } = new Color(0, 1f, 0, 0.3f);

        public GridEditor Grid { get; private set; }


        private VisualElement _header;
        private VisualElement _body;


        private FloatField _levelHeightField;
        private VisualElement _newHouseVE;
        private HouseEditingVisualElement _houseEditingVE;

        public event Action OnUpdate;
        public event Action<SceneView> OnSceneGUI;
        public event Action OnDestroyed;
        public event Action OnBeforeSelectionChange;
        public event Action<GameObject> OnSelectionChanged;
        public event Action OnFocused;
        public event Action OnDisabled;
        public event Action OnEnabled;

        public IHouse currentHouse { get; private set; }


        public void OnEnable()
        {
            SceneView.duringSceneGui += SceneGUICallback;
            OnEnabled?.Invoke();
        }

        public void OnDisable()
        {
            SceneView.duringSceneGui -= SceneGUICallback;
            OnDisabled?.Invoke();
        }

        public void CreateGUI()
        {
            Grid = new GridEditor();

            _header = new VisualElement();
            _body = new VisualElement();

            rootVisualElement.Add(_header);
            rootVisualElement.Add(_body);

            _header.Add(_levelHeightField);

            _newHouseVE = new NewHouseVisualElement();
            _houseEditingVE = new HouseEditingVisualElement(this);

            UpdateVisualElements();
        }

        private void Update()
        {
            UpdateGridValues();

            OnUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }

        private void OnFocus()
        {
            OnFocused?.Invoke();
        }

        private void OnSelectionChange()
        {
            OnBeforeSelectionChange?.Invoke();

            currentHouse = null;

            if (Selection.activeGameObject && Selection.activeGameObject.scene.IsValid())
            {
                currentHouse = Selection.activeGameObject.GetComponent<IHouse>();
            }

            UpdateGridValues();

            OnSelectionChanged?.Invoke(Selection.activeGameObject);


            UpdateVisualElements();
        }

        private void UpdateGridValues()
        {
            if (currentHouse == null || currentHouse.hasReference == false) return;
            if (currentHouse != null)
            {
                Grid.position = currentHouse.origin;
                Grid.gridSize = currentHouse.gridSize;
                Grid.oneLevelHeight = currentHouse.levelHeight;
            }
        }

        private void UpdateVisualElements()
        {
            _body.Clear();

            if (currentHouse == null || currentHouse.hasReference == false)
            {
                _body.Add(_newHouseVE);
            }
            else
            {
                _body.Add(_houseEditingVE);
            }
        }


        private void SceneGUICallback(SceneView view)
        {
            if (currentHouse != null && currentHouse.hasReference)
            {
                Grid.DrawGrid(gridColor);
            }

            OnSceneGUI?.Invoke(view);
        }

    }
}