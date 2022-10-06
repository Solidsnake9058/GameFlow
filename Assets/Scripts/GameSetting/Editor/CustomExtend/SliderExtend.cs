using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class SliderExtend : Slider
{
    public SliderExtend(string label, string bindingPath, SerializedObject serializedObject, float start = 0f, float end = 10f) : base(label)
    {
        this.lowValue = start;
        this.highValue = end;
        this.bindingPath = bindingPath;
        this.Bind(serializedObject);
    }

    public SliderExtend(string label, string bindingPath, SerializedObject serializedObject, bool showInputField, float start = 0f, float end = 10f)
        : this(label, bindingPath,  serializedObject,  start = 0f,  end)
    {
        this.showInputField = showInputField;
    }
}
