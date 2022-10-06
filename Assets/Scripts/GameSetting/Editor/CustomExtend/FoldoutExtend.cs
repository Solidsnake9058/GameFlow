using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class FoldoutExtend : Foldout
{
    public FoldoutExtend(string label) : base()
    {
        text = label;
    }

    public FoldoutExtend(string label, string labelClass) : base()
    {
        text = label;
        Label labelText = this.Q<Label>();
        labelText?.AddToClassList(labelClass);
    }
}