using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class FloatEvent : UnityEvent<float> { }
[System.Serializable]
public class Vec2Event : UnityEvent<Vector2> { }
[System.Serializable]
public class IntEvent : UnityEvent<int> { }
[System.Serializable]
public class SimpleEvent : UnityEvent { }
[System.Serializable]
public class ObjEvent : UnityEvent<object[]> { }
[System.Serializable]
public class StrEvent : UnityEvent<string> { }