using System;
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

        void Add(string type, int level, GameObject module);

        GameObject GetFirstByQuery(Func<GameObject, bool> query);
        List<GameObject> GetByQuery(Func<GameObject, bool> query);
        List<GameObject> GetAtPosition(string type, int level, Vector3 worldPosition, float precision);

        void HideLevelRange(int minInclusive, int maxExlusive);

        void ShowAllLevels();

        List<GameObject> GetAllAtHeight(string type, int level, int heightIndex);
    }
}