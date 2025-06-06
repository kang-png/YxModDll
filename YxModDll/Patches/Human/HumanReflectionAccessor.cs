using System;
using System.Reflection;
using UnityEngine;

public class HumanReflectionAccessor
{
    private readonly Human human;

    private static readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

    private static readonly FieldInfo thisFrameHitField = typeof(Human).GetField("thisFrameHit", flags);
    private static readonly FieldInfo lastFrameHitField = typeof(Human).GetField("lastFrameHit", flags);
    private static readonly FieldInfo jumpDelayField = typeof(Human).GetField("jumpDelay", flags);
    private static readonly FieldInfo groundDelayField = typeof(Human).GetField("groundDelay", flags);
    private static readonly FieldInfo disableInputField = typeof(Human).GetField("disableInput", flags);
    private static readonly FieldInfo grabManagerField = typeof(Human).GetField("grabManager", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo grabStartPositionField = typeof(Human).GetField("grabStartPosition", flags);
    private static readonly FieldInfo groundManagerField = typeof(Human).GetField("groundManager", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo velocitiesField = typeof(Human).GetField("velocities", flags);

    private static readonly MethodInfo processInputMethod = typeof(Human).GetMethod("ProcessInput", flags);
    private static readonly MethodInfo limitFallSpeedMethod = typeof(Human).GetMethod("LimitFallSpeed", flags);
    private static readonly MethodInfo processUnconsciousMethod = typeof(Human).GetMethod("ProcessUnconscious", flags);
    private static readonly MethodInfo processFallMethod = typeof(Human).GetMethod("ProcessFall", flags);
    private static readonly FieldInfo overridenDragField = typeof(Human).GetField("overridenDrag", BindingFlags.Instance | BindingFlags.NonPublic);

    public bool overridenDrag
    {
        get => (bool)overridenDragField.GetValue(human);
        set => overridenDragField.SetValue(human, value);
    }


    public HumanReflectionAccessor(Human human)
    {
        this.human = human ?? throw new ArgumentNullException(nameof(human));
    }

    public float thisFrameHit
    {
        get => (float)thisFrameHitField.GetValue(human);
        set => thisFrameHitField.SetValue(human, value);
    }

    public float lastFrameHit
    {
        get => (float)lastFrameHitField.GetValue(human);
        set => lastFrameHitField.SetValue(human, value);
    }

    public float jumpDelay
    {
        get => (float)jumpDelayField.GetValue(human);
        set => jumpDelayField.SetValue(human, value);
    }

    public float groundDelay
    {
        get => (float)groundDelayField.GetValue(human);
        set => groundDelayField.SetValue(human, value);
    }

    public bool disableInput
    {
        get => (bool)disableInputField.GetValue(human);
        set => disableInputField.SetValue(human, value);
    }
    public GrabManager grabManager
    {
        get => (GrabManager)grabManagerField.GetValue(human);
        set => grabManagerField.SetValue(human, value);
    }

    public Vector3 grabStartPosition
    {
        get => (Vector3)grabStartPositionField.GetValue(human);
        set => grabStartPositionField.SetValue(human, value);
    }

    public GroundManager groundManager
    {
        get => (GroundManager)groundManagerField.GetValue(human);
        set => groundManagerField.SetValue(human, value);
    }

    public Vector3[] velocities
    {
        get => (Vector3[])velocitiesField.GetValue(human);
        set => velocitiesField.SetValue(human, value);
    }

    // 实例方法调用，反射调用Human内部私有方法
    public void ProcessInput()
    {
        processInputMethod.Invoke(human, null);
    }

    public void LimitFallSpeed()
    {
        limitFallSpeedMethod.Invoke(human, null);
    }

    public void ProcessUnconscious()
    {
        processUnconsciousMethod.Invoke(human, null);
    }

    public void ProcessFall()
    {
        processFallMethod.Invoke(human, null);
    }
}
