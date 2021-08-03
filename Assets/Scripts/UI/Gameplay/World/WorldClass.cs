using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WorldClass : NetworkBehaviour
{
    //Ячейка мира
    public struct Cell
    {
        public sbyte posX;
        public sbyte posY;

        public bool blockingMove;

        public sbyte height; //высота у ячейки

        public ushort traficDanger; //Опасность пути

        public ulong ownerSteamID;

        public float traficNum; //Номер ячейки для пути
        public float traficNum1;
        public float traficNum2;

        public string note; //Примечание

        public float cost;

        public string build;

        public GPResearch.Allgun ResearchAllGun;
        public GPResearch.Turret ResearchTurret;
        public GPResearch.Minigun ResearchMinigun;
        public GPResearch.Lasergun ResearchLasergun;

        public float timeLastUpdate; //Время последнего изменения данных ячейки
        public float timeLastBuilding; //Время последнего строительства на ячейке
        public float timeoutBuild; //Таймаут строительства

        public bool isTech(string nameTech) {
            bool isTech = false;

            if (nameTech == GPResearch.Allgun.accuracy1STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.accuracy2STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.accuracy3STR) isTech = true;

            else if (nameTech == GPResearch.Allgun.damage1STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.damage2STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.damage3STR) isTech = true;

            else if (nameTech == GPResearch.Allgun.dist1STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.dist2STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.dist3STR) isTech = true;

            else if (nameTech == GPResearch.Allgun.rotate1STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.rotate2STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.rotate3STR) isTech = true;

            else if (nameTech == GPResearch.Allgun.speed1STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.speed2STR) isTech = true;
            else if (nameTech == GPResearch.Allgun.speed3STR) isTech = true;

            return isTech;
        }
        public int getCountTech() {
            int count = 0;

            if (ResearchAllGun.accuracy1) count++;
            if (ResearchAllGun.accuracy2) count++;
            if (ResearchAllGun.accuracy3) count++;

            if (ResearchAllGun.damage1) count++;
            if (ResearchAllGun.damage2) count++;
            if (ResearchAllGun.damage3) count++;

            if (ResearchAllGun.dist1) count++;
            if (ResearchAllGun.dist2) count++;
            if (ResearchAllGun.dist3) count++;

            if (ResearchAllGun.rotate1) count++;
            if (ResearchAllGun.rotate2) count++;
            if (ResearchAllGun.rotate3) count++;

            if (ResearchAllGun.speed1) count++;
            if (ResearchAllGun.speed2) count++;
            if (ResearchAllGun.speed3) count++;

            return count;
        }
    }

}
