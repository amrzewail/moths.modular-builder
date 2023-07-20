using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface ISceneEditor
    {
        void OnSceneGUI(SceneView view);

        void ExtrudeHeight();
    }
}