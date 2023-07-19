using System;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface IEditor
    {
        event Action OnUpdate;
        event Action<SceneView> OnSceneGUI;
        event Action OnDestroyed;
        event Action OnBeforeSelectionChange;
        event Action<GameObject> OnSelectionChanged;
        event Action OnFocused;
        event Action OnDisabled;
        event Action OnEnabled;

        bool IsHouseValid { get; }

        Materials Materials { get; }

        ILogger Logger { get; }

        IInputController Input { get; }
        IGrid Grid { get; }
        IHouse House { get; }
        IPaletteManager Palettes { get; }
        IPrefabPreviewer Previewer { get; }
        ISceneEditor SceneEditor { get; }

        IOutliner Outliner { get; }

        ISelector Selector { get; }

    }
}