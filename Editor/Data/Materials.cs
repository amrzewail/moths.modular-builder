using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.ModularBuilder.Editor
{
    public struct Materials
    {
        public Material deletingModule;
        public Material outline;

        public static Materials Default()
        {
            Materials mats;

            mats.deletingModule = Resources.Load<Material>("ModularBuilder/Material_Deleting");
            mats.outline = Resources.Load<Material>("ModularBuilder/Material_Outline");

            return mats;
        }
    }
}