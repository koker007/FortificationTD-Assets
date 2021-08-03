using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameplaySoundsCTRL : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        iniSetings();
        iniMainCam();
        iniGameplayCTRL();
    }

    Setings setings;
    void iniSetings() {
        if (setings == null) {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj != null) {
                setings = setingsObj.GetComponent<Setings>();
            }
        }
    }

    GameObject mainCamObj;
    void iniMainCam() {
        if (mainCamObj == null) {
            mainCamObj = GameObject.FindGameObjectWithTag("MainCamera"); 
        }
    }

    GameplayCTRL gameplayCTRL;
    void iniGameplayCTRL() {
        if (gameplayCTRL == null) {
            gameplayCTRL = gameObject.GetComponent<GameplayCTRL>();
            if (gameplayCTRL == null) {
                GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
                if (gameplayObj != null) {
                    gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
                }
            }
        }
    }

    float musicGameMode2VolumeNow = 0;

    [Header("Audio Sources")]
    [SerializeField]
    AudioSource ASAmbientSea;
    [SerializeField]
    AudioSource ASAmbiendEarth;
    [SerializeField]
    AudioSource ASAmbientHight10;
    [SerializeField]
    AudioSource ASAmbientBattle1020;
    [SerializeField]
    AudioSource ASAmbientBatte2050;

    [Header("Audio Clip")]
    [SerializeField]
    AudioClip ACSea;
    [SerializeField]
    AudioClip ACEarthStart;
    [SerializeField]
    AudioClip ACEarthWar;

    public float PriorityEnd = 100;

    public float ActiveTurretNeed = 0;
    float ActiveTurretNow = 0;

    // Update is called once per frame
    void Update()
    {
        TestAmbientSounds();
        TestMusic();
        TestFiltersMap();
    }

    [SyncVar] bool war = false;
    float camPlusNow = 100;
    void TestAmbientSounds() {
        //Проверяем что все на месте
        if (setings != null && setings.game != null && mainCamObj != null && gameplayCTRL != null) {
            //Узнаем насколько камера должна быть приподнята от земли
            float camPlusNeed = mainCamObj.transform.position.y;
            if (mainCamObj.transform.position.x >= 0 && mainCamObj.transform.position.x < 59 && mainCamObj.transform.position.z >= 0 && mainCamObj.transform.position.z < 59)
                camPlusNeed -= gameplayCTRL.cellsData[(int)mainCamObj.transform.position.x, (int)mainCamObj.transform.position.z].height;


            camPlusNow += (camPlusNeed - camPlusNow) * Time.unscaledDeltaTime;


            float volumeCoofEarth = Calc.CutFrom0To1(2 - camPlusNow/3);
            float volumeCoofHeight = Calc.CutFrom0To1((camPlusNow/3) - 1);

            if(SoundTurretCTRL.SoundMap != null)
                ActiveTurretNow += Calc.Nolmalize(SoundTurretCTRL.SoundMap.ActivePixelsOld - ActiveTurretNow) * Time.unscaledDeltaTime;

            float volumeCoofBattle1020 = Calc.CutFrom0To1(ActiveTurretNow/10 - 1);
            float volumeCoofBattle2050 = Calc.CutFrom0To1(ActiveTurretNow/20 - 1);

            //Атмосфера на земле
            if (ASAmbiendEarth && ASAmbientSea) {
                //Проверяем состояние войны
                if (gameplayCTRL.timeGamePlay > 20 && ActiveTurretNow >= 1 && !war && isServer)
                    war = true;

                if (war && (ASAmbiendEarth.clip != ACEarthWar || !ASAmbiendEarth.isPlaying ) && ACEarthWar != null)
                {
                    ASAmbiendEarth.Stop();
                    ASAmbiendEarth.clip = ACEarthWar;
                    ASAmbiendEarth.Play();
                }
                else if (!war && (ASAmbiendEarth.clip != ACEarthStart || !ASAmbiendEarth.isPlaying) && ACEarthStart != null ) {
                    ASAmbiendEarth.Stop();
                    ASAmbiendEarth.clip = ACEarthStart;
                    ASAmbiendEarth.Play();
                }

                if (ACSea && !ASAmbientSea.isPlaying) {
                    ASAmbientSea.clip = ACSea;
                    ASAmbientSea.Play();
                }

                //Узнаем растояние от цента карты.
                float volumeSea = Vector2.Distance(new Vector2(30,30), new Vector2(mainCamObj.transform.position.x, mainCamObj.transform.position.z)) / 30;
                if (volumeSea > 1) volumeSea = 1;

                ASAmbiendEarth.volume = volumeCoofEarth * setings.game.volume_sound * setings.game.volume_all * (1-volumeSea);
                ASAmbiendEarth.priority = (int)(PriorityEnd - volumeCoofEarth * 50 *(1 - volumeSea));

                ASAmbientSea.volume = 0.25f * volumeCoofEarth * setings.game.volume_sound * setings.game.volume_all * volumeSea;
                ASAmbientSea.priority = (int)(PriorityEnd - volumeCoofEarth * 50 * volumeSea);
            }

            //Атмосфера в воздухе
            if (ASAmbientHight10) {
                if (!ASAmbientHight10.isPlaying && ASAmbientHight10.clip != null)
                    ASAmbientHight10.Play();

                //Проверка громкости если быстрое перемещение
                float volumeVelosity = 0;
                if (MainCamera.main) volumeVelosity = MainCamera.main.nowVelosity * 0.25f;
                float volumeBasic = 0.1f * volumeCoofHeight;
                float volumeResult = volumeVelosity;
                if (volumeResult < volumeBasic) volumeResult = volumeBasic;
                if (volumeResult > 1) volumeResult = 1;

                //Уменьшение громкости если за пределами карты
                if (MainCamera.mainCamera && Vector3.Distance(MainCamera.mainCamera.transform.position, new Vector3(30, 5, 30)) > 30) {
                    volumeResult -= (Vector3.Distance(MainCamera.mainCamera.transform.position, new Vector3(30, 5, 30)) - 30)*0.1f;
                }

                ASAmbientHight10.volume = volumeResult * setings.game.volume_sound * setings.game.volume_all;
                ASAmbientHight10.priority = (int)(PriorityEnd - 500);
                ASAmbientHight10.pitch = 1 + volumeVelosity * 0.1f;
            }

            //война 10-20
            if (ASAmbientBattle1020 != null) {
                if (!ASAmbientBattle1020.isPlaying && ASAmbientBattle1020.clip != null)
                    ASAmbientBattle1020.Play();

                ASAmbientBattle1020.volume = 0.5f * volumeCoofBattle1020 * setings.game.volume_sound * setings.game.volume_all;
                ASAmbientBattle1020.priority = (int)(PriorityEnd - volumeCoofBattle1020 * 50);
            }

            //война 20-50
            if (ASAmbientBatte2050 != null)
            {
                if (!ASAmbientBatte2050.isPlaying && ASAmbientBatte2050.clip != null)
                    ASAmbientBatte2050.Play();

                ASAmbientBatte2050.volume = 0.5f * volumeCoofBattle2050 * setings.game.volume_sound * setings.game.volume_all;
                ASAmbientBatte2050.priority = (int)(PriorityEnd - volumeCoofBattle2050 * 50);
            }
        }
    }


    [SerializeField]
    AudioSource ASMusic1;
    [SerializeField]
    AudioSource ASMusic2;

    [SerializeField]
    AudioClip[] MusicStart;
    [SerializeField]
    AudioClip[] MusicWar;
    [SerializeField]
    AudioClip[] MusicEnd;

    //Счетчик музыки чтобы определить четный или нечетный использовать аудио сурс для включения нового трека
    int playMusicCount = 0;
    [SerializeField]
    [SyncVar] int playMusicType = 0;
    [SerializeField]
    [SyncVar] int playMusicNumNeed = 0;

    float timeMusicNow = 0;
    float timeMusicEnd = 0;
    float timePerehod = 3;

    [SerializeField]
    AudioHighPassFilter[] audioHighPassFilterMap;

    void TestMusic() {
        if (setings != null && ASMusic1 != null && ASMusic2 != null && gameplayCTRL != null) {
            //Время музыки идет вперед
            timeMusicNow += Time.unscaledDeltaTime;

            //Сервер проверяет нужно ли сменить музыку
            if (isServer && gameplayCTRL.gamemode == 2) {

                //Если не играет музыка
                if (playMusicCount % 2 == 0 && (!ASMusic1.isPlaying || timeMusicNow > (timeMusicEnd - timePerehod))) {
                    playNext();
                }
                else if (playMusicCount % 2 == 1 && (!ASMusic2.isPlaying || timeMusicNow > (timeMusicEnd - timePerehod))) {
                    playNext();
                }

                //Выбрать следующий трек
                void playNext() {
                    //проверяем какой тип музыки выбрать

                    //Концовка
                    if (gameplayCTRL.BaceHealth < 80) {
                        playMusicType = 0;
                        playMusicNumNeed = UnityEngine.Random.Range(0, MusicEnd.Length);
                    }
                    //Динамичный
                    else if (SoundTurretCTRL.SoundMap.ActivePixelsOld > 5) {
                        playMusicType = 1;
                        playMusicNumNeed = UnityEngine.Random.Range(0, MusicWar.Length);
                    }
                    //Спокойный
                    else {
                        playMusicType = 2;
                        playMusicNumNeed = UnityEngine.Random.Range(0, MusicStart.Length);
                    }
                }
            }

            //Проверяем какой трек выбран

            //Если война то выбираем музон из войны
            AudioClip SelectClip = null;

            //концовка
            if (playMusicType == 0 && MusicEnd != null && playMusicNumNeed < MusicEnd.Length) {
                SelectClip = MusicEnd[playMusicNumNeed];
            }
            //Динамичная
            else if (playMusicType == 1 && MusicWar != null && playMusicNumNeed < MusicWar.Length) {
                SelectClip = MusicWar[playMusicNumNeed];
            }
            //Спокойная
            else if (playMusicType == 2 && MusicStart != null && playMusicNumNeed < MusicStart.Length) {
                SelectClip = MusicStart[playMusicNumNeed];
            }

            //Если текущая музыка не соответствует ожидаемой, то меняем
            if (playMusicCount % 2 == 0)
            {
                if (ASMusic1.clip != SelectClip) {
                    SetAndPlayNextClip();
                }
            }
            else {
                if (ASMusic2.clip != SelectClip) {
                    SetAndPlayNextClip();
                }
            }

            TestVolume();

            void SetAndPlayNextClip() {
                if (gameplayCTRL && gameplayCTRL.gamemode == 2) {
                    playMusicCount++;

                    //устанавливаем время
                    timeMusicNow = 0;
                    timeMusicEnd = SelectClip.length;

                    //Ставим клип и включаем
                    if (playMusicCount % 2 == 0) {
                        ASMusic1.Stop();
                        ASMusic1.clip = SelectClip;
                        ASMusic1.Play();
                    }
                    else {
                        ASMusic2.Stop();
                        ASMusic2.clip = SelectClip;
                        ASMusic2.Play();
                    }
                }
            }
            void TestVolume() {
                float volumeClipOld = Calc.CutFrom0To1(1 - (timeMusicNow/timePerehod));
                float volumeClipNow = Calc.CutFrom0To1(timeMusicNow/timePerehod);

                if (gameplayCTRL != null)
                {
                    if (gameplayCTRL.gamemode == 2)
                    {
                        musicGameMode2VolumeNow += Time.unscaledDeltaTime / 5;
                        if (musicGameMode2VolumeNow > 1)
                            musicGameMode2VolumeNow = 1;
                    }
                    else {
                        musicGameMode2VolumeNow -= Time.unscaledDeltaTime / 2;
                        if (musicGameMode2VolumeNow < 0) {
                            musicGameMode2VolumeNow = 0;
                        }
                    }
                }

                if (playMusicCount % 2 == 0) {
                    ASMusic1.volume = volumeClipNow * musicGameMode2VolumeNow * setings.game.volume_all * setings.game.volume_music;
                    ASMusic2.volume = volumeClipOld * musicGameMode2VolumeNow * setings.game.volume_all * setings.game.volume_music;

                    ASMusic1.priority = (int)(50 - volumeClipNow * 20);
                    ASMusic2.priority = (int)(50 - volumeClipOld * 20);
                }
                else {
                    ASMusic1.volume = volumeClipOld * musicGameMode2VolumeNow * setings.game.volume_all * setings.game.volume_music;
                    ASMusic2.volume = volumeClipNow * musicGameMode2VolumeNow * setings.game.volume_all * setings.game.volume_music;

                    ASMusic1.priority = (int)(50 - volumeClipOld * 20);
                    ASMusic2.priority = (int)(50 - volumeClipNow * 20);
                }

                if (gameplayCTRL) {
                    if (gameplayCTRL.pause){
                        ASMusic1.pitch = 0;
                        ASMusic2.pitch = 0;
                    }
                    else {
                        ASMusic1.pitch = 1;
                        ASMusic2.pitch = 1;
                    }
                }
            }
        }
    }

    void TestFiltersMap() {
        if (audioHighPassFilterMap != null && audioHighPassFilterMap.Length > 0) {
            int value = 0;

            if(mainCamObj.transform.position.y > 13)
                value = (int)Math.Pow(1.5f, (mainCamObj.transform.position.y - 13)/1.5f);
            else
                value = 1;

            foreach (AudioHighPassFilter filter in audioHighPassFilterMap) {
                filter.cutoffFrequency = value;
            }
        }
    }
}
