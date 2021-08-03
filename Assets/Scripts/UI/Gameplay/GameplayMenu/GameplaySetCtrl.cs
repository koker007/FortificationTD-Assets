using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameplaySetCtrl : MonoBehaviour
{
    [SerializeField]
    GameplayCTRL gameplay;
    void iniGameplay() {
        if (gameplay == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                gameplay = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    [SerializeField]
    Setings setings;
    void iniSet() {
        if (setings == null)
        {
            GameObject setObj = GameObject.FindGameObjectWithTag("Setings");
            if (setObj != null)
            {
                setings = setObj.GetComponent<Setings>();
            }
        }
    }

    public void ButtonRandomKeyGenClick() {
        if (gameplay != null) {
            gameplay.KeyGenRandomMapClick();
        }
    }
    public void ButtonVerifiedKeyGenClick()
    {
        if (GameplayCTRL.main != null)
        {
            GameplayCTRL.main.KeyGenVerifiedMapClick();
        }
    }

    [SerializeField]
    InputField InputfieldTextSeedMap;
    public void InputfieldSeedClick() {
        if (InputfieldTextSeedMap != null && gameplay != null) {
            gameplay.SetKeyGen(InputfieldTextSeedMap.text);
        }
    }

    void TestInputfieldSeed() {
        if (gameplay != null && InputfieldTextSeedMap != null) {
            if (InputfieldTextSeedMap.text != gameplay.KeyGen) {
                InputfieldTextSeedMap.text = gameplay.KeyGen;
            }
        }
    }

    public void ButtonReadyClick() {
        if (gameplay != null) {
            foreach (Player player in gameplay.players) {
                if (player.isLocalPlayer) {
                    player.CmdSetReady();
                    break;
                }
            }
        }
    }

    [SerializeField]
    Slider sliderColor;
    [SerializeField]
    Image HandleColor;
    public void SliderColorClick() {
        if (sliderColor != null) {
            if (setings != null && setings.game != null) {
                setings.game.playerColor = sliderColor.value;
            }

            if (gameplay != null) {
                foreach (Player player in gameplay.players) {
                    if (player.isLocalPlayer) {
                        //Получаем цвет 
                        float r = 0;
                        float g = 0;
                        float b = 0;
                        if (sliderColor.value < 1) {
                            r = 1;
                            g = sliderColor.value;
                        }
                        else if (sliderColor.value < 2) {
                            r = 1 - (sliderColor.value - 1);
                            g = 1;
                        }
                        else if (sliderColor.value < 3) {
                            g = 1;
                            b = sliderColor.value - 2;
                        }
                        else if (sliderColor.value < 4) {
                            g = 1 - (sliderColor.value - 3);
                            b = 1;
                        }
                        else if (sliderColor.value < 5) {
                            r = sliderColor.value - 4;
                            b = 1;
                        }
                        else if (sliderColor.value <= 6) {
                            r = 1;
                            b = 1 - (sliderColor.value - 5);
                        }

                        Vector3 newColor = new Vector3(r,g,b);

                        if (HandleColor != null) {
                            Vector3 handleColor = new Vector3((newColor.x + 3)/4, (newColor.y + 3)/4, (newColor.z + 3)/4);
                            HandleColor.color = new Color(handleColor.x, handleColor.y, handleColor.z);
                        }

                        player.CmdSetColor(newColor);
                        break;
                    }
                }
            }


        }
    }
    bool IniColorSlider = false;
    void IniSliderColor() {
        if (!IniColorSlider && gameplay != null && sliderColor != null && setings != null && setings.game != null) {
            sliderColor.value = setings.game.playerColor;

            SliderColorClick(); //чтобы передать цвет серверу

            IniColorSlider = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("iniGameplay", 0, 2);
        InvokeRepeating("iniSet", 0, 1.99f);
    }

    // Update is called once per frame
    void Update()
    {
        IniSliderColor();
        TestInputfieldSeed();
    }
}
