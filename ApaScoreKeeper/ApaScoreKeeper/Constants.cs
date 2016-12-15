using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    static public class Constants
    {
        static public Dictionary<int, int> TimeOutsForSkill = new Dictionary<int, int>()
        {
            { 1, 2 },
            { 2, 2 },
            { 3, 2 },
            { 4, 1 },
            { 5, 1 },
            { 6, 1 },
            { 7, 1 },
            { 8, 1 },
            { 9, 1 },
        };

        static public Dictionary<int, int> PointsRequiredForSkill = new Dictionary<int, int>()
        {
            { 1, 14 },
            { 2, 19 },
            { 3, 25 },
            { 4, 31 },
            { 5, 38 },
            { 6, 46 },
            { 7, 55 },
            { 8, 65 },
            { 9, 75 },
        };

        static public void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
