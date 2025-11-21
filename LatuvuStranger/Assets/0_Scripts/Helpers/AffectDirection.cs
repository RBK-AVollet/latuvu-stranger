using System;
using UnityEngine;

namespace Latuvu
{
    [Serializable]
    public struct AffectDirection
    {
        public bool FromRight;
        public bool FromLeft;
        public bool FromUp;
        public bool FromDown;

        public bool AllowsMovement(Vector3Int relativeMovement)
        {
            if (relativeMovement == Vector3Int.right) return FromLeft;
            if (relativeMovement == Vector3Int.left) return FromRight;
            if (relativeMovement == Vector3Int.up) return FromDown;
            if (relativeMovement == Vector3Int.down) return FromUp;
            return false;
        }
    }
}
