using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Mirror;

public class GameplayUIInfoPanel : MonoBehaviour
{
    [SerializeField]
    RawImage Ping;
    [SerializeField]
    Texture2D[] PingImage;
    [SerializeField]
    Text TextPing;

    [SerializeField]
    RawImage Color;
    [SerializeField]
    RawImage Icon;
    Texture2D AvatarTexture;

    [SerializeField]
    Text Name;

    [SerializeField]
    Text KillAll;
    [SerializeField]
    Text KillPehota;
    [SerializeField]
    Text KillCar;
    [SerializeField]
    Text KillCrab;

    public Player MePlayer;
    public void SetPlayer(Player player) {
        MePlayer = player;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TestLivePlayer();
        TestUpdate();
    }

    void TestLivePlayer() {
        if (MePlayer == null) {
            Destroy(gameObject);
        }
    }

    struct TimeUpdate {
        public float ping;
        public float color;
        public float avatar;
        public float name;

        public TimeUpdate(float x) {
            ping = 9999;
            color = 9999;
            avatar = 9999;
            name = 9999;
        }

        public void PlusTime() {
            ping += Time.unscaledDeltaTime;
            color += Time.unscaledDeltaTime;
            avatar += Time.unscaledDeltaTime;
            name += Time.unscaledDeltaTime;
        }
    }
    TimeUpdate timeUpdate = new TimeUpdate(0);

    NetworkConnection networkConnection;
    void TestUpdate() {
        if (MePlayer != null) {
            timeUpdate.PlusTime();

            TestPing();
            TestColor();
            TestAvatar();
            TestName();
        }

        void TestPing() {
            if (timeUpdate.ping > 1) {
                timeUpdate.ping = 0;

                if (MePlayer.PingRTT > 1000) {
                    Ping.texture = PingImage[0];
                }
                else if (MePlayer.PingRTT > 500)
                {
                    Ping.texture = PingImage[1];
                }
                else if (MePlayer.PingRTT > 250)
                {
                    Ping.texture = PingImage[2];
                }
                else if (MePlayer.PingRTT > 125)
                {
                    Ping.texture = PingImage[3];
                }
                else //if (MePlayer.PingRTT > 60)
                {
                    Ping.texture = PingImage[4];
                }

                if(TextPing)
                    TextPing.text = System.Convert.ToString((int)MePlayer.PingRTT);
            }
        }
        void TestColor() {
            if (timeUpdate.color > 1) {
                timeUpdate.color = 0;

                Color.color = new Color(MePlayer.colorVec.x, MePlayer.colorVec.y, MePlayer.colorVec.z);
            }
        }
        void TestAvatar() {
            if (timeUpdate.avatar > 10) {
                timeUpdate.avatar = 0;

                setAvatar();
            }

            void setAvatar()
            {
                uint width = 0;
                uint height = 0;
                CSteamID cSteamID = new CSteamID(MePlayer.SteamID);
                int avatarInt = SteamFriends.GetLargeFriendAvatar(cSteamID);


                if (avatarInt > 0)
                {
                    SteamUtils.GetImageSize(avatarInt, out width, out height);
                }

                if (width > 0 && height > 0)
                {
                    byte[] avatarStream = new byte[4 * (int)width * (int)height];
                    SteamUtils.GetImageRGBA(avatarInt, avatarStream, 4 * (int)width * (int)height);

                    AvatarTexture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                    AvatarTexture.LoadRawTextureData(avatarStream);
                    AvatarTexture.Apply();

                    Icon.texture = AvatarTexture;
                    Icon.rectTransform.localScale = new Vector2(1, -1);
                }
                else {
                    timeUpdate.avatar += 5;
                }
            }
        }
        void TestName() {
            if (timeUpdate.name > 10) {
                timeUpdate.name = 0;

                Name.text = MePlayer.Name;
            }
        }

        if (KillAll != null && KillPehota != null && KillCrab != null && KillCar != null) {
            KillPehota.text = System.Convert.ToString(MePlayer.KillPehota);
            KillCar.text = System.Convert.ToString(MePlayer.KillCar);
            KillCrab.text = System.Convert.ToString(MePlayer.KillCrab);

            KillAll.text = System.Convert.ToString(MePlayer.KillPehota + MePlayer.KillCar + MePlayer.KillCrab);
        }
    }
}
