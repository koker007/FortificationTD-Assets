using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Building : NetworkBehaviour
{
    public enum Type {
        None,
        EnemyBase,
        Base,
        Slope,
        Platform,
        TraficBlocker,
        PillBox,
        Turret,
        Minigun,
        Laser,
        Thunder,
        Rocket,
        Artillery
    }

    public enum Rotate {
        Down,
        Left,
        Right,
        Up
    }

    public enum TargetMode {
        SpeedMax,
        SpeedMin,
        HeathMax,
        HeathMin,
        DistMax,
        DistMin,
        TraficMax,
        TraficMin
    }

    [SyncVar]
    public bool VisualContactNeed = false;
    [SyncVar]
    public bool IgnoreGround1 = false;

    GameplayCTRL gameplayCTRL;
    void iniGameplayCTRL() {
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    [SerializeField]
    public SoundTurretCTRL soundTurretCTRL;

    [SerializeField]
    public int price = 0;

    [SerializeField]
    public Type type;

    [SerializeField]
    public float timeoutBuilding = 0;

    public static string getTypeName(Type type) {
        string name = "";
        if (type == Type.Base)
        {
            name = "base";
        }
        else if (type == Type.Slope)
        {
            name = "Slope";
        }
        else if (type == Type.Platform)
        {
            name = "platform";
        }
        else if (type == Type.TraficBlocker)
        {
            name = "traficBlocker";
        }
        else if (type == Type.PillBox)
        {
            name = "pillbox";
        }
        else if (type == Type.Turret)
        {
            name = "turret";
        }
        else if (type == Type.Artillery) {
            name = "artillery";
        }
        else if (type == Type.Minigun)
        {
            name = "minigun";
        }
        else if (type == Type.Laser)
        {
            name = "laser";
        }
        else if (type == Type.Thunder)
        {
            name = "thunder";
        }
        else if (type == Type.Rocket)
        {
            name = "rocketLauncher";
        }

        return name;
    }
    public static string getRotName(Rotate rotate) {
        string rotStr = "";
        if (rotate == Rotate.Down) {
            rotStr = "toBack";
        }
        else if (rotate == Rotate.Left) {
            rotStr = "toLeft";
        }
        else if (rotate == Rotate.Right) {
            rotStr = "toRight";
        }
        else if (rotate == Rotate.Up) {
            rotStr = "toFace";
        }

        return rotStr;
    }

    public static string getTargetModeName(TargetMode targetMode) {
        string modeStr = "";
        if (targetMode == TargetMode.DistMax)
            modeStr = "modeDistMax";
        else if (targetMode == TargetMode.HeathMax)
            modeStr = "modeHeathMax";
        else if (targetMode == TargetMode.SpeedMax)
            modeStr = "modeSpeedMax";
        else if (targetMode == TargetMode.TraficMax)
            modeStr = "modeTraficMax";
        else if (targetMode == TargetMode.DistMin)
            modeStr = "modeDistMin";
        else if (targetMode == TargetMode.HeathMin)
            modeStr = "modeHeathMin";
        else if (targetMode == TargetMode.SpeedMin)
            modeStr = "modeSpeedMin";
        else if (targetMode == TargetMode.TraficMin)
            modeStr = "modeTraficMin";

        return modeStr;
    }
    public static TargetMode getTargetModeName(string modeStr) {
        TargetMode mode = TargetMode.DistMin;

        if (modeStr == getTargetModeName(TargetMode.DistMax))
            mode = TargetMode.DistMax;
        else if (modeStr == getTargetModeName(TargetMode.DistMin))
            mode = TargetMode.DistMin;

        else if (modeStr == getTargetModeName(TargetMode.HeathMax))
            mode = TargetMode.HeathMax;
        else if (modeStr == getTargetModeName(TargetMode.HeathMin))
            mode = TargetMode.HeathMin;

        else if (modeStr == getTargetModeName(TargetMode.SpeedMax))
            mode = TargetMode.SpeedMax;
        else if (modeStr == getTargetModeName(TargetMode.SpeedMin))
            mode = TargetMode.SpeedMin;

        else if (modeStr == getTargetModeName(TargetMode.TraficMax))
            mode = TargetMode.TraficMax;
        else if (modeStr == getTargetModeName(TargetMode.TraficMin))
            mode = TargetMode.TraficMin;

        return mode;
    }
    public static float getSlopeHeightPlus(Rotate rotateSlope, Vector3 posObj) { //Получить смещение по высоте для обьекта на склоне
        float height = 0;

        //Спуск вверх
        if (rotateSlope == Rotate.Up) {
            height = 1 - (posObj.z % 1);
        }
        //Спуск влево
        else if (rotateSlope  == Rotate.Left) {
            height = posObj.x % 1; 
        }
        //Спуск вправо
        else if (rotateSlope == Rotate.Right) {
            height = 1 - (posObj.x % 1);
        }
        //Спуск вниз
        else if (rotateSlope == Rotate.Down) {
            height = posObj.z % 1;
        }

        return height;
    }

    [SerializeField]
    public GameObject Platform;

    [SerializeField]
    public GameObject Turret;
    [SerializeField]
    public GameObject HorizontalRotateObj;
    [SerializeField]
    bool isDecorateTurret = false;
    [SerializeField]
    Animator AnimTurrel;

    [SerializeField]
    [SyncVar] public TargetMode targetMode;
    [SerializeField]
    GameObject Bullet; //Префаб пули.. осуществляет контроль за уроном и достижением цели

    [SerializeField]
    Transform[] PosSpawnBullet;
    int PosSpawnBulletNow = 0;
    [SerializeField]
    float DamageDefaut = 20;
    [SerializeField]
    float StunDefaut = 0;

    [SerializeField]
    GameObject Prop;

    float timeStartLive = 0;

    //[SerializeField]
    //BuildTurret buildTurret;

    //Получить тип строения
    void iniBuildType() {
        //if (buildTurret == null) {
        //    buildTurret = gameObject.GetComponent<BuildTurret>();
        //}

    }

    [SerializeField]
    bool isArtilery = false;

    float TimeTestDestroyLast = 0;
    float RandomFix = 0;
    void testDestroy()
    {
        //Пока живы сообщаяем о своем присутствии на карту
        if (GameplayCTRL.main && 
            (type == Type.Base || type == Type.Laser || type == Type.Minigun || type == Type.Platform || type == Type.Turret || type == Type.PillBox)) {

            if (transform.position.x >= 0 && transform.position.x < 60 && transform.position.z >= 0 && transform.position.z < 60)
                GameplayCTRL.main.BuildMap.SetData(new Vector2Int((int)transform.position.x, (int)transform.position.z));
        }

        //if (Time.unscaledTime - timeStartLive > 10 && TimeTestDestroyLast + RandomFix < Time.unscaledTime) {
        //    TimeTestDestroyLast = Time.unscaledDeltaTime;
            if (isServer && GameplayCTRL.main && (GameplayCTRL.main.gamemode == 4 || GameplayCTRL.main.gamemode == 1)) {
                Destroy(gameObject);
            }
        //}
    }


    MeshRenderer PlatformMesh;
    float timeToTestColor = 0.5f;
    void testColorPlatform() {
        if (Platform != null && gameplayCTRL != null
            && transform.position.x >= 0 && transform.position.x < 59 && transform.position.z >= 0 && transform.position.z < 59) {
            timeToTestColor -= Time.unscaledDeltaTime;
            if (timeToTestColor < 0) {
                timeToTestColor = UnityEngine.Random.Range(5f,10f);

                if (PlatformMesh == null) {
                    PlatformMesh = Platform.GetComponent<MeshRenderer>();
                    if(PlatformMesh != null)
                        timeToTestColor = 0;
                }
                else {
                    Material[] materials = PlatformMesh.materials;
                    if (materials.Length > 1)
                    {
                        bool found = false;
                        foreach (Player player in gameplayCTRL.players)
                        {
                            //Если имя игрока совпадает то это владелец
                            if (gameplayCTRL.cellsData[(int)(transform.position.x + 0.1f), (int)(transform.position.z + 0.1f)].ownerSteamID == player.SteamID)
                            {
                                found = true;
                                materials[1].color = new Color(0.2f + player.colorVec.x * 0.7f, 0.2f + player.colorVec.y * 0.7f, 0.2f + player.colorVec.z * 0.7f);
                                break;
                            }
                        }
                        if (!found)
                        {
                            materials[1].color = new Color(0.8f, 0.8f, 0.8f);
                        }
                        PlatformMesh.materials = materials;
                    }
                }
            }
        }

    }

    //Если строение только появилось нужно оповестить ячейку, для которой предназначено строение, что она занята данным строением
    void iniBuildForCell() {
        if (gameplayCTRL != null && gameplayCTRL.cellsData != null && 
            gameObject.transform.position.x < 60 &&
            gameObject.transform.position.z < 60 &&
            gameObject.transform.position.x >= 0 &&
            gameObject.transform.position.z >= 0) {

            //вытаскиваем позицию данного строения
            Vector2Int pos = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.z);

            gameplayCTRL.cellsData[pos.x, pos.y].build = getTypeName(type);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timeStartLive = Time.unscaledTime;

        iniGameplayCTRL();
        iniBuildType();
        iniBuilding();

        soundTurretCTRL = gameObject.GetComponent<SoundTurretCTRL>();
        RandomFix = UnityEngine.Random.Range(1, 3f);


        scaleTest = UnityEngine.Random.Range(2f, 5f);
    }

    void iniBuilding() {
        if (gameplayCTRL != null && gameplayCTRL.buildings != null && transform.position.x >= 0 && transform.position.x < 60 && transform.position.z >= 0 && transform.position.z < 60) {
            gameplayCTRL.buildings[(int)transform.position.x, (int)transform.position.z] = this;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        testDestroy();

        iniBuildForCell();
        CtrlFireTurrel();
        testColorPlatform();

        TestRotate();
    }


    //Группа базовых параметров строения
    float health = 100;

    ///////////////////////////////////////////////////////
    [SerializeField]
    public int StartUpgrages = 1;
    //Группа базовых параметров данной турели
    [SerializeField]
    float speedRot = 100; //Скорость поворота башни
    [SerializeField]
    float speedFire = 0.1f; //Скорость стрельбы
    float nextFire = 0; //Следующий выстрел через
    [SerializeField]
    int BulletsOneShot = 1; //количество пуль выпускаемых за раз
    [SerializeField]
    int BulletsNow = 1;
    [SerializeField]
    float DelayOneShot = 0; //малая задержка перед выстрелами 

    [SerializeField]
    float distRot = 10; //Дистанция для начала поворота
    [SerializeField]
    float distFire = 5; //Дистанция для начала стрельбы
    [SerializeField]
    float accuracy = 10; //Точность стрельбы
    [SerializeField]
    float correctRotStart = 0.5f;

    [SerializeField]
    float WaitingFire = 0.1f; //Время ожидания перед началом стрельбы
    float WaitingFireNow = 0;

    bool StartFire = false; //Стреляет ли башня в этом кадре или нет

    Vector3 olderPosTarget = new Vector3();
    Quaternion olderRotTarget = new Quaternion();

    public float timeLastFire = 0;//Время последнего выстрела
    void CtrlFireTurrel() {
        //
        if (Turret != null && gameplayCTRL != null && gameplayCTRL.gamemode <= 3 && correctryPos()) {
            AICTRL target = null;

            if (olderPosTarget == new Vector3())
                olderPosTarget = Turret.transform.forward + Turret.transform.position;

            float correctRot = 2; //разница векторов

            //getEnemy();
            //Проверка на наличие врагов в зоне видимости
            target = AICTRL.getEnemy(Turret.transform.position, GetFireDistance(), targetMode, VisualContactNeed);
            //Запоминаем последнюю позицию на случай потери цели
            if (target != null)
            {
                BoxCollider collider = target.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    olderPosTarget = target.transform.position + collider.center;
                }
                else
                {
                    olderPosTarget = target.transform.position;
                }
            }

            float speedRotNow = speedRot;

            Rotate();
            Fire();

            Animator();

            //////////////////////////////////////////////////////////////////////////////////////
            ///Конец функции


            //Вращение турели
            void Rotate() {
                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.rotate1)
                    speedRotNow += speedRotNow * 0.5f;
                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.rotate2)
                    speedRotNow += speedRotNow * 0.5f;
                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.rotate3)
                    speedRotNow += speedRotNow * 0.5f;

                if (isArtilery)
                {
                    if (target != null)
                    {

                        BoxCollider collider = target.GetComponent<BoxCollider>();
                        Quaternion rotNeed;

                        //Узнаем растояние до врага
                        float distHorizontToEnemy = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.transform.position.x, target.transform.position.z));


                        Quaternion rotNow = Turret.transform.rotation;

                        //В зависимости от того видно ли цель напрямую
                        //Узнаем поворот необходимый чтобы смотреть на цель
                        float fireDistNow = GetFireDistance();

                        RaycastHit raycastHit;
                        //пускаем луч на этого врага, если луч попал значит видно, стреляем по градусам ниже 45
                        if (Physics.Raycast(Turret.transform.position, target.transform.position - Turret.transform.position, out raycastHit, distHorizontToEnemy*1.5f) && raycastHit.collider.gameObject == target.gameObject)
                        {
                            //Поворачиваем так чтобы смотреть на врага
                            Quaternion rotPlus = Quaternion.LookRotation((target.transform.position - Turret.transform.position).normalized);

                            rotNeed = Quaternion.LookRotation(target.transform.position - Turret.transform.position);
                            rotNeed.eulerAngles = new Vector3(-45 + getGrad(distHorizontToEnemy, 9.81f, fireDistNow) + rotPlus.eulerAngles.x, rotNeed.eulerAngles.y, 0);
                        }
                        else
                        {
                            //Изходя из того какой градус сейчас у пушки, получаем время полета
                            float endTime = 2; //Calc.Phyisic.GetTimeToVelosityY0(rotNow.eulerAngles.x, distFire, 9.81f);
                            Vector3 targetMoveVec = (target.transform.position - target.posOld).normalized;
                            Vector3 targetPosFuture = target.transform.position + (targetMoveVec * target.speed);
                            distHorizontToEnemy = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPosFuture.x, targetPosFuture.z));

                            rotNeed = Quaternion.LookRotation(targetPosFuture - Turret.transform.position);
                            float GradNew = -45 - getGrad(distHorizontToEnemy, 9.81f, fireDistNow);
                            if (GradNew < -87)
                                GradNew = -87;
                            rotNeed.eulerAngles = new Vector3(GradNew, rotNeed.eulerAngles.y, 0);
                        }

                        //получаем вектор того куда надо смотреть
                        Turret.transform.rotation = rotNeed;
                        Vector3 vecNeed = Turret.transform.forward;
                        Turret.transform.rotation = rotNow;

                        //Узнаем текушую разницу, векторов от того куда надо смотреть от того куда смотрим сейчас
                        correctRot = (vecNeed - Turret.transform.forward).magnitude;

                        olderRotTarget = rotNeed;

                        Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, olderRotTarget, speedRotNow * Time.deltaTime);
                        Quaternion rotNew = Turret.transform.rotation;
                        rotNew.eulerAngles = new Vector3(Turret.transform.rotation.eulerAngles.x, Turret.transform.rotation.eulerAngles.y, 0);
                        Turret.transform.rotation = rotNew;

                        float getGradArtileryFire(float EnemyDist, float gravity)
                        {
                            return (float)(Math.Asin((EnemyDist * gravity)/(Math.Pow(fireDistNow * 1.3f,2)))/2);
                        }
                        float getGrad(float enemyDist, float gravity, float distFireFunc) {
                            float result = 0;

                            float maxDist = distFireFunc * 1f;
                            float distCoof = enemyDist / maxDist;

                            result = (1 - distCoof)*45;

                            return result;
                        }
                    }
                    else
                    {
                        //Поворачиваемся в сторону цели
                        if (!isDecorateTurret)
                            Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, olderRotTarget, speedRotNow * Time.deltaTime);
                        else Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, olderRotTarget, 360);
                        //Узнаем текушую разницу, векторов от того куда надо смотреть от того куда смотрим сейчас
                        correctRot = ((olderPosTarget - Turret.transform.position).normalized - Turret.transform.forward).magnitude;
                    }
                }
                else {
                    //Если есть цель
                    if (target != null)
                    {
                        BoxCollider collider = target.GetComponent<BoxCollider>();
                        Quaternion rotNeed;
                        //Узнаем поворот необходимый чтобы смотреть на цель
                        if (collider != null)
                        {
                            rotNeed = Quaternion.LookRotation((target.transform.position + collider.center) - Turret.transform.position);
                            //Узнаем текушую разницу, векторов от того куда надо смотреть от того куда смотрим сейчас
                            correctRot = (((target.transform.position + collider.center) - Turret.transform.position).normalized - Turret.transform.forward).magnitude;
                        }
                        else
                        {
                            rotNeed = Quaternion.LookRotation(target.transform.position - Turret.transform.position);
                            //Узнаем текушую разницу, векторов от того куда надо смотреть от того куда смотрим сейчас
                            correctRot = ((target.transform.position - Turret.transform.position).normalized - Turret.transform.forward).magnitude;
                        }
                        //Поворачиваемся в сторону цели
                        if (!isDecorateTurret)
                            Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, rotNeed, speedRotNow * Time.deltaTime);
                        else Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, rotNeed, 360);

                    }
                    else
                    {
                        //Узнаем поворот необходимый чтобы смотреть на цель
                        Quaternion rotNeed = Quaternion.LookRotation(olderPosTarget - Turret.transform.position);

                        //Поворачиваемся в сторону цели
                        if (!isDecorateTurret)
                            Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, rotNeed, speedRotNow * Time.deltaTime);
                        else Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, rotNeed, 360);
                        //Узнаем текушую разницу, векторов от того куда надо смотреть от того куда смотрим сейчас
                        correctRot = ((olderPosTarget - Turret.transform.position).normalized - Turret.transform.forward).magnitude;
                    }
                }

                if (HorizontalRotateObj != null) {
                    //Узнаем двухмерное направление взгляда турели
                    Vector3 posNeedView = new Vector3(Turret.gameObject.transform.forward.x, 0, Turret.gameObject.transform.forward.z);
                    HorizontalRotateObj.transform.rotation = Quaternion.LookRotation(posNeedView, new Vector3(0,1,0));
                }
            }

            void Fire() {
                //Перезарежаемся
                nextFire -= Time.deltaTime;

                float distatanceFire = GetFireDistance();

                if (target != null)
                {

                    if (isArtilery)
                    {
                        //Позиция врага
                        Vector3 posTarget = new Vector3();
                        BoxCollider collider = target.GetComponent<BoxCollider>();
                        if (collider != null)
                            posTarget = target.transform.position + collider.center;
                        else
                            posTarget = target.transform.position;

                        if (nextFire < 0)
                            nextFire = 0;

                        //Расчет точности
                        float accuracyNew = accuracy;
                        float correctRotStartNow = correctRotStart;//Порог вращения после которого считается что турель наведена на цель
                        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.accuracy1)
                        {
                            accuracyNew += accuracy / 2;
                            correctRotStartNow *= 0.5f;
                        }
                        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.accuracy2)
                        {
                            accuracyNew += accuracy / 2;
                            correctRotStartNow *= 0.5f;
                        }
                        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.accuracy3)
                        {
                            accuracyNew += accuracy / 2;
                            correctRotStartNow *= 0.5f;
                        }

                        //Если смотрим на врага то стреляем
                        if (correctRot < correctRotStartNow)
                        {
                            WaitingFireNow -= Time.deltaTime;
                        }
                        else
                        {
                            WaitingFireNow = WaitingFire;
                        }

                        bool reloaded = false;
                        if (timeLastFire + GetFireSpeed() <= Time.time)
                        {
                            BulletsNow = BulletsOneShot;
                        }

                        if (timeLastFire + GetFireSpeed() <= Time.time || (BulletsNow > 0 && timeLastFire + DelayOneShot <= Time.time)) reloaded = true;

                        if (correctRot < correctRotStartNow && WaitingFireNow < 0 && reloaded && Vector3.Distance(Turret.transform.position, posTarget) < distatanceFire)
                        {

                            nextFire = GetFireSpeed();

                            //Если есть префаб пули
                            if (isServer && Bullet != null && PosSpawnBullet.Length > 0)
                            {
                                BulletsNow--;
                                timeLastFire = Time.time;

                                //Считаем урон
                                float timeDamage = (float)Math.Pow(DamageDefaut, 1 + ((gameplayCTRL.timeGamePlay + GameplayCTRL.main.traficLenght) / 700f));
                                float damageNew = timeDamage;
                                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.damage1)
                                    damageNew = (float)Math.Pow(DamageDefaut, 1.25f + (gameplayCTRL.timeGamePlay / 670f));//(float)Math.Pow(timeDamage, 1.25f);
                                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.damage2)
                                    damageNew = (float)Math.Pow(DamageDefaut, 1.5f + (gameplayCTRL.timeGamePlay / 665f));//(float)Math.Pow(timeDamage, 1.5f);
                                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.damage3)
                                    damageNew = (float)Math.Pow(DamageDefaut, 2f + (gameplayCTRL.timeGamePlay / 660f));//(float)Math.Pow(timeDamage, 1.75f);

                                GameObject bulletObj = Instantiate(Bullet);
                                PosSpawnBulletNow++;
                                if (PosSpawnBulletNow >= PosSpawnBullet.Length)
                                    PosSpawnBulletNow = 0;

                                //Перемещаем и задаем направление
                                bulletObj.transform.position = PosSpawnBullet[PosSpawnBulletNow].position;
                                //Рандомный вектор
                                Vector3 vectorRandom = new Vector3(UnityEngine.Random.RandomRange(-1f, 1f), UnityEngine.Random.RandomRange(-1f, 1f), UnityEngine.Random.RandomRange(-1f, 1f));

                                if (!isDecorateTurret)
                                    bulletObj.transform.rotation = Quaternion.LookRotation((Turret.transform.forward * accuracyNew + vectorRandom).normalized);
                                else bulletObj.transform.rotation = Quaternion.LookRotation(transform.up.normalized);

                                Shot();

                                void Shot()
                                {
                                    //Задаем параметры пули
                                    BulletCtrl bulletCtrl = bulletObj.GetComponent<BulletCtrl>();
                                    if (bulletCtrl != null)
                                    {
                                        bulletCtrl.flyDistMax = distatanceFire * 2;
                                        bulletCtrl.damageMax = damageNew;

                                        //Рапоминаем позицию стреляющей турели
                                        bulletCtrl.posTurrel = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
                                        bulletCtrl.target = target;
                                        bulletCtrl.targetMode = targetMode;
                                        bulletCtrl.distStep2 *= distatanceFire;
                                        bulletCtrl.flyRotate = speedRotNow;
                                        bulletCtrl.accuracy = accuracyNew;
                                        bulletCtrl.randomvec = vectorRandom;

                                        bulletCtrl.PositionStart = bulletObj.transform.position;
                                        float velosity = distatanceFire * 1f;
                                        bulletCtrl.VelosityStart = Turret.transform.forward * velosity;

                                        bulletCtrl.timeToDie = 10;//Calc.Phyisic.GetTimeToVelosityY0(Turret.transform.rotation.eulerAngles.x, velosity, 9.81f)*2;

                                        bulletCtrl.timeStuninig = StunDefaut + StunDefaut * gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].getCountTech();
                                        bulletCtrl.MaxTargets = 1 + gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].getCountTech();

                                        foreach (Player player in gameplayCTRL.players)
                                        {
                                            if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ownerSteamID == player.SteamID)
                                            {
                                                bulletCtrl.Owner = player;
                                            }
                                        }

                                    }

                                    NetworkServer.Spawn(bulletObj);
                                }
                            }

                            //Звук выстрела
                            //if (soundTurretCTRL != null) {
                            //soundTurretCTRL.Fire();
                            //}
                        }

                    }
                    else {
                        //Позиция врага
                        Vector3 posTarget = new Vector3();
                        BoxCollider collider = target.GetComponent<BoxCollider>();
                        if (collider != null)
                            posTarget = target.transform.position + collider.center;
                        else
                            posTarget = target.transform.position;

                        if (nextFire < 0)
                            nextFire = 0;

                        //Расчет точности
                        float accuracyNew = accuracy;
                        float correctRotStartNow = correctRotStart;//Порог вращения после которого считается что турель наведена на цель
                        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.accuracy1)
                        {
                            accuracyNew += accuracy / 2;
                            correctRotStartNow *= 0.5f;
                        }
                        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.accuracy2)
                        {
                            accuracyNew += accuracy / 2;
                            correctRotStartNow *= 0.5f;
                        }
                        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.accuracy3)
                        {
                            accuracyNew += accuracy / 2;
                            correctRotStartNow *= 0.5f;
                        }

                        //Если смотрим на врага то стреляем
                        if (correctRot < correctRotStartNow)
                        {
                            WaitingFireNow -= Time.deltaTime;
                        }
                        else
                        {
                            WaitingFireNow = WaitingFire;
                        }

                        bool reloaded = false;
                        if (timeLastFire + GetFireSpeed() <= Time.time)
                        {
                            BulletsNow = BulletsOneShot;
                        }

                        if (timeLastFire + GetFireSpeed() <= Time.time || (BulletsNow > 0 && timeLastFire + DelayOneShot <= Time.time)) reloaded = true;

                        if (correctRot < correctRotStartNow && WaitingFireNow < 0 && reloaded && Vector3.Distance(Turret.transform.position, posTarget) < distatanceFire)
                        {

                            nextFire = GetFireSpeed();

                            //Если есть префаб пули
                            if (isServer && Bullet != null && PosSpawnBullet.Length > 0)
                            {
                                BulletsNow--;
                                timeLastFire = Time.time;

                                //Считаем урон
                                float timeDamage = (float)Math.Pow(DamageDefaut, 1 + ((gameplayCTRL.timeGamePlay + GameplayCTRL.main.traficLenght) / 700f));
                                float damageNew = timeDamage;
                                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.damage1)
                                    damageNew = (float)Math.Pow(DamageDefaut, 1.25f + (gameplayCTRL.timeGamePlay / 670f));//(float)Math.Pow(timeDamage, 1.25f);
                                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.damage2)
                                    damageNew = (float)Math.Pow(DamageDefaut, 1.5f + (gameplayCTRL.timeGamePlay / 665f));//(float)Math.Pow(timeDamage, 1.5f);
                                if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.damage3)
                                    damageNew = (float)Math.Pow(DamageDefaut, 2f + (gameplayCTRL.timeGamePlay / 660f));//(float)Math.Pow(timeDamage, 1.75f);

                                GameObject bulletObj = Instantiate(Bullet);
                                PosSpawnBulletNow++;
                                if (PosSpawnBulletNow >= PosSpawnBullet.Length)
                                    PosSpawnBulletNow = 0;

                                //Перемещаем и задаем направление
                                bulletObj.transform.position = PosSpawnBullet[PosSpawnBulletNow].position;
                                //Рандомный вектор
                                Vector3 vectorRandom = new Vector3(UnityEngine.Random.RandomRange(-1f, 1f), UnityEngine.Random.RandomRange(-1f, 1f), UnityEngine.Random.RandomRange(-1f, 1f));

                                if (!isDecorateTurret)
                                    bulletObj.transform.rotation = Quaternion.LookRotation((Turret.transform.forward * accuracyNew + vectorRandom).normalized);
                                else bulletObj.transform.rotation = Quaternion.LookRotation(transform.up.normalized);

                                Shot();

                                void Shot()
                                {
                                    //Задаем параметры пули
                                    BulletCtrl bulletCtrl = bulletObj.GetComponent<BulletCtrl>();
                                    if (bulletCtrl != null)
                                    {
                                        bulletCtrl.flyDistMax = distatanceFire * 2;
                                        bulletCtrl.damageMax = damageNew;

                                        //Рапоминаем позицию стреляющей турели
                                        bulletCtrl.posTurrel = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
                                        bulletCtrl.target = target;
                                        bulletCtrl.targetMode = targetMode;
                                        bulletCtrl.distStep2 *= distatanceFire;
                                        bulletCtrl.flyRotate = speedRotNow;
                                        bulletCtrl.accuracy = accuracyNew;
                                        bulletCtrl.randomvec = vectorRandom;

                                        bulletCtrl.PositionStart = bulletObj.transform.position;
                                        float velosity = 5;
                                        bulletCtrl.VelosityStart = Turret.transform.forward * velosity;

                                        bulletCtrl.timeStuninig = StunDefaut + StunDefaut * gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].getCountTech();
                                        bulletCtrl.MaxTargets = 1 + gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].getCountTech();

                                        foreach (Player player in gameplayCTRL.players)
                                        {
                                            if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ownerSteamID == player.SteamID)
                                            {
                                                bulletCtrl.Owner = player;
                                            }
                                        }
                                    }

                                    NetworkServer.Spawn(bulletObj);
                                }
                            }

                            //Звук выстрела
                            //if (soundTurretCTRL != null) {
                            //soundTurretCTRL.Fire();
                            //}
                        }
                    }
                }
            }

            void Animator() {
                if (AnimTurrel) {
                    if (Time.time - timeLastFire < 0.25f)
                    {
                        AnimTurrel.SetBool("fire", true);
                        AnimTurrel.SetFloat("fire_speed", speedFire/speedFire);
                    }
                    else {
                        AnimTurrel.SetBool("fire", false);
                    }
                }
            }
        }
    }

    bool correctryPos() {
        bool good = false;
        if (transform.position.x >= 0 && transform.position.x <= 59 && transform.position.z >= 0 && transform.position.z <= 59) {
            good = true;
        }

        return good;
    }

    public float GetFireDistance() {
        float distatanceFire = distFire;
        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.dist1)
            distatanceFire = distFire * 1.2f + 1;
        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.dist2)
            distatanceFire = distFire * 1.4f + 1;
        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.dist3)
            distatanceFire = distFire * 1.6f + 1;

        return distatanceFire;
    }
    public float GetFireSpeed() {
        float speedFireNew = speedFire;
        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.speed1)
            speedFireNew -= speedFireNew / 4;
        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.speed2)
            speedFireNew -= speedFireNew / 4;
        if (gameplayCTRL.cellsData[(int)transform.position.x, (int)transform.position.z].ResearchAllGun.speed3)
            speedFireNew -= speedFireNew / 4;

        return speedFireNew;
    }

    public Vector3 GetTurretPos() {
        if (Turret != null)
        {
            return Turret.transform.position;
        }
        else return this.transform.position;
    }

    [Header("Testing")]
    [SerializeField]
    bool NeedTestRotate = false;
    float oldTest = 0;
    float scaleTest  = 2;

    [Header("Track")]
    [SerializeField]
    bool canMoveCar = false;
    [SerializeField]
    bool canMovePehota = false;
    [SerializeField]
    bool canMoveCrab = false;

    [SerializeField]
    public bool[] traficBlocker = new bool[3];
    void TestRotate() {
        if (NeedTestRotate && GameplayCTRL.main && Time.unscaledTime - oldTest > scaleTest && transform.position.x > 0 && Platform && transform.position.x < 59 && transform.position.z > 0 && transform.position.z < 59) {
            //Проверяем какой поворот должен быть
            if (GameplayCTRL.main.cellsData[(int)transform.position.x, (int)transform.position.z].note == Building.getRotName(Building.Rotate.Up))
            {
                Quaternion rotNew = new Quaternion();
                rotNew.eulerAngles = new Vector3(Platform.transform.rotation.eulerAngles.x, 0, Platform.transform.rotation.y);
                Platform.transform.rotation = rotNew;
            }
            else if (GameplayCTRL.main.cellsData[(int)transform.position.x, (int)transform.position.z].note == Building.getRotName(Building.Rotate.Left))
            {
                Quaternion rotNew = new Quaternion();
                rotNew.eulerAngles = new Vector3(Platform.transform.rotation.eulerAngles.x, -90, Platform.transform.rotation.y);
                Platform.transform.rotation = rotNew;
            }
            else if (GameplayCTRL.main.cellsData[(int)transform.position.x, (int)transform.position.z].note == Building.getRotName(Building.Rotate.Right))
            {
                Quaternion rotNew = new Quaternion();
                rotNew.eulerAngles = new Vector3(Platform.transform.rotation.eulerAngles.x, 90, Platform.transform.rotation.y);
                Platform.transform.rotation = rotNew;
            }
            else if (GameplayCTRL.main.cellsData[(int)transform.position.x, (int)transform.position.z].note == Building.getRotName(Building.Rotate.Down))
            {
                Quaternion rotNew = new Quaternion();
                rotNew.eulerAngles = new Vector3(Platform.transform.rotation.eulerAngles.x, 180, Platform.transform.rotation.y);
                Platform.transform.rotation = rotNew;
            }
        }
    }
}
