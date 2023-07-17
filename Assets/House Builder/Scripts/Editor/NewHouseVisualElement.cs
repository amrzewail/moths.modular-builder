using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class NewHouseVisualElement : VisualElement
    {
        public NewHouseVisualElement()
        {
            Button btnCreateNew = new Button();
            btnCreateNew.text = "Create New House";
            btnCreateNew.clicked += CreateNewHouseCallback;
            this.Add(btnCreateNew);
        }

        private void CreateNewHouseCallback()
        {
            GameObject g = new GameObject("New House", typeof(House));
            Selection.activeGameObject = g;
        }

    }
}