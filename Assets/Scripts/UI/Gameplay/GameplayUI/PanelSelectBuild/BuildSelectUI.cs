using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildSelectUI : MonoBehaviour
{
    [SerializeField]
    RawImage SelectCellImage;
    [SerializeField]
    RenderTexture SelectCellRenderTexture;
    [SerializeField]
    Image OpenImage;
    [SerializeField]
    Image OpenFireModeImage;
    [SerializeField]
    Image ReloadImage;

    [SerializeField]
    GameplayCTRL gameplayCTRL;
    [SerializeField]
    Player playerMe;
    [SerializeField]
    Camera SelectCellCamera;

    void iniGamePlayCTRL() {
        if (gameplayCTRL == null) {
            GameObject gamePlayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gamePlayObj != null)
                gameplayCTRL = gamePlayObj.GetComponent<GameplayCTRL>();
        }
    }
    void iniPlayerMe() {
        if (playerMe == null && gameplayCTRL != null) {
            foreach (Player player in gameplayCTRL.players) {
                if (player.isLocalPlayer) {
                    playerMe = player;
                    break;
                }
            }
        }
    }
    void iniCamera() {
        if (SelectCellCamera == null) {
            GameObject camera = GameObject.FindGameObjectWithTag("SelectCellCamera");
            if (camera != null) {
                SelectCellCamera = camera.GetComponent<Camera>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("iniGamePlayCTRL", 0.5f, Random.Range(1f, 10f));
        InvokeRepeating("iniPlayerMe", 0.5f, Random.Range(1f, 10f));
        InvokeRepeating("iniCamera", 0.5f, Random.Range(1f, 10f));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayCTRL.main != null)
        {
            iniTestCelectCell();
            TestOpenImage();
        }
    }

    void iniTestCelectCell() {
        if (SelectCellImage != null)
        {
            if (SelectCellRenderTexture == null && SelectCellCamera != null && SelectCellImage != null)
            {
                SelectCellRenderTexture = new RenderTexture(200, 200, 0);
                SelectCellCamera.targetTexture = SelectCellRenderTexture;
                SelectCellImage.texture = SelectCellRenderTexture;
            }

            cameraTransform();

            void cameraTransform() {
                if (SelectCellCamera != null && playerMe != null && playerMe.controlsMouse.SelectCellCTRL != null) {
                    Vector2Int posCell = new Vector2Int(playerMe.controlsMouse.SelectCellCTRL.posX, playerMe.controlsMouse.SelectCellCTRL.posY); 
                    //Позиция ячейки
                    Vector3 posCam = new Vector3(posCell.x + 0.5f, gameplayCTRL.cellsData[posCell.x, posCell.y].height + 1 , posCell.y + 0.5f);
                    Vector3 posTurretPlus = new Vector3(0.5f, 0, 0.5f);
                    if (gameplayCTRL.buildings[posCell.x, posCell.y] != null && gameplayCTRL.buildings[posCell.x, posCell.y].Turret != null) {
                        posTurretPlus = gameplayCTRL.buildings[posCell.x, posCell.y].Turret.transform.forward + gameplayCTRL.buildings[posCell.x, posCell.y].Turret.transform.right * 0.3f;
                    }

                    SelectCellCamera.transform.position = posCam + posTurretPlus;
                    SelectCellCamera.transform.rotation = Quaternion.LookRotation(-posTurretPlus + new Vector3(0,-0.75f, 0));
                }
            }
        }
    }

    //предыдущая позиция выделенной ячейки
    Vector2Int SelectCellOld = new Vector2Int();
    void TestOpenImage() {
        Vector2Int posCell = new Vector2Int();
        if(playerMe != null && playerMe.controlsMouse.SelectCellCTRL != null)
            posCell = new Vector2Int(playerMe.controlsMouse.SelectCellCTRL.posX, playerMe.controlsMouse.SelectCellCTRL.posY);

        if (OpenImage) {

            //Раскрываем
            if (OpenImage.pixelsPerUnitMultiplier < 1 && GameplayCTRL.main.buildings[posCell.x, posCell.y] != null) {
                OpenImage.pixelsPerUnitMultiplier += Time.unscaledDeltaTime;
                OpenImage.SetAllDirty();
            }
            if (SelectCellOld != posCell) {
                OpenImage.pixelsPerUnitMultiplier = 0.01f;
                OpenImage.SetAllDirty();

            }
        }
        
        if (OpenFireModeImage) {
            //Раскрываем
            if (OpenFireModeImage.pixelsPerUnitMultiplier < 1 && GameplayCTRL.main.buildings[posCell.x, posCell.y] != null
                && (GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.PillBox
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Turret
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Artillery
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Minigun
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Laser
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Thunder
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Rocket))
            {
                OpenFireModeImage.pixelsPerUnitMultiplier += Time.unscaledDeltaTime;
                OpenFireModeImage.SetAllDirty();
            }
            if (SelectCellOld != posCell)
            {
                OpenFireModeImage.pixelsPerUnitMultiplier = 0.01f;
                OpenFireModeImage.SetAllDirty();

            }
        }

        if (ReloadImage) {
            //Если на ячейке есть строение
            if (GameplayCTRL.main.buildings[posCell.x, posCell.y] != null)
            {
                if (GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.PillBox
                ||  GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Turret
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Artillery
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Minigun
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Laser
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Thunder
                || GameplayCTRL.main.buildings[posCell.x, posCell.y].type == Building.Type.Rocket)
                {
                    float timeReloadAll = GameplayCTRL.main.buildings[posCell.x, posCell.y].GetFireSpeed();
                    float timeReloadNow = Time.time - GameplayCTRL.main.buildings[posCell.x, posCell.y].timeLastFire;
                    ReloadImage.fillAmount = 1 - (timeReloadNow / timeReloadAll);
                }
                else
                {
                    ReloadImage.fillAmount = 0;
                }
            }
            else {
                ReloadImage.fillAmount = 0;
            }
        }

        SelectCellOld = posCell;
    }
}
