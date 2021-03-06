﻿using System.Collections.Generic;

namespace PortableCSharpLib.DataType
{
    /// <summary>
    /// bi-directional dictionary
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public class BiDictionary<TFirst, TSecond>
    {
        static BiDictionary() { General.CheckDateTime(); }

        IDictionary<TFirst, TSecond> firstToSecond = new Dictionary<TFirst, TSecond>();
        IDictionary<TSecond, TFirst> secondToFirst = new Dictionary<TSecond, TFirst>();

        private static IList<TFirst> EmptyFirstList = new TFirst[0];
        private static IList<TSecond> EmptySecondList = new TSecond[0];

        /// <summary>
        /// Add element
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void Add(TFirst first, TSecond second)
        {
            if (!firstToSecond.ContainsKey(first) && !secondToFirst.ContainsKey(second))
            {
                firstToSecond.Add(first, second);
                secondToFirst.Add(second, first);
            }
        }

        // Note potential ambiguity using indexers (e.g. mapping from int to int)
        // Hence the methods as well...
        public TSecond this[TFirst first]
        {
            get
            {
                if (!firstToSecond.ContainsKey(first))
                    return default(TSecond);
                return firstToSecond[first];
            }
        }

        public TFirst this[TSecond second]
        {
            get
            {
                if (!secondToFirst.ContainsKey(second))
                    return default(TFirst);
                return secondToFirst[second];
            }
        }

        public bool TryGetByFirst(TFirst first, out TSecond second)
        {
            return firstToSecond.TryGetValue(first, out second);
        }

        public bool TryGetBySecond(TSecond second, out TFirst first)
        {
            return secondToFirst.TryGetValue(second, out first);
        }

        public bool ContainsKey(TFirst first)
        {
            return firstToSecond.ContainsKey(first);
        }

        public bool ContainsKey(TSecond second)
        {
            return secondToFirst.ContainsKey(second);
        }

        public void Remove(TFirst first)
        {
            if (firstToSecond.ContainsKey(first))
            {
                secondToFirst.Remove(firstToSecond[first]);
                firstToSecond.Remove(first);
            }
        }

        public void Remove(TSecond second)
        {
            if (secondToFirst.ContainsKey(second))
            {
                firstToSecond.Remove(secondToFirst[second]);
                secondToFirst.Remove(second);
            }
        }
    }
}
