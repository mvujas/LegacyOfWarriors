using System;

namespace Utils
{
    public static class ArrayUtils
    {
        private static Random random = new Random();

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

        public static void Shuffle<T>(ref T[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                int newIndex = random.Next(arr.Length);
                if(i != newIndex)
                {
                    T tmp = arr[i];
                    arr[i] = arr[newIndex];
                    arr[newIndex] = tmp;
                }
            }
        }
    }
}
