using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        testText();
    }

    const string randomText = "ЙФЯЦЫЧУВСКАМЕПИНРТГОЬШЛБЩДЮЗЖQAZWSXEDCRFVTGBYHNUJMIKOLP";
    const string randonNum = "0123456789IV-+";

    [SerializeField]
    Text nameText;
    [SerializeField]
    Text valueText;

    string nameStr = "";

    float timeStart = 0;
    string valueStr = "";
    public void SetInfo(float timestartFunc, string name ,string valueFunc) {
        timeStart = timestartFunc + Time.unscaledTime;
        nameStr = name;
        valueStr = valueFunc;

    }

    float timeForOneSymbol = 0.2f;
    void testText()
    {
        if (timeStart < Time.unscaledTime)
        {
            string nameNow = "";
            foreach (char s in nameStr)
            {
                if (nameNow.Length < (Time.unscaledTime - timeStart) / timeForOneSymbol)
                {
                    nameNow += s;
                }
            }

            nameNow += randomText[UnityEngine.Random.Range(0, randomText.Length)];
            if (nameNow.Length > nameStr.Length)
            {
                nameNow = nameStr;
            }

            nameText.text = nameNow;

            string valueNow = "";
            foreach (char s in valueStr)
            {
                if (valueNow.Length < (Time.unscaledTime - timeStart) / timeForOneSymbol)
                {
                    valueNow += s;
                }
            }

            valueNow += randonNum[UnityEngine.Random.Range(0, randonNum.Length)];
            if (valueNow.Length > valueStr.Length)
            {
                valueNow = valueStr;
            }

            valueText.text = valueNow;

        }
        else
        {
            nameText.text = "";
            valueText.text = "";
        }
    }
}
