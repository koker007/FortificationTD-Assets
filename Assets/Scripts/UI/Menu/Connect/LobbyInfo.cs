using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Steamworks;

public class LobbyInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Image Fon;
    [SerializeField]
    Color EnterColor;
    [SerializeField]
    Color SelectColor;

    [SerializeField]
    RawImage Avatar;
    Texture2D downloadedAvatar;

    [SerializeField]
    Text Name;
    [SerializeField]
    Text Mode;
    [SerializeField]
    Text Players;
    [SerializeField]
    Text Password;
    string passwordFactStr;

    static public SteamLobby.Lobby SelectedLobby;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Fon)
        {
            Image[] images = Fon.gameObject.GetComponentsInChildren<Image>();
            Fon.color = EnterColor;

            foreach (Image image in images) {
                image.color = EnterColor;
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Fon)
        {
            Fon.color = new Color(1f, 1f, 1f);

            Image[] images = Fon.gameObject.GetComponentsInChildren<Image>();
            foreach (Image image in images)
            {
                image.color = new Color(1f, 1f, 1f);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    float timeToUpdate = 0;
    // Update is called once per frame
    void Update()
    {
        timeToUpdate -= Time.unscaledDeltaTime;

        if (timeToUpdate < 0) {
            timeToUpdate = 5;

            testName();
            testGameMode();
            testPlayers();
            testPassword();
        }

        testSelect();
    }

    SteamLobby.Lobby lobby;
    public void SetLobby(SteamLobby.Lobby lobby_funk) {
        lobby = lobby_funk;
    }

    void testName() {
        if (Name && lobby != null) {
            string nameStr = "";
            foreach (SteamLobby.MetaData metaData in  lobby.metaDatas) {
                if (metaData.key == SteamLobby.LobbyKeys.name) {
                    nameStr = metaData.value;
                }
            }
            Name.text = nameStr;
        }
    }
    void testGameMode() {
        if (Mode && lobby != null)
        {
            string modeStr = "";
            foreach (SteamLobby.MetaData metaData in lobby.metaDatas)
            {
                if (metaData.key == SteamLobby.LobbyKeys.mode)
                {
                    modeStr = metaData.value;
                }
            }
            Mode.text = modeStr;
        }
    }

    void testPlayers() {
        if (Players && lobby != null)
        {
            string playersStr = "";

            int count = SteamLobby.Lobby.GetPlayersCount(lobby);
            int limit = SteamLobby.Lobby.GetPlayersLimit(lobby);

            Players.text = count + "/" + limit;
        }
    }

    void testPassword() {
        if (Password && lobby != null)
        {
            foreach (SteamLobby.MetaData metaData in lobby.metaDatas)
            {
                if (metaData.key == SteamLobby.LobbyKeys.password)
                {
                    passwordFactStr = metaData.value;
                }
            }

            if (passwordFactStr == null || passwordFactStr == "")
                Password.text = "";
            else Password.text = "X";
        }
    }

    //Проверка выделения сервера
    void testSelect() {
        if (SelectedLobby == lobby) {
            if (Fon)
            {
                Image[] images = Fon.gameObject.GetComponentsInChildren<Image>();
                Fon.color = SelectColor;

                foreach (Image image in images)
                {
                    image.color = SelectColor;
                }
            }
        }
    }

    float timeClickOld = 0;
    public void ClickLobby() {
        if (PanelConnect.main)
        {

            bool doubleClick = false;
            if (Time.unscaledTime - timeClickOld < 0.25f)
                doubleClick = true;

            timeClickOld = Time.unscaledTime;

            //Запоминаем последнее выделенное лобби
            SelectedLobby = lobby;

            /*
            foreach (SteamLobby.MetaData metaData in lobby.metaDatas)
            {
                if (metaData.key != null && metaData.key == SteamLobby.LobbyKeys.hostAddres)
                {
                    InputIP.main.SetIPText(metaData.value);
                    break;
                }
            }

            if (passwordFactStr != null && passwordFactStr != "")
            {
                string IPgetted = "set password";
                InputIP.main.SetIndicatorText(IPgetted, true);

            }
            else
            {
                string IPgetted = "IP is getted";
                InputIP.main.SetIndicatorText(IPgetted, true);
            }
            */

            if (doubleClick)
            {
                PanelConnect.ConnectSelectLobby();
            }
            else {
                PanelConnect.main.lobbyInfoText.Set(0, 0.05f, PanelConnect.main.TextConnect.text, false, true); ;
            }
        }
    }
    string GetIpStr() {
        string IPstr = "";
        foreach (SteamLobby.MetaData metaData in lobby.metaDatas) {
            if (metaData.key == SteamLobby.LobbyKeys.hostAddres) {
                IPstr = metaData.value;
            }
        }
        return IPstr;
    }

}
