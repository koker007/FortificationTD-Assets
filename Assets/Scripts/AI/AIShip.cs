using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro.EditorUtilities;
using UnityEngine;
using Mirror;

public class AIShip : NetworkBehaviour
{
    [SerializeField]
    Animator animator;

    static public int count = 0;

    [SerializeField]
    GameObject TraficVizualizator;
    float timeToTraficVisualize = 0;

    [SerializeField]
    GameObject[] AIEnemies;

    [SerializeField]
    bool Testing = false;
    [SerializeField]
    GameObject AITest;

    [SerializeField]
    GameplayCTRL GPCTRL;

    [SerializeField]
    Transform PositionSpawn;

    [SerializeField]
    float speed = 1;

    int numSpawn = 0;

    void iniGameplayCTRL() {
        if (GPCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                GPCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        iniGameplayCTRL();
    }

    // Update is called once per frame
    void Update()
    {
        //testMove();
        testMove();
        testSpawn();

        testAnimator();

        TestDestroy();
    }

    [SerializeField]
    Vector3 posNeed = new Vector3(-1, -1, -1); //Целевая позиция
    float rotNeedPlus = 0;
    float rotNeedMinus = 0;

    void testMove() {
        if (gameObject.transform.position.x > 0 && gameObject.transform.position.x < 59 && gameObject.transform.position.z > 0 && gameObject.transform.position.z < 59)
            transformInside();
        //Если лодка за пределами карты
        else
            transformOutside();

        //Движение в пределах карты
        void transformInside() {
            if (GPCTRL != null && GPCTRL.cellsData != null) {
                //Находим позицию лодки
                short posX = (short)gameObject.transform.position.x;
                short posY = (short)gameObject.transform.position.z;

                //Выбираем следуюшую точку если необходимо
                if (posNeed == new Vector3(-1, -1, -1) || Vector3.Distance(gameObject.transform.position, posNeed) < 0.25f) {
                    CalcNextCell(posX, posY);
                }


                //Двигаемся вперед
                if (cellSpawn.posX == 0 && cellSpawn.posY == 0 && Vector3.Distance(gameObject.transform.position, posNeed) > 0.05f) {
                    gameObject.transform.position += (posNeed - gameObject.transform.position).normalized * Time.deltaTime;
                }

                //Узнаем направление необходимое для поворота
                //Ближе вращать в минус
                if (Math.Abs(rotNeedMinus - gameObject.transform.rotation.eulerAngles.y) < Math.Abs(rotNeedPlus - gameObject.transform.rotation.eulerAngles.y)) {
                    //Уменьшаем
                    Vector3 rotNew = new Vector3(
                        gameObject.transform.rotation.eulerAngles.x, 
                        gameObject.transform.rotation.eulerAngles.y + (rotNeedMinus - gameObject.transform.rotation.eulerAngles.y) * Time.deltaTime, 
                        gameObject.transform.rotation.eulerAngles.z);
                    Quaternion rotNewQuater = new Quaternion();
                    rotNewQuater.eulerAngles = rotNew;
                    gameObject.transform.rotation = rotNewQuater;
                }
                //Ближе вращать в плюс
                else {
                    //Прибавляем
                    Vector3 rotNew = new Vector3(
                        gameObject.transform.rotation.eulerAngles.x,
                        gameObject.transform.rotation.eulerAngles.y + (rotNeedPlus - gameObject.transform.rotation.eulerAngles.y) * Time.deltaTime,
                        gameObject.transform.rotation.eulerAngles.z);
                    Quaternion rotNewQuater = new Quaternion();
                    rotNewQuater.eulerAngles = rotNew;
                    gameObject.transform.rotation = rotNewQuater;
                }
            }

            //Перерасчитать следуюшую точку в маршруте
            void CalcNextCell(int posX, int posY) {
                //Ищем точку с ближайшим растоянием до базы
                posNeed = new Vector3(posX + 0.5f, 0.8f, posY + 0.5f);

                //Проверки маршрута
                //Лево-низ
                if (posY > 0 && GPCTRL.cellsData[posX, posY - 1].height == 0 &&
                    posX > 0 && GPCTRL.cellsData[posX - 1, posY].height == 0 &&
                    GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY - 1].traficNum) {
                    posNeed = new Vector3(posX - 1 + 0.5f, 0.8f, posY - 1 + 0.5f);
                    rotNeedPlus = 225;
                    rotNeedMinus = -135;
                }
                //право-низ
                else if (posY > 0 && GPCTRL.cellsData[posX, posY - 1].height == 0 &&
                    posX < 59 && GPCTRL.cellsData[posX + 1, posY].height == 0 &&
                    GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY - 1].traficNum) {
                    posNeed = new Vector3(posX + 1 + 0.5f, 0.8f, posY - 1 + 0.5f);
                    rotNeedPlus = 135;
                    rotNeedMinus = -225;
                }
                //право-верх
                else if (posY < 59 && GPCTRL.cellsData[posX, posY + 1].height == 0 &&
                    posX < 59 && GPCTRL.cellsData[posX + 1, posY].height == 0 &&
                    GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY + 1].traficNum) {
                    posNeed = new Vector3(posX + 1 + 0.5f, 0.8f, posY + 1 + 0.5f);
                    rotNeedPlus = 45;
                    rotNeedMinus = -315;
                }
                //лево-верх
                else if (posY < 59 && GPCTRL.cellsData[posX, posY + 1].height == 0 &&
                    posX > 0 && GPCTRL.cellsData[posX - 1, posY].height == 0 &&
                    GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY + 1].traficNum) {
                    posNeed = new Vector3(posX - 1 + 0.5f, 0.8f, posY + 1 + 0.5f);
                    rotNeedPlus = 315;
                    rotNeedMinus = -45;
                }
                //снизу
                else if (posY > 0 && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY - 1].traficNum && GPCTRL.cellsData[posX, posY - 1].height == 0) {
                    posNeed = new Vector3(posX + 0.5f, 0.8f, posY - 1 + 0.5f);
                    rotNeedPlus = 180;
                    rotNeedMinus = -180;
                }
                //Слева
                else if (posX > 0 && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY].traficNum && GPCTRL.cellsData[posX - 1, posY].height == 0) {
                    posNeed = new Vector3(posX - 1 + 0.5f, 0.8f, posY + 0.5f);
                    rotNeedPlus = 270;
                    rotNeedMinus = -90;
                }
                //Справа
                else if (posX < 59 && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY].traficNum && GPCTRL.cellsData[posX + 1, posY].height == 0) {
                    posNeed = new Vector3(posX + 1 + 0.5f, 0.8f, posY + 0.5f);
                    rotNeedPlus = 90;
                    rotNeedMinus = -270;
                }
                //Сверху
                else if (posY < 59 && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY + 1].traficNum && GPCTRL.cellsData[posX, posY + 1].height == 0) {
                    posNeed = new Vector3(posX + 0.5f, 0.8f, posY + 1 + 0.5f);
                    rotNeedPlus = 360;
                    rotNeedMinus = 0;
                }
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Двигаться некуда - Ищем берег
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //снизу
                else if (posY > 0 && GPCTRL.cellsData[posX, posY - 1].height == 1) {
                    rotNeedPlus = 180;
                    rotNeedMinus = -180;
                    cellSpawn.posX = (sbyte)posX;
                    cellSpawn.posY = (sbyte)(posY - 1);
                    needOpen = true;
                }
                //слева
                else if (posX > 0 && GPCTRL.cellsData[posX - 1, posY].height == 1) {
                    rotNeedPlus = 270;
                    rotNeedMinus = -90;
                    cellSpawn.posX = (sbyte)(posX -1);
                    cellSpawn.posY = (sbyte)posY;
                    needOpen = true;
                }
                //справа
                else if (posX < 59 && GPCTRL.cellsData[posX + 1, posY].height == 1) {
                    rotNeedPlus = 90;
                    rotNeedMinus = -270;
                    cellSpawn.posX = (sbyte)(posX + 1);
                    cellSpawn.posY = (sbyte)posY;
                    needOpen = true;
                }
                //сверху
                else if (posY < 59 && GPCTRL.cellsData[posX, posY + 1].height  == 1) {
                    rotNeedPlus = 360;
                    rotNeedMinus = 0;
                    cellSpawn.posX = (sbyte)posX;
                    cellSpawn.posY = (sbyte)(posY + 1);
                    needOpen = true;
                }

            }
        }

        //Движение за пределами карты
        void transformOutside() {
            //Двигаемся прямо в центр
            gameObject.transform.position += (new Vector3(29, 0.8f, 29) - gameObject.transform.position).normalized * Time.deltaTime;
            if (transform.position.y > 0.8f) {
                transform.position = new Vector3(transform.position.x, 0.8f, transform.position.z);
            }
            else {
                transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * 0.20f, transform.position.z);
            }
            //берем вектор в котором нужно повернуть
            //Vector3 vectorMove = (new Vector3(29, 0.8f, 29) - gameObject.transform.position).normalized;
            //вытаскиваем позицию
            //Quaternion quaternionRotate = gameObject.transform.rotation;
            gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(29, 0.8f, 29) - transform.position), Time.deltaTime * 90);
            
        }
    }

    WorldClass.Cell cellSpawn;
    float[] timeTospawn;
    [SerializeField]
    float timespawnMax = 60;
    void testSpawn() {
        //Спавнить может только сервер
        if (isServer && GPCTRL != null && GPCTRL.gamemode >= 2 && GPCTRL.gamemode < 4 && needOpen)
        {
            if (!Testing)
            {
                if (timeTospawn == null)
                {
                    timeTospawn = new float[AIEnemies.Length];

                    //Стартовая задержка
                    for (int num = 0; num < timeTospawn.Length; num++)
                    {
                        timeTospawn[num] = 4;
                    }

                }

                for (int num = 0; num < timeTospawn.Length; num++)
                {
                    timeTospawn[num] -= Time.deltaTime;

                    //Проверка эпичности
                    if (num == 0 && AICTRL.EpicTime.Infantry > GPCTRL.timeEpicSpawnInfantry && timeTospawn[num] > timespawnMax / 15)
                    {
                        timeTospawn[num] = timespawnMax / 15;
                    }
                    if (num == 1 && AICTRL.EpicTime.Automobile > GPCTRL.timeEpicSpawnAutomobile && timeTospawn[num] > timespawnMax / 10)
                    {
                        timeTospawn[num] = timespawnMax / 10;
                    }
                    if (num == 2 && AICTRL.EpicTime.crab > GPCTRL.timeEpicSpawnCrab && timeTospawn[num] > timespawnMax / (2 + (GPCTRL.timeGamePlay / 1200)))
                    {
                        timeTospawn[num] = timespawnMax / 10;
                    }
                    // /////////////////////////////////////////////////////////////////////////////////

                    if (timeTospawn[num] < 0 && cellSpawn.posX != 0 && cellSpawn.posY != 0)
                    {
                        //пехота
                        if (num == 0)
                        {
                            if (GPCTRL.timeEpicSpawnInfantry < AICTRL.EpicTime.Infantry)
                                timeTospawn[num] = timespawnMax / 15;//(GPCTRL.timeGamePlay/200);
                            else
                                timeTospawn[num] = timespawnMax;
                        }
                        //автомобиль
                        else if (num == 1)
                        {
                            if (GPCTRL.timeEpicSpawnAutomobile < AICTRL.EpicTime.Automobile)
                                timeTospawn[num] = timespawnMax / 10;
                            else
                                timeTospawn[num] = timespawnMax;
                        }
                        //краб
                        else if (num == 2)
                        {
                            if (GPCTRL.timeEpicSpawnCrab < AICTRL.EpicTime.crab)
                                timeTospawn[num] = (timespawnMax * 1.5f) / (2 + (GPCTRL.timeGamePlay / 1200));
                            else
                                timeTospawn[num] = timespawnMax * 3;
                        }

                        GameObject SpawnObj = null;
                        if (AIEnemies != null)
                        {
                            //Создаем врага
                            SpawnObj = Instantiate(AIEnemies[num]);
                        }

                        if (SpawnObj != null)
                        {
                            SpawnObj.transform.position = PositionSpawn.position;
                            AICTRL aICTRL = SpawnObj.GetComponent<AICTRL>();
                            aICTRL.cellPosNext = new Vector3(cellSpawn.posX + 0.5f, 0, cellSpawn.posY + 0.5f);
                            aICTRL.SyncRandomTraficNext = numSpawn;
                            numSpawn++;
                            aICTRL.ID = AICTRL.EnemyIDLast;
                            AICTRL.EnemyIDLast++;

                            int LVLNOW = (int)(GPCTRL.timeGamePlay / 600);

                            //Усложнение в зависимости от длины карты и скорости врага
                            float plusTraficSec = 0;
                            if (GameplayCTRL.main)
                                plusTraficSec = (GameplayCTRL.main.traficLenght * 0.80f) / aICTRL.speed;

                            //один из тысячи появляется супер сильным
                            if (UnityEngine.Random.Range(0, 100) < (GPCTRL.timeGamePlay / 400) - 0.5f)
                            {
                                float HarderSec = 520;
                                aICTRL.healthNow = (float)Math.Pow(aICTRL.healthBasic, 0.5f + (GPCTRL.timeGamePlay + HarderSec + plusTraficSec) / HarderSec); ;
                                if (LVLNOW < 5)
                                    aICTRL.reward = aICTRL.reward * (1 + GPCTRL.timeGamePlay / (HarderSec * 3));
                                else aICTRL.reward = aICTRL.reward * (1 + GPCTRL.timeGamePlay / (HarderSec * 16));

                                aICTRL.healthStart = aICTRL.healthNow;
                                aICTRL.super = true;
                            }
                            //обычный
                            else
                            {
                                float HarderSec = 600; //Количество секунд для усложнения игры
                                                       //Здоровье согласно времени игры
                                aICTRL.healthNow = (float)Math.Pow(aICTRL.healthBasic, (GPCTRL.timeGamePlay + HarderSec + plusTraficSec) / HarderSec);
                                if (LVLNOW < 5)
                                    aICTRL.reward = aICTRL.reward * (1 + GPCTRL.timeGamePlay / (HarderSec * 6));
                                else aICTRL.reward = aICTRL.reward * (GPCTRL.timeGamePlay / (HarderSec * 16));

                                aICTRL.healthStart = aICTRL.healthNow;
                                aICTRL.super = false;
                            }

                            NetworkServer.Spawn(SpawnObj);
                        }
                    }
                }
            }

            //Тестовый враг
            else if (AITest) {
                GameObject[] aICTRLs = GameObject.FindGameObjectsWithTag("Enemy");
                if (aICTRLs.Length <= 0) {
                    //Создаем врага
                    GameObject spawnObj = Instantiate(AITest);
                    if (spawnObj)
                    {
                        spawnObj.transform.position = PositionSpawn.position;
                        AICTRL aICTRL = spawnObj.GetComponent<AICTRL>();
                        aICTRL.cellPosNext = new Vector3(cellSpawn.posX + 0.5f, 0, cellSpawn.posY + 0.5f);
                        aICTRL.SyncRandomTraficNext = numSpawn;
                        numSpawn++;
                        aICTRL.ID = AICTRL.EnemyIDLast;
                        AICTRL.EnemyIDLast++;

                        int LVLNOW = (int)(GPCTRL.timeGamePlay / 600);

                        //Усложнение в зависимости от длины карты и скорости врага
                        float plusTraficSec = 0;
                        if (GameplayCTRL.main)
                            plusTraficSec = (GameplayCTRL.main.traficLenght * 0.80f) / aICTRL.speed;

                        float HarderSec = 600; //Количество секунд для усложнения игры
                                               //Здоровье согласно времени игры
                        aICTRL.healthNow = (float)Math.Pow(aICTRL.healthBasic, (GPCTRL.timeGamePlay + HarderSec + plusTraficSec) / HarderSec);
                        if (LVLNOW < 5)
                            aICTRL.reward = aICTRL.reward * (1 + GPCTRL.timeGamePlay / (HarderSec * 6));
                        else aICTRL.reward = aICTRL.reward * (GPCTRL.timeGamePlay / (HarderSec * 16));

                        aICTRL.healthStart = aICTRL.healthNow;
                        aICTRL.super = false;

                        NetworkServer.Spawn(spawnObj);
                    }
                }
            }
        }

        if (TraficVizualizator != null && !Testing) {
            timeToTraficVisualize -= Time.unscaledDeltaTime;

            if (timeToTraficVisualize < 0 && cellSpawn.posX != 0 && cellSpawn.posY != 0)
            {
                timeToTraficVisualize = 30;
                GameObject SpawnObj = null;
                if (AIEnemies != null)
                {
                    SpawnObj = Instantiate(TraficVizualizator);
                }

                if (SpawnObj != null)
                {
                    SpawnObj.transform.position = PositionSpawn.position;
                    AICTRL aICTRL = SpawnObj.GetComponent<AICTRL>();
                    aICTRL.cellPosNext = new Vector3(cellSpawn.posX + 0.5f, 0, cellSpawn.posY + 0.5f);
                }
            }
        }
    }

    void TestDestroy() {
        if (GameplayCTRL.main && (GameplayCTRL.main.gamemode == 4 || GameplayCTRL.main.gamemode == 1)) {
            Destroy(gameObject);
        }
    }

    [SyncVar]
    bool needOpen = false;
    void testAnimator() {
        if (animator) {
            animator.SetBool("Opening", needOpen);
        }
    }

    AIShip() {
        count++;
    }
    ~AIShip() {
        count--;
    }
}
