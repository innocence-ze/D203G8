using System;
using UnityEngine;

/// <summary>
/// 满足特定条件时才显示，不可以和Label特性连用（自带Label效果）
/// Make the field show on the inspector only in the certain condition. Don't use with attribute "Label" (already has the "Label" effect).
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ConditionalShowAttribute : PropertyAttribute
{
    /// <summary>
    /// 条件字段，必须是可转化为整型的类型，如int, enum, bool. 
    /// The target conditional field to determine whether the condition is met. Must be convertible to integer.(E.G. int,enum,bool)
    /// </summary>
    public string ConditionalIntField = "";
    /// <summary>
    /// 预期值数组，匹配数组中任意值时满足显示条件
    /// Array of expected values. Condition is met if any of the values matches the target field.
    /// </summary>
    public int[] ExpectedValues;
    /// <summary>
    /// 标签，用于实现在Inspector上，留空显示默认名称
    /// The label to show on the inspector. Leave empty to remain default label.
    /// </summary>
    public string Label = "";
    /// <summary>
    /// 选中此项后，条件不满足时该项会以灰色不可交互状态显示
    /// Check this to disable the field instead of hiding it when condition is not met
    /// </summary>
    public bool AlwaysShow = false;
    /// <summary>
    /// 常量在游戏中不允许改变
    /// if Const, disallow changes during play
    /// </summary>
    public bool Const = false;

    public ConditionalShowAttribute(string conditionalIntField, bool expectedValue)
    {
        this.ConditionalIntField = conditionalIntField;
        this.ExpectedValues = new int[] { expectedValue ? 1 : 0 };
    }
    public ConditionalShowAttribute(string conditionalIntField, object expectedValue)
    {
        this.ConditionalIntField = conditionalIntField;
        this.ExpectedValues = new int[] { (int)expectedValue };
    }
    public ConditionalShowAttribute(string conditionalIntField, params object[] expectedValues)
    {
        this.ConditionalIntField = conditionalIntField;
        if (expectedValues.Length == 0)
        {
            this.ExpectedValues = new int[] { 1 };
        }
        else
        {
            this.ExpectedValues = new int[expectedValues.Length];
            for (int i = 0; i < expectedValues.Length; i++) this.ExpectedValues[i] = (int)expectedValues[i];
        }
    }

    public bool Disabled = false;
    /// <summary>
    /// 不传参使用特性则隐藏此域。和HideInInspector特性不同，此时debug模式可以看到此变量。
    /// Use the attribute without passing any parameter to hide the field.
    /// Unlike the attribute "HideInInspector", the variable is available in Debug mode.
    /// </summary>
    public ConditionalShowAttribute()
    {
        Disabled = true;
    }
}