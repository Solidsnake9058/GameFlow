using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class ColorFieldExtend : ColorField
{
    public ColorFieldExtend(string label, string bindingPath, SerializedObject serializedObject) : base(label)
    {
        this.bindingPath = bindingPath;
        this.Bind(serializedObject);
    }
}
