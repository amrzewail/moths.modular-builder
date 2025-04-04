using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.ModularBuilder.Editor.Controllers
{
    public interface IPrefabPreviewer
    {
        Vector3 position { get; set; }

        Vector3 eulerAngles { get; set; }

        Vector3 localScale { get; set; }

        Material material { get; set; }

        GameObject Prefab { get; }

        void SetPrefab(GameObject prefab);

        void Show();
        void Hide();

        void Clean();

        void Refresh(IHouse house);
    }
}