using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPLightCTRL : MonoBehaviour
{

    void iniMainLight() {
        if (!mainLight) {
            mainLight = gameObject.GetComponent<Light>();

            if (!mainLight)
                Invoke("iniMainLight", 1);
        }
    }
    Light mainLight;

    void iniGameplay() {
        if (!gameplayCTRL) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }

        }
    }
    GameplayCTRL gameplayCTRL;

    Setings setings;
    void iniSetings() {
        if (!setings) {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj) {
                setings = setingsObj.GetComponent<Setings>();
            }
        }
    }

    [SerializeField]
    AudioSource ASSpark;
    [SerializeField]
    AudioClip[] ACSparks;

    // Start is called before the first frame update
    void Start()
    {
        iniMainLight();
    }

    // Update is called once per frame
    void Update()
    {
        iniGameplay();
        iniSetings();

        testDamage();
        testLight();
    }


    float BaceHealthOld = 0;
    void testDamage() {
        if (gameplayCTRL) {
            if (gameplayCTRL.gamemode == 2) {
                if (gameplayCTRL.BaceHealth != BaceHealthOld) {
                    if (gameplayCTRL.BaceHealth < BaceHealthOld && gameplayCTRL.BaceHealth >= 0)
                    {
                        lastShakeTime = Time.unscaledTime;
                    }

                    BaceHealthOld = gameplayCTRL.BaceHealth;
                }
            }
        }
    }

    float lightNeed = 1;
    float lastShakeTime = -60;
    void testLight() {
        if (gameplayCTRL && mainLight) {

            TestNeed();
            CalcNeed();

            void TestNeed() {
                float timeShake = Time.unscaledTime - lastShakeTime;
                if (gameplayCTRL.BaceHealth > 80) {
                    lightNeed = 1;
                }
                else if (gameplayCTRL.BaceHealth > 60)
                {
                    if (timeShake < 0.1f)
                    {
                        lightNeed = 0;
                    }
                    else lightNeed = 1;
                }
                else if (gameplayCTRL.BaceHealth > 40)
                {
                    if (timeShake < 0.3f)
                    {
                        lightNeed = 0;
                    }
                    else if (timeShake < 0.6f) {
                        lightNeed = 1;
                    }
                    else if (timeShake < 0.7f) {
                        lightNeed = 0;
                    }
                    else lightNeed = 1;
                }
                else if (gameplayCTRL.BaceHealth >= 0)
                {
                    if (timeShake < 0.1f)
                    {
                        lightNeed = 0;
                    }
                    else if (timeShake < 0.5f)
                    {
                        lightNeed = 1;
                    }
                    else if (timeShake < 0.9f)
                    {
                        lightNeed = 0;
                    }
                    else lightNeed = 1;
                }
            }
            void CalcNeed() {
                if (mainLight.intensity < lightNeed) {
                    mainLight.intensity += Time.deltaTime;
                    if (mainLight.intensity > lightNeed)
                        mainLight.intensity = lightNeed;
                }
                else if (mainLight.intensity > lightNeed) {
                    mainLight.intensity = 0;
                    playSoundSpark();
                }

                mainLight.intensity = Calc.CutFrom0To1(mainLight.intensity);

            }
            
            void playSoundSpark() {
                if (ASSpark && ACSparks.Length > 0 && setings && setings.game != null) {
                    int numClip = Random.Range(0, ACSparks.Length);

                    ASSpark.pitch = Random.Range(0.9f, 1.1f);
                    ASSpark.volume = setings.game.volume_all * setings.game.volume_sound;
                    ASSpark.PlayOneShot(ACSparks[numClip]);
                }
            }
        }
    }
}
