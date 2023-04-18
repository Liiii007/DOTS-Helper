# DOTS-Helper
A set of helper for Unity DOTS and ECS Architecture. 
Tested on Unity 2022.2.11f1.
Any issue and pull request is welcomed!!!

## Contain
- Native1DArray(unsafe) : Use inside ComponentData struct, work with System and Burst Compiler.
- Native2DArray(unsafe) : Build with Native1DArray(like vector<vector<int>> in c++), can also work with Burst Compiler

## Warning 
- Need to Dispose manually or use Using pattern
- Not Fully Test.
- Not Thread-safe and Atomic.
- Unstable, maybe work?
