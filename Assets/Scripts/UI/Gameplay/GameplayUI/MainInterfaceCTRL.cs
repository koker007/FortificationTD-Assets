using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager.UI;
using UnityEngine;

public class MainInterfaceCTRL : MonoBehaviour
{
    public static MainInterfaceCTRL main;

    GameplayCTRL gameplayCTRL;
    void iniGameplayCTRL() {
        if (gameplayCTRL == null) {
            GameObject gameplayCTRLObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayCTRLObj != null) {
                gameplayCTRL = gameplayCTRLObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    [SerializeField]
    GameObject playerIndicatorPrefab;
    [SerializeField]
    GameObject playerIndicators;
    List<PlayerIndicatorUI> playerIndicatorsUI = new List<PlayerIndicatorUI>();
    void TestPlayerIndicators() {
        if (gameplayCTRL != null && playerIndicatorPrefab != null && playerIndicators != null) {

            testCreate();
            testDelite();

            //Проверка на создание обьекта
            void testCreate()
            {
                foreach (Player player in gameplayCTRL.players) {
                    if (!player.isLocalPlayer)
                    {
                        bool foundIndicator = false;

                        foreach (PlayerIndicatorUI playerIndicator in playerIndicatorsUI)
                        {
                            if (playerIndicator.playerMe != null && playerIndicator.playerMe == player)
                            {
                                foundIndicator = true;
                                break;
                            }
                        }


                        //Создаем индикатор если он не был найден
                        if (!foundIndicator && player.MyCursorHand != null)
                        {
                            //создаем обьект
                            GameObject PlayerIndicatorObjNew = Instantiate(playerIndicatorPrefab, playerIndicators.transform);
                            //Вытаскиваем контроллер
                            PlayerIndicatorUI playerIndicatorUI = PlayerIndicatorObjNew.GetComponent<PlayerIndicatorUI>();
                            if (playerIndicatorUI != null)
                            {
                                playerIndicatorUI.playerMe = player;
                                playerIndicatorsUI.Add(playerIndicatorUI);
                            }
                        }
                    }
                }
            }

            //Проверка на удаление
            void testDelite()
            {
                foreach (PlayerIndicatorUI playerIndicator in playerIndicatorsUI) {
                    //Если игрок не обнаружен
                    if (playerIndicator.playerMe == null) {
                        playerIndicatorsUI.Remove(playerIndicator);
                        Destroy(playerIndicator.gameObject);
                    }
                }
            }
        }
    }

    [SerializeField]
    GameObject Map;

    [SerializeField]
    GameObject BuildPanels;

    [SerializeField]
    GameObject UIPause;

    [SerializeField]
    Player playerMe;
    void iniPlayerMe() {
        if (playerMe == null && gameplayCTRL != null) {
            //Перебираем всех игроков в поисках себя
            foreach (Player player in gameplayCTRL.players) {
                if (player.isLocalPlayer) {
                    playerMe = player;
                    break;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;

        InvokeRepeating("iniGameplayCTRL", 0, Random.Range(3f, 5f));
        InvokeRepeating("iniPlayerMe", 0.1f, Random.Range(3f, 5f));
    }

    // Update is called once per frame
    void Update()
    {
        //TestPosBuildPanel();
        TestPlayerIndicators();

        TestInfoPanel();

        TestHidePanels();

        TestPause();
    }

    Vector3 buildPanelNormalPos = new Vector3();
    void TestPosBuildPanel() {
        if (gameplayCTRL != null && BuildPanels != null && playerMe != null) {
            //Запоминаем стартовую позицию
            if (buildPanelNormalPos == new Vector3()) {
                RectTransform rectTransform = BuildPanels.GetComponent<RectTransform>();
                buildPanelNormalPos = rectTransform.localPosition;
            }
                

            //Если есть выбраная точка
            if (playerMe.controlsMouse.SelectCellCTRL != null)
            {
                //поднимаем панель
                RectTransform rectTransform = BuildPanels.GetComponent<RectTransform>();

                float need = buildPanelNormalPos.y;
                float now = rectTransform.localPosition.y;
                now += (need - rectTransform.localPosition.y)* Time.unscaledDeltaTime * 2;
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, now, rectTransform.localPosition.z);

                
            }
            else {
                //Опускаем пнель
                RectTransform rectTransform = BuildPanels.GetComponent<RectTransform>();

                float need = buildPanelNormalPos.y - rectTransform.sizeDelta.y;
                float now = rectTransform.localPosition.y;
                now += (need - rectTransform.localPosition.y) * Time.unscaledDeltaTime * 2;
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, now, rectTransform.localPosition.z);
            }
        }
    }

    [SerializeField]
    GameObject InfoPanelTAB;

    void TestInfoPanel() {
        if (InfoPanelTAB != null) {
            //если нажата кнопка вызывающая панель, то открываем
            if (Input.GetKey(KeyCode.Tab)) {
                if (!InfoPanelTAB.active) {
                    InfoPanelTAB.active = true;
                }
            }
            else {
                if (InfoPanelTAB.active) {
                    InfoPanelTAB.active = false;
                }
            }
        }
    }

    [SerializeField]
    RectTransform UpPanel;
    [SerializeField]
    RectTransform DownPanel;
    bool NeedHidePanels = false;
    void TestHidePanels() {
        if (gameplayCTRL && UpPanel && DownPanel) {
            bool needHide;

            if (gameplayCTRL.gamemode == 2)
            {
                if (Input.GetKeyDown(KeyCode.KeypadMinus)) {
                    NeedHidePanels = !NeedHidePanels;
                }

                needHide = NeedHidePanels;
            }
            else {
                NeedHidePanels = false;
                needHide = true;
            }

            float needUP = 0;
            float needDown = 0;

            if (needHide)
            {
                needUP = -10;
                needDown = 1.5f;
            }
            else {
                needUP = 0;
                needDown = 0;
            }


            //Проверяем положение мыщи на экране
            Vector2 mousePosPixNow = Input.mousePosition;
            Vector2 mousePosPercentNow = new Vector2(mousePosPixNow.x/Screen.width, mousePosPixNow.y/Screen.height);

            if (mousePosPercentNow.y > 0.95f) needUP += 1;

            //Двигаем панели
            UpPanel.pivot = new Vector2(UpPanel.pivot.x, UpPanel.pivot.y+((needUP-UpPanel.pivot.y)*Time.unscaledDeltaTime*2));
            DownPanel.pivot = new Vector2(DownPanel.pivot.x, DownPanel.pivot.y + ((needDown - DownPanel.pivot.y) * Time.unscaledDeltaTime*2));
            //если за пределами
            if (UpPanel.pivot.y > 1) {
                UpPanel.pivot = new Vector2(UpPanel.pivot.x, 0.999f);
            }
            if (DownPanel.pivot.y < 0) {
                DownPanel.pivot = new Vector2(DownPanel.pivot.x, 0.001f);
            }
        }
    }

    void TestPause() {
        if (Player.me && Input.GetKeyDown(KeyCode.Pause)) {
            Player.me.CmdSetComFloat(Player.CommandSTR.TypePause, 0);
        }

        if (GameplayCTRL.main && UIPause) {
            if (GameplayCTRL.main.pause)
            {
                UIPause.SetActive(true);
            }
            else {
                UIPause.SetActive(false);
            }
        }
    }
}
