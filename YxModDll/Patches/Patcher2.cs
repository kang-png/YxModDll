using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YxModDll.Patches
{
    public class Patcher2
    {
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        public static void MethodPatch(Type oldcls, string oldMethodName, Type[] oldtypes,
                              Type newcls, string newMethodName, Type[] newtypes)
        {
            // 查找原方法（包括无参数方法）
            MethodInfo original = oldtypes == null || oldtypes.Length == 0
                ? oldcls.GetMethod(oldMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                : oldcls.GetMethod(oldMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, null, oldtypes, null);

            if (original == null)
            {
                Debug.LogError($"未找到 {oldcls.Name}.{oldMethodName} 方法！");
                return;
            }

            // 检查替换方法
            MethodInfo patch = newcls.GetMethod(newMethodName);//, newtypes
            if (patch == null)
            {
                Debug.LogError($"未找到 {newcls.Name}.{newMethodName} 方法！");
                return;
            }

            // 执行补丁
            try
            {
                Patch(original, patch);
                Debug.Log($"成功补丁 {oldcls.Name}.{oldMethodName} -> {newcls.Name}.{newMethodName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"补丁失败: {ex}");
            }
        }

        public static void Patch(MethodBase original, MethodInfo replacement)
        {
            try
            {

                IntPtr originalPtr = original.MethodHandle.GetFunctionPointer();
                IntPtr replacementPtr = replacement.MethodHandle.GetFunctionPointer();

                // 2. 修改内存保护（允许写入）
                VirtualProtect(originalPtr, 8, 0x40, out _); // PAGE_EXECUTE_READWRITE

                // 3. 写入跳转指令（x86/x64兼容）
                unsafe
                {
                    byte* ptr = (byte*)originalPtr.ToPointer();

                    if (IntPtr.Size == 8)
                    { // x64
                        *ptr = 0x48; // MOV RAX
                        *(ptr + 1) = 0xB8;
                        *(ulong*)(ptr + 2) = (ulong)replacementPtr.ToInt64();
                        *(ptr + 10) = 0xFF; // JMP RAX
                        *(ptr + 11) = 0xE0;
                    }
                    else
                    { // x86
                        *ptr = 0xE9;
                        *(int*)(ptr + 1) = (int)(replacementPtr.ToInt32() - originalPtr.ToInt32() - 5);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[失败] 补丁错误: {ex}");
            }
        }



        public static void MethodPatch(Type oldcls, string oldMethodName, Type[] oldtypes,
                              object handlerInstance, string newMethodName, Type[] newtypes)
        {
            // 查找原方法
            MethodInfo original = oldtypes == null || oldtypes.Length == 0
                ? oldcls.GetMethod(oldMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                : oldcls.GetMethod(oldMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, oldtypes, null);

            if (original == null)
            {
                Debug.LogError($"未找到 {oldcls.Name}.{oldMethodName} 方法！");
                return;
            }

            // 查找替换方法（非静态）
            MethodInfo patch = handlerInstance.GetType().GetMethod(newMethodName, newtypes);
            if (patch == null)
            {
                Debug.LogError($"未找到 {handlerInstance.GetType().Name}.{newMethodName} 方法！");
                return;
            }

            try
            {
                Patch(original, patch, handlerInstance);
                Debug.Log($"成功补丁 {oldcls.Name}.{oldMethodName} -> {handlerInstance.GetType().Name}.{newMethodName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"补丁失败: {ex}");
            }
        }
        public static void Patch(MethodBase original, MethodInfo replacement, object handlerInstance)
        {
            try
            {
                IntPtr originalPtr = original.MethodHandle.GetFunctionPointer();
                IntPtr replacementPtr = replacement.MethodHandle.GetFunctionPointer();

                // 保存原始内存保护状态
                uint oldProtect;
                VirtualProtect(originalPtr, 32, 0x40, out oldProtect); // 0x40 = PAGE_EXECUTE_READWRITE

                // 使用GCHandle固定对象
                GCHandle handle = GCHandle.Alloc(handlerInstance, GCHandleType.Pinned);
                IntPtr instancePtr = handle.AddrOfPinnedObject();

                unsafe
                {
                    byte* ptr = (byte*)originalPtr.ToPointer();

                    if (IntPtr.Size == 8) // x64
                    {
                        // 保存原始寄存器状态
                        *ptr++ = 0x55;        // PUSH RBP
                        *ptr++ = 0x48;        // MOV RBP, RSP
                        *ptr++ = 0x41;        // PUSH R12
                        *ptr++ = 0x50;        // PUSH RAX

                        // 保存原始this指针(RCX)到栈上
                        *ptr++ = 0x48;        // MOV [RBP+0x20], RCX
                        *ptr++ = 0x89;
                        *ptr++ = 0x4D;
                        *ptr++ = 0x20;

                        // 加载handlerInstance到RCX
                        *ptr++ = 0x48;        // MOV RCX, handlerInstance
                        *ptr++ = 0xB9;
                        *(ulong*)ptr = (ulong)instancePtr.ToInt64();
                        ptr += 8;

                        // 保存原始方法地址
                        *ptr++ = 0x48;        // MOV [RBP+0x18], RDX
                        *ptr++ = 0x89;
                        *ptr++ = 0x55;
                        *ptr++ = 0x18;

                        // 加载替换方法地址到RDX
                        *ptr++ = 0x48;        // MOV RDX, replacementPtr
                        *ptr++ = 0xBA;
                        *(ulong*)ptr = (ulong)replacementPtr.ToInt64();
                        ptr += 8;

                        // 恢复原始this指针到RDX
                        *ptr++ = 0x48;        // MOV RDX, [RBP+0x20]
                        *ptr++ = 0x8B;
                        *ptr++ = 0x55;
                        *ptr++ = 0x20;

                        // 调用替换方法 (CALL RDX)
                        *ptr++ = 0xFF;
                        *ptr++ = 0xD2;

                        // 恢复寄存器状态
                        *ptr++ = 0x58;        // POP RAX
                        *ptr++ = 0x41;        // POP R12
                        *ptr++ = 0x5D;        // POP RBP

                        // 返回
                        *ptr++ = 0xC3;        // RET
                    }
                    else // x86
                    {
                        // 保存EBP
                        *ptr++ = 0x55;        // PUSH EBP
                        *ptr++ = 0x89;        // MOV EBP, ESP
                        *ptr++ = 0xE5;

                        // 保存原始this指针(ECX)
                        *ptr++ = 0x89;        // MOV [EBP+0x8], ECX
                        *ptr++ = 0x4D;
                        *ptr++ = 0x08;

                        // 加载handlerInstance到ECX
                        *ptr++ = 0xB9;        // MOV ECX, handlerInstance
                        *(uint*)ptr = (uint)instancePtr.ToInt32();
                        ptr += 4;

                        // 保存原始方法地址
                        *ptr++ = 0x89;        // MOV [EBP+0xC], EDX
                        *ptr++ = 0x55;
                        *ptr++ = 0x0C;

                        // 加载替换方法地址到EDX
                        *ptr++ = 0xBA;        // MOV EDX, replacementPtr
                        *(uint*)ptr = (uint)replacementPtr.ToInt32();
                        ptr += 4;

                        // 恢复原始this指针到EDX
                        *ptr++ = 0x8B;        // MOV EDX, [EBP+0x8]
                        *ptr++ = 0x55;
                        *ptr++ = 0x08;

                        // 调用替换方法
                        *ptr++ = 0xFF;        // CALL EDX
                        *ptr++ = 0xD2;

                        // 恢复EBP
                        *ptr++ = 0x5D;        // POP EBP

                        // 返回
                        *ptr++ = 0xC3;        // RET
                    }
                }

                // 恢复原始内存保护状态
                VirtualProtect(originalPtr, 32, oldProtect, out _);

                // 释放GCHandle
                handle.Free();
            }
            catch (Exception ex)
            {
                Debug.LogError($"补丁失败: {ex}");
            }
        }
        //public static void Patch(MethodBase original, MethodInfo replacement, object handlerInstance)
        //{
        //    try
        //    {
        //        IntPtr originalPtr = original.MethodHandle.GetFunctionPointer();
        //        IntPtr replacementPtr = replacement.MethodHandle.GetFunctionPointer();

        //        // 修改内存保护
        //        VirtualProtect(originalPtr, 8, 0x40, out _);

        //        // 写入跳转指令（支持实例方法调用）
        //        unsafe
        //        {
        //            byte* ptr = (byte*)originalPtr.ToPointer();

        //            if (IntPtr.Size == 8) // x64
        //            {
        //                // 将handlerInstance存入RCX（this指针）
        //                *ptr = 0x48; // MOV RCX, handlerInstance
        //                *(ptr + 1) = 0xB9;
        //                *(ulong*)(ptr + 2) = (ulong)GCHandle.ToIntPtr(GCHandle.Alloc(handlerInstance)).ToInt64();//(ulong)GCHandle.Alloc(handlerInstance).ToInt64();

        //                // 跳转到目标方法
        //                *(ptr + 10) = 0x48; // MOV RAX, replacementPtr
        //                *(ptr + 11) = 0xB8;
        //                *(ulong*)(ptr + 12) = (ulong)replacementPtr.ToInt64();
        //                *(ptr + 20) = 0xFF; // JMP RAX
        //                *(ptr + 21) = 0xE0;
        //            }
        //            else // x86
        //            {
        //                // 将handlerInstance存入ECX（this指针）
        //                *ptr = 0xB9; // MOV ECX, handlerInstance
        //                *(int*)(ptr + 1) = GCHandle.ToIntPtr(GCHandle.Alloc(handlerInstance)).ToInt32();

        //                // 跳转到目标方法
        //                *(ptr + 5) = 0xE9;
        //                *(int*)(ptr + 6) = (int)(replacementPtr.ToInt32() - originalPtr.ToInt32() - 10);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError($"[失败] 补丁错误: {ex}");
        //    }
        //}
    }
}
