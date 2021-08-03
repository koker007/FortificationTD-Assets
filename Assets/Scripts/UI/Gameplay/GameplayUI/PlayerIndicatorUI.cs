using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class PlayerIndicatorUI : MonoBehaviour
{
    [SerializeField]
    GameObject IndicatorOffOn;

    [SerializeField]
    Image[] ColorImage;

    [SerializeField]
    RawImage PlayerIconSteam;
    [SerializeField]
    Texture2D downloadedAvatar;

    [SerializeField]
    Text PlayerName;
    [SerializeField]
    Text PlayerResurce;

    [SerializeField]
    public Player playerMe;
    PlayerHand playerHandMe;
    Camera CamMain;

    // Start is called before the first frame update
    void Start()
    {
        iniPlayerInfo();
    }

    // Update is called once per frame
    void Update()
    {
        testIndicator();
    }

    public void iniPlayerInfo() {
        if (playerMe != null) {
            if (playerHandMe == null)
                playerHandMe = playerMe.MyCursorHand.GetComponent<PlayerHand>();
            if (CamMain == null) {
                GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
                if (camObj != null)
                    CamMain = camObj.GetComponent<Camera>();
            }

            //Меняем цвет иконки
            for (int num = 0; num < ColorImage.Length; num++) {
                if(ColorImage[num] != null)
                    ColorImage[num].color = new Color(0.7f+playerMe.colorVec.x*0.3f, 0.7f+playerMe.colorVec.y*0.3f, 0.7f+playerMe.colorVec.z*0.3f);
            }

            //Меняем ник
            if (PlayerName != null) {
                PlayerName.text = playerMe.Name;
            }
        }
    }

    float timeToReAvatar = 0.1f;
    void testIndicator() {
        if (playerMe != null) {
            testOffOn();
            testTransform();
            testMoney();
            setAvatar();

            void testOffOn() {
                if (IndicatorOffOn != null && playerHandMe != null) {
                    if (IndicatorOffOn.active != playerHandMe.HandRender.gameObject.active)
                        IndicatorOffOn.SetActive(playerHandMe.HandRender.gameObject.active);
                }
            }

            //Перемещение индикатора над указателем игрока
            void testTransform() {
                if (CamMain != null && playerHandMe != null && playerHandMe.PosForIndicator != null && IndicatorOffOn != null && IndicatorOffOn.active) {
                    //Нужно найти положение точки

                    //Преобразуем положение мирового пространства в положение в пикселях на плоском экране
                    Vector3 rectPos = CamMain.WorldToScreenPoint(playerHandMe.PosForIndicator.position);

                    RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                    rectTransform.position = rectPos;

                    //Размер
                    float distToCam = Vector3.Distance(CamMain.transform.position, playerHandMe.PosForIndicator.position);
                    //Считаем размер
                    float size = 1 / (distToCam/3);
                    if (size > 1)
                        size = 1;
                    else if (size < 0.2f)
                        size = 0.2f;

                    gameObject.transform.localScale = new Vector3(size, size, size);
                }
            }

            void testMoney()
            {
                //корректируем количество денег
                if (PlayerResurce != null)
                {
                    PlayerResurce.text = System.Convert.ToString((int)playerMe.Money);
                }
            }

            void setAvatar()
            {
                timeToReAvatar -= Time.unscaledDeltaTime;
                if (timeToReAvatar < 0)
                {
                    timeToReAvatar = Random.Range(10f,30f);

                    uint width = 0;
                    uint height = 0;
                    CSteamID cSteamID = new CSteamID(playerMe.SteamID);
                    int avatarInt = SteamFriends.GetLargeFriendAvatar(cSteamID);


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

                        PlayerIconSteam.texture = downloadedAvatar;
                        PlayerIconSteam.rectTransform.localScale = new Vector2(1, -1);
                    }
                    else
                    {
                        Debug.Log("Get Steam AvatarINT Error");
                        //needReLoad = true;
                    }
                }
            }
        }
    }
}
