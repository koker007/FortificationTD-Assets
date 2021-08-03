using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPMenuPlayersListCTRL : MonoBehaviour
{

    [SerializeField]
    GameObject prefabPlayerInfo;

    [SerializeField]
    GameObject ContentList;

    [SerializeField]
    GameplayCTRL gameplay;

    float timeToGet = 0;
    void iniGamePlay() {
        if (gameplay == null) {
            timeToGet -= Time.unscaledDeltaTime;
            if (timeToGet < 0) {
                timeToGet = 3;
                GameObject GameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
                if (GameplayObj != null) {
                    gameplay = GameplayObj.GetComponent<GameplayCTRL>();
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        iniGamePlay();
        testPlayers();
    }

    float PlayerCountOld = 0;
    void testPlayers() {
        if (gameplay != null) {
            //Получаем игроков
            if (gameplay.players.Count != PlayerCountOld) {
                PlayerCountOld = gameplay.players.Count;

                //Сперва удаляем старые данные
                GPMPlayerInfo[] playersInfoOld = ContentList.GetComponentsInChildren<GPMPlayerInfo>();
                if (playersInfoOld != null && playersInfoOld.Length > 0) {
                    for (int num = 0; num < playersInfoOld.Length; num++)
                    {
                        Destroy(playersInfoOld[num].gameObject);
                    }
                }

                //Добавляем новые
                int numAdd = 0;
                foreach (Player player in gameplay.players) {
                    GameObject playerInfoObj = Instantiate(prefabPlayerInfo, ContentList.transform);

                    //смещаем правильно
                    RectTransform rectTransform = playerInfoObj.GetComponent<RectTransform>();
                    rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, numAdd * -15, rectTransform.localPosition.z);

                    //Запоминаем какому игроку принадлежин инфо
                    GPMPlayerInfo playerInfo = playerInfoObj.GetComponent<GPMPlayerInfo>();
                    playerInfo.player = player;

                    numAdd++;
                }

                //изменяем размер контента
                RectTransform Rect = ContentList.GetComponent<RectTransform>();
                float sizeY = numAdd * 15;
                if (sizeY < 80.01f)
                    sizeY = 80.01f;
                Rect.sizeDelta = new Vector2(Rect.sizeDelta.x, sizeY);
            }
        }
    }
}
