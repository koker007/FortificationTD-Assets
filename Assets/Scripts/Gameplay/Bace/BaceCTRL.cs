using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaceCTRL : MonoBehaviour
{

    float timeRosketStartSmoke = 5;

    [SerializeField]
    ParticleSystem RocketStartSmoke;
    void iniGameplayCTRL()
    {
        if (!gameplayCTRL)
        {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj)
            {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }

        if (!gameplayCTRL)
        {
            Invoke("iniGameplayCTRL", 1);
        }
    }
    GameplayCTRL gameplayCTRL;

    void iniSetings()
    {
        if (!setings)
        {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj)
            {
                setings = setingsObj.GetComponent<Setings>();
            }
        }

        if (!setings)
        {
            Invoke("iniSetings", 1);
        }
    }
    Setings setings;

    [SerializeField]
    Animator AnimatorBace;

    [SerializeField]
    public GameObject MainBaceObj;

    [SerializeField]
    ParticleSystem BaceSmoke;
    void testBaceSmoke() {
        if (BaceSmoke && GameplayCTRL.main) {
            if (!BaceSmoke.isPlaying && GameplayCTRL.main.BaceHealth <= 30)
            {
                BaceSmoke.Play();
            }
            else if(BaceSmoke.isPlaying && GameplayCTRL.main.BaceHealth > 30) {
                BaceSmoke.Stop();
            }
        }
    }


    [Header("AudioSource")]
    [SerializeField]
    AudioSource BaceShake;
    float healthOld = 0;
    float shakeVolumeNow = 0;
    void TestShake() {
        if (gameplayCTRL && BaceShake && setings && setings.game != null) {
            if (gameplayCTRL.BaceHealth != healthOld) {
                if (gameplayCTRL.BaceHealth < healthOld)
                {
                    shakeVolumeNow = 1;
                    gameplayCTRL.timeDamage = Time.unscaledTime;
                }
                healthOld = gameplayCTRL.BaceHealth;
            }
            else if (gameplayCTRL.gamemode == 3 && gameplayCTRL.timeEndScene > 1f) {
                shakeVolumeNow += Time.deltaTime;
            }

            shakeVolumeNow -= Time.deltaTime/3;
            shakeVolumeNow = Calc.CutFrom0To1(shakeVolumeNow);

            if (!BaceShake.isPlaying && BaceShake.clip != null)
                BaceShake.Play();

            BaceShake.volume = shakeVolumeNow * setings.game.volume_all * setings.game.volume_sound;
        }
    }

    void Start()
    {
        iniGameplayCTRL();
        iniSetings();
    }

    // Update is called once per frame
    void Update()
    {
        testRosketStartSmoke();
        testBaceAnim();

        TestShake();

        testBaceSmoke();
    }

    void testRosketStartSmoke()
    {
        if (gameplayCTRL && RocketStartSmoke)
        {
            if (gameplayCTRL.gamemode == 3)
            {
                if (gameplayCTRL.timeEndScene > timeRosketStartSmoke && gameplayCTRL.BaceHealth > 0)
                {
                    if (!RocketStartSmoke.isPlaying)
                    {
                        RocketStartSmoke.Play();
                    }
                }
                else
                {
                    if (RocketStartSmoke.isPlaying)
                    {
                        RocketStartSmoke.Stop();
                    }
                }
            }
            else if (RocketStartSmoke.isPlaying)
            {
                RocketStartSmoke.Stop();
            }
        }
    }

    void testBaceAnim() {
        if (gameplayCTRL && AnimatorBace) {
            if (gameplayCTRL.gamemode != 3) {
                AnimatorBace.SetBool("IsOpen", false);
            }
            else {
                if (gameplayCTRL.timeEndScene > 1 && gameplayCTRL.BaceHealth > 0)
                    AnimatorBace.SetBool("IsOpen", true);
            }
        }
    }

}
