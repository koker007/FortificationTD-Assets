using System.Collections;
using System.Collections.Generic;
//using System.Security.Policy;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class GPResearch : MonoBehaviour
{
    public static GPResearch main;

    public struct Allgun {
        public bool damage1;
        public bool damage2;
        public bool damage3;
        public bool dist1;
        public bool dist2;
        public bool dist3;
        public bool accuracy1;
        public bool accuracy2;
        public bool accuracy3;
        public bool rotate1;
        public bool rotate2;
        public bool rotate3;
        public bool speed1;
        public bool speed2;
        public bool speed3;

        public bool buildMinigun;
        public bool buildLaser;
        public bool buildThunder;
        public bool buildArtillery;
        public bool buildRocket;

        public const string damage1STR = "resAllDamage1";
        public const string damage2STR = "resAllDamage2";
        public const string damage3STR = "resAllDamage3";
        public const string dist1STR = "resAllDist1";
        public const string dist2STR = "resAllDist2";
        public const string dist3STR = "resAllDist3";
        public const string accuracy1STR = "resAllAccuracy1";
        public const string accuracy2STR = "resAllAccuracy2";
        public const string accuracy3STR = "resAllAccuracy3";
        public const string rotate1STR = "resAllRotate1";
        public const string rotate2STR = "resAllRotate2";
        public const string rotate3STR = "resAllRotate3";
        public const string speed1STR = "resAllSpeed1";
        public const string speed2STR = "resAllSpeed2";
        public const string speed3STR = "resAllSpeed3";

        public const string buildMinigunSTR = "resBuildMinigun";
        public const string buildLaserSTR = "resBuildLaser";
        public const string buildThunderSTR = "resBuildThunder";
        public const string buildArtillerySTR = "resBuildArtillery";
        public const string buildRosketSTR = "resBuildRocket";


        public const int damage1Cost = 15;
        public const int damage2Cost = 30;
        public const int damage3Cost = 80;
        public const int dist1Cost = 10;
        public const int dist2Cost = 20;
        public const int dist3Cost = 50;
        public const int accuracy1Cost = 20;
        public const int accuracy2Cost = 40;
        public const int accuracy3Cost = 80;
        public const int rotate1Cost = 10;
        public const int rotate2Cost = 10;
        public const int rotate3Cost = 10;
        public const int speed1Cost = 10;
        public const int speed2Cost = 15;
        public const int speed3Cost = 20;
    }
    public static Allgun allGun;

    public struct Turret
    {
        public bool explosiveShells;
        public bool shotgunShells;

        public const string explosiveShellsSTR = "resTurretExplosiveShells";
        public const string shotgunShellsSTR = "resTurretShotgunShells";
    }
    public static Turret turret;

    public struct Minigun {
        public bool expSpeed;
        public bool expNonstop;

        public const string expSpeedSTR = "resMinigunExpSpeed";
        public const string expNonstopSTR = "resTurretExpNonstop";
    }
    public static Minigun minigun;

    public struct Lasergun {
        public bool distDamage1;
        public bool distDamage2;
        public bool distDamage3;

        public const string distDamage1STR = "resLasergunDistDamage1";
        public const string distDamage2STR = "resLasergunDistDamage2";
        public const string distDamage3STR = "resLasergunDistDamage3";
    }
    public static Lasergun lasergun;

    public class Stats {
        public int neirodata = -1;
        public const string neirodataSTR = "neurodataStat";

        public float researchTimeStart = -0;
        public const string researchTimeStartSTR = "researchStart";
        public float researchTimeEnd = -0;
        public const string researchTimeEndSTR = "researchEnd";
    }
    public Stats stats = new Stats();

    static void UnlockTech(string nameRes) {
        if (main != null && CanResearchTech(nameRes)) {
            bool isOk = SteamUserStats.SetAchievement(nameRes);
            if (!isOk) {
                Debug.LogError("achivement not unlock:" + nameRes);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        main = this;

        InvokeRepeating("InvokeSteamGetAchivements", 1, UnityEngine.Random.Range(10, 15));
        //InvokeRepeating("InvokeSteamResearchGet", 2, Random.Range(2f, 3f));
        steam.timeLastLoadToSteamServer -= 60;

    }

    // Update is called once per frame
    void Update()
    {
        TestLoadToSteam(); //Подгрузка последних данных с сервера стима
        TestSaveToSteam(); //Попытка сохранения данных на сервер стима

        TestCompliteReserch();
        TestSaveOnSteam();
    }

    struct SteamAchivements {
        public float timeLastOnline;
        public float timeLastGetAll;
        public float timeLastLoadToSteamServer;

        public bool needSaveStats;

        public bool acivementsGets;
        public SteamAPICall_t dataStats;
    }
    static SteamAchivements steam;
    void InvokeSteamGetAchivements() {
        //получить ачивки из стима
        if (steam.timeLastOnline + 60 < Time.unscaledTime || steam.timeLastOnline <= 0) {
            steam.timeLastOnline = Time.unscaledTime;
            steam.acivementsGets = SteamUserStats.RequestCurrentStats();
            steam.dataStats = SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
        }
    }

    [SerializeField]
    bool unlockAll = false;
    [SerializeField]
    bool clearAll = false;
    void TestLoadToSteam() {
        //Если стим удачно загрузил данные с сервера
        if (steam.timeLastOnline > 0.5f && steam.timeLastGetAll + 5 < Time.unscaledTime)
        {
            steam.timeLastGetAll = Time.unscaledTime;

            if (unlockAll && SteamFriends.GetPersonaName() == "Koker 007") {
                allGun.accuracy1 = true;
                allGun.accuracy2 = true;
                allGun.accuracy3 = true;
                allGun.damage1 = true;
                allGun.damage2 = true;
                allGun.damage3 = true;
                allGun.dist1 = true;
                allGun.dist2 = true;
                allGun.dist3 = true;
                allGun.rotate1 = true;
                allGun.rotate2 = true;
                allGun.rotate3 = true;
                allGun.speed1 = true;
                allGun.speed2 = true;
                allGun.speed3 = true;

                turret.explosiveShells = true;
                turret.shotgunShells = true;

                minigun.expSpeed = true;
                minigun.expNonstop = true;

                lasergun.distDamage1 = true;
                lasergun.distDamage2 = true;
                lasergun.distDamage3 = true;

            }
            else
            {
                if (clearAll && SteamFriends.GetPersonaName() == "Koker 007") {
                    SteamUserStats.ResetAllStats(clearAll);
                    Setings.main.game.ResearchTech = "";
                    Setings.main.timeToSave = 0;
                }

                bool GetOk = false;
                //Общее
                GetOk = SteamUserStats.GetAchievement(Allgun.accuracy1STR, out allGun.accuracy1);
                GetOk = SteamUserStats.GetAchievement(Allgun.accuracy2STR, out allGun.accuracy2);
                GetOk = SteamUserStats.GetAchievement(Allgun.accuracy3STR, out allGun.accuracy3);
                GetOk = SteamUserStats.GetAchievement(Allgun.damage1STR, out allGun.damage1);
                GetOk = SteamUserStats.GetAchievement(Allgun.damage2STR, out allGun.damage2);
                GetOk = SteamUserStats.GetAchievement(Allgun.damage3STR, out allGun.damage3);
                GetOk = SteamUserStats.GetAchievement(Allgun.dist1STR, out allGun.dist1);
                GetOk = SteamUserStats.GetAchievement(Allgun.dist2STR, out allGun.dist2);
                GetOk = SteamUserStats.GetAchievement(Allgun.dist3STR, out allGun.dist3);
                GetOk = SteamUserStats.GetAchievement(Allgun.rotate1STR, out allGun.rotate1);
                GetOk = SteamUserStats.GetAchievement(Allgun.rotate2STR, out allGun.rotate2);
                GetOk = SteamUserStats.GetAchievement(Allgun.rotate3STR, out allGun.rotate3);
                GetOk = SteamUserStats.GetAchievement(Allgun.speed1STR, out allGun.speed1);
                GetOk = SteamUserStats.GetAchievement(Allgun.speed2STR, out allGun.speed2);
                GetOk = SteamUserStats.GetAchievement(Allgun.speed3STR, out allGun.speed3);

                GetOk = SteamUserStats.GetAchievement(Allgun.buildMinigunSTR, out allGun.buildMinigun);
                GetOk = SteamUserStats.GetAchievement(Allgun.buildLaserSTR, out allGun.buildLaser);
                GetOk = SteamUserStats.GetAchievement(Allgun.buildThunderSTR, out allGun.buildThunder);
                GetOk = SteamUserStats.GetAchievement(Allgun.buildArtillerySTR, out allGun.buildArtillery);
                GetOk = SteamUserStats.GetAchievement(Allgun.buildRosketSTR, out allGun.buildRocket);

                //Туррель
                //GetOk = SteamUserStats.GetAchievement(Turret.explosiveShellsSTR, out turret.explosiveShells);
                //GetOk = SteamUserStats.GetAchievement(Turret.shotgunShellsSTR, out turret.shotgunShells);

                //Миниган
                //GetOk = SteamUserStats.GetAchievement(Minigun.expSpeedSTR, out minigun.expSpeed);
                //GetOk = SteamUserStats.GetAchievement(Minigun.expNonstopSTR, out minigun.expNonstop);

                //Лазерган
                //GetOk = SteamUserStats.GetAchievement(Lasergun.distDamage1STR, out lasergun.distDamage1);
                //GetOk = SteamUserStats.GetAchievement(Lasergun.distDamage2STR, out lasergun.distDamage2);
                //GetOk = SteamUserStats.GetAchievement(Lasergun.distDamage3STR, out lasergun.distDamage3);


                if (!GetOk) Debug.LogError("Steam not get achivements");

                if (steam.dataStats != null) {
                    //получаем данные если нечего не ждет отправку
                    if (!steam.needSaveStats)
                    {
                        GetOk = SteamUserStats.GetStat(Stats.neirodataSTR, out stats.neirodata);
                        GetOk = SteamUserStats.GetStat(Stats.researchTimeStartSTR, out stats.researchTimeStart);
                        GetOk = SteamUserStats.GetStat(Stats.researchTimeEndSTR, out stats.researchTimeEnd);
                    }
                }
            }
        }
    }
    void TestSaveToSteam() {
        //Если нужно обновить данные и пришло время попытки
        if (steam.needSaveStats && steam.timeLastLoadToSteamServer + 60 < Time.unscaledTime) {
            //Запоминаем время последней попытки
            steam.timeLastLoadToSteamServer = Time.unscaledTime;

            bool ok = false;

            ok = SteamUserStats.SetStat(Stats.neirodataSTR, stats.neirodata);
            if (stats.researchTimeStart >= 0 || stats.researchTimeEnd >= 0) {
                ok = SteamUserStats.SetStat(Stats.researchTimeStartSTR, stats.researchTimeStart);
                ok = SteamUserStats.SetStat(Stats.researchTimeEndSTR, stats.researchTimeEnd);
            }

            //если отправка успешна
            if (SteamUserStats.StoreStats()) {
                //Попытки прекращяем
                steam.needSaveStats = false;
            }
        }
    }

    public void SetStatValue(string name, float value) {
        steam.needSaveStats = true;

        if (name == Stats.neirodataSTR) {
            stats.neirodata = (int)value + stats.neirodata;
        }

        else if (name == Stats.researchTimeStartSTR) {
            stats.researchTimeStart = value;
        }
        else if (name == Stats.researchTimeEndSTR) {
            stats.researchTimeEnd = value;
        }
    }

    public int GetCountTech() {
        int count = 0;

        if (allGun.accuracy1)
            count++;
        if (allGun.accuracy2)
            count++;
        if (allGun.accuracy3)
            count++;
        if (allGun.damage1)
            count++;
        if (allGun.damage2)
            count++;
        if (allGun.damage3)
            count++;
        if (allGun.dist1)
            count++;
        if (allGun.dist2)
            count++;
        if (allGun.dist3)
            count++;
        if (allGun.rotate1)
            count++;
        if (allGun.rotate2)
            count++;
        if (allGun.rotate3)
            count++;
        if (allGun.speed1)
            count++;
        if (allGun.speed2)
            count++;
        if (allGun.speed3)
            count++;

        if (allGun.buildMinigun)
            count++;
        if (allGun.buildLaser)
            count++;
        if (allGun.buildThunder)
            count++;
        if (allGun.buildArtillery)
            count++;
        if (allGun.buildRocket)
            count++;

        if (turret.explosiveShells)
            count++;
        if (turret.shotgunShells)
            count++;

        if (minigun.expSpeed)
            count++;
        if (minigun.expNonstop)
            count++;

        if (lasergun.distDamage1)
            count++;
        if (lasergun.distDamage2)
            count++;
        if (lasergun.distDamage3)
            count++;

        return count;
    }
    public int GetMaxTech() {
        int count = 0;

        if (allGun.accuracy1 || !allGun.accuracy1)
            count++;
        if (allGun.accuracy2 || !allGun.accuracy2)
            count++;
        if (allGun.accuracy3 || !allGun.accuracy3)
            count++;
        if (allGun.damage1 || !allGun.damage1)
            count++;
        if (allGun.damage2 || !allGun.damage2)
            count++;
        if (allGun.damage3 || !allGun.damage3)
            count++;
        if (allGun.dist1 || !allGun.dist1)
            count++;
        if (allGun.dist2 || !allGun.dist2)
            count++;
        if (allGun.dist3 || !allGun.dist3)
            count++;
        if (allGun.rotate1 || !allGun.rotate1)
            count++;
        if (allGun.rotate2 || !allGun.rotate2)
            count++;
        if (allGun.rotate3 || !allGun.rotate3)
            count++;
        if (allGun.speed1 || !allGun.speed1)
            count++;
        if (allGun.speed2 || !allGun.speed2)
            count++;
        if (allGun.speed3 || !allGun.speed3)
            count++;

        if (allGun.buildMinigun || !allGun.buildMinigun)
            count++;
        if (allGun.buildLaser || !allGun.buildLaser)
            count++;
        if (allGun.buildThunder || !allGun.buildThunder)
            count++;
        if (allGun.buildRocket || !allGun.buildRocket)
            count++;
        if (allGun.buildArtillery || !allGun.buildArtillery)
            count++;

        /*
        if (turret.explosiveShells || !turret.explosiveShells)
            count++;
        if (turret.shotgunShells || !turret.shotgunShells)
            count++;

        if (minigun.expSpeed || !minigun.expSpeed)
            count++;
        if (minigun.expNonstop || !minigun.expNonstop)
            count++;

        if (lasergun.distDamage1 || !lasergun.distDamage1)
            count++;
        if (lasergun.distDamage2 || !lasergun.distDamage2)
            count++;
        if (lasergun.distDamage3 || !lasergun.distDamage3)
            count++;
        */

        return count;
    }


    static public bool CanResearchTech(string techName) {
        bool result = false;
        //проверяем можно ли начать изучение этой технологии
        if ((techName == Allgun.accuracy1STR && !allGun.accuracy1)
            || (techName == Allgun.accuracy2STR && allGun.accuracy1 && !allGun.accuracy2)
            || (techName == Allgun.accuracy3STR && allGun.accuracy2 && !allGun.accuracy3)
            || (techName == Allgun.damage1STR && !allGun.damage1)
            || (techName == Allgun.damage2STR && allGun.damage1 && !allGun.damage2)
            || (techName == Allgun.damage3STR && allGun.damage2 && !allGun.damage3)
            || (techName == Allgun.dist1STR && !allGun.dist1)
            || (techName == Allgun.dist2STR && allGun.dist1 && !allGun.dist2)
            || (techName == Allgun.dist3STR && allGun.dist2 && !allGun.dist3)
            || (techName == Allgun.rotate1STR && !allGun.rotate1)
            || (techName == Allgun.rotate2STR && allGun.rotate1 && !allGun.rotate2)
            || (techName == Allgun.rotate3STR && allGun.rotate2 && !allGun.rotate3)
            || (techName == Allgun.speed1STR && !allGun.speed1)
            || (techName == Allgun.speed2STR && allGun.speed1 && !allGun.speed2)
            || (techName == Allgun.speed3STR && allGun.speed2 && !allGun.speed3)

            || (techName == Allgun.buildMinigunSTR && allGun.speed1 && !allGun.buildMinigun)
            || (techName == Allgun.buildLaserSTR && allGun.dist1 && !allGun.buildLaser)
            || (techName == Allgun.buildThunderSTR && allGun.buildLaser && allGun.accuracy2 && allGun.damage2 && allGun.dist2 && allGun.rotate2 && allGun.speed2 && !allGun.buildThunder)
            || (techName == Allgun.buildRosketSTR && allGun.accuracy1 && !allGun.buildRocket)
            || (techName == Allgun.buildArtillerySTR && allGun.rotate1 && !allGun.buildArtillery))
        {
            result = true;
        }

        return result;
    }
    //Начать изучение технологии
    public void StartResearchTech(string techName, int cost) {

        float researctTimeAll = stats.researchTimeEnd - stats.researchTimeStart;
        float researctTimeNow = (float)TimeFromNET.GetSecondsFrom2020(TimeFromNET.GetFastestNISTDate()) - stats.researchTimeStart;

        //проверяем можно ли начать изучение этой технологии
        //Если это не тоже самое иследование и иследование выполнено менее 10% или больще 100%
        if (CanResearchTech(techName) && (techName != Setings.main.game.ResearchTech) && (researctTimeNow / researctTimeAll) >= 1 && stats.neirodata >= cost)
        {
            //Платим
            stats.neirodata -= cost;
            steam.needSaveStats = true;

            //Начинаем изучение
            DateTime timeStart = TimeFromNET.GetFastestNISTDate();
            if (timeStart != DateTime.MinValue)
            {
                DateTime timeEnd = new DateTime(timeStart.Ticks).AddSeconds(60 * 1.5f * (GetCountTech() + 1));

                Setings.main.game.ResearchTech = techName;
                Setings.main.timeToSave = 0;

                SetStatValue(Stats.researchTimeStartSTR, (float)TimeFromNET.GetSecondsFrom2020(timeStart));
                SetStatValue(Stats.researchTimeEndSTR, (float)TimeFromNET.GetSecondsFrom2020(timeEnd));

            }
        }
        //Если начать изучение не получилось, обновляем список иследований, на всякий случай
        else {
            TechSelectScroll.timeLastUpdateTech = 0;
        }


    }

    public void TestCompliteReserch()
    {
        if (stats.researchTimeEnd > 0) {
            DateTime TimeEnd = new DateTime(2020, 01, 01).AddSeconds(stats.researchTimeEnd);
            DateTime TimeNow = TimeFromNET.GetFastestNISTDate();
            //Если выбрано какоето иследование и время изучения вышло
            if (Setings.main && Setings.main.game != null && Setings.main.game.ResearchTech != "" && stats.researchTimeEnd != 0 && TimeNow != DateTime.MinValue) {
                //Проверяем время
                if (stats.researchTimeEnd != 0 && TimeEnd.Ticks < TimeNow.Ticks) {
                    UnlockTech(Setings.main.game.ResearchTech);
                    bool ok = SteamUserStats.StoreStats();
                    timeToSaveOnSteam = 0;

                    Setings.main.game.ResearchTech = "";
                    Setings.main.timeToSave = 0;
                }
            }
        }
    }

    float timeToSaveOnSteam = 0.1f;
    float timeToSaveOnSteamLast = 0;
    void TestSaveOnSteam() {
        timeToSaveOnSteam -= Time.unscaledDeltaTime;
        if (timeToSaveOnSteam < 0 && timeToSaveOnSteamLast + 60 < Time.unscaledTime) {
            timeToSaveOnSteamLast = Time.unscaledTime;
            timeToSaveOnSteam = 3 * 60;
        }
    }
}
