using System;
using System.Reflection;
using UnityEngine;

namespace YxModDll.Patches
{
    public static class SafePatch
    {
        public static T GetField<T>(object instance, string fieldName)
        {
            if (instance == null)
            {
                Debug.LogError($"[SafePatch] 实例为 null，无法获取字段 {fieldName}");
                return default;
            }
            var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null)
            {
                Debug.LogError($"[SafePatch] 字段 {fieldName} 未找到");
                return default;
            }
            return (T)field.GetValue(instance);
        }

        public static void CallBase(object instance, string baseMethodName)
        {
            if (instance == null)
            {
                Debug.LogError("[SafePatch] 实例为 null，无法调用 base 方法");
                return;
            }

            var baseType = instance.GetType().BaseType;
            var method = baseType?.GetMethod(baseMethodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null)
            {
                Debug.LogWarning($"[SafePatch] 父类未找到方法 {baseMethodName}");
                return;
            }

            try
            {
                method.Invoke(instance, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SafePatch] base 方法调用异常: {ex}");
            }
        }
    }
}
