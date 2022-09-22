using System;
using System.Security.Cryptography;

/// <summary>
/// 使用 RNGCryptoServiceProvider 產生由密碼編譯服務供應者 (CSP) 提供的亂數產生器。
/// </summary>
public static class RNGCryptoServiceProviderExtensions
{
    private static byte[] rb = new byte[4];

    private static RNGCryptoServiceProvider rngp;

    static RNGCryptoServiceProviderExtensions()
    {
        rngp = new RNGCryptoServiceProvider();
    }

    /// <summary>
    /// 產生一個非負數的亂數
    /// </summary>
    public static int Next()
    {
        rngp.GetBytes(rb);
        int value = BitConverter.ToInt32(rb, 0);
        return Math.Abs(value);
    }

    /// <summary>
    /// 產生一個非負數且小於最大值 max 的亂數
    /// </summary>
    /// <param name="max">最大值</param>
    public static int Next(int max)
    {
        if (max == 0)
        {
            return 0;
        }
        return Next() % max;
    }

    /// <summary>
    /// 產生一個非負數且最小值在 min 以上小於最大值 max 的亂數
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public static int Next(int min, int max)
    {
        return Next(max - min) + min;
    }

    /// <summary>
    /// 產生一個浮點數的亂數
    /// </summary>
    public static float NextFloat()
    {
        rngp.GetBytes(rb);
        float value = BitConverter.ToUInt32(rb, 0) / (uint.MaxValue + 1.0f);
        return value;
    }

    /// <summary>
    /// 產生一個非負數且小於最大值 max 的亂數
    /// </summary>
    /// <param name="max">最大值</param>
    public static float NextFloat(float max)
    {
        return NextFloat() * max;
    }

    /// <summary>
    /// 產生一個非負數且最小值在 min 以上小於最大值 max 的亂數
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    public static float NextFloat(float min, float max)
    {
        return NextFloat(max - min) + min;
    }

    /// <summary>
    /// Randoms the boolean.
    /// </summary>
    public static bool RandomBoolean()
    {
        return NextFloat(1f) >= 0.5f;
    }

    /// <summary>
    /// Randoms the boolean int.
    /// </summary>
    /// <returns><c>1</c>, if boolean was true, <c>-1</c>,boolean was false.</returns>
    public static int RandomBooleanInt()
    {
        return RandomBoolean() ? 1 : -1;
    }
}