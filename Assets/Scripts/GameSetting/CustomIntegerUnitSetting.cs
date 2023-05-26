using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace UnityGame.Data
{
    using System;
    using System.Linq;
    using UnityGame.App;

    [CreateAssetMenu(fileName = "CustomIntegerUnitSetting", menuName = "ScriptableObject/CustomIntegerUnitSetting")]
    public class CustomIntegerUnitSetting : SettingBase
    {
        public LangUnitData m_LangUnitDataDefault;
        public List<LangUnitData> m_LangUnitDatas;

        public LangUnitData GetUnitData()
        {
            var unitData = m_LangUnitDatas.Where(x => Application.systemLanguage.Equals(x.m_Language)).FirstOrDefault();
            if (unitData != null)
            {
                return unitData;
            }
            return m_LangUnitDataDefault;
        }
    }

    [Serializable]
    public class LangUnitData
    {
        public SystemLanguage m_Language;
        public int m_DigitInterval;
        public List<UnitData> m_UnitDatas;

        public UnitData GetUnitName(CustomInteger value, out bool over, out BigInteger digit, bool shortUnit = false)
        {
            int digits = value.Abs().ToString().Length;
            over = digits / Math.Max(m_DigitInterval, 1) >= m_UnitDatas.Count;
            int dataIndex = Math.Min(digits / (m_DigitInterval + 1), m_UnitDatas.Count) - 1;
            digit = CustomInteger.Pow(10, (dataIndex + 1) * m_DigitInterval).m_Value;
            if (dataIndex >= 0 && m_UnitDatas.Count > 0)
            {
                string unit = m_UnitDatas[dataIndex].Unit;
                return m_UnitDatas[dataIndex];//(shortUnit && unit.Length > 3) ? unit.Substring(0, 3) : unit;
            }
            return null;
        }
    }

    [Serializable]
    public class UnitData
    {
        public BigInteger Digits;
        public string Unit;

        //public UnitData(int digit, string unit)
        //{
        //    Digits = Pow(10, digit)._Value;
        //    Unit = unit;
        //}
    }
}