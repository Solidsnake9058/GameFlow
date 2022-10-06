using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class SliderIntExtend : SliderInt
{
    public SliderIntExtend(string label, string bindingPath, SerializedObject serializedObject, int start = 0, int end = 10) : base(label)
    {
        this.bindingPath = bindingPath;
        this.Bind(serializedObject);
    }

    public SliderIntExtend(string label, string bindingPath, SerializedObject serializedObject, bool showInputField, int start = 0, int end = 10)
        : this(label, bindingPath, serializedObject, start, end)
    {
        this.showInputField = showInputField;
    }
}
