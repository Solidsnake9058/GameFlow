using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class CommonRandomResult
{
    /// <summary>
    /// Gets the random result.
    /// </summary>
    /// <returns>The random result.</returns>
    /// <param name="itemRates">Item rates, total of rate must equal or less then 1.</param>
    /// <param name="resultCount">Result count.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T[] GetRandomResult<T>(Dictionary<T, float> itemRates, int resultCount = 1)
    {
        Dictionary<T, float> itemRateTotal = new Dictionary<T, float>();
        List<T> result = new List<T>();
        float sumRate = 0;
        if (itemRates != null && itemRates.Count != 0)
        {
            foreach (var item in itemRates)
            {
                if (item.Value > 0)
                {
                    sumRate += item.Value;
                    itemRateTotal.Add(item.Key, sumRate);
                }
            }

            for (int i = 0; i < resultCount; i++)
            {
                float value = RNGCryptoServiceProviderExtensions.NextFloat();
                //UnityEngine.Debug.Log("Rate:" + value);
                foreach (var item in itemRateTotal)
                {
                    if (item.Value > value)
                    {
                        result.Add(item.Key);
                        break;
                    }
                }
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Gets the random result. Items are possible be duplicate
    /// </summary>
    /// <returns>The random result.</returns>
    /// <param name="itemWeightys">Item weighty.</param>
    /// <param name="resultCount">Result count.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T[] GetRandomResult<T>(Dictionary<T, int> itemWeightys, out Dictionary<T, int> itemSumWeightys, out int sumWeighty, int resultCount = 1)
    {
        itemSumWeightys = new Dictionary<T, int>();
        List<T> result = new List<T>();
        sumWeighty = 0;
        if (itemWeightys != null && itemWeightys.Count != 0)
        {
            foreach (var item in itemWeightys)
            {
                if (item.Value > 0)
                {
                    sumWeighty += item.Value;
                    itemSumWeightys.Add(item.Key, sumWeighty);
                }
            }
            return GetRandomResult(itemSumWeightys, sumWeighty, resultCount);
        }

        return result.ToArray();
    }

    public static T[] GetRandomResult<T>(Dictionary<T, int> itemWeightys, int resultCount = 1)
    {
        Dictionary<T, int> itemSumWeightys = new Dictionary<T, int>();
        itemSumWeightys = new Dictionary<T, int>();
        List<T> result = new List<T>();
        int sumWeighty = 0;
        if (itemWeightys != null && itemWeightys.Count != 0)
        {
            foreach (var item in itemWeightys)
            {
                if (item.Value > 0)
                {
                    sumWeighty += item.Value;
                    itemSumWeightys.Add(item.Key, sumWeighty);
                }
            }

            return GetRandomResult(itemSumWeightys, sumWeighty, resultCount);
        }

        return result.ToArray();
    }

    public static T[] GetRandomResult<T>(Dictionary<T, int> itemSumWeightys, int sumWeighty, int resultCount = 1)
    {
        List<T> result = new List<T>();
        if (itemSumWeightys != null && itemSumWeightys.Count != 0)
        {
            for (int i = 0; i < resultCount; i++)
            {
                int value = RNGCryptoServiceProviderExtensions.Next(sumWeighty);
                //UnityEngine.Debug.Log("Rate:" + value);
                foreach (var item in itemSumWeightys)
                {
                    if (item.Value > value)
                    {
                        result.Add(item.Key);
                        break;
                    }
                }
            }
        }
        return result.ToArray();
    }

    public static T[] GetRandomResultRawItem<T>(Dictionary<T, int> itemSumWeightys, int sumWeighty, int resultCount = 1)
    {
        List<T> result = new List<T>();
        if (itemSumWeightys != null && itemSumWeightys.Count != 0)
        {
            for (int i = 0; i < resultCount; i++)
            {
                int value = RNGCryptoServiceProviderExtensions.Next(sumWeighty);
                //UnityEngine.Debug.Log("Rate:" + value);
                foreach (var item in itemSumWeightys)
                {
                    if (item.Value > 0 && item.Value > value)
                    {
                        result.Add(item.Key);
                        break;
                    }
                    value -= item.Value;
                }
            }
        }
        return result.ToArray();
    }

    public static T[] GetRandomResultSimple<T>(T[] items, int resultCount = 1)
    {
        List<T> temp = new List<T>();
        for (int i = 0; i < items.Length; i++)
        {
            temp.Add(items[i]);
        }

        return GetRandomResultSimple(temp, resultCount, true);
    }

    public static T[] GetRandomResultSimple<T>(List<T> items, int resultCount = 1, bool isTemp = false)
    {
        List<T> temp = items;
        if (!isTemp)
        {
            temp = new List<T>();
            for (int i = 0; i < items.Count; i++)
            {
                temp.Add(items[i]);
            }
        }

        temp = Shuffle(temp, 5);
        resultCount = Math.Min(resultCount, items.Count);

        while (temp.Count > 0 && temp.Count != resultCount)
        {
            temp.RemoveAt(temp.Count - 1);
        }

        return temp.ToArray();
    }

    public static List<T> GetRandomResultSimpleList<T>(List<T> items, int resultCount = 1)
    {
        List<T> temp = new List<T>();
        for (int i = 0; i < items.Count; i++)
        {
            temp.Add(items[i]);
        }

        temp = Shuffle(temp, 5);
        resultCount = Math.Min(resultCount, items.Count);

        while (temp.Count > 0 && temp.Count != resultCount)
        {
            temp.RemoveAt(temp.Count - 1);
        }

        return temp;
    }

    public static List<T> Shuffle<T>(List<T> values, int runTimes = 1)
    {
        List<T> list = new List<T>(values);
        T tmp;
        int iS;
        for (int i = 0; i < runTimes; i++)
        {
            for (int j = 0; j < list.Count; j++)
            {
                iS = RNGCryptoServiceProviderExtensions.Next(j, list.Count);

                if (j == iS)
                {
                    continue;
                }
                tmp = list[j];
                list[j] = list[iS];
                list[iS] = tmp;
            }
        }
        return list;
    }

    public static Vector2 GetRandomPosition(Vector2 range)
    {
        return new Vector2(
            RNGCryptoServiceProviderExtensions.NextFloat(range.x, range.y),
            RNGCryptoServiceProviderExtensions.NextFloat(range.x, range.y)
            );
    }

    public static Vector2 GetRandomPosition(Vector2 min, Vector2 max)
    {
        return new Vector2(
            RNGCryptoServiceProviderExtensions.NextFloat(min.x, max.x),
            RNGCryptoServiceProviderExtensions.NextFloat(min.y, max.y)
            );
    }

    public static Vector3 GetRandomPosition(float min, float max)
    {
        return new Vector3(
            RNGCryptoServiceProviderExtensions.NextFloat(min, max),
            RNGCryptoServiceProviderExtensions.NextFloat(min, max),
            RNGCryptoServiceProviderExtensions.NextFloat(min, max)
            );
    }

    public static Vector3 GetRandomPosition(Vector3 min, Vector3 max)
    {
        return new Vector3(
            RNGCryptoServiceProviderExtensions.NextFloat(min.x, max.x),
            RNGCryptoServiceProviderExtensions.NextFloat(min.y, max.y),
            RNGCryptoServiceProviderExtensions.NextFloat(min.z, max.z)
            );
    }

    public static Quaternion GetRanAngle(float min, float max)
    {
        return Quaternion.Euler(GetRandomPosition(min, max));
    }

    public static Quaternion GetRanAngle(Vector3 min, Vector3 max)
    {
        return Quaternion.Euler(GetRandomPosition(min, max));
    }

    public static float GetRandomValue(float max)
    {
        return RNGCryptoServiceProviderExtensions.NextFloat(max);
    }

    public static float GetRandomValue(Vector2 range)
    {
        return GetRandomValue(range.x, range.y);
    }

    public static float GetRandomValue(float min, float max)
    {
        return RNGCryptoServiceProviderExtensions.NextFloat(min, max);
    }
}
