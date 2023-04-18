using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace DOTSHelper
{
    namespace CustomContainer
    {
        namespace Unsafe
        {
            /// <summary>
            /// A native 2D array, build with Native1DArray
            /// </summary>
            /// <typeparam name="T">A unmanaged type.</typeparam>
            public unsafe struct Native2DArray<T> : System.IDisposable where T : unmanaged
            {
                const int defaultAlignment = 8;

                [NativeDisableUnsafePtrRestriction]
                Native1DArray<T>* _nativePtr;
                int               _size;
                bool              _isCreated;
                bool              _isLocked;
                Allocator         _allocator;

                public Native2DArray(int size, int colSize, Allocator allocator, int alignment = defaultAlignment, bool clearFlag = true, T data = default)
                {
                    _allocator = allocator;
                    _isLocked = false;
                    
                    _size = size;

                    _nativePtr = (Native1DArray<T>*)UnsafeUtility.Malloc(sizeof(Native1DArray<T>*) * size, alignment, allocator);
                    for (int i = 0; i < size; i++)
                    {
                        _nativePtr[i]   = new Native1DArray<T>(colSize, allocator);
                        for (int j = 0; j < colSize && clearFlag; j++)
                        {
                            _nativePtr[i][j] = data;
                        }
                    }

                    _isCreated = true;
                }
                public Native2DArray(List<List<T>> list,    Allocator allocator, int alignment = defaultAlignment)
                {
                    if (list == null) throw new System.ArgumentNullException("Native2DArray:List is null");

                    _size      = list.Count;
                    _isLocked  = true;
                    _allocator = allocator;

                    _nativePtr = (Native1DArray<T>*)UnsafeUtility.Malloc(sizeof(Native1DArray<T>) * list.Count, alignment, allocator);
                    for (int i = 0; i < list.Count; i++) _nativePtr[i] = new(list[i], allocator);

                    _isCreated = true;
                }
                public Native2DArray(T[][] array,           Allocator allocator, int alignment = defaultAlignment)
                {
                    if (array == null) throw new System.ArgumentNullException("Native2DArray:Array is null");

                    _size      = array.Length;
                    _isLocked  = false;
                    _allocator = allocator;

                    _nativePtr = (Native1DArray<T>*)UnsafeUtility.Malloc(sizeof(Native1DArray<T>*) * array.Length, alignment, allocator);
                    for (int i = 0; i < array.Length; i++) _nativePtr[i] = new(array[i], allocator);

                    _isCreated = true;
                }

                public ref Native1DArray<T> this[int index]
                {
                    get
                    {
                        if (!_isCreated)     throw new System.AccessViolationException("Native2DArray:Not Create, Create first");
                        if (!InRange(index)) throw new System.IndexOutOfRangeException("Native2DArray:Index out of range");
                        
                        return ref _nativePtr[index];
                    }
                }
                
                public bool WriteLock
                {
                    get => _isLocked;
                    set => _isLocked = value;
                }
                public bool IsCreate => _isCreated;
                public int  Length   => _size;
                public int  Count    => _size;
                public bool InRange(int index) => 0 <= index && index < _size;

                public void Dispose()
                {
                    if (!_isCreated) 
                        return;
                    
                    for (int i = 0; i < _size; i++) 
                        _nativePtr[i].Dispose();
                    
                    UnsafeUtility.Free(_nativePtr, _allocator);

                    _isCreated = false;
                    _size = 0;
                    _isLocked = false;
                }
            }
        }
    }
}