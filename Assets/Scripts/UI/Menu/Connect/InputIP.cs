using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

public class InputIP : MonoBehaviour
{
   public static InputIP main;

    [SerializeField]
    NetworkManager networkManager;

    public SteamLobby.Lobby selectLobby;

    void iniNetManager()
    {
        networkManager = GameObject.FindGameObjectWithTag("Network").GetComponent<NetworkManager>();
    }

    [SerializeField]
    InputField InputedIP;

    [SerializeField]
    RawImage IndicatorImage;

    [SerializeField]
    Text IndicatorText;

    [SerializeField]
    Texture Bad;
    [SerializeField]
    Texture Process;

    public void SetIPText(string IPstr) {
        if (InputedIP) {
            InputedIP.text = IPstr;
        }
    }
    
    bool TestIpCorrect() {
        bool correct = true;

        //вытаскиваем текст
        string Ip = InputedIP.text;

        //проверяем
        string numStr = "";
        //точки должно быть 3
        int comma = 0;
        for (int x = 0; x < Ip.Length; x++) {
            //Нужна проверка числа
            bool numTest = false;
            //если число
            if (Ip[x] == '0' || Ip[x] == '1' || Ip[x] == '2' || Ip[x] == '3' || Ip[x] == '4' || Ip[x] == '5' || Ip[x] == '6' || Ip[x] == '7' || Ip[x] == '8' || Ip[x] == '9')
            {
                numStr += Ip[x];
                //Если число последнее
                if ((Ip.Length-1) == x) {
                    numTest = true;
                }
            }
            else if (Ip[x] == '.') {
                numTest = true;
                //Если не первая запятая
                if (comma != 0 && Ip[x-1] == '.')
                {
                    //Если предыдущий символ запятая, все плохо
                    correct = false;
                    numTest = false;
                }
                comma++;
            }
            else
            {
                correct = false;
            }

            //проверка числа когда число добавлено
            if (numTest) {
                //Если число вышло за диапазон то все плохо
                int count = System.Convert.ToInt32(numStr);
                if (count < 0 || count > 255) {
                    correct = false;
                }
                numStr = "";
            }
        }

        //Если запятых больше 3-х то ошибка
        if (comma != 3) {
            correct = false;
        }

        return correct;
    }

    void setIndicatorColor(Texture texture) {
        IndicatorImage.texture = texture;

        IndicatorImage.color = new Color(1,1,1,1);
    }

    public void ClickConnect() {
        if (networkManager != null && InputedIP != null && IndicatorImage != null && IndicatorText != null) {
            //проверяем текст на коррекстность
            bool correctIP = TestIpCorrect();
            


            string text = "";
            if (!correctIP)
            {
                text = "Not correct IP";
                setIndicatorColor(Bad);
            }
            else {

                //проверяем айпи на совпадение в списке лобби
                string lobbyPass = "";
                foreach (SteamLobby.Lobby lobbyTest in SteamLobby.lobbiesList) {
                    bool found = false;
                    if (lobbyTest != null) {
                        lobbyPass = "";
                        foreach (SteamLobby.MetaData metaData in lobbyTest.metaDatas) {
                            if (metaData.key == SteamLobby.LobbyKeys.hostAddres)
                            {
                                if (metaData.value == InputedIP.text)
                                {
                                    found = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if (metaData.key == SteamLobby.LobbyKeys.password) {
                                lobbyPass = metaData.value;
                            }
                        }
                    }

                    if (found)
                        break;
                }

                if (lobbyPass == "")
                {

                    text = "";
                    setIndicatorColor(Process);

                    if (mainParam != null)
                        mainParam.game.ConnectIp = InputedIP.text;

                    networkManager.networkAddress = InputedIP.text;
                    networkManager.StartClient();
                }
                else if (lobbyPass != "" && lobbyPass == PanelConnect.main.InputPassword.text)
                {
                    text = "";
                    setIndicatorColor(Process);

                    if (mainParam != null)
                        mainParam.game.ConnectIp = InputedIP.text;

                    networkManager.networkAddress = InputedIP.text;
                    networkManager.StartClient();
                }
                else {
                    text = "Bad password";
                    setIndicatorColor(Bad);
                }
            }

            IndicatorText.text = text;
            
        }
    }

    public void ConnectSelectLobby() {
        if (networkManager && selectLobby != null) {
            if (networkManager.isNetworkActive) {
                return;
            }



            string hostAddress = SteamMatchmaking.GetLobbyData(selectLobby.lobbySID, SteamLobby.LobbyKeys.hostAddres);
            NetworkCTRL.networkManager.networkAddress = hostAddress;
            NetworkCTRL.networkManager.StartClient();
        }
    }
    public void SetIndicatorText(string text_funk, bool good) {
        if (IndicatorText) {
            IndicatorText.text = text_funk;
            if(good)
                setIndicatorColor(Process);
            else setIndicatorColor(Bad);
        }
    }

    Setings mainParam;
    public void iniMainParam() {
        mainParam = GameObject.FindGameObjectWithTag("Setings").GetComponent<Setings>();
    }

    public void setIpDefolt() {
        if (mainParam != null && InputedIP != null) {
            InputedIP.text = mainParam.game.ConnectIp;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        iniNetManager();
        iniMainParam();
        Invoke("setIpDefolt", 0.1f);
        //setIpDefolt();
    }

    //Уменьшение прозрачности
    void TestIndicatorColor() {
        if (IndicatorImage != null && IndicatorImage.color.a != 0) {
            Color color = IndicatorImage.color;
            color.a -= Time.deltaTime;
            if (color.a < 0)
                color.a = 0;

            IndicatorImage.color = color;

        }
    }

    // Update is called once per frame
    void Update()
    {
        TestIndicatorColor();
    }

}
