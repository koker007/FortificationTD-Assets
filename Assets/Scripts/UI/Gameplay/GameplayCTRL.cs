using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Mirror;

public class GameplayCTRL : NetworkBehaviour
{
    [SerializeField]
    Vector2Int TestCell = new Vector2Int();

    [SerializeField]
    bool TestingMode = false;
    void IniGameplayCTRL() {
        main = this;
    }
    static public GameplayCTRL main;

    public bool ServerIsWorking = false;

    //Режим игры в данный момент
    //0 - ничего
    //1 - меню ожидания игроков
    //2 - игра
    [Header("Server Parametrs")]
    [SyncVar] public ulong lobbyID = 0;
    [SyncVar] public int gamemode = 0;
    [SyncVar] public int gamemodeDinamic = 0;
    int gamemodeOld = 0;

    [SyncVar] public string KeyGen = "";

    [Header("GameMode 1: Waiting")]
    [SyncVar] public float timeToPlay = 60;

    bool CanSpawnFlot = false;

    [Header("GameMode 2: Gameplay")]
    [SyncVar] public float timeGamePlay = 0;
    [SyncVar] public float timeEpicSpawnInfantry = 0;
    [SyncVar] public float timeEpicSpawnAutomobile = 0;
    [SyncVar] public float timeEpicSpawnCrab = 0;
    [SyncVar] public int kills = 0;
    [SyncVar] public float timeScale = 1;
    [SyncVar] public bool pause = false;
    [SerializeField] public float traficLenght = 0;
    [HideInInspector]
    public AICTRL[] aICTRLs = new AICTRL[50000]; //Для хранения ссылок на врага по его ID

    [Header("GameMode 3: EndScene")]
    [SyncVar] public float timeEndScene = 0;

    [SerializeField]
    public GameObject SeaOff; //Море отключаемое при запуске интерьера

    [Header("Buldings")]
    [SerializeField]
    public GameObject enemyShip;
    [SerializeField]
    public GameObject buildEnemyBace;
    [SerializeField]
    public GameObject buildBase;
    [SerializeField]
    public GameObject buildSlope;
    [SerializeField]
    public GameObject buildPlatform;
    [SerializeField]
    public GameObject buildBlockCar;
    [SerializeField]
    public GameObject buildBlockPehota;
    [SerializeField]
    public GameObject buildBlockCrab;
    [SerializeField]
    public GameObject buildPillBox;
    [SerializeField]
    public GameObject buildTurret;
    [SerializeField]
    public GameObject buildArtillery;
    [SerializeField]
    public GameObject buildMinigun;
    [SerializeField]
    public GameObject buildLaser;
    [SerializeField]
    public GameObject buildThunder;
    [SerializeField]
    public GameObject buildRocketLauncher;

    [SerializeField]
    public Color[] EnemyColors;

    //Ячейки земли для отправки
    //public WorldClass.CellsList SyncListCellsNet = new WorldClass.CellsList();
    public readonly SyncList<WorldClass.Cell> SyncListCellsNet = new SyncList<WorldClass.Cell>();
    public WorldClass.Cell[,] cellsData = new WorldClass.Cell[60, 60];
    public CellCTRL[,] cellCTRLs = new CellCTRL[60, 60];
    public Building[,] buildings = new Building[60, 60];

    public int[,] CellsToBace = new int[60, 60];

    //Добавить элемент в лист и сказать клиентам об изменении
    public void addAndCellToSyncList(WorldClass.Cell cellAdd) {
        if (isServer)
        {

            //Если ячеек набралось больше определенного порога, то половину чистим
            if (SyncListCellsNet.Count > 50)
            {
                List<WorldClass.Cell> buffer = new List<WorldClass.Cell>();
                //Запоминаем все что больше половины
                for (int num = SyncListCellsNet.Count / 2; num < SyncListCellsNet.Count; num++)
                {
                    buffer.Add(SyncListCellsNet[num]);
                }

                //Чистим
                SyncListCellsNet.Clear();
                for (int num = 0 / 2; num < buffer.Count; num++)
                {
                    SyncListCellsNet.Add(buffer[num]);
                }
            }

            int index = SyncListCellsNet.IndexOf(cellAdd);

            //Если проверили весь список но так и не нашли значит добавляем
            if (index == -1)
            {
                SyncListCellsNet.Add(cellAdd);
            }
            else
            {
                SyncListCellsNet.Insert(index, cellAdd);
            }
        }
    }

    [SyncVar]
    public Vector2 HightPoint = new Vector2();


    //проиницилизировать карту
    void iniDataWorld() {
        //Создаем карту размером 60 на 60 только если сервер
        if (isServer) {
            if (SyncListCellsNet != null && cellsData != null) {
                for (short x = 0; x < cellsData.GetLength(0); x++) {
                    for (short y = 0; y < cellsData.GetLength(1); y++) {

                        //Создаем ячейку
                        WorldClass.Cell cell = new WorldClass.Cell();
                        cell.posX = (sbyte)x;
                        cell.posY = (sbyte)y;

                        cell.height = (sbyte)(10 * Generator.getHeight(KeyGen, x, y, 60));

                        cell.build = ""; //Никакое строение

                        //вытаскиваем случайного игрока и присваиваем ему эту ячейку
                        int numPlayer = Random.Range(0, players.Count);
                        cell.traficDanger = 1; //вероятность смерти в данной ячейке для бота
                        cell.traficNum = -1;

                        //Проверка высщей точки
                        if (cell.height > cellsData[(int)HightPoint.x, (int)HightPoint.y].height) {
                            HightPoint = new Vector2Int(x, y);
                        }

                        //Занижаем слишком высокую точку
                        if (cell.height > 8)
                            cell.height = 8;

                        //Сохраняем ячейку
                        cellsData[x, y] = cell;

                    }
                }

                //Если высшая точка найдена, строим там базу
                if (buildBase != null) {

                    //Нужно проверить что у высщей точки есть граничашая ячейка для постройки склона
                    //Если нету то понижаем высоту верхней точки
                    if (cellsData[(int)HightPoint.x, (int)HightPoint.y - 1].height != cellsData[(int)HightPoint.x, (int)HightPoint.y].height &&
                        cellsData[(int)HightPoint.x + 1, (int)HightPoint.y].height != cellsData[(int)HightPoint.x, (int)HightPoint.y].height &&
                        cellsData[(int)HightPoint.x - 1, (int)HightPoint.y].height != cellsData[(int)HightPoint.x, (int)HightPoint.y].height &&
                        cellsData[(int)HightPoint.x, (int)HightPoint.y - 1].height != cellsData[(int)HightPoint.x, (int)HightPoint.y].height) {

                        cellsData[(int)HightPoint.x, (int)HightPoint.y].height = cellsData[(int)HightPoint.x, (int)HightPoint.y - 1].height;
                    }

                    GameObject buildBaseObj = Instantiate(buildBase);
                    cellsData[(int)HightPoint.x, (int)HightPoint.y].build = Building.getTypeName(Building.Type.Base);
                    buildBaseObj.transform.position = new Vector3(HightPoint.x, cellsData[(int)HightPoint.x, (int)HightPoint.y].height, HightPoint.y);
                    //Нужно проверить с какой стороны от базы есть свободное пространство

                    if (cellsData[(int)HightPoint.x, (int)HightPoint.y - 1].height == cellsData[(int)HightPoint.x, (int)HightPoint.y].height) {
                        cellsData[(int)HightPoint.x, (int)HightPoint.y].note = Building.getRotName(Building.Rotate.Down);
                        BaceCTRL buildBace = buildBaseObj.GetComponent<BaceCTRL>();
                        if (buildBace != null && buildBace.MainBaceObj != null) {
                            buildBace.MainBaceObj.transform.Rotate(0, 180, 0);
                        }
                    }
                    else if (cellsData[(int)HightPoint.x + 1, (int)HightPoint.y].height == cellsData[(int)HightPoint.x, (int)HightPoint.y].height) {
                        cellsData[(int)HightPoint.x, (int)HightPoint.y].note = Building.getRotName(Building.Rotate.Left);
                        BaceCTRL buildBace = buildBaseObj.GetComponent<BaceCTRL>();
                        if (buildBace != null && buildBace.MainBaceObj != null) {
                            buildBace.MainBaceObj.transform.Rotate(0, 90, 0);
                        }
                    }
                    else if (cellsData[(int)HightPoint.x - 1, (int)HightPoint.y].height == cellsData[(int)HightPoint.x, (int)HightPoint.y].height) {
                        cellsData[(int)HightPoint.x, (int)HightPoint.y].note = Building.getRotName(Building.Rotate.Right);
                        BaceCTRL buildBace = buildBaseObj.GetComponent<BaceCTRL>();
                        if (buildBace != null && buildBace.MainBaceObj != null) {
                            buildBace.MainBaceObj.transform.Rotate(0, -90, 0);
                        }
                    }
                    else if (cellsData[(int)HightPoint.x, (int)HightPoint.y + 1].height == cellsData[(int)HightPoint.x, (int)HightPoint.y].height) {
                        cellsData[(int)HightPoint.x, (int)HightPoint.y].note = Building.getRotName(Building.Rotate.Up);
                        BaceCTRL buildBace = buildBaseObj.GetComponent<BaceCTRL>();
                        if (buildBace != null && buildBace.MainBaceObj != null) {
                            buildBace.MainBaceObj.transform.Rotate(0, 0, 0);
                        }
                    }

                    NetworkServer.Spawn(buildBaseObj);
                }

                //База построена, строем маршрут до базы с построением склонов
                CalcTraficAll(true, new Vector2Int(0, 0), true);

                //Маршрут построен теперь в лист синхронизации добавляем все ячейки с текущими данными
                for (short x = 0; x < cellsData.GetLength(0); x++) {
                    for (short y = 0; y < cellsData.GetLength(1); y++) {
                        addAndCellToSyncList(cellsData[x,y]);
                    }
                }
            }
        }
    }

    //Проверка не сломается ли маршрут после строительства на указанной клетке
    public bool CanBuildThisPos(short posX, short posY) {
        //Изначально строить нельзя
        bool canBuild = false;

        //создаем временное хранилище маршрута
        int[,] traficNum = new int[60, 60];
        for (int x = 0; x < 59; x++) {
            for (int y = 0; y < 59; y++) {
                traficNum[x, y] = -1;
            }
        }

        //если точка спавна базы есть
        if (HightPoint != new Vector2Int()) {
            //Для хранения ячеек
            List<WorldClass.Cell> Next = new List<WorldClass.Cell>();
            List<WorldClass.Cell> Now = new List<WorldClass.Cell>();
            List<WorldClass.Cell> Slope = new List<WorldClass.Cell>();

            //Начинаем построение маршрута с базы
            Now.Add(cellsData[(int)HightPoint.x, (int)HightPoint.y]);

            //Трафик равер нулю
            int traficNow = 0;

            //Перебор высот пока не опустимся
            for (int heightNow = cellsData[(int)HightPoint.x, (int)HightPoint.y].height; heightNow >= 0;) {
                //Обнуляем следующие ячейки
                Next = new List<WorldClass.Cell>();
                //Перебираем текущие ячейки
                foreach (WorldClass.Cell cNow in Now) {
                    //Проверяем ячейки
                    //Если это ячейка базы
                    if (cNow.build == Building.getTypeName(Building.Type.Base)) {
                        //Добавляем слот в зависимости от повернутости базы

                        //вниз
                        if (cNow.posY > 0 && cNow.note == Building.getRotName(Building.Rotate.Down) && cellsData[cNow.posX, cNow.posY - 1].build == "" && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX, cNow.posY - 1) && cellsData[cNow.posX, cNow.posY - 1].height == cNow.height) {
                            Next.Add(cellsData[cNow.posX, cNow.posY - 1]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Влево
                        else if (cNow.posX > 0 && cNow.note == Building.getRotName(Building.Rotate.Left) && cellsData[cNow.posX - 1, cNow.posY].build == "" && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX - 1, cNow.posY) && cellsData[cNow.posX - 1, cNow.posY].height == cNow.height) {
                            Next.Add(cellsData[cNow.posX - 1, cNow.posY]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Вправо
                        else if (cNow.posX < 59 && cNow.note == Building.getRotName(Building.Rotate.Right) && cellsData[cNow.posX + 1, cNow.posY].build == "" && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX + 1, cNow.posY) && cellsData[cNow.posX + 1, cNow.posY].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX + 1, cNow.posY]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Вверх
                        else if (cNow.posY < 59 && cNow.note == Building.getRotName(Building.Rotate.Up) && cellsData[cNow.posX, cNow.posY].build == "" && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX, cNow.posY + 1) && cellsData[cNow.posX, cNow.posY + 1].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX, cNow.posY + 1]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                    }
                    //Если это склон, добавляем ячейку только в направлении склона
                    else if (cNow.build == Building.getTypeName(Building.Type.Base)) {
                        //Сперва проверяем на ячейки тойже высоты
                        //вниз
                        if (cNow.posY > 0 && cNow.note == Building.getRotName(Building.Rotate.Down) && cellsData[cNow.posX, cNow.posY - 1].build == "" && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX, cNow.posY - 1) && cellsData[cNow.posX, cNow.posY - 1].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX, cNow.posY - 1]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Влево
                        else if (cNow.posX > 0 && cNow.note == Building.getRotName(Building.Rotate.Left) && cellsData[cNow.posX - 1, cNow.posY].build == "" && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX - 1, cNow.posY) && cellsData[cNow.posX - 1, cNow.posY].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX - 1, cNow.posY]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Вправо
                        else if (cNow.posX < 59 && cNow.note == Building.getRotName(Building.Rotate.Right) && cellsData[cNow.posX + 1, cNow.posY].build == "" && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX + 1, cNow.posY) && cellsData[cNow.posX + 1, cNow.posY].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX + 1, cNow.posY]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Вверх
                        else if (cNow.posY < 59 && cNow.note == Building.getRotName(Building.Rotate.Up) && cellsData[cNow.posX, cNow.posY + 1].build == "" && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX, cNow.posY + 1) && cellsData[cNow.posX, cNow.posY + 1].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX, cNow.posY + 1]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }

