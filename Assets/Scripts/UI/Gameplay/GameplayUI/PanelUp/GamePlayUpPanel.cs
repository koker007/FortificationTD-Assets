using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUpPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        iniGameplay();
        TestAlarm();

        TestTimerToEnd();
        TestNeirodata();
        TestSeed();
        TestGameplaySpeed();

    }

    GameplayCTRL gameplayCTRL;
    void iniGameplay() {
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    [Header("StatGame")]
    [SerializeField]
    Text TimerToReadyEnd;
    [SerializeField]
    Text NeiroDataCount;
    [SerializeField]
    Text SeedMap;

    [SerializeField]
    RectTransform EndGameButton;

    [Header("Alarms")]
    [SerializeField]
    AudioSource AlarmAS;
    [SerializeField]
    AudioClip AlarmAC;
    [SerializeField]
    AudioClip AlarmACEnd;

    [SerializeField]
    RectTransform AlarmPos;

    [SerializeField]
    GameObject AlarmPehota;
    [SerializeField]
    GameObject AlarmCar;
    [SerializeField]
    GameObject AlarmCrab;

    List<EnemyUPIconCTRL> Alarms = new List<EnemyUPIconCTRL>();

    bool AlarmSoundNeed = false;
    void TestAlarm() {
        if (gameplayCTRL != null) {
            //Сперва узнаем какое количество тревог необходимо
            int needAlarms = 0;

            if (gameplayCTRL.timeEpicSpawnInfantry < 10 && AlarmPehota != null)
                needAlarms++;
            if (gameplayCTRL.timeEpicSpawnAutomobile < 10 && AlarmCar != null)
                needAlarms++;
            if (gameplayCTRL.timeEpicSpawnCrab < 10 && AlarmCrab != null)
                needAlarms++;

            if (needAlarms != Alarms.Count) {
                ReCalcAlarms();
                AlarmSoundNeed = true;
            }
            //постоянный сигнал если врагов больше 2
            if (Alarms.Count > 2) {
                AlarmSoundNeed = true;
            }

            TestFlash();
            AlarmASTest();

            //Перерасчитать тревоги
            void ReCalcAlarms() {
                //Очистить предыдущие
                foreach (EnemyUPIconCTRL transform in Alarms) {
                    Destroy(transform.gameObject);
                }
                Alarms.Clear();

                if (gameplayCTRL.timeEpicSpawnInfantry < 10 && AlarmPehota != null)
                    AddAndTransform(AlarmPehota);
                if (gameplayCTRL.timeEpicSpawnAutomobile < 10 && AlarmCar != null)
                    AddAndTransform(AlarmCar);
                if (gameplayCTRL.timeEpicSpawnCrab < 10 && AlarmCrab != null)
                    AddAndTransform(AlarmCrab);

                //Добавить и сместить
                void AddAndTransform(GameObject IconObjPrefab) {
                    //Создаем обьект
                    GameObject iconObj = Instantiate(IconObjPrefab, AlarmPos.gameObject.transform.parent);
                    EnemyUPIconCTRL iconCTRL = iconObj.GetComponent<EnemyUPIconCTRL>();
                    RectTransform iconRect = iconObj.GetComponent<RectTransform>();

                    if (iconCTRL != null) {
                        iconCTRL.gameplayCTRL = gameplayCTRL;
                    }

                    //обьект создан перемещаем
                    if (iconRect != null) {
                        //iconRect.localPosition = new Vector3((4 + iconRect.sizeDelta.x) * Alarms.Count, iconRect.position.y);
                        iconRect.localScale = new Vector3(1, 1, 1);
                        iconRect.pivot = new Vector2(1 + Alarms.Count + 0.25f*Alarms.Count, 1);
                        //iconRect.position = new Vector3(iconRect.position.x - (5 + iconRect.position.x/20) * Alarms.Count, iconRect.position.y, iconRect.position.z);
                    }

                    if (iconCTRL == null || iconRect == null)
                    {
                        Destroy(iconObj);
                    }
                    else {
                        Alarms.Add(iconCTRL);
                    }

                }
            }

            //Мигание обводки
            void TestFlash() {
                if (gameplayCTRL.timeGamePlay%1 < 0.5f) {
                    foreach (EnemyUPIconCTRL alarm in Alarms) {
                        alarm.setObvodkaVisual(true);
                    }
                }
                else {
                    foreach (EnemyUPIconCTRL alarm in Alarms)
                    {
                        alarm.setObvodkaVisual(false);
                    }
                }
            }

            void AlarmASTest() {
                if (AlarmSoundNeed && AlarmAC && AlarmAS && !AlarmAS.isPlaying && Setings.main && Setings.main.game != null) {
                    AlarmSoundNeed = false;
                    if (Alarms.Count != 0)
                    {
                        AlarmAS.volume = Setings.main.game.volume_all * Setings.main.game.volume_sound;
                        AlarmAS.pitch = 0.8f + (Alarms.Count * 0.1f);
                        AlarmAS.PlayOneShot(AlarmAC);
                    }
                    //Звук выхода из усиленного спавна
                    else {
                        AlarmAS.volume = Setings.main.game.volume_all * Setings.main.game.volume_sound;
                        AlarmAS.pitch = 1;
                        AlarmAS.PlayOneShot(AlarmACEnd);
                    }
                }
            }
        }
    }

    float ReadyButtonPosNeed;
    void TestTimerToEnd() {
        if (gameplayCTRL && EndGameButton && TimerToReadyEnd) {
            int minMax = 10;

            int timeMin = 0;
            int timeSec = 0;

            GetTime();
            TestPos();
            TestTimer();

            void GetTime() {
                int time = (int)(gameplayCTRL.timeGamePlay % (60 * minMax));
                timeMin = time / 60;
                timeSec = time % 60;
            }
            void TestPos() {

                if (timeMin == minMax-1)
                {
                    EndGameButton.pivot = new Vector2(0, (EndGameButton.pivot.y+(1-EndGameButton.pivot.y)*Time.unscaledDeltaTime));
                }
                else {
                    EndGameButton.pivot = new Vector2(0, (EndGameButton.pivot.y + (0 - EndGameButton.pivot.y) * Time.unscaledDeltaTime));
                }
            }
            
            //Показать время
            void TestTimer() {
                int viewMin = 0;
                int viewSec = 0;

                if (timeMin < minMax - 1)
                {
                    viewMin = (minMax - 1) - timeMin;
                    viewSec = 60 - timeSec;

                    TimerToReadyEnd.text = System.Convert.ToString(viewMin) + ":" + System.Convert.ToString(viewSec);
                }
                else {
                    viewSec = 60 - timeSec;

                    TimerToReadyEnd.text = System.Convert.ToString(viewMin) + ":" + System.Convert.ToString(viewSec);
                }
            }
        }
    }
    void TestNeirodata() {
        if (gameplayCTRL && NeiroDataCount) {
            NeiroDataCount.text = "Neirodata: " + Convert.ToString(gameplayCTRL.neirodata);
        }
    }
    void TestSeed() {
        if (gameplayCTRL && SeedMap) {
            SeedMap.text = "Seed: " + Convert.ToString(gameplayCTRL.KeyGen);
        }
    }
    void TestGameplaySpeed() {
        if (GameplaySpeedNow && GameplayCTRL.main)
        {
            GameplaySpeedNow.value += (GameplayCTRL.main.timeScale - GameplaySpeedNow.value) * Time.unscaledDeltaTime;
        }
    }

    public void ClickReadyToEnd() {
        if (gameplayCTRL) {
            foreach (Player player in gameplayCTRL.players) {
                if (player.isLocalPlayer) {
                    if (!player.ReadyToEndGame) {
                        player.CmdSetComString("clickReadyEnd", "true");
                    }
                    else player.CmdSetComString("clickReadyEnd", "false");

                    break;
                }
            }
        }
    }
    public void ClickMenu() {
        if (!menu)
        {
            GameObject menuObj = GameObject.FindGameObjectWithTag("Menu");
            if (menuObj)
                menu = menuObj.GetComponent<MenuCtrl>();
        }

        if (menu && menu.MenuALL) {
            if (menu.MenuALL.active)
                menu.MenuALL.active = false;
            else
                menu.MenuALL.active = true;
        }
    }
    MenuCtrl menu;

    [SerializeField]
    Slider GameplaySpeedNow;
    [SerializeField]
    Slider GameplaySpeedNeed;

    public void SetGameplaySpeed() {
        if (GameplaySpeedNow && GameplaySpeedNeed && Player.me) {
            Player.me.CmdSetComFloat(Player.CommandSTR.TypeTimeScaleNeed, GameplaySpeedNeed.value);
        }    
    }

}
