using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPMPlayerInfo : MonoBehaviour
{
    //Контроллер информации об игроке в списке игроков

    public Player player;

    [SerializeField]
    Text playerName;
    [SerializeField]
    Image color;
    [SerializeField]
    Text readyText;
    [SerializeField]
    Text notReadyText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TestNickName();
        TestColor();
        TestReady();
    }

    void TestNickName() {
        if (player != null && playerName != null) {
            playerName.text = player.Name;
        }
    }
    void TestColor() {
        if (player != null && color != null) {
            color.color = new Color(player.colorVec.x, player.colorVec.y, player.colorVec.z);
        }
    }

    void TestReady() {
        if (player != null && readyText != null) {
            if (player.ReadyToStartGame)
            {
                readyText.gameObject.active = true;
                notReadyText.gameObject.active = false;
            }
            else
            {
                readyText.gameObject.active = false;
                notReadyText.gameObject.active = true;
            }
        }
    }
}
