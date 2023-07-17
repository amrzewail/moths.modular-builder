using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public interface IHouse
    {
        bool hasReference { get; }
        Vector3 origin { get; }

        Vector3 gridSize { get; }

        float levelHeight { get; }


        void Add(ModuleType type, int level, GameObject module);

        GameObject GetAtPosition(ModuleType type, int level, Vector3 worldPosition, float precision);

        void HideLevelRange(int minInclusive, int maxExlusive);

        void ShowAllLevels();

        List<GameObject> GetAllAtHeight(ModuleType type, int level, int heightIndex);
    }
}