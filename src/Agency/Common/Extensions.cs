using System;
using System.Collections.Generic;

namespace Agency.Common
{
    public static class Extensions
    {

        public static T PickRandom<T>(this IList<T> list, Random random = null)
        {
            if (random == null)
            {
                random = new Random();
            }

            if (list.Count == 0)
            {
                throw new InvalidOperationException("Can't pick item from an empty list");
            }

            return list[random.Next(list.Count)];
        }
        
    }
}