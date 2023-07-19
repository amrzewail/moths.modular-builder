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
        public Quaternion rotation { get; set; }
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

            Matrix4x4 defaultMatrix = Handles.matrix;

            Handles.matrix = Matrix4x4.TRS(Center, rotation, Vector3.one);
            Handles.color = color;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            float lineLength = 100 * gridSize.z;
            for (float z = 0 - lineLength; z < 0 + lineLength; z += gridSize.z)
            {
                Handles.DrawLine(new Vector3(0 - lineLength, 0, z), new Vector3(0 + lineLength, 0, z));
            }
            lineLength = 100 * gridSize.x;
            for (float x = 0 - lineLength; x < 0 + lineLength; x += gridSize.x)
            {
                Handles.DrawLine(new Vector3(x, 0, 0 - lineLength), new Vector3(x, 0, 0 + lineLength));
            }

            Handles.color = handlesColor;

            Handles.matrix = defaultMatrix;
        }

        public Vector3 Snap(Vector3 position)
        {
            position = GridToWorldSpace(position);
            position.x = BuilderEditorUtility.Snap(position.x, gridSize.x,/* _editor.GRID_SIZE.x / 2*/0);
            position.z = BuilderEditorUtility.Snap(position.z, gridSize.z, /*_editor.GRID_SIZE.z / 2*/0);
            position = WorldToGridSpace(position);
            return position;
        }

        private Vector3 GridToWorldSpace(Vector3 point)
        {
            point -= Center;
            Matrix4x4 transformationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(rotation), Vector3.one);
            Vector3 p = transformationMatrix * point;
            return p;
        }

        private Vector3 WorldToGridSpace(Vector3 point)
        {
            Matrix4x4 transformationMatrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            Vector3 p = transformationMatrix * point;
            p += Center;
            return p;
        }
    }
}