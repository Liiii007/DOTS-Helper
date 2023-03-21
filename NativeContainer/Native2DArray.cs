using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace DOTS
{
    namespace CustomContainer
    {
        namespace Unsafe
        {
            /// <summary>
            /// A 2D Array, can be used inside a DOTS->ComponentData or separately.
            /// </summary>
            /// <typeparam name="T">A unmanaged type.</typeparam>
            public unsafe struct Native2DArray<T> : System.IDisposable where T : unmanaged
            {
                const int defaultAlignment = 8;

                [NativeDisableUnsafePtrRestriction]
                readonly T** m_1DPtr;
                [NativeDisableUnsafePtrRestriction]
                readonly int* m_ysize;
                readonly int m_xsize;
                readonly bool m_isCreate;
                readonly Allocator m_AllocatorLabel;
                bool m_isLocked;

                //Init
                public Native2DArray(int xsize, int ysize, Allocator allocator, T data = default, int alignment = defaultAlignment)
                {
                    m_isLocked = false;
                    m_AllocatorLabel = allocator;

                    m_xsize = xsize;
                    m_ysize = (int*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<int>() * xsize, 8, allocator);

                    m_1DPtr = (T**)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * xsize, alignment, allocator);
                    for (int i = 0; i < xsize; i++)
                    {
                        m_ysize[i] = xsize;
                        m_1DPtr[i] = (T*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * ysize, alignment, allocator);

                        for (int j = 0; j < ysize; j++)
                        {
                            m_1DPtr[i][j] = data;
                        }
                    }

                    m_isCreate = true;
                }
                public Native2DArray(int xsize, Allocator allocator, T data = default, int alignment = defaultAlignment)
                {
                    this = new(xsize, xsize, allocator, data, alignment);
                }
                public Native2DArray(List<List<T>> list, Allocator label, T data = default, int alignment = defaultAlignment)
                {
                    if (list == null)
                    {
                        throw new System.ArgumentNullException("Native2DArray:List is null");
                    }

                    m_isLocked = false;
                    m_AllocatorLabel = label;

                    m_xsize = list.Count;
                    m_ysize = (int*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<int>() * list.Count, 8, label);

                    m_1DPtr = (T**)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * list.Count, alignment, label);
                    for (int i = 0; i < list.Count; i++)
                    {
                        m_ysize[i] = list[i].Count;
                        m_1DPtr[i] = (T*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * list[i].Count, alignment, label);
                        for (int j = 0; j < list[i].Count; j++)
                        {
                            m_1DPtr[i][j] = list[i][j];
                        }
                    }

                    m_isCreate = true;
                }
                public Native2DArray(T[][] array, Allocator label, int alignment = defaultAlignment)
                {
                    if (array == null)
                    {
                        throw new System.ArgumentNullException("Native2DArray:Array is null");
                    }

                    m_isLocked = false;
                    m_AllocatorLabel = label;

                    m_xsize = array.Length;
                    m_ysize = (int*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<int>() * array.Length, 8, label);

                    m_1DPtr = (T**)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * array.Length, alignment, label);
                    for (int i = 0; i < array.Length; i++)
                    {
                        m_ysize[i] = array[i].Length;
                        m_1DPtr[i] = (T*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * array[i].Length, alignment, label);
                        for (int j = 0; j < array[i].Length; j++)
                        {
                            m_1DPtr[i][j] = array[i][j];
                        }
                    }

                    m_isCreate = true;
                }
                public bool IsCreate
                {
                    get => m_isCreate;
                }

                //Data Get&Set
                public T Get(int x, int y)
                {
                    if (x >= m_xsize || y >= m_ysize[x])
                    {
                        throw new System.ArgumentNullException("Native2DArray:Index out of range");
                    }
                    else
                    {
                        return m_1DPtr[x][y];
                    }
                }
                public bool Set(int x, int y, T data)
                {
                    if (m_isLocked)
                    {
                        throw new System.AccessViolationException("Native2DArray:Had locked,please unlock first");
                    }

                    if (x >= m_xsize || y >= m_ysize[x])
                    {
                        throw new System.ArgumentNullException("Native2DArray:Index out of range");
                    }
                    else
                    {
                        m_1DPtr[x][y] = data;
                        return true;
                    }
                }

                //WriteLocker
                public void WriteLock()
                {
                    m_isLocked = true;
                }
                public void WriteUnlock()
                {
                    m_isLocked = false;
                }

                //Size Counter
                public int Count()
                {
                    return m_xsize;
                }
                public int Count(int x)
                {
                    return m_ysize[x];
                }

                public void Dispose()
                {
                    for (int i = 0; i < m_xsize; i++)
                    {
                        UnsafeUtility.Free(m_1DPtr[i], m_AllocatorLabel);
                    }
                    UnsafeUtility.Free(m_1DPtr, m_AllocatorLabel);
                    UnsafeUtility.Free(m_ysize, m_AllocatorLabel);
                }
            }
        }
    }
}