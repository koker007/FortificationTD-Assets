using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCTRL : MonoBehaviour
{

    Player playerMe;
    void iniPlayer() {
        if (playerMe == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                GameplayCTRL gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
                if (gameplayCTRL != null) {
                    foreach (Player player in gameplayCTRL.players) {
                        if (player.isLocalPlayer) {
                            playerMe = player;
                        }
                    }
                }
            }
        }
    }

    [SerializeField]
    Text money;
    TimeText moneyTime;

    [SerializeField]
    Text moneyInMinuts;
    TimeText moneyMinutTime;

    float moneyOld = -100;
    float healthBaceOld = -100;

    void testMoney() {
        if (money != null && moneyInMinuts != null && playerMe != null && GameplayCTRL.main != null) {
            if (playerMe.Money != moneyOld || healthBaceOld != GameplayCTRL.main.BaceHealth) {
                moneyTime.Set(0, 0.1f, Convert.ToString((int)playerMe.Money), true, true);
                moneyOld = playerMe.Money;
                healthBaceOld = GameplayCTRL.main.BaceHealth;

                moneyMinutTime.Set(0, 0.05f, "+" + Convert.ToString((int)playerMe.moneyTime), true, true);
            }

            money.text = moneyTime.Get();
            moneyInMinuts.text = moneyMinutTime.Get();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("iniPlayer", 1, UnityEngine.Random.RandomRange(4f, 9f));
    }

    // Update is called once per frame
    void Update()
    {
        testMoney();
    }
}
