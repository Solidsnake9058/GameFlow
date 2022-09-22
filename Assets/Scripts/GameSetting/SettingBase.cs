using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGame.Data
{
    public abstract class SettingBase : ScriptableObject
    {
        public string SettingName;
        public int ShowOrder;

#if UNITY_EDITOR
        public abstract string GetEditorName();
#endif
    }
}