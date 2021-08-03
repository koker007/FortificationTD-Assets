using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletCtrl : NetworkBehaviour
{
    [SerializeField]
    public AICTRL target;
    [SerializeField]
    public float timeToDie = 0;
    [SerializeField]
    public float damageMax = 10;
    [SerializeField]
    public float explosionDist = 3;
    [SerializeField]
    public float MaxTargets = 1;

    [Header("Effects")]
    [SerializeField]
    public float timeStuninig = 0;
    [SerializeField]
    public float timeSlowing = 0;
    [SerializeField]
    public float SpeedCoof = 0;

    public bool die = false;
    [SyncVar]
    bool dieNeed = false;

    [SerializeField]
    GameObject VisualObj; //Визуальная часть пули которую надо выключить после смерти
    [SerializeField]
    ParticleSystem particleTraser;
    [SerializeField]
    GameObject PrefabBoom;

    [SerializeField]
    TypeMove typeMove = TypeMove.NonControlledLinear;
    [SerializeField]
    TypeDamage typeDamage = TypeDamage.Bullet;
    [SerializeField] [SyncVar]
    public Building.TargetMode targetMode = Building.TargetMode.TraficMax;
    [SerializeField]
    GameObject PrefabParticleDent;

    [SerializeField]
    public float flySpeed = 10;
    public float flyDistMax = 10;  //Максимальное растояние полета пули
    public float flyRotate = 50;
    float flyDistNow = 0;
    [SerializeField]
    public float distStep2 = 10; //Растояние полета для переключения на следующий шаг

    [SerializeField]
    GameObject[] lightsObj;


    [SerializeField]
    public Player Owner;

    [SyncVar] public int TargetID = 0;
    [SyncVar] public Vector2 posTurrel;
    [SyncVar] public Vector3 randomvec;
    [SyncVar] public float accuracy;

    [Header("Mortar")]
    [SerializeField]
    [SyncVar] public Vector3 PositionStart;
    [SerializeField]
    [SyncVar] public Vector3 VelosityStart;
    [SerializeField]
    [SyncVar] public float TimeLive;

    public Vector3 oldPos = new Vector3();

    enum TypeMove {
        NonControlledLinear,
        NonControlledLinearLaser,
        ControlledAimingRocket,
        NonControlledParabola,
        NotMove
    }
    enum TypeDamage {
        Explosion,
        Bullet,
        Laser,
        Lightning
    }

    // Start is called before the first frame update
    void Start()
    {
        SetTurrelFire();
        CreateFireSmoke();
    }

    [SerializeField]
    GameObject PrefabParticleFire;
    void CreateFireSmoke() {
        //Говорим запустить частицы стрельбы
        if (PrefabParticleFire != null)
        {
            //Создаем и перемещаем
            GameObject partFireObj = Instantiate(PrefabParticleFire);
            if (partFireObj != null)
            {
                partFireObj.transform.position = gameObject.transform.position;
                partFireObj.transform.rotation = gameObject.transform.rotation;
            }
        }
    }

    // Update is called once per frame
    bool pausedFirstFrame = false;
    void Update()
    {
        live();
        move();
    }

    //Сказать турели что она выстрелила
    void SetTurrelFire()
    {
        if (posTurrel != null)
        {
            if (GameplayCTRL.main != null && GameplayCTRL.main.buildings[(int)posTurrel.x, (int)posTurrel.y] != null)
            {

                if (GameplayCTRL.main.buildings[(int)posTurrel.x, (int)posTurrel.y].soundTurretCTRL != null)
                    GameplayCTRL.main.buildings[(int)posTurrel.x, (int)posTurrel.y].soundTurretCTRL.Fire();

                GameplayCTRL.main.buildings[(int)posTurrel.x, (int)posTurrel.y].timeLastFire = Time.time;
            }
        }
    }

    void live() {

        TimeLive += Time.deltaTime;

        //Если он умер а время жизни не закончилось то завершаем
        if (die && timeToDie > 0)
        {
            timeToDie = 0;
        }
        else if (lightsObj.Length > 0) {
            foreach (GameObject lightObj in lightsObj) {
                if (lightObj) {
                    if (!lightObj.active)
                        lightObj.SetActive(true);
                    else {
                        Destroy(lightObj);
                    }
                    break;
                }
            }
        }
        
        //Уменьшаем жизнь
        timeToDie -= Time.deltaTime;

        //если не умер но время жизни кончелось
        if ((!die && timeToDie < 0) || dieNeed) {
            //Взрываем
            Damage();
        }

        if (timeToDie < -3) {
            Destroy(gameObject);
        }
    }
    void move() {
        oldPos = transform.position;

        if (pausedFirstFrame)
        {
            //В зависимости от типа движения перемещаем

            //Прямолинейное движение в сторону цели
            if (typeMove == TypeMove.NonControlledLinear)
            {
                float flyDistOld = flyDistNow; //для выполнения последнего движения

                flyDistNow += Time.deltaTime * flySpeed;

                //если пуля не достигла максимума
                if (flyDistNow < flyDistMax)
                {
                    transform.position += transform.forward * Time.deltaTime * flySpeed;
                }
                //последнее перемещение
                else if (flyDistOld < flyDistMax)
                {
                    transform.position += transform.forward * (flyDistMax - flyDistOld);
                }
            }
            else if (typeMove == TypeMove.NonControlledLinearLaser)
            {

                float flyDistOld = flyDistNow; //для выполнения последнего движения

                //Сразу перемещаем до конца
                if (flyDistOld < flyDistMax)
                {
                    flyDistNow += (flyDistMax - flyDistOld);
                    transform.position += transform.forward * (flyDistMax - flyDistOld);
                }
            }

            //Движение с наведением на цель
            else if (typeMove == TypeMove.ControlledAimingRocket && !die)
            {

                float flyDistOld = flyDistNow; //для выполнения последнего движения
                flyDistNow += Time.deltaTime * flySpeed;

                //Следующий шаг наведение на цель
                if (flyDistNow > distStep2)
                {

                    RaycastHit raycastHit;
                    //пускаем луч в сторону движения, если луч попал значит пора взрывать
                    if (Physics.Raycast(transform.position - transform.forward, transform.forward, out raycastHit, 0.75f))
                    {
                        Damage();
                    }

                    if (TargetID != 0 && GameplayCTRL.main && GameplayCTRL.main.aICTRLs.Length > TargetID)
                    {
                        target = GameplayCTRL.main.aICTRLs[TargetID];
                    }
                    //Проверка на наличие врагов в зоне видимости
                    if (target == null && GameplayCTRL.main.isServer)
                    {
                        target = AICTRL.getEnemy(transform.position, flyDistMax, targetMode, true);
                        if (target != null)
                        {
                            TargetID = target.ID;
                            if (TargetID != 0 && GameplayCTRL.main && GameplayCTRL.main.aICTRLs.Length > TargetID)
                            {
                                target = GameplayCTRL.main.aICTRLs[TargetID];
                            }
                        }

                    }

                    if (target != null)
                    {
                        Quaternion rotNow = gameObject.transform.rotation;
                        Quaternion rotNeed = new Quaternion();
                        rotNeed.SetLookRotation(target.transform.position - gameObject.transform.position);

                        gameObject.transform.rotation = Quaternion.RotateTowards(rotNow, rotNeed, flyRotate * Time.deltaTime);
                        gameObject.transform.Rotate(randomvec.x * 180 / accuracy * Time.deltaTime, randomvec.y * 180 / accuracy * Time.deltaTime, randomvec.z * 180 / accuracy * Time.deltaTime);
                    }
                    else
                    {
                        if (GameplayCTRL.main.isServer)
                        {
                            dieNeed = true;
                        }
                    }
                }

                //Летим куда смотрит ракета
                transform.position += transform.forward * Time.deltaTime * flySpeed;
            }

            else if (typeMove == TypeMove.NonControlledParabola && !die)
            {
                float posX = VelosityStart.x * TimeLive;
                float posY = VelosityStart.y * TimeLive - ((9.81f * (TimeLive * TimeLive)) / 2);
                float posZ = VelosityStart.z * TimeLive;

                oldPos = gameObject.transform.position;
                gameObject.transform.position = new Vector3(PositionStart.x + posX, PositionStart.y + posY, PositionStart.z + posZ);

                Vector3 forwardMove = (transform.position - oldPos).normalized;
                RaycastHit raycastHit;
                //пускаем луч в сторону движения, если луч попал значит пора взрывать
                if (Physics.Raycast(transform.position, forwardMove, out raycastHit, 0.75f))
                {
                    Damage();
                }
            }

            else if (typeMove == TypeMove.NotMove)
            {
                //если есть частицы
                if (particleTraser && !particleTraser.gameObject.activeSelf)
                {
                    particleTraser.gameObject.SetActive(true);
                    ParticleSystem.MainModule main = particleTraser.main;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(flyDistMax / main.startSpeed.constant);
                    timeToDie = main.startLifetime.constant - 3;
                }
            }
        }
        pausedFirstFrame = true;
    }

    void Damage() {
        //если еще не умирал
        if (!die) {
            if (timeToDie > 0)
                timeToDie = 0;

            die = true;
            if (VisualObj) {
                VisualObj.active = false;
            }
            if (PrefabBoom) {
                GameObject particleBoom = Instantiate(PrefabBoom);
                if (particleBoom) {
                    particleBoom.transform.position = gameObject.transform.position;
                }
            }

            //Взрываем, согласно типу пули
            if (typeDamage == TypeDamage.Bullet)
                BulletDamage();
            else if (typeDamage == TypeDamage.Laser)
                LaserDamage();
            else if (typeDamage == TypeDamage.Explosion)
                ExplosionDamage();
            else if (typeDamage == TypeDamage.Lightning) {
                LightningDamage();
            }
        }

        
        void BulletDamage() {
            //пуля пускает луч с лицевой стороны
            RaycastHit Rayinfo;
            if (Physics.Raycast(transform.position, transform.forward, out Rayinfo, flyDistMax)) {
                AICTRL rayTarget = Rayinfo.collider.GetComponent<AICTRL>();
                //изменяем значение максимального полета пули на "пока не попадем в препятствие"
                flyDistMax = Vector3.Distance(gameObject.transform.position, Rayinfo.point);

                if (rayTarget != null) {
                    rayTarget.SetDamage(Owner ,damageMax);
                }
                //иначе создаем вмятину на поверхности попадания
                else {
                    if (PrefabParticleDent != null) {
                        GameObject particleObj = Instantiate(PrefabParticleDent);
                        ParticleDentCTRL particleDentCTRL = particleObj.GetComponent<ParticleDentCTRL>();
                        if (particleDentCTRL != null)
                        {
                            particleObj.transform.position = Rayinfo.point + (-0.01f*transform.forward);
                            particleDentCTRL.normal = Rayinfo.normal;
                        }
                        else {
                            Destroy(particleObj);
                        }
                    }
                }
            }
        }
        void LaserDamage() {
            //пуля пускает луч с лицевой стороны
            RaycastHit Rayinfo;
            if (Physics.Raycast(transform.position, transform.forward, out Rayinfo, flyDistMax))
            {
                AICTRL rayTarget = Rayinfo.collider.GetComponent<AICTRL>();
                //изменяем значение максимального полета пули на "пока не попадем в препятствие"
                flyDistNow = Vector3.Distance(gameObject.transform.position, Rayinfo.point);

                if (rayTarget != null)
                {
                    rayTarget.SetDamage(Owner, (damageMax * 0.01f + (damageMax * (1-(flyDistNow/flyDistMax)) * 0.99f)));
                }
                //иначе создаем вмятину на поверхности попадания
                else
                {
                    if (PrefabParticleDent != null)
                    {
                        GameObject particleObj = Instantiate(PrefabParticleDent);
                        ParticleDentCTRL particleDentCTRL = particleObj.GetComponent<ParticleDentCTRL>();
                        if (particleDentCTRL != null)
                        {
                            particleObj.transform.position = Rayinfo.point + (-0.01f * transform.forward);
                            particleDentCTRL.normal = Rayinfo.normal;
                        }
                        else
                        {
                            Destroy(particleObj);
                        }
                    }
                }

                flyDistMax = flyDistNow;
                flyDistNow = 0;
            }
        }
        void ExplosionDamage() {

            //берем все колайдеры в радиусе взрыва
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionDist);
            //вытаскиваем только колайдеры врагов
            List<AICTRL> enemyes = new List<AICTRL>();
            List<AICTRL> enemyesSort = new List<AICTRL>();
            foreach (Collider collider in colliders)
            {
                AICTRL aICTRL = collider.GetComponent<AICTRL>();
                //проверка чтобы 2 раза не добавлять одного и тогоже врага
                if (aICTRL)
                {
                    bool found = false;
                    foreach (AICTRL aICTRL1 in enemyes)
                    {
                        if (aICTRL1 == aICTRL)
                        {
                            found = true;
                            break;
                        }
                    }

                    //Если этот враг не обнаружен в списке
                    if (!found)
                    {
                        enemyes.Add(aICTRL);
                    }
                }
            }

            //Сортируем по удаленности
            foreach (AICTRL enemyNow in enemyes)
            {
                if (enemyesSort.Count >= 1)
                {
                    float distNow = Vector3.Distance(transform.position, enemyNow.gameObject.transform.position);
                    //Перебираем список сортированный
                    for (int num = 0; num < enemyesSort.Count; num++)
                    {
                        float distSort = Vector3.Distance(transform.position, enemyesSort[num].gameObject.transform.position);
                        if (distNow < distSort)
                        {
                            enemyesSort.Insert(num, enemyNow);
                            break;
                        }
                    }
                }
                else enemyesSort.Add(enemyNow);
            }

            //Проверяем коллайдеры на то враги ли они
            for (int num = 0; num < MaxTargets && num < enemyesSort.Count; num++)
            {
                //Считаем урон для этой цели
                float dist = Vector3.Distance(transform.position, enemyesSort[num].transform.position);
                float damageThis = damageMax * (1 - (dist / explosionDist));
                float stunThis = (timeStuninig * (1 - (dist / explosionDist))) / (1 + num);
                damageThis = damageThis / 4;
                //Проверяем визуальный контакт с этим обьектом
                RaycastHit raycastHit;
                //пускаем луч в сторону движения, если луч попал значит пора взрывать
                if (Physics.Raycast(transform.position, (enemyesSort[num].transform.position - transform.position).normalized, out raycastHit, explosionDist))
                {
                    if (raycastHit.collider == enemyesSort[num])
                    {
                        damageThis *= 4;
                    }
                }

                enemyesSort[num].SetDamage(Owner, damageThis, stunThis, 0);
            }
        }
        void LightningDamage() {
            //пуля пускает луч с лицевой стороны
            RaycastHit Rayinfo;
            if (Physics.Raycast(transform.position, transform.forward, out Rayinfo, flyDistMax))
            {
                AICTRL rayTarget = Rayinfo.collider.GetComponent<AICTRL>();
                //изменяем значение максимального полета пули на "пока не попадем в препятствие"
                flyDistMax = Vector3.Distance(gameObject.transform.position, Rayinfo.point);

                //берем все колайдеры в радиусе взрыва
                Collider[] colliders = Physics.OverlapSphere(Rayinfo.point, explosionDist);
                //вытаскиваем только колайдеры врагов
                List<AICTRL> enemyes = new List<AICTRL>();
                List<AICTRL> enemyesSort = new List<AICTRL>();
                foreach (Collider collider in colliders) {
                    AICTRL aICTRL = collider.GetComponent<AICTRL>();
                    //проверка чтобы 2 раза не добавлять одного и тогоже врага
                    if (aICTRL) {
                        bool found = false;
                        foreach (AICTRL aICTRL1 in enemyes) {
                            if (aICTRL1 == aICTRL) {
                                found = true;
                                break;
                            }
                        }

                        //Если этот враг не обнаружен в списке
                        if (!found) {                            
                            enemyes.Add(aICTRL);
                        }
                    }
                }

                //Сортируем по удаленности
                foreach (AICTRL enemyNow in enemyes) {
                    if (enemyesSort.Count >= 1)
                    {
                        float distNow = Vector3.Distance(Rayinfo.point, enemyNow.gameObject.transform.position);
                        //Перебираем список сортированный
                        for (int num = 0; num < enemyesSort.Count; num++) {
                            float distSort = Vector3.Distance(Rayinfo.point, enemyesSort[num].gameObject.transform.position);
                            if (distNow < distSort) {
                                enemyesSort.Insert(num, enemyNow);
                                break;
                            }
                        }
                    }
                    else enemyesSort.Add(enemyNow);
                }


                //Проверяем коллайдеры на то враги ли они
                for (int num = 0; num < MaxTargets && num < enemyesSort.Count; num++)
                {
                    //Считаем урон для этой цели
                    float dist = Vector3.Distance(Rayinfo.point, enemyesSort[num].transform.position);
                    float damageThis = damageMax * (1 - (dist / explosionDist));
                    float stunThis = (timeStuninig * (1 - (dist / explosionDist))) / (1 + num);
                    damageThis = damageThis / 4;
                    //Проверяем визуальный контакт с этим обьектом
                    RaycastHit raycastHit;
                    //пускаем луч в сторону движения, если луч попал значит пора взрывать
                    if (Physics.Raycast(Rayinfo.point, (enemyesSort[num].transform.position - Rayinfo.point).normalized, out raycastHit, explosionDist))
                    {
                        if (raycastHit.collider == enemyesSort[num])
                        {
                            damageThis *= 4;
                        }
                    }

                    enemyesSort[num].SetDamage(Owner, damageThis, stunThis, 0);
                }

                if (rayTarget == null)
                //создаем вмятину на поверхности попадания
                {
                    if (PrefabParticleDent != null)
                    {
                        GameObject particleObj = Instantiate(PrefabParticleDent);
                        ParticleDentCTRL particleDentCTRL = particleObj.GetComponent<ParticleDentCTRL>();
                        if (particleDentCTRL != null)
                        {
                            particleObj.transform.position = Rayinfo.point + (-0.01f * transform.forward);
                            particleDentCTRL.normal = Rayinfo.normal;
                        }
                        else
                        {
                            Destroy(particleObj);
                        }
                    }
                }
            }
        }
    }
}
