using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class LeaderInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        testChange();
        TestText();
    }

    [SerializeField]
    RawImage Icon;
    Texture2D IconTexture2D;

    [SerializeField]
    Text NameText;
    LeaderInThisMap.TimeText NameTime;
    string NameNeed;

    [SerializeField]
    Text ScoreText;
    LeaderInThisMap.TimeText ScoreTime;
    int ScoreNeed;

    [SerializeField]
    Image ObvodkaOpen;

    float StartTime = 0;
    LeaderboardEntry_t cSteamID;

    float lastChange = 0;
    void testChange() {
        if (Time.unscaledTime - lastChange > 5) {
            lastChange = Time.unscaledTime;

            NameNeed = SteamFriends.GetFriendPersonaName(cSteamID.m_steamIDUser);

            if (Icon) {
                setAvatar();
            }
            void setAvatar()
            {
                uint width = 0;
                uint height = 0;
                int avatarInt = SteamFriends.GetSmallFriendAvatar(cSteamID.m_steamIDUser);


                if (avatarInt > 0)
                {
                    SteamUtils.GetImageSize(avatarInt, out width, out height);
                }

                if (width > 0 && height > 0)
                {
                    byte[] avatarStream = new byte[4 * (int)width * (int)height];
                    SteamUtils.GetImageRGBA(avatarInt, avatarStream, 4 * (int)width * (int)height);

                    IconTexture2D = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                    IconTexture2D.LoadRawTextureData(avatarStream);
                    IconTexture2D.Apply();

                    Icon.texture = IconTexture2D;
                    Icon.rectTransform.localScale = new Vector2(1, -1);
                }
                else
                {
                    Debug.Log("Get Steam LeaderInfo AvatarInt Error");
                }
            }
        }
    }
    public void setSteamID(LeaderboardEntry_t steamID, int score) {
        StartTime = Time.unscaledTime;
        cSteamID = steamID;
        //Получаем имя
        NameNeed = SteamFriends.GetFriendPersonaName(steamID.m_steamIDUser);
        ScoreNeed = score;

        NameTime.SetStartTime(0, 0.05f, false);
        ScoreTime.SetStartTime(0.5f, 0.05f, true);

    }

    public void TestText() {

        if(NameText != null)
            NameText.text = NameTime.calc(NameNeed);
        if(ScoreText != null)
            ScoreText.text = ScoreTime.calc(System.Convert.ToString(ScoreNeed));

        if (ObvodkaOpen) {
            ObvodkaOpen.pixelsPerUnitMultiplier += (1.1f - ObvodkaOpen.pixelsPerUnitMultiplier) * 0.1f;
            ObvodkaOpen.SetAllDirty();

            if (Time.unscaledTime - StartTime > 2)
                Destroy(ObvodkaOpen);
        }
    }
}
