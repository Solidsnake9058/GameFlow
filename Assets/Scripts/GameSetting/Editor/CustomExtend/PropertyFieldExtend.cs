using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class PropertyFieldExtend : PropertyField
{
    public PropertyFieldExtend(string label, string bindingPath, SerializedObject serializedObject) : base()
    {
        this.label = label;
        this.bindingPath = bindingPath;
        this.Bind(serializedObject);
    }
}