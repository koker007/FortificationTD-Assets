using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class LeaderInThisMap : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        iniTitleLanguage();
    }

    [SerializeField]
    Text LeaderTitleText;
    string LeaderTitleStr;
    TimeText LeaderTitleTime;
    [SerializeField]
    Text LeaderTitleNameText;
    string LeaderTitleNameStr;
    TimeText LeaderTitleNameTime;
    [SerializeField]
    Text LeaderTitleScoreText;
    string LeaderTitleScoreStr;
    TimeText LeaderTitleScoreTime;

    [Header("Language Key")]
    [SerializeField]
    string KeyLeaderTitleText = "";
    [SerializeField]
    string KeyLeaderName = "";
    [SerializeField]
    string KeyLeaderScore = "";

    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Header("Steamleaders")]

    //лидеры текущей карты
    bool LeaderboardSeedNeedIni = false; //Нужна ли Общая инициализация

    bool getterLeaderboard = false;
    LeaderboardFindResult_t Steamleaderboard;
    private CallResult<LeaderboardFindResult_t> m_LeaderboardScoresDownloaded;
    private void IniSteamLeaderboard(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard == 0 || bIOFailure)
        {
            Debug.Log("SteamLeaderboard_t Error");
        }
        else
        {
            Debug.Log("SteamLeaderboard_t OK");
            Steamleaderboard = pCallback;
            getterLeaderboard = true;
        }
    }
    string KeyLeaderBoard = "";


    //Загруженная таблица с лидерами на карте
    public bool getterLeaderboardDownloadTOP = false;
    public LeaderboardScoresDownloaded_t SteamleaderboardScoresDownloaded_TOP;
    private CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded_TOP;
    private void IniSteamLeaderboardScoresDownloadedTOP(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard == 0 || bIOFailure)
        {
            Debug.Log("SteamLeaderboard_t Error");
        }
        else
        {
            Debug.Log("SteamLeaderboard_t OK");
            SteamleaderboardScoresDownloaded_TOP = pCallback;
            getterLeaderboardDownloadTOP = true;
            LeaderStartView = Time.unscaledTime;
        }
    }

    //Загружены ли очки в таблицу лидеров - загрузка очков
    public bool uploadScore = false;
    public bool reWriteTop = false;
    public LeaderboardScoreUploaded_t SteamLeaderboardScoreUploaded;
    private CallResult<LeaderboardScoreUploaded_t> m_LeaderboardScoreUploaded_t;
    private void IniLeaderboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_bSuccess == 0 || bIOFailure)
        {
            Debug.Log("SteamLeaderboardScoreUploaded Error");
        }
        else
        {
            Debug.Log("SteamLeaderboardScoreUploaded OK");
            SteamLeaderboardScoreUploaded = pCallback;
            uploadScore = true;
            reWriteTop = false;
        }
    }


    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    bool iniOk = false;
    void iniTitleLanguage() {
        if (Setings.main && Setings.main.LangugeText != null && !iniOk) {
            LeaderTitleStr = Setings.main.LangugeText.get_text_from_key(KeyLeaderTitleText);
            LeaderTitleNameStr = Setings.main.LangugeText.get_text_from_key(KeyLeaderName);
            LeaderTitleScoreStr = Setings.main.LangugeText.get_text_from_key(KeyLeaderScore);

            if (LeaderTitleStr != "")
                iniOk = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        iniTitleLanguage();
        testTitleText();

        iniLeadersBoard();
        CalcLiaderList();
    }

    public struct TimeText {
        const string randomText = "ЙФЯЦЫЧУВСКАМЕПИНРТГОЬШЛБЩДЮЗЖQAZWSXEDCRFVTGBYHNUJMIKOLP";
        const string randonNum = "0123456789IV-+";

        float timeStart;
        float speedSymbol;

        bool isNumeric;
        public void SetStartTime(float timestartFunc, float speed, bool numeric)
        {
            timeStart = timestartFunc + Time.unscaledTime;
            speedSymbol = speed;
            isNumeric = numeric;
        }

        public string calc(string textFunc) {
            string text = "";

            if (timeStart < Time.unscaledTime && textFunc != null)
            {
                string nameNow = "";
                foreach (char s in textFunc)
                {
                    if (nameNow.Length < (Time.unscaledTime - timeStart) / speedSymbol)
                    {
                        nameNow += s;
                    }
                }

                    if (!isNumeric)
                        nameNow += randomText[UnityEngine.Random.Range(0, randomText.Length)];
                    else nameNow += randonNum[UnityEngine.Random.Range(0, randonNum.Length)];

                if (nameNow.Length > textFunc.Length)
                {
                    nameNow = textFunc;
                }
                text = nameNow;

            }
            else
            {
                text = "";
            }

            return text;
        }
    }

    public void restart()
    {
        LeaderTitleTime.SetStartTime(1, 0.1f, false);
        LeaderTitleNameTime.SetStartTime(1.25f, 0.1f, false);
        LeaderTitleScoreTime.SetStartTime(1.5f, 0.1f, false);

        getterLeaderboard = false;
        uploadScore = false;
        getterLeaderboardDownloadTOP = false;

        LeaderboardSeedNeedIni = false;

        ClearLiaderList();

        NeedSaveSeedPopular = true;
        NeedSaveSeedVerified = true;

        Debug.Log("Restart LeadersTab");
    }

    void testTitleText() {
        LeaderTitleText.text = LeaderTitleTime.calc(LeaderTitleStr);
        LeaderTitleNameText.text = LeaderTitleNameTime.calc(LeaderTitleNameStr);
        LeaderTitleScoreText.text = LeaderTitleScoreTime.calc(LeaderTitleScoreStr);
    }

    float LeaderTimeOld = 0;
    void iniLeadersBoard() {
        if (SteamManager.Initialized && Time.unscaledTime-LeaderTimeOld > 1 && GameplayCTRL.main != null && GameplayCTRL.main.KeyGen != "") {
            LeaderTimeOld = Time.unscaledTime;

            //Если таблица не проинициализирована
            if (!LeaderboardSeedNeedIni)
            {
                //Загрузить таблицу
                m_LeaderboardScoresDownloaded = CallResult<LeaderboardFindResult_t>.Create(IniSteamLeaderboard);
                //Загрузить 2 ступень таблицы
                m_LeaderboardScoresDownloaded_TOP = CallResult<LeaderboardScoresDownloaded_t>.Create(IniSteamLeaderboardScoresDownloadedTOP);
                //Загрузить свой результат в таблицу
                m_LeaderboardScoreUploaded_t = CallResult<LeaderboardScoreUploaded_t>.Create(IniLeaderboardScoreUploaded);


                LeaderboardSeedNeedIni = true;
            }

            //Сперва пытаемся загрузить таблицу
            if (!getterLeaderboard)
            {

                KeyLeaderBoard = "SeedKill:" + GameplayCTRL.main.KeyGen;
                SteamAPICall_t hedder = SteamUserStats.FindOrCreateLeaderboard(KeyLeaderBoard, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
                m_LeaderboardScoresDownloaded.Set(hedder);
            }
            else if (getterLeaderboard && !uploadScore) {
                SteamLeaderboard_t steamLeaderboard_T;
                steamLeaderboard_T.m_SteamLeaderboard = Steamleaderboard.m_hSteamLeaderboard.m_SteamLeaderboard;
                int[] testArray = new int[5];
                SteamAPICall_t handle;
                if (!reWriteTop)
                {
                    handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_T, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, GameplayCTRL.main.kills, testArray, 5);
                }
                else
                {
                    handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_T, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, GameplayCTRL.main.kills, testArray, 5);
                }
                m_LeaderboardScoreUploaded_t.Set(handle);
            }
            //Теперь, если таблица загружена то грузим список лидеров
            else if (getterLeaderboard && !getterLeaderboardDownloadTOP) {
                Debug.Log("getterLeaderboardDownload_TOP test");
                SteamLeaderboard_t steamLeaderboard_T;
                steamLeaderboard_T.m_SteamLeaderboard = Steamleaderboard.m_hSteamLeaderboard.m_SteamLeaderboard;

                SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(steamLeaderboard_T, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, 99);
                m_LeaderboardScoresDownloaded_TOP.Set(handle);
            }
        }
    }

    float LeaderStartView = 0;
    List<LeaderInfo> ListLeaders = new List<LeaderInfo>();
    [SerializeField]
    GameObject LeaderInfoPrefab;
    [SerializeField]
    Transform content;


    bool NeedSaveSeedVerified = true;
    bool NeedSaveSeedPopular = true;
    void CalcLiaderList() {
        //Если таблица получена и лидеров добавлено меньше чем их в таблице
        if (getterLeaderboardDownloadTOP && SteamleaderboardScoresDownloaded_TOP.m_cEntryCount > ListLeaders.Count && ListLeaders.Count/0.1f < Time.unscaledTime - LeaderStartView) {
            //вытаскиваем лидера
            LeaderboardEntry_t LeaderData;
            int[] details = new int[1];
            int detailsMax = 0;
            SteamUserStats.GetDownloadedLeaderboardEntry(SteamleaderboardScoresDownloaded_TOP.m_hSteamLeaderboardEntries, ListLeaders.Count, out LeaderData, details, detailsMax);
            
            if (LeaderData.m_steamIDUser.m_SteamID != 0) {
                GameObject LeaderInfoObj = Instantiate(LeaderInfoPrefab, content);
                if (!LeaderInfoObj) return;

                RectTransform rectTransform = LeaderInfoObj.GetComponent<RectTransform>();
                rectTransform.pivot = new Vector2(0, ListLeaders.Count + 1);

                LeaderInfo leaderInfo = LeaderInfoObj.GetComponent<LeaderInfo>();
                leaderInfo.setSteamID(LeaderData, LeaderData.m_nScore);
                ListLeaders.Add(leaderInfo);

                RectTransform RectContent = content.GetComponent<RectTransform>();
                RectContent.sizeDelta = new Vector2(RectContent.sizeDelta.x,  rectTransform.sizeDelta.y * ListLeaders.Count);
            }
        }

        TestSaveSeed();

        void TestSaveSeed() {
            if (GameplayCTRL.main.BaceHealth < 90) {
                int minutesFrom2020 = (int)(TimeFromNET.GetSecondsFrom2020(TimeFromNET.GetFastestNISTDate()) / 60);

                if (NeedSaveSeedVerified && GlobalLeaderBoardCTRL.main.SeedsVerified != null) {
                    NeedSaveSeedVerified = false;
                    int[] seedInt = Calc.Convert.ToIntArray(GameplayCTRL.main.KeyGen);
                    if (!FoundThisSeed(GlobalLeaderBoardCTRL.main.SeedsVerified, seedInt)) {
                        GlobalLeaderBoardCTRL.main.SeedsVerified.ReUploadMyScore(false, minutesFrom2020, seedInt);
                    }
                }
                if (NeedSaveSeedPopular && ListLeaders.Count > 2 && GlobalLeaderBoardCTRL.main.SeedsPopular != null) {
                    NeedSaveSeedPopular = false;
                    int[] seedInt = Calc.Convert.ToIntArray(GameplayCTRL.main.KeyGen);
                    GlobalLeaderBoardCTRL.main.SeedsPopular.ReUploadMyScore(false, minutesFrom2020, seedInt);

                }

                bool FoundThisSeed(SteamLeaderboard.Leaderboard leaderboard, int[] data) {
                    bool Found = false;
                    //Перебираем всех в поисках такой же карты
                    foreach (SteamLeaderboard.LeaderData leaderData in leaderboard.ListUsers) {
                        if (leaderData.details.Length == data.Length) {
                            bool Correct = true;
                            for (int num = 0; num < data.Length; num++) {
                                if (leaderData.details[num] != data[num]) {
                                    Correct = false;
                                }
                            }

                            if (Correct) {
                                Found = true;
                                break;
                            }
                        }
                    }
                    return Found;
                }
            }
        }
    }
    void ClearLiaderList() {
        if (ListLeaders != null)
        {
            foreach (LeaderInfo leaderInfo in ListLeaders)
            {
                if (leaderInfo && leaderInfo.gameObject)
                {
                    Destroy(leaderInfo.gameObject);
                }
            }
            ListLeaders.Clear();
        }
    }
}
