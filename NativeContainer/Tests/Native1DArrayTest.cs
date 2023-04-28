using System.Collections.Generic;
using DOTSHelper.CustomContainer.Unsafe;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

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
        var ints = NativeArrayHelper.GetRandomList();
     
        Native1DArray<int> native1DArray = new(ints, Allocator.TempJob);

        //Size tests
        Assert.AreEqual(ints.Count, native1DArray.Length);
        Assert.AreEqual(ints.Count, native1DArray.Count);
        
        //Modify test
        native1DArray[0] = 5;
        Assert.AreEqual(5, native1DArray[0]);
                    
        for (int i = 1; i < ints.Count; i++)
        {
            Assert.AreEqual(ints[i], native1DArray[i]);
        }

        //Out of range tests
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
            native1DArray[ints.Count] = 3;
        });
        
        Assert.Catch(() =>
        {
            var a = native1DArray[ints.Count];
        });
    }
    
    [Test]
    public void Native1DArrayTestCopyPasses()
    {
        var ints = NativeArrayHelper.GetRandomList();
        
        Native1DArray<int> a = new(ints, Unity.Collections.Allocator.TempJob);
        Native1DArray<int> b = new();
        
        Assert.IsFalse(NativeArrayHelper.AreSame(a, b));
        Assert.AreEqual(0, b.Length);
        
        b.CopyFrom(in a);
        Assert.IsTrue(NativeArrayHelper.AreSame(a, b));
        Assert.AreEqual(a.Length, b.Length);
        a[0] = -a[0];
        Assert.IsFalse(NativeArrayHelper.AreSame(a, b));

        b.CopyTo(ref a);
        Assert.IsTrue(NativeArrayHelper.AreSame(a, b));
    }
    
}