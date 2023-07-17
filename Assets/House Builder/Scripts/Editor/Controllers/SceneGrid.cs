using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class SceneGrid : IGrid
    {
        private int _totalHeightIndex;

        public Vector3 gridSize { get; set; }
        public Vector3 position { get; set; }
        public float oneLevelHeight { get; set; }

        public int totalHeightIndex
        {
            get => _totalHeightIndex;
            set
            {
                _totalHeightIndex = value;
                _totalHeightIndex = Mathf.Max(_totalHeightIndex, 0);
            }
        }

        public Vector3 Center
        {
            get
            {
                Vector3 origin = position;
                origin.y += totalHeightIndex * gridSize.y;
                return origin;
            }
        }

        public int LevelHeightCount => Mathf.Max((int)(oneLevelHeight / gridSize.y), 0);

        public int CurrentHeightIndex
        {
            get
            {
                return Mathf.FloorToInt(totalHeightIndex % oneLevelHeight);
            }
            set
            {
                totalHeightIndex = CurrentLevelIndex * LevelHeightCount + value;
            }
        }

        public int CurrentLevelIndex
        {
            get
            {
                if (LevelHeightCount == 0) return 0;
                return Mathf.FloorToInt(totalHeightIndex / LevelHeightCount);
            }
            set
            {
                totalHeightIndex = value * LevelHeightCount;
            }
        }

        public void Draw(Color color)
        {
            var handlesColor = Handles.color;

            Handles.color = color;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            float lineLength = 100 * gridSize.z;
            for (float z = Center.z - lineLength; z < Center.z + lineLength; z += gridSize.z)
            {
                Handles.DrawLine(new Vector3(Center.x - lineLength, Center.y, z), new Vector3(Center.x + lineLength, Center.y, z));
            }
            lineLength = 100 * gridSize.x;
            for (float x = Center.x - lineLength; x < Center.x + lineLength; x += gridSize.x)
            {
                Handles.DrawLine(new Vector3(x, Center.y, Center.z - lineLength), new Vector3(x, Center.y, Center.z + lineLength));
            }

            Handles.color = handlesColor;
        }

        public Vector3 Snap(Vector3 position)
        {
            position.x = BuilderEditorUtility.Snap(position.x, gridSize.x,/* _editor.GRID_SIZE.x / 2*/0);
            position.z = BuilderEditorUtility.Snap(position.z, gridSize.z, /*_editor.GRID_SIZE.z / 2*/0);
            position.y = Center.y;
            return position;
        }
    }
}