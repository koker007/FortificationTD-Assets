using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Steamworks;

public class PlayMenuCtrl : MonoBehaviour
{
    [SerializeField]
    NetworkManager networkManager;
    void iniNetworkManager() {
        if (networkManager == null) {
            GameObject netObj = GameObject.FindGameObjectWithTag("Network");
            if (netObj != null) {
                networkManager = netObj.GetComponent<NetworkManager>();
            }
        }
    }


    string gamemode = "";

    [Header("Server Parametrs")]
    [SerializeField]
    InputField ServerName;
    [SerializeField]
    Slider PlayersMax;
    [SerializeField]
    Text PlayerMaxCount;
    [SerializeField]
    InputField ServerPassword;

    [Header("Other")]
    [SerializeField]
    Image imageSingle;
    [SerializeField]
    Image imageServer;

    [SerializeField]
    Button buttonSingle;
    [SerializeField]
    Button buttonServer;

    [SerializeField]
    Sprite Deselect;
    [SerializeField]
    Sprite Select;

    [SerializeField]
    Button StartPlay;

    public void ClickButtonStartGame() {
        if (networkManager != null && NetworkCTRL.fizzySteamworks) {
            if (gamemode == "single") {
                //NetworkCTRL.fizzySteamworks. = (ushort)Random.Range(7100, 9999);
                networkManager.StartHost();
            }
            else if (gamemode == "server") {

                for (int num = 0; num < SteamLobby.LobbyNeedParam.metaDatas.Length; num++) {
                    //имя сервера
                    if (ServerName && SteamLobby.LobbyNeedParam.metaDatas[num].key == SteamLobby.LobbyKeys.name) {
                        if (ServerName.text == "")
                            SteamLobby.LobbyNeedParam.metaDatas[num].value = SteamFriends.GetPersonaName() + "'s game";
                        else {
                            SteamLobby.LobbyNeedParam.metaDatas[num].value = ServerName.text;
                        }
                    }
                    //пароль сервера
                    else if (ServerPassword && SteamLobby.LobbyNeedParam.metaDatas[num].key == SteamLobby.LobbyKeys.password)
                    {
                        SteamLobby.LobbyNeedParam.metaDatas[num].value = ServerPassword.text;
                    }
                    //хост адресс сервера
                    else if (SteamLobby.LobbyNeedParam.metaDatas[num].key == SteamLobby.LobbyKeys.hostAddres) {
                        SteamLobby.LobbyNeedParam.metaDatas[num].value = SteamUser.GetSteamID().ToString();
                    }
                }

                //NetworkCTRL.telepathyTransport.port = 7007;
                networkManager.StartHost();

                if(PlayersMax)
                    SteamLobby.CreateLobby(ELobbyType.k_ELobbyTypePublic , (int)PlayersMax.value);
            }
        }
    }

    public void ClickSingleButton() {
        gamemode = "single";
    }
    public void ClickServerButton() {
        gamemode = "server";
    }

    void testButtonsMode() {
        if (buttonSingle != null && imageSingle != null && buttonServer != null && imageServer != null) {
            if (gamemode == "single") {
                buttonSingle.interactable = false;
                imageSingle.sprite = Select;
                imageServer.sprite = Deselect;
                buttonServer.interactable = true;
            }
            else if (gamemode == "server") {
                buttonServer.interactable = false;
                imageServer.sprite = Select;
                imageSingle.sprite = Deselect;
                buttonSingle.interactable = true;
            }
        }
    }

    void iniServerParametrs() {
        if (Setings.main && Setings.main.game != null) {
            if (ServerName) {
                ServerName.text = Setings.main.game.serverName;
            }
            if (PlayersMax && Setings.main.game != null) {
                PlayersMax.value = Setings.main.game.serverPlayersMax;
            }
            if (ServerPassword) {
                ServerPassword.text = Setings.main.game.serverPassword;
            }
        }
    }
    void testServerParametrs()
    {
        if (Setings.main && Setings.main.game != null)
        {
            if (ServerName && ServerName.text != Setings.main.game.serverName)
            {
                Setings.main.game.serverName = ServerName.text;
                Setings.main.timeToSave = 0;
            }
            if (PlayersMax && PlayersMax.value != Setings.main.game.serverPlayersMax) {
                Setings.main.game.serverPlayersMax = (int)PlayersMax.value;
                Setings.main.timeToSave = 0;
            }
            if (ServerPassword && ServerPassword.text != Setings.main.game.serverPassword) {
                Setings.main.game.serverPassword = ServerPassword.text;
                Setings.main.timeToSave = 0;
            }

            if (PlayerMaxCount && PlayersMax)
            {
                PlayerMaxCount.text = System.Convert.ToString(PlayersMax.value);
            }
        }
    }

    void testPlayButton() {
        if (StartPlay != null) {
            if (gamemode == "single" || gamemode == "server")
            {
                StartPlay.interactable = true;
            }
            else {
                StartPlay.interactable = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        iniNetworkManager();

        iniServerParametrs();
    }

    // Update is called once per frame
    void Update()
    {
        testButtonsMode();
        testPlayButton();

        testServerParametrs();
    }
}
