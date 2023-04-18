using System.Collections.Generic;
using DOTSHelper.CustomContainer.Unsafe;
using NUnit.Framework;

public unsafe class Native1DArrayTest
{
    [Test]
    public void PointerSizeCheck()
    {
#if UNITY_64
        Assert.AreEqual(sizeof(Native1DArray<int>*), 8);
#else
        Assert.AreEqual(sizeof(Native1DArray<int>*), 4);
#endif
    }
    
    [Test]
    public void Native1DArrayTestSimplePasses()
    {
        var ints = new List<int>(new int[] { 1, 2, 3 });
     
        Native1DArray<int> native1DArray = new(ints, Unity.Collections.Allocator.TempJob);
        native1DArray[0] = 5;

        Assert.AreEqual(3, native1DArray.Length);
        Assert.AreEqual(5, native1DArray[0]);
        Assert.AreEqual(2, native1DArray[1]);
        Assert.AreEqual(3, native1DArray[2]);

        Assert.Catch(() =>
        {
            native1DArray[-1] = 3;
        });
        
        Assert.Catch(() =>
        {
            var a = native1DArray[-1];
        });
        
        Assert.Catch(() =>
        {
            native1DArray[3] = 3;
        });
        
        Assert.Catch(() =>
        {
            var a = native1DArray[3];
        });
    }
    
    [Test]
    public void Native1DArrayTestCopyPasses()
    {
        var ints = new List<int>(new int[] { 1, 2, 3 });
        Native1DArray<int> a = new(ints, Unity.Collections.Allocator.TempJob);
        Native1DArray<int> b = new();
        
        b.CopyFrom(in a);
        Assert.IsTrue(AreSame(a, b));
        b[2] = -1;
        a[1] = -1;
        Assert.IsFalse(AreSame(a, b));

        b.CopyTo(ref a);
        Assert.IsTrue(AreSame(a, b));
    }

    bool AreSame(Native1DArray<int> a, Native1DArray<int> b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
                return false;
        }

        return true;
    }
}
