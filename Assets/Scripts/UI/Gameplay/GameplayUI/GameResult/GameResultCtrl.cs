using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResultCtrl : MonoBehaviour
{

    GameplayCTRL gameplayCTRL;
    void iniGameplayCtrl() {
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    Setings setings;
    void iniSetingsCtrl() {
        if (setings == null) {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj != null) {
                setings = setingsObj.GetComponent<Setings>();
                IniTranslate();
            }
        }
    }

    //последнее время обновления
    float timeUpdateOld = 0;

    //Время показа результата
    float timeResultView = 0;
    void testReCalc() {
        if (gameplayCTRL != null) {
            if (timeUpdateOld != gameplayCTRL.timeGameplayEnd) {
                timeUpdateOld = gameplayCTRL.timeGameplayEnd;
                timeResultView = 0;

                ReCalcTitleText();
                ReCalcMainText();
            }

            void ReCalcTitleText()
            {
                if (TitleText && setings)
                {
                    if (gameplayCTRL.GameoverVictory)
                    {
                        string text = "";
                        if (KeyVictory != "")
                        {
                            foreach (StringKeyFileLoader.KeyAndText kat in setings.LangugeText.KaT)
                            {
                                if (kat.Key == KeyVictory)
                                {
                                    text = kat.Text;
                                    break;
                                }
                            }
                        }

                        if (text == "")
                        {
                            TitleText.text = "Victory";
                        }
                        else
                        {
                            TitleText.text = text;
                        }
                    }
                    else
                    {
                        string text = "";
                        if (KeyDefeat != "")
                        {
                            foreach (StringKeyFileLoader.KeyAndText kat in setings.LangugeText.KaT)
                            {
                                if (kat.Key == KeyDefeat)
                                {
                                    text = kat.Text;
                                    break;
                                }
                            }
                        }

                        if (text == "")
                        {
                            TitleText.text = "Defeat";
                        }
                        else
                        {
                            TitleText.text = text;
                        }
                    }
                }
            }
            void ReCalcMainText() {
                if (MainDefeatText) {
                    if (!gameplayCTRL.GameoverVictory)
                    {
                        MainDefeatText.gameObject.active = true;

                        string text = "";
                        int num = UnityEngine.Random.Range(0, KeyMainDefeat.Length);
                        if (KeyMainDefeat.Length > 0 && KeyMainDefeat[num] != "")
                        {
                            foreach (StringKeyFileLoader.KeyAndText kat in setings.LangugeText.KaT)
                            {
                                if (kat.Key == KeyMainDefeat[num])
                                {
                                    text = kat.Text;
                                    break;
                                }
                            }
                        }

                        if (text == "")
                        {
                            MainDefeatStr = "the defeated one does not write history";
                        }
                        else
                        {
                            MainDefeatStr = text;
                        }
                    }
                    else MainDefeatText.gameObject.active = false;
                }
            }
        }
        else
            iniGameplayCtrl();
    }
    void testTime() {
        timeResultView += Time.unscaledDeltaTime;
    }

    [SerializeField]
    Text TitleText;

    [SerializeField]
    string KeyVictory = "";
    [SerializeField]
    string KeyDefeat = "";
    [SerializeField]
    string KeySeed = "";
    string TranslateSeed = "";
    [SerializeField]
    string KeyGameplay = "";
    string TranslateGameplay = "";
    [SerializeField]
    string KeyKills = "";
    string TranslateKills = "";
    [SerializeField]
    string KeyNeirodata = "";
    string TranslateNeirodata = "";

    [SerializeField]
    RectTransform TitleRect;
    [SerializeField]
    RectTransform MainRect;
    [SerializeField]
    Image MainFonImage;
    [SerializeField]
    Text MainDefeatText;
    [SerializeField]
    string[] KeyMainDefeat;
    string MainDefeatStr = "";
    [SerializeField]
    Color FonStartColor;
    [SerializeField]
    Color FonDefeatColor;
    [SerializeField]
    Color FonVictoryColor;

    /// <summary>
    /// раскрытие таблицы
    /// </summary>
    void TestOpenTab() {
        float titleNeed = 40;
        float mainNeed = 342;

        if (timeResultView < 1)
        {
            TitleRect.sizeDelta = new Vector2(TitleRect.sizeDelta.x, 360);
            MainRect.sizeDelta = new Vector2(MainRect.sizeDelta.x, 1);

            TitleRect.localScale = new Vector3(1, 1, 1);
            MainRect.localScale = new Vector3(1, 1, 1);
        }
        else if (timeResultView < 5)
        {
            float titleNow = TitleRect.sizeDelta.y;
            titleNow += (titleNeed - titleNow) * Time.unscaledDeltaTime;

            float MainNow = MainRect.sizeDelta.y;
            MainNow += (mainNeed - MainNow) * Time.unscaledDeltaTime;

            TitleRect.sizeDelta = new Vector2(TitleRect.sizeDelta.x, titleNow);
            MainRect.sizeDelta = new Vector2(MainRect.sizeDelta.x, MainNow);

            TitleRect.localScale = new Vector3(1, 1, 1);
            MainRect.localScale = new Vector3(1, 1, 1);
        }
        else {
            TitleRect.sizeDelta = new Vector2(TitleRect.sizeDelta.x, titleNeed);
            MainRect.sizeDelta = new Vector2(MainRect.sizeDelta.x, mainNeed);

            TitleRect.localScale = new Vector3(1, 1, 1);
            MainRect.localScale = new Vector3(1, 1, 1);
        }
    }

    void TestVictory() {
        if (GameplayCTRL.main && GameplayCTRL.main.GameoverVictory) {
            if (MainDefeatText && MainDefeatText.gameObject.active)
                MainDefeatText.gameObject.active = false;

            if (MainFonImage && FonDefeatColor != null && FonStartColor != null)
            {
                if (timeResultView < 1)
                {
                    MainFonImage.color = FonStartColor;
                    if (RestartDefeatButton) RestartDefeatButton.ReStartValue();

                    ClearPlayersIndicators();
                    CloseVistoryInfo();
                }
                else if (timeResultView < 5)
                {
                    float coof = (timeResultView - 1) / 4;
                    MainFonImage.color = Color.Lerp(FonStartColor, FonDefeatColor, coof);

                    CreatePlayersIndicators();
                    SetVictoryInfo();
                }
                else
                {
                    MainFonImage.color = FonDefeatColor;
                    InirestartButton();
                }
            }
        }
    }
    void TestDefeat() {
        if (gameplayCTRL && !gameplayCTRL.GameoverVictory && MainDefeatText && MainDefeatText.gameObject.active) {
            string TextNow = "";
            foreach(char s in MainDefeatStr) {
                if (TextNow.Length < (timeResultView - 2.5f) / 0.05f)
                    TextNow += s;
            }

            MainDefeatText.text = TextNow;

            if (MainFonImage && FonDefeatColor != null && FonStartColor != null) {
                if (timeResultView < 1)
                {
                    MainFonImage.color = FonStartColor;
                    if(RestartDefeatButton) RestartDefeatButton.ReStartValue();

                    ClearPlayersIndicators();
                    CloseVistoryInfo();
                }
                else if (timeResultView < 5)
                {
                    float coof = (timeResultView - 1) / 4;
                    MainFonImage.color = Color.Lerp(FonStartColor, FonDefeatColor, coof);

                    
                }
                else {
                    MainFonImage.color = FonDefeatColor;
                    InirestartButton();
                }
            }

        }
    }


    void InirestartButton() {
        if (RestartDefeatButton) {
            if (!RestartDefeatButton.gameObject.active) {
                RestartDefeatButton.gameObject.active = true;
            }
        }
    }
    [SerializeField]
    ReStartButton RestartDefeatButton;

    void IniTranslate() {
        if (setings && setings.game != null) {
            TranslateSeed = setings.LangugeText.get_text_from_key(KeySeed);
            TranslateGameplay = setings.LangugeText.get_text_from_key(KeyGameplay);
            TranslateKills = setings.LangugeText.get_text_from_key(KeyKills);
            TranslateNeirodata = setings.LangugeText.get_text_from_key(KeyNeirodata);

            if (TranslateSeed == "") TranslateSeed = "Seed";
            if (TranslateGameplay == "") TranslateGameplay = "Gameplay";
            if (TranslateKills == "") TranslateKills = "Kills";
            if (TranslateNeirodata == "") TranslateNeirodata = "Neirodata";
        }
    }

    public void ClickRestartButton() {
        if (GameplayCTRL.main != null && GameplayCTRL.main.players != null) {
            foreach (Player player in GameplayCTRL.main.players) {
                if (player.isLocalPlayer) {
                    player.CmdSetComString("clickReadyRestart", "true");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        iniSetingsCtrl();
        testReCalc();
        testTime();

        TestOpenTab();
        testOpenFon();

        TestVictory();
        TestDefeat();
    }

    [SerializeField]
    Image fonOpen;
    void testOpenFon() {
        if (fonOpen && fonOpen.pixelsPerUnitMultiplier < 1) {
            fonOpen.pixelsPerUnitMultiplier += 0.10f * Time.unscaledDeltaTime;
            fonOpen.SetAllDirty();
        }
    }

    [SerializeField]
    Image InfoPanel;
    [SerializeField]
    VictoryInfo CountEnemy;
    [SerializeField]
    VictoryInfo CountGamePlayTime;
    [SerializeField]
    VictoryInfo CountNeirodata;
    [SerializeField]
    VictoryInfo CountSeed;

    [SerializeField]
    LeaderInThisMap LeadersTab;

    void SetVictoryInfo() {
        if (LeadersTab)
        {
            if (!LeadersTab.gameObject.active)
                LeadersTab.gameObject.active = true;

            LeadersTab.restart();
        }


        if (InfoPanel && !InfoPanel.enabled)
            InfoPanel.enabled = true;

        if (CountEnemy && !CountEnemy.gameObject.active)
        {
            CountEnemy.gameObject.active = true;
            string name = TranslateKills;

            CountEnemy.SetInfo(2, name, Convert.ToString(GameplayCTRL.main.kills));
        }
        if (CountGamePlayTime && !CountGamePlayTime.gameObject.active)
        {
            CountGamePlayTime.gameObject.active = true;

            string timeStr = ((int)(GameplayCTRL.main.timeGamePlay / 60)) + ":";
            if ((int)(GameplayCTRL.main.timeGamePlay % 60) < 10)
                timeStr += "0" + (int)(GameplayCTRL.main.timeGamePlay % 60);
            else timeStr += (int)(GameplayCTRL.main.timeGamePlay % 60);

            string name = TranslateGameplay;
            CountGamePlayTime.SetInfo(1, name, timeStr);
        }
        if (CountNeirodata && !CountNeirodata.gameObject.active)
        {
            CountNeirodata.gameObject.active = true;

            string name = TranslateNeirodata;
            CountNeirodata.SetInfo(3, name, Convert.ToString(GameplayCTRL.main.neirodata));
        }
        if (CountSeed && !CountSeed.gameObject.active) {
            CountSeed.gameObject.active = true;

            string name = TranslateSeed;
            CountSeed.SetInfo(3, name, GameplayCTRL.main.KeyGen);
        }

    }
    void CloseVistoryInfo() {
        if (LeadersTab && LeadersTab.gameObject.active)
            LeadersTab.gameObject.active = false;

        if (InfoPanel && InfoPanel.enabled)
            InfoPanel.enabled = false;

        if (CountEnemy && CountEnemy.gameObject.active)
            CountEnemy.gameObject.active = false;
        if (CountGamePlayTime && CountGamePlayTime.gameObject.active)
            CountGamePlayTime.gameObject.active = false;
        if (CountNeirodata && CountNeirodata.gameObject.active)
            CountNeirodata.gameObject.active = false;
        if (CountSeed && CountSeed.gameObject.active)
            CountSeed.gameObject.active = false;
    }

    [SerializeField]
    GameObject PrefabPlayerIndicator;
    List<GameplayUIInfoPanel> playerIndicatorList = new List<GameplayUIInfoPanel>();
    [SerializeField]
    RectTransform NullPosPlayerIndicator;
    [SerializeField]
    Transform content;
    void CreatePlayersIndicators() {
        if (playerIndicatorList.Count == 0 && GameplayCTRL.main && GameplayCTRL.main.players != null && GameplayCTRL.main.players.Count > 0 && content) {
            int startPivot = 8;

            //Создаем играющих игроков
            foreach (Player player in GameplayCTRL.main.players) {
                if (player.ReadyToStartGame) {
                    GameObject indicatorObj = Instantiate(PrefabPlayerIndicator, content);
                    if (indicatorObj) {


                        RectTransform rectTransform = indicatorObj.GetComponent<RectTransform>();
                        rectTransform.position = NullPosPlayerIndicator.position;
                        rectTransform.offsetMax = NullPosPlayerIndicator.offsetMax;
                        rectTransform.offsetMin = NullPosPlayerIndicator.offsetMin;
                        rectTransform.localScale = NullPosPlayerIndicator.localScale;
                        rectTransform.pivot = new Vector2(rectTransform.pivot.x, startPivot + playerIndicatorList.Count);

                        GameplayUIInfoPanel indicator = indicatorObj.GetComponent<GameplayUIInfoPanel>();


                        if (indicator)
                        {
                            indicator.SetPlayer(player);
                            playerIndicatorList.Add(indicator);
                        }
                        else {
                            Destroy(indicatorObj);
                        }
                    }
                }
            }
        }
    }
    void ClearPlayersIndicators() {
        if (playerIndicatorList.Count > 0)
        {
            foreach (GameplayUIInfoPanel playerInfo in playerIndicatorList)
            {
                if (playerInfo)
                {
                    Destroy(playerInfo.gameObject);
                }
            }

            playerIndicatorList.Clear();
        }
    }

    public void CloseOpenFon() {
        if (fonOpen)
        {
            fonOpen.pixelsPerUnitMultiplier = 0.01f;
            fonOpen.SetAllDirty();
        }
    }
}
