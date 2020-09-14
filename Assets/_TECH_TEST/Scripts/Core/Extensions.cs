/// <summary>
/// Extensions for core data types
/// </summary>

using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;

public static class Extensions
{
    private static System.Random gen = new System.Random();

    #region Date/time

    public static DateTime UTCStart()
    {
        return new System.DateTime(1970, 1, 1);
    }

    public static DateTime RandomDay()
    {
        DateTime start = UTCStart();
        int range = (DateTime.UtcNow - start).Days;
        return start.AddDays(gen.Next(range));
    }

    #endregion

    #region Rect

    public static Rect GetScreenRect(this RectTransform rectTransform, Canvas canvas)
    {

        Vector3[] corners = new Vector3[4];
        Vector3[] screenCorners = new Vector3[2];

        rectTransform.GetWorldCorners(corners);

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
        }
        else
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
        }

        screenCorners[0].y = Screen.height - screenCorners[0].y;
        screenCorners[1].y = Screen.height - screenCorners[1].y;

        return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
    }

    #endregion

    #region Hash



    public static byte[] GetHash(string inputString)
    {
        using (HashAlgorithm algorithm = MD5.Create())
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }

    public static string GetHashString(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

    #endregion

    #region Float

    // Remaps a value [0 - 1] to [min - max]
    public static float Remap(this float number, float min, float max)
    {
        if (min == max)
            return min; // Return value immediately if min and max are the same

        if (min > max)
        {
            float temp = max;
            max = min;
            min = temp;
        }

        return (number * (max - min)) + min;
    }

    // Remaps a value from [min0 - max0] to [min1 - max1]
    public static float Remap(this float number, float min0, float max0, float min1, float max1)
    {
        if (min1 == max1)
            return min1; // Return value immediately if min and max are the same

        if (min0 > max0)
        {
            float temp = max0;
            max0 = min0;
            min0 = temp;
        }

        if (min1 > max1)
        {
            float temp = max1;
            max1 = min1;
            min1 = temp;
        }

        float interval = Mathf.Clamp01((number - min0) / (max0 - min0));
        return (interval * (max1 - min1)) + min1;
    }
    // A non-bullshit (non-rick) version of remap
    //(remapNoRickBullshit)
    public static float RemapNRB(this float oldValue, float oldMin, float oldMax, float newMin, float newMax, bool clamped = true)
    {
        if (clamped)
        {
            float realOldMax = Mathf.Max(oldMin, oldMax);
            float realOldMin = Mathf.Min(oldMin, oldMax);
            oldValue = Mathf.Clamp(oldValue, realOldMin, realOldMax);
        }

        float oldRange = (oldMax - oldMin);
        float newRange = (newMax - newMin);
        float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
        return newValue;
    }

    public static float Absolute(this float number)
    {
        return Mathf.Abs(number);
    }

    public static Color NextHue(this Color color, float offset)
    {
        float h, s, v;

        Color.RGBToHSV(color, out h, out s, out v);

        h += offset;
        h = Mathf.Repeat(h, 1f);

        return Color.HSVToRGB(h, s, v);
    }

    // Integrate area under AnimationCurve between start and end time
    public static float IntegrateCurve(AnimationCurve curve, float startTime, float endTime, int steps)
    {
        return Integrate(curve.Evaluate, startTime, endTime, steps);
    }

    // Integrate function f(x) using the trapezoidal rule between x=x_low..x_high
    public static float Integrate(System.Func<float, float> f, float x_low, float x_high, int N_steps)
    {
        float h = (x_high - x_low) / N_steps;
        float res = (f(x_low) + f(x_high)) / 2;
        for (int i = 1; i < N_steps; i++)
        {
            res += f(x_low + i * h);
        }
        return h * res;
    }

    #endregion

    #region Errors

    public static System.Exception PrependErrorMessage(this System.Exception err, string prepend)
    {
        if (err != null)
        {
            if (err.Message.Contains("Network error*") || err.Message.Contains("Server error*"))
                return err;

            err = new SystemException(err.Message.Insert(0, prepend));
        }

        return err;
    }

    public static bool IsNetworkErrorException(this System.Exception err)
    {
        return err.Message.Contains("Network error");
    }

    #endregion
}


