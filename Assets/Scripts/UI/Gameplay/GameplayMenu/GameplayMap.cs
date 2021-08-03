using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameplayMap : MonoBehaviour
{

    [SerializeField]
    RawImage map;
    [SerializeField]
    RawImage mapBace;
    [SerializeField]
    RawImage EnemyMap;
    [SerializeField]
    RawImage BuildMap;
    [SerializeField]
    string KeyGenOld = "";

    void iniMap() {
        if (map == null) {
            map = gameObject.GetComponent<RawImage>();
        }
    }

    [SerializeField]
    GameplayCTRL gameplayCTRL;
    int gameplayModeOld = 0;
    void iniGameplayCTRL() {
        GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");

        if(gameplayObj != null)
            gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
    }
    void iniEnemyMap() {
        if (gameplayCTRL != null && gameplayCTRL.EnemyMap != null && EnemyMap != null && EnemyMap.texture == null) {
            EnemyMap.texture = gameplayCTRL.EnemyMap.GetTexture2D();
        }
    }
    void iniBuildMap() {
        if (GameplayCTRL.main && BuildMap != null && BuildMap.texture == null)
        {
            BuildMap.texture = gameplayCTRL.BuildMap.GetTexture2D();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        iniMap();

    }

    // Update is called once per frame
    void Update()
    {
        testGenMap();
        iniGameplayCTRL();

        if (gameplayCTRL != null)
            gameplayModeOld = gameplayCTRL.gamemode;

        iniEnemyMap();
        iniBuildMap();

        testBaceAlpha();
    }

    Texture2D mapTexture;
    Texture2D BaceTexture;
    bool needGen = true;
    void testGenMap() {

        if (gameplayCTRL != null) {
            //проверяем нужна ли генерация
            if (needGen)
            {
                float maximumHeight = 0;
                Vector2 maximumHeightPos = new Vector2();
                if (mapTexture == null)
                {
                    mapTexture = new Texture2D(60, 60);
                }

                map.texture = mapTexture;

                for (int x = 0; x < map.texture.width; x++)
                {
                    for (int y = 0; y < map.texture.height; y++)
                    {

                        Color colorOld = mapTexture.GetPixel(x, y);

                        if (colorOld.a < 1)
                        {

                            Color colorPix = colorOld;

                            float height = Generator.getHeight(gameplayCTRL.KeyGen, x, y, mapTexture.width);
                            //Вода
                            if (height < 0.1f)
                            {
                                colorPix = new Color(0, 0.5f, 0.5f);
                            }
                            //Песок
                            else if (height < 0.2f)
                            {
                                colorPix = new Color(0.9f, 0.8f, 0);
                            }
                            //трава
                            else if (height < 0.3f)
                            {
                                colorPix = new Color(0.4f, 0.6f, 0);
                            }
                            //грязь
                            else if (height < 0.3f)
                            {
                                colorPix = new Color(0.4f, 0.6f, 0);
                            }
                            else if (height < 0.4f)
                            {
                                colorPix = new Color(0.05f, 0.4f, 0);
                            }
                            else if (height < 0.5f)
                            {
                                colorPix = new Color(0.5f, 0.3f, 0);
                            }
                            else if (height < 0.6f)
                            {
                                colorPix = new Color(0.5f, 0.5f, 0.5f);
                            }
                            else
                            {
                                colorPix = new Color(0.5f, 0.5f, 0.5f);
                            }

                            //Красный пиксель
                            //if (x == 0 && y == 0) {
                            //    colorPix = new Color(1.0f, 0f, 0f);
                            //}

                            mapTexture.SetPixel(x, y, colorPix);

                            if (maximumHeight < height)
                            {
                                maximumHeight = height;
                                if (maximumHeight > 8) maximumHeight = 8;

                                maximumHeightPos = new Vector2(x, y);
                            }
                        }

                        if (x + 1 == map.texture.width && y + 1 == map.texture.height)
                        {
                            needGen = false;
                        }

                    }
                }

                mapTexture.filterMode = FilterMode.Point;
                mapTexture.Apply();

                if (mapBace)
                {
                    //Изменение текстуры с пикселем
                    if (BaceTexture == null)
                    {
                        BaceTexture = new Texture2D(60, 60);
                    }
                    mapBace.texture = BaceTexture;
                    for (int x = 0; x < map.texture.width; x++)
                    {
                        for (int y = 0; y < map.texture.height; y++)
                        {
                            Color colorNew = new Color(1, 1, 1, 0);

                            if (x == maximumHeightPos.x && y == maximumHeightPos.y)
                            {
                                colorNew = new Color(1, 0, 0, 1);
                            }

                            BaceTexture.SetPixel(x, y, colorNew);
                        }
                    }
                    BaceTexture.filterMode = FilterMode.Point;
                    BaceTexture.Apply();
                }
            }

            //Если изменилось слово ключ
            if (gameplayCTRL.KeyGen != KeyGenOld || gameplayModeOld != gameplayCTRL.gamemode) {
                needGen = true;
                KeyGenOld = gameplayCTRL.KeyGen;

                //сброс всей картинки
                if (mapTexture == null)
                {
                    mapTexture = new Texture2D(60, 60);
                }

                map.texture = mapTexture;

                for (int x = 0; x < map.texture.width; x++)
                {
                    for (int y = 0; y < map.texture.height; y++)
                    {
                        mapTexture.SetPixel(x, y, new Color(0,0,0,0.5f));
                    }
                }

                mapTexture.filterMode = FilterMode.Point;
                mapTexture.Apply();
            }
        }
    }

    Vector2 testPixOld = new Vector2();
    void testMap() {
        if (GameplayCTRL.main) {
            
        }
    }

    bool baceAlphaPlus = true;
    void testBaceAlpha() {
        if (mapBace != null) {
            Color colorNew = mapBace.color;
            if (baceAlphaPlus){
                colorNew.a += Time.unscaledDeltaTime * 4;
            }
            else {
                colorNew.a -= Time.unscaledDeltaTime * 4;
            }

            if (colorNew.a >= 1)
            {
                colorNew.a = 1;
                baceAlphaPlus = false;
            }
            else if(colorNew.a <= 0) {
                colorNew.a = 0;
                baceAlphaPlus = true;
            }

            mapBace.color = colorNew;
        }
    }
}
