using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoTextCTRL : MonoBehaviour
{
    [SerializeField]
    static public InfoTextCTRL main;

    [SerializeField]
    Setings setings;
    void iniSettings() {
        if (setings == null) {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj != null) {
                setings = setingsObj.GetComponent<Setings>();
            }
        }
    }

    [SerializeField]
    Player playerMe;
    void iniPlayer() {
        if (playerMe == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                GameplayCTRL gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
                if (gameplayCTRL != null) {
                    foreach (Player player in gameplayCTRL.players) {
                        if (player.isLocalPlayer)
                        {
                            playerMe = player;
                            return;
                        }
                    }
                }
            }
        }
    }

    [SerializeField]
    Text textComponent;

    [SerializeField]
    GameObject InfoPanelHide;

    public string textKeyNow = "";
    string textKeyOld = "";


    // Start is called before the first frame update
    void Start()
    {
        iniSettings();
        InvokeRepeating("iniPlayer", 0.5f, Random.RandomRange(4.0f, 8.0f));
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestTextInfo();
    }

    bool foundText = false;
    float InfoViewPause = 0;
    bool InfoView = false;
    string InfotextStr = "";
    void TestTextInfo()
    {
        if (setings != null && setings.game != null && setings.LangugeText != null && textComponent != null && playerMe != null && InfoPanelHide != null)
        {
            //Если положение мыщи долго не меняется
            if (playerMe.controlsMouse.OldMousePos == new Vector2(Input.mousePosition.x, Input.mousePosition.y) &&
                !playerMe.controlsMouse.NormalClickL &&
                !playerMe.controlsMouse.DoubleClickL
                )
            {
                InfoViewPause += Time.unscaledDeltaTime;
                if (InfoViewPause > 0.25f)
                {
                    InfoView = true;
                }
            }

            //Если ключ изменился
            bool changedTextKey = false;
            if (textKeyOld != textKeyNow)
            {
                InfoView = false;
                InfoViewPause = 0;
                changedTextKey = true;
                InfotextStr = setings.LangugeText.get_text_from_key(textKeyNow);

                textKeyOld = textKeyNow;
            }

            if (InfoView)
            {
                if(foundText)
                    InfoPanelHide.active = true;
                

                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();


                if (InfotextStr != "")
                {
                    textComponent.text = InfotextStr;
                    foundText = true;
                }
                else {
                    textComponent.text = "";
                    foundText = false;
                }

                if (foundText)
                {
                    //количество строк для текста
                    int MaxStr = textComponent.text.Length / 42; //42 символа в одной строке
                    MaxStr++;

                    rectTransform.sizeDelta = new Vector2(150, 8.8f * MaxStr + 10);
                }
                else
                {
                    InfoPanelHide.active = false;
                }

                //Проверяем положение курсора мыши на экране
                int toRight = Screen.width - (int)Input.mousePosition.x;
                int toDown = (int)Input.mousePosition.y - Screen.height;

                float x = -0.025f;
                float y = 1.05f;
                //отсчет слева на право снизу вверх
                if (toRight < rectTransform.sizeDelta.x)
                {
                    x = 1.025f;
                }

                if (toDown < rectTransform.sizeDelta.y - Screen.height)
                {
                    y = -0.05f;
                }

                //Debug.Log(toDown + " " + (rectTransform.sizeDelta.y - Screen.height));

                rectTransform.pivot = new Vector2(x, y);
                rectTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            }
            //скрываем
            else {
                InfoPanelHide.active = false;
            }
        }
    }
}
