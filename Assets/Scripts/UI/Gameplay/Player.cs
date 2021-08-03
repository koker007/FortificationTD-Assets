using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager.UI;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class Player : NetworkBehaviour
{
    public static Player me;

    [Header("Main")]
    [SyncVar] public string Name = "noName";
    [SyncVar] public Vector3 colorVec = new Vector3();
    [SyncVar] public bool ReadyToStartGame = false;
    [SyncVar] public bool ReadyToEndGame = false;
    [SyncVar] public bool ReadyToRestartGame = false;
    [SyncVar] public float Money = 300;
    [SyncVar] public ulong SteamID = 0;

    [SyncVar] public int KillPehota = 0;
    [SyncVar] public int KillCar = 0;
    [SyncVar] public int KillCrab = 0;

    [Header("Cursor")]
    [SyncVar] public Vector3 CursorPos = new Vector3();
    [SyncVar] public bool CursorInUI = false;
    [SerializeField]
    GameObject PrefabCursorHand; //Префаб визуализатора курсора
    [SerializeField]
    public GameObject MyCursorHand;


    [Header("Other")]
    [SyncVar] public Vector3 MyCameraPos = new Vector3();
    public float TimeConnected = 0;
    [SyncVar] public float timeScaleNeed = 1;

    [SerializeField]
    Setings setings;
    void iniSettings() {
        if (setings == null) {
            GameObject setObj = GameObject.FindGameObjectWithTag("Setings");
            if (setObj != null) {
                setings = setObj.GetComponent<Setings>();
            }
        }
    }

    [SerializeField]
    GameplayCTRL gameplayCTRL;
    void iniGameplayCtrl() {
        if (gameplayCTRL == null && GameplayCTRL.main) {
            gameplayCTRL = GameplayCTRL.main;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer) me = this;

        iniSettings();
        IniCamera();
        Invoke("iniGameplayCtrl", 1f);

        iniCursorHand();

        TimeConnected = Time.unscaledTime;

        //Поместить обьект игрока в специальное для этого место
        if (PlayersObjs.main) transform.parent = PlayersObjs.main.transform;
    }

    void iniCursorHand() {
        if (PrefabCursorHand != null) {
            MyCursorHand = Instantiate(PrefabCursorHand);
            PlayerHand playerHand = MyCursorHand.GetComponent<PlayerHand>();

            GameObject MainCamObj = GameObject.FindGameObjectWithTag("MainCamera");
            Camera camera = new Camera();
            if (MainCamObj != null)
            {
                camera = MainCamObj.GetComponent<Camera>();
            }

            if (playerHand == null || MainCamObj == null) {
                Destroy(MyCursorHand);
            }

            playerHand.MyPlayer = this;
            playerHand.MainCamera = camera;
        }
    }

    // Update is called once per frame
    void Update()
    {
        TestPingRTT();
        TestSounds();

        testControl();
        testCamHeight();
        testPlayerSteam();
        testEndGameVictory();

        TestMoneyOfMin();
    }

    [Command]
    public void CmdSetReady()
    {
        //Поменять готовность можно только в режиме ожидания
        if (isServer && GameplayCTRL.main && GameplayCTRL.main.gamemode == 1)
        {
            if (ReadyToStartGame)
                ReadyToStartGame = false;
            else ReadyToStartGame = true;
        }
    }
    [Command] public void CmdSetColor(Vector3 colorNew) {
        if (isServer) {
            colorVec = colorNew;
        }
    }
    [Command]
    public void CmdSetName(string playerName, ulong cSteamID)
    {
        if (isServer)
        {
            Name = playerName;
            SteamID = cSteamID;
        }
    }
    void testPlayerSteam() {
        string steamName = SteamFriends.GetPersonaName();
        CSteamID cSteamID = SteamUser.GetSteamID();
        if (isLocalPlayer && (Name != steamName || cSteamID.m_SteamID != SteamID)) {
            CmdSetName(steamName, cSteamID.m_SteamID);
        }
    }

    [Command] public void CmdBuild(Vector2 poscell, string nameBuild) {
        //if (!isServer) {
        //    return;
        //}

        //Если нужно просто получить последние данные ячейки
        if (gameplayCTRL != null && nameBuild == MouseControls.cmdUpdate) {
            gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            return;
        }

        //Строительство
        if (gameplayCTRL != null && ReadyToStartGame) {
            GameObject buildPrefab = null;

            //Подчищать не надо
            bool dontClearUpdates = false;

            //строить строение блокирующее путь //Если время кд закончелось
            if (gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].height > 1 &&
                gameplayCTRL.CalcTraficAll(false, new Vector2Int((int)poscell.x, (int)poscell.y), false) &&
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeLastBuilding + gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeoutBuild < gameplayCTRL.timeGamePlay
            //&& gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].build == ""
            )
            {
                if (nameBuild == Building.getTypeName(Building.Type.Platform))
                {
                    buildPrefab = gameplayCTRL.buildPlatform;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.TraficBlocker) + "Car")
                {
                    buildPrefab = gameplayCTRL.buildBlockCar;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.TraficBlocker) + "Crab") {
                    buildPrefab = gameplayCTRL.buildBlockCrab;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.PillBox))
                {
                    buildPrefab = gameplayCTRL.buildPillBox;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.Turret))
                {
                    buildPrefab = gameplayCTRL.buildTurret;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.Artillery))
                {
                    dontClearUpdates = true;
                    buildPrefab = gameplayCTRL.buildArtillery;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.Minigun))
                {
                    buildPrefab = gameplayCTRL.buildMinigun;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.Laser))
                {
                    buildPrefab = gameplayCTRL.buildLaser;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.Thunder))
                {
                    dontClearUpdates = true;
                    buildPrefab = gameplayCTRL.buildThunder;
                }
                else if (nameBuild == Building.getTypeName(Building.Type.Rocket))
                {
                    buildPrefab = gameplayCTRL.buildRocketLauncher;
                }

                //Перерасчитываем маршрут если строение выбрано
                if (buildPrefab != null)
                {
                    gameplayCTRL.CalcTraficAll(false, new Vector2Int((int)poscell.x, (int)poscell.y), true);
                }
            }

            //Строить строение не блокирующее путь
            if (buildPrefab == null) {

            }

            //Если строение выбрано, подчищаем место и строим
            if (buildPrefab != null)
            {

                Building building = buildPrefab.GetComponent<Building>();

                //Проверяем можно ли строить
                if (Money < building.price)
                {
                    TypeSoundPlayNeed = 2;
                    return;
                }

                //подчищаем что надо
                ClearCell(dontClearUpdates);

                //Уменьшаем деньги
                Money -= building.price;
                //Присваиваем ячейке владельца
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ownerSteamID = SteamID;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost += building.price;

                //Присваеваем ячейке время строительства и кд
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeLastBuilding = gameplayCTRL.timeGamePlay;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeoutBuild = building.timeoutBuilding;

                GameObject buildObj = Instantiate(buildPrefab);
                buildObj.transform.position = new Vector3(poscell.x, gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].height, poscell.y);
                NetworkServer.Spawn(buildObj);

                TypeSoundPlayNeed = 3;
            }
            //Если не строим а удаляем
            else if (nameBuild == Building.getTypeName(Building.Type.None))
            {
                //Возвращаем средства
                Money += (gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost / 100) * 50;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost = 0;
                ClearCell(false);

                TypeSoundPlayNeed = 4;
            }
            else {
                TypeSoundPlayNeed = 2;
            }

            //Говорим обновить данные ячейки, обновляет только если сервер
            gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
        }

        void ClearCell(bool dontClearUpdates) {
            //Если уже есть строение, я владелец, ссылка на строение есть
            if (gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].build != ""
                && gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ownerSteamID == SteamID
                && gameplayCTRL.buildings[(int)poscell.x, (int)poscell.y] != null)
            {
                Destroy(gameplayCTRL.buildings[(int)poscell.x, (int)poscell.y].gameObject);
                gameplayCTRL.buildings[(int)poscell.x, (int)poscell.y] = null;
                //Присваиваем ячейке владельца
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ownerSteamID = 0;

                if (!dontClearUpdates)
                {
                    //Удаляем улучшения на постройке
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy1 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy2 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy3 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage1 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage2 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage3 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist1 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist2 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist3 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate1 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate2 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate3 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed1 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed2 = false;
                    gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed3 = false;
                }

                //Таймаут 3 секунды
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeoutBuild = 3;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeLastBuilding = gameplayCTRL.timeGamePlay;

                //Строения нету
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].build = Building.getTypeName(Building.Type.None);
            }
        }
    }
    [Command] public void CmdResearch(Vector2 poscell, string nameRes) {
        //Улучшаем если сервер и есть строение
        if (isServer && ReadyToStartGame && gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].build != "" && gameplayCTRL.buildings[(int)poscell.x, (int)poscell.y] != null
            //И время для улучщения
            && gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeLastBuilding + gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeoutBuild < gameplayCTRL.timeGamePlay
            //И количество улучшений меньше максимума
            && gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].getCountTech() < Tech.getMaxCountTechThisGameplayTime() + gameplayCTRL.buildings[(int)poscell.x, (int)poscell.y].StartUpgrages
            ) {

            float costTech = -1;

            //Проверяем можно ли там строить
            //Точность
            if (GPResearch.Allgun.accuracy1STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy1
                && Money >= GPResearch.Allgun.accuracy1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f) {
                costTech = GPResearch.Allgun.accuracy1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy1 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.accuracy2STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy2
                && Money >= GPResearch.Allgun.accuracy2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.accuracy2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy2 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.accuracy3STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy3
                && Money >= GPResearch.Allgun.accuracy3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.accuracy3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.accuracy3 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }

            //Скорострельность
            else if (GPResearch.Allgun.speed1STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed1
                && Money >= GPResearch.Allgun.speed1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.speed1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed1 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.speed2STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed2
                && Money >= GPResearch.Allgun.speed2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.speed2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed2 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.speed3STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed3
                && Money >= GPResearch.Allgun.speed3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.speed3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.speed3 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            //Урон
            else if (GPResearch.Allgun.damage1STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage1
                && Money >= GPResearch.Allgun.damage1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.damage1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage1 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.damage2STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage2
                && Money >= GPResearch.Allgun.damage2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.damage2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage2 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.damage3STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage3
                && Money >= GPResearch.Allgun.damage3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.damage3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.damage3 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            //дальность
            else if (GPResearch.Allgun.dist1STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist1
                && Money >= GPResearch.Allgun.dist1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.dist1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist1 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.dist2STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist2
                && Money >= GPResearch.Allgun.dist2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.dist2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist2 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.dist3STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist3
                && Money >= GPResearch.Allgun.dist3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.dist3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.dist3 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            //вращение
            else if (GPResearch.Allgun.rotate1STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate1
                && Money >= GPResearch.Allgun.rotate1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.rotate1Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate1 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.rotate2STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate2
                && Money >= GPResearch.Allgun.rotate2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.rotate2Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate2 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }
            else if (GPResearch.Allgun.rotate3STR == nameRes && !gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate3
                && Money >= GPResearch.Allgun.rotate3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f)
            {
                costTech = GPResearch.Allgun.rotate3Cost * gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost * 0.01f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].ResearchAllGun.rotate3 = true;
                gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
            }


            if (costTech >= 0 && gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].isTech(nameRes))
            {
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeLastBuilding = gameplayCTRL.timeGamePlay;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].timeoutBuild = 60 * 1.25f;
                gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y].cost += costTech;
                Money -= costTech;

                //Звук
                TypeSoundPlayNeed = 1;
            }

            //Обновить ячейку
            gameplayCTRL.addAndCellToSyncList(gameplayCTRL.cellsData[(int)poscell.x, (int)poscell.y]);
        }
        else
        {
            TypeSoundPlayNeed = 2;
        }
    }

    //Тераформирование ландшафта
    [Command] public void CmdTerraform(Vector2 poscell, bool plusLvL) {
        if (gameplayCTRL != null) {
            //Если терраформинг возможен
            int cost = gameplayCTRL.CalcTerraforming(plusLvL, new Vector2Int((int)poscell.x, (int)poscell.y), false);
            if (cost != 0) {
                if (Money > cost) {
                    Money -= cost;
                    gameplayCTRL.CalcTerraforming(plusLvL, new Vector2Int((int)poscell.x, (int)poscell.y), true);
                }
            }
        }
    }

    [Command] public void CmdTargetMode(Vector2 posCell, string nameMode) {
        if (isServer && gameplayCTRL != null) {
            if (gameplayCTRL.buildings[(int)posCell.x, (int)posCell.y] != null) {
                gameplayCTRL.buildings[(int)posCell.x, (int)posCell.y].targetMode = Building.getTargetModeName(nameMode);
            }
        }
    }

    //Передать параметры курсора
    [Command] void CmdSetMouse(bool funcMouseInUI, Vector3 funcPosCursor, Vector3 funcPosCamera) {
        if (!isServer) {
            return;
        }
        CursorInUI = funcMouseInUI;
        CursorPos = funcPosCursor;
        MyCameraPos = funcPosCamera;

    }


    //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Применяет разные команды текстовых значений
    /// </summary>
    //Для применения разных команд
    [Command] public void CmdSetComString(string command, string value) {
        if (isServer)
        {
            if (command == "clickReadyEnd" && GameplayCTRL.main)
            {
                int minMax = 10;

                int timeMin = 0;
                int timeSec = 0;

                GetTime();

                void GetTime()
                {
                    int time = (int)(GameplayCTRL.main.timeGamePlay % (60 * minMax));
                    timeMin = time / 60;
                    timeSec = time % 60;
                }
                if (timeMin == minMax - 1) {
                    if (value == "true" && !ReadyToEndGame)
                        ReadyToEndGame = true;
                    else if (value == "false" && ReadyToEndGame)
                        ReadyToEndGame = false;
                }
            }
            else if (command == "clickReadyRestart") {
                if (value == "true" && !ReadyToRestartGame)
                    ReadyToRestartGame = true;
                else if (value == "false" && ReadyToRestartGame)
                    ReadyToRestartGame = false;
            }

        }
    }
    [Command] public void CmdSetComFloat(string command, float value) {
        if (command == CommandSTR.TypeSoundToNull)
        {
            TypeSoundPlayNeed = (int)value;
        }
        else if (command == CommandSTR.TypeTimeScaleNeed)
        {
            timeScaleNeed = value;
        }
        else if (command == CommandSTR.TypePause) {
            //Если этот игрок играет
            if (ReadyToStartGame && GameplayCTRL.main) {
                gameplayCTRL.pause = !gameplayCTRL.pause;
            }
        }
    }

    [SyncVar]
    public int TypeSoundPlayNeed = 0;
    void TestSounds() {
        if (me == this) {
            if (TypeSoundPlayNeed != 0) {
                //Если звук улучшения
                if (TypeSoundPlayNeed == 1)
                {
                    ButtonSounds.SelectTech();
                }
                //Звук неверной команды
                else if (TypeSoundPlayNeed == 2)
                {
                    ButtonSounds.PlayNotAccept();
                }
                //Звук строительства
                else if (TypeSoundPlayNeed == 3) {
                    ButtonSounds.SelectBuild();
                }
                else if (TypeSoundPlayNeed == 4) {
                    ButtonSounds.SelectDestroy();
                }

                //Звук победы игры
                else if (TypeSoundPlayNeed == 100) {
                    ButtonSounds.GameVictory();
                }
                //Звук поражения игры
                else if (TypeSoundPlayNeed == 101) {
                    ButtonSounds.GameDefeated();
                }

                //Возвращаем звук на место
                CmdSetComFloat(CommandSTR.TypeSoundToNull, 0);
                TypeSoundPlayNeed = 0;
            }
        }
    }

    public static class CommandSTR{
        public static string TypeSoundToNull = "TypeSoundToNull";
        public static string TypeTimeScaleNeed = "TypeTimeScale";
        public static string TypePause = "TypePause";
    }

    public struct MouseControls {
        public const string cmdUpdate = "update";

        public float OldClickLTime;
        public float OldClickRTime;
        public float timeDoubleClick; //Время для двойного клика

        public bool TransformL; //Перемещение активно ли
        public Vector3 PosClickL; //позиция последнего клика
        public bool DoubleClickL; //Был ли двойной клик
        public bool NormalClickL; //Был ли совершен обычный клик
        //public bool InfoView; //Можно ли показывать информамционный текст
        //public float InfoViewPause;

        public bool UserRotate; //Пользовательское вращение
        public float ScrollHeight; //Высота камеры

        public Vector2 OldMousePos;
        public RaycastHit MouseRayHit;

        public CellCTRL SelectCellCTRL;

        public MouseControls(int num) {
            OldClickLTime = 0;
            OldClickRTime = 0;
            timeDoubleClick = 0.15f;

            TransformL = false;
            PosClickL = new Vector3();
            DoubleClickL = false;
            NormalClickL = false;
            //InfoView = false;
            //InfoViewPause = 0;

            UserRotate = false;
            ScrollHeight = 20f;

            //Запоминаем позицию мыши сейчас
            OldMousePos = new Vector2();
            MouseRayHit = new RaycastHit();

            SelectCellCTRL = null;
        }

        public Vector3 getMousePos(Camera MainCam) {
            Vector3 posMouse = new Vector3();

            //получаем луч идущий от экрана
            Ray rayNow = MainCam.ScreenPointToRay(Input.mousePosition);
            //Проверяем луч на столкновение с чем либо
            Physics.Raycast(rayNow, out MouseRayHit, 1000f);
            if (MouseRayHit.point != null) {
                PosClickL = MouseRayHit.point;
            }

            return posMouse;
        }

        public void TransformCam(Camera MainCam) {
            //Перемещаем только если позиция клика ниже позиции мыши //чтобы мышь быстро не меремещала
            if (MainCam.transform.position.y > PosClickL.y + 0.25f) {
                //получаем позицию клика сейчас и раньше
                Ray rayOld = MainCam.ScreenPointToRay(OldMousePos);
                Ray rayNow = MainCam.ScreenPointToRay(Input.mousePosition);

                //Получаем плоскость клика
                Plane mainPlane = new Plane(Vector3.up * -1, new Vector3(PosClickL.x, PosClickL.y, PosClickL.z));

                float enterOld = 0;
                float enterNew = 0;

                Vector3 PosOld = new Vector3();
                Vector3 PosNew = new Vector3();

                if (mainPlane.Raycast(rayOld, out enterOld))
                {
                    PosOld = rayOld.GetPoint(enterOld);
                }

                if (mainPlane.Raycast(rayNow, out enterNew))
                {
                    PosNew = rayNow.GetPoint(enterNew);
                }
                //Debug.Log(enterNew);
                //Если каст удачный
                if (enterNew != 0 && enterOld != 0)
                {
                    //Двигаем
                    MainCam.transform.position += (PosOld - PosNew);
                    //Проверяем выход за границы
                    if (MainCam.transform.position.x > 60) {
                        MainCam.transform.position = new Vector3(60, MainCam.transform.position.y, MainCam.transform.position.z);
                    }
                    else if (MainCam.transform.position.x < 0) {
                        MainCam.transform.position = new Vector3(0, MainCam.transform.position.y, MainCam.transform.position.z);
                    }

                    if (MainCam.transform.position.z > 60) {
                        MainCam.transform.position = new Vector3(MainCam.transform.position.x, MainCam.transform.position.y, 60);
                    }
                    else if (MainCam.transform.position.z < 0) {
                        MainCam.transform.position = new Vector3(MainCam.transform.position.x, MainCam.transform.position.y, 0);
                    }

                }
            }
        }

        public void RotateCam(Camera MainCam, Setings setings)
        {
            if (setings != null && setings.game != null)
            {
                UserRotate = true;

                if (Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.Locked;

                var dx = Input.GetAxis("Mouse X");
                var dy = Input.GetAxis("Mouse Y");

                //меняем угол просмотра
                Quaternion camRot = MainCam.transform.rotation;
                float ViewAngle_y = camRot.eulerAngles.y;
                float ViewAngle_x = camRot.eulerAngles.x;
                ViewAngle_y += dx * setings.game.speedMouse * Time.deltaTime * 100;
                ViewAngle_x -= dy * setings.game.speedMouse * Time.deltaTime * 100;

                //проверяем выход за пределы
                if (ViewAngle_x > 85 && ViewAngle_x < 180)
                    ViewAngle_x = 85;
                else if (ViewAngle_x < 335 && ViewAngle_x > 180)
                    ViewAngle_x = 335;

                camRot.eulerAngles = new Vector3(ViewAngle_x, ViewAngle_y, 0);
                MainCam.transform.rotation = camRot;
            }
        }

        public void NormalClick() {
            //Выполнять только если нормальный клик
            if (NormalClickL) {
                //Последний клик который был. Проверяем что это ячейка и вытаскиваем компонент
                CellCTRL cellCTRL = null;
                if(MouseRayHit.collider != null)
                    cellCTRL = MouseRayHit.collider.GetComponent<CellCTRL>();

                //запоминаем старую ячейку
                CellCTRL oldCellSelect = SelectCellCTRL;

                if (cellCTRL != null)
                {
                    //То выбираем эту ячейку как активную
                    SelectCellCTRL = cellCTRL;
                    SelectCellCTRL.ReDraw();
                    Debug.Log("Select Cell: " + SelectCellCTRL.posX + "|" + SelectCellCTRL.posY);

                    //Отправляем команду серверу чтобы обновил данные ячейки
                    if (Player.me != null)
                        Player.me.CmdBuild(new Vector2(cellCTRL.posX, cellCTRL.posY), MouseControls.cmdUpdate);

                }
                else {
                    SelectCellCTRL = null;
                }

                //Чистим старое
                if (oldCellSelect != null)
                    oldCellSelect.ReDraw();

                NormalClickL = false;
            }
        }
    }
    public MouseControls controlsMouse = new MouseControls(0);
    public struct KeyboardControl {
        float timeMovingCam;
        public KeyboardControl(float none) {
            timeMovingCam = 0;
        }

        public void TestMoveCam(Camera mainCam) {
            if (mainCam != null) {
                //Если хотябы одна кнопка перемещения нажата, увеличиваем скорость
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ||
                    Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ||
                    Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ||
                    Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                    if (timeMovingCam < 2)
                        timeMovingCam = 2;

                    if (me)
                    {
                        //При нажатии кнопки начинаем с минимального рывка
                        float mininum = me.controlsMouse.ScrollHeight;
                        if (timeMovingCam < mininum)
                            timeMovingCam = mininum;

                        timeMovingCam += Time.unscaledDeltaTime * 1 * me.controlsMouse.ScrollHeight;
                    }

                    if (timeMovingCam > 20)
                        timeMovingCam = 20;
                }
                else {
                    timeMovingCam = 0;
                }

                //Перемещение влево
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                    //Сперва надо определить вектор для движения влево
                    Vector3 toLeft = mainCam.transform.right * -1;
                    Vector3 pos = mainCam.transform.position;

                    pos += toLeft * Time.unscaledDeltaTime * timeMovingCam;

                    mainCam.transform.position = pos;
                }
                //Перемещение вправо
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    //Сперва надо определить вектор для движения вправо
                    Vector3 toRight = mainCam.transform.right;
                    Vector3 pos = mainCam.transform.position;

                    pos += toRight * Time.unscaledDeltaTime * timeMovingCam;

                    mainCam.transform.position = pos;
                }
                //Перемещение вверх
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                    //Сперва надо определить вектор для движения вперед
                    Vector3 toForward = mainCam.transform.forward;
                    //Обнуляем вектор Y
                    toForward.y = 0;
                    toForward.Normalize();

                    Vector3 pos = mainCam.transform.position;

                    pos += toForward * Time.unscaledDeltaTime * timeMovingCam;

                    mainCam.transform.position = pos;
                }
                //Перемещение вниз
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    //Сперва надо определить вектор для движения вперед
                    Vector3 toBack = mainCam.transform.forward * -1;
                    //Обнуляем вектор Y
                    toBack.y = 0;
                    toBack.Normalize();

                    Vector3 pos = mainCam.transform.position;

                    pos += toBack * Time.unscaledDeltaTime * timeMovingCam;

                    mainCam.transform.position = pos;
                }

                //Возвращяем камеру на игровое поле если улетели далеко
                float distCamFromCenterNow = Vector2.Distance(new Vector2(mainCam.transform.position.x, mainCam.transform.position.z), new Vector2(30,30));
                float distCamFromCenterMax = 45;
                if (distCamFromCenterNow > distCamFromCenterMax) {
                    float raznica = distCamFromCenterMax - distCamFromCenterNow;
                    Vector2 vecToCenter = new Vector2(30, 30) - new Vector2(mainCam.transform.position.x, mainCam.transform.position.z);
                    vecToCenter.Normalize();
                    mainCam.transform.position -= new Vector3(vecToCenter.x, 0, vecToCenter.y) * raznica * Time.unscaledDeltaTime; 
                }
            }
        }
    }
    public KeyboardControl controlKey = new KeyboardControl(0);

    void testControl() {
        if (isLocalPlayer && mainCamera != null) {
            controlsMouse.OldClickLTime += Time.unscaledDeltaTime;
            controlsMouse.OldClickRTime += Time.unscaledDeltaTime;

            //Если мышь не наведена на интерфейс
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                //Кнопка мыши только что нажалась
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("press mouse 0 not interface");
                    //Получаем позицию клика
                    controlsMouse.getMousePos(mainCamera);
                    controlsMouse.TransformL = true; //Перемещение мыши активно
                    controlsMouse.NormalClickL = true;

                    //Если это дабл клик
                    if (controlsMouse.OldClickLTime < controlsMouse.timeDoubleClick) {
                        controlsMouse.TransformL = false;
                        controlsMouse.DoubleClickL = true;
                        controlsMouse.NormalClickL = false;
                        Debug.Log("press mouse 0 double click");
                    }

                    controlsMouse.OldClickLTime = 0;
                }

                //Если мышь нажата и зажата
                if (Input.GetMouseButton(0))
                {
                    if (controlsMouse.TransformL)
                    {
                        controlsMouse.TransformCam(mainCamera);
                        //Убираем нормальный клик так как это обычное перемещение
                        if (controlsMouse.OldClickLTime > controlsMouse.timeDoubleClick && controlsMouse.NormalClickL) {
                            controlsMouse.NormalClickL = false;
                        }
                    }
                }
                //Если мышь отжата
                else {
                    //Если прошло больше 0.5 секунды и был нормальный клик
                    if (controlsMouse.OldClickLTime > controlsMouse.timeDoubleClick && controlsMouse.NormalClickL) {
                        //Обычный клик
                        Debug.Log("Normal click left mouse");
                        controlsMouse.NormalClick();
                        controlsMouse.NormalClickL = false;
                    }
                    controlsMouse.TransformL = false;
                    controlsMouse.DoubleClickL = false;
                }

                //////////////////////////////////////////////////////////////////
                //Правая кнопка мыши
                if (Input.GetMouseButton(1))
                {
                    //Только что опустилась кнопка
                    if (Input.GetMouseButtonDown(1)) {
                        controlsMouse.OldClickRTime = 0;
                        Debug.Log("press mouse 1 not interface");
                    }

                    if (controlsMouse.OldClickRTime > 0.10f) {
                        controlsMouse.RotateCam(mainCamera, setings);
                    }
                }
                else {
                    Cursor.lockState = CursorLockMode.None;
                }

                //////////////////////////////////////////////////////////////////
                //Нажатие колеса мыши
                if (Input.GetMouseButton(2))
                {
                    Debug.Log("press mouse midle not interface");
                }

                //////////////////////////////////////////////////////////////////
                //Вращение колеса мыши
                if (Input.mouseScrollDelta.y != 0) {
                    //controlsMouse.UserRotate = false; //отмена пользовательского угла
                    //приближение
                    if (Input.mouseScrollDelta.y > 0)
                    {
                        controlsMouse.ScrollHeight -= controlsMouse.ScrollHeight / (Input.mouseScrollDelta.y * 5);
                        if (controlsMouse.ScrollHeight < 1)
                            controlsMouse.ScrollHeight = 1;

                    }
                    //отдаление
                    else {
                        controlsMouse.ScrollHeight += controlsMouse.ScrollHeight / (Input.mouseScrollDelta.y * -5);
                        if (controlsMouse.ScrollHeight > 40)
                            controlsMouse.ScrollHeight = 40;
                    }
                }

                /////////////////////////////////////////////////////////////////////////
                //Получаем позицию мыщи в мире и отправляем серверу
                //получаем луч идущий от экрана
                if (mainCamera != null)
                {
                    Ray rayNow = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit MouseRayHit = new RaycastHit();
                    Vector3 PosMouse = new Vector3();
                    //Проверяем луч на столкновение с чем либо
                    Physics.Raycast(rayNow, out MouseRayHit, 1000f);
                    if (MouseRayHit.point != null)
                    {
                        PosMouse = MouseRayHit.point;
                    }
                    //Уведомляем сервер о позиции курсора
                    CmdSetMouse(false, PosMouse, mainCamera.transform.position);
                }
                //////////////////////////////////////////////////////////////////////////
            }
            else {
                //Уведомляем сервер о позиции курсора
                CmdSetMouse(true, new Vector3(), mainCamera.transform.position);

                //Кнопка мыши только что нажалась
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("press mouse on interface");
                }
            }

            /*
            //Информационное меню
            if (controlsMouse.OldMousePos == new Vector2(Input.mousePosition.x, Input.mousePosition.y) &&
                !controlsMouse.NormalClickL &&
                !controlsMouse.DoubleClickL
                ) {
                controlsMouse.InfoViewPause += Time.unscaledDeltaTime;
                if (controlsMouse.InfoViewPause > 0.25f) {
                    controlsMouse.InfoView = true;
                }
            }
            else {
                controlsMouse.InfoView = false;
                controlsMouse.InfoViewPause = 0;
            }
            */

            //Запоминаем позицию мыши сейчас
            controlsMouse.OldMousePos = Input.mousePosition;

            //Перемещение клавиотурой AWSD и стрелки
            controlKey.TestMoveCam(mainCamera);

            //Синхронизируем с задней камерой
            if(InterierVisualizatorCTRL.main)
                InterierVisualizatorCTRL.main.InterierCamTransform();
        }
    }

    float shakeCam = 0;
    float BaceHealthOld = 0;
    void testCamHeight() {
        if (mainCamera != null)
        {
            if (gameplayCTRL)
            {
                if (gameplayCTRL.gamemode == 2)
                {
                    BaceRocket = null;

                    //необходимо узнать высоту ландшафта
                    float terrainHeight = 1;
                    if (gameplayCTRL != null && gameplayCTRL.cellsData != null)
                    {

                        //Нужно взять высоту из data
                        //Если позиция камеры не больше 60 по ху и не менее 0
                        if (mainCamera.transform.position.x > 0 && mainCamera.transform.position.x < 59 &&
                            mainCamera.transform.position.z > 0 && mainCamera.transform.position.z < 59)
                        {
                            terrainHeight = getTerrainHeight();//gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height;
                        }
                    }

                    if (terrainHeight < 1)
                        terrainHeight = 1;

                    float needHeight = terrainHeight + 0.3f + controlsMouse.ScrollHeight;
                    //делаем нужную высоту
                    if (mainCamera.transform.position.y != needHeight)
                    {
                        if (Time.unscaledDeltaTime < 0.10f)
                            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + ((needHeight - mainCamera.transform.position.y) * Time.unscaledDeltaTime * 10f), mainCamera.transform.position.z);
                        else
                        {
                            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + ((needHeight - mainCamera.transform.position.y) * Time.unscaledDeltaTime * 1f), mainCamera.transform.position.z);
                        }
                    }

                    //Угл наклона камеры
                    if (!controlsMouse.UserRotate)
                    {
                        //Считаем нужный угл
                        float needRotx = 70 - (40 - controlsMouse.ScrollHeight);
                        Quaternion rotCam = mainCamera.transform.rotation;
                        rotCam.eulerAngles = new Vector3(needRotx, rotCam.eulerAngles.y, 0);
                        mainCamera.transform.rotation = rotCam;
                    }
                    //Дрожание камеры от урона
                    TestShakeCam();
                }
                else if (gameplayCTRL.gamemode == 3) {
                    Vector3 posNeed = new Vector3();
                    Quaternion RotNeed = mainCamera.transform.rotation;

                    CalcCamPosNeed();
                    CalcCamRotNeed();

                    if (BaceRocket == null)
                        BaceRocket = GameObject.FindGameObjectWithTag("BaceRosket");

                    //находим требуемое положение камеры
                    void CalcCamPosNeed() {
                        //Узнаем мой номер в списке
                        int myNum = 0;
                        foreach (Player player in gameplayCTRL.players) {
                            if (player == this)
                            {
                                break;
                            }
                            else {
                                myNum++;
                            }
                        }

                        //Считаем смещение x
                        float smeshenieX = (float)(5 * Math.Cos(gameplayCTRL.timeEndScene / 2+ (Math.PI*2*myNum)/gameplayCTRL.players.Count));
                        float smeshenieY = (float)(5 * Math.Sin(gameplayCTRL.timeEndScene / 2+ (Math.PI*2*myNum)/gameplayCTRL.players.Count));

                        //Debug.LogError(gameplayCTRL.timeEndScene);

                        posNeed = new Vector3(gameplayCTRL.HightPoint.x + smeshenieX, gameplayCTRL.cellsData[(int)(gameplayCTRL.HightPoint.x+0.25f), (int)(gameplayCTRL.HightPoint.y+0.25f)].height + 5 + gameplayCTRL.timeEndScene/10f , gameplayCTRL.HightPoint.y + smeshenieY);
                    }
                    //Находим требуемый поворот камеры
                    void CalcCamRotNeed() {
                        if(BaceRocket == null)
                            RotNeed = Quaternion.LookRotation(new Vector3(gameplayCTRL.HightPoint.x, gameplayCTRL.cellsData[(int)gameplayCTRL.HightPoint.x, (int)gameplayCTRL.HightPoint.y].height ,gameplayCTRL.HightPoint.y) - mainCamera.transform.position);
                        else RotNeed = Quaternion.RotateTowards(mainCamera.transform.rotation,Quaternion.LookRotation(BaceRocket.transform.position - mainCamera.transform.position),Time.unscaledDeltaTime * 90);
                    }

                    mainCamera.transform.position += (posNeed - mainCamera.transform.position) * Time.unscaledDeltaTime;
                    mainCamera.transform.rotation = RotNeed;
                }
            }
        }

        float getTerrainHeight() {
            float value = 0;
            if (mainCamera != null && gameplayCTRL != null && gameplayCTRL.cellsData != null) {
                //Из всех позиций берем наибольщее
                float newValue = 0;

                //Этот блок
                value = gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height;

                //Сверху
                if (mainCamera.transform.position.z < 59) {
                    newValue = valueLerp(
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height,
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z + 1].height,
                        mainCamera.transform.position.z % 1);
                }
                if (value < newValue) value = newValue;

                //Слева
                if (mainCamera.transform.position.x >= 1)
                {
                    newValue = valueLerp(
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height,
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x - 1, (int)mainCamera.transform.position.z].height,
                        1 - mainCamera.transform.position.x % 1);
                }
                if (value < newValue) value = newValue;

                //Справа
                if (mainCamera.transform.position.x < 59)
                {
                    newValue = valueLerp(
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height,
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x + 1, (int)mainCamera.transform.position.z].height,
                        mainCamera.transform.position.x % 1);
                }
                if (value < newValue) value = newValue;

                //Снизу
                if (mainCamera.transform.position.z >= 1)
                {
                    newValue = valueLerp(
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height,
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z - 1].height,
                        1 - mainCamera.transform.position.z % 1);
                }
                if (value < newValue) value = newValue;

                ////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Верхо-право
                if (mainCamera.transform.position.x < 59
                    && mainCamera.transform.position.z < 59)
                {
                    Vector2 camVector = new Vector2(mainCamera.transform.position.x % 1, mainCamera.transform.position.z % 1);
                    float lerp = camVector.x;
                    if (lerp > camVector.y) lerp = camVector.y;

                    newValue = valueLerp(
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height,
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x + 1, (int)mainCamera.transform.position.z + 1].height,
                        lerp);
                }
                if (value < newValue) value = newValue;

                //Верхо-лево
                if (mainCamera.transform.position.x >= 1
                    && mainCamera.transform.position.z < 59)
                {
                    Vector2 camVector = new Vector2(1 - mainCamera.transform.position.x % 1, mainCamera.transform.position.z % 1);
                    float lerp = camVector.x;
                    if (lerp > camVector.y) lerp = camVector.y;

                    newValue = valueLerp(
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height,
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x - 1, (int)mainCamera.transform.position.z + 1].height,
                        lerp);
                }
                if (value < newValue) value = newValue;

                //Низо-лево
                if (mainCamera.transform.position.x >= 1
                    && mainCamera.transform.position.z >= 1)
                {
                    Vector2 camVector = new Vector2(1 - mainCamera.transform.position.x % 1, 1 - mainCamera.transform.position.z % 1);
                    float lerp = camVector.x;
                    if (lerp > camVector.y) lerp = camVector.y;

                    newValue = valueLerp(
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height,
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x - 1, (int)mainCamera.transform.position.z - 1].height,
                        lerp);
                }
                if (value < newValue) value = newValue;

                //Низо-право
                if (mainCamera.transform.position.x < 59
                    && mainCamera.transform.position.z >= 1)
                {
                    Vector2 camVector = new Vector2(mainCamera.transform.position.x % 1, 1 - mainCamera.transform.position.z % 1);
                    float lerp = camVector.x;
                    if (lerp > camVector.y) lerp = camVector.y;

                    newValue = valueLerp(
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x, (int)mainCamera.transform.position.z].height,
                        gameplayCTRL.cellsData[(int)mainCamera.transform.position.x + 1, (int)mainCamera.transform.position.z - 1].height,
                        lerp);
                }
                if (value < newValue) value = newValue;
            }

            return value;

            float valueLerp(float StartHeight, float EndHeight, float lerpFrom0to1) {
                float lerp = lerpFrom0to1;
                if (lerpFrom0to1 > 1)
                    lerp = 1;
                else if (lerpFrom0to1 < 0)
                    lerp = 0;

                float valueLocal = StartHeight + (EndHeight - StartHeight) * lerp;

                return valueLocal;
            }
        }
        void TestShakeCam() {
            if (gameplayCTRL.BaceHealth != BaceHealthOld) {
                if (gameplayCTRL.BaceHealth < BaceHealthOld) {
                    shakeCam = 1;
                }

                BaceHealthOld = gameplayCTRL.BaceHealth;

            }
            else if (gameplayCTRL.gamemode == 3 && gameplayCTRL.timeEndScene > 1.5f) {
                shakeCam = 1;
            }

            shakeCam -= Time.unscaledDeltaTime / 3;
            if (shakeCam < 0)
                shakeCam = 0;
            else
            {
                Vector3 randomVec = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

                mainCamera.transform.position += randomVec * shakeCam * 0.1f;
            }
        }
    }

    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    GameObject CameraSlot;

    GameObject BaceRocket;

    /// /////////////////////////////////////////////////////////////////////////////////////////
    /// Тестирование пинга
    /// 1) клинет инициигрует запрос пинга, сохраняет текущее время, отправляет северу команду
    /// 2) сервер получает команду и ининиирует запуск отклика на клиенте
    /// 3) клиент получает обратный отклик и записывает разницу, которую отправляет обратно на сервер
    /// 4) сервер принимает пинг и сохраняет

    //Тестим пинг каждые 10 секунд
    float timeToStartPingTest = 0;
    float timeToTestPing = 0;
    void TestPingRTT() {
        if (isLocalPlayer) {
            timeToTestPing += Time.unscaledDeltaTime;
            if (timeToTestPing > 4)
            {
                timeToTestPing = 0;
                timeToStartPingTest = Time.time;
                CmdStartTestPing();
            }
        }
    }

    [Command]
    void CmdStartTestPing()
    {
        if (isServer)
        {
            RpcEndTestPing();
        }
    }
    [ClientRpc]
    void RpcEndTestPing() {
        float newPingRTT = (Time.time - timeToStartPingTest)*1000;
        CmdSetPing(newPingRTT);
    }
    [SyncVar]
    public float PingRTT = 0;
    [Command]
    void CmdSetPing(float newPing) {
        PingRTT = newPing;
    }
    /// /////////////////////////////////////////////////////////////////////////////////////////

    void IniCamera() {
        if (isLocalPlayer) {
            if (mainCamera == null) {
                GameObject MainCamObj = GameObject.FindGameObjectWithTag("MainCamera");
                if (MainCamObj != null) {
                    mainCamera = MainCamObj.GetComponent<Camera>();

                    //камеру нужно сдвинуть в центр при инициализации
                    MainCamObj.transform.position = new Vector3(0, 40, 0);
                }
            }
        }
    }

    [SerializeField]
    GPResearch research;


    bool sendedNeirodata = false;
    void testEndGameVictory() {
        if(!gameplayCTRL)
            iniGameplayCtrl();

        if (GameplayCTRL.main && GameplayCTRL.main.gamemode == 4 && GameplayCTRL.main.GameoverVictory) {
            if (!research) {
                GameObject researchObj = GameObject.FindGameObjectWithTag("Steam");
                if (researchObj) {
                    research = researchObj.GetComponent<GPResearch>();
                }
            }

            if (research)
            {
                if (!sendedNeirodata)
                {
                    sendedNeirodata = true;
                    research.SetStatValue(GPResearch.Stats.neirodataSTR, GameplayCTRL.main.neirodata);
                }
            }
        }
    }

    public struct MoneyOfTime {
        public float time; //Время получения денег
        public float money; //Количество денег
    }

    [SyncVar]
    public float moneyTime = 0;

    public Chains<MoneyOfTime> ChainsMoney;
    void TestMoneyOfMin() {
        //Только сервер проводит расчет дохода денег в минуту
        if (isServer) {
            //Инициализируем
            if (ChainsMoney == null)
            {
                ChainsMoney = new Chains<MoneyOfTime>();
                moneyTime = 0;
            }
            //Считаем
            else {
                int chainCount = 0;
                float chainMoney = 0;

                Chains<MoneyOfTime>.Chain<MoneyOfTime> next;
                next = ChainsMoney.start;
                bool firstFound = false;
                while (next != null) {
                    //Проверяем время в цепи
                    if (Time.time - 60 < next.data.time) {
                        //Передвигаем цепь на следующее звено если надо
                        if (!firstFound) {
                            firstFound = true;
                            ChainsMoney.start = next;
                        }

                        //Считаем добавляем деньги
                        chainCount++;
                        chainMoney += next.data.money;
                    }

                    //Переключаемся на следующее звено
                    if (next.next != null) {
                        next = next.next;
                    }
                    else {
                        next = null;
                    }
                }

                //Перебор звеньев закончен считаем деньги
                moneyTime = chainMoney;
            }

        }
    }
    public void PlusChainMoney(float money) {
    
    }
}
