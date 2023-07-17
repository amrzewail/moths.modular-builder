using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface IGrid
    {
        public Vector3 gridSize { get; set; }
        public Vector3 position { get; set; }
        public float oneLevelHeight { get; set; }

        public int totalHeightIndex { get; set; }

        public Vector3 Center { get; }

        public int LevelHeightCount => Mathf.Max((int)(oneLevelHeight / gridSize.y), 0);

        public int CurrentHeightIndex { get; set; }
        public int CurrentLevelIndex { get; set; }

        public void Draw(Color color);

        public Vector3 Snap(Vector3 position);
    }
}