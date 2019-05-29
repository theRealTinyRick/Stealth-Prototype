using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.System.Stats
{
    [Serializable]
    public class Modifier
    {
        /// <summary>
        /// The identity that placed this modifier
        /// </summary>
        public IdentityType modifierOwner;

        public float modificationAmount;
    }
}

