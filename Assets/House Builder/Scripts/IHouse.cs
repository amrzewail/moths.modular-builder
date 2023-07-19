using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public interface IHouse
    {
        bool hasReference { get; }
        Vector3 origin { get; }

        Quaternion rotation { get; }

        Vector3 gridSize { get; }

        int levelGridHeight { get; }

        void Add(ModuleType type, int level, GameObject module);

        List<GameObject> GetAtPosition(ModuleType type, int level, Vector3 worldPosition, float precision);

        void HideLevelRange(int minInclusive, int maxExlusive);

        void ShowAllLevels();

        List<GameObject> GetAllAtHeight(ModuleType type, int level, int heightIndex);
    }
}