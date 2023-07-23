using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor
{
    public struct Materials
    {
        public Material deletingModule;
        public Material outline;

        public static Materials Default()
        {
            Materials mats;

            mats.deletingModule = Resources.Load<Material>("HouseBuilder/Material_Deleting");
            mats.outline = Resources.Load<Material>("HouseBuilder/Material_Outline");

            return mats;
        }
    }
}