using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System;

public class ResearchMenuCTRL : MonoBehaviour
{
    void iniSetings()
    {
        if (setings == null)
        {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj)
            {
                setings = setingsObj.GetComponent<Setings>();
            }

            if (!setings)
                Invoke("iniSetings", 1f);
        }
    }
    Setings setings;

    void iniResearh()
    {
        if (research == null)
        {
            GameObject researchObj = GameObject.FindGameObjectWithTag("Steam");
            if (researchObj)
            {
                research = researchObj.GetComponent<GPResearch>();
            }

            if (!research)
                Invoke("iniResearh", 1f);
        }
    }
    GPResearch research;

    void TestTextNeirodata() {
        if (neirodataText && research) {
            if (neirodataStr == "") {
                if (setings)
                {
                    foreach (StringKeyFileLoader.KeyAndText KaT in setings.LangugeText.KaT)
                    {
                        if (KaT.Key == KeyNeirodata)
                        {
                            neirodataStr = KaT.Text;
                            break;
                        }
                    }
                }

                if (neirodataStr == "")
                    neirodataStr = "Neirodata";
            }

            neirodataText.text = neirodataStr + ": " + research.stats.neirodata;  
        }
    }
    [SerializeField]
    Text neirodataText;
    [SerializeField]
    string KeyNeirodata = "asd";

    string neirodataStr = "";

    void TestTextTech() {
        if (TechText && research)
        {
            if (TechStr == "")
            {
                if (setings)
                {
                    foreach (StringKeyFileLoader.KeyAndText KaT in setings.LangugeText.KaT)
                    {
                        if (KaT.Key == KeyTech)
                        {
                            TechStr = KaT.Text;
                            break;
                        }
                    }
                }

                if (TechStr == "")
                    TechStr = "Tech";
            }

            TechText.text = countTechNow + " " + TechStr;
        }
    }
    [SerializeField]
    Text TechText;
    [SerializeField]
    string KeyTech = "er";
    string TechStr = "";

    void TestCountTech() {
        if (research) {
            countTechNow = research.GetCountTech();
        }
    }
    int countTechNow = 0;

    void testImageFon() {
        if (titleResearhFon && research) {
            countTechImageNow += (countTechNow - countTechImageNow) * Time.unscaledDeltaTime;
            countTechImageMax = research.GetMaxTech();

            titleResearhFon.fillAmount = countTechImageNow/countTechImageMax;
        }
    }
    [SerializeField]
    Image titleResearhFon;
    float countTechImageNow = 0;
    float countTechImageMax = 0;

    void testNowReseach() {
        //Если есть время текущее, время окончания иследования не равно нулю и время начала не равно нулю
        if (TimeFromNET.GetFastestNISTDate() != DateTime.MinValue && GPResearch.main != null &&
            GPResearch.main.stats.researchTimeStart != 0 &&
            GPResearch.main.stats.researchTimeEnd != 0 &&
            researchProgressImage && researchProgressText) {

            //Считаем время окончания
            DateTime timeEnd = new DateTime(2020, 1, 1).AddSeconds(GPResearch.main.stats.researchTimeEnd);
            DateTime timeNow = TimeFromNET.GetFastestNISTDate();

            //Если иследование не завершено
            if (timeNow.Ticks < timeEnd.Ticks)
            {
                DateTime timeStart = new DateTime(2020, 1, 1).AddSeconds(GPResearch.main.stats.researchTimeStart);

                //обшая Продолжительность иследования
                double reserchSecMax = timeEnd.Subtract(timeStart).TotalSeconds;
                double reserchSecNow = timeNow.Subtract(timeStart).TotalSeconds;

                float percent = (float)(reserchSecNow / reserchSecMax);

                double reserchSecToEnd = timeEnd.Subtract(timeNow).TotalSeconds;
                DateTime timeToEnd = new DateTime().AddSeconds(reserchSecToEnd);

                researchProgressImage.fillAmount = percent;

                string timeToEndSTR = "";
                string timeDay = "";
                string timeHour = "";
                string timeMinute = "";
                string timeSecond = "";

                if (timeToEnd.Day > 1)
                    timeDay += Convert.ToString(timeToEnd.Day) + "|";

                if (timeToEnd.Hour > 0 || (timeToEnd.Hour <= 0 && timeToEndSTR != "")) {
                    timeHour += Convert.ToString(timeToEnd.Hour) + ":";
                }
                if (timeToEnd.Minute > 0 || (timeToEnd.Minute <= 0 && timeToEndSTR != ""))
                {
                    if (timeToEnd.Minute < 10)
                        timeMinute += "0";

                    timeMinute += Convert.ToString(timeToEnd.Minute) + ":";
                }

                    if (timeToEnd.Second < 10 && timeToEnd.Minute != 0)
                        timeSecond += "0";
                timeSecond += Convert.ToString(timeToEnd.Second);

                timeToEndSTR = timeDay + timeHour + timeMinute + timeSecond;

                string text = "Research " + timeToEndSTR;
                researchProgressText.text = text;

            }
            //Если иследование завершено
            else {
                researchProgressImage.fillAmount = 1;
                string text = "Research not selected";
                researchProgressText.text = text;
            }
        }

    }
    [SerializeField]
    Image researchProgressImage;
    [SerializeField]
    Text researchProgressText;

    [SerializeField]
    Text infoText;
    void InfoTest() {
        if (infoText && research) {
            if (research.GetCountTech() < 3) {
                infoText.gameObject.SetActive(true);
            }
            else {
                infoText.gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        iniSetings();
        iniResearh();
    }

    // Update is called once per frame
    void Update()
    {
        TestCountTech();
        TestTextNeirodata();
        TestTextTech();

        testImageFon();

        testNowReseach();
        InfoTest();
    }
}
