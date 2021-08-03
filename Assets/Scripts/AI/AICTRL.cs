using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Mirror;

public class AICTRL : NetworkBehaviour
{
    static public int EnemyIDLast = 0;

    [Header("Parametrs")]
    [SyncVar]
    public int ID = 0;
    [SyncVar]
    public float healthNow = 10;
    [SyncVar]
    public float healthStart = 10;
    public float healthBasic = 10;
    [SyncVar]
    public bool super = false;
    [SerializeField]
    float OffsetMax = 0;
    [SyncVar]
    public Vector2 offset;

    public Vector3 posOld = new Vector3();

    [SerializeField]
    float damage = 0;
    [SerializeField]
    public float reward = 0;
    [SerializeField]
    int neirodata = 1;

    [SerializeField]
    [Range(0, 15f)]
    public float speed = 1;
    [SerializeField]
    float SlopeSpeed = 1;

    [Header("Effects")]
    [SerializeField]
    [SyncVar]
    float timeStunning = 0;
    [SerializeField]
    [SyncVar]
    float timeSlowing = 0;
    [SerializeField]
    float SpeedCoof = 0;

    [SyncVar]
    bool kiled = false;
    [SerializeField]
    TypeEnemy typeEnemy = TypeEnemy.TraficVisualizator;
    int visualizatorTypeNum = -1;

    struct DamageInfo {
        public Player player;
        public float damage;

        public DamageInfo(Player newPlayer, float newDamage) {
            player = newPlayer;
            damage = newDamage;
        }
        public void PlusDamage(float funcDamage) {
            damage += funcDamage;
        }
    }
    List<DamageInfo> DamageList = new List<DamageInfo>();

    [SyncVar]
    bool TraficNextMin = true; //Следующий трафик поменьше или такой же
    [SyncVar]
    public int SyncRandomTraficNext = 0; //синхронизированное рандомное число для трафика

    [Header("Color")]
    [SerializeField]
    int numMatColor = 0;
    [SerializeField]
    SkinnedMeshRenderer meshRender;

    //Тип врага
    public enum TypeEnemy {
        TraficVisualizator,
        Pehota,
        Car,
        Crab,
    }
    public static class EpicTime{
        public const float Infantry = 10;
        public const float Automobile = 10;
        public const float crab = 10;
    }

    //Получить врага на которого нужно нацеливаться из конкретного места и конкретными способами
    static public AICTRL getEnemy(Vector3 posLook, float radius, Building.TargetMode targetMode, bool needVisual)
    {
        AICTRL target = null;
        List<AICTRL> Enemys = new List<AICTRL>();

        if (GameplayCTRL.main) {

            //Проверяем есть ли враги которые попадают в поле зрения
            foreach (AICTRL aICTRL in GameplayCTRL.main.AllEnemy)
            {
                if (aICTRL && Vector3.Distance(aICTRL.gameObject.transform.position, posLook) < radius)
                {
                    Enemys.Add(aICTRL);
                }
            }

            //выбираем метод фильтрации цели
            if (targetMode == Building.TargetMode.DistMin)
                DistMin();
            else if (targetMode == Building.TargetMode.DistMax)
                DistMax();
            else if (targetMode == Building.TargetMode.HeathMin)
                HeathMin();
            else if (targetMode == Building.TargetMode.HeathMax)
                HeathMax();
            else if (targetMode == Building.TargetMode.SpeedMin)
                SpeedMin();
            else if (targetMode == Building.TargetMode.SpeedMax)
                SpeedMax();
            else if (targetMode == Building.TargetMode.TraficMin)
                TraficMin();
            else if (targetMode == Building.TargetMode.TraficMax)
                TraficMax();
        }

        return target;

        void DistMin()
        {
            //Выбираем цель по близости
            foreach (AICTRL aICTRL in Enemys)
            {
                //только если враг живой
                if (aICTRL.healthNow > 0)
                {
                    if (needVisual)
                    {
                        //только если врага видно
                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, добавляем врага в список
                        if (Physics.Raycast(posLook, aICTRL.transform.position - posLook, out raycastHit, radius) && raycastHit.collider.gameObject == aICTRL.gameObject)
                            TestTarget();
                    }
                    else {
                        TestTarget();
                    }
                }

                void TestTarget() {
                    if (target != null && Vector3.Distance(target.transform.position, posLook) > Vector3.Distance(aICTRL.transform.position, posLook))
                    {
                        target = aICTRL;
                    }
                    else if (target == null)
                    {
                        target = aICTRL;
                    }
                }
            }
        }
        void DistMax()
        {
            //Выбираем цель по близости
            foreach (AICTRL aICTRL in Enemys)
            {
                //только если враг живой
                if (aICTRL.healthNow > 0)
                {
                    if (needVisual)
                    {
                        //только если врага видно
                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, добавляем врага в список
                        if (Physics.Raycast(posLook, aICTRL.transform.position - posLook, out raycastHit, radius) && raycastHit.collider.gameObject == aICTRL.gameObject)
                            TestTarget();
                    }
                    else
                    {
                        TestTarget();
                    }
                }

                void TestTarget()
                {
                    if (target != null && Vector3.Distance(target.transform.position, posLook) < Vector3.Distance(aICTRL.transform.position, posLook))
                    {
                        target = aICTRL;
                    }
                    else if (target == null)
                    {
                        target = aICTRL;
                    }
                }
            }
        }
        void HeathMin()
        {
            //Выбираем цель по близости
            foreach (AICTRL aICTRL in Enemys)
            {
                //только если враг живой
                if (aICTRL.healthNow > 0)
                {
                    if (needVisual)
                    {
                        //только если врага видно
                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, добавляем врага в список
                        if (Physics.Raycast(posLook, aICTRL.transform.position - posLook, out raycastHit, radius) && raycastHit.collider.gameObject == aICTRL.gameObject)
                            TestTarget();
                    }
                    else
                    {
                        TestTarget();
                    }
                }

                void TestTarget()
                {
                    if (target != null && target.healthNow > aICTRL.healthNow)
                    {
                        target = aICTRL;
                    }
                    else if (target == null)
                    {
                        target = aICTRL;
                    }
                }
            }
        }
        void HeathMax()
        {
            //Выбираем цель по близости
            foreach (AICTRL aICTRL in Enemys)
            {
                //только если враг живой
                if (aICTRL.healthNow > 0)
                {
                    if (needVisual)
                    {
                        //только если врага видно
                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, добавляем врага в список
                        if (Physics.Raycast(posLook, aICTRL.transform.position - posLook, out raycastHit, radius) && raycastHit.collider.gameObject == aICTRL.gameObject)
                            TestTarget();
                    }
                    else
                    {
                        TestTarget();
                    }
                }

                void TestTarget()
                {
                    if (target != null && target.healthNow < aICTRL.healthNow)
                    {
                        target = aICTRL;
                    }
                    else if (target == null)
                    {
                        target = aICTRL;
                    }
                }
            }
        }
        void SpeedMin()
        {

            //Выбираем цель по близости
            foreach (AICTRL aICTRL in Enemys)
            {
                //только если враг живой
                if (aICTRL.healthNow > 0)
                {
                    if (needVisual)
                    {
                        //только если врага видно
                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, добавляем врага в список
                        if (Physics.Raycast(posLook, aICTRL.transform.position - posLook, out raycastHit, radius) && raycastHit.collider.gameObject == aICTRL.gameObject)
                            TestTarget();
                    }
                    else
                    {
                        TestTarget();
                    }
                }

                void TestTarget()
                {
                    if (target != null && target.speed > aICTRL.speed)
                    {
                        target = aICTRL;
                    }
                    else if (target == null)
                    {
                        target = aICTRL;
                    }
                }
            }
        }
        void SpeedMax()
        {
            //Выбираем цель по близости
            foreach (AICTRL aICTRL in Enemys)
            {
                //только если враг живой
                if (aICTRL.healthNow > 0)
                {
                    if (needVisual)
                    {
                        //только если врага видно
                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, добавляем врага в список
                        if (Physics.Raycast(posLook, aICTRL.transform.position - posLook, out raycastHit, radius) && raycastHit.collider.gameObject == aICTRL.gameObject)
                            TestTarget();
                    }
                    else
                    {
                        TestTarget();
                    }
                }

                void TestTarget()
                {
                    if (target != null && target.speed < aICTRL.speed)
                    {
                        target = aICTRL;
                    }
                    else if (target == null)
                    {
                        target = aICTRL;
                    }
                }
            }
        }
        void TraficMin()
        {

            //Выбираем цель по близости
            foreach (AICTRL aICTRL in Enemys)
            {
                //только если враг живой
                if (aICTRL.healthNow > 0)
                {
                    if (needVisual)
                    {
                        //только если врага видно
                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, добавляем врага в список
                        if (Physics.Raycast(posLook, aICTRL.transform.position - posLook, out raycastHit, radius) && raycastHit.collider.gameObject == aICTRL.gameObject)
                            TestTarget();
                    }
                    else
                    {
                        TestTarget();
                    }
                }

                void TestTarget()
                {
                    if (target != null && target.traficNow > aICTRL.traficNow)
                    {
                        target = aICTRL;
                    }
                    else if (target == null)
                    {
                        target = aICTRL;
                    }
                }
            }
        }
        void TraficMax()
        {
            //Выбираем цель по близости
            foreach (AICTRL aICTRL in Enemys)
            {
                //только если враг живой
                if (aICTRL.healthNow > 0)
                {
                    if (needVisual)
                    {
                        //только если врага видно
                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, добавляем врага в список
                        if (Physics.Raycast(posLook, aICTRL.transform.position - posLook, out raycastHit, radius) && raycastHit.collider.gameObject == aICTRL.gameObject)
                            TestTarget();
                    }
                    else
                    {
                        TestTarget();
                    }
                }

                void TestTarget()
                {
                    if (target != null && target.traficNow < aICTRL.traficNow)
                    {
                        target = aICTRL;
                    }
                    else if (target == null)
                    {
                        target = aICTRL;
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ReCalcColor", 0.5f);
        if (GameplayCTRL.main && GameplayCTRL.main.aICTRLs != null && GameplayCTRL.main.aICTRLs.Length > ID) {
            GameplayCTRL.main.aICTRLs[ID] = this;
        }

        //смещение движения от центра
        offset = new Vector2(UnityEngine.Random.Range(-OffsetMax, OffsetMax), UnityEngine.Random.Range(-OffsetMax,OffsetMax));
    }

    // Update is called once per frame
    void Update()
    {
        if (GPCTRL)
        {
            TestDistToNext();
            TestLive();
            TestMove();
            TestRotate();
            TestAnimator();
            //ReCalcColor();
            TestBaceDamage();
        }
        else iniGameplay();
    }

    enum toMove{
        Left,
        Right,
        Forvard,
        Back
    }

    [Header("Trafic")]
    [SyncVar]
    public Vector3 cellPosNext = new Vector3();

    [Header("Animation")]
    public int CountDead = 0;
    public int CountStun = 0;

    [Header("Other")]
    [SerializeField]
    GameplayCTRL GPCTRL; //GameplayCTRL
    public float traficNow = 9999999;
    void iniGameplay() {
        if (GPCTRL == null) {
            GPCTRL = GameplayCTRL.main;
        }
    }

    //проверка на то что нужно ли переключиться на новую точку
    void TestDistToNext() {
        if (GPCTRL == null) {
            iniGameplay();
        }
        //Если дистанция слишком маленькая то переключаемся на следуюшую точку
        else if (Vector2.Distance(new Vector2(cellPosNext.x, cellPosNext.z), new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)) < 0.1f) {
            if (typeEnemy == TypeEnemy.Car) {
                testTraficType0();
            }
            else if (typeEnemy == TypeEnemy.Pehota) {
                testTraficType1();
            }
            else if (typeEnemy == TypeEnemy.Crab) {
                testTraficType2();
            }
            else if (typeEnemy == TypeEnemy.TraficVisualizator) {

                if (visualizatorTypeNum == 0) {
                    testTraficType0();
                }
                else if (visualizatorTypeNum == 1) {
                    testTraficType1();
                }
                else if (visualizatorTypeNum == 2) {
                    testTraficType2();
                }
                else if (visualizatorTypeNum < 0)
                {
                    visualizatorTypeNum = UnityEngine.Random.Range(0, 3);
                }
            }
        }
    
        void testTraficType0() {
            //Ячейка сейчас
            WorldClass.Cell cellNow = GPCTRL.cellsData[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z];
            //Ячейка следуюшая
            //WorldClass.Cell cellNext;
            //cellNext.posX = 0;
            //cellNext.posY = 0;

            //Варианты следующих ячеек
            List<WorldClass.Cell> cellsTraficMin = new List<WorldClass.Cell>(); //Варианты с трафиком поменьше
            List<WorldClass.Cell> cellsTraficNorm = new List<WorldClass.Cell>(); //Варианты с трафиком такимже

            //вытаскиваем ячейку
            int posX = (int)gameObject.transform.position.x;
            int posY = (int)gameObject.transform.position.z;

            //Если таже самая высота
            //для трафика поменьше
            //Движения наискось
            //Вниз-лево
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY - 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && (GPCTRL.cellsData[posX - 1, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY - 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY - 1].height
                && GPCTRL.cellsData[posX - 1, posY - 1].traficNum >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY - 1]);
            }
            //Вниз-право
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY - 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && (GPCTRL.cellsData[posX + 1, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY - 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY - 1].height
                && GPCTRL.cellsData[posX + 1, posY - 1].traficNum >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY - 1]);
            }
            //Вверх-право
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY + 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && (GPCTRL.cellsData[posX + 1, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY + 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY + 1].height
                && GPCTRL.cellsData[posX + 1, posY + 1].traficNum >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY + 1]);
            }
            //Вверх-лево
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY + 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && (GPCTRL.cellsData[posX - 1, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY + 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY + 1].height
                && GPCTRL.cellsData[posX - 1, posY + 1].traficNum >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY + 1]);
            }
            //Прямые движения
            //вниз
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None) 
                || GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX, posY - 1] != null && !GPCTRL.buildings[posX, posY - 1].traficBlocker[0])) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY - 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && GPCTRL.cellsData[posX, posY - 1].traficNum >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY - 1]);
            }
            //влево
            if (posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None) 
                || GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX- 1, posY].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX - 1, posY] != null && !GPCTRL.buildings[posX - 1, posY].traficBlocker[0])) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && GPCTRL.cellsData[posX - 1, posY].traficNum >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY]);
            }
            //вправо
            if (posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None) 
                || GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX + 1, posY] != null && !GPCTRL.buildings[posX + 1, posY].traficBlocker[0])) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && GPCTRL.cellsData[posX + 1, posY].traficNum >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY]);
            }
            //вверх
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None) 
                || GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX, posY + 1] != null && !GPCTRL.buildings[posX, posY + 1].traficBlocker[0])) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY + 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && GPCTRL.cellsData[posX, posY + 1].traficNum >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY + 1]);
            }

            //Если склон
            //Переход на высоту повыше
            //Наверх
            if ((posY < 59 && GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY + 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height && GPCTRL.cellsData[posX, posY + 1].note == Building.getRotName(Building.Rotate.Down)) ||
                (posY < 59 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY + 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Down)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY + 1]);
            }
            //влево
            else if ((posX > 0 && GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height && GPCTRL.cellsData[posX - 1, posY].note == Building.getRotName(Building.Rotate.Right)) ||
                (posX > 0 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX - 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Right)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY]);
            }
            //вправо
            else if ((posX < 59 && GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height && GPCTRL.cellsData[posX + 1, posY].note == Building.getRotName(Building.Rotate.Left)) ||
                (posX < 59 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX + 1, posY].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Left)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY]);
            }
            //вниз
            else if ((posY > 0 && GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY - 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height && GPCTRL.cellsData[posX, posY - 1].note == Building.getRotName(Building.Rotate.Up)) ||
                (posY > 0 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum > GPCTRL.cellsData[posX, posY - 1].traficNum && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Up)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY - 1]);
            }

            //если выбрана следующая позиция
            if (cellsTraficMin.Count > 0)
            {
                WorldClass.Cell cellNext;
                //Если трафик не на понижение
                if (!TraficNextMin && cellsTraficNorm.Count > 0)
                {
                    int randNext = SyncRandomTraficNext % cellsTraficNorm.Count;
                    cellNext = cellsTraficNorm[randNext];
                }
                //Трафик на понижение
                else
                {
                    int randNext = SyncRandomTraficNext % cellsTraficMin.Count;
                    cellNext = cellsTraficMin[randNext];
                }

                //ставим позицию на нее
                //cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x, 0, cellNext.posY + 0.5f + offset.y);

                //если на следующей ячейке ограничитель трафика то идем по центру
                if (GPCTRL.cellsData[cellNext.posX, cellNext.posY].build == Building.getTypeName(Building.Type.TraficBlocker))
                {
                    cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x / 4, 0, cellNext.posY + 0.5f + offset.y / 4);
                }
                //Обычная позиция
                else
                {
                    //ставим позицию на нее
                    cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x, 0, cellNext.posY + 0.5f + offset.y);
                }

                traficNow = cellNext.traficNum;

                //Рандомизируем числа если сервер
                if (isServer)
                {
                    if (UnityEngine.Random.Range(0f, 100f) < 0)
                        TraficNextMin = false;
                    else
                        TraficNextMin = true;

                    //SyncRandomTraficNext = UnityEngine.Random.Range(0, 100);

                }
            }
        }
        void testTraficType1() {
            //Ячейка сейчас
            WorldClass.Cell cellNow = GPCTRL.cellsData[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z];
            //Ячейка следуюшая
            //WorldClass.Cell cellNext;
            //cellNext.posX = 0;
            //cellNext.posY = 0;

            //Варианты следующих ячеек
            List<WorldClass.Cell> cellsTraficMin = new List<WorldClass.Cell>(); //Варианты с трафиком поменьше
            List<WorldClass.Cell> cellsTraficNorm = new List<WorldClass.Cell>(); //Варианты с трафиком такимже

            //вытаскиваем ячейку
            int posX = (int)gameObject.transform.position.x;
            int posY = (int)gameObject.transform.position.z;

            //Если таже самая высота
            //для трафика поменьше
            //Движения наискось
            //Вниз-лево
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY - 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX - 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && (GPCTRL.cellsData[posX - 1, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX - 1, posY - 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY - 1].height
                && GPCTRL.cellsData[posX - 1, posY - 1].traficNum1 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY - 1]);
            }
            //Вниз-право
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY - 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX + 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && (GPCTRL.cellsData[posX + 1, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX + 1, posY - 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY - 1].height
                && GPCTRL.cellsData[posX + 1, posY - 1].traficNum1 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY - 1]);
            }
            //Вверх-право
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY + 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX + 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && (GPCTRL.cellsData[posX + 1, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX + 1, posY + 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY + 1].height
                && GPCTRL.cellsData[posX + 1, posY + 1].traficNum1 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY + 1]);
            }
            //Вверх-лево
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY + 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX - 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && (GPCTRL.cellsData[posX - 1, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX - 1, posY + 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY + 1].height
                && GPCTRL.cellsData[posX - 1, posY + 1].traficNum1 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY + 1]);
            }


            //Прямые движения 1
            //вниз
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None)
                || GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX, posY - 1] != null && !GPCTRL.buildings[posX, posY - 1].traficBlocker[1])) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY - 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && GPCTRL.cellsData[posX, posY - 1].traficNum1 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY - 1]);
            }
            //влево
            if (posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None)
                || GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX - 1, posY] != null && !GPCTRL.buildings[posX - 1, posY].traficBlocker[1])) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX - 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && GPCTRL.cellsData[posX - 1, posY].traficNum1 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY]);
            }
            //вправо
            if (posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None)
                || GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX + 1, posY] != null && !GPCTRL.buildings[posX + 1, posY].traficBlocker[1])) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX + 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && GPCTRL.cellsData[posX + 1, posY].traficNum1 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY]);
            }
            //вверх
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None)
                || GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX, posY + 1] != null && !GPCTRL.buildings[posX, posY + 1].traficBlocker[1])) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY + 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && GPCTRL.cellsData[posX, posY + 1].traficNum1 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY + 1]);
            }

            //Если склон
            //Переход на высоту повыше
            //Наверх
            if ((posY < 59 && GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY + 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height && GPCTRL.cellsData[posX, posY + 1].note == Building.getRotName(Building.Rotate.Down)) ||
                (posY < 59 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY + 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Down)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY + 1]);
            }
            //влево
            else if ((posX > 0 && GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX - 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height && GPCTRL.cellsData[posX - 1, posY].note == Building.getRotName(Building.Rotate.Right)) ||
                (posX > 0 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX - 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Right)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY]);
            }
            //вправо
            else if ((posX < 59 && GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX + 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height && GPCTRL.cellsData[posX + 1, posY].note == Building.getRotName(Building.Rotate.Left)) ||
                (posX < 59 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX + 1, posY].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Left)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY]);
            }
            //вниз
            else if ((posY > 0 && GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY - 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height && GPCTRL.cellsData[posX, posY - 1].note == Building.getRotName(Building.Rotate.Up)) ||
                (posY > 0 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum1 > GPCTRL.cellsData[posX, posY - 1].traficNum1 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Up)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY - 1]);
            }

            //если выбрана следующая позиция
            if (cellsTraficMin.Count > 0)
            {
                WorldClass.Cell cellNext;
                //Если трафик не на понижение
                if (!TraficNextMin && cellsTraficNorm.Count > 0)
                {
                    int randNext = SyncRandomTraficNext % cellsTraficNorm.Count;
                    cellNext = cellsTraficNorm[randNext];
                }
                //Трафик на понижение
                else
                {
                    int randNext = SyncRandomTraficNext % cellsTraficMin.Count;
                    cellNext = cellsTraficMin[randNext];
                }

                //ставим позицию на нее
                //cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x, 0, cellNext.posY + 0.5f + offset.y);

                //если на следующей ячейке ограничитель трафика то идем по центру
                if (GPCTRL.cellsData[cellNext.posX, cellNext.posY].build == Building.getTypeName(Building.Type.TraficBlocker))
                {
                    cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x / 4, 0, cellNext.posY + 0.5f + offset.y / 4);
                }
                //Обычная позиция
                else
                {
                    //ставим позицию на нее
                    cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x, 0, cellNext.posY + 0.5f + offset.y);
                }

                traficNow = cellNext.traficNum1;

                //Рандомизируем числа если сервер
                if (isServer)
                {
                    if (UnityEngine.Random.Range(0f, 100f) < 0)
                        TraficNextMin = false;
                    else
                        TraficNextMin = true;

                    //SyncRandomTraficNext = UnityEngine.Random.Range(0, 100);

                }
            }
        }
        void testTraficType2() {
            //Ячейка сейчас
            WorldClass.Cell cellNow = GPCTRL.cellsData[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z];
            //Ячейка следуюшая
            //WorldClass.Cell cellNext;
            //cellNext.posX = 0;
            //cellNext.posY = 0;

            //Варианты следующих ячеек
            List<WorldClass.Cell> cellsTraficMin = new List<WorldClass.Cell>(); //Варианты с трафиком поменьше
            List<WorldClass.Cell> cellsTraficNorm = new List<WorldClass.Cell>(); //Варианты с трафиком такимже

            //вытаскиваем ячейку
            int posX = (int)gameObject.transform.position.x;
            int posY = (int)gameObject.transform.position.z;

            //Если таже самая высота
            //для трафика поменьше
            //Движения наискось
            //Вниз-лево
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY - 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX - 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && (GPCTRL.cellsData[posX - 1, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX - 1, posY - 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY - 1].height
                && GPCTRL.cellsData[posX - 1, posY - 1].traficNum2 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY - 1]);
            }
            //Вниз-право
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY - 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX + 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && (GPCTRL.cellsData[posX + 1, posY - 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX + 1, posY - 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY - 1].height
                && GPCTRL.cellsData[posX + 1, posY - 1].traficNum2 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY - 1]);
            }
            //Вверх-право
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY + 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX + 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && (GPCTRL.cellsData[posX + 1, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX + 1, posY + 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY + 1].height
                && GPCTRL.cellsData[posX + 1, posY + 1].traficNum2 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY + 1]);
            }
            //Вверх-лево
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY + 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX - 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && (GPCTRL.cellsData[posX - 1, posY + 1].build == Building.getTypeName(Building.Type.None)) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX - 1, posY + 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY + 1].height
                && GPCTRL.cellsData[posX - 1, posY + 1].traficNum2 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY + 1]);
            }

            //Прямые движения
            //вниз
            if (posY > 0 && (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.None)
                || GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX, posY - 1] != null && !GPCTRL.buildings[posX, posY - 1].traficBlocker[2])) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY - 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height
                && GPCTRL.cellsData[posX, posY - 1].traficNum2 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY - 1]);
            }
            //влево
            if (posX > 0 && (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.None)
                || GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX - 1, posY] != null && !GPCTRL.buildings[posX - 1, posY].traficBlocker[2])) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX - 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height
                && GPCTRL.cellsData[posX - 1, posY].traficNum2 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY]);
            }
            //вправо
            if (posX < 59 && (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.None)
                || GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX + 1, posY] != null && !GPCTRL.buildings[posX + 1, posY].traficBlocker[2])) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX + 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height
                && GPCTRL.cellsData[posX + 1, posY].traficNum2 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY]);
            }
            //вверх
            if (posY < 59 && (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.None)
                || GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.Base)
                || (GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.TraficBlocker) && GPCTRL.buildings[posX, posY + 1] != null && !GPCTRL.buildings[posX, posY + 1].traficBlocker[2])) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY + 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height
                && GPCTRL.cellsData[posX, posY + 1].traficNum2 >= 0)
            {
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY + 1]);
            }

            //Если склон
            //Переход на высоту повыше
            //Наверх
            if ((posY < 59 && GPCTRL.cellsData[posX, posY + 1].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY + 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height && GPCTRL.cellsData[posX, posY + 1].note == Building.getRotName(Building.Rotate.Down)) ||
                (posY < 59 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY + 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY + 1].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Down)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY + 1]);
            }
            //влево
            else if ((posX > 0 && GPCTRL.cellsData[posX - 1, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX - 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height && GPCTRL.cellsData[posX - 1, posY].note == Building.getRotName(Building.Rotate.Right)) ||
                (posX > 0 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX - 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX - 1, posY].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Right)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX - 1, posY]);
            }
            //вправо
            else if ((posX < 59 && GPCTRL.cellsData[posX + 1, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX + 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height && GPCTRL.cellsData[posX + 1, posY].note == Building.getRotName(Building.Rotate.Left)) ||
                (posX < 59 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX + 1, posY].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX + 1, posY].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Left)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX + 1, posY]);
            }
            //вниз
            else if ((posY > 0 && GPCTRL.cellsData[posX, posY - 1].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY - 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height && GPCTRL.cellsData[posX, posY - 1].note == Building.getRotName(Building.Rotate.Up)) ||
                (posY > 0 && GPCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Slope) && GPCTRL.cellsData[posX, posY].traficNum2 > GPCTRL.cellsData[posX, posY - 1].traficNum2 && GPCTRL.cellsData[posX, posY].height == GPCTRL.cellsData[posX, posY - 1].height - 1 && GPCTRL.cellsData[posX, posY].note == Building.getRotName(Building.Rotate.Up)))
            {
                cellsTraficMin = new List<WorldClass.Cell>();
                cellsTraficNorm = new List<WorldClass.Cell>();
                cellsTraficMin.Add(GPCTRL.cellsData[posX, posY - 1]);
            }

            //если выбрана следующая позиция
            if (cellsTraficMin.Count > 0)
            {
                WorldClass.Cell cellNext;
                //Если трафик не на понижение
                if (!TraficNextMin && cellsTraficNorm.Count > 0)
                {
                    int randNext = SyncRandomTraficNext % cellsTraficNorm.Count;
                    cellNext = cellsTraficNorm[randNext];
                }
                //Трафик на понижение
                else
                {
                    int randNext = SyncRandomTraficNext % cellsTraficMin.Count;
                    cellNext = cellsTraficMin[randNext];
                }

                //ставим позицию на нее
                //cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x, 0, cellNext.posY + 0.5f + offset.y);

                //если на следующей ячейке ограничитель трафика то идем по центру
                if (GPCTRL.cellsData[cellNext.posX, cellNext.posY].build == Building.getTypeName(Building.Type.TraficBlocker)) {
                    cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x/2, 0, cellNext.posY + 0.5f + offset.y/2);
                }
                //Обычная позиция
                else {
                    //ставим позицию на нее
                    cellPosNext = new Vector3(cellNext.posX + 0.5f + offset.x, 0, cellNext.posY + 0.5f + offset.y);
                }
                traficNow = cellNext.traficNum2;

                //Рандомизируем числа если сервер
                if (isServer)
                {
                    if (UnityEngine.Random.Range(0f, 100f) < 0)
                        TraficNextMin = false;
                    else
                        TraficNextMin = true;

                    //SyncRandomTraficNext = UnityEngine.Random.Range(0, 100);

                }
            }
        }
    }

    float timeToStartDown = 1.5f;

    int AnimkillNum = 0;
    bool AnimSlope = false;
    void TestLive() {
        //Делаем цель мертвой
        if (healthNow <= 0 && !kiled) {
            kiled = true;

            //прибавляем опасности ячейке
            if (isServer && GPCTRL != null) {
                GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].traficDanger = (ushort)(GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].traficDanger + damage);
                //Границы и порог опасности
                if (GPCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].traficDanger < 1)
                    GPCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].traficDanger = 1;
                else if (GPCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].traficDanger > 1000)
                    GPCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].traficDanger = 1000;

                //нужно синхронизировать
                GPCTRL.CalcTraficAll(false, new Vector2Int(0, 0), true);
            }

            AnimkillNum = UnityEngine.Random.Range(0, CountDead);

            Collider[] colliders = gameObject.GetComponents<Collider>();
            foreach (Collider collider in colliders) {
                Destroy(collider);
            }

            int LVLGame = (int)(GPCTRL.timeGamePlay / 600);
            float moneyCoof = 1;
            if (LVLGame == 0) moneyCoof = 2f;
            else if (LVLGame == 1) moneyCoof = 1.5f;
            else if (LVLGame == 2) moneyCoof = 1f;
            else if (LVLGame == 3) moneyCoof = 0.6f;
            else if (LVLGame == 4) moneyCoof = 0.3f;
            else if (LVLGame == 5) moneyCoof = 0.2f;
            else if (LVLGame == 6) moneyCoof = 0.1f;
            else moneyCoof = 0.01f;

            //Добавляем денег игрокам
            if (GPCTRL != null) {
                foreach (Player player in GPCTRL.players) {
                    float plusMoney = (reward * moneyCoof) / GPCTRL.players.Count;
                    player.Money += plusMoney;

                    Player.MoneyOfTime moneyOfTime;
                    moneyOfTime.money = plusMoney;
                    moneyOfTime.time = Time.time;
                    player.ChainsMoney.AddChain(moneyOfTime);
                }
            }

            //Прибавялем убийство игроку, только если сервер
            if (isServer) {
                List<DamageInfo> PlayerDamageList = new List<DamageInfo>();

                //Добавляем убийство игрокам
                foreach (DamageInfo info in DamageList) {
                    //Ищем игрока которому принадлежит убийство
                    bool found = false;
                    foreach (DamageInfo infoSum in PlayerDamageList) {
                        if (infoSum.player == info.player && info.player != null) {
                            infoSum.PlusDamage(info.damage);
                            found = true;
                            break;
                        }
                    }

                    //Если игрок и его урон не был найден, создаем
                    if (!found && info.player != null) {
                        PlayerDamageList.Add(new DamageInfo(info.player, info.damage));
                    }
                }

                //Теперь количество урона посчитано
                //Нужно прибавить убийство игроку с большим уроном
                DamageInfo FirstDamage = new DamageInfo(null, 0);
                foreach (DamageInfo itogDamage in PlayerDamageList) {
                    if (FirstDamage.player == null)
                        FirstDamage = itogDamage;

                    if (FirstDamage.damage < itogDamage.damage) {
                        FirstDamage = itogDamage;
                    }
                }

                //прибавляем убийство
                if (FirstDamage.player != null) {
                    if (typeEnemy == TypeEnemy.Pehota)
                        FirstDamage.player.KillPehota++;
                    else if (typeEnemy == TypeEnemy.Car)
                        FirstDamage.player.KillCar++;
                    else if (typeEnemy == TypeEnemy.Crab)
                        FirstDamage.player.KillCrab++;

                    GPCTRL.kills++;
                }

                GPCTRL.neirodata += (int)(neirodata * (1 + GPCTRL.timeGamePlay/600));
            }

        }
        //если игра закончилась
        else if (isServer && GPCTRL && GPCTRL.gamemode == 4) {
            Destroy(gameObject);
        }
    }
    void TestMove() {
        posOld = transform.position;

        if (typeEnemy != TypeEnemy.TraficVisualizator)
        {
            //Движение
            if (cellPosNext != new Vector3() && GPCTRL != null && healthNow > 0 && timeStunning <= 0)
            {
                //Корректируем высоту
                Vector3 posNow = gameObject.transform.position;
                float height = GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].height;
                if (height < 1)
                    height = 1;

                //проверяем не будет ли перебор с перемещением
                if (Vector3.Distance(cellPosNext, posNow) > Time.deltaTime * speed)
                {
                    //Если подьем
                    if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].build == Building.getTypeName(Building.Type.Slope))
                    {
                        //Перемещаем со скоростью подьема
                        posNow += (cellPosNext - posNow).normalized * Time.deltaTime * SlopeSpeed;
                    }
                    else
                    {
                        //Перемещаем
                        posNow += (cellPosNext - posNow).normalized * Time.deltaTime * speed;
                    }
                }
                else
                {
                    //Перемещаем
                    posNow = cellPosNext;
                }

                //Если строение - склон
                if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].build == Building.getTypeName(Building.Type.Slope))
                {
                    //Корректируем высоту
                    if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Up))
                        height += Building.getSlopeHeightPlus(Building.Rotate.Up, posNow);
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Left))
                        height += Building.getSlopeHeightPlus(Building.Rotate.Left, posNow);
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Right))
                        height += Building.getSlopeHeightPlus(Building.Rotate.Right, posNow);
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Down))
                        height += Building.getSlopeHeightPlus(Building.Rotate.Down, posNow);
                }
                posNow = new Vector3(posNow.x, height, posNow.z);
                if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].build == Building.getTypeName(Building.Type.Slope))
                {
                    if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Down)) {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + Building.getSlopeHeightPlus(Building.Rotate.Down, new Vector3(cellPosNext.x, 0, cellPosNext.z)), cellPosNext.z);
                    }
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Left))
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + Building.getSlopeHeightPlus(Building.Rotate.Left, new Vector3(cellPosNext.x, 0, cellPosNext.z)), cellPosNext.z);
                    }
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Right))
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + Building.getSlopeHeightPlus(Building.Rotate.Right, new Vector3(cellPosNext.x, 0, cellPosNext.z)), cellPosNext.z);
                    }
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Up))
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + Building.getSlopeHeightPlus(Building.Rotate.Up, new Vector3(cellPosNext.x, 0, cellPosNext.z)), cellPosNext.z);
                    }
                    else 
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + 0.5f, cellPosNext.z);
                    }
                }
                else
                {
                    cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height, cellPosNext.z);
                }

                gameObject.transform.position = posNow;

                //Уведомняем карту врагов о своем местонахождении
                GPCTRL.EnemyMap.SetData(new Vector2Int((int)posNow.x, (int)posNow.z));

                //Если врагу осталось пройти 1\10 трафика до базы то уведомляем карту опасности
                if (GPCTRL.traficDistToBace[(int)posNow.x, (int)posNow.z] < GPCTRL.traficLenght/10) {
                    GPCTRL.AttentionMap.SetData(new Vector2Int((int)posNow.x, (int)posNow.z));
                }
            }
            else if (healthNow <= 0)
            {
                timeToStartDown -= Time.deltaTime;
                if (timeToStartDown <= 0)
                {
                    transform.position -= Vector3.up * Time.deltaTime * 0.1f;
                }
                if (timeToStartDown < -10)
                {
                    Destroy(gameObject);
                }
            }

            //Уменьшение стана
            if (timeStunning > 0)
            {
                timeStunning -= Time.deltaTime;
                if (timeStunning < 0) timeStunning = 0;
            }
            //Уменьшение замедления
            if (timeSlowing > 0) {
                timeSlowing -= Time.deltaTime;
                if (timeSlowing < 0) {
                    timeSlowing = 0;
                    SpeedCoof = 1; //Делаем обычную скорость
                }
            }
        }

        //Если тип врага - визуализатор
        else {
            //перемещаем на следующую точку
            if (cellPosNext != new Vector3() && GPCTRL != null && healthNow > 0)
            {
                //Корректируем высоту
                Vector3 posNow = gameObject.transform.position;
                float height = GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].height;
                if (height < 1)
                    height = 1;

                //проверяем не будет ли перебор с перемещением
                if (Vector3.Distance(cellPosNext, posNow) > Time.deltaTime * speed)
                {
                    //Если подьем
                    if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].build == Building.getTypeName(Building.Type.Slope))
                    {
                        //Перемещаем со скоростью подьема
                        posNow += (cellPosNext - posNow).normalized * Time.deltaTime * SlopeSpeed;
                    }
                    else
                    {
                        //Перемещаем
                        posNow += (cellPosNext - posNow).normalized * Time.deltaTime * speed;
                    }
                }
                else
                {
                    //Перемещаем
                    posNow = cellPosNext;
                }

                //Если строение - склон
                if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].build == Building.getTypeName(Building.Type.Slope))
                {
                    //Корректируем высоту
                    if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Up))
                        height += Building.getSlopeHeightPlus(Building.Rotate.Up, posNow);
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Left))
                        height += Building.getSlopeHeightPlus(Building.Rotate.Left, posNow);
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Right))
                        height += Building.getSlopeHeightPlus(Building.Rotate.Right, posNow);
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Down))
                        height += Building.getSlopeHeightPlus(Building.Rotate.Down, posNow);
                }
                posNow = new Vector3(posNow.x, height, posNow.z);
                if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].build == Building.getTypeName(Building.Type.Slope))
                {
                    if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Down))
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + Building.getSlopeHeightPlus(Building.Rotate.Down, new Vector3(cellPosNext.x, 0, cellPosNext.z)), cellPosNext.z);
                    }
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Left))
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + Building.getSlopeHeightPlus(Building.Rotate.Left, new Vector3(cellPosNext.x, 0, cellPosNext.z)), cellPosNext.z);
                    }
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Right))
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + Building.getSlopeHeightPlus(Building.Rotate.Right, new Vector3(cellPosNext.x, 0, cellPosNext.z)), cellPosNext.z);
                    }
                    else if (GPCTRL.cellsData[(int)posNow.x, (int)posNow.z].note == Building.getRotName(Building.Rotate.Up))
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + Building.getSlopeHeightPlus(Building.Rotate.Up, new Vector3(cellPosNext.x, 0, cellPosNext.z)), cellPosNext.z);
                    }
                    else
                    {
                        cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height + 0.5f, cellPosNext.z);
                    }
                }
                else
                {
                    cellPosNext = new Vector3(cellPosNext.x, GPCTRL.cellsData[(int)cellPosNext.x, (int)cellPosNext.z].height, cellPosNext.z);
                }

                gameObject.transform.position = posNow;

            }
        }
    }
    void TestRotate() {
        //Если следующая точка есть
        if (cellPosNext != new Vector3() && healthNow > 0 && timeStunning <= 0) {
            Vector3 lookRot = (cellPosNext - gameObject.transform.position);
            if (lookRot != new Vector3(0,0,0)) {
                //Узнаем необходимый поворот
                Quaternion rotNeed = Quaternion.LookRotation(lookRot);
                //Поворачиваемся
                gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, rotNeed, speed * Time.deltaTime * 180);
            }
        }
    }

    public void ReCalcColor() {
        if (meshRender != null && GPCTRL != null) {
            //Узнаем какой цвет для здоровья выбран
            //float colorDanger = healthNow / healthStart;
            float colorDanger = (float)Math.Log(healthNow ,healthBasic);
            if (healthNow <= 0)
                colorDanger = 0;

            //Считаем цвет
            Color colorNow = new Color();
            //Если значение меньше чем максимальный цвет, есть место под цвет выше и цветов больше нуля
            //Debug.Log("colorDanger " + colorDanger + " enemyColorsMax " + GPCTRL.EnemyColors.Length);
            if (colorDanger < GPCTRL.EnemyColors.Length-1 && GPCTRL.EnemyColors.Length > 0 && colorDanger >= 0)
            {
                colorNow = Color.Lerp(GPCTRL.EnemyColors[(int)colorDanger], GPCTRL.EnemyColors[(int)colorDanger + 1], colorDanger % 1);
            }
            //Если опасность отрицательная и цвета есть
            else if (colorDanger < 0 && GPCTRL.EnemyColors.Length > 0) {
                colorNow = GPCTRL.EnemyColors[0];
            }
            //Иначе максимальный цвет
            else if (GPCTRL.EnemyColors.Length > 0) {
                colorNow = GPCTRL.EnemyColors[GPCTRL.EnemyColors.Length - 1];
            }

            Material[] materials = meshRender.materials;
            materials[numMatColor].color = colorNow;
            meshRender.materials = materials;
        }
    }
    public void SetDamage(Player player, float damage, float timeStunningFunc, float timeSlowingFunc) {
        DamageInfo damageInfo = new DamageInfo(player, damage);
        DamageList.Add(damageInfo);

        healthNow -= damage;
        ReCalcColor();

        TestHealthIndicator();

        if (timeStunning < timeStunningFunc)
        {
            //если стан только что получен, выбираем анимацию стана
            if (timeStunning <= 0 && animator) {
                animator.SetFloat("TypeStun", UnityEngine.Random.Range(0, CountStun));
            }
            timeStunning = timeStunningFunc;
        }
        if (timeSlowing < timeSlowingFunc)
            timeSlowing = timeSlowingFunc;
    }
    public void SetDamage(Player player, float damage)
    {
        SetDamage(player, damage, 0 ,0);
    }

    [SerializeField]
    Animator animator;
    void TestAnimator() {
        if (animator != null && GPCTRL != null 
            && transform.position.x >= 0 && transform.position.x < 59 
            && transform.position.z >= 0 && transform.position.z < 59) {
            //Получаем тип поверхности для
            TestMove();

            //если цель жива
            if (!kiled) {
                if (timeStunning > 0) {
                    animator.SetInteger("TypeAnimation", 3);
                    animator.speed = 1;
                }
                else if (AnimSlope)
                {
                    animator.SetInteger("TypeAnimation", 2);
                    animator.speed = SlopeSpeed;
                }
                //обычная ходьба
                else {
                    animator.SetInteger("TypeAnimation", 1);
                    animator.speed = speed;
                }
            }
            else {
                animator.SetInteger("TypeAnimation", 10);
                animator.SetFloat("TypeDead", AnimkillNum);
                animator.speed = 1;
            }
        }

        void TestMove() {
            if (GPCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].build == Building.getTypeName(Building.Type.Slope))
            {
                AnimSlope = true;
            }
            else {
                AnimSlope = false;
            }
        }
    }

    float toKillTrafic = 10;
    void TestBaceDamage() {
        if (GPCTRL != null) {
            //Если достигли базы
            if (Vector2.Distance(new Vector2(GPCTRL.HightPoint.x + 0.5f + offset.x, GPCTRL.HightPoint.y + 0.5f + offset.y), new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)) < 0.05f) {

                //Считаем какой процент от урона нанести
                float percert = (GPCTRL.BaceHealth + 100) / 200;

                //Наносим урон
                GPCTRL.BaceHealth -= damage* percert;

                //Удаляем обьект
                if (typeEnemy != TypeEnemy.TraficVisualizator) {
                    Destroy(gameObject);
                }
                else {
                    toKillTrafic -= Time.deltaTime;
                    if (toKillTrafic < 0) {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    void TestHealthIndicator() {
        if (EnemyIndicators.main && EnemyIndicators.main.PrefabAIHealth && ID < EnemyIndicators.main.aIHealthSliders.Length) {
            //Если индикатора нет, создаем
            EnemyIndicators.CreateIndicator(this);

            //меняем значения индикатора
            if (EnemyIndicators.main.aIHealthSliders[ID]) {
                EnemyIndicators.main.aIHealthSliders[ID].SetHP();
            }
        }
    }

}
