using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalShowAttribute))]
public class ConditionalShowDrawer : PropertyDrawer
{
    ConditionalShowAttribute Attr => attribute as ConditionalShowAttribute;
    PropertyDrawer drawerOverride = null;
    bool initialized = false;

    bool IsConditionMet(SerializedProperty property)
    {
        if (Attr.Disabled) return false;
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(Attr.ConditionalIntField);
        if (sourcePropertyValue == null)
        {
            Debug.LogWarning("ConditionalShowAttribute 指向了一个不存在的条件字段: " + Attr.ConditionalIntField);
            return false;
        }
        int intVal = sourcePropertyValue.propertyType == SerializedPropertyType.Boolean ? (sourcePropertyValue.boolValue ? 1 : 0) : sourcePropertyValue.intValue;
        for (int i = 0; i < Attr.ExpectedValues.Length; i++)
        {
            if (Attr.ExpectedValues[i] == intVal) return true;
        }
        return false;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!initialized)
        {
            if (property.type.EndsWith("Event")) drawerOverride = new UnityEditorInternal.UnityEventDrawer();
            if (property.type.EndsWith("Map")) drawerOverride = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(property.type + "Drawer") as PropertyDrawer;
            initialized = true;
        }

        if (IsConditionMet(property))
        {
            // 条件满足，开始绘制
            LabelDrawer.DrawLabel(position, property, Attr.Label, Attr.Const, drawerOverride);
        }
        else if (Attr.AlwaysShow && Event.current.type == EventType.Repaint)
        {
            var tc = GUI.color;
            GUI.color = Color.gray;
            LabelDrawer.DrawLabel(position, property, Attr.Label, Attr.Const, drawerOverride);
            GUI.color = tc;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (IsConditionMet(property) || Attr.AlwaysShow)
        {
            return LabelDrawer.GetHeight(property, label, drawerOverride);
        }
        return -EditorGUIUtility.standardVerticalSpacing;
    }
}
