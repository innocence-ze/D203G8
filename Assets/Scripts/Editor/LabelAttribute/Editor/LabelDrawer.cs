using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
    LabelAttribute Attr => attribute as LabelAttribute;
    PropertyDrawer drawerOverride = null;
    bool initialized = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!initialized)
        {
            if (property.type.EndsWith("Event")) drawerOverride = new UnityEditorInternal.UnityEventDrawer();
            if (property.type.EndsWith("Map")) drawerOverride = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(property.type + "Drawer") as PropertyDrawer;
            initialized = true;
        }

        DrawLabel(position, property, Attr.Label, Attr.Const, drawerOverride);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GetHeight(property, label, drawerOverride);
    }

    // static API
    public static void DrawLabel(Rect position, SerializedProperty property, string label, bool isConst = false, PropertyDrawer drawerOverride = null)
    {
        var tc = GUI.color;

        if (isConst && EditorApplication.isPlaying)
        {
            // 常量在播放时只在Repaint绘制并上灰色
            if (Event.current.type == EventType.Repaint) GUI.color = Color.gray;
            else return;
        }

        // 开始绘制
        label = string.IsNullOrEmpty(label) ? property.displayName : (property.displayName.StartsWith("Element") ? label + property.displayName.Substring(8) : label);

        if (drawerOverride == null)
        {
            var ic = GUI.color * (1 - EditorGUI.indentLevel * 0.15f);
            ic.a = 1;
            GUI.color = ic;

            float indentWidth = EditorGUI.indentLevel * 16;

            Rect windowRect = new Rect(position.x + indentWidth, position.y + 2, position.width - indentWidth, position.height - 4);
            GUI.Box(windowRect, GUIContent.none, property.propertyType == SerializedPropertyType.Generic ? "FrameBox" : "button");

            Rect propRect = new Rect(position.x + 4, position.y + 6, position.width - 8, position.height - 12);
            EditorGUI.PropertyField(propRect, property, new GUIContent(label), true);
        }
        else
            drawerOverride.OnGUI(position, property, new GUIContent(label));

        GUI.color = tc;
    }

    public static float GetHeight(SerializedProperty property, GUIContent label, PropertyDrawer drawerOverride = null)
    {
        if (drawerOverride != null) return drawerOverride.GetPropertyHeight(property, label);
        return EditorGUI.GetPropertyHeight(property, label) + 12;
    }
}
