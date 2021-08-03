using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildEnemyBace : MonoBehaviour
{
    [SerializeField]
    GameObject TraficVizualizator;
    float timeToTraficVizualizator = 0;

    [SerializeField]
    GameObject testEnemy;

    Vector3 spawnPoint;

    float timeToSpawn = 0;
    float timeWaitSpawnMax = 2;
    void TestSpawn() {
        timeToSpawn -= Time.deltaTime;
        if (timeToSpawn < 0 && gameplayCTRL != null && gameplayCTRL.isServer) {
            timeToSpawn = timeWaitSpawnMax;

            GameObject enemy = null;
            if (testEnemy != null) {
                enemy = Instantiate(testEnemy);
            }

            if (enemy != null) {
                enemy.transform.position = new Vector3(gameObject.transform.position.x + 0.5f, 0, gameObject.transform.position.z + 0.5f);
                AICTRL aICTRL = enemy.GetComponent<AICTRL>();
                aICTRL.cellPosNext = spawnPoint;
                Mirror.NetworkServer.Spawn(enemy);
            }
        }

        timeToTraficVizualizator -= Time.unscaledDeltaTime;
        if (timeToTraficVizualizator < 0) {
            timeToTraficVizualizator = 30;

            GameObject enemy = null;
            if (TraficVizualizator != null)
            {
                enemy = Instantiate(TraficVizualizator);
            }

            if (enemy != null)
            {
                enemy.transform.position = new Vector3(gameObject.transform.position.x + 0.5f, 0, gameObject.transform.position.z + 0.5f);
                AICTRL aICTRL = enemy.GetComponent<AICTRL>();
                aICTRL.cellPosNext = spawnPoint;
                //UnityEngine.Networking.NetworkServer.Spawn(enemy);
            }
        }
    }

    Building building;
    GameplayCTRL gameplayCTRL;
    void iniBace() {
        if (building == null) {
            building = gameObject.GetComponent<Building>();
        }
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }

        if (building != null && gameplayCTRL != null) {
            if (gameplayCTRL.cellsData[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z].note == Building.getRotName(Building.Rotate.Down)) {
                spawnPoint = new Vector3((int)gameObject.transform.position.x + 0.5f, 0, (int)gameObject.transform.position.z + 0.5f - 1);
            }
            else if (gameplayCTRL.cellsData[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z].note == Building.getRotName(Building.Rotate.Left)) {
                spawnPoint = new Vector3((int)gameObject.transform.position.x + 0.5f - 1, 0, (int)gameObject.transform.position.z + 0.5f);
            }
            else if (gameplayCTRL.cellsData[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z].note == Building.getRotName(Building.Rotate.Right)) {
                spawnPoint = new Vector3((int)gameObject.transform.position.x + 0.5f + 1, 0, (int)gameObject.transform.position.z + 0.5f);
            }
            else if (gameplayCTRL.cellsData[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z].note == Building.getRotName(Building.Rotate.Up)) {
                spawnPoint = new Vector3((int)gameObject.transform.position.x + 0.5f, 0, (int)gameObject.transform.position.z + 0.5f + 1);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        iniBace();
    }

    // Update is called once per frame
    void Update()
    {
        TestSpawn();
    }
}
