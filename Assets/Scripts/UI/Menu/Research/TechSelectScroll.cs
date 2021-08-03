using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechSelectScroll : MonoBehaviour
{

    [Header("Techs")]
    [SerializeField]
    GameObject[] TechDamage;
    [SerializeField]
    GameObject[] TechDistance;
    [SerializeField]
    GameObject[] TechAccuracy;
    [SerializeField]
    GameObject[] TechSpeed;
    [SerializeField]
    GameObject[] TechRotate;

    [SerializeField]
    GameObject TechBuildMinigun;
    [SerializeField]
    GameObject TechBuildLaser;
    [SerializeField]
    GameObject TechBuildThunder;
    [SerializeField]
    GameObject TechBuildRocket;
    [SerializeField]
    GameObject TechBuildArtillery;

    [Header("Scrolls")]
    [SerializeField]
    GameObject ReserchTechsContent;
    [SerializeField]
    GameObject DoneTechsContent;

    void iniRestTransform() {
        if (!MainRect) {
            MainRect = gameObject.GetComponent<RectTransform>();
            if (MainRect)
                posYReserch = MainRect.position.y;

            if (TitleRect) {
                posTitleLeft = TitleRect.offsetMin.x;
                posTitleRight = TitleRect.offsetMax.x;
            }
        }
    }

    float posYReserch = 0;
    RectTransform MainRect;
    [Header("Title")]
    [SerializeField]
    Text TitleSelectTech;
    float posTitleLeft = 0;
    float posTitleRight = 0;
    [SerializeField]
    RectTransform TitleRect;
    [SerializeField]
    string TitleKeyChose = "";
    [SerializeField]
    string TitleKeyAllDone = "";

    List<GameObject> TechList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        iniRestTransform();
    }

    // Update is called once per frame
    void Update()
    {
        TestRect();

        TestTextTitle();
        CalcTechListInContent();
    }

    void TestRect() {
        if (MainRect && TitleRect) {
            float posYNeed = 1;
            float posLeftNeed = posTitleLeft;
            float posRightNeed = posTitleRight;

            bool needUP = true;
            if (Setings.main && Setings.main.game.ResearchTech == "" || GPResearch.main && GPResearch.main.stats.researchTimeEnd < TimeFromNET.GetSecondsFrom2020(TimeFromNET.GetFastestNISTDate()))
            {
                posYNeed = posYNeed + 0.2f;
                posLeftNeed += 20;
                posRightNeed -= 20;

                needUP = false;
            }

            float posNow = MainRect.pivot.y;
            float posLeftNow = TitleRect.offsetMin.x;
            float posRightNow = TitleRect.offsetMax.x;

            if (needUP)
            {
                posNow += Time.unscaledDeltaTime * 0.01f;
                if (posNow > posYNeed) posNow = posYNeed;

                posLeftNow -= Time.unscaledDeltaTime * 10;
                if (posLeftNow < posLeftNeed) posLeftNow = posLeftNeed;
                posRightNow += Time.unscaledDeltaTime * 10;
                if (posRightNow > posRightNeed) posRightNow = posRightNeed;


            }
            else {
                posNow -= Time.unscaledDeltaTime * 0.01f;
                if (posNow < posYNeed) {
                    posNow = posYNeed;
                }

                posLeftNow += Time.unscaledDeltaTime * 10;
                if (posLeftNow > posLeftNeed) posLeftNow = posLeftNeed;
                posRightNow -= Time.unscaledDeltaTime * 10;
                if (posRightNow < posRightNeed) posRightNow = posRightNeed;
            }


            MainRect.pivot = new Vector2(MainRect.pivot.x, posNow);

            TitleRect.offsetMin = new Vector2(posLeftNow, TitleRect.offsetMin.y);
            TitleRect.offsetMax = new Vector2(posRightNow, TitleRect.offsetMax.y);
        }
    }

    TimeText TitleText;
    string oldTitleText = "-1";
    void TestTextTitle() {
        if (Setings.main.game != null && TitleSelectTech) {
            //Если иследование не выбрано
            if (Setings.main && Setings.main.game.ResearchTech == "")
            {
                if (oldTitleText != Setings.main.game.ResearchTech) {
                    oldTitleText = Setings.main.game.ResearchTech;

                    string titleStr = "";
                    if (GPResearch.main.GetCountTech() < GPResearch.main.GetMaxTech())
                    {
                        titleStr = Setings.main.LangugeText.get_text_from_key(TitleKeyChose);
                    }
                    else {
                        titleStr = Setings.main.LangugeText.get_text_from_key(TitleKeyAllDone);
                    }

                    TitleText.Set(0, 0.1f, titleStr, false, true);
                }
            }
            else {
                if (oldTitleText != Setings.main.game.ResearchTech)
                {
                    oldTitleText = Setings.main.game.ResearchTech;

                    string titleStr = "";

                    titleStr = Setings.main.LangugeText.get_text_from_key(Setings.main.game.ResearchTech + "Name");


                    TitleText.Set(0, 0.1f, titleStr, false, true);
                }
            }

            TitleSelectTech.text = TitleText.Get();
        }
    }

    static public float timeLastUpdateTech = 0; //Время на основе которого выбраны иследования
    void CalcTechListInContent() {
        if (ReserchTechsContent && TechList != null) {
            //Проверяем состояние


            //Если иследование не идет и времясписка не совпадает, обновляем
            if ((GPResearch.main.stats.researchTimeStart != timeLastUpdateTech && Setings.main.game.ResearchTech == "")) {
                //Чистим убираем
                if (TechList.Count > 0)
                {
                    foreach (GameObject TechObj in TechList) {
                        if (TechObj != null) {
                            Destroy(TechObj);
                        }
                    }
                    TechList.Clear();
                }
                //Создаем заного
                int inFail = 0;
                int techNow = GPResearch.main.GetCountTech();
                bool[] techIsUsed = new bool[GPResearch.main.GetMaxTech()];

                while (TechList.Count < 3 && inFail < 1000) {
                    float FixTime = GPResearch.main.stats.researchTimeStart + 719 * techNow + (TechList.Count + inFail) * 11;
                    //Получаем рандомное число
                    float FixRand = Calc.FixRandom(FixTime, 0, 10);

                    //Технология
                    GameObject TechPrefab = null;

                    //Добавляем только если ранее этот тип технологий еще не был добавлен
                    if (!techIsUsed[(int)FixRand]) {
                        //Алл дамаге
                        if ((int)FixRand == 0)
                            TechPrefab = getTechAllDamage();
                        //Алл Дистанс
                        else if ((int)FixRand == 1)
                            TechPrefab = getTechAllDistance();
                        //Алл акурасити
                        else if ((int)FixRand == 2)
                            TechPrefab = getTechAllAccurasity();
                        //Алл спид
                        else if ((int)FixRand == 3)
                            TechPrefab = getTechAllSpeed();
                        //Алл ротате
                        else if ((int)FixRand == 4)
                            TechPrefab = getTechAllRotate();

                        //Build Minigun
                        else if ((int)FixRand == 5)
                            TechPrefab = getTechBuildMinigun();
                        else if ((int)FixRand == 6)
                            TechPrefab = getTechBuildLaser();
                        else if ((int)FixRand == 7)
                            TechPrefab = getTechBuildThunder();
                        else if ((int)FixRand == 8)
                            TechPrefab = getTechBuildArtillery();
                        else if ((int)FixRand == 9)
                            TechPrefab = getTechBuildRocket();

                        techIsUsed[(int)FixRand] = true;
                    }

                    //Если технология есть
                    if (TechPrefab)
                    {
                        //Создаем технологию
                        GameObject TechObj = Instantiate(TechPrefab, ReserchTechsContent.transform);
                        RectTransform rect = TechObj.GetComponent<RectTransform>();
                        if (rect)
                        {
                            rect.pivot = new Vector2(TechList.Count * -1, rect.pivot.y);
                            TechList.Add(TechObj);
                        }
                        else {
                            Destroy(TechObj);
                        }
                    }
                    else {
                        inFail++;
                    }
                    
                }

                //оБНОВЛЕНИЕ СПИСКА ЗАВЕРШЕНО
                timeLastUpdateTech = GPResearch.main.stats.researchTimeStart;
            }
            else if(TechList.Count > 0 && Setings.main.game.ResearchTech != "") {
                //Чистим убираем
                if (TechList.Count > 0)
                {
                    foreach (GameObject TechObj in TechList)
                    {
                        if (TechObj != null)
                        {
                            Destroy(TechObj);
                        }
                    }
                    TechList.Clear();
                }
            }
        }

        GameObject getTechAllDamage() {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (!GPResearch.allGun.damage1) {
                techPrefabObj = TechDamage[0];
            }
            else if (!GPResearch.allGun.damage2) {
                techPrefabObj = TechDamage[1];
            }
            else if (!GPResearch.allGun.damage3) {
                techPrefabObj = TechDamage[2];
            }

            return techPrefabObj;
        }
        GameObject getTechAllDistance() {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (!GPResearch.allGun.dist1)
            {
                techPrefabObj = TechDistance[0];
            }
            else if (!GPResearch.allGun.dist2)
            {
                techPrefabObj = TechDistance[1];
            }
            else if (!GPResearch.allGun.dist3)
            {
                techPrefabObj = TechDistance[2];
            }

            return techPrefabObj;
        }
        GameObject getTechAllAccurasity()
        {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (!GPResearch.allGun.accuracy1)
            {
                techPrefabObj = TechAccuracy[0];
            }
            else if (!GPResearch.allGun.accuracy2)
            {
                techPrefabObj = TechAccuracy[1];
            }
            else if (!GPResearch.allGun.accuracy3)
            {
                techPrefabObj = TechAccuracy[2];
            }

            return techPrefabObj;
        }
        GameObject getTechAllSpeed()
        {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (!GPResearch.allGun.speed1)
            {
                techPrefabObj = TechSpeed[0];
            }
            else if (!GPResearch.allGun.speed2)
            {
                techPrefabObj = TechSpeed[1];
            }
            else if (!GPResearch.allGun.speed3)
            {
                techPrefabObj = TechSpeed[2];
            }

            return techPrefabObj;
        }
        GameObject getTechAllRotate()
        {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (!GPResearch.allGun.rotate1)
            {
                techPrefabObj = TechRotate[0];
            }
            else if (!GPResearch.allGun.rotate2)
            {
                techPrefabObj = TechRotate[1];
            }
            else if (!GPResearch.allGun.rotate3)
            {
                techPrefabObj = TechRotate[2];
            }

            return techPrefabObj;
        }

        GameObject getTechBuildMinigun() {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (GPResearch.CanResearchTech(GPResearch.Allgun.buildMinigunSTR))
            {
                techPrefabObj = TechBuildMinigun;
            }

            return techPrefabObj;
        }
        GameObject getTechBuildLaser()
        {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (GPResearch.CanResearchTech(GPResearch.Allgun.buildLaserSTR))
            {
                techPrefabObj = TechBuildLaser;
            }

            return techPrefabObj;
        }
        GameObject getTechBuildThunder()
        {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (GPResearch.CanResearchTech(GPResearch.Allgun.buildThunderSTR))
            {
                techPrefabObj = TechBuildThunder;
            }

            return techPrefabObj;
        }
        GameObject getTechBuildRocket()
        {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (GPResearch.CanResearchTech(GPResearch.Allgun.buildRosketSTR))
            {
                techPrefabObj = TechBuildRocket;
            }

            return techPrefabObj;
        }
        GameObject getTechBuildArtillery()
        {
            GameObject techPrefabObj = null;

            //Проверяем можно ли изучать 
            if (GPResearch.CanResearchTech(GPResearch.Allgun.buildArtillerySTR))
            {
                techPrefabObj = TechBuildArtillery;
            }

            return techPrefabObj;
        }
    }
}
