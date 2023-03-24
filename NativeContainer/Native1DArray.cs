using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace DOTSHelper
{
    namespace CustomContainer
    {
        namespace Unsafe
        {
            public unsafe struct Native1DArray<T> : System.IDisposable where T : unmanaged
            {
                const int defaultAlignment = 8;

                [NativeDisableUnsafePtrRestriction]
                T*        _1DPtr;
                int       _size;
                bool      _isCreated;
                bool      _isLocked;
                Allocator _AllocatorLabel;

                public Native1DArray(int size,     Allocator allocator, int alignment = defaultAlignment, bool locked = false)
                {
                    if (size < 0) { throw new System.ArgumentOutOfRangeException("Native1DArray:Size cannot be minus than zero"); }
                    _isLocked = locked;
                    _AllocatorLabel = allocator;
                    _size = size;
                    _1DPtr = (T*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>() * size, alignment, allocator);
                    _isCreated = true;
                }
                public Native1DArray(List<T> list, Allocator allocator, int alignment = defaultAlignment, bool locked = false)
                {
                    if (list == null) { throw new System.ArgumentNullException("Native2DArray:List is null"); }

                    this = new(list.Count, allocator, alignment, false);

                    //Set lock bit after copy data
                    for (int i = 0; i < list.Count; i++) { _1DPtr[i] = list[i]; }
                    _isLocked = locked;
                }

                public void Resize(int size, Allocator allocator, int alignment = defaultAlignment, bool locked = false)
                {
                    if (size < 0)
                    {
                        throw new System.ArgumentOutOfRangeException("Native1DArray:Size cannot be minus than zero");
                    }
                    else
                    {
                        if (size == _size) { return; }
                        this.Dispose();
                        this = new(size, allocator, alignment, locked);
                    }
                }
                public void Dispose()
                {
                    if (_isCreated)
                    {
                        UnsafeUtility.Free(_1DPtr, _AllocatorLabel);
                        _size = 0;
                        _isCreated = false;
                        _isLocked  = false;
                    }
                }

                //WriteLocker
                public void WriteLock()
                {
                    _isLocked = true;
                }
                public void WriteUnlock()
                {
                    _isLocked = false;
                }

                //Size Counter
                public int Count  => _size;
                public int Length => _size;

                //Indexer
                public T this[int index]
                {
                    get
                    {
                        if (!_isCreated) { throw new System.AccessViolationException("Native1DArray:Not Create, Create first"); }
                        if (_isLocked)   { throw new System.AccessViolationException("Native1DArray:Had locked, Unlock first"); }

                        if (index >= _size || index < 0)
                        {
                            throw new System.ArgumentNullException("Native1DArray:Index out of range");
                        }
                        else
                        {
                            return _1DPtr[index];
                        }
                    }
                    set
                    {
                        if (!_isCreated) { throw new System.AccessViolationException("Native1DArray:Not Create, Create first"); }
                        if (_isLocked)   { throw new System.AccessViolationException("Native1DArray:Had locked, Unlock first"); }

                        if (index >= _size || index < 0)
                        {
                            throw new System.ArgumentNullException("Native1DArray:Index out of range");
                        }
                        else
                        {
                            _1DPtr[index] = value;
                        }
                    }
                }

                //MemCopy
                //TODO:Check Bugs
                public void CopyFrom(ref Native1DArray<T> array)
                {
                    CopyFrom(ref array, array._AllocatorLabel);
                }
                public void CopyFrom(ref Native1DArray<T> array, Allocator allocator)
                {
                    _isLocked = array._isLocked;
                    Resize(array.Length, allocator);
                    UnsafeUtility.MemCpy(_1DPtr, array._1DPtr, UnsafeUtility.SizeOf<T>() * _size);
                }
                public void CopyFrom(    NativeArray<T>   array, Allocator allocator)
                {
                    throw new System.NotImplementedException();
                }
                public void CopyTo  (ref Native1DArray<T> array)
                {
                    throw new System.NotImplementedException();
                }
                public void CopyTo  (ref Native1DArray<T> array, Allocator allocator)
                {
                    throw new System.NotImplementedException();
                }
                public void CopyTo  (    NativeArray<T>   array, Allocator allocator)
                {
                    throw new System.NotImplementedException();
                }

                public void Slice(int start, int end)
                {
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}