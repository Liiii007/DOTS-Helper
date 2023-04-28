using System.Collections.Generic;
using DOTSHelper.CustomContainer.Unsafe;
using Unity.Collections;

public struct NativeArrayHelper
{
    public static List<int> GetRandomList(int size = 128, int min = 0, int max = 128)
    {
        System.Random random = new();
        var ints = new List<int>();

        for (int i = 0; i < size; i++)
        {
            ints.Add(random.Next(min, max));
        }

        return ints;
    }
    
    public static bool AreSame(Native1DArray<int> a, Native1DArray<int> b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }
    
    public static bool AreSame(Native1DArray<int> a, List<int> b)
    {
        return AreSame(a, new Native1DArray<int>(b, Allocator.TempJob));
    }
}
