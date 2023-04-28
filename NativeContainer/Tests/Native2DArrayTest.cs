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
        List<List<int>> ints = new List<List<int>>();

        for (int i = 0; i < 16; i++)
        {
            ints.Add(NativeArrayHelper.GetRandomList());
        }

        Native2DArray<int> native2DArray = new(ints, Unity.Collections.Allocator.TempJob);

        for (int i = 0; i < 16; i++)
        {
            Assert.IsTrue(NativeArrayHelper.AreSame(native2DArray[i], ints[i]));
        }

        //Out of range tests
        Assert.Catch(() =>
        {
            native2DArray[0][ints[0].Count] = 3;
        });
        
        Assert.Catch(() =>
        {
            native2DArray[-1][0] = 3;
        });
        
        Assert.Catch(() =>
        {
            native2DArray[ints.Count][0] = 3;
        });
    }
}
