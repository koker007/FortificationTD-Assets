using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayPlayersInfoListCTRL : MonoBehaviour
{
    
    [SerializeField]
    GameObject PrefabPlayerIndicatorGPInfo;
    [SerializeField]
    RectTransform FirstPosition;

    [SerializeField]
    GameObject Content;

    List<GameplayUIInfoPanel> PlayersInfoList = new List<GameplayUIInfoPanel>();

    [SerializeField]
    GameplayCTRL gameplayCTRL;
    void iniGamePlayCTRL() {
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        iniGamePlayCTRL();
        iniPlayers();
    }

    void iniPlayers() {
            if (gameplayCTRL != null && Content != null && PrefabPlayerIndicatorGPInfo != null && FirstPosition != null) {
            foreach (Player player in gameplayCTRL.players) {
                //Ищем этого игрока в списке таб
                bool found = false;
                foreach (GameplayUIInfoPanel infoPanel in PlayersInfoList) {
                    if (infoPanel.MePlayer == player) {
                        found = true;
                        break;
                    }
                }

                //Если панель игрока не найдена, создаем ее
                if (!found) {
                    GameObject newPanelObj = Instantiate(PrefabPlayerIndicatorGPInfo, Content.transform);
                    GameplayUIInfoPanel newPanel = null;
                    RectTransform rectTransform = null;

                    if (newPanelObj)
                    {
                        newPanel = newPanelObj.GetComponent<GameplayUIInfoPanel>();
                        rectTransform = newPanelObj.GetComponent<RectTransform>();
                    }
                    //Заполняем индикатор
                    if (newPanel != null)
                    {
                        newPanel.MePlayer = player;
                        //Перемещаем индикатор
                        rectTransform.position = new Vector3(rectTransform.position.x, FirstPosition.position.y, rectTransform.position.z);
                        rectTransform.localScale = new Vector3(1, 1, 1);
                        rectTransform.pivot = new Vector2(rectTransform.pivot.x, 1 + PlayersInfoList.Count);
                        PlayersInfoList.Add(newPanel);
                    }
                    //иначе удаляем
                    else {
                        Destroy(newPanelObj);
                    }
                }
            }
        }
    }
}
