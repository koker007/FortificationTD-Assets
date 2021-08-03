using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TimeText
{
    const string randomText = "ЙФЯЦЫЧУВСКАМЕПИНРТГОЬШЛБЩДЮЗЖQAZWSXEDCRFVTGBYHNUJMIKOLP";
    const string randonNum = "0123456789IV-+";

    float timeStart;
    public float TimeStart { get { return timeStart; } }
    float timeZaderka;
    float timeLastShow;
    float speedSymbol;
    float timeDone;
    public float TimeDone { get { return timeDone; } }

    string text;

    bool isNumeric;
    bool refreshOlder;
    public void Set(float timestartFunc, float speed, string textFunk, bool numeric, bool refreshOlderFunk)
    {
        timeLastShow = -100;
        timeZaderka = timestartFunc;
        timeStart = timeZaderka + Time.unscaledTime;
        speedSymbol = speed;
        text = textFunk;
        isNumeric = numeric;
        refreshOlder = refreshOlderFunk;
    }

    public string Get()
    {
        string textRet = "";

        if (refreshOlder && Time.unscaledTime - timeLastShow > 5) {
            timeStart = timeZaderka + Time.unscaledTime;
        }

        if (timeStart < Time.unscaledTime && text != null)
        {
            string nameNow = "";
            foreach (char s in text)
            {
                if (nameNow.Length < (Time.unscaledTime - timeStart) / speedSymbol)
                {
                    nameNow += s;
                }
            }

            if (!isNumeric)
                nameNow += randomText[UnityEngine.Random.Range(0, randomText.Length)];
            else nameNow += randonNum[UnityEngine.Random.Range(0, randonNum.Length)];

            if (nameNow.Length > text.Length)
            {
                nameNow = text;
                timeDone += Time.unscaledDeltaTime;
            }
            else {
                timeDone = 0;
            }
            textRet = nameNow;

        }
        else
        {
            textRet = "";
            timeDone = 0;
        }

        timeLastShow = Time.unscaledTime;

        return textRet;
    }
}
