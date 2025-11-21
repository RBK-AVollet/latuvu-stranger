using UnityEngine;

namespace Latuvu
{
    public static class GridHelper
    {
        public static Vector3Int GetRelativePosition(Vector3Int from, Vector3Int to)
        {
            Vector3Int difference = to - from;
            return difference.sqrMagnitude != 1 ? throw new System.Exception("The provided positions are not adjacent on the grid.") : difference;
        }
    }
}
