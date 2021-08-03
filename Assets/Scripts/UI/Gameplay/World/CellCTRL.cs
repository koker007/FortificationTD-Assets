using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCTRL : MonoBehaviour
{
    [SerializeField]
    public GameplayCTRL gameplayCTRL;
    public int posX = 0;
    public int posY = 0;

    [SerializeField]
    Color[] colorsHeight;

    MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        testHeight(true);
        Invoke("testColor", UnityEngine.Random.Range(0.5f, 2f));
        Invoke("iniBuildCTRL", UnityEngine.Random.Range(0.1f, 0.5f));
    }

    void iniBuildCTRL()
    {
        if (gameplayCTRL != null && gameplayCTRL.cellCTRLs != null && gameplayCTRL.cellCTRLs[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z] == null)
        {
            gameplayCTRL.cellCTRLs[(int)gameObject.transform.position.x, (int)gameObject.transform.position.z] = this;
        }
    }


    public void testHeight(bool fast)
    {
        if (gameplayCTRL.cellsData[posX, posY].height != gameObject.transform.localScale.y) {
            if (!fast) {
                Vector3 scale = gameObject.transform.localScale;
                scale.y += (gameplayCTRL.cellsData[posX, posY].height - scale.y) * Time.unscaledDeltaTime;
                if (Math.Abs(scale.y - gameplayCTRL.cellsData[posX, posY].height) < 0.05f) {
                    scale.y = gameplayCTRL.cellsData[posX, posY].height;
                }

                gameObject.transform.localScale = scale;
            }
            else {
                Vector3 scale = gameObject.transform.localScale;
                scale.y = gameplayCTRL.cellsData[posX, posY].height;
                gameObject.transform.localScale = scale;
            }
        }
    }

    public void testColor() {
        if(mesh == null)
            mesh = gameObject.GetComponentInChildren<MeshRenderer>();

        if (mesh != null)
        {
            Color BasicColor = new Color();
            Color SelectColor = new Color(0.8f, 0.25f, 1);

            Material mat = mesh.material;
            if (gameplayCTRL.cellsData[posX, posY].height > 0.5f && gameplayCTRL.cellsData[posX, posY].height < 1.5f)
            {
                BasicColor = colorsHeight[1];
            }
            else if (gameplayCTRL.cellsData[posX, posY].height > 1.5f && gameplayCTRL.cellsData[posX, posY].height < 2.5f)
            {
                BasicColor = colorsHeight[2];
            }
            else if (gameplayCTRL.cellsData[posX, posY].height > 2.5f && gameplayCTRL.cellsData[posX, posY].height < 3.5f)
            {
                BasicColor = colorsHeight[3];
            }
            else if (gameplayCTRL.cellsData[posX, posY].height > 3.5f && gameplayCTRL.cellsData[posX, posY].height < 4.5f)
            {
                BasicColor = colorsHeight[4];
            }
            else if (gameplayCTRL.cellsData[posX, posY].height > 4.5f && gameplayCTRL.cellsData[posX, posY].height < 5.5f)
            {
                BasicColor = colorsHeight[5];
            }
            else if (gameplayCTRL.cellsData[posX, posY].height > 5.5f && gameplayCTRL.cellsData[posX, posY].height < 6.5f)
            {
                BasicColor = colorsHeight[6];
            }
            else if (gameplayCTRL.cellsData[posX, posY].height > 6.5f && gameplayCTRL.cellsData[posX, posY].height < 7.5f)
            {
                BasicColor = colorsHeight[7];
            }
            else if (gameplayCTRL.cellsData[posX, posY].height > 7.5f && gameplayCTRL.cellsData[posX, posY].height < 8.5f)
            {
                BasicColor = colorsHeight[8];
            }
            else if (gameplayCTRL.cellsData[posX, posY].height > 8.5f && gameplayCTRL.cellsData[posX, posY].height < 9.5f)
            {
                BasicColor = colorsHeight[9];
            }

            //Вытаскиваем игрока
            Player playerMe = null;
            if (gameplayCTRL != null && gameplayCTRL.players != null)
            {
                foreach (Player player in gameplayCTRL.players)
                {
                    if (player.isLocalPlayer)
                    {
                        playerMe = player;
                        break;
                    }
                }
            }

            if (playerMe != null && playerMe.controlsMouse.SelectCellCTRL == this)
            {
                mat.color = new Color(SelectColor.r + BasicColor.r/3, SelectColor.g + BasicColor.g/3, SelectColor.b + BasicColor.b/3);
            }
            else
            {
                mat.color = BasicColor;
            }
        }
    }


    public void OnMouseEnter()
    {
        if (gameplayCTRL != null && gameplayCTRL.infoText != null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            //Проверяем тип обьекта и вставляем ключ в зависимости от типа
            if (gameplayCTRL.cellsData[posX, posY].build == Building.getTypeName(Building.Type.Base))
            {
                gameplayCTRL.infoText.textKeyNow = "сellBuildBase";
            }
            else
            {
                gameplayCTRL.infoText.textKeyNow = "";
            }
        }
    }

    //принудительно перерисовать
    public void ReDraw() {
        testColor();
    }
}
