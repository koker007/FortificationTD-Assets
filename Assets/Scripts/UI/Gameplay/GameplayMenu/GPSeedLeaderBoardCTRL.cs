using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class GPSeedLeaderBoardCTRL : MonoBehaviour
{

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

    public struct TimeText
    {
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

        public string calc(string textFunc)
        {
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

    // Start is called before the first frame update
    void Start()
    {
        iniTitleLanguage();
    }

    // Update is called once per frame
    void Update()
    {
        iniTitleLanguage();
        testTitleText();

        CalcLiaderList();
    }

    bool iniOk = false;
    void iniTitleLanguage()
    {
        if (Setings.main && Setings.main.LangugeText != null && !iniOk)
        {
            LeaderTitleStr = Setings.main.LangugeText.get_text_from_key(KeyLeaderTitleText);
            LeaderTitleNameStr = Setings.main.LangugeText.get_text_from_key(KeyLeaderName);
            LeaderTitleScoreStr = Setings.main.LangugeText.get_text_from_key(KeyLeaderScore);

            if (LeaderTitleStr != "")
                iniOk = true;
        }
    }

    void testTitleText()
    {
        LeaderTitleText.text = LeaderTitleTime.calc(LeaderTitleStr);
        LeaderTitleNameText.text = LeaderTitleNameTime.calc(LeaderTitleNameStr);
        LeaderTitleScoreText.text = LeaderTitleScoreTime.calc(LeaderTitleScoreStr);
    }

    float LeaderStartView = 0;
    List<LeaderInfo> ListLeaders = new List<LeaderInfo>();
    [SerializeField]
    GameObject LeaderInfoPrefab;
    [SerializeField]
    Transform content;

    string keyLeaderboardOld = "";
    void CalcLiaderList()
    {
        if (GlobalLeaderBoardCTRL.main.ThisSeedMap != null) {
            if (keyLeaderboardOld != GlobalLeaderBoardCTRL.main.ThisSeedMap.Key) {
                ClearLiaderList();
                keyLeaderboardOld = GlobalLeaderBoardCTRL.main.ThisSeedMap.Key;
            }

            //Если таблица получена и лидеров добавлено меньше чем их в таблице
            if (GlobalLeaderBoardCTRL.main.ThisSeedMap.ListUsers.Count > ListLeaders.Count && ListLeaders.Count / 0.1f < Time.unscaledTime - LeaderStartView)
            {
                LeaderboardEntry_t LeaderData = GlobalLeaderBoardCTRL.main.ThisSeedMap.ListUsers[ListLeaders.Count].data;
                if (LeaderData.m_steamIDUser.m_SteamID != 0)
                {
                    GameObject LeaderInfoObj = Instantiate(LeaderInfoPrefab, content);
                    if (!LeaderInfoObj) return;

                    RectTransform rectTransform = LeaderInfoObj.GetComponent<RectTransform>();
                    rectTransform.pivot = new Vector2(0, ListLeaders.Count + 1);

                    LeaderInfo leaderInfo = LeaderInfoObj.GetComponent<LeaderInfo>();
                    leaderInfo.setSteamID(LeaderData, LeaderData.m_nScore);

                    ListLeaders.Add(leaderInfo);

                    RectTransform RectContent = content.GetComponent<RectTransform>();
                    RectContent.sizeDelta = new Vector2(RectContent.sizeDelta.x, rectTransform.sizeDelta.y * ListLeaders.Count);
                }
            }
        }
    }

    void ClearLiaderList()
    {
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
    public void restart()
    {
        LeaderTitleTime.SetStartTime(1, 0.1f, false);
        LeaderTitleNameTime.SetStartTime(1.25f, 0.1f, false);
        LeaderTitleScoreTime.SetStartTime(1.5f, 0.1f, false);

        ClearLiaderList();

        Debug.Log("Restart LeadersTab");
    }
}
