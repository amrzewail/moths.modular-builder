using Moths.ModularBuilder.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.ModularBuilder
{
    public interface IHouse
    {
        bool hasReference { get; }
        Vector3 origin { get; }

        Quaternion rotation { get; }

        Vector3 gridSize { get; }

        int gridsPerLevel { get; }

        void Add(string type, int level, GameObject module);

        void Replace(GameObject oldExistingModule, GameObject newModule);

        GameObject GetFirstByQuery(Func<GameObject, bool> query, Vector3? worldPosition = null);
        List<GameObject> GetByQuery(Func<GameObject, bool> query, Vector3? worldPosition = null);
        List<GameObject> GetAtPosition(Vector3 worldPosition);

        void HideLevelRange(int minInclusive, int maxExlusive);

        void ShowAllLevels();

        List<GameObject> GetAllAtHeight(string type, int level, int heightIndex);

        string GetModuleType(GameObject g);

        int GetModuleLevel(Vector3 position);

        MeshRenderer[] GetModuleRenderers(GameObject module);

        List<Material> GetAllModuleMaterials();

        List<Material> GetModulesMaterials(List<GameObject> modules);

        List<GameObject> GetAllModulesOfMaterial(Material material);

        List<Rule> GetRulesAtPosition(Vector3 position);
    }
}