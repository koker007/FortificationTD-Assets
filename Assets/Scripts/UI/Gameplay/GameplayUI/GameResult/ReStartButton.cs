using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class ReStartButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TestHideImage();
        TestClockImage();
    }


    void TestHideImage() {
        if (GameplayCTRL.main && HideImage != null) {
            
            if(HideImage.pixelsPerUnitMultiplier < 0.5f)
                HideImage.pixelsPerUnitMultiplier += (1 - HideImage.pixelsPerUnitMultiplier) * Time.deltaTime * 0.1f;
            else HideImage.pixelsPerUnitMultiplier += (1 - HideImage.pixelsPerUnitMultiplier) * Time.deltaTime;

            HideImage.SetAllDirty();
        }
    }
    [SerializeField]
    Image HideImage;

    void TestClockImage() {
        if (GameplayCTRL.main && ClockImage != null && GameplayCTRL.main.players != null && GameplayCTRL.main.players.Count > 0) {
            int playersMax = 0;
            int playersReady = 0;
            foreach (Player player in GameplayCTRL.main.players) {
                if (player.ReadyToStartGame) {
                    playersMax++;
                    if (player.ReadyToRestartGame)
                        playersReady++;
                }
            }

            float needAmount = (float)playersReady / (float)playersMax;
            ClockImage.fillAmount += (needAmount - ClockImage.fillAmount) * Time.deltaTime;
        }
    }
    [SerializeField]
    Image ClockImage;

    public void ReStartValue() {
        if (ClockImage)
            ClockImage.fillAmount = 0;
        if (HideImage)
        {
            HideImage.pixelsPerUnitMultiplier = 0.01f;
            HideImage.SetAllDirty();
        }
        gameObject.active = false;
    }

    public void ButtonClickReStart() {
        if (GameplayCTRL.main != null && GameplayCTRL.main.players != null) {
            if (GameplayCTRL.main != null && GameplayCTRL.main.players != null)
            {
                foreach (Player player in GameplayCTRL.main.players)
                {
                    if (player.isLocalPlayer)
                    {
                        player.CmdSetComString("clickReadyRestart", "true");
                    }
                }
            }
        }
    }
}
