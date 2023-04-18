using System.Collections;
using System.Collections.Generic;
using DOTSHelper.CustomContainer.Unsafe;
using NUnit.Framework;using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.TestTools;

public unsafe class Native2DArrayTest
{
    [Test]
    public void Native2DArrayTestSimplePasses()
    {
        List<List<int>> ints= new List<List<int>>();
        ints.Add(new List<int>(new int[] { 1, 2, 3 }));
        ints.Add(new List<int>(new int[] { 4, 5 }));
        ints.Add(new List<int>(new int[] { 6, 7, 8, 9 }));

        Native2DArray<int> native2DArray = new(ints, Unity.Collections.Allocator.TempJob);
        native2DArray[0][2] = 5;
        
        Assert.AreEqual(1, native2DArray[0][0]);
        Assert.AreEqual(5, native2DArray[0][2]);
        Assert.AreEqual(5, native2DArray[1][1]);
        Assert.AreEqual(9, native2DArray[2][3]);

        Assert.Catch(() =>
        {
            native2DArray[0][3] = 3;
        });
        
        Assert.Catch(() =>
        {
            native2DArray[-1][3] = 3;
        });
        
        Assert.Catch(() =>
        {
            native2DArray[3][0] = 3;
        });
        
        Assert.Catch(() =>
        {
            native2DArray[3][4] = 3;
        });
    }
}
