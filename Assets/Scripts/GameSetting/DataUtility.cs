using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.Data
{
    /// <summary>
    /// For ReorderableListEx
    /// </summary>
    [Serializable]
    public abstract class SortingObject
    {
        public bool IsSelectedForMove { get; set; }
    }

    /// <summary>
    /// Id Management
    /// </summary>
    [Serializable]
    public class Identifier
    {
        public int Id;
        public string Name;

        public override bool Equals(object obj)
        {
            return (obj as Identifier).Id == Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    // for unity serializable
    [Serializable]
    public class MinMaxDataInt : MinMaxData<int>
    {
        public int Random() { return UnityEngine.Random.Range(Min, Max); }
    }
    [Serializable]
    public class MinMaxDataFloat : MinMaxData<float>
    {
        public MinMaxDataFloat(float min, float max) { Min = min; Max = max; }
        public float Random() { return UnityEngine.Random.Range(Min, Max); }
    }
    [Serializable]
    public class MinMaxDataVector3 : MinMaxData<Vector3>
    {
        public MinMaxDataVector3(Vector3 min, Vector3 max) { Min = min; Max = max; }
        public Vector3 Random()
        {
            var retval = Vector3.zero;
            retval.x = UnityEngine.Random.Range(Min.x, Max.x);
            retval.y = UnityEngine.Random.Range(Min.y, Max.y);
            retval.z = UnityEngine.Random.Range(Min.z, Max.z);
            return retval;
        }
    }

    [Serializable]
    public class MinMaxData<T>
    {
        public T Min;
        public T Max;
    }

    [Serializable]
    public class ListWrapper<T>
    {
        public List<T> List;
        public int Count { get { return List.Count; } }
    }

    [Serializable]
    public class ArrayWrapper<T>
    {
        public T[] Array;
        public int Count { get { return Array.Length; } }
    }
}
