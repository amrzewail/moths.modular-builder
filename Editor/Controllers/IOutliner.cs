using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Moths.ModularBuilder.Editor.Controllers
{
    public interface IOutliner
    {
        void AddGameObject(GameObject go, Color color);

        void RemoveGameObject(GameObject go);

        void RemoveAll();

        void Cleanup();
    }
}