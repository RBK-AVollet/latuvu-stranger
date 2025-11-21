using System;
using UnityEngine;

namespace Latuvu
{
    [Serializable]
    public struct EntitySpawn
    {
        public TileEntity EntityPrefab;
        public Vector3Int Position;
        public Vector3Int Direction;
    }
}