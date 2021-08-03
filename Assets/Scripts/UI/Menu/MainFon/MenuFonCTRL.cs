using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuFonCTRL : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("iniSetings", 0.1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        TestMouseView();
        TestShakeTime();
        TestLogoLight();

        TestSoundShake();
        TestSoundAmbient();
        TestSoundFonPeople();
        TestSoundFonPeopleSays();
        TestSoundFallDust();

        testSpawnSilhouette();

        MainPipleAnimator();
    }
    [Header("Fon: image")]
    [SerializeField]
    RectTransform[] rect;
    [SerializeField]
    float[] Coof;

    [SerializeField]
    int SilhouetteLayerNum;
    [SerializeField]
    GameObject SilhouetteObjPrefab;

    float SilhouetteNextSpawn = 3;
    void testSpawnSilhouette() {
        if (rect != null && rect.Length > SilhouetteLayerNum && rect[SilhouetteLayerNum] && SilhouetteObjPrefab) {
            SilhouetteNextSpawn -= Time.unscaledDeltaTime;
            if (SilhouetteNextSpawn < 0) {
                SilhouetteNextSpawn = Random.Range(1.5f, 10f);
                GameObject SilhouetteObj = Instantiate(SilhouetteObjPrefab, rect[SilhouetteLayerNum].gameObject.transform);
                if (SilhouetteObj == null)
                    return;

                FonSilhouette Silhouette = SilhouetteObj.GetComponent<FonSilhouette>();
                if (Silhouette) {
                    Silhouette.iniSilhouette();
                }
                else
                {
                    Destroy(SilhouetteObj);
                }
            }
        }
    }

    Vector2 mousePosNow = new Vector2();
    void TestMouseView() {
        //Сперва вытаскиваем позицию мыши на экране
        Vector2 mousePosNeed = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
        //Сместим на половину назад
        //mousePos -= new Vector2(0.5f, 0.5f);
        if (Time.deltaTime < 1f/5) {
            mousePosNow += (mousePosNeed - mousePosNow) * Time.deltaTime * 5;
        }
        else {
            mousePosNow += (mousePosNeed - mousePosNow) * Time.deltaTime;
        }

        //Дрожание
        Vector2 shakeCam = new Vector2();
        float shakeTimeMax = 2;
        if (ShakeTimeNow < shakeTimeMax) {
            float shakeCoof = 1 - (ShakeTimeNow / shakeTimeMax);
            shakeCam = new Vector2(Random.Range(0, shakeCoof), Random.Range(0, shakeCoof));
        }

        //Вращение камеры
        Vector2 rotateCam = new Vector2(Mathf.Sin(Time.time), Mathf.Cos(Time.time));

        //Теперь смещаем позицию каждой картины
        if (rect.Length == Coof.Length) {
            for (int num = 0; num < rect.Length; num++) {
                rect[num].position = new Vector3(
                    mousePosNow.x * (Coof[num]/1) + Screen.width/2 + (shakeCam.x * mousePosNow.x * Coof[num]) + shakeCam.y/3 + rotateCam.x * Coof[num]/1,
                    mousePosNow.y * (Coof[num]/2) + Screen.height/2 + (shakeCam.y * mousePosNow.y * Coof[num]) + shakeCam.x / 3 + rotateCam.y * Coof[num]/5);
            }
        }
    }

    float ShakeTimeLogoLight = 99;
    float logoLightAlphaNow = 1;
    void TestLogoLight() {
        float logoLightAlphaNeed = 1;

        ShakeTimeLogoLight += Time.unscaledDeltaTime;


        if (ShakeTimeLogoLight < 0.25f)
            Off();
        else if (ShakeTimeLogoLight < 0.5f)
            On(1);
        else if (ShakeTimeLogoLight < 1f)
            Off();
        else if (ShakeTimeLogoLight < 2.6f)
            On(0.4f);
        else if (ShakeTimeLogoLight < 3f)
            Off();
        else
            On(1);

        if (logoLightAlphaNow > logoLightAlphaNeed)
            logoLightAlphaNow = logoLightAlphaNeed;

        void Off() {
            logoLightAlphaNeed = 0;
            if (logoLightAlphaNow != logoLightAlphaNeed)
            {
                logoLightAlphaNow = logoLightAlphaNeed;
                PlaySoundLogoSparks();
            }
        }
        void On(float need) {
            logoLightAlphaNeed = need;
            if (logoLightAlphaNow != logoLightAlphaNeed)
            {
                logoLightAlphaNow += Time.unscaledDeltaTime;

            }
        }

        foreach (UnityEngine.UI.Image logo in LogoLighg) {
            logo.color = new Color(1,1,1, logoLightAlphaNow);
        }
    }

    float ShakeTimeNow = 15;
    float ShakeTimeNext = 20;
    [SerializeField]
    ParticleSystem[] ShakeParticle;
    [SerializeField]
    AnimatorPlay MapBaceAnim;
    void TestShakeTime() {
        ShakeTimeNow += Time.deltaTime;
        if (ShakeTimeNow > ShakeTimeNext) {
            ShakeTimeNext = Random.Range(5f,20f);
            ShakeTimeNow = 0;


            if (MapBaceAnim)
                MapBaceAnim.play();

            //С некой вероятностью начинает мигать лампа
            if (Random.Range(0, 100f) < 20f) {
                ShakeTimeLogoLight = 0;

                //Анимируем испуг центральных
                AnimatedBaceDamage = true;
            }

            //отсрачиваем разговоры
            timePeopleSay += 3;

            //С некой вероятностью падает предмет
            if (Random.Range(0, 100f) < 90f)
                Invoke("PlaySoundShakeFallItems", Random.Range(0f, 1.1f));

            //С некой вероятностью шумит человек
            if (Random.Range(0, 100f) < 50f)
                Invoke("PlaySoundShakePeople", Random.Range(2f, 6f));

            //С некой вероятностью говорит база
            if (Random.Range(0, 100f) < 50f) {
                Invoke("PlaySoundBaceVoice", Random.Range(2f, 14f));
            }

            foreach (ParticleSystem particle in ShakeParticle) {
                if (particle != null) {
                    particle.Play();
                }
            }
        }
    }

    [Header("Sounds")]
    Setings setings;
    void iniSetings() {
        if (setings == null) {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj != null) {
                setings = setingsObj.GetComponent<Setings>();
            }
        }
    }

    [Header("Sounds: Shake")]
    [SerializeField]
    AudioSource ASShakeBOOM;

    [SerializeField]
    AudioSource ASShakeFallDust;

    [SerializeField]
    AudioSource ASShakePeople;
    [SerializeField]
    AudioClip[] ACShakePeople;
    [SerializeField]
    AudioSource ASShakeFallItems;
    [SerializeField]
    AudioClip[] ACShakeFallItems;

    [Header("Sounds: Fon")]
    [SerializeField]
    UnityEngine.UI.Image[] LogoLighg;
    [SerializeField]
    AudioSource ASFonLogoSpark;
    [SerializeField]
    AudioClip[] ACFonLogoSparks;
    [SerializeField]
    AudioSource ASFonAmbient;
    [SerializeField]
    AudioSource ASFonPeople;
    [SerializeField]
    AudioClip[] ACFonPeople;
    [SerializeField]
    AudioSource ASFonRandomSound;
    [SerializeField]
    AudioClip[] ACFonRandonSounds;

    [SerializeField]
    AudioSource ASBaceVoice;
    [SerializeField]
    AudioClip[] ACBaceVoices;

    [SerializeField]
    AudioSource ASFonElectronics;
    [SerializeField]
    AudioSource[] ACFonElectronics;

    void TestSoundShake() {
        if (setings != null && setings.game != null && ASShakeBOOM != null && ASShakeBOOM.clip != null) {

            //Громкость
            float volumeShakeNow = 0;
            float SoundShakeBoomTimeMax = 3;
            if (ShakeTimeNow < SoundShakeBoomTimeMax)
                volumeShakeNow = 1 - (ShakeTimeNow / SoundShakeBoomTimeMax);

            ASShakeBOOM.volume = volumeShakeNow * setings.game.volume_all * setings.game.volume_sound;
            
            //Запускаем если не запущено
            if (!ASShakeBOOM.isPlaying)
                ASShakePeople.Play();
            
            
        }
    }

    float volumeAmdientNow = 0;
    void TestSoundAmbient()
    {
        if (setings != null && setings.game != null && ASFonAmbient != null && ASFonAmbient.clip != null)
        {
            float volumeAmdientNeed = 0.10f;

            volumeAmdientNow += Time.unscaledDeltaTime * 0.1f;
            if (volumeAmdientNow > volumeAmdientNeed)
                volumeAmdientNow = volumeAmdientNeed;

            ASFonAmbient.volume = volumeAmdientNow * setings.game.volume_all * setings.game.volume_sound;

            //Запускаем если не запущено
            if (!ASFonAmbient.isPlaying)
                ASFonAmbient.Play();
        }
    }

    float volumeFallDustNow = 0;
    void TestSoundFallDust()
    {
        if (setings != null && setings.game != null && ASShakeFallDust != null && ASShakeFallDust.clip != null)
        {
            float volumeFallDustNeed = 0f;
            if (ShakeTimeNow > 0.5f && ShakeTimeNow < Random.Range(1f, 2f))
                volumeFallDustNeed = 0.5f;

            volumeFallDustNow += (volumeFallDustNeed - volumeFallDustNow) * Time.unscaledDeltaTime;

            ASShakeFallDust.volume = volumeFallDustNow * setings.game.volume_all * setings.game.volume_sound;

            //Запускаем если не запущено
            if (!ASShakeFallDust.isPlaying)
                ASShakeFallDust.Play();
        }
    }

    void TestSoundFonPeople()
    {
        if (setings != null && setings.game != null && ASFonPeople != null && ASFonPeople.clip != null)
        {

            ASFonPeople.volume = volumeAmdientNow * setings.game.volume_all * setings.game.volume_sound * 2;

            //Запускаем если не запущено
            if (!ASFonPeople.isPlaying)
                ASFonPeople.Play();
        }
    }

    float timePeopleSay = 10;
    void TestSoundFonPeopleSays() {
        if (setings != null && setings.game != null && ASFonRandomSound != null && ACFonRandonSounds.Length > 0)
        {
            timePeopleSay -= Time.unscaledDeltaTime;

            //Запускаем если не запущено
            if (timePeopleSay < 0) {
                timePeopleSay = Random.Range(6, 20);

                //выбираем рандомный звук
                int numClip = Random.Range(0, ACFonRandonSounds.Length);

                ASFonRandomSound.volume = Random.Range(0.01f, 0.1f) * setings.game.volume_all * setings.game.volume_sound;

                ASFonRandomSound.pitch = Random.Range(0.8f, 1.2f);
                ASFonRandomSound.panStereo = Random.Range(-0.75f, 0.75f);
                ASFonRandomSound.PlayOneShot(ACFonRandonSounds[numClip]);
            }

        }
    }

    void PlaySoundShakeFallItems() {
        if (setings != null && setings.game != null && ASShakeFallItems != null && ACShakeFallItems.Length > 0)
        {
            //Запускаем если не запущено
            if (!ASShakeFallItems.isPlaying)
            {
                //выбираем рандомный звук
                int numClip = Random.Range(0, ACShakeFallItems.Length);

                ASShakeFallItems.volume = Random.Range(0.1f, 0.5f) * setings.game.volume_all * setings.game.volume_sound;
                ASShakeFallItems.pitch = Random.Range(0.8f, 1.2f);
                ASShakeFallItems.panStereo = Random.Range(-0.75f, 0.75f);

                if(ACShakeFallItems[numClip] != null)
                    ASShakeFallItems.PlayOneShot(ACShakeFallItems[numClip]);
            }
        }
    }

    void PlaySoundShakePeople()
    {
        if (setings != null && setings.game != null && ASShakePeople != null && ACShakePeople.Length > 0)
        {
            //Запускаем если не запущено
            if (!ASShakePeople.isPlaying)
            {
                //выбираем рандомный звук
                int numClip = Random.Range(0, ACShakePeople.Length);

                ASShakePeople.volume = Random.Range(0.05f, 0.1f) * setings.game.volume_all * setings.game.volume_sound;
                ASShakePeople.pitch = Random.Range(0.8f, 1.2f);
                ASShakePeople.panStereo = Random.Range(-0.75f, 0.75f);

                if (ACShakePeople[numClip] != null)
                    ASShakePeople.PlayOneShot(ACShakePeople[numClip]);
            }
        }
    }

    void PlaySoundLogoSparks() {
        if (setings != null && setings.game != null && ASFonLogoSpark != null && ACFonLogoSparks.Length > 0)
        {
            //выбираем рандомный звук
            int numClip = Random.Range(0, ACFonLogoSparks.Length);

            ASFonLogoSpark.volume = 0.05f * setings.game.volume_all * setings.game.volume_sound;
            ASFonLogoSpark.pitch = Random.Range(0.8f, 1.2f);
            //ASFonLogoSpark.panStereo = Random.Range(-0.75f, 0.75f);

            if (ACFonLogoSparks[numClip] != null)
                ASFonLogoSpark.PlayOneShot(ACFonLogoSparks[numClip]);
        }
    }

    float baceVoiceOldPlay = -20;
    void PlaySoundBaceVoice() {
        if (setings != null && setings.game != null && ASBaceVoice != null && ACBaceVoices.Length > 0 && baceVoiceOldPlay + 20 < Time.unscaledTime)
        {
            baceVoiceOldPlay = Time.unscaledTime;

            //выбираем рандомный звук
            int numClip = Random.Range(0, ACBaceVoices.Length);

            ASBaceVoice.volume = 0.07f * setings.game.volume_all * setings.game.volume_sound;
            ASBaceVoice.pitch = 1;
            //ASFonLogoSpark.panStereo = Random.Range(-0.75f, 0.75f);

            if (ACBaceVoices[numClip] != null)
                ASBaceVoice.PlayOneShot(ACBaceVoices[numClip]);
        }
    }

    [Header("MainPeople")]
    [SerializeField]
    Animator LeftPiople;
    [SerializeField]
    Animator MidlePiople;
    [SerializeField]
    Animator RightPiople;

    bool AnimatedBaceDamage = false;
    float AnimatedLeftSpeaking = 0;
    float AnimatedRightSpeaking = 0;

    float AnimatedNormalNow = 0;
    float AnimatedNormalNeed = 0;
    float AnimatedNornalTimeNext = 0;
    void MainPipleAnimator() {
        if (LeftPiople && MidlePiople && RightPiople) {
            AnimatedNornalTimeNext -= Time.deltaTime;
            if (AnimatedNornalTimeNext < 0) {
                AnimatedNornalTimeNext = Random.Range(7f, 15f);
                AnimatedNormalNeed = Random.Range(0,2);
            }

            //Изменение нормальной анимации
            if (AnimatedNormalNow < AnimatedNormalNeed)
            {
                AnimatedNormalNow += Time.deltaTime;
                if (AnimatedNormalNow > AnimatedNormalNeed) {
                    AnimatedNormalNow = AnimatedNormalNeed;
                }
            }
            else {
                AnimatedNormalNow -= Time.deltaTime;
                if (AnimatedNormalNow < AnimatedNormalNeed)
                {
                    AnimatedNormalNow = AnimatedNormalNeed;
                }
            }
            LeftPiople.SetFloat("Normal", AnimatedNormalNow);
            RightPiople.SetFloat("Normal", AnimatedNormalNow);


            if (AnimatedBaceDamage) {
                LeftPiople.SetBool("BaceDamage", true);
                MidlePiople.SetBool("BaceDamage", true);
                RightPiople.SetBool("BaceDamage", true);
                AnimatedBaceDamage = false;
            }
            else {
                LeftPiople.SetBool("BaceDamage", false);
                MidlePiople.SetBool("BaceDamage", false);
                RightPiople.SetBool("BaceDamage", false);


            }
        }
    }
}
