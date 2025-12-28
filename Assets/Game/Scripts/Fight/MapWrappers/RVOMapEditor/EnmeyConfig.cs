using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RVOMapEditor {
    [Serializable]
    public class EnmeyConfig
    {
        public int typeId;

        public Vector3 pos;
        public Vector3 rot;

        public FightConfig fConfig = null;
        // ...
    }
}

