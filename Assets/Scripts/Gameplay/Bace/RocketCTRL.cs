using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCTRL : MonoBehaviour
{
    static public RocketCTRL main;

    [SerializeField]
    float TimeStartEngineSmoke = 7;

    [SerializeField]
    ParticleSystem rosketEngine;

    void iniGameplayCTRL() {
        if (!gameplayCTRL)
        {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }

        if (!gameplayCTRL)
        {
            Invoke("iniGameplayCTRL", 1);
        }
    }
    GameplayCTRL gameplayCTRL;

    // Start is called before the first frame update
    void Start()
    {
        iniGameplayCTRL();
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        testRosketEngine();
    }

    void testRosketEngine() {
        if (gameplayCTRL && rosketEngine) {
            if (gameplayCTRL.gamemode == 3)
            {
                if (gameplayCTRL.timeEndScene > TimeStartEngineSmoke && gameplayCTRL.BaceHealth > 0) {
                    if (!rosketEngine.isPlaying)
                    {
                        rosketEngine.Play();
                    }
                }
                else
                {
                    if (rosketEngine.isPlaying) {
                        rosketEngine.Stop();
                    }
                }
            }
            else if(rosketEngine.isPlaying) {
                rosketEngine.Stop();
            }
        }
    }
}
