using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayUICTRL : MonoBehaviour
{
    public static GamePlayUICTRL main;

    //Контроллер UI в геймплее

    [SerializeField]
    NetworkManager networkManager;
    void iniNetwokManager() {
        if (networkManager == null) {
            GameObject networkObj = GameObject.FindGameObjectWithTag("Network");
            if (networkObj != null) {
                networkManager = networkObj.GetComponent<NetworkManager>();
            }
        }
    }

    [SerializeField]
    GameplayCTRL gameplayCTRL;
    void iniGameplayCTRL()
    {
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    [SerializeField]
    GameObject Connecting;

    [SerializeField]
    GameObject GameplayPlay1;

    [SerializeField]
    GameObject GameplayMainInteface;

    [SerializeField]
    public GameObject Gamemode4EndResult;
    [SerializeField]
    public GameObject Gamemode5BadGen;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        iniNetwokManager();
        iniGameplayCTRL();
        TestMenu();
    }

    void CloseAll() {
        if (Connecting != null)
            Connecting.active = false;
        if (GameplayPlay1 != null)
            GameplayPlay1.active = false;
        if (GameplayMainInteface != null)
            GameplayMainInteface.active = false;
        if (Gamemode4EndResult != null)
            Gamemode4EndResult.active = false;
        if (Gamemode5BadGen != null)
            Gamemode5BadGen.active = false;
    }
    void TestMenu() {
        if (networkManager != null) {
            if (networkManager.isNetworkActive && (gameplayCTRL == null || gameplayCTRL.players.Count == 0)) {
                if (!Connecting.active)
                {
                    Connecting.active = true;
                }
            }
            else if (gameplayCTRL != null) {
                if (gameplayCTRL.players.Count == 0) {
                    if (!Connecting.active) {
                        CloseAll();
                        Connecting.active = true;
                    }
                }
                else
                {
                    if(Connecting.active)
                        Connecting.active = false;

                    if (gameplayCTRL.gamemode == 1)
                    {
                        if (!GameplayPlay1.active)
                        {
                            CloseAll();
                            GameplayPlay1.active = true;
                        }
                    }
                    else if (gameplayCTRL.gamemode == 2)
                    {
                        if (!GameplayMainInteface.active)
                        {
                            CloseAll();
                            GameplayMainInteface.active = true;
                        }
                    }
                    else if (gameplayCTRL.gamemode == 4) {
                        if (!Gamemode4EndResult.active)
                        {
                            CloseAll();
                            Gamemode4EndResult.active = true;
                        }
                    }
                    else if (gameplayCTRL.gamemode == 5) {
                        if (!Gamemode5BadGen.active) {
                            CloseAll();
                            Gamemode5BadGen.active = true;
                        }
                    }
                }
            }
        }
    }
}
