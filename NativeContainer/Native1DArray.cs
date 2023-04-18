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
            /// A native 1D array, can be used inside DOTS->ComponentData or separately.
            /// </summary>
            /// <typeparam name="T">A unmanaged type.</typeparam>
            public unsafe struct Native1DArray<T> : System.IDisposable where T : unmanaged
            {
                const int defaultAlignment = 8;

                [NativeDisableUnsafePtrRestriction]
                T*        _nativePtr;
                int       _size;
                bool      _isCreated;
                bool      _isLocked;
                Allocator _allocator;

                public Native1DArray(int size,     Allocator allocator, int alignment = defaultAlignment, bool locked = false, bool clearFlag = true)
                {
                    if (size < 0) throw new System.ArgumentOutOfRangeException("Native1DArray:Size cannot < 0");

                    _allocator = allocator;
                    _size   = size;
                    _nativePtr  = (T*)UnsafeUtility.Malloc(sizeof(T) * size, alignment, allocator);
                    if(clearFlag) UnsafeUtility.MemClear(_nativePtr, sizeof(T) * size);

                    _isLocked = locked;
                    _isCreated = true;
                }
                public Native1DArray(List<T> list, Allocator allocator, int alignment = defaultAlignment, bool locked = false, int size = 0)
                {
                    if (list == null) throw new System.ArgumentNullException("Native1DArray:List is null");

                    this = new(list.Count > size ? list.Count : size, allocator, alignment, locked, false);
                    for (int i = 0; i < list.Count; i++) this[i] = list[i];

                    _isLocked = locked;
                }
                public Native1DArray(T[] list,     Allocator allocator, int alignment = defaultAlignment, bool locked = false, int size = 0)
                {
                    if (list == null) throw new System.ArgumentNullException("Native1DArray:Array is null");

                    this = new(list.Length > size ? list.Length : size, allocator, alignment, false, false);
                    for (int i = 0; i < list.Length; i++) _nativePtr[i] = list[i];

                    _isLocked = locked;
                }

                public void Resize(int size, Allocator allocator, int alignment = defaultAlignment, bool locked = false, bool clearFlag = true)
                {
                    if (size < 0) throw new System.ArgumentOutOfRangeException("Native1DArray:Size cannot be minus than zero");
                    if (size == _size) return;
                    
                    this.Dispose();
                    this = new(size, allocator, alignment, locked, clearFlag);
                }

                public bool WriteLock
                {
                    get => _isLocked;
                    set => _isLocked = value;
                }
                public bool IsCreate => _isCreated;
                public int  Count    => _size;
                public int  Length   => _size;
                public bool InRange(int index) => 0 <= index && index < _size;
                
                public T this[int index]
                {
                    get
                    {
                        if (!_isCreated)     throw new System.AccessViolationException("Native1DArray:Not Create, Create first");
                        if (!InRange(index)) throw new System.IndexOutOfRangeException("Native1DArray:Index out of range");

                        return _nativePtr[index];
                    }
                    set
                    {
                        if (!_isCreated)     throw new System.AccessViolationException("Native1DArray:Not create");
                        if (_isLocked)       throw new System.AccessViolationException("Native1DArray:Had locked");
                        if (!InRange(index)) throw new System.IndexOutOfRangeException("Native1DArray:Index out of range");
                        
                        _nativePtr[index] = value;
                    }
                }
                
                public void Dispose()
                {
                    if (!_isCreated) 
                        return;
                    
                    UnsafeUtility.Free(_nativePtr, _allocator);
                    
                    _size = 0;
                    _isCreated = false;
                    _isLocked  = false;
                }

                public void CopyFrom(in  List<T> array, Allocator allocator)
                {
                    this = new(array, allocator);
                }
                public void CopyFrom(in  T[] array, Allocator allocator)
                {
                    this = new(array, allocator);
                }
                public void CopyFrom(in  Native1DArray<T> array, Allocator allocator)
                {
                    _isLocked = array._isLocked;
                    Resize(array.Length, allocator);
                    UnsafeUtility.MemCpy(_nativePtr, array._nativePtr, UnsafeUtility.SizeOf<T>() * _size);
                }
                public void CopyFrom(in  Native1DArray<T> array)
                {
                    CopyFrom(in array, array._allocator);
                }
                public void CopyFrom(    NativeArray<T>   array, Allocator allocator)
                {
                    Resize(array.Length, allocator);
                    UnsafeUtility.MemCpy(_nativePtr, array.GetUnsafeReadOnlyPtr(), UnsafeUtility.SizeOf<T>() * _size);
                }
                public void CopyTo  (ref Native1DArray<T> array)
                {
                    array.CopyFrom(in this);
                }
                public void CopyTo  (ref Native1DArray<T> array, Allocator allocator)
                {
                    array.CopyFrom(in this, allocator);
                }
                public void CopyTo  (ref NativeArray<T>   array, Allocator allocator)
                {
                    if (array.Length != _size)
                    {
                        array.Dispose();
                        array = new NativeArray<T>(_size, allocator);
                    }
                    UnsafeUtility.MemCpy(array.GetUnsafeReadOnlyPtr(), _nativePtr, UnsafeUtility.SizeOf<T>() * _size);
                }

                public void Slice(int start, int end)
                {
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}