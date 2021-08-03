using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calc
{
    public const float hypotenuse1 = 1.41421356237f;


    public static int Nolmalize(float value) {
        int ret = 0;
        if (value > 0) {
            ret = 1;
        }
        else if (value < 0) {
            ret = -1;
        }
        return ret;
    }
    public static float CutFrom0To1(float value) {
        float ret = value;
        if (ret > 1) {
            ret = 1;
        }
        else if (ret < 0) {
            ret = 0;
        }

        return ret;
    }

    public static float FixRandom(float fixNum, float min, float max) {
        float scale = Mathf.Abs(max - min);
        float perlinPos = fixNum / 3;
        float ret = Mathf.PerlinNoise(perlinPos, Mathf.PI);

        ret *= 100;
        ret = ret % 1;
        ret = ret * scale;
        ret += fixNum % scale;
        if (ret > scale) {
            ret -= scale;
        }

        if (min < max)
        {
            ret = min + ret;
        }
        else {
            ret = max + ret;
        }

        return ret;
    }


    public static class Convert
    {
        public static int[] ToIntArray(string textStr)
        {
            int[] textInt = new int[textStr.Length];
            for (int num = 0; num < textStr.Length && num < textInt.Length; num++)
            {
                textInt[num] = textStr[num];
            }
            return textInt;
        }
        public static string ToString(int[] textInt)
        {
            string textStr = "";
            for (int num = 0; num < textInt.Length; num++)
            {
                textStr += (char)textInt[num];
            }
            return textStr;
        }

    }

    public static class Phyisic {
        //Получить затраченное время для достижения вертикального ускорения 0
        public static float GetTimeToVelosityY0(float gradus, float startImpuls, float gravity) {
            float coofSin = Mathf.Sin(gradus);
            return (startImpuls * coofSin) / gravity;
        }
    }

    public static class Sound {
        static public float getCoofPriory(float distForMax, float distListener)
        {
            //Приоритет для дальнего звука
            float distPrior = 0;
            if (distListener < distForMax && distListener > 0)
                distPrior = distListener / distForMax;
            else if (distListener > distForMax && distListener < distForMax * 2)
                distPrior = 1 - ((distListener - distForMax) / distForMax);

            return distPrior;
        }
    }

}
