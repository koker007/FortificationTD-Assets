using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class SteamPlayerIndicator : MonoBehaviour
{

    [SerializeField]
    RawImage Playerimage;
    [SerializeField]
    Texture2D downloadedAvatar;

    [SerializeField]
    Text PlayerNickname;

    bool needReLoad = false;

    // Use this for initialization
    void Start()
    {
        setNickname();
        setAvatar();
    }

    // Update is called once per frame
    void Update()
    {
        TestReUpdate();
    }

    void setNickname()
    {
        if (PlayerNickname != null)
        {
            PlayerNickname.text = SteamFriends.GetPersonaName();
        }
    }
    void setAvatar()
    {
        uint width = 0;
        uint height = 0;
        int avatarInt = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());


        if (avatarInt > 0)
        {
            SteamUtils.GetImageSize(avatarInt, out width, out height);
        }

        if (width > 0 && height > 0)
        {
            byte[] avatarStream = new byte[4 * (int)width * (int)height];
            SteamUtils.GetImageRGBA(avatarInt, avatarStream, 4 * (int)width * (int)height);

            downloadedAvatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            downloadedAvatar.LoadRawTextureData(avatarStream);
            downloadedAvatar.Apply();

            Playerimage.texture = downloadedAvatar;
            Playerimage.rectTransform.localScale = new Vector2(1, -1);
        }
        else
        {
            Debug.Log("Get Steam AvatarINT Error");
            needReLoad = true;
        }
    }

    void TestReUpdate()
    {
        if (needReLoad)
        {
            needReLoad = false;
            setAvatar();
        }
    }
}
