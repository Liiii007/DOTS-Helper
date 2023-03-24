using System.Collections;
using System.Collections.Generic;
using DOTSHelper.CustomContainer.Unsafe;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Native2DArrayTest
{
    [Test]
    public void Native2DArrayTestSimplePasses()
    {
        List<List<int>> ints= new List<List<int>>();
        ints.Add(new List<int>(new int[] { 1, 2, 3 }));
        ints.Add(new List<int>(new int[] { 4, 5 }));
        ints.Add(new List<int>(new int[] { 6, 7, 8, 9 }));

        using(Native2DArray<int> native2DArray = new(ints, Unity.Collections.Allocator.TempJob))
        {
            native2DArray.Set(0, 2, 5);

            Assert.AreEqual(1, native2DArray.Get(0, 0));
            Assert.AreEqual(5, native2DArray.Get(0, 2));
            Assert.AreEqual(5, native2DArray.Get(1, 1));
            Assert.AreEqual(9, native2DArray.Get(2, 3));
            //native2DArray.Get(9, 9);
        }
    }
}
