using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDAP_Waypoint_Editor
{
    internal class Helpers
    {
    }

    public static class ListExtensions
    {
        public static void Move<T>(this IList<T> list, int iIndexToMove, MoveDirection direction)
        {
            if (direction == MoveDirection.Up)
            {
                if (iIndexToMove > 0)
                {
                    var old = list[iIndexToMove - 1];
                    list[iIndexToMove - 1] = list[iIndexToMove];
                    list[iIndexToMove] = old;
                }
            }
            else
            {
                if (iIndexToMove < list.Count - 1)
                {
                    var old = list[iIndexToMove + 1];
                    list[iIndexToMove + 1] = list[iIndexToMove];
                    list[iIndexToMove] = old;
                }
            }
        }
    }

    public enum MoveDirection
    {
        Up,
        Down
    }
}