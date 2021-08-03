using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayBaceIndicator :MonoBehaviour
{
    [SerializeField]
    AudioSource ASIndidator;
    [SerializeField]
    AudioClip ACAttention;
    [SerializeField]
    AudioClip ACDamage;

    [SerializeField]
    GameObject AttentionIndicator;
    [SerializeField]
    GameObject AttentionIndicatorSymbol;

    [SerializeField]
    GameObject DamageIndicator;
    [SerializeField]
    GameObject DamageIndicatorSymbol;

    [SerializeField]
    GameplayCTRL gameplayCTRL;
    void iniGameplayCTRL() {
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null)
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
        }
    }


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (gameplayCTRL != null) {
            TestBaceHealth();
            TestBaceHealthColor();
            TestRocketTransform();
        }
        else {
            iniGameplayCTRL();
        }

        TestIndicators();
    }

    [SerializeField]
    Image HealthImage;
    [SerializeField]
    RectTransform RocketRect;
    [SerializeField]
    RectTransform RocketRectPosUP;
    [SerializeField]
    RectTransform RocketRectPosDown;
    [SerializeField]
    RawImage RocketArrowUP;
    [SerializeField]
    RawImage RocketArrowDown;

    [SerializeField]
    RawImage Smoke;
    [SerializeField]
    Texture[] SmokeTextures;    

    void TestBaceHealth(){
        if (HealthImage != null) {
            HealthImage.fillAmount = 1 - (gameplayCTRL.BaceHealth / 100);
        }
    }
    void TestRocketTransform() {
        if (GameplayCTRL.main && RocketRect && RocketRectPosDown && RocketRectPosUP && RocketArrowUP && RocketArrowDown) {
            int LVLNow = (int)(GameplayCTRL.main.timeGamePlay / 600);
            float LVLSec = GameplayCTRL.main.timeGamePlay % 600;
            if (GameplayCTRL.main.gamemode == 2)
            {
                //Опускание
                if (LVLNow > 0 && LVLSec < 50)
                {
                    float progress = (50 - LVLSec) / 50;
                    float pos = Mathf.Lerp(RocketRectPosDown.position.y, RocketRectPosUP.position.y, progress);
                    RocketRect.position = new Vector3(RocketRect.position.x, pos, RocketRect.position.z);

                    RocketArrowDown.gameObject.SetActive(true);
                    Rect rect = RocketArrowDown.uvRect;
                    rect.y = Time.unscaledTime * 0.5f;
                    RocketArrowDown.uvRect= rect;
                    RocketArrowUP.gameObject.SetActive(false);
                }
                //Поднимание
                else if (LVLSec > 480 && LVLSec < 540)
                {
                    float progress = (LVLSec - 480) / 60;
                    float pos = Mathf.Lerp(RocketRectPosDown.position.y, RocketRectPosUP.position.y, progress);
                    RocketRect.position = new Vector3(RocketRect.position.x, pos, RocketRect.position.z);

                    RocketArrowUP.gameObject.SetActive(true);
                    Rect rect = RocketArrowUP.uvRect;
                    rect.y = Time.unscaledTime * -0.5f;
                    RocketArrowUP.uvRect = rect;
                    RocketArrowDown.gameObject.SetActive(false);
                }
                //Поднятый
                else if (LVLSec >= 540)
                {
                    RocketRect.position = RocketRectPosUP.position;
                    RocketArrowUP.gameObject.SetActive(false);
                    RocketArrowDown.gameObject.SetActive(false);
                }
                //Опущенный
                else {
                    RocketRect.position = RocketRectPosDown.position;
                    RocketArrowUP.gameObject.SetActive(false);
                    RocketArrowDown.gameObject.SetActive(false);
                }
            }
            else {
                //Опущенный
                RocketRect.position = RocketRectPosDown.position;
            }
        }
    }

    float healthOld = 100;
    float TimeHealthFlash = 0;
    float NeedFlash = 1;
    void TestBaceHealthColor() {
        if (HealthImage != null) {
            if (healthOld > gameplayCTRL.BaceHealth) {
                healthOld = gameplayCTRL.BaceHealth;
                TimeHealthFlash = 3;
            }

            if (TimeHealthFlash > 0) {
                TimeHealthFlash -= Time.unscaledDeltaTime;
                if (TimeHealthFlash <= 0)
                    TimeHealthFlash = 0;

                float alpha;

                if (NeedFlash < 0.5f) {
                    alpha = HealthImage.color.a - Time.unscaledDeltaTime * 4;
                    if (alpha < NeedFlash) {
                        NeedFlash = 1;
                        alpha = 0;
                    }
                }
                else {
                    alpha = HealthImage.color.a + Time.unscaledDeltaTime * 4;
                    if (alpha > NeedFlash) {
                        NeedFlash = 0;
                        alpha = 1;
                    }
                }
                HealthImage.color = new Color(HealthImage.color.r, HealthImage.color.g, HealthImage.color.b, alpha);
            }
            else {
                float alpha = HealthImage.color.a + Time.unscaledDeltaTime;

                HealthImage.color = new Color(HealthImage.color.r, HealthImage.color.g, HealthImage.color.b, alpha);
            }
        }
    }

    float AttentionVolume = 1;
    float AttentionActiveTime = 0;
    void TestIndicators() {
        if (GameplayCTRL.main && DamageIndicator && DamageIndicatorSymbol && AttentionIndicator && AttentionIndicatorSymbol && Smoke) {
            float frequency = 1f;

            //Если получен урон
            if (Time.unscaledTime - GameplayCTRL.main.timeDamage < 10 && GameplayCTRL.main.BaceHealth < 60)
            {
                visualDamage();
                closeAttention();
            }
            else if (GameplayCTRL.main.AttentionMap != null && gameplayCTRL.AttentionMap.ActivePixelsOld > 0) {
                visualAttention();
                closeDamage();
            }
            else {
                closeDamage();
                closeAttention();
            }

            TestSmoke();

            void closeDamage() {
                if (DamageIndicator.activeSelf)
                    DamageIndicator.SetActive(false);
                if (DamageIndicatorSymbol.activeSelf)
                    DamageIndicatorSymbol.SetActive(false);
            }
            void closeAttention() {
                if (AttentionIndicator.activeSelf)
                    AttentionIndicator.SetActive(false);
                if (AttentionIndicatorSymbol.activeSelf)
                    AttentionIndicatorSymbol.SetActive(false);

                AttentionVolume += (1-AttentionVolume) * Time.unscaledDeltaTime * 0.10f;

            }

            void visualDamage() {
                if (!DamageIndicator.activeSelf)
                    DamageIndicator.SetActive(true);


                if (GameplayCTRL.main.timeGamePlay % frequency < frequency / 2f)
                {
                    DamageIndicatorSymbol.SetActive(false);
                }
                else {
                    if (!DamageIndicatorSymbol.activeSelf)
                        PlaySoundDamage();
                    DamageIndicatorSymbol.SetActive(true);
                }
            }
            void visualAttention()
            {

                AttentionActiveTime += Time.unscaledDeltaTime;
                if (AttentionActiveTime > 3)
                {
                    AttentionVolume -= AttentionVolume * Time.unscaledDeltaTime * 0.25f;
                }

                if (!AttentionIndicator.activeSelf)
                    AttentionIndicator.SetActive(true);


                if (GameplayCTRL.main.timeGamePlay % frequency < frequency / 2f)
                {
                    AttentionIndicatorSymbol.SetActive(false);
                }
                else
                {
                    if (!AttentionIndicatorSymbol.activeSelf)
                        PlaySoundAttention();
                    AttentionIndicatorSymbol.SetActive(true);
                }
            }

            void PlaySoundAttention() {
                if (Setings.main && Setings.main.game != null && ASIndidator && ACAttention) {
                    ASIndidator.volume = 0.10f * Setings.main.game.volume_all * Setings.main.game.volume_sound * AttentionVolume;
                    ASIndidator.pitch = 1;
                    ASIndidator.priority = 40;
                    ASIndidator.PlayOneShot(ACAttention);
                }
            }
            void PlaySoundDamage() {
                if (Setings.main && Setings.main.game != null && ASIndidator && ACDamage)
                {
                    ASIndidator.volume = 0.10f * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASIndidator.pitch = 1;
                    ASIndidator.priority = 40;
                    ASIndidator.PlayOneShot(ACDamage);
                }
            }

            void TestSmoke() {
                if (GameplayCTRL.main.BaceHealth < 40) {
                    Smoke.gameObject.SetActive(true);

                    int texNum = (int)((GameplayCTRL.main.timeGamePlay*4) % SmokeTextures.Length);
                    if (SmokeTextures[texNum] != null) {
                        Smoke.texture = SmokeTextures[texNum];
                    }
                }
                else {
                    Smoke.gameObject.SetActive(false);
                }
            }
        }
    }
}
