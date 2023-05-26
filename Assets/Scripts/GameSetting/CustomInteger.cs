using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityGame.App
{
    using UnityGame.App.Manager;

    [Serializable]
    public class CustomInteger
    {
        [JsonProperty]
        private BigInteger _Value;
        public BigInteger m_Value => _Value;

        public CustomInteger()
        {
            _Value = new BigInteger(0);
        }

        public CustomInteger(int value)
        {
            _Value = value;
        }

        public CustomInteger(BigInteger value)
        {
            _Value = value;
        }

        //public CustomInteger(string value)
        //{
        //    _Value = new BigInteger(value);
        //}

        public override string ToString()
        {
            return _Value.ToString();
        }

        public static int ToInt32(BigInteger value)
        {
            if (object.ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }
            return System.Int32.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
        }


        #region unit
        private class UnitData
        {
            public BigInteger Digits { get; set; }
            public string Unit { get; set; } = "";

            public UnitData(int digit, string unit)
            {
                Digits = Pow(10, digit)._Value;
                Unit = unit;
            }
        }
        // 日本語
        private static readonly UnitData[] UnitList = new UnitData[]
        {
            new UnitData( 4, "万"),
            new UnitData( 8, "億"),
            new UnitData(12, "兆"),
            new UnitData(16, "京"),
            new UnitData(20, "垓"),
            new UnitData(24, "秭"),
            new UnitData(28, "穣"),
            new UnitData(32, "溝"),
            new UnitData(36, "澗"),
            new UnitData(40, "正"),
            new UnitData(44, "載"),
            new UnitData(48, "極"),
            new UnitData(52, "恒河沙"),
            new UnitData(56, "阿僧祇"),
            new UnitData(60, "那由他"),
            new UnitData(64, "不可思議"),
            new UnitData(68, "無量大数"),
        };
        // 英語
        private static readonly UnitData[] UnitListEng = new UnitData[]
        {
            new UnitData( 3, "Thousand"),
            new UnitData( 6, "Million"),
            new UnitData( 9, "Billion"),
            new UnitData(12, "Trillion"),
            new UnitData(15, "Quadrillion"),
            new UnitData(18, "Quinyillion"),
            new UnitData(21, "Sextillion"),
            new UnitData(24, "Septillion"),
            new UnitData(27, "Octillion"),
            new UnitData(30, "Nonillion"),
            new UnitData(33, "Decillion"),
            new UnitData(36, "Undecillion"),
            new UnitData(39, "Duodecillion"),
            new UnitData(303, "Centillion"),
        };
        // 中国語繁体字
        private static readonly UnitData[] UnitListTraditional = new UnitData[]
        {
            new UnitData( 4, "萬"),
            new UnitData( 8, "億"),
            new UnitData(12, "兆"),
            new UnitData(16, "京"),
            new UnitData(20, "垓"),
            new UnitData(24, "秭"),
            new UnitData(28, "穰"),
            new UnitData(32, "溝"),
            new UnitData(36, "澗"),
            new UnitData(40, "正"),
            new UnitData(44, "載"),
            new UnitData(48, "極"),
            new UnitData(52, "恆河沙"),
            new UnitData(56, "阿僧祇"),
            new UnitData(60, "那由他"),
            new UnitData(64, "不可思議"),
            new UnitData(68, "無量大數"),
        };
        // 中国語簡体字
        private static readonly UnitData[] UnitListSimplified = new UnitData[]
        {
            new UnitData( 4, "万"),
            new UnitData( 8, "亿"),
            new UnitData(12, "兆"),
            new UnitData(16, "京"),
            new UnitData(20, "垓"),
            new UnitData(24, "秭"),
            new UnitData(28, "穰"),
            new UnitData(32, "沟"),
            new UnitData(36, "涧"),
            new UnitData(40, "正"),
            new UnitData(44, "载"),
            new UnitData(48, "极"),
            new UnitData(52, "恒河沙"),
            new UnitData(56, "阿僧祇"),
            new UnitData(60, "那由他"),
            new UnitData(64, "不可思议"),
            new UnitData(68, "无量大数"),
        };

        private static UnitData[] GetUnitList()
        {
            //switch (SystemLanguage.English)
            //{
            //    case SystemLanguage.Japanese:
            //        return UnitList;

            //    case SystemLanguage.Chinese:
            //    case SystemLanguage.ChineseSimplified:
            //        return UnitListSimplified;

            //    case SystemLanguage.ChineseTraditional:
            //        return UnitListTraditional;
            //}
            return UnitListEng;
        }

        public string ToUnit(bool shortNum = false, bool shortUnit = false)
        {
            var minus = _Value < 0 ? "-" : "";
            var abs = _Value < 0 ? -_Value : _Value;
            if (GameMediator.m_Instance == null || GameMediator.m_CustomIntegerUnitSetting == null)
            {
                return $"{_Value.ToString()}";
            }
            bool isOver;
            BigInteger digit;
            var unitName = GameMediator.m_CustomIntegerUnitSetting.GetUnitData().GetUnitName(this, out isOver, out digit, shortUnit);
            if (unitName == null)
            {
                return $"{_Value.ToString()}";
            }
            if (isOver)
            {
                return $"{minus}{unitName.Unit}";
            }
            var positiveNum = abs / digit;
            var decimalNum = (abs - positiveNum * digit) / (digit / 100);
            if (shortNum && positiveNum >= RankingUnit)
            {
                while (positiveNum >= RankingUnit) positiveNum /= 10;
                return $"{minus}{positiveNum}...{unitName.Unit}";
            }
            return $"{minus}{positiveNum}.{ToInt32(decimalNum):00}{unitName.Unit}";

            /*
            var data = GetUnitList().Where(unit => abs >= unit.Digits).LastOrDefault();
            if (data == null) return $"{_Value.ToString()}";
            var unitName = (shortUnit && data.Unit.Length > 3) ? data.Unit.Substring(0, 3) : data.Unit;
            if (data == GetUnitList().Last()) return $"{minus}{unitName}";
            var positiveNum = abs / data.Digits;
            var decimalNum = (abs - positiveNum * data.Digits) / (data.Digits / 100);
            if (shortNum && positiveNum >= RankingUnit)
            {
                while (positiveNum >= RankingUnit) positiveNum /= 10;
                return $"{minus}{positiveNum}...{unitName}";
            }
            return $"{minus}{positiveNum}.{ToInt32(decimalNum):00}{unitName}";
            */
        }

        public string ToAtk(bool shortUnit = false)
        {
            var minus = _Value < 0 ? "-" : "";
            var abs = _Value < 0 ? -_Value : _Value;
            var data = GetUnitList().Where(unit => abs >= unit.Digits).LastOrDefault();
            if (data == null) return $"{_Value.ToString()}";
            var unitName = (shortUnit && data.Unit.Length > 3) ? data.Unit.Substring(0, 3) : data.Unit;
            if (data == GetUnitList().Last()) return $"{minus}{unitName}";
            var positiveNum = abs / data.Digits;
            return $"{minus}{positiveNum}{unitName}";
        }
        #endregion

        #region ranking
        private const int RankingUnit = 1000000;
        public int ToRank()
        {
            var data = UnitList.Where(unit => _Value >= unit.Digits).LastOrDefault();
            BigInteger digits = 1;
            int index = 0;
            if (data != null)
            {
                digits = data.Digits / 100;
                index = UnitList.ToList().IndexOf(data) + 1;
            }

            var num = ToInt32(_Value / digits);
            num = Math.Max(num, 0);
            num = Math.Min(num, RankingUnit - 1);
            num += RankingUnit * index;
            return num;
        }

        public static CustomInteger FromRank(int score)
        {
            int index = score / RankingUnit - 1;
            BigInteger digits = 1;
            index = Mathf.Clamp(index, 0, UnitList.Length - 1);
            digits = UnitList[index].Digits / 100;
            var num = new BigInteger(0);
            num = score - (score / RankingUnit) * RankingUnit;
            num *= digits;
            return new CustomInteger(num);
        }
        #endregion

        #region operator
        public static implicit operator CustomInteger(int value)
        {
            return new CustomInteger(value);
        }
        public static implicit operator int(CustomInteger value)
        {
            return ToInt32(value._Value);
        }

        private const int Digit = 100;
        public static CustomInteger operator +(CustomInteger leftSide, float rightSide)
        {
            return new CustomInteger(leftSide._Value + new BigInteger((ulong)(rightSide * Digit)) / Digit);
        }
        public static CustomInteger operator +(CustomInteger leftSide, int rightSide)
        {
            return new CustomInteger(leftSide._Value + rightSide);
        }
        public static CustomInteger operator +(CustomInteger leftSide, CustomInteger rightSide)
        {
            return new CustomInteger(leftSide._Value + rightSide._Value);
        }

        public static CustomInteger operator -(CustomInteger v)
        {
            return new CustomInteger(-v._Value);
        }
        public static CustomInteger operator -(CustomInteger leftSide, float rightSide)
        {
            return new CustomInteger(leftSide._Value - new BigInteger((ulong)(rightSide * Digit)) / Digit);
        }
        public static CustomInteger operator -(CustomInteger leftSide, int rightSide)
        {
            return new CustomInteger(leftSide._Value - rightSide);
        }
        public static CustomInteger operator -(CustomInteger leftSide, CustomInteger rightSide)
        {
            return new CustomInteger(leftSide._Value - rightSide._Value);
        }

        public static CustomInteger operator *(CustomInteger leftSide, float rightSide)
        {
            return new CustomInteger(leftSide._Value * new BigInteger((ulong)(rightSide * Digit)) / Digit);
        }
        public static CustomInteger operator *(CustomInteger leftSide, CustomInteger rightSide)
        {
            return new CustomInteger(leftSide._Value * rightSide._Value);
        }

        public static CustomInteger operator /(CustomInteger leftSide, CustomInteger rightSide)
        {
            return new CustomInteger(leftSide._Value / rightSide._Value);
        }

        public static bool operator >(CustomInteger leftSide, float rightSide)
        {
            return leftSide._Value > (int)rightSide;
        }
        public static bool operator >(CustomInteger leftSide, CustomInteger rightSide)
        {
            return leftSide._Value > rightSide._Value;
        }
        public static bool operator >=(CustomInteger leftSide, float rightSide)
        {
            return leftSide._Value >= (int)rightSide;
        }
        public static bool operator >=(CustomInteger leftSide, CustomInteger rightSide)
        {
            return leftSide._Value >= rightSide._Value;
        }

        public static bool operator <(CustomInteger leftSide, float rightSide)
        {
            return leftSide._Value < (int)rightSide;
        }
        public static bool operator <(CustomInteger leftSide, CustomInteger rightSide)
        {
            return leftSide._Value < rightSide._Value;
        }
        public static bool operator <=(CustomInteger leftSide, float rightSide)
        {
            return leftSide._Value <= (int)rightSide;
        }
        public static bool operator <=(CustomInteger leftSide, CustomInteger rightSide)
        {
            return leftSide._Value <= rightSide._Value;
        }
        #endregion

        #region extension
        /// <summary>
        /// べき乗
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static CustomInteger Pow(CustomInteger a, CustomInteger b)
        {
            BigInteger v = 1;
            for (int i = 0; i < b._Value; i++)
            {
                v *= a._Value;
            }

            return new CustomInteger(v);
        }

        /// <summary>
        /// べき乗
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static CustomInteger Pow(int a, int b)
        {
            BigInteger v = 1;
            for (var i = 0; i < b; i++)
            {
                v *= a;
            }

            return new CustomInteger(v);
        }

        public static CustomInteger Pow(ulong a, ulong b)
        {
            BigInteger v = 1;
            for (ulong i = 0; i < b; i++)
            {
                v *= a;
            }

            return new CustomInteger(v);
        }

        public static CustomInteger Max(CustomInteger a, CustomInteger b)
        {
            if (a >= b) return a;
            return b;
        }

        public static CustomInteger Min(CustomInteger a, CustomInteger b)
        {
            if (a <= b) return a;
            return b;
        }

        /// <summary>
        /// 割合
        /// 0.0～1.0で返す
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Rate(CustomInteger a, CustomInteger b)
        {
            var min = Min(a, b);
            var max = Max(a, b);

            while (max > 0)
            {
                if (max < 100000)
                {
                    break;
                }
                max /= 10;
                min /= 10;
            }
            return (float)min / (float)max;
        }

        public CustomInteger Abs()
        {
            return _Value > 0 ? this : -this;
        }
        #endregion
    }
}
