using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public abstract class HouseModule : MonoBehaviour
    {
        //[field: SerializeField] public string Type { get; private set; }

        public virtual GameObject previewGameObject => this.gameObject;

        public abstract void Refresh(IHouse house, bool refreshNeighbours);
    }
}