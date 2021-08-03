using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    static SteamLobby main;

    public static class LobbyKeys {
        public const string name = "keyName";
        public const string hostAddres = "keyHostAddres";
        public const string password = "keyPassword";
        public const string seed = "keySeed";
        public const string mode = "keyGameMode";
        public const string timeIsPlay = "keyGameTime";
    }
    public struct MetaData {
        public string key;
        public string value;
    }

    public struct Members {
        public CSteamID cSteamID;
        public MetaData[] Data;
    }

    public class Lobby {
        public CSteamID lobbySID;
        public CSteamID ownerSID;
        public Members[] members;
        public int membersLimit;
        public MetaData[] metaDatas = new MetaData[10];

        public Lobby() {
            metaDatas[0].key = LobbyKeys.name;
            metaDatas[1].key = LobbyKeys.hostAddres;
            metaDatas[2].key = LobbyKeys.seed;
            metaDatas[3].key = LobbyKeys.mode;
            metaDatas[4].key = LobbyKeys.timeIsPlay;
            metaDatas[5].key = LobbyKeys.password;
        }
        public static Lobby SetParametrs(CSteamID lobbySID)
        {
            Lobby lobby = null;
            if (lobbySID != null) {
                lobby = new Lobby();

                lobby.lobbySID = lobbySID;
                lobby.ownerSID = SteamMatchmaking.GetLobbyOwner(lobbySID);
                lobby.membersLimit = SteamMatchmaking.GetLobbyMemberLimit(lobbySID);

                int membersFound = SteamMatchmaking.GetNumLobbyMembers(lobbySID);

                //получаем пользователей лобби только если сами в нем находимся
                if (lobbyNow != null && lobbyNow.lobbySID != null && lobby.lobbySID == lobbyNow.lobbySID)
                {
                    lobby.members = new Members[membersFound];
                    for (int numMember = 0; numMember < membersFound; numMember++)
                    {
                        lobby.members[numMember].cSteamID = SteamMatchmaking.GetLobbyMemberByIndex(lobbySID, numMember);
                    }
                }

                //добавляем данные
                for (int num = 0; num < lobby.metaDatas.Length; num++) {
                    if (lobby.metaDatas[num].key == LobbyKeys.name) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData((CSteamID)lobby.lobbySID.m_SteamID, LobbyKeys.name);
                    else if (lobby.metaDatas[num].key == LobbyKeys.mode) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData((CSteamID)lobby.lobbySID.m_SteamID, LobbyKeys.mode);
                    else if (lobby.metaDatas[num].key == LobbyKeys.timeIsPlay) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData((CSteamID)lobby.lobbySID.m_SteamID, LobbyKeys.timeIsPlay);
                    else if (lobby.metaDatas[num].key == LobbyKeys.hostAddres) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData((CSteamID)lobby.lobbySID.m_SteamID, LobbyKeys.hostAddres);
                    else if (lobby.metaDatas[num].key == LobbyKeys.seed) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData((CSteamID)lobby.lobbySID.m_SteamID, LobbyKeys.seed);
                    else if (lobby.metaDatas[num].key == LobbyKeys.password) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData((CSteamID)lobby.lobbySID.m_SteamID, LobbyKeys.password);
                }

            }
            return lobby;
        }
        public static int GetPlayersLimit(Lobby lobby) {
            int result = 0;
            if (lobby != null) {
                result = SteamMatchmaking.GetLobbyMemberLimit(lobby.lobbySID);
            }
            return result;
        }
        public static int GetPlayersCount(Lobby lobby) {
            int result = 0;
            if (lobby != null)
            {
                result = SteamMatchmaking.GetNumLobbyMembers(lobby.lobbySID);
            }
            return result;
        }
    }

    public static List<Lobby> lobbiesList = new List<Lobby>();
    public static int playersMax = 6;

    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> CallbackGameLobbyJoinRequested;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyUpdate;

    protected Callback<LobbyChatMsg_t> Callback_lobbyChatMsg;
    protected Callback<LobbyChatUpdate_t> Callback_lobbyChatUpdate;

    static CSteamID MySteamID; //Мой ID

    public static Lobby lobbyNow = new Lobby(); //Текущее лобби

    //Параметры лобби которые надо установить
    public static Lobby LobbyNeedParam = new Lobby();

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
            Debug.Log("Lobby created -- SUCCESS!");
        else
            Debug.Log("Lobby created -- failure ...");

        //Лоби созданно. заполняем данными
        if (result.m_eResult == EResult.k_EResultOK)
        {
            CSteamID lobbySID = new CSteamID(result.m_ulSteamIDLobby);
            lobbyNow = Lobby.SetParametrs(lobbySID);

            string personalName = SteamFriends.GetPersonaName();
            //SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name", personalName + "'s game");
            //SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, LobbyKeys.name, personalName + "'s game");


        }
    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {
        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    void OnGetLobbyUpdate(LobbyDataUpdate_t result)
    {
        //добавляем лобби в список
        CSteamID cSteamID;
        cSteamID.m_SteamID = result.m_ulSteamIDLobby;
        Lobby lobby = new Lobby();
        lobby.lobbySID = cSteamID;

        //если это лобби в котором я нахожусь
        if (lobbyNow != null && lobbyNow.lobbySID != null && lobbyNow.lobbySID.m_SteamID == result.m_ulSteamIDLobby) {
            CSteamID lobbySID = new CSteamID(result.m_ulSteamIDLobby);
            lobbyNow = Lobby.SetParametrs(lobbySID);
        }

        //Ищем в списке это лобби
        for (int i = 0; i < lobbiesList.Count; i++)
        {
            if (lobbiesList[i] != null && lobbiesList[i].lobbySID.m_SteamID == result.m_ulSteamIDLobby)
            {
                Debug.Log("Lobby " + i + " :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobbiesList[i].lobbySID.m_SteamID, "name"));

                //Заполняем данными это лобби
                lobbiesList[i] = Lobby.SetParametrs(lobbiesList[i].lobbySID);

                return;
            }
        }

        //Заполняем данными это лобби
        Debug.Log("Lobby Add " + " :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobby.lobbySID.m_SteamID, "name"));
        lobby = Lobby.SetParametrs(lobby.lobbySID);
        lobbiesList.Add(lobby);

    }

    void OnLobbyEntered(LobbyEnter_t result)
    {
        //Если интернет работает то значит мы хост, выходим.
        if (NetworkCTRL.networkManager && NetworkCTRL.networkManager.isNetworkActive)
        {
            return;
        }

        CSteamID lobbySID = new CSteamID(result.m_ulSteamIDLobby);
        lobbyNow.lobbySID = lobbySID;

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(result.m_ulSteamIDLobby), LobbyKeys.hostAddres);
        NetworkCTRL.networkManager.networkAddress = hostAddress;
        NetworkCTRL.networkManager.StartClient();

        if (result.m_EChatRoomEnterResponse == 1)
            Debug.Log("Lobby joined!");
        else
            Debug.Log("Failed to join lobby.");
    }

    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t result) {
        
    }

    void OnLobbyChatMsg(LobbyChatMsg_t result) {

    }
    void OnLobbyChatUpdate(LobbyChatUpdate_t result) {

    }

    //Проверка на нужду изменения данных лобби если я владелец
    float timeOwnerOldTest = 0;
    void TestLobbyOwner() {
        //Если я владелец текущего лобби и пришло время для теста
        if (Time.unscaledTime - 10 > timeOwnerOldTest && lobbyNow != null && lobbyNow.ownerSID == MySteamID) {
            timeOwnerOldTest = Time.unscaledTime;
            for(int numNow = 0; numNow < lobbyNow.metaDatas.Length; numNow++) {


                for (int numNeed = 0; numNeed < LobbyNeedParam.metaDatas.Length; numNeed++)
                {
                    //Если ключи совпадают, но значения нет
                    if (lobbyNow.metaDatas[numNow].key == LobbyNeedParam.metaDatas[numNeed].key 
                        && lobbyNow.metaDatas[numNow].value != LobbyNeedParam.metaDatas[numNeed].value 
                        && LobbyNeedParam.metaDatas[numNeed].value != null)
                    {

                        if (lobbyNow.metaDatas[numNow].key == LobbyKeys.name) SteamMatchmaking.SetLobbyData(lobbyNow.lobbySID, LobbyKeys.name, LobbyNeedParam.metaDatas[numNeed].value);
                        else if (lobbyNow.metaDatas[numNow].key == LobbyKeys.hostAddres) SteamMatchmaking.SetLobbyData(lobbyNow.lobbySID, LobbyKeys.hostAddres, LobbyNeedParam.metaDatas[numNeed].value);
                        else if (lobbyNow.metaDatas[numNow].key == LobbyKeys.mode) SteamMatchmaking.SetLobbyData(lobbyNow.lobbySID, LobbyKeys.mode, LobbyNeedParam.metaDatas[numNeed].value);
                        else if (lobbyNow.metaDatas[numNow].key == LobbyKeys.seed) SteamMatchmaking.SetLobbyData(lobbyNow.lobbySID, LobbyKeys.seed, LobbyNeedParam.metaDatas[numNeed].value);
                        else if (lobbyNow.metaDatas[numNow].key == LobbyKeys.timeIsPlay) SteamMatchmaking.SetLobbyData(lobbyNow.lobbySID, LobbyKeys.timeIsPlay, LobbyNeedParam.metaDatas[numNeed].value);
                        else if (lobbyNow.metaDatas[numNow].key == LobbyKeys.password) SteamMatchmaking.SetLobbyData(lobbyNow.lobbySID, LobbyKeys.password, LobbyNeedParam.metaDatas[numNeed].value);
                        break;
                    }
                }
            }
        }
    }

    void Start()
    {
        MySteamID = SteamUser.GetSteamID();

        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated); // При создании лобби
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList); // При получении списка лобби
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered); // При входе в лобби
        Callback_lobbyUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyUpdate);  // При обновлении мета-данных лобби
        CallbackGameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested); //При запроссе на вход в лобби

        Callback_lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsg); // При получении сообщения в лобби
        Callback_lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate); // При изменении списка игроков в лобби (когда какой-либо игрок входит в лобби или выходит)

        if (SteamAPI.Init())
            Debug.Log("Steam API init -- SUCCESS!");
        else
            Debug.Log("Steam API init -- failure ...");
    }

    void Update()
    {
        main = this;
        MyIP.TestMyServerInfo();

        SteamAPI.RunCallbacks();

        testing();

        //Если я владелец лобби
        TestLobbyOwner();
    }

    void testing() {
        if (Input.GetKey(KeyCode.Minus) && Input.GetKey(KeyCode.Alpha1)) {
            // Command - Create new lobby
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Trying to create lobby ...");
                SteamAPICall_t try_toHost = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 8);
            }

            // Command - List lobbies
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("Trying to get list of available lobbies ...");
                SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
            }

            // Command - Join lobby at index 0 (testing purposes)
            if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("Trying to join FIRST listed lobby ...");
                SteamAPICall_t try_joinLobby = SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
            }

            // Command - List lobby members
            if (Input.GetKeyDown(KeyCode.Q))
            {
                int numPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyNow.lobbySID);

                Debug.Log("\t Number of players currently in lobby : " + numPlayers);
                for (int i = 0; i < numPlayers; i++)
                {
                    Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex(lobbyNow.lobbySID, i)));
                }
            }
        }
    }

    /// <summary>
    /// поиск: шаг 1 выбрать тип обновления списка
    /// </summary>
    public static void SetLobbyListDistance(ELobbyDistanceFilter eLobbyDistanceFilter) {
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(eLobbyDistanceFilter);
    }

    /// <summary>
    /// поиск: шаг 2 Обновить список
    /// </summary>
    public static void GetNewLobbyList() {
        SteamAPICall_t steamAPICall = SteamMatchmaking.RequestLobbyList();
        lobbiesList = new List<Lobby>();
    }

    /// <summary>
    /// сервер: шаг 1
    /// </summary>
    public static void CreateLobby(ELobbyType eLobbyType, int playersMax) {
        SteamMatchmaking.CreateLobby(eLobbyType, playersMax);
    }

    /// <summary>
    /// сервер: шаг 2 установить могут ли другие игроки подключаться к этому лобби
    /// </summary>
    public static void SetLobbyJoinable() {
        
    }

    /// <summary>
    /// сервер: шаг 3 заполнение данными
    /// </summary>
    public static void SetDataMyLobby(string key, string value) {
        //Если лобби запущено и я владелец
        if (lobbyNow.lobbySID.m_SteamID != 0 && SteamMatchmaking.GetLobbyOwner(lobbyNow.lobbySID) == MySteamID) {
            SteamMatchmaking.SetLobbyData(lobbyNow.lobbySID, key, value);
        }
    }

    public static void JoinLobby(CSteamID lobbySID) {
        SteamMatchmaking.JoinLobby(lobbySID);
    }
    public static void LeaveLobbyNow() {
        SteamMatchmaking.LeaveLobby(lobbyNow.lobbySID);
        lobbyNow = new Lobby();
    }

    public static class MyIP
    {
        public static string IPstr = "";
        public static int[] IpInt;
        public static bool[] trying = new bool[4];
        public static string[] addresForTest = {
            "http://ipecho.net/plain",
            "https://api.ipify.org",
            "http://ipinfo.io/ip",
            "http://icanhazip.com"
        };
        public static void TestMyServerInfo()
        {
            if (trying == null) {
                trying = new bool[addresForTest.Length];
                for (int num = 0; num < trying.Length; num++) {
                    trying[num] = false;
                }
            }

            if (IPstr.Length < 7)
            {
                for (int num = 0; num < addresForTest.Length; num++) {
                    if (!trying[num]) {
                        trying[num] = true;
                        System.Net.IPAddress addr = GetMyIPAddress(num);
                        IPstr = addr.ToString();
                        Debug.Log(addr.ToString());
                        break;
                    }
                }
                System.Net.IPAddress GetMyIPAddress(int num) {
                    using (var client = new System.Net.WebClient())
                    {
                        client.Headers.Add(System.Net.HttpRequestHeader.UserAgent, ".NET Application");
                        return System.Net.IPAddress.Parse(client.DownloadString(addresForTest[num]));
                    }
                }
            }
            else if (IpInt == null)
            {
                IpInt = IpStringToInt(IPstr);
            }
        }

        static int[] IpStringToInt(string IPstr)
        {
            int[] IPint = new int[5];

            string numer = "";
            int pos = 0;
            for (int num = 0; num < IPstr.Length; num++)
            {
                if (IPstr[num] != '.')
                    numer += IPstr[num];
                if (IPstr[num] == '.' || num + 1 == IPstr.Length)
                {
                    if (pos < 4)
                        IPint[pos] = System.Convert.ToInt32(numer);
                    numer = "";
                    pos++;
                }
            }
            Debug.Log(IPint[0] + " " + IPint[1] + " " + IPint[2] + " " + IPint[3]);
            return IPint;
        }
    }
}
