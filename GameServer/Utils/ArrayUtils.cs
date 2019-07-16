using System;

namespace GameServer.Utils
{
    public static class ArrayUtils
    {
        public static T[] MergeArrays<T>(T[] a1, T[] a2)
        {
            T[] result = new T[a1.Length + a2.Length];
            Array.Copy(a1, result, a1.Length);
            Array.Copy(a2, 0, result, a1.Length, a2.Length);
            return result;
        }

        public static T[] SubArray<T>(T[] source, int index, int length)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (index + length > source.Length)
            {
                throw new ArgumentOutOfRangeException(
                    "Source is too short to extract given array");
            }
            T[] result = new T[length];
            Array.Copy(source, index, result, 0, length);
            return result;
        }
    }
}
