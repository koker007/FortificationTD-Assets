using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCTRL : MonoBehaviour
{
    public static TerrainCTRL main;

    [SerializeField]
    GameplayCTRL gameplayCTRL;
    void iniGameplayCTRL() {
        if (gameplayCTRL == null) {
            gameplayCTRL = gameObject.GetComponentInParent<GameplayCTRL>();
        }
    }

    [SerializeField]
    //Префаб ячейки поверхности
    GameObject PrefabCell;

    //CellCTRL[,] Cells = new CellCTRL[60,60];
    int cellTestDraw = 0;
    void iniCells() {
        if (PrefabCell != null && gameplayCTRL != null) {

            //Нужно проинициализировать префаб ячейки
            //по x
            for (int x = 0; x < gameplayCTRL.cellsData.GetLength(0); x++) {
                for (int y = 0; y < gameplayCTRL.cellsData.GetLength(1); y++) {
                    //Если высота больше нуля
                    if (gameplayCTRL.cellsData[x,y].height > 0.5f && gameplayCTRL.cellCTRLs[x, y] == null)
                    {
                        Debug.Log("cell " + x + " " + y + " create");
                        //Создаем префаб ячейки
                        GameObject cellObj = Instantiate(PrefabCell, gameObject.transform);
                        //перемещаем
                        cellObj.transform.position = new Vector3(x, 0, y);
                        //получаем контроллер ячейки
                        gameplayCTRL.cellCTRLs[gameplayCTRL.cellsData[x, y].posX, gameplayCTRL.cellsData[x, y].posY] = cellObj.GetComponent<CellCTRL>();
                        gameplayCTRL.cellCTRLs[gameplayCTRL.cellsData[x, y].posX, gameplayCTRL.cellsData[x, y].posY].gameplayCTRL = gameplayCTRL;
                        gameplayCTRL.cellCTRLs[gameplayCTRL.cellsData[x, y].posX, gameplayCTRL.cellsData[x, y].posY].posX = x;
                        gameplayCTRL.cellCTRLs[gameplayCTRL.cellsData[x, y].posX, gameplayCTRL.cellsData[x, y].posY].posY = y;
                    }
                }
            }
        }
    }

    public static void iniCell(WorldClass.Cell cell) {
        //Если у ячейки нету контроллера
        if (GameplayCTRL.main && main && GameplayCTRL.main.cellCTRLs[cell.posX, cell.posY] == null) {
            if (GameplayCTRL.main.cellsData[cell.posX, cell.posY].height > 0.5f) {
                Debug.Log("cell " + cell.posX + " " + cell.posY + " create");
                //Создаем префаб ячейки
                GameObject cellObj = Instantiate(main.PrefabCell, main.gameObject.transform);
                //перемещаем
                cellObj.transform.position = new Vector3(cell.posX, 0, cell.posY);
                //получаем контроллер ячейки
                GameplayCTRL.main.cellCTRLs[cell.posX, cell.posY] = cellObj.GetComponent<CellCTRL>();
                GameplayCTRL.main.cellCTRLs[cell.posX, cell.posY].gameplayCTRL = GameplayCTRL.main;
                GameplayCTRL.main.cellCTRLs[cell.posX, cell.posY].posX = cell.posX;
                GameplayCTRL.main.cellCTRLs[cell.posX, cell.posY].posY = cell.posY;
            }
        }
    }

    void ReDrawCell() {
        if (gameplayCTRL != null && gameplayCTRL.cellCTRLs != null) {
            int posx = cellTestDraw / gameplayCTRL.cellCTRLs.GetLength(0);
            int posy = cellTestDraw % gameplayCTRL.cellCTRLs.GetLength(0);

            if (posx < 60 && posy < 60 && gameplayCTRL.cellCTRLs[posx, posy] != null) {
                gameplayCTRL.cellCTRLs[posx, posy].ReDraw();
                gameplayCTRL.cellCTRLs[posx, posy].testHeight(true);
            }
            //Переключаемся на следующую ячейку
            cellTestDraw++;
            if (cellTestDraw > gameplayCTRL.cellCTRLs.GetLength(0) * gameplayCTRL.cellCTRLs.GetLength(0)) {
                cellTestDraw = 0;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        iniGameplayCTRL();
    }

    // Update is called once per frame
    void Update()
    {
        testGen();

        ReDrawCell();
        StartTestHeight();
    }

    float timeChangeHeight = 10;
    void StartTestHeight() {
        if (gameplayCTRL.gamemode > 1)
        {
            timeChangeHeight -= Time.deltaTime;
            if (timeChangeHeight > 0)
            {
                testHeight(false);
            }
            else if (timeChangeHeight > -10)
            {
                testHeight(true);
                timeChangeHeight = -9999;
            }
        }
    }
    public void testHeight(bool fast)
    {
        if (1 != gameObject.transform.localScale.y)
        {
            if (!fast)
            {
                Vector3 scale = gameObject.transform.localScale;
                scale.y += (1 - scale.y) * Time.unscaledDeltaTime;
                if (Mathf.Abs(scale.y - 1) < 0.05f)
                {
                    scale.y = 1;
                }

                gameObject.transform.localScale = scale;
            }
            else
            {
                Vector3 scale = gameObject.transform.localScale;
                scale.y = 1;
                gameObject.transform.localScale = scale;
            }
        }
    }

    int oldGameMode = 0;
    void testGen() {
        if (gameplayCTRL != null) {
            if (gameplayCTRL.gamemode != oldGameMode && Player.me && Time.unscaledTime - Player.me.TimeConnected > 3) {

                //1 => 2
                if (gameplayCTRL.gamemode == 2 && oldGameMode < 2) {
                    iniCells();
                }

                oldGameMode = gameplayCTRL.gamemode;
            }
        }
    }
}
