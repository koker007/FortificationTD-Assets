using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelConnect : MonoBehaviour
{
    public static PanelConnect main;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        RefreshLobbyList();
    }

    // Update is called once per frame
    void Update()
    {
        testList();
        testSelectLobbyInfo();

        testAutoRefreshLobby();
    }

    [SerializeField]
    GameObject content;
    [SerializeField]
    GameObject LobbyInfoPrefab;
    [SerializeField]
    public InputField InputPassword;

    [SerializeField]
    Text[] ConnectInfoText;

    public void RefreshLobbyList() {
        SteamLobby.GetNewLobbyList();
    }

    List<LobbyInfo> lobbyInfos = new List<LobbyInfo>();
    void testList() {
        if (lobbyInfos.Count != SteamLobby.lobbiesList.Count) {
            if (SteamLobby.lobbiesList.Count > 0 && SteamLobby.lobbiesList.Count > lobbyInfos.Count)
            {
                ButtonSounds.LobbyFound();
            }

            ClearContent();
            AddLobyyList();
        }

        void ClearContent() {
            foreach (LobbyInfo lobbyInfo in lobbyInfos) {
                Destroy(lobbyInfo.gameObject);
            }
            lobbyInfos = new List<LobbyInfo>();
        }
        void AddLobyyList() {

            if (LobbyInfoPrefab && content) {
                //Перебираем все найденные лобби
                foreach (SteamLobby.Lobby lobby in SteamLobby.lobbiesList) {
                    if (lobby != null) {
                        GameObject lobbyInfoObj = Instantiate(LobbyInfoPrefab, content.transform);
                        if (lobbyInfoObj) {
                            LobbyInfo lobbyInfo = lobbyInfoObj.GetComponent<LobbyInfo>();

                            RectTransform rect = lobbyInfo.GetComponent<RectTransform>();
                            if (!lobbyInfo || !rect)
                            {
                                Destroy(lobbyInfoObj);
                                break;
                            }
                            lobbyInfos.Add(lobbyInfo);

                            lobbyInfo.SetLobby(lobby);

                            rect.pivot = new Vector2(rect.pivot.x, lobbyInfos.Count);

                        }
                    }
                }
            }
        }
    }

    public TimeText lobbyInfoText;

    [SerializeField]
    public Text TextConnect;
    [SerializeField]
    Text TextChooseLobby;
    [SerializeField]
    Text TextInputPassword;
    [SerializeField]
    Text TextNotCorrectPassword;
    [SerializeField]
    Text TextLobbyIsFull;
    void testSelectLobbyInfo() {
        if (ConnectInfoText != null) {
            for (int num = 0; num < ConnectInfoText.Length; num++) {
                if (num == 0)
                {
                    ConnectInfoText[num].text = lobbyInfoText.Get();
                }
                else {
                    ConnectInfoText[num].text = ConnectInfoText[num-1].text;
                }
            }
        }
    }

    [SerializeField]
    Text textRefreshExample;
    [SerializeField]
    Text textRefreshButton;
    bool AutoRefreshON = false;
    float timeToRefresh = 0;
    void testAutoRefreshLobby() {
        if (AutoRefreshON)
        {
            timeToRefresh -= Time.unscaledDeltaTime;
            if (timeToRefresh < 0)
            {
                timeToRefresh = 5;
                RefreshLobbyList();
            }

            if (textRefreshButton != null && textRefreshExample != null)
            {
                textRefreshButton.text = textRefreshExample.text + " " + ((int)timeToRefresh + 1);
            }
        }
        else {
            if (textRefreshButton != null && textRefreshExample != null)
            {
                textRefreshButton.text = textRefreshExample.text;
            }
        }
    }

    public void ClickButtonRefresh() {
        if (AutoRefreshON) {
            AutoRefreshON = false;
        }
        else if (!AutoRefreshON) {
            AutoRefreshON = true;
            timeToRefresh = 5;
            RefreshLobbyList();
        }
    }

    static public void ConnectSelectLobby()
    {
        if (NetworkCTRL.networkManager.isNetworkActive)
        {
            return;
        }

        //Если лобби не выбрано
        if (LobbyInfo.SelectedLobby == null) {
            main.lobbyInfoText.Set(0, 0.05f, main.TextChooseLobby.text, false, true);
            return;
        }
        //Если выбранное лобби полное
        else if (SteamLobby.Lobby.GetPlayersLimit(LobbyInfo.SelectedLobby) <= SteamLobby.Lobby.GetPlayersCount(LobbyInfo.SelectedLobby)) {
            main.lobbyInfoText.Set(0, 0.05f, main.TextLobbyIsFull.text, false, true);
            return;
        }
        else {
            string passwordLobby = "";
            foreach (SteamLobby.MetaData metaData in LobbyInfo.SelectedLobby.metaDatas) {
                if (metaData.key == SteamLobby.LobbyKeys.password) {
                    passwordLobby = metaData.value;
                    break;
                }
            }
            //если у лобби есть пароль
            if (passwordLobby != "") {
                //Если пароль не введен
                if (main.InputPassword.text == "") {
                    main.lobbyInfoText.Set(0, 0.05f, main.TextInputPassword.text, false, true);
                    return;
                }
                //если введен неправильный пароль
                else if (main.InputPassword.text != passwordLobby) {
                    main.lobbyInfoText.Set(0, 0.05f, main.TextNotCorrectPassword.text, false, true);
                    return;
                }
            }
        }



        string hostAddress = Steamworks.SteamMatchmaking.GetLobbyData(LobbyInfo.SelectedLobby.lobbySID, SteamLobby.LobbyKeys.hostAddres);
        NetworkCTRL.networkManager.networkAddress = hostAddress;
        NetworkCTRL.networkManager.StartClient();
    }
}