                        //Если на тойже высоте не обнаружилось то проверяем на спуск по ниже
                        //вниз
                        if (cNow.posY > 0 && cNow.note == Building.getRotName(Building.Rotate.Down) && cellsData[cNow.posX, cNow.posY - 1].build == Building.getTypeName(Building.Type.Slope) && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX, cNow.posY - 1) && cellsData[cNow.posX, cNow.posY - 1].height + 1 == cNow.height)
                        {
                            Slope.Add(cellsData[cNow.posX, cNow.posY - 1]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Влево
                        else if (cNow.posX > 0 && cNow.note == Building.getRotName(Building.Rotate.Left) && cellsData[cNow.posX - 1, cNow.posY].build == Building.getTypeName(Building.Type.Slope) && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX - 1, cNow.posY) && cellsData[cNow.posX - 1, cNow.posY].height + 1 == cNow.height)
                        {
                            Slope.Add(cellsData[cNow.posX - 1, cNow.posY]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Вправо
                        else if (cNow.posX < 59 && cNow.note == Building.getRotName(Building.Rotate.Right) && cellsData[cNow.posX + 1, cNow.posY].build == Building.getTypeName(Building.Type.Slope) && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX + 1, cNow.posY) && cellsData[cNow.posX + 1, cNow.posY].height + 1 == cNow.height)
                        {
                            Slope.Add(cellsData[cNow.posX + 1, cNow.posY]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                        //Вверх
                        else if (cNow.posY < 59 && cNow.note == Building.getRotName(Building.Rotate.Up) && cellsData[cNow.posX, cNow.posY + 1].build == Building.getTypeName(Building.Type.Slope) && new Vector2Int(posX, posY) != new Vector2Int(cNow.posX, cNow.posY + 1) && cellsData[cNow.posX, cNow.posY + 1].height + 1 == cNow.height)
                        {
                            Slope.Add(cellsData[cNow.posX, cNow.posY + 1]);
                            traficNum[cNow.posX, cNow.posY] = traficNow + 1;
                        }
                    }

                    //Если это пустая ячейка
                    else if (cNow.build == Building.getTypeName(Building.Type.None)) {
                        //Проверяем
                        //Снизу
                        if (cNow.posY > 0 && traficNum[cNow.posX, cNow.posY - 1] == -1 && cellsData[cNow.posX, cNow.posY - 1].build == "" && cellsData[cNow.posX, cNow.posY - 1].height == cNow.height) {
                            Next.Add(cellsData[cNow.posX, cNow.posY - 1]);
                            traficNum[cNow.posX, cNow.posY - 1] = traficNum[cNow.posX, cNow.posY] + 1;
                        }
                        //слева
                        if (cNow.posX > 0 && traficNum[cNow.posX - 1, cNow.posY] == -1 && cellsData[cNow.posX - 1, cNow.posY].build == "" && cellsData[cNow.posX - 1, cNow.posY].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX - 1, cNow.posY]);
                            traficNum[cNow.posX - 1, cNow.posY] = traficNum[cNow.posX, cNow.posY] + 1;
                        }
                        //справа
                        if (cNow.posX < 59 && traficNum[cNow.posX + 1, cNow.posY] == -1 && cellsData[cNow.posX + 1, cNow.posY].build == "" && cellsData[cNow.posX + 1, cNow.posY].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX + 1, cNow.posY]);
                            traficNum[cNow.posX + 1, cNow.posY] = traficNum[cNow.posX, cNow.posY] + 1;
                        }
                        //сверху
                        if (cNow.posY < 59 && traficNum[cNow.posX, cNow.posY + 1] == -1 && cellsData[cNow.posX, cNow.posY + 1].build == "" && cellsData[cNow.posX, cNow.posY + 1].height == cNow.height)
                        {
                            Next.Add(cellsData[cNow.posX, cNow.posY + 1]);
                            traficNum[cNow.posX, cNow.posY + 1] = traficNum[cNow.posX, cNow.posY] + 1;
                        }
                    }

                    //проверка ячейки на наличие спуска рядом, если есть спуск на высоте пониже, с правильным поворотом
                    //Сверху
                    if (cNow.posY > 0 && cellsData[cNow.posX, cNow.posY - 1].height + 1 == cNow.height && cellsData[cNow.posX, cNow.posY - 1].build == Building.getTypeName(Building.Type.Slope) && cellsData[cNow.posX, cNow.posY - 1].note == Building.getRotName(Building.Rotate.Down)) {
                        Slope.Add(cellsData[cNow.posX, cNow.posY - 1]);
                        traficNum[cNow.posX, cNow.posY - 1] = traficNum[cNow.posX, cNow.posY] + 1;
                    }
                    //Слева
                    if (cNow.posX > 0 && cellsData[cNow.posX - 1, cNow.posY].height + 1 == cNow.height && cellsData[cNow.posX - 1, cNow.posY].build == Building.getTypeName(Building.Type.Slope) && cellsData[cNow.posX - 1, cNow.posY].note == Building.getRotName(Building.Rotate.Left)) {
                        Slope.Add(cellsData[cNow.posX - 1, cNow.posY]);
                        traficNum[cNow.posX - 1, cNow.posY] = traficNum[cNow.posX, cNow.posY] + 1;
                    }
                    //Справа
                    if (cNow.posY > 0 && cellsData[cNow.posX + 1, cNow.posY].height + 1 == cNow.height && cellsData[cNow.posX + 1, cNow.posY].build == Building.getTypeName(Building.Type.Slope) && cellsData[cNow.posX + 1, cNow.posY].note == Building.getRotName(Building.Rotate.Right))
                    {
                        Slope.Add(cellsData[cNow.posX + 1, cNow.posY]);
                        traficNum[cNow.posX + 1, cNow.posY] = traficNum[cNow.posX, cNow.posY] + 1;
                    }
                    //Снизу
                    if (cNow.posX > 0 && cellsData[cNow.posX, cNow.posY + 1].height + 1 == cNow.height && cellsData[cNow.posX, cNow.posY + 1].build == Building.getTypeName(Building.Type.Slope) && cellsData[cNow.posX, cNow.posY + 1].note == Building.getRotName(Building.Rotate.Up))
                    {
                        Slope.Add(cellsData[cNow.posX, cNow.posY + 1]);
                        traficNum[cNow.posX, cNow.posY + 1] = traficNum[cNow.posX, cNow.posY] + 1;
                    }
                }

                //Все ячейки проверены
                //Если следующие ячейки на текущей высоте есть
                if (Next.Count > 0) {
                    Now = Next;
                }
                //Иначе переходим на другой склон
                else {
                    Now = Slope;
                    heightNow--;
                }
            }

        }

        return canBuild;
    }

    List<WorldClass.Cell>[] CellsStartTrafic = new List<WorldClass.Cell>[10];
    //просчитать маршрут для всей карты
    float[,,] traficNumTypeBuffer = new float[60, 60, 3];
    float[,,] traficNumType = new float[60, 60, 3];
    short[,] traficChange = new short[60, 60];
    public int[,] traficDistToBace = new int[60, 60];
    public bool CalcTraficType(bool BuildingSlope, Vector2Int ignorePos, bool acceptNewTrafic, int traficType) {
        //Был ли успешно построен маршрут
        bool CalcOk = false;

        int[,] traficDistToBaceBuffer = new int[60,60];

        //Обнуляем временный трафик
        for (int x = 0; x < 60; x++) {
            for (int y = 0; y < 60; y++) {
                traficNumTypeBuffer[x, y, traficType] = 9999999;
                traficChange[x, y] = 1;
                traficDistToBaceBuffer[x, y] = 9999999;
            }
        }

        if (HightPoint != null) {
            //Инициализируем все листы исходных позиций для каждых высот
            for (short height = 0; height < CellsStartTrafic.Length; height++) {
                //Нужно создать листы для точек
                CellsStartTrafic[height] = new List<WorldClass.Cell>();
            }

            //Создаем список для хранения предыдущих ячеек
            List<WorldClass.Cell> testCellNow = new List<WorldClass.Cell>();
            List<WorldClass.Cell> testCellBreak = new List<WorldClass.Cell>(); //Плавные переходы
            List<WorldClass.Cell> testCellBreak2 = new List<WorldClass.Cell>(); //Резкие переходы
            List<WorldClass.Cell> testCellCanExit = new List<WorldClass.Cell>(); //Возможность прохода
            List<WorldClass.Cell> testCellStartCanyon = new List<WorldClass.Cell>(); //Ячейки для старта просчета каньена из ямы.
            List<WorldClass.Cell> testCellSeaNow = new List<WorldClass.Cell>(); //Ячейки берега
            List<WorldClass.Cell> testCellSea = new List<WorldClass.Cell>();
            traficNumTypeBuffer[(int)HightPoint.x, (int)HightPoint.y, traficType] = 0;             //Делаем базу стартовой ячейкой
            traficDistToBaceBuffer[(int)HightPoint.x, (int)HightPoint.y] = 0;
            testCellNow.Add(cellsData[(int)HightPoint.x, (int)HightPoint.y]);        //запоминаем в списке ячеек

            //Нужно проверить все ячейки высоты начиная с высоты основной базы
            for (short heightNow = cellsData[(int)HightPoint.x, (int)HightPoint.y].height; heightNow > 0;) {
                //Список ячеек добавленных сейчас
                List<WorldClass.Cell> testCellNext = new List<WorldClass.Cell>();

                bool foundSlope = false;
                //проверяем старые ячейки на то что граничат ли они с другими
                foreach (WorldClass.Cell c in testCellNow) {
                    //Только для теста// если соседняя ячейка игнорируется
                    if (new Vector2Int(c.posX, c.posY - 1) == ignorePos ||
                        new Vector2Int(c.posX, c.posY + 1) == ignorePos ||
                        new Vector2Int(c.posX - 1, c.posY) == ignorePos ||
                        new Vector2Int(c.posX + 1, c.posY) == ignorePos) {
                        float test = 0;
                    }

                    //Если ячейка база
                    if (c.build == Building.getTypeName(Building.Type.Base))
                    {
                        //Проверка спереди, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (cellsData[c.posX, c.posY].note == Building.getRotName(Building.Rotate.Down) &&
                            c.posY != 0 && cellsData[c.posX, c.posY - 1].height == cellsData[c.posX, c.posY].height &&
                            traficNumTypeBuffer[c.posX, c.posY, traficType] < traficNumTypeBuffer[c.posX, c.posY - 1, traficType]
                            && new Vector2Int(c.posX, c.posY - 1) != ignorePos
                            && traficChange[c.posX, c.posY - 1] > 0
                            )
                        {
                            traficChange[c.posX, c.posY - 1]--;
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX, c.posY - 1, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX, c.posY - 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX, c.posY - 1]);
                        }
                        //Проверка сзади, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (cellsData[c.posX, c.posY].note == Building.getRotName(Building.Rotate.Up) &&
                            c.posY < 60 && cellsData[c.posX, c.posY + 1].height == cellsData[c.posX, c.posY].height &&
                            traficNumTypeBuffer[c.posX, c.posY, traficType] < traficNumTypeBuffer[c.posX, c.posY + 1, traficType]
                            && new Vector2Int(c.posX, c.posY + 1) != ignorePos
                            && traficChange[c.posX, c.posY + 1] > 0
                            )
                        {
                            traficChange[c.posX, c.posY + 1]--;
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX, c.posY + 1, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX, c.posY + 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX, c.posY + 1]);
                        }

                        //Проверка справа, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (cellsData[c.posX, c.posY].note == Building.getRotName(Building.Rotate.Left) &&
                            c.posX < 60 && cellsData[c.posX + 1, c.posY].height == cellsData[c.posX, c.posY].height &&
                            traficNumTypeBuffer[c.posX, c.posY, traficType] < traficNumTypeBuffer[c.posX + 1, c.posY, traficType]
                            && new Vector2Int(c.posX + 1, c.posY) != ignorePos
                            && traficChange[c.posX + 1, c.posY] > 0
                            )
                        {
                            traficChange[c.posX + 1, c.posY]--;
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX + 1, c.posY, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX + 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX + 1, c.posY]);
                        }

                        //Проверка слева, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (cellsData[c.posX, c.posY].note == Building.getRotName(Building.Rotate.Right) &&
                            c.posX > 0 && cellsData[c.posX - 1, c.posY].height == cellsData[c.posX, c.posY].height &&
                            traficNumTypeBuffer[c.posX, c.posY, traficType] < traficNumTypeBuffer[c.posX - 1, c.posY, traficType]
                            && new Vector2Int(c.posX - 1, c.posY) != ignorePos
                            && traficChange[c.posX - 1, c.posY] > 0
                            )
                        {
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX - 1, c.posY, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX - 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX - 1, c.posY]);
                        }
                    }
                    //Если на проверяемой ячейке нет строения
                    else if (c.build == "" || (c.build == Building.getTypeName(Building.Type.TraficBlocker))) {
                        //Проверка спереди, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (c.posY != 0 && cellsData[c.posX, c.posY - 1].height == cellsData[c.posX, c.posY].height &&
                            traficNumTypeBuffer[c.posX, c.posY, traficType] < traficNumTypeBuffer[c.posX, c.posY - 1, traficType]
                            && new Vector2Int(c.posX, c.posY - 1) != ignorePos
                            && traficChange[c.posX, c.posY - 1] > 0
                            && NotBlockTrafic(cellsData[c.posX, c.posY - 1])) {
                            traficChange[c.posX, c.posY - 1]--;
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX, c.posY - 1, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX, c.posY - 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX, c.posY - 1]);
                        }
                        //Проверка сзади, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (c.posY < 60 && cellsData[c.posX, c.posY + 1].height == cellsData[c.posX, c.posY].height &&
                            traficNumTypeBuffer[c.posX, c.posY, traficType] < traficNumTypeBuffer[c.posX, c.posY + 1, traficType]
                            && new Vector2Int(c.posX, c.posY + 1) != ignorePos
                            && traficChange[c.posX, c.posY + 1] > 0
                            && NotBlockTrafic(cellsData[c.posX, c.posY + 1])) {
                            traficChange[c.posX, c.posY + 1]--;
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX, c.posY + 1, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX, c.posY+1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX, c.posY + 1]);
                        }

                        //Проверка справа, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (c.posX < 60 && cellsData[c.posX + 1, c.posY].height == cellsData[c.posX, c.posY].height &&
                            traficNumTypeBuffer[c.posX, c.posY, traficType] < traficNumTypeBuffer[c.posX + 1, c.posY, traficType]
                            && new Vector2Int(c.posX + 1, c.posY) != ignorePos
                            && traficChange[c.posX + 1, c.posY] > 0
                            && NotBlockTrafic(cellsData[c.posX + 1, c.posY])) {
                            traficChange[c.posX + 1, c.posY]--;
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX + 1, c.posY, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX + 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX + 1, c.posY]);
                        }

                        //Проверка слева, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (c.posX > 0 && cellsData[c.posX - 1, c.posY].height == cellsData[c.posX, c.posY].height &&
                            traficNumTypeBuffer[c.posX, c.posY, traficType] < traficNumTypeBuffer[c.posX - 1, c.posY, traficType]
                            && new Vector2Int(c.posX - 1, c.posY) != ignorePos
                            && traficChange[c.posX - 1, c.posY] > 0
                            && NotBlockTrafic(cellsData[c.posX - 1, c.posY])) {
                            traficChange[c.posX - 1, c.posY]--;
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX - 1, c.posY, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX - 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX - 1, c.posY]);
                        }

                        bool NotBlockTrafic(WorldClass.Cell testBlockCell) {
                            bool notBlock = true;

                            //Если ячейка является выборочным блокиратором пути
                            if (testBlockCell.build == Building.getTypeName(Building.Type.TraficBlocker)) {
                                //Ищем строение в списке строений
                                if (buildings[testBlockCell.posX, testBlockCell.posY] != null
                                    && buildings[testBlockCell.posX, testBlockCell.posY].traficBlocker[traficType]) {

                                    notBlock = false;
                                }
                            }

                            return notBlock;
                        }
                    }
                    //Если данная ячейка склон
                    else if (c.build == Building.getTypeName(Building.Type.Slope)) {
                        //то берем следуюшую ячейку по направлению склона

                        if ((c.note == Building.getRotName(Building.Rotate.Down) &&
                            cellsData[c.posX, c.posY].height == cellsData[c.posX, c.posY - 1].height)// || c.height - 1 == cellsData[c.posX, c.posY - 1].height && cellsData[c.posX, c.posY - 1].build == Building.getTypeName(Building.Type.Slope))
                            && new Vector2Int(c.posX, c.posY - 1) != ignorePos
                            && traficChange[c.posX, c.posY - 1] > 0) {
                            traficNumTypeBuffer[c.posX, c.posY - 1, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX, c.posY - 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX, c.posY - 1]);
                        }
                        else if ((c.note == Building.getRotName(Building.Rotate.Left) &&
                            cellsData[c.posX, c.posY].height == cellsData[c.posX - 1, c.posY].height)// || c.height - 1 == cellsData[c.posX - 1, c.posY].height && cellsData[c.posX - 1, c.posY].build == Building.getTypeName(Building.Type.Slope))
                            && new Vector2Int(c.posX - 1, c.posY) != ignorePos
                            && traficChange[c.posX - 1, c.posY] > 0) {
                            traficNumTypeBuffer[c.posX - 1, c.posY, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX - 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX - 1, c.posY]);
                        }
                        else if ((c.note == Building.getRotName(Building.Rotate.Right) &&
                            cellsData[c.posX, c.posY].height == cellsData[c.posX + 1, c.posY].height)// || c.height - 1 == cellsData[c.posX + 1, c.posY].height && cellsData[c.posX + 1, c.posY].build == Building.getTypeName(Building.Type.Slope))
                            && new Vector2Int(c.posX + 1, c.posY) != ignorePos
                            && traficChange[c.posX + 1, c.posY] > 0) {
                            traficNumTypeBuffer[c.posX + 1, c.posY, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX + 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX + 1, c.posY]);
                        }
                        else if ((c.note == Building.getRotName(Building.Rotate.Up) &&
                            cellsData[c.posX, c.posY].height == cellsData[c.posX, c.posY + 1].height)// || c.height - 1 == cellsData[c.posX, c.posY + 1].height && cellsData[c.posX, c.posY + 1].build == Building.getTypeName(Building.Type.Slope))
                            && new Vector2Int(c.posX, c.posY + 1) != ignorePos
                            && traficChange[c.posX, c.posY + 1] > 0) {
                            traficNumTypeBuffer[c.posX, c.posY + 1, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1 * cellsData[c.posX, c.posY].traficDanger);
                            traficDistToBaceBuffer[c.posX, c.posY + 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                            testCellNext.Add(cellsData[c.posX, c.posY + 1]);
                        }
                    }



                    if (c.build == Building.getTypeName(Building.Type.Slope) || c.build == "") {
                        //Проверка на плавный обрыв
                        if (testCellBreak != null) {
                            //Если сбоку есть место под строительство лестницы, запоминаем эту ячейку
                            if (BuildSlopeDown(c.posX, c.posY, false, ignorePos) != new Vector2Int()) {
                                testCellBreak.Add(cellsData[c.posX, c.posY]);
                            }
                        }
                        //Проверка на резкий обрыв
                        if (testCellBreak2 != null) {
                            if (BuildSlopeDown2(c.posX, c.posY, false) != new Vector2Int()) {
                                testCellBreak2.Add(cellsData[c.posX, c.posY]);
                            }
                        }
                        //Проверка на прорыв из ямы
                        if (testCellCanExit != null) {
                            if (PitWithNoExit(c.posX, c.posY, false) != new Vector2Int()) {
                                if (c.posX == 23 && c.posY == 23)
                                {
                                    float nup = 0;
                                }
                                testCellCanExit.Add(cellsData[c.posX, c.posY]);
                            }
                        }
                        //Проверка на каньен (добавление в список потенциальных ячеек)
                        if (TestStartCanyon(cellsData[c.posX, c.posY])) {
                            testCellStartCanyon.Add(cellsData[c.posX, c.posY]);
                        }
                        //Проверка на берег
                        if (testCellSea != null) {
                            if (IniEndCell(c.posX, c.posY, false, traficType)) {
                                testCellSea.Add(cellsData[c.posX, c.posY]);
                            }
                        }
                        //проверка на берег 2
                        if (testCellSeaNow != null) {
                            //Если это берег то он будет добавлен
                            AddShore(c);
                        }
                    }

                    //Если следующая точка на текущей высоте не была обнаружена
                    //Ищем существующие спуски
                    if (testCellNext.Count == 0) {
                        //Проверяем запомнятые ячейки на наличие склона
                        if (testCellBreak != null) {
                            foreach (WorldClass.Cell cellBreak in testCellBreak) {
                                //Если сбоку на уровень ниже и там построен склон, который повернут правильно
                                //если склон был найден то эта ячейка добавляется в лист следующей ступени, и общий уровень понижается на 1

                                //снизу
                                if (cellBreak.height == (cellsData[cellBreak.posX, cellBreak.posY - 1].height + 1) &&
                                    cellsData[cellBreak.posX, cellBreak.posY - 1].build == Building.getTypeName(Building.Type.Slope) &&
                                    cellsData[cellBreak.posX, cellBreak.posY - 1].note == Building.getRotName(Building.Rotate.Down)) {
                                    traficNumTypeBuffer[cellBreak.posX, cellBreak.posY - 1, traficType] = (short)(traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType] + 1);
                                    traficDistToBaceBuffer[c.posX, c.posY - 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[cellBreak.posX, cellBreak.posY - 1]);
                                    foundSlope = true;
                                }
                                //Слева
                                else if (cellBreak.height == (cellsData[cellBreak.posX - 1, cellBreak.posY].height + 1) &&
                                    cellsData[cellBreak.posX - 1, cellBreak.posY].build == Building.getTypeName(Building.Type.Slope) &&
                                    cellsData[cellBreak.posX - 1, cellBreak.posY].note == Building.getRotName(Building.Rotate.Left)) {
                                    traficNumTypeBuffer[cellBreak.posX - 1, cellBreak.posY, traficType] = (short)(traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType] + 1);
                                    traficDistToBaceBuffer[c.posX - 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[cellBreak.posX - 1, cellBreak.posY]);
                                    foundSlope = true;
                                }
                                //справа
                                else if (cellBreak.height == (cellsData[cellBreak.posX + 1, cellBreak.posY].height + 1) &&
                                    cellsData[cellBreak.posX + 1, cellBreak.posY].build == Building.getTypeName(Building.Type.Slope) &&
                                    cellsData[cellBreak.posX + 1, cellBreak.posY].note == Building.getRotName(Building.Rotate.Right)) {
                                    traficNumTypeBuffer[cellBreak.posX + 1, cellBreak.posY, traficType] = (short)(traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType] + 1);
                                    traficDistToBaceBuffer[c.posX + 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[cellBreak.posX + 1, cellBreak.posY]);
                                    foundSlope = true;
                                }
                                //вперед
                                else if (cellBreak.height == (cellsData[cellBreak.posX, cellBreak.posY + 1].height + 1) &&
                                    cellsData[cellBreak.posX, cellBreak.posY + 1].build == Building.getTypeName(Building.Type.Slope) &&
                                    cellsData[cellBreak.posX, cellBreak.posY + 1].note == Building.getRotName(Building.Rotate.Up)) {
                                    traficNumTypeBuffer[cellBreak.posX, cellBreak.posY + 1, traficType] = (short)(traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType] + 1);
                                    traficDistToBaceBuffer[c.posX, c.posY + 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[cellBreak.posX, cellBreak.posY + 1]);
                                    foundSlope = true;
                                }
                            }
                        }
                        //Если спуск все еще не был обнаружен, ищем в ячейках крутого спуска
                        if (testCellNext.Count == 0 && testCellBreak2 != null) {
                            foreach (WorldClass.Cell cellBreak in testCellBreak2)
                            {
                                //Если сбоку на уровень ниже и там построен склон, который повернут правильно
                                //если склон был найден то эта ячейка добавляется в лист следующей ступени, и общий уровень понижается на 1

                                //снизу
                                if (cellBreak.height == (cellsData[cellBreak.posX, cellBreak.posY - 1].height + 1) &&
                                    cellsData[cellBreak.posX, cellBreak.posY - 1].build == Building.getTypeName(Building.Type.Slope) &&
                                    cellsData[cellBreak.posX, cellBreak.posY - 1].note == Building.getRotName(Building.Rotate.Down))
                                {
                                    traficNumTypeBuffer[cellBreak.posX, cellBreak.posY - 1, traficType] = (short)(traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType] + 1);
                                    traficDistToBaceBuffer[c.posX, c.posY - 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[cellBreak.posX, cellBreak.posY - 1]);
                                    foundSlope = true;
                                }
                                //Слева
                                else if (cellBreak.height == (cellsData[cellBreak.posX - 1, cellBreak.posY].height + 1) &&
                                    cellsData[cellBreak.posX - 1, cellBreak.posY].build == Building.getTypeName(Building.Type.Slope) &&
                                    cellsData[cellBreak.posX - 1, cellBreak.posY].note == Building.getRotName(Building.Rotate.Left))
                                {
                                    traficNumTypeBuffer[cellBreak.posX - 1, cellBreak.posY, traficType] = (short)(traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType] + 1);
                                    traficDistToBaceBuffer[c.posX - 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[cellBreak.posX - 1, cellBreak.posY]);
                                    foundSlope = true;
                                }
                                //справа
                                else if (cellBreak.height == (cellsData[cellBreak.posX + 1, cellBreak.posY].height + 1) &&
                                    cellsData[cellBreak.posX + 1, cellBreak.posY].build == Building.getTypeName(Building.Type.Slope) &&
                                    cellsData[cellBreak.posX + 1, cellBreak.posY].note == Building.getRotName(Building.Rotate.Right))
                                {
                                    traficNumTypeBuffer[cellBreak.posX + 1, cellBreak.posY, traficType] = (short)(traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType] + 1);
                                    traficDistToBaceBuffer[c.posX + 1, c.posY] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[cellBreak.posX + 1, cellBreak.posY]);
                                    foundSlope = true;
                                }
                                //вперед
                                else if (cellBreak.height == (cellsData[cellBreak.posX, cellBreak.posY + 1].height + 1) &&
                                    cellsData[cellBreak.posX, cellBreak.posY + 1].build == Building.getTypeName(Building.Type.Slope) &&
                                    cellsData[cellBreak.posX, cellBreak.posY + 1].note == Building.getRotName(Building.Rotate.Up))
                                {
                                    traficNumTypeBuffer[cellBreak.posX, cellBreak.posY + 1, traficType] = (short)(traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType] + 1);
                                    traficDistToBaceBuffer[c.posX, c.posY + 1] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[cellBreak.posX, cellBreak.posY + 1]);
                                    foundSlope = true;
                                }
                            }
                        }
                    }
                }


                //Если следующих ячеек нету, то это последняя ячейка на текущей высоте
                if (testCellNext.Count == 0) {
                    bool notLastCellThisHeight = false;

                    bool BuildSlope1 = false;
                    bool BuildSlope2 = false;
                    bool BuildCanExit = false;
                    bool BuildCanyon = false;
                    bool BuildSea = false;
                    bool buildSea2 = false;

                    //То ячейки которые остались - последние, строим спуск
                    //Значит надо построить переход
                    foreach (WorldClass.Cell c in testCellNow) {
                        //Пытаемся построить из списка обрывов
                        WorldClass.Cell cellToBuild = new WorldClass.Cell();
                        foreach (WorldClass.Cell cellBreak in testCellBreak) {
                            bool re = false;
                            if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < 9999999) {
                                if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < traficNumTypeBuffer[cellBreak.posX, cellBreak.posY, traficType]) {
                                    re = true;
                                }
                            }
                            else {
                                re = true;
                            }

                            if (re) {
                                cellToBuild = cellBreak;
                            }
                        }
                        //Если у ячейки есть номер трафика, то значит строим на этой точке
                        if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] >= 0 && traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < 9999999) {
                            //Нужно выбрать позицию для строительства
                            Vector2Int posSlopeBreak = new Vector2Int();
                            if (BuildingSlope) {
                                posSlopeBreak = BuildSlopeDown(cellToBuild.posX, cellToBuild.posY, true, ignorePos);
                            }
                            else {
                                //проверяем по бокам на наличие уже построенного склона
                                if (cellToBuild.posY > 0 && cellsData[cellToBuild.posX, cellToBuild.posY - 1].build == Building.getTypeName(Building.Type.Slope) && cellsData[cellToBuild.posX, cellToBuild.posY - 1].note == Building.getRotName(Building.Rotate.Down) && cellsData[cellToBuild.posX, cellToBuild.posY - 1].height + 1 == cellsData[cellToBuild.posX, cellToBuild.posY].height)
                                    posSlopeBreak = new Vector2Int(cellToBuild.posX, cellToBuild.posY - 1);

                                if (cellToBuild.posX > 0 && cellsData[cellToBuild.posX - 1, cellToBuild.posY].build == Building.getTypeName(Building.Type.Slope) && cellsData[cellToBuild.posX - 1, cellToBuild.posY].note == Building.getRotName(Building.Rotate.Left) && cellsData[cellToBuild.posX - 1, cellToBuild.posY].height + 1 == cellsData[cellToBuild.posX, cellToBuild.posY].height)
                                    posSlopeBreak = new Vector2Int(cellToBuild.posX - 1, cellToBuild.posY);

                                if (cellToBuild.posX < 59 && cellsData[cellToBuild.posX + 1, cellToBuild.posY].build == Building.getTypeName(Building.Type.Slope) && cellsData[cellToBuild.posX + 1, cellToBuild.posY].note == Building.getRotName(Building.Rotate.Right) && cellsData[cellToBuild.posX + 1, cellToBuild.posY].height + 1 == cellsData[cellToBuild.posX, cellToBuild.posY].height)
                                    posSlopeBreak = new Vector2Int(cellToBuild.posX + 1, cellToBuild.posY);

                                if (cellToBuild.posY < 59 && cellsData[cellToBuild.posX, cellToBuild.posY + 1].build == Building.getTypeName(Building.Type.Slope) && cellsData[cellToBuild.posX, cellToBuild.posY + 1].note == Building.getRotName(Building.Rotate.Up) && cellsData[cellToBuild.posX, cellToBuild.posY + 1].height + 1 == cellsData[cellToBuild.posX, cellToBuild.posY].height)
                                    posSlopeBreak = new Vector2Int(cellToBuild.posX, cellToBuild.posY + 1);
                            }
                            //если есть позиция то значит ступень была построена
                            if (posSlopeBreak != new Vector2Int()) {

                                //запоминаем ячейку как следуюшую для теста на следуюшем цикле
                                traficNumTypeBuffer[posSlopeBreak.x, posSlopeBreak.y, traficType] = (traficNumTypeBuffer[c.posX, c.posY, traficType] + 1) + cellsData[c.posX, c.posY].traficDanger;
                                traficDistToBaceBuffer[posSlopeBreak.x, posSlopeBreak.y] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                testCellNext.Add(cellsData[posSlopeBreak.x, posSlopeBreak.y]);
                                setTraficLenght(cellsData[posSlopeBreak.x, posSlopeBreak.y]);
                                BuildSlope1 = true;
                            }
                        }

                        /////////////////////////////////////////////////////////////////////////////////////
                        //иначе пытаемся найти точку по резкому спуску
                        if (!BuildSlope1)
                        {
                            foreach (WorldClass.Cell cellBreak2 in testCellBreak2)
                            {
                                bool re = false;
                                if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < 9999999)
                                {
                                    if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < traficNumTypeBuffer[cellBreak2.posX, cellBreak2.posY, traficType])
                                    {
                                        re = true;
                                    }
                                }
                                else
                                {
                                    re = true;
                                }

                                if (re)
                                {
                                    cellToBuild = cellBreak2;
                                }
                            }
                            if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] >= 0 && traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < 9999999)
                            {
                                //Нужно выбрать позицию для строительства
                                Vector2Int posSlopeBreak = new Vector2Int();
                                if (BuildingSlope)
                                {
                                    posSlopeBreak = BuildSlopeDown2(cellToBuild.posX, cellToBuild.posY, true);
                                }
                                //Если строить нельзя то проверяем по бокам наличие спусков-склонов
                                else
                                {
                                    //posSlopeBreak = BuildSlopeDown2(cellToBuild.posX, cellToBuild.posY, false);

                                    //проверяем по бокам на наличие уже построенного склона
                                    if (cellToBuild.posY > 0 && cellsData[cellToBuild.posX, cellToBuild.posY - 1].build == Building.getTypeName(Building.Type.Slope) && cellsData[cellToBuild.posX, cellToBuild.posY - 1].note == Building.getRotName(Building.Rotate.Down) && cellsData[cellToBuild.posX, cellToBuild.posY - 1].height + 1 == cellsData[cellToBuild.posX, cellToBuild.posY].height)
                                        posSlopeBreak = new Vector2Int(cellToBuild.posX, cellToBuild.posY - 1);

                                    if (cellToBuild.posX > 0 && cellsData[cellToBuild.posX - 1, cellToBuild.posY].build == Building.getTypeName(Building.Type.Slope) && cellsData[cellToBuild.posX - 1, cellToBuild.posY].note == Building.getRotName(Building.Rotate.Left) && cellsData[cellToBuild.posX - 1, cellToBuild.posY].height + 1 == cellsData[cellToBuild.posX, cellToBuild.posY].height)
                                        posSlopeBreak = new Vector2Int(cellToBuild.posX - 1, cellToBuild.posY);

                                    if (cellToBuild.posX < 59 && cellsData[cellToBuild.posX + 1, cellToBuild.posY].build == Building.getTypeName(Building.Type.Slope) && cellsData[cellToBuild.posX + 1, cellToBuild.posY].note == Building.getRotName(Building.Rotate.Right) && cellsData[cellToBuild.posX + 1, cellToBuild.posY].height + 1 == cellsData[cellToBuild.posX, cellToBuild.posY].height)
                                        posSlopeBreak = new Vector2Int(cellToBuild.posX + 1, cellToBuild.posY);

                                    if (cellToBuild.posY < 59 && cellsData[cellToBuild.posX, cellToBuild.posY + 1].build == Building.getTypeName(Building.Type.Slope) && cellsData[cellToBuild.posX, cellToBuild.posY + 1].note == Building.getRotName(Building.Rotate.Up) && cellsData[cellToBuild.posX, cellToBuild.posY + 1].height + 1 == cellsData[cellToBuild.posX, cellToBuild.posY].height)
                                        posSlopeBreak = new Vector2Int(cellToBuild.posX, cellToBuild.posY + 1);
                                }

                                //если есть позиция то значит ступень была построена
                                if (posSlopeBreak != new Vector2Int())
                                {

                                    //запоминаем ячейку как следуюшую для теста на следуюшем цикле
                                    traficNumTypeBuffer[posSlopeBreak.x, posSlopeBreak.y, traficType] = (short)(traficNumTypeBuffer[c.posX, c.posY, traficType] + 1);
                                    traficDistToBaceBuffer[posSlopeBreak.x, posSlopeBreak.y] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[posSlopeBreak.x, posSlopeBreak.y]);
                                    setTraficLenght(cellsData[posSlopeBreak.x, posSlopeBreak.y]);
                                    BuildSlope2 = true;
                                }

                            }
                        }

                        //проверяем наличие выхода в море
                        if (!BuildSlope1 && !BuildSlope2) {
                            //Устанавливаем ячейкам морской трафик на ноль
                            foreach (WorldClass.Cell cell in testCellSeaNow) {
                                traficNumTypeBuffer[cell.posX, cell.posY, traficType] = 1;
                            }

                            //Запускаем цикл перебора ячеек
                            for (; testCellSeaNow.Count > 0 ;) {
                                List<WorldClass.Cell> testCellSeaNext = new List<WorldClass.Cell>(); //Ячейки берега
                                //тестим ячейки сейчас
                                foreach (WorldClass.Cell cellNow in testCellSeaNow) {
                                    //Снизу
                                    if (cellNow.posY > 0 && cellsData[cellNow.posX, cellNow.posY - 1].height == cellNow.height &&
                                        traficNumTypeBuffer[cellNow.posX, cellNow.posY - 1, traficType] > traficNumTypeBuffer[cellNow.posX, cellNow.posY, traficType] + 1)
                                    {
                                        traficNumTypeBuffer[cellNow.posX, cellNow.posY - 1, traficType] = traficNumTypeBuffer[cellNow.posX, cellNow.posY, traficType] + 1;
                                        testCellSeaNext.Add(cellsData[cellNow.posX, cellNow.posY - 1]);
                                    }
                                    //слева
                                    if (cellNow.posX > 0 && cellsData[cellNow.posX - 1, cellNow.posY].height == cellNow.height &&
                                        traficNumTypeBuffer[cellNow.posX - 1, cellNow.posY, traficType] > traficNumTypeBuffer[cellNow.posX, cellNow.posY, traficType] + 1)
                                    {
                                        traficNumTypeBuffer[cellNow.posX - 1, cellNow.posY, traficType] = traficNumTypeBuffer[cellNow.posX, cellNow.posY, traficType] + 1;
                                        testCellSeaNext.Add(cellsData[cellNow.posX - 1, cellNow.posY]);
                                    }
                                    //справа
                                    if (cellNow.posX < 59 && cellsData[cellNow.posX + 1, cellNow.posY].height == cellNow.height &&
                                        traficNumTypeBuffer[cellNow.posX + 1, cellNow.posY, traficType] > traficNumTypeBuffer[cellNow.posX, cellNow.posY, traficType] + 1)
                                    {
                                        traficNumTypeBuffer[cellNow.posX + 1, cellNow.posY, traficType] = traficNumTypeBuffer[cellNow.posX, cellNow.posY, traficType] + 1;
                                        testCellSeaNext.Add(cellsData[cellNow.posX + 1, cellNow.posY]);
                                    }
                                    //сверху)
                                    if (cellNow.posY < 59 && cellsData[cellNow.posX, cellNow.posY + 1].height == cellNow.height &&
                                        traficNumTypeBuffer[cellNow.posX, cellNow.posY + 1, traficType] > traficNumTypeBuffer[cellNow.posX, cellNow.posY, traficType] + 1)
                                    {
                                        traficNumTypeBuffer[cellNow.posX, cellNow.posY + 1, traficType] = traficNumTypeBuffer[cellNow.posX, cellNow.posY, traficType] + 1;
                                        testCellSeaNext.Add(cellsData[cellNow.posX, cellNow.posY + 1]);
                                    }

                                    if (cellNow.posX == 0 || cellNow.posX == 59 || cellNow.posY == 0 || cellNow.posY == 59) {
                                        CalcOk = true;
                                        CanSpawnFlot = true;
                                    }
                                }
                                testCellSeaNow = testCellSeaNext;
                            }
                        }

                        ///////старое место каньена

                        ////////////////////////////////////////////////////////////////////////////////////////
                        //Иначе место для спуска не было найдено
                        //Если места для постройки спуска нет
                        if (!BuildSlope1
                            && !BuildSlope2
                            && !buildSea2
                            && !BuildCanyon
                            && BuildingSlope //Прорубаем только если строительство разрешено
                            ) {
                            //Нужно выбрать позицию для строительства
                            Vector2Int posSlopeBreak = new Vector2Int(); //BuildSlopeDown(cellToBuild.posX, cellToBuild.posY, true);
                            foreach (WorldClass.Cell cellExit in testCellCanExit)
                            {
                                bool re = false;
                                if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < 9999999)
                                {
                                    if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < traficNumTypeBuffer[cellExit.posX, cellExit.posY, traficType])
                                    {
                                        re = true;
                                    }
                                }
                                else
                                {
                                    re = true;
                                }

                                if (re)
                                {
                                    cellToBuild = cellExit;
                                }
                            }
                            if (traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] >= 0 && traficNumTypeBuffer[cellToBuild.posX, cellToBuild.posY, traficType] < 9999999)
                            {
                                //Нужно выбрать направление для копания ямы
                                if (BuildingSlope)
                                    posSlopeBreak = PitWithNoExit(cellToBuild.posX, cellToBuild.posY, true);
                                else {
                                    posSlopeBreak = PitWithNoExit(cellToBuild.posX, cellToBuild.posY, false);
                                }

                                //если есть позиция то значит напраавление выбрано и прокопано
                                if (posSlopeBreak != new Vector2Int())
                                {
                                    notLastCellThisHeight = true;
                                    //запоминаем ячейку как следуюшую для теста на следуюшем цикле
                                    traficNumTypeBuffer[posSlopeBreak.x, posSlopeBreak.y, traficType] = (short)(traficNumTypeBuffer[c.posX, c.posY, traficType] + 1);
                                    traficDistToBaceBuffer[posSlopeBreak.x, posSlopeBreak.y] = traficDistToBaceBuffer[c.posX, c.posY] + 1;
                                    testCellNext.Add(cellsData[posSlopeBreak.x, posSlopeBreak.y]);
                                    BuildCanExit = true;
                                }
                            }
                            Debug.Log("Копаем прорыв height " + heightNow);
                        }


                        /*
                        ////////////////////////////////////////////////////////////////////////////////////////
                        //Если ямы не было обнаружено то проверяем ячейки на возможность строительства берега
                        if (!BuildSlope1 //Если обычный спуск не был построен
                            && !BuildSlope2 //Если резкий спукс не был построен
                            && !BuildCanExit //Если выход из тупика не был построен
                            && heightNow <= 1)
                        {

                            //Выбираем самую далекую точку
                            foreach (WorldClass.Cell cellTest in testCellSea)
                            {
                                bool re = false;
                                if (traficNum[cellToBuild.posX, cellToBuild.posY] < 9999999)
                                {
                                    if (traficNum[cellToBuild.posX, cellToBuild.posY] < traficNum[cellTest.posX, cellTest.posY])
                                    {
                                        re = true;
                                    }
                                }
                                else
                                {
                                    re = true;
                                }

                                if (re)
                                {
                                    cellToBuild = cellTest;
                                }
                            }
                            //Если трафик не нулевой
                            if (traficNum[cellToBuild.posX, cellToBuild.posY] >= 0 && traficNum[cellToBuild.posX, cellToBuild.posY] < 9999999)
                            {
                                if (BuildingSlope)
                                {
                                    AddShore(cellToBuild);
                                    IniEndCell(cellToBuild.posX, cellToBuild.posY, true);
                                    CalcOk = true;
                                }
                                else
                                {
                                    IniEndCell(cellToBuild.posX, cellToBuild.posY, true);
                                    CalcOk = true;
                                }
                            }
                            //Если берег не обнаружен либо не выходит на границы карты
                            if (!CanSpawnFlot)
                            {
                                if (BuildingSlope)
                                {
                                    
                                }
                                //CalcOk = true;
                            }
                            else
                            {
                                Debug.Log("EnemyFlot Spawn");
                            }
                        }

                        */

                        bool buildCanyon = true;
                        //Иначе либо яма либо обрыв (прорубаем выход к ближайшим другим ячейкам той же высоты) Только если решим строительства
                        if (buildCanyon && BuildingSlope && !BuildSlope1 && !BuildSlope2 && !CanSpawnFlot)
                        {
                            //Начинаем провеку на выравнивание с препятствием в одну клетку и увеличиваем если не нашли
                            for (int maxHard = 1; maxHard < 100; maxHard++)
                            {
                                //Выбираем ячеку для поиска ближайших соседей того же уровня и без трафика
                                //Начинаем с дальней
                                for (int testNum = testCellStartCanyon.Count - 1; testNum >= 0; testNum--)
                                {

                                    //Получаем ячейки каньена, если они достигают потенциального продожения маршрута
                                    List<WorldClass.Cell> canyonCells = GetCellsToChange(testCellStartCanyon[testNum], maxHard);

                                    //Если потенциальный каньен найден.
                                    if (canyonCells.Count > 0)
                                    {
                                        //Если каньен был проложен
                                        bool isBuild = BuildingCanyon(canyonCells, testCellStartCanyon[testNum].height);
                                        if (isBuild)
                                        {
                                            //Добавляем заного эту ячейку на следующий шаг, она продолжит строить трафик через каньон
                                            testCellNext.Add(canyonCells[0]);
                                            //Трафик + 1
                                            traficNumTypeBuffer[canyonCells[0].posX, canyonCells[0].posY, traficType] = 1 + traficNumTypeBuffer[testCellStartCanyon[testNum].posX, testCellStartCanyon[testNum].posY, traficType];
                                            traficDistToBaceBuffer[canyonCells[0].posX, canyonCells[0].posY] = 1 + traficDistToBaceBuffer[testCellStartCanyon[testNum].posX, testCellStartCanyon[testNum].posY];
                                            BuildCanyon = true; //Информируем что каньен построен
                                            notLastCellThisHeight = true;
                                            break;
                                        }
                                    }
                                }
                                //Если каньен был построен завершаем цикл
                                if (BuildCanyon) break;
                            }



                            //Получаем лист ячеек которые можно менять
                            List<WorldClass.Cell> GetCellsToChange(WorldClass.Cell cell, int maxLenght)
                            {
                                List<WorldClass.Cell> result = new List<WorldClass.Cell>();

                                //ищем в какой стороне найдется ближайшая подходящая ячейка
                                bool found = false;
                                int hardLine = 0; //Сложность построения линии.. чем сильнее различается высота тем сложнее
                                //Тестим вниз..
                                if (!found && cell.posY - maxLenght > 0 && cellsData[cell.posX, cell.posY - 1].height != cell.height)
                                { //Если за границы карты не вышли и с этой стороны отличается высота
                                    hardLine = 0;
                                    int plus = 1;
                                    for (plus = 1; plus <= maxLenght; plus++)
                                    {
                                        hardLine += Mathf.Abs(cell.height - cellsData[cell.posX, cell.posY - plus].height);
                                        if (hardLine > maxLenght) break;
                                    }
                                    //Тестируем
                                    found = TestCellGoodNeighbourhood(cellsData[cell.posX, cell.posY - plus], cell.height);
                                    //Если тест успешный заносим ячейки с лист
                                    if (found)
                                    {
                                        for (int num = 1; num <= plus; num++)
                                        {
                                            result.Add(cellsData[cell.posX, cell.posY - num]);
                                        }
                                    }
                                }
                                //Тестим лево
                                if (!found && cell.posX - maxLenght > 0 && cellsData[cell.posX - 1, cell.posY].height != cell.height)
                                { //Если за границы карты не вышли и с этой стороны отличается высота
                                    hardLine = 0;
                                    int plus = 1;
                                    for (plus = 1; plus <= maxLenght; plus++)
                                    {
                                        hardLine += Mathf.Abs(cell.height - cellsData[cell.posX - plus, cell.posY].height);
                                        if (hardLine > maxLenght) break;
                                    }
                                    //Тестируем
                                    found = TestCellGoodNeighbourhood(cellsData[cell.posX - plus, cell.posY], cell.height);
                                    //Если тест успешный заносим ячейки с лист
                                    if (found)
                                    {
                                        for (int num = 1; num <= plus; num++)
                                        {
                                            result.Add(cellsData[cell.posX - num, cell.posY]);
                                        }
                                    }
                                }
                                //Тестим право
                                if (!found && cell.posX + maxLenght < 59 && cellsData[cell.posX + 1, cell.posY].height != cell.height)
                                { //Если за границы карты не вышли и с этой стороны отличается высота
                                    hardLine = 0;
                                    int plus = 1;
                                    for (plus = 1; plus <= maxLenght; plus++)
                                    {
                                        hardLine += Mathf.Abs(cell.height - cellsData[cell.posX + plus, cell.posY].height);
                                        if (hardLine > maxLenght) break;
                                    }
                                    //Тестируем
                                    found = TestCellGoodNeighbourhood(cellsData[cell.posX + plus, cell.posY], cell.height);
                                    //Если тест успешный заносим ячейки с лист
                                    if (found)
                                    {
                                        for (int num = 1; num <= plus; num++)
                                        {
                                            result.Add(cellsData[cell.posX + num, cell.posY]);
                                        }
                                    }
                                }
                                //Тестим вверх
                                if (!found && cell.posY + maxLenght < 59 && cellsData[cell.posX, cell.posY + 1].height != cell.height)
                                { //Если за границы карты не вышли и с этой стороны отличается высота
                                    hardLine = 0;
                                    int plus = 1;
                                    for (plus = 1; plus <= maxLenght; plus++)
                                    {
                                        hardLine += Mathf.Abs(cell.height - cellsData[cell.posX, cell.posY + plus].height);
                                        if (hardLine > maxLenght) break;
                                    }
                                    //Тестируем
                                    found = TestCellGoodNeighbourhood(cellsData[cell.posX, cell.posY + plus], cell.height);
                                    //Если тест успешный заносим ячейки с лист
                                    if (found)
                                    {
                                        for (int num = 1; num <= plus; num++)
                                        {
                                            result.Add(cellsData[cell.posX, cell.posY + num]);
                                        }
                                    }
                                }

                                return result;

                                //Проверяем ячейку на соседство с потенциальным продолжением маршрута
                                bool TestCellGoodNeighbourhood(WorldClass.Cell cellTestNeighbour, int height)
                                {

                                    //Если соседняя ячейка тойже высоты что и начало линии и трафика на ней нет то она теперь наша цель
                                    bool isGood = false;
                                    //Снизу
                                    if (cellTestNeighbour.posY > 0 && cellsData[cellTestNeighbour.posX, cellTestNeighbour.posY - 1].height == height &&
                                        traficNumTypeBuffer[cellTestNeighbour.posX, cellTestNeighbour.posY - 1, traficType] > 999999)
                                    {
                                        isGood = true;
                                    }
                                    //слева
                                    if (cellTestNeighbour.posX > 0 && cellsData[cellTestNeighbour.posX - 1, cellTestNeighbour.posY].height == height &&
                                        traficNumTypeBuffer[cellTestNeighbour.posX - 1, cellTestNeighbour.posY, traficType] > 999999)
                                    {
                                        isGood = true;
                                    }
                                    //справа
                                    if (cellTestNeighbour.posX < 59 && cellsData[cellTestNeighbour.posX + 1, cellTestNeighbour.posY].height == height &&
                                        traficNumTypeBuffer[cellTestNeighbour.posX + 1, cellTestNeighbour.posY, traficType] > 999999)
                                    {
                                        isGood = true;
                                    }
                                    //сверху)
                                    if (cellTestNeighbour.posY < 59 && cellsData[cellTestNeighbour.posX, cellTestNeighbour.posY + 1].height == height &&
                                        traficNumTypeBuffer[cellTestNeighbour.posX, cellTestNeighbour.posY + 1, traficType] > 999999)
                                    {
                                        isGood = true;
                                    }

                                    return isGood;
                                }
                            }

                            //Тест трафика игнорируя список ячеек
                            bool BuildingCanyon(List<WorldClass.Cell> canyonCells, int needHeight)
                            {
                                bool isBuild = false;

                                bool canBuild = true;
                                //Начинаем тестировать потенциальные ячейки каньена на возможность ихнего изменения
                                canBuild = testCanyonTrafic();

                                //Если можно изменять все ячейки каньена то меняем
                                if (canBuild)
                                {
                                    foreach (WorldClass.Cell canyonCell in canyonCells)
                                    {
                                        cellsData[canyonCell.posX, canyonCell.posY].height = (sbyte)needHeight;
                                        traficNumTypeBuffer[canyonCell.posX, canyonCell.posY, traficType] = 9999999; //обнуляем трафик ячеек чтобы проложить новый маршрут по ним
                                        traficDistToBaceBuffer[canyonCell.posX, canyonCell.posY] = 9999999;
                                        traficChange[canyonCell.posX, canyonCell.posY] = 1;
                                    }
                                    isBuild = true;
                                }

                                return isBuild;


                                //Нашел ли трафик спуск для каждых из высот или нет
                                bool testCanyonTrafic()
                                {
                                    bool result = true;

                                    List<WorldClass.Cell> StartCanyonHeights = new List<WorldClass.Cell>(); //для хранения старта каждых из высот каньена
                                    //Получаем минимум для каждых из высот
                                    getStartCanyonHeights();
                                    //Перебираем все ячейки для теста маршрута

                                    bool[,] isCalcCanyonTrafic = new bool[60, 60];  //Для просчета маршрута до спуска, не меняя текущий трафик

                                    List<WorldClass.Cell> traficCanyonNow = new List<WorldClass.Cell>();
                                    List<WorldClass.Cell> traficCanyonNext = new List<WorldClass.Cell>();
                                    for (int num = 0; num < StartCanyonHeights.Count; num++)
                                    {
                                        //
                                        traficCanyonNow.Add(StartCanyonHeights[num]);

                                        //Указываем что этот трафик проверен чтобы его не добавляли с следующем цикле
                                        isCalcCanyonTrafic[StartCanyonHeights[num].posX, StartCanyonHeights[num].posY] = true;

                                        //Начинаем попытку построения маршрута до спуска
                                        //Если спуск не найден
                                        if (!searchSlopeInTrafic())
                                        {
                                            //этот каньен строить нельзя
                                            return false;
                                        }
                                        //Чистим текущий трафик
                                        traficCanyonNow.Clear();
                                    }

                                    return result;

                                    //Получить минимум трафика соседних ячеек каньена, то есть получаем соседнюю ячейку с минимальным трафиком
                                    //Ячейка не должна быть каньеном
                                    void getStartCanyonHeights()
                                    {
                                        //перебираем все ячейки каньена
                                        foreach (WorldClass.Cell canyonCell in canyonCells)
                                        {
                                            //выполняем поиск минимума у этой ячейки среди соседей
                                            getMinimum(canyonCell);
                                        }

                                        void getMinimum(WorldClass.Cell cellTest)
                                        {
                                            //Если трафик есть, то проводим поиск минимума
                                            if (traficNumTypeBuffer[cellTest.posX, cellTest.posY, traficType] < 999999)
                                            {
                                                //низ
                                                bool thisCanyon = false;
                                                //Если низ тойже высоты
                                                if (cellTest.height == cellsData[cellTest.posX, cellTest.posY - 1].height)
                                                {
                                                    //Проверка ячейка часть каньена?
                                                    foreach (WorldClass.Cell canyonCell in canyonCells)
                                                    {
                                                        //Да, ячейка часть каньена, отменяем дальнейший тест
                                                        if (canyonCell.posX == cellTest.posX && canyonCell.posY == cellTest.posY - 1)
                                                        {
                                                            thisCanyon = true;
                                                            break;
                                                        }
                                                    }
                                                    //Если ячейка не была частью каньена
                                                    if (!thisCanyon)
                                                    {
                                                        //Добавляе минимум в список минимумов
                                                        addMininum(cellsData[cellTest.posX, cellTest.posY - 1]);
                                                    }
                                                }

                                                //лево
                                                thisCanyon = false;
                                                //Если низ тойже высоты
                                                if (cellTest.height == cellsData[cellTest.posX - 1, cellTest.posY].height)
                                                {
                                                    //Проверка ячейка часть каньена?
                                                    foreach (WorldClass.Cell canyonCell in canyonCells)
                                                    {
                                                        //Да, ячейка часть каньена, отменяем дальнейший тест
                                                        if (canyonCell.posX == cellTest.posX - 1 && canyonCell.posY == cellTest.posY)
                                                        {
                                                            thisCanyon = true;
                                                            break;
                                                        }
                                                    }
                                                    //Если ячейка не была частью каньена
                                                    if (!thisCanyon)
                                                    {
                                                        //Добавляе минимум в список минимумов
                                                        addMininum(cellsData[cellTest.posX - 1, cellTest.posY]);
                                                    }
                                                }

                                                //право
                                                thisCanyon = false;
                                                //Если низ тойже высоты
                                                if (cellTest.height == cellsData[cellTest.posX + 1, cellTest.posY].height)
                                                {
                                                    //Проверка ячейка часть каньена?
                                                    foreach (WorldClass.Cell canyonCell in canyonCells)
                                                    {
                                                        //Да, ячейка часть каньена, отменяем дальнейший тест
                                                        if (canyonCell.posX == cellTest.posX + 1 && canyonCell.posY == cellTest.posY)
                                                        {
                                                            thisCanyon = true;
                                                            break;
                                                        }
                                                    }
                                                    //Если ячейка не была частью каньена
                                                    if (!thisCanyon)
                                                    {
                                                        //Добавляе минимум в список минимумов
                                                        addMininum(cellsData[cellTest.posX + 1, cellTest.posY]);
                                                    }
                                                }

                                                //верх
                                                thisCanyon = false;
                                                //Если низ тойже высоты
                                                if (cellTest.height == cellsData[cellTest.posX, cellTest.posY].height + 1)
                                                {
                                                    //Проверка ячейка часть каньена?
                                                    foreach (WorldClass.Cell canyonCell in canyonCells)
                                                    {
                                                        //Да, ячейка часть каньена, отменяем дальнейший тест
                                                        if (canyonCell.posX == cellTest.posX && canyonCell.posY == cellTest.posY + 1)
                                                        {
                                                            thisCanyon = true;
                                                            break;
                                                        }
                                                    }
                                                    //Если ячейка не была частью каньена
                                                    if (!thisCanyon)
                                                    {
                                                        //Добавляе минимум в список минимумов
                                                        addMininum(cellsData[cellTest.posX, cellTest.posY + 1]);
                                                    }
                                                }

                                            }

                                            void addMininum(WorldClass.Cell minimumNew)
                                            {
                                                //Ищем минимум тойже высоты
                                                bool found = false;
                                                for (int num = 0; num < StartCanyonHeights.Count; num++)
                                                {
                                                    //Если высота одинаковая и трафик меньше
                                                    if (StartCanyonHeights[num].height == minimumNew.height &&
                                                        traficNumTypeBuffer[StartCanyonHeights[num].posX, StartCanyonHeights[num].posY, traficType] < 999999 &&
                                                        traficNumTypeBuffer[StartCanyonHeights[num].posX, StartCanyonHeights[num].posY, traficType] > traficNumTypeBuffer[minimumNew.posX, minimumNew.posY, traficType])
                                                    {
                                                        //Заменяем
                                                        StartCanyonHeights[num] = minimumNew;
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                                //Если минимум тойже высоты не был обнарущен - добавляем
                                                if (!found)
                                                    StartCanyonHeights.Add(cellsData[minimumNew.posX, minimumNew.posY]);
                                            }
                                        }
                                    }

                                    bool searchSlopeInTrafic()
                                    {
                                        bool found = false;

                                        //Ищем спуск
                                        for (int cycle = 0; traficCanyonNow.Count > 0 && !found; cycle++)
                                        {
                                            //перебираем ячейки текущего шага
                                            for (int num = 0; num < traficCanyonNow.Count && !found; num++)
                                            {
                                                //проверяем все стороны текущей ячейки на возможность продолжения волны.. и правельно повернутого спуска

                                                //Проверка снизу
                                                if (traficCanyonNow[num].posY > 0)
                                                {
                                                    if (TestTraficCanyonCellToNext(cellsData[traficCanyonNow[num].posX, traficCanyonNow[num].posY - 1], traficCanyonNow[num].height))
                                                    {
                                                        traficCanyonNext.Add(cellsData[traficCanyonNow[num].posX, traficCanyonNow[num].posY - 1]);
                                                    }
                                                }
                                                //Проверка слева
                                                if (traficCanyonNow[num].posX > 0)
                                                {
                                                    if (TestTraficCanyonCellToNext(cellsData[traficCanyonNow[num].posX - 1, traficCanyonNow[num].posY], traficCanyonNow[num].height))
                                                    {
                                                        traficCanyonNext.Add(cellsData[traficCanyonNow[num].posX - 1, traficCanyonNow[num].posY]);
                                                    }
                                                }
                                                //Проверка справа
                                                if (traficCanyonNow[num].posX < 59)
                                                {
                                                    if (TestTraficCanyonCellToNext(cellsData[traficCanyonNow[num].posX + 1, traficCanyonNow[num].posY], traficCanyonNow[num].height))
                                                    {
                                                        traficCanyonNext.Add(cellsData[traficCanyonNow[num].posX + 1, traficCanyonNow[num].posY]);
                                                    }
                                                }
                                                //Проверка сверху
                                                if (traficCanyonNow[num].posY < 59)
                                                {
                                                    if (TestTraficCanyonCellToNext(cellsData[traficCanyonNow[num].posX, traficCanyonNow[num].posY + 1], traficCanyonNow[num].height))
                                                    {
                                                        traficCanyonNext.Add(cellsData[traficCanyonNow[num].posX, traficCanyonNow[num].posY + 1]);
                                                    }
                                                }

                                                //Проверяем на наличие спуска по бокам
                                                found = TestTraficCanyonCellToSlope(cellsData[traficCanyonNow[num].posX, traficCanyonNow[num].posY]);

                                            }

                                            //Все текущие ячейки были проверены //Двигаем шаг
                                            traficCanyonNow = traficCanyonNext;
                                            traficCanyonNext = new List<WorldClass.Cell>();
                                        }

                                        return found;

                                        //Проверка можно ли добавить эту ячейку в список
                                        bool TestTraficCanyonCellToNext(WorldClass.Cell traficCanyonCellTest, int heightCanyonTraficNow)
                                        {

                                            //Проверка высот
                                            if (traficCanyonCellTest.height != heightCanyonTraficNow)
                                                return false;

                                            //Если уже считали
                                            if (isCalcCanyonTrafic[traficCanyonCellTest.posX, traficCanyonCellTest.posY])
                                                return false;

                                            //Если эта ячейка оказалась частью прорубаемого каньена
                                            foreach (WorldClass.Cell canyonCell in canyonCells)
                                            {
                                                if (canyonCell.posX == traficCanyonCellTest.posX && canyonCell.posY == traficCanyonCellTest.posY)
                                                {
                                                    return false;
                                                }
                                            }

                                            //Все проверки пройдены можно добавлять в маршрут
                                            isCalcCanyonTrafic[traficCanyonCellTest.posX, traficCanyonCellTest.posY] = true;
                                            return true;
                                        }
                                        //проверка на наличие у прилегающих ячеек правильно повернутого спуска
                                        bool TestTraficCanyonCellToSlope(WorldClass.Cell traficCanyonCellTest)
                                        {
                                            //проверка снизу
                                            if (traficCanyonCellTest.posY > 0 &&
                                                cellsData[traficCanyonCellTest.posX, traficCanyonCellTest.posY - 1].build == Building.getTypeName(Building.Type.Slope) &&
                                                cellsData[traficCanyonCellTest.posX, traficCanyonCellTest.posY - 1].note == Building.getRotName(Building.Rotate.Down))
                                            {
                                                return true;
                                            }
                                            //Слева
                                            if (traficCanyonCellTest.posX > 0 &&
                                                cellsData[traficCanyonCellTest.posX - 1, traficCanyonCellTest.posY].build == Building.getTypeName(Building.Type.Slope) &&
                                                cellsData[traficCanyonCellTest.posX - 1, traficCanyonCellTest.posY].note == Building.getRotName(Building.Rotate.Left))
                                            {
                                                return true;
                                            }
                                            //Справа
                                            if (traficCanyonCellTest.posX < 59 &&
                                                cellsData[traficCanyonCellTest.posX + 1, traficCanyonCellTest.posY].build == Building.getTypeName(Building.Type.Slope) &&
                                                cellsData[traficCanyonCellTest.posX + 1, traficCanyonCellTest.posY].note == Building.getRotName(Building.Rotate.Right))
                                            {
                                                return true;
                                            }
                                            //Сверху
                                            if (traficCanyonCellTest.posY < 59 &&
                                                cellsData[traficCanyonCellTest.posX, traficCanyonCellTest.posY + 1].build == Building.getTypeName(Building.Type.Slope) &&
                                                cellsData[traficCanyonCellTest.posX, traficCanyonCellTest.posY + 1].note == Building.getRotName(Building.Rotate.Up))
                                            {
                                                return true;
                                            }

                                            //По бокам не было обнаружено спуска
                                            return false;
                                        }
                                    }
                                }
                            }
                        }

                        //}

                    }

                    
                    //Если ячейка была последняя
                    if (!notLastCellThisHeight) {
                        //Переходы построены переходим на уровень ниже
                        //Если не был проложен каньен
                        if(!BuildCanyon)
                            heightNow--;

                        testCellBreak = new List<WorldClass.Cell>();
                        testCellBreak2 = new List<WorldClass.Cell>();
                        testCellCanExit = new List<WorldClass.Cell>();
                        testCellStartCanyon = new List<WorldClass.Cell>();
                        testCellSea = new List<WorldClass.Cell>();
                        testCellSeaNow = new List<WorldClass.Cell>();
                    }
                }
                else if (foundSlope) {
                    //Переходы найдены переходим на уровень ниже
                    heightNow--;
                    testCellBreak = new List<WorldClass.Cell>();
                    testCellBreak2 = new List<WorldClass.Cell>();
                    testCellCanExit = new List<WorldClass.Cell>();
                    testCellStartCanyon = new List<WorldClass.Cell>();
                    testCellSea = new List<WorldClass.Cell>();

                    //Запоминаем
                }

                //переключаемся на следующий шаг
                testCellNow = testCellNext;

                bool AddShore(WorldClass.Cell TestCell)
                {
                    bool shore = false;

                    //Короче, блядь, если ячейка рядом с морем это, сука, берег, добавляем ячейку моря в список
                    if (TestCell.posY > 0 && cellsData[TestCell.posX, TestCell.posY - 1].height == 0)
                    {
                        traficNumTypeBuffer[TestCell.posX, TestCell.posY - 1, traficType] = (short)(traficNumTypeBuffer[TestCell.posX, TestCell.posY, traficType] + 1);
                        testCellSeaNow.Add(cellsData[TestCell.posX, TestCell.posY - 1]);
                    }
                    //слева
                    else if (TestCell.posX > 0 && cellsData[TestCell.posX - 1, TestCell.posY].height == 0)
                    {
                        traficNumTypeBuffer[TestCell.posX - 1, TestCell.posY, traficType] = (short)(traficNumTypeBuffer[TestCell.posX, TestCell.posY, traficType] + 1);
                        testCellSeaNow.Add(cellsData[TestCell.posX - 1, TestCell.posY]);
                    }
                    //справа
                    else if (TestCell.posX < 59 && cellsData[TestCell.posX + 1, TestCell.posY].height == 0)
                    {
                        traficNumTypeBuffer[TestCell.posX + 1, TestCell.posY, traficType] = (short)(traficNumTypeBuffer[TestCell.posX, TestCell.posY, traficType] + 1);
                        testCellSeaNow.Add(cellsData[TestCell.posX + 1, TestCell.posY]);
                    }
                    //Сверху
                    else if (TestCell.posY < 59 && cellsData[TestCell.posX, TestCell.posY + 1].height == 0)
                    {
                        traficNumTypeBuffer[TestCell.posX, TestCell.posY + 1, traficType] = (short)(traficNumTypeBuffer[TestCell.posX, TestCell.posY, traficType] + 1);
                        testCellSeaNow.Add(cellsData[TestCell.posX, TestCell.posY + 1]);
                    }

                    return shore;
                }
            }
        }

        //Нужно ли применить новый трафик
        if (acceptNewTrafic) {
            for (int x = 0; x < 60; x++) {
                for (int y = 0; y < 60; y++) {
                    cellsData[x, y].traficNum = traficNumTypeBuffer[x, y, traficType];

                    if (traficDistToBace[x,y] > traficDistToBaceBuffer[x,y]) {
                        traficDistToBace[x, y] = traficDistToBaceBuffer[x, y];
                    }
                }
            }

            //Переотправить всем клиентам ячейки
            NeedReUpdateCells = true;
        }

        return CalcOk;

        //Установить длину трафика карты.
        void setTraficLenght(WorldClass.Cell cell)
        {
            if (traficLenght <= 0 && cell.height == 1)
            {
                traficLenght = traficNumTypeBuffer[cell.posX, cell.posY, traficType];
            }
        }

        bool TestStartCanyon(WorldClass.Cell TestCell) {

            //Выходим если:
            //ячейка граница карты
            if (TestCell.posX == 0 || TestCell.posY == 0 || TestCell.posX == cellsData.GetLength(0) || TestCell.posY == cellsData.GetLength(1)) {
                return false;
            }

            //Если рядом есть ячейки другой высоты (которая потенциально может быть препятствием)
            if (cellsData[TestCell.posX, TestCell.posY - 1].height != TestCell.height || cellsData[TestCell.posX + 1, TestCell.posY].height != TestCell.height ||
                cellsData[TestCell.posX - 1, TestCell.posY].height != TestCell.height || cellsData[TestCell.posX, TestCell.posY + 1].height != TestCell.height) {
                return true;
            }

            return false;
            
        }

    }
    public bool CalcTraficAll(bool BuildingSlope, Vector2Int ignorePos, bool acceptNewTrafic) {
        //Проверяем 3 типа трафика на проходимость
        bool allOk = true;

        if (acceptNewTrafic) {
            for (int x = 0; x < traficDistToBace.GetLength(0); x++) {
                for (int y = 0; y < traficDistToBace.GetLength(0); y++)
                {
                    traficDistToBace[x, y] = 9999999;
                }
            }
        }

        //если режим первой генерации карты
        if (BuildingSlope)
        {
            allOk = CalcTraficType(BuildingSlope, ignorePos, acceptNewTrafic, 0);
            if(allOk)
                allOk = CalcTraficType(false, ignorePos, acceptNewTrafic, 1);
            if(allOk)
                allOk = CalcTraficType(false, ignorePos, acceptNewTrafic, 2);
        }
        else {
            allOk = CalcTraficType(false, ignorePos, acceptNewTrafic, 0);
            if (allOk)
                allOk = CalcTraficType(false, ignorePos, acceptNewTrafic, 1);
            if (allOk)
                allOk = CalcTraficType(false, ignorePos, acceptNewTrafic, 2);
        }

        //Если все ок и нужно применить трафик, то применяем
        if (allOk && acceptNewTrafic) {
            traficNumType = traficNumTypeBuffer;
            for (int x = 0; x < 60; x++) {
                for (int y = 0; y < 60; y++) {
                    cellsData[x, y].traficNum = traficNumTypeBuffer[x, y, 0];
                    cellsData[x, y].traficNum1 = traficNumTypeBuffer[x, y, 1];
                    cellsData[x, y].traficNum2 = traficNumTypeBuffer[x, y, 2];
                }
            }
        }

        return allOk;
    }
    public int CalcTerraforming(bool buildUP, Vector2Int poscell, bool acceptTerraforming) {
        bool terraformOk = true;
        int terraformCost = 0;

        //Если появление препятствия не помешает маршруту
        if (CalcTraficAll(false, new Vector2Int((int)poscell.x, (int)poscell.y), false))
        {

            //получаем новую высоту
            int heightNew = cellsData[(int)poscell.x, (int)poscell.y].height;
            if (buildUP)
                heightNew++;
            else heightNew--;

            //Максимальная разница в высотах при строительстве
            int heightMax = 1;

            //Строить нельзя если
            //новая высота меньше либо равна 1
            if (heightNew <= 1)
            {
                terraformOk = false;
            }
            //Если строительство вверх и 
            //хотябы одна из соседних ячеек ниже 2-х уровней
            else if (buildUP
                && (cellsData[poscell.x - 1, poscell.y].height < heightNew - heightMax 
                || cellsData[poscell.x + 1, poscell.y].height < heightNew - heightMax 
                || cellsData[poscell.x, poscell.y - 1].height < heightNew - heightMax 
                || cellsData[poscell.x, poscell.y + 1].height < heightNew - heightMax)) {
                terraformOk = false;
            }
            //Если строительство вниз и 
            //Хотябы одна из соседних ячеек выше 2-х уровней
            else if (!buildUP
                && cellsData[poscell.x - 1, poscell.y].height > heightNew + heightMax 
                || cellsData[poscell.x + 1, poscell.y].height > heightNew + heightMax 
                || cellsData[poscell.x, poscell.y - 1].height > heightNew + heightMax 
                || cellsData[poscell.x, poscell.y + 1].height > heightNew + heightMax) {
                terraformOk = false;
            }

            //Если можно то меняем высоту
            if (terraformOk)
            {
                int count = 0;
                if (buildUP)
                {
                    if (cellsData[poscell.x - 1, poscell.y].height <= cellsData[poscell.x, poscell.y].height)
                        count++;
                    if (cellsData[poscell.x + 1, poscell.y].height <= cellsData[poscell.x, poscell.y].height)
                        count++;
                    if (cellsData[poscell.x, poscell.y - 1].height <= cellsData[poscell.x, poscell.y].height)
                        count++;
                    if (cellsData[poscell.x, poscell.y + 1].height <= cellsData[poscell.x, poscell.y].height)
                        count++;
                }
                else {
                    if (cellsData[poscell.x - 1, poscell.y].height >= cellsData[poscell.x, poscell.y].height)
                        count++;
                    if (cellsData[poscell.x + 1, poscell.y].height >= cellsData[poscell.x, poscell.y].height)
                        count++;
                    if (cellsData[poscell.x, poscell.y - 1].height >= cellsData[poscell.x, poscell.y].height)
                        count++;
                    if (cellsData[poscell.x, poscell.y + 1].height >= cellsData[poscell.x, poscell.y].height)
                        count++;
                }

                terraformCost = cellsData[poscell.x, poscell.y].height * 5 + cellsData[poscell.x, poscell.y].height * count * 5;

                if (acceptTerraforming) {
                    cellsData[(int)poscell.x, (int)poscell.y].height = (sbyte)heightNew;
                    CalcTraficAll(false, new Vector2Int((int)poscell.x, (int)poscell.y), true);
                    addAndCellToSyncList(cellsData[(int)poscell.x, (int)poscell.y]);
                }
            }
        }

        return terraformCost;
    }

    //Построить плавный склон вниз
    Vector2Int BuildSlopeDown(short posX, short posY, bool needBuild, Vector2Int ignorePos) {
        //Позиция на котором был построен склон
        Vector2Int buildPos = new Vector2Int();
        GameObject slopeObj = null;
        string toRotate = "";

        //Проверяем на возможность строительства позиции пристройки к данной ячейки
        //Если на ячейке ничего не построено и высота выше 1
        if (Building.getTypeName(Building.Type.None) == cellsData[posX, posY].build && cellsData[posX, posY].height > 1) {
            //Спуск на нас
            if (posY > 1 && cellsData[posX, posY].height == (cellsData[posX, posY - 1].height + 1) && cellsData[posX, posY - 1].height == cellsData[posX, posY - 2].height && new Vector2Int(posX, posY - 2) != ignorePos && new Vector2Int(posX, posY - 1) != ignorePos && new Vector2Int(posX, posY) != ignorePos) {
                if (needBuild)
                    slopeObj = Instantiate(buildSlope);
                buildPos = new Vector2Int(posX, posY - 1);
                toRotate = Building.getRotName(Building.Rotate.Down);
            }
            //Спуск влево
            else if (posX > 1 && cellsData[posX, posY].height == (cellsData[posX - 1, posY].height + 1) && cellsData[posX - 1, posY].height == cellsData[posX - 2, posY].height && new Vector2Int(posX - 2, posY) != ignorePos && new Vector2Int(posX - 1, posY) != ignorePos && new Vector2Int(posX, posY) != ignorePos) {
                if (needBuild)
                    slopeObj = Instantiate(buildSlope);
                buildPos = new Vector2Int(posX - 1, posY);
                toRotate = Building.getRotName(Building.Rotate.Left);
            }
            //Спуск вправо
            else if (posX > 1 && cellsData[posX, posY].height == (cellsData[posX + 1, posY].height + 1) && cellsData[posX + 1, posY].height == cellsData[posX + 2, posY].height && new Vector2Int(posX + 2, posY) != ignorePos && new Vector2Int(posX + 1, posY) != ignorePos && new Vector2Int(posX, posY) != ignorePos) {
                if (needBuild)
                    slopeObj = Instantiate(buildSlope);
                buildPos = new Vector2Int(posX + 1, posY);
                toRotate = Building.getRotName(Building.Rotate.Right);
            }
            //спуск спереди
            else if (posX > 1 && cellsData[posX, posY].height == (cellsData[posX, posY + 1].height + 1) && cellsData[posX, posY + 1].height == cellsData[posX, posY + 2].height && new Vector2Int(posX, posY + 2) != ignorePos && new Vector2Int(posX, posY + 1) != ignorePos && new Vector2Int(posX, posY) != ignorePos) {
                if (needBuild)
                    slopeObj = Instantiate(buildSlope);
                buildPos = new Vector2Int(posX, posY + 1);
                toRotate = Building.getRotName(Building.Rotate.Up);
            }
        }



        //Если склон был построен
        if (slopeObj != null) {
            //Перемещаем на место
            slopeObj.transform.position = new Vector3(buildPos.x, cellsData[buildPos.x, buildPos.y].height, buildPos.y);

            //Поворачиваем
            //вытаскиваем компонент строения
            Building building = slopeObj.GetComponent<Building>();
            Quaternion rot = building.Platform.transform.rotation;

            if (toRotate == Building.getRotName(Building.Rotate.Down)) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, 180, 0);
            }
            else if (toRotate == Building.getRotName(Building.Rotate.Left)) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, -90, 0);
            }
            else if (toRotate == Building.getRotName(Building.Rotate.Right)) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, 90, 0);
            }
            else if (toRotate == Building.getRotName(Building.Rotate.Up)) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, 0, 0);
            }
            building.Platform.transform.rotation = rot; //применяем

            cellsData[buildPos.x, buildPos.y].build = Building.getTypeName(Building.Type.Slope); //даем тип ячейке
            cellsData[buildPos.x, buildPos.y].note = toRotate; //Запоминаем ориентацию спуска

            NetworkServer.Spawn(slopeObj);
        }

        return buildPos;
    }
    //Построить резкий склон вниз
    Vector2Int BuildSlopeDown2(short posX, short posY, bool needBuild) {
        //Позиция на котором был построен склон
        Vector2Int buildPos = new Vector2Int();
        GameObject slopeObj = null;
        string toRotate = "";

        //только если высота выше 1
        if (cellsData[posX, posY].height > 1) {
            //Сперва проверяем есть ли на тестируемой ячейке уже построенный спуск
            if (cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope)) {
                //Если спуск есть то продолжаем строительство в том же направлении
                //Вниз
                if (cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Down) && cellsData[posX, posY].height > cellsData[posX, posY - 1].height) {
                    //поднимаем ячейку на уровень подходящий для строительства спуска
                    if (needBuild) {
                        slopeObj = Instantiate(buildSlope);
                        cellsData[posX, posY - 1].height = (sbyte)(cellsData[posX, posY].height - 1);
                    }
                    buildPos = new Vector2Int(posX, posY - 1);
                    toRotate = Building.getRotName(Building.Rotate.Down);

                }
                //Влево
                else if (cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Left) && cellsData[posX, posY].height > cellsData[posX - 1, posY].height) {
                    if (needBuild) {
                        slopeObj = Instantiate(buildSlope);
                        cellsData[posX - 1, posY].height = (sbyte)(cellsData[posX, posY].height - 1);
                    }
                    buildPos = new Vector2Int(posX - 1, posY);
                    toRotate = Building.getRotName(Building.Rotate.Left);
                }
                //Вправо
                else if (cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Right) && cellsData[posX, posY].height > cellsData[posX + 1, posY].height) {
                    if (needBuild) {
                        slopeObj = Instantiate(buildSlope);
                        cellsData[posX + 1, posY].height = (sbyte)(cellsData[posX, posY].height - 1);
                    }
                    buildPos = new Vector2Int(posX + 1, posY);
                    toRotate = Building.getRotName(Building.Rotate.Right);
                }
                //Вперед
                else if (cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Up) && cellsData[posX, posY].height > cellsData[posX + 1, posY].height) {
                    if (needBuild) {
                        slopeObj = Instantiate(buildSlope);
                        cellsData[posX, posY + 1].height = (sbyte)(cellsData[posX, posY].height - 1);
                    }
                    buildPos = new Vector2Int(posX, posY + 1);
                    toRotate = Building.getRotName(Building.Rotate.Up);
                }
            }
            //Спуска нету
            else if (cellsData[posX, posY].build == "") {
                //Вниз
                if (posY > 1 && cellsData[posX, posY].height == (cellsData[posX, posY - 1].height + 1) && cellsData[posX, posY - 1].height == cellsData[posX, posY - 2].height + 1) {
                    if (needBuild)
                        slopeObj = Instantiate(buildSlope);
                    buildPos = new Vector2Int(posX, posY - 1);
                    toRotate = Building.getRotName(Building.Rotate.Down);
                }
                //слева
                else if (posX > 1 && cellsData[posX, posY].height == (cellsData[posX - 1, posY].height + 1) && cellsData[posX - 1, posY].height == cellsData[posX - 2, posY].height + 1) {
                    if (needBuild)
                        slopeObj = Instantiate(buildSlope);
                    buildPos = new Vector2Int(posX - 1, posY);
                    toRotate = Building.getRotName(Building.Rotate.Left);
                }
                //справа
                else if (posX < cellsData.GetLength(0) - 1 && cellsData[posX, posY].height == (cellsData[posX + 1, posY].height + 1) && cellsData[posX + 1, posY].height == cellsData[posX + 2, posY].height + 1) {
                    if (needBuild)
                        slopeObj = Instantiate(buildSlope);
                    buildPos = new Vector2Int(posX + 1, posY);
                    toRotate = Building.getRotName(Building.Rotate.Right);
                }
                //вверх
                else if (posY < cellsData.GetLength(1) - 1 && cellsData[posX, posY].height == (cellsData[posX, posY + 1].height + 1) && cellsData[posX, posY + 1].height == cellsData[posX, posY + 2].height + 1) {
                    if (needBuild)
                        slopeObj = Instantiate(buildSlope);
                    buildPos = new Vector2Int(posX, posY + 1);
                    toRotate = Building.getRotName(Building.Rotate.Up);
                }
            }
        }

        //Если склон был построен
        if (slopeObj != null) {
            //Перемещаем на место
            slopeObj.transform.position = new Vector3(buildPos.x, cellsData[buildPos.x, buildPos.y].height, buildPos.y);

            //Поворачиваем
            //вытаскиваем компонент строения
            Building building = slopeObj.GetComponent<Building>();
            Quaternion rot = building.Platform.transform.rotation;

            if (toRotate == Building.getRotName(Building.Rotate.Down)) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, 180, 0);
            }
            else if (toRotate == Building.getRotName(Building.Rotate.Left)) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, -90, 0);
            }
            else if (toRotate == Building.getRotName(Building.Rotate.Right)) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, 90, 0);
            }
            else if (toRotate == Building.getRotName(Building.Rotate.Up)) {
                rot.eulerAngles = new Vector3(rot.eulerAngles.x, 0, 0);
            }
            building.Platform.transform.rotation = rot; //применяем

            cellsData[buildPos.x, buildPos.y].build = Building.getTypeName(Building.Type.Slope); //даем тип ячейке
            cellsData[buildPos.x, buildPos.y].note = toRotate; //Запоминаем ориентацию спуска

            NetworkServer.Spawn(slopeObj);
        }

        return buildPos;
    }

    //проверяем возможность, прорубания ущелья до выхода в море
    Vector2Int PitWithNoExit(short posX, short posY, bool needBuild) {
        Vector2Int found = new Vector2Int();

        Building.Rotate rot1;
        Building.Rotate rot2;
        Building.Rotate rot3;
        Building.Rotate rot4;

        //Сперва определяем порядок поиска
        //Вниз
        if (posY < 30) {
            rot1 = Building.Rotate.Down;
            //низ-лево
            if (posX < 30) {
                rot2 = Building.Rotate.Left;
                rot3 = Building.Rotate.Right;
            }
            //низ-право
            else {
                rot2 = Building.Rotate.Right;
                rot3 = Building.Rotate.Left;
            }
            rot4 = Building.Rotate.Up;
        }
        //Вверх
        else {
            //верх лево
            if (posX < 30) {
                rot1 = Building.Rotate.Left;
                rot2 = Building.Rotate.Right;
            }
            //верх право
            else {
                rot1 = Building.Rotate.Right;
                rot2 = Building.Rotate.Left;
            }
            rot3 = Building.Rotate.Up; //Вдаль
            rot4 = Building.Rotate.Down; //На игрока
        }

        Vector2Int NULL = new Vector2Int();
        if (posX == 26 && posY == 21) {
            bool test = false;
        }

        if (found == NULL)
            found = testAndBuildLine(rot1);
        if (found == NULL)
            found = testAndBuildLine(rot2);
        if (found == NULL)
            found = testAndBuildLine(rot3);
        if (found == NULL)
            found = testAndBuildLine(rot4);

        return found;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Проверка и при необходимости прорыв каньена
        Vector2Int testAndBuildLine(Building.Rotate rot) {
            Vector2Int foundEnd = new Vector2Int();
            //Если вниз
            if (rot == Building.Rotate.Down) {
                //проверяем есть ли ячейка уровня повыше на которой нет маршрута
                if (posY > 0 && cellsData[posX, posY].height == cellsData[posX, posY - 1].height - 1 && traficNumTypeBuffer[posX, posY - 1, 0] < 0) {
                    //узнаем можно ли пройти насквозь до другой ячейки того же уровня
                    bool complite = false;
                    int offset = 0;
                    while (!complite) {
                        offset++;
                        //Если не край карты, высота выше исходной точки, нету трафика
                        if (!((posY - offset) > 0 && cellsData[posX, posY].height < cellsData[posX, posY - offset].height && traficNumTypeBuffer[posX, posY - offset, 0] < 0)) {
                            complite = true;
                        }

                        //Если завершилось и причина в достижении тогоже уровня
                        if (complite && cellsData[posX, posY].height == cellsData[posX, posY - offset].height && traficNumTypeBuffer[posX, posY - offset, 0] < 0) {
                            //То следующая точка найдена
                            foundEnd = new Vector2Int(posX, posY - 1);
                            //Копаем выход
                            if (needBuild) {
                                for (short y = 0; y < offset; y++) {
                                    cellsData[posX, posY - y].height = cellsData[posX, posY].height;
                                }
                            }
                        }
                    }
                }
            }
            //Если лево
            else if (rot == Building.Rotate.Left) {
                //проверяем есть ли ячейка уровня повыше на которой нет маршрута
                if (posX > 0 && cellsData[posX, posY].height == cellsData[posX - 1, posY].height - 1 && traficNumTypeBuffer[posX - 1, posY, 0] < 0) {
                    //узнаем можно ли пройти насквозь до другой ячейки того же уровня
                    bool complite = false;
                    int offset = 0;
                    while (!complite) {
                        offset++;
                        //Если не край карты, высота выше исходной точки, нету трафика
                        if (!((posX - offset) > 0 && cellsData[posX, posY].height < cellsData[posX - offset, posY].height && traficNumTypeBuffer[posX - offset, posY, 0] < 0)) {
                            complite = true;
                        }

                        //Если завершилось и причина в достижении тогоже уровня
                        if (complite && cellsData[posX, posY].height == cellsData[posX - offset, posY].height && traficNumTypeBuffer[posX - offset, posY, 0] < 0) {
                            //То следующая точка найдена
                            foundEnd = new Vector2Int(posX - 1, posY);
                            //Копаем выход
                            if (needBuild) {
                                for (short x = 0; x < offset; x++) {
                                    cellsData[posX - x, posY].height = cellsData[posX, posY].height;
                                }
                            }
                        }
                    }
                }
            }
            //Если право
            else if (rot == Building.Rotate.Right) {
                //проверяем есть ли ячейка уровня повыше на которой нет маршрута
                if (posX < 59 && cellsData[posX, posY].height == cellsData[posX + 1, posY].height - 1 && traficNumTypeBuffer[posX + 1, posY, 0] < 0) {
                    //узнаем можно ли пройти насквозь до другой ячейки того же уровня
                    bool complite = false;
                    int offset = 0;
                    while (!complite) {
                        offset++;
                        //Если не край карты, высота выше исходной точки, нету трафика
                        if (!((posX + offset) < 59 && cellsData[posX, posY].height < cellsData[posX + offset, posY].height && traficNumTypeBuffer[posX + offset, posY, 0] < 0)) {
                            complite = true;
                        }

                        //Если завершилось и причина в достижении тогоже уровня
                        if (complite && cellsData[posX, posY].height == cellsData[posX + offset, posY].height && traficNumTypeBuffer[posX + offset, posY, 0] < 0) {
                            //То следующая точка найдена
                            foundEnd = new Vector2Int(posX + 1, posY);
                            //Копаем выход
                            if (needBuild) {
                                for (short x = 0; x < offset; x++) {
                                    cellsData[posX + x, posY].height = cellsData[posX, posY].height;
                                }
                            }
                        }
                    }
                }
            }
            //Если верх
            else if (rot == Building.Rotate.Up) {
                //проверяем есть ли ячейка уровня повыше на которой нет маршрута
                if (posY < 59 && cellsData[posX, posY].height == cellsData[posX, posY + 1].height - 1 && traficNumTypeBuffer[posX, posY + 1, 0] < 0) {
                    //узнаем можно ли пройти насквозь до другой ячейки того же уровня
                    bool complite = false;
                    int offset = 0;
                    while (!complite) {
                        offset++;
                        //Если не край карты, высота выше исходной точки, нету трафика
                        if (!((posY + offset) < 59 && cellsData[posX, posY].height < cellsData[posX, posY + offset].height && traficNumTypeBuffer[posX, posY + offset, 0] < 0)) {
                            complite = true;
                        }

                        //Если завершилось и причина в достижении тогоже уровня
                        if (complite && cellsData[posX, posY].height == cellsData[posX, posY + offset].height && traficNumTypeBuffer[posX, posY + offset, 0] < 0) {
                            //То следующая точка найдена
                            foundEnd = new Vector2Int(posX, posY + 1);
                            //Копаем выход
                            if (needBuild) {
                                for (short y = 0; y < offset; y++) {
                                    cellsData[posX, posY + 1].height = cellsData[posX, posY].height;
                                }
                            }
                        }
                    }
                }
            }

            return foundEnd;
        }
    }

    //Проинициализировать последнюю ячейку на выход в море и спавн кораблей
    bool IniEndCell(short posX, short posY, bool needTest, int traficType) {
        bool SeaNear = false;

        List<WorldClass.Cell> testCellNow = new List<WorldClass.Cell>(); //Список для хранения текущих ячеек
        //Проверяем ячейки сбоку на наличие воды
        //Снизу
        if (posY > 0 && cellsData[posX, posY - 1].height == 0) {
            traficNumTypeBuffer[posX, posY - 1, traficType] = (short)(traficNumTypeBuffer[posX, posY, traficType] + 1);
            testCellNow.Add(cellsData[posX, posY - 1]);
            SeaNear = true;
        }
        //слева
        else if (posX > 0 && cellsData[posX - 1, posY].height == 0) {
            traficNumTypeBuffer[posX - 1, posY, traficType] = (short)(traficNumTypeBuffer[posX, posY, traficType] + 1);
            testCellNow.Add(cellsData[posX - 1, posY]);
            SeaNear = true;
        }
        //справа
        else if (posX < 59 && cellsData[posX + 1, posY].height == 0) {
            traficNumTypeBuffer[posX + 1, posY, traficType] = (short)(traficNumTypeBuffer[posX, posY, traficType] + 1);
            testCellNow.Add(cellsData[posX + 1, posY]);
            SeaNear = true;
        }
        //Сверху
        else if (posY < 59 && cellsData[posX, posY + 1].height == 0) {
            traficNumTypeBuffer[posX, posY + 1, traficType] = (short)(traficNumTypeBuffer[posX, posY, traficType] + 1);
            testCellNow.Add(cellsData[posX, posY + 1]);
            SeaNear = true;
        }

        //Если есть морская ячейка
        if (testCellNow.Count > 0 && needTest) {
            bool complite = false;
            while (!complite) {
                List<WorldClass.Cell> testCellNext = new List<WorldClass.Cell>(); //Список для хранения следующих ячеек

                //проверяем старые ячейки на то что граничат ли они с другими
                foreach (WorldClass.Cell c in testCellNow) {
                    //Если на проверяемой ячейке нет строения
                    if (c.build == "" || c.build == Building.getTypeName(Building.Type.EnemyBase)) {
                        //Проверка спереди, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (c.posY != 0 && cellsData[c.posX, c.posY - 1].height == c.height &&
                            traficNumTypeBuffer[c.posX, c.posY - 1, traficType] >= 9999999) {
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX, c.posY - 1, traficType] = (short)(traficNumTypeBuffer[c.posX, c.posY, traficType] + 1);
                            testCellNext.Add(cellsData[c.posX, c.posY - 1]);
                        }
                        //Проверка сзади, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (c.posY < 59 && cellsData[c.posX, c.posY + 1].height == c.height &&
                            traficNumTypeBuffer[c.posX, c.posY + 1, traficType] >= 9999999) {
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX, c.posY + 1, traficType] = (short)(traficNumTypeBuffer[c.posX, c.posY, traficType] + 1);
                            testCellNext.Add(cellsData[c.posX, c.posY + 1]);
                        }

                        //Проверка справа, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (c.posX < 59 && cellsData[c.posX + 1, c.posY].height == c.height &&
                            traficNumTypeBuffer[c.posX + 1, c.posY, traficType] >= 9999999) {
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX + 1, c.posY, traficType] = (short)(traficNumTypeBuffer[c.posX, c.posY, traficType] + 1);
                            testCellNext.Add(cellsData[c.posX + 1, c.posY]);
                        }

                        //Проверка слева, если не граничащая точка, если высота одинаковая, если ранее не пронумеровано или имеет маркер похуже
                        if (c.posX > 0 && cellsData[c.posX - 1, c.posY].height == c.height &&
                            traficNumTypeBuffer[c.posX - 1, c.posY, traficType] >= 9999999) {
                            //То маркируем и добавляем в список ячеек
                            traficNumTypeBuffer[c.posX - 1, c.posY, traficType] = (short)(traficNumTypeBuffer[c.posX, c.posY, traficType] + 1);
                            testCellNext.Add(cellsData[c.posX - 1, c.posY]);
                        }
                    }

                    //Если ячейка у границы карты
                    if (c.posY == 0 || c.posX == 0 || c.posX == 59 || c.posY == 59) {
                        CanSpawnFlot = true;
                    }
                }

                if (testCellNext.Count == 0) {
                    complite = true;
                }
                //Переключаемся на следующий шаг
                testCellNow = testCellNext;
            }
        }

        return SeaNear;
    }

    //Конвертируем информацию из сети в базу
    void EventCellNetToData(SyncList<WorldClass.Cell>.Operation op, int index, WorldClass.Cell cellOld, WorldClass.Cell cellNew) {
        //Только если это клиект
        if (isClient) {
            if (SyncListCellsNet != null && index < SyncListCellsNet.Count) {
                if (SyncListCellsNet[index].posX < cellsData.GetLength(0) && SyncListCellsNet[index].posX >= 0 &&
                    SyncListCellsNet[index].posY < cellsData.GetLength(1) && SyncListCellsNet[index].posY >= 0) {


                    if (cellsData[SyncListCellsNet[index].posX, SyncListCellsNet[index].posY].ownerSteamID != 0 &&
                        SyncListCellsNet[index].ownerSteamID == 0) {
                        int test = 0;
                    }

                    //Сохраняем данные ячейки
                    cellsData[SyncListCellsNet[index].posX, SyncListCellsNet[index].posY] = SyncListCellsNet[index];

                    if (cellCTRLs[SyncListCellsNet[index].posX, SyncListCellsNet[index].posY] != null) {
                        cellCTRLs[SyncListCellsNet[index].posX, SyncListCellsNet[index].posY].testHeight(true);
                        cellCTRLs[SyncListCellsNet[index].posX, SyncListCellsNet[index].posY].testColor();
                    }

                    //Создаем визуально ячейку если ее нет
                    TerrainCTRL.iniCell(SyncListCellsNet[index]);

                    //Устанавливаем время последнего обновления ячейки
                    cellsData[SyncListCellsNet[index].posX, SyncListCellsNet[index].posY].timeLastUpdate = Time.unscaledTime;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        IniGameplayCTRL();

        //Подписываем событие если данные изменились
        SyncListCellsNet.Callback += EventCellNetToData;
        iniInfoTextCtrl();

        EnemyMap = new InfoMap(true, 3);
        BuildMap = new InfoMap(true, 3);
        BuildMap.Color = new Color(1, 1, 1);

        AttentionMap = new InfoMap(true, 3);
    }

    // Update is called once per frame
    void Update() {
        TestGetAllPlayers();

        testReUpdateCell();

        TestGamePlay();

        testSpawnShip();

        getAllEnemy();

        EnemyMap.Update();
        BuildMap.Update();
        AttentionMap.Update();

        TestSoundMap();

        testLobby();

        autoReUpdateTrafic();
    }

    static public string keygenSymbols = "qwertyuiopasdfghjklzxcvbnm123456789";
    string GetRandomKeyGen() {
        string keyNew = "";

        //выбираем рандомное количество символов
        int countSumbols = Random.Range(3, 10);
        for (int sumNow = 0; sumNow < countSumbols; sumNow++) {
            //получаем рандомный мисвол
            char sumbol = keygenSymbols[Random.Range(0, keygenSymbols.Length)];
            keyNew += sumbol;
        }


        return keyNew;
    }

    public void KeyGenRandomMapClick() {
        if (isServer) {
            KeyGen = GetRandomKeyGen();
        }
    }
    public void KeyGenVerifiedMapClick()
    {
        if (GlobalLeaderBoardCTRL.main && GlobalLeaderBoardCTRL.main.SeedsVerified != null && isServer)
        {
            int num = Random.Range(0, GlobalLeaderBoardCTRL.main.SeedsVerified.ListUsers.Count);
            string KeyGenNew = "";
            GetNewKey();
            GetNewKey();
            GetNewKey();
            KeyGen = KeyGenNew;

            void GetNewKey() {
                if (KeyGenNew == KeyGen || KeyGenNew == "")
                {
                    num = Random.Range(0, GlobalLeaderBoardCTRL.main.SeedsVerified.ListUsers.Count);
                    if (GlobalLeaderBoardCTRL.main.SeedsVerified.ListUsers[num].details.Length > 0)
                        KeyGenNew = Calc.Convert.ToString(GlobalLeaderBoardCTRL.main.SeedsVerified.ListUsers[num].details);
                }
            }
        }
    }
    public void SetKeyGen(string NewKeyGen) {
        if (isServer) {
            KeyGen = NewKeyGen;
        }
    }


    public List<Player> players = new List<Player>();
    float timeToGetplayers = 0;

    [SerializeField][SyncVar]
    public int neirodata = 0;

    //Получить всех игроков на сервере
    void TestGetAllPlayers() {
        timeToGetplayers -= Time.unscaledDeltaTime;
        if (timeToGetplayers < 0) {
            timeToGetplayers = 3;
            GameObject[] playersObj = GameObject.FindGameObjectsWithTag("Player");
            if (playersObj != null) {
                List<Player> newPlayerList = new List<Player>();
                foreach (GameObject playerObj in playersObj) {
                    Player player = playerObj.GetComponent<Player>();
                    if (player != null) {
                        newPlayerList.Add(player);
                    }
                }

                //Если количество игроков увеличилось
                if (isServer && players.Count < newPlayerList.Count && cellsData != null) {
                    //Ищем нового игрока и обновляем для его данные, (отправляем ему данные)
                    for (int num = 0; num < newPlayerList.Count; num++) {
                        if (Time.unscaledTime - newPlayerList[num].TimeConnected < 5) {
                            

                            SyncListCellsNet.Clear();
                            lastReUpdateCellX = 0;
                            lastReUpdateCellY = 0;
                            NeedReUpdateCells = true;
                            break;
                        }
                    }

                    SetDirtyAllNetValue();
                }

                players = newPlayerList;
            }
        }

        //Если есть игроки, режим игры становится в режим ожидания игроков
        if (players.Count > 0 && gamemode < 1) {
            gamemode = 1;
        }

    }

    int lastReUpdateCellX = 0;
    int lastReUpdateCellY = 0;
    public bool NeedReUpdateCells = false;
    void testReUpdateCell() {
        if (NeedReUpdateCells && cellsData != null && isServer)
        {
            for (int testCellNum = 0; (testCellNum < (0.5f / Time.unscaledDeltaTime) + 1 && testCellNum < 10); testCellNum++)
            {
                //Отправляем ячейку
                addAndCellToSyncList(cellsData[lastReUpdateCellX, lastReUpdateCellY]);

                //Переключаемся на следующую
                lastReUpdateCellY++;

                //если кончелись Y
                if (lastReUpdateCellY >= cellsData.GetLength(1))
                {
                    lastReUpdateCellY = 0;
                    lastReUpdateCellX++;

                    //Если кончеличь X
                    if (lastReUpdateCellX >= cellsData.GetLength(0))
                    {
                        lastReUpdateCellX = 0;
                        NeedReUpdateCells = false;
                        break;
                    }
                }
            }
        }
    }

    float timeToReUpdateTrafic = 10;
    void autoReUpdateTrafic() {
        if (isServer) {
            timeToReUpdateTrafic -= Time.deltaTime;
            if (timeToReUpdateTrafic < 0) {
                timeToReUpdateTrafic = 10;
                CalcTraficAll(false, new Vector2Int(0, 0), true);
            }
        }
    }


    [SerializeField]
    GameObject RosketObj;

    void TestGamePlay() {
        testGamemodeDinamic();

        //Если режим ожидания игроков
        if (gamemode == 1) {
            //Камеру ставим по центру карты и чтобы смотрела вниз
            if (MainCamera.main) {
                MainCamera.main.transform.position = new Vector3(27, 24, 2);
                Quaternion rotate = MainCamera.main.transform.rotation;
                rotate.eulerAngles = new Vector3(65, 0, 0);
                MainCamera.main.transform.rotation = rotate;

                if (Player.me != null) {
                    Player.me.controlsMouse.ScrollHeight = 20;
                }
            }

            if (isServer) {
                timeScale = 1;
                //Считаем количество готовых игроков 
                int readyCount = 0;
                foreach (Player player in players) {
                    if (player.ReadyToStartGame) {
                        readyCount++;
                    }
                }

                if (players.Count <= 2) {
                    //если все игроки готовы
                    if (players.Count == readyCount) {
                        StartGamePlay1to2Mode();
                    }
                }
                else {
                    //Если больше половины готовы
                    if (players.Count / 2 <= readyCount) {
                        StartGamePlay1to2Mode();
                    }
                }
            }
        }
        //Если игра в самом разгаре
        else if (gamemode == 2) {
            timeGamePlay += Time.deltaTime;
            timeEpicSpawnInfantry -= Time.deltaTime;
            timeEpicSpawnAutomobile -= Time.deltaTime;
            timeEpicSpawnCrab -= Time.deltaTime;

            if (isServer) {
                float EpicMinus = timeGamePlay / 60;

                if (timeEpicSpawnInfantry < 0)
                    timeEpicSpawnInfantry = Random.Range(60, 300-(EpicMinus*10));
                if (timeEpicSpawnAutomobile < 0)
                    timeEpicSpawnAutomobile = Random.Range(60, 300-(EpicMinus*10));
                if (timeEpicSpawnCrab < 0)
                    timeEpicSpawnCrab = Random.Range(60, 300-(EpicMinus*10));

                //Закончить игру если база поражена
                if (BaceHealth <= 0) {
                    StartGamePlay2to3Mode(false);
                }

                TestAcceptEndGame();
                TestBadGen();
                TestTimeScale();
                //Проверка готовности игроков завершить игру
                void TestAcceptEndGame() {

                    int playersPlay = 0;
                    int playersReadyEnd = 0;

                    foreach (Player player in players) {
                        //Проверка играет ли игрок
                        if (player.ReadyToStartGame) {
                            playersPlay++;

                            //Проверка готовности закончить игру
                            if (player.ReadyToEndGame)
                            {
                                playersReadyEnd++;
                            }
                        }
                    }

                    //Считаем количество готовых закончить игру
                    if (playersPlay >= 1) {
                        if (playersPlay == playersReadyEnd) {
                            StartGamePlay2to3Mode(true);
                        }
                        else if (playersPlay >= 3 && playersPlay / 2 < playersReadyEnd) {
                            StartGamePlay2to3Mode(true);
                        }
                    }

                    if (BaceHealth <= 0)
                        StartGamePlay2to3Mode(false);
                }

                //Проверка на ошибку генерации
                void TestBadGen() {
                    if (!TestingMode && timeGamePlay > 1 && !CanSpawnFlot) {
                        GameoverVictory = false;
                        gamemode = 5;
                    }
                }
                
                void TestTimeScale()
                {
                    float timeScaleNeed = 1;

                    if (!pause)
                    {
                        //Проверяем время запуска ракеты
                        float gameplayMin = timeGamePlay / 60;
                        gameplayMin = gameplayMin % 10;

                        //Время возможного запуска
                        if (AttentionMap != null && AttentionMap.ActivePixelsOld > 0)
                        {
                            timeScaleNeed = 1;
                        }
                        else if (gameplayMin < 9)
                        {
                            //Перебираем всех игроков
                            int count = 0;
                            float sum = 0;
                            foreach (Player player in players)
                            {
                                if (player.ReadyToStartGame)
                                {
                                    count++;
                                    sum += player.timeScaleNeed;
                                }
                            }

                            if (count > 0)
                            {
                                timeScaleNeed = sum / count;
                            }
                            else
                            {
                                timeScaleNeed = 1;
                            }
                        }
                        else
                        {
                            timeScaleNeed = 1;
                        }
                    }
                    else {
                        timeScaleNeed = 0;
                    }

                    timeScale = timeScaleNeed;
                }
            }
        }

        //Завершаюшая сцена
        else if (gamemode == 3) {
            float victoryTime = 20;
            float defeatTime = 10;

            //Время идет если база жива или сцена показывалась меньше 3-х секунд
            if (BaceHealth > 0 || timeEndScene < defeatTime)
            {
                timeEndScene += Time.deltaTime;

                EndRosketScene();

                //Время взлета вышло значит победа
                if (timeEndScene > victoryTime)
                {

                    if (isServer)
                        StartGamePlay3to4Mode(true);
                }
            }
            //иначе если база не жива и время больше defeatTime, проигрываем
            else
            {
                if (isServer)
                    StartGamePlay3to4Mode(false);
            }


        }
        else if (gamemode == 4) {
            TestClearTerrain();

            //Если игроки готовы к рестарту игры
            testReStartGame();
            CellsDataToNull();
            CellsDataNetToNull();
        }
        else if (gamemode == 5) {
            TestClearTerrain();
            testReStartGame();
            CellsDataToNull();
            CellsDataNetToNull();
        }

        //Если игровой режим только что поменялся
        if (gamemode != gamemodeOld)
        {
            if (gamemode == 4)
            {
                if (GamePlayUICTRL.main.Gamemode4EndResult)
                {
                    GameResultCtrl gameResultCtrl = GamePlayUICTRL.main.Gamemode4EndResult.GetComponent<GameResultCtrl>();
                    gameResultCtrl.CloseOpenFon();
                }
            }

            //Обновить данные лобби
            if (isServer) {
                TimeLobbyToUpdate = 0;
                //for (int num = 0; num < SteamLobby.LobbyNeedParam.metaDatas.Length; num++) {
                    //if (SteamLobby.LobbyNeedParam.metaDatas[num].key == SteamLobby.LobbyKeys.mode)
                    //    SteamLobby.LobbyNeedParam.metaDatas[num].value = System.Convert.ToString(gamemode);
                //    else if (SteamLobby.LobbyNeedParam.metaDatas[num].key == SteamLobby.LobbyKeys.ip)
                //        SteamLobby.LobbyNeedParam.metaDatas[num].value = SteamLobby.MyIP.IPstr;
                //}
            }
        }

        Time.timeScale = timeScale;

        void ReStartGame() {
            gamemode = 1;

            GameoverVictory = false;
            CanSpawnFlot = false;
            timeGamePlay = 0;
            timeToPlay = 0;
            timeEndScene = 0;
            timeEpicSpawnAutomobile = 0;
            timeEpicSpawnCrab = 0;
            timeEpicSpawnInfantry = 0;

            BaceHealth = 100;
            neirodata = 0;

            kills = 0;
            foreach (Player player in players) {
                player.Money = 0;
                player.KillCar = 0;
                player.KillCrab = 0;
                player.KillPehota = 0;
                player.ReadyToEndGame = false;
                player.ReadyToRestartGame = false;
                player.ReadyToStartGame = false;
            }

            CellsDataToNull();
            CellsDataNetToNull();
            AICTRLsClear();

            Destroy(gameObject);
        }
        void CellsDataToNull() {
            cellsData = new WorldClass.Cell[60, 60];
        }
        void CellsDataNetToNull() {
            SyncListCellsNet.Clear();
        }
        void AICTRLsClear() {
            for (int num = 0; num < aICTRLs.Length; num++) {
                if (aICTRLs[num] != null) {
                    Destroy(aICTRLs[num].gameObject);
                }
                aICTRLs[num] = null;
            }
        }

        void StartGamePlay1to2Mode()
        {
            if (gamemode == 1)
            {
                gamemode = 2;
                //Все подключенные игроки становятся готовыми
                foreach (Player player in players)
                {
                    player.ReadyToStartGame = true;
                    //Делаем деньги
                    player.Money = 300;
                }

                //Инициализируем карту
                iniDataWorld();

                timeGamePlay = 0;
                timeEpicSpawnInfantry = Random.Range(60, 300);
                timeEpicSpawnAutomobile = Random.Range(60, 300);
                timeEpicSpawnCrab = Random.Range(60, 300);
            }
        }
        void StartGamePlay2to3Mode(bool TryVictory) {
            gamemode = 3;
        }
        //Закончить игру и установить победу или поражение
        void StartGamePlay3to4Mode(bool victory)
        {
            timeGameplayEnd = Time.unscaledTime;
            GameoverVictory = victory;
            gamemode = 4;

            if (victory)
            {
                //Отправить всем игрокам победу
                foreach (Player player in players) {
                    player.TypeSoundPlayNeed = 100;
                }
            }
            else {
                //Отправить всем игрокам поражение
                foreach (Player player in players)
                {
                    player.TypeSoundPlayNeed = 101;
                }
            }

        }

        void EndRosketScene() {
            if (!RosketObj) {
                RosketObj = GameObject.FindGameObjectWithTag("BaceRosket");
            }


            if (RosketObj && BaceHealth > 0)
            {
                float startUpRosket = 5;
                //Взлет ракеты
                if (timeEndScene > startUpRosket)
                {
                    RosketObj.transform.position = new Vector3(RosketObj.transform.position.x, RosketObj.transform.position.y + ((timeEndScene-startUpRosket) / 2.5f) * Time.unscaledDeltaTime, RosketObj.transform.position.z);
                }
            }
        }

        void testReStartGame()
        {
            if (isServer)
            {
                int playersMax = 0;
                int playersReady = 0;
                foreach (Player player in players)
                {
                    if (player.ReadyToStartGame)
                    {
                        playersMax++;
                        if (player.ReadyToRestartGame)
                        {
                            playersReady++;
                        }
                    }
                }

                if (playersMax <= 2 && playersReady == playersMax)
                {
                    ReStartGame();
                }
                else if (playersReady > playersMax / 2)
                {
                    ReStartGame();
                }
            }
        }
        void TestClearTerrain()
        {
            for (int x = 0; x < cellCTRLs.GetLength(0); x++)
            {
                for (int y = 0; y < cellCTRLs.GetLength(1); y++)
                {
                    if (cellCTRLs[x, y] != null)
                    {
                        Destroy(cellCTRLs[x, y].gameObject);
                    }
                }
            }
        }

        //Чтобы у подключившихся во время игры, изменилось состояние игры
        void testGamemodeDinamic() {
            if (isServer)
            {
                int sostoanie = (int)(Time.unscaledTime % 2);
                if (sostoanie == 0)
                    gamemodeDinamic = 0;
                else gamemodeDinamic = gamemode;

                SetDirtyAllNetValue();

            }
            else {
                if (gamemodeDinamic != 0 && gamemode != gamemodeDinamic)
                    gamemode = gamemodeDinamic;
            }
        }

        gamemodeOld = gamemode;
    }

    public InfoTextCTRL infoText;
    void iniInfoTextCtrl() {
        if (infoText == null) {
            GameObject infoTextObj = GameObject.FindGameObjectWithTag("InfoText");
            if (infoTextObj != null) {
                infoText = infoTextObj.GetComponent<InfoTextCTRL>();
            }
        }
    }

    public class InfoMap {
        bool isUpdateTime = true; //должно ли идти время для этой карты
        public bool IsUpdateTime{ get { return isUpdateTime; } set{ isUpdateTime = value; } }
        float time = 0;
        float[,] data = new float[60, 60];

        //Время меньше которого пиксель активируется
        public float TrigerTime = 1;

        Color color = new Color(1, 0, 0);
        public Color Color { set { color = value;} }

        Texture2D texture = new Texture2D(60, 60);
        int lastPixels = 0; //Последняя обновленная строка пикселей

        //Для подсчета активных пикселей
        public int ActivePixelsOld = 0;
        int ActivePixelsNow = 0;

        public InfoMap(bool updateTime, float trigerTime) {
            if (updateTime)
            {
                time = 0;
            }
            else {
                time -= trigerTime + 1;
                TrigerTime = trigerTime;
            }
        }

        public void Update() {
            //Нужно ли увеличивать время
            if (isUpdateTime) {
                time += Time.unscaledDeltaTime;
            }

            //Рисуем карту
            int maxUpdate = 5;
            for (int plus = 0; plus < maxUpdate; plus++) {
                if (lastPixels + plus < texture.height) {
                    for (int x = 0; x < texture.width; x++) {
                        //если время пришло рисуем пиксель
                        if (time - data[x, lastPixels + plus] < TrigerTime)
                        {
                            ActivePixelsNow++;
                            texture.SetPixel(x, lastPixels + plus, color);
                        }
                        //иначе - прозрачный
                        else
                        {
                            texture.SetPixel(x, lastPixels + plus, new Color(0, 0, 0, 0));
                        }
                    }
                }
            }
            //Если дошли до конца
            if (lastPixels + maxUpdate >= texture.height) {
                lastPixels = 0;
                ActivePixelsOld = ActivePixelsNow;
                ActivePixelsNow = 0;
            }
            //если нет - переключаем шаг
            else {
                lastPixels += maxUpdate;
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();
        }

        public void SetData(Vector2Int array) {
            if (array.x >= 0 && array.y >= 0 && array.x < data.GetLength(0) && array.y < data.GetLength(1)) {
                data[array.x, array.y] = time;
            }
        }
        public void ClearData(Vector2Int array)
        {
            if (array.x >= 0 && array.y >= 0 && array.x < data.GetLength(0) && array.y < data.GetLength(1))
            {
                data[array.x, array.y] = -9999;
            }
        }

        public Texture2D GetTexture2D() {
            return texture;
        }
    }
    public InfoMap EnemyMap;
    public InfoMap BuildMap;
    public InfoMap AttentionMap;

    public void TestSoundMap() {
        if (SoundTurretCTRL.SoundMap == null)
        {
            SoundTurretCTRL.SoundMap = new InfoMap(true, 5);
        }
        else {
            SoundTurretCTRL.SoundMap.Update();

        }
    }

    [SerializeField]
    [SyncVar]
    public float BaceHealth = 100;
    public float timeDamage = 0; //Время последнего урона
    public float timeAttention = 0; //Время последнего предупреждения
    public void TestAttentionMap() {
        if (AttentionMap != null)
        {
            AttentionMap = new InfoMap(true, 1);
        }
        else {
            AttentionMap.Update();
        }
    }

    [SerializeField] 
    [SyncVar]
    public float BaceReady = 0;


    void testSpawnShip() {

        GameObject[] ShipObjs = GameObject.FindGameObjectsWithTag("Ship");

        //Спавним корабль? если сервер, есть маршрут, префаб корабля есть, и сейчас кораблей меньше чем нужно
        if (isServer && gamemode == 2 && CanSpawnFlot && enemyShip != null && ShipObjs.Length < (timeGamePlay+60)/600) {
            GameObject shipObj = Instantiate(enemyShip);
            Vector2 vectorSpawn = (new Vector2(Random.RandomRange(-1.0f, 1.0f), Random.RandomRange(-1.0f, 1.0f))).normalized;
            shipObj.transform.position = new Vector3((vectorSpawn.x * 60) + 30, -1f, (vectorSpawn.y * 60) + 30);

            NetworkServer.Spawn(shipObj);
        }
    }

    public List<AICTRL> AllEnemy = new List<AICTRL>();
    int EnemyTestID = 0;
    void getAllEnemy() {

        if (EnemyTestID != (int)timeGamePlay) {
            EnemyTestID = (int)timeGamePlay;
            //Создаем новый список
            AllEnemy = new List<AICTRL>();

            //Вытаскиваем все обьеты с тегом враг
            GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");

            //Перебираем всех врагов добавляя их в список
            foreach (GameObject enemyObj in enemyObjs) {
                AICTRL aICTRL = enemyObj.GetComponent<AICTRL>();
                if (aICTRL != null) {
                    AllEnemy.Add(aICTRL);
                }
            }
        }
    }

    /// <summary>
    /// Время окончания геймплея, от победы или поражения
    /// </summary>
    [SyncVar] public float timeGameplayEnd = 0;
    [SyncVar] public bool GameoverVictory = false;

    float TimeLobbyToUpdate = 0;
    void testLobby()
    {
        TimeLobbyToUpdate -= Time.unscaledDeltaTime;

        if (isServer)
        {
            //Если лобби есть и я владелец
            if (SteamLobby.lobbyNow != null && SteamLobby.lobbyNow.ownerSID.m_SteamID == Steamworks.SteamUser.GetSteamID().m_SteamID)
            {
                //обновляем номер лобби если отличается
                if (Time.unscaledTime % 2 > 1)
                {
                    if (SteamLobby.lobbyNow.lobbySID.m_SteamID != lobbyID) lobbyID = SteamLobby.lobbyNow.lobbySID.m_SteamID;
                }
                else {
                    lobbyID = 0;
                }

                //Если пришло время для обновления
                if (TimeLobbyToUpdate < 0)
                {
                    TimeLobbyToUpdate = 60;

                    string GameModeStr = "";
                    if (gamemode == 0)
                        GameModeStr = "Error";
                    else if (gamemode == 1)
                        GameModeStr = "Waiting";
                    else if (gamemode == 2)
                        GameModeStr = "Playing";
                    else if (gamemode == 3 || gamemode == 4 || gamemode == 5)
                        GameModeStr = "Gameover";

                    for (int num = 0; num < SteamLobby.LobbyNeedParam.metaDatas.Length; num++)
                    {
                        if (SteamLobby.LobbyNeedParam.metaDatas[num].key == SteamLobby.LobbyKeys.mode)
                            SteamLobby.LobbyNeedParam.metaDatas[num].value = System.Convert.ToString(GameModeStr);
                        else if (SteamLobby.LobbyNeedParam.metaDatas[num].key == SteamLobby.LobbyKeys.hostAddres)
                            SteamLobby.LobbyNeedParam.metaDatas[num].value = Steamworks.SteamUser.GetSteamID().ToString();
                        else if (SteamLobby.LobbyNeedParam.metaDatas[num].key == SteamLobby.LobbyKeys.timeIsPlay)
                            SteamLobby.LobbyNeedParam.metaDatas[num].value = "" + (int)(timeGamePlay / 60) + ":" +(int)(timeGamePlay % 60);
                    }
                }
            }
        }
        else if (isClient)
        {
            //Если клиент не в лобби или лобби отличается
            if (lobbyID > 1 && SteamLobby.lobbyNow != null && SteamLobby.lobbyNow.lobbySID.m_SteamID != lobbyID)
            {
                Debug.Log("Join lobby ID " + lobbyID);
                //подключаемся к лобби этого сервера
                Steamworks.CSteamID lobbySID = new Steamworks.CSteamID(lobbyID);
                SteamLobby.JoinLobby(lobbySID);
            }
        }
    }

    //Изменить все сетевые переменные чтобы они отправились по сети.
    void SetDirtyAllNetValue() {

        var Buffer001 = KeyGen;
        KeyGen = null;
        KeyGen = Buffer001;

        var Buffer002 = lobbyID;
        lobbyID = 0;
        lobbyID = Buffer002;

        var Buffer003 = gamemode;
        gamemode = 999999;
        gamemode = Buffer003;

        var Buffer004 = HightPoint;
        HightPoint = new Vector2();
        HightPoint = Buffer004;

        var buffer005 = neirodata;
        neirodata = 0;
        neirodata = buffer005;

        var buffer006 = BaceHealth;
        BaceHealth = 0;
        BaceHealth = buffer006;

        var buffer007 = BaceReady;
        BaceReady = 0;
        BaceReady = buffer007;

        GameoverVictory = !GameoverVictory;
        GameoverVictory = !GameoverVictory;
    }
    
}
