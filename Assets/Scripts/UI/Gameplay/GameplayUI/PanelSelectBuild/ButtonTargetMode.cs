using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTargetMode : MonoBehaviour
{
    [SerializeField]
    Image selected;
    [SerializeField]
    Building.TargetMode targetMode = Building.TargetMode.DistMin;


    Player playerMe;
    void iniPlayer() {
        if (playerMe == null) {
            GameObject[] PlayersObj = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject playerObj in PlayersObj) {
                Player player = playerObj.GetComponent<Player>();
                if (player.isLocalPlayer) {
                    playerMe = player;
                }
            }
        }
    }

    GameplayCTRL gameplayCTRL;
    void iniGameplayCTRL() {
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");
            if (gameplayObj != null) {
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
            }
        }
    }

    private void Start()
    {
        InvokeRepeating("iniPlayer", 0.5f, Random.RandomRange(5f, 10f));
        InvokeRepeating("iniGameplayCTRL", 0.5f, Random.RandomRange(5f, 10f));
    }

    void Update()
    {
        testButtonCelect();
    }

    Button button;
    Image image;

    void testButtonCelect() {
        if (gameplayCTRL != null && playerMe != null) {
            //Если элемент кнопки не обраружен
            if (button == null || image == null) {
                button = gameObject.GetComponent<Button>();
                if (button == null) {
                    button = gameObject.GetComponentInChildren<Button>();
                }

                image = gameObject.GetComponent<Image>();
            }
            else {
                //проверяем что есть строение на выделенной ячейке и у этого строения есть пушка и у этой пушки не активен текущий режим стрельбы
                if (playerMe.controlsMouse.SelectCellCTRL != null &&
                    gameplayCTRL.buildings[playerMe.controlsMouse.SelectCellCTRL.posX, playerMe.controlsMouse.SelectCellCTRL.posY] != null &&
                    gameplayCTRL.buildings[playerMe.controlsMouse.SelectCellCTRL.posX, playerMe.controlsMouse.SelectCellCTRL.posY].Turret != null &&
                    gameplayCTRL.buildings[playerMe.controlsMouse.SelectCellCTRL.posX, playerMe.controlsMouse.SelectCellCTRL.posY].targetMode != targetMode) {

                    //Если кнопка не активирована то активируем
                    if (!button.interactable) {
                        button.interactable = true;
                        image.color = new Color(1,1,1);
                        if (selected != null)
                            selected.gameObject.active = false;
                    }
                }
                else {
                    //Деактивируем кнопку
                    if (button.interactable) {
                        button.interactable = false;
                        image.color = new Color(1, 1, 1);
                        if (selected != null)
                            selected.gameObject.active = true;
                    }
                }
            }
        }
    }

    public void ButtonModeClick() {
        //Меняем состояние башни только если есть игрок
        if (playerMe != null && playerMe.controlsMouse.SelectCellCTRL != null) {
            //выделенная ячейка
            Vector2Int selectCell = new Vector2Int(playerMe.controlsMouse.SelectCellCTRL.posX, playerMe.controlsMouse.SelectCellCTRL.posY);

            playerMe.CmdTargetMode(selectCell, Building.getTargetModeName(targetMode));
        }
    }

}
