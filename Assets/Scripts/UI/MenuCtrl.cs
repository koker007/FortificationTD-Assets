using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MenuCtrl : MonoBehaviour
{

    Setings setings;
    void iniSetings()
    {
        if (setings == null)
        {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj != null)
            {
                setings = setingsObj.GetComponent<Setings>();
            }
        }
    }

    [SerializeField]
    NetworkManager networkManager;
    void iniNetworkManager() {
        if (networkManager == null) {
            GameObject networkGameObj = GameObject.FindGameObjectWithTag("Network");
            if (networkGameObj != null)
                networkManager = networkGameObj.GetComponent<NetworkManager>();
        }
    }

    [Header("Menu")]
    [SerializeField]
    public GameObject MenuALL;

    [SerializeField]
    GameObject MenuGameplayFon; //Фон который просвечивается и слегка видно гейм плей
    [SerializeField]
    GameObject MenuMainFon;
    [SerializeField]
    GameObject MenuGameLogo;
    [SerializeField]
    GameObject MenuPanels;
    [SerializeField]
    GameObject MenuButtonReturn;
    [SerializeField]
    GameObject MenuButtonPlay;
    [SerializeField]
    GameObject MenuPanelPlay;
    [SerializeField]
    GameObject MenuButtonConnect;
    [SerializeField]
    GameObject MenuPanelConnect;
    [SerializeField]
    GameObject MenuPanelOptions;
    [SerializeField]
    GameObject MenuPanelResearch;
    [SerializeField]
    GameObject MenuPanelExit;

    [SerializeField]
    GameObject MenuConnecting;

    [SerializeField]
    GameplayCTRL gameplayCTRL;

    [SerializeField]
    GameObject GamePlayUIAll;
    [SerializeField]
    GameObject GamePlayMenu1Set;

    [Header("Music")]
    [SerializeField]
    AudioSource ASMusic;
    [SerializeField]
    AudioClip[] ACMusicMainMenu;

    [Header("Sounds")]
    [SerializeField]
    AudioSource ASConnection;
    [SerializeField]
    AudioClip ACConnecting;
    [SerializeField]
    AudioClip ACConnectComplite;

    void ButtonTest() {
        if (networkManager != null && networkManager.isNetworkActive)
        {
            if (MenuButtonPlay != null)
                MenuButtonPlay.active = false;
            if (MenuButtonConnect != null)
                MenuButtonConnect.active = false;
        }
        else {
            if (MenuButtonPlay != null)
                MenuButtonPlay.active = true;
            if (MenuButtonConnect != null)
                MenuButtonConnect.active = true;
        }
    }

    void CloseAll() {
        if (MenuGameLogo != null)
            MenuGameLogo.active = false;
        if (MenuPanelPlay != null)
            MenuPanelPlay.active = false;
        if (MenuPanelConnect != null)
            MenuPanelConnect.active = false;
        if (MenuPanelResearch)
            MenuPanelResearch.active = false;
        if (MenuPanelOptions != null)
            MenuPanelOptions.active = false;
        if (MenuConnecting != null)
            MenuConnecting.active = false;
        if (MenuPanelExit != null)
            MenuPanelExit.active = false;

    }

    public void ButtonReturnClick()
    {
        if (MenuALL != null)
        {
            if (MenuALL.active)
            {
                CloseAll();
                MenuPanelPlay.active = false;
                MenuGameLogo.active = true;
                MenuALL.active = false;
            }
        }
    }
    public void ButtonPlayClick() {
        if (MenuPanelPlay != null)
        {
            if (MenuPanelPlay.active)
            {
                CloseAll();
                MenuPanelPlay.active = false;
                MenuGameLogo.active = true;
            }
            else
            {
                CloseAll();
                MenuPanelPlay.active = true;
                MenuGameLogo.active = true;
            }
        }
    }
    public void ButtonConnectClick() {
        if (MenuPanelConnect != null)
        {
            if (MenuPanelConnect.active)
            {
                CloseAll();
                MenuPanelConnect.active = false;
                MenuGameLogo.active = true;
            }
            else
            {
                CloseAll();
                MenuPanelConnect.active = true;
            }
        }
    }

    public void ButtonResearchClick() {
        if (MenuPanelResearch != null)
        {
            if (MenuPanelResearch.active)
            {
                CloseAll();
                MenuPanelResearch.active = false;
                MenuGameLogo.active = true;
            }
            else
            {
                CloseAll();
                MenuPanelResearch.active = true;
            }
        }
    }
    public void ButtonOptionsClick() {
        if (MenuPanelOptions != null)
        {
            if (MenuPanelOptions.active)
            {
                CloseAll();
                MenuPanelOptions.active = false;
                MenuGameLogo.active = true;
            }
            else
            {
                CloseAll();
                MenuPanelOptions.active = true;
            }
        }
    }

    public void ButtonExitYesClick() {
        if (networkManager != null)
        {
            if (networkManager != null && networkManager.isNetworkActive)
            {
                CloseAll();
                networkManager.StopHost();
                networkManager.StopServer();
                networkManager.StopClient();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                Application.Quit();
            }
        }
    }
    public void ButtonExitMenuClick() {
        if (MenuPanelExit != null) {
            if (MenuPanelExit.active)
            {
                CloseAll();
                MenuPanelExit.active = false;
                MenuGameLogo.active = true;
            }
            else {
                CloseAll();
                MenuPanelExit.active = true;
                MenuGameLogo.active = true;
            }
        }
    }

    public void ButtonMainMenu() {
        if (MenuGameLogo != null && MenuPanels != null) {
            MenuGameLogo.active = true;
            MenuPanels.active = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        iniNetworkManager();
        iniSetings();

        //Выходим из лобби если меню только что запустилось
        SteamLobby.LeaveLobbyNow();
    }

    // Update is called once per frame
    void Update()
    {
        randRanelCanvas();
        ButtonTest();
        MainFunc();

        TestMusic();
        TestConnectSound();

        TestPlayCredits();
    }


    bool NetworkOld = false;
    void MainFunc() {
        if (MenuALL != null) {
            if (networkManager != null && networkManager.isNetworkActive) {
                if (MenuPanelConnect != null)
                    MenuPanelConnect.active = false;
                if (MenuPanelPlay != null)
                    MenuPanelPlay.active = false;

                //Если нетворк только заработал
                if (!NetworkOld) {
                    NetworkOld = true;

                    PlaySoundConnecting();

                    CloseAll();

                    MenuButtonPlay.active = false;
                    MenuButtonConnect.active = false;

                    MenuALL.active = false;
                }

                //Открытие игрового интерфейса
                if (GamePlayUIAll != null) {
                    GamePlayUIAll.active = true;
                }

                //Открытие или закрытие меню
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    if (MenuALL.active) {
                        MenuALL.active = false;
                    }
                    else {
                        MenuALL.active = true;
                    }
                }
                if (MenuMainFon != null && MenuMainFon.active) {
                    MenuMainFon.active = false;
                    MenuGameplayFon.active = true;
                }
            }
            else {
                //Если ранее игра запускалась перезапускаем
                if (NetworkOld) {
                    Time.timeScale = 1;
                    CloseAll();
                    Cursor.visible = true;
                    networkManager.StopHost();
                    networkManager.StopServer();
                    networkManager.StopClient();

                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                MenuALL.active = true;
                
                if(GamePlayUIAll != null)
                    GamePlayUIAll.active = false;

                StopSoundConnecting();
            }

            //Звук соединения
            void PlaySoundConnecting() {
                if (ASConnection != null && ACConnecting != null && setings != null && setings.game != null) {
                    ASConnection.volume = 0.05f * setings.game.volume_all * setings.game.volume_sound;
                    ASConnection.PlayOneShot(ACConnecting);
                }
            }
            void StopSoundConnecting() {
                if (ASConnection != null) {
                    ASConnection.Stop();
                }
            }
        }
    }

    float VolumeTimeMusicNow = 0;
    float StopTimeMusicMenu = 0;
    void TestMusic() {
        //Спервая проверяем что настройки все есть
        if (setings != null && setings.game != null && ASMusic != null && ACMusicMainMenu.Length > 0 && networkManager != null) {

            //Если главное меню
            if (!networkManager.isNetworkActive)
            {
                if (VolumeTimeMusicNow < 1)
                    VolumeTimeMusicNow += Time.unscaledDeltaTime * 0.1f;
            }
            else {
                if (VolumeTimeMusicNow > 0)
                    VolumeTimeMusicNow -= Time.unscaledDeltaTime * 0.1f;
            }


            if (!ASMusic.isPlaying)
            {
                StopTimeMusicMenu -= Time.unscaledDeltaTime;
                if (StopTimeMusicMenu < 0) {
                    StopTimeMusicMenu = Random.Range(180, 500);

                    ASMusic.Stop();
                    ASMusic.clip = ACMusicMainMenu[Random.Range(0, ACMusicMainMenu.Length)];
                    if (ASMusic.clip != null)
                        ASMusic.Play();
                }
            }
            else {
                ASMusic.volume = VolumeTimeMusicNow * setings.game.volume_all * setings.game.volume_music;
            }

        }
    }

    bool ConnectOld = false;
    bool networkingOld = false;
    void TestConnectSound() {
        if (GameplayCTRL.main != null)
        {
            if (!ConnectOld)
            {
                ConnectOld = true;
                StopSoundConnecting();
                PlayConnectOk();
            }

        }
        //Если игровое поле не создано
        else {
            //попытка подключения началась
            if (NetworkCTRL.networkManager.isNetworkActive && !networkingOld) {
                networkingOld = true;
                PlayConnecting();
            }
            //попытка подключения закончилась
            else if (!NetworkCTRL.networkManager.isNetworkActive && networkingOld) {
                networkingOld = false;
                StopSoundConnecting();
            }
        }

        void PlayConnecting()
        {
            if (setings != null && setings.game != null && ASConnection != null && ACConnecting != null)
            {
                ASConnection.volume = 0.5f * setings.game.volume_all * setings.game.volume_sound;
                ASConnection.PlayOneShot(ACConnecting);
            }
        }
        void PlayConnectOk() {
                        if (setings != null && setings.game != null && ASConnection != null && ACConnectComplite != null)
            {
                ASConnection.volume = 0.5f * setings.game.volume_all * setings.game.volume_sound;
                ASConnection.PlayOneShot(ACConnectComplite);
            }
        }
        void StopSoundConnecting()
        {
            if (ASConnection != null)
            {
                ASConnection.Stop();
            }
        }
    }

    [SerializeField]
    Canvas PanelCanvas;
    void randRanelCanvas() {
        if (PanelCanvas) {
            PanelCanvas.sortingOrder = 100 + Random.Range(0,1);
        }
    }

    float timeToPlayTitle = 10;
    void TestPlayCredits() {
        if (CreditsCtrl.main) {
            //Если геймплей
            if (GameplayCTRL.main != null) {
                CreditsCtrl.main.gameObject.active = false;
            }
            //Если гейм плея нет
            else {
                CreditsCtrl.main.gameObject.active = true;
            }


            if (!MenuPanelPlay.activeSelf &&
                !MenuPanelResearch.activeSelf &&
                !MenuPanelOptions.activeSelf &&
                !MenuPanelConnect.activeSelf &&
                !MenuPanelExit.activeSelf &&
                !MenuConnecting.activeSelf)
            {
                timeToPlayTitle -= Time.deltaTime;
            }
            else {
                timeToPlayTitle = 10;
            }

            if (timeToPlayTitle < 0)
            {
                CreditsCtrl.main.playTitle = true;
            }
            else {
                CreditsCtrl.main.playTitle = false;
            }
        }
    }
}
