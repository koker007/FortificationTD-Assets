using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUpgrade : MonoBehaviour
{

    //[Header("")]
    [SerializeField]
    SkinnedMeshRenderer renderTurret;
    [SerializeField]
    MeshRenderer renderTurret2;
    [SerializeField]
    int numUpgradeMaterial = 0;

    //Время изменения цвета
    const float timeChangeColor = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        testColorUpgrade();
    }

    void testColorUpgrade() {
        if (GameplayCTRL.main) {
            //получаем позицию строения
            Vector2Int cellPos = new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.z);
            //выходим если строение вышло за рамки игровой зоны
            if (cellPos.x < 0 || cellPos.y < 0 || cellPos.x >= GameplayCTRL.main.cellsData.GetLength(0) || cellPos.y > GameplayCTRL.main.cellsData.GetLength(1))
                return;

            //Если апгрейд произошел более 5 секунд назад
            if (GameplayCTRL.main.cellsData[cellPos.x, cellPos.y].timeLastBuilding + timeChangeColor < GameplayCTRL.main.timeGamePlay)
                return;

            if (renderTurret && numUpgradeMaterial < renderTurret.materials.Length) {
                Material materialUpgrade = renderTurret.materials[numUpgradeMaterial];
                if (materialUpgrade) {
                    materialUpgrade.color = GetColorNow();
                    renderTurret.materials[numUpgradeMaterial] = materialUpgrade;
                }
            }
            else if (renderTurret2 && numUpgradeMaterial < renderTurret2.materials.Length) {
                Material materialUpgrade = renderTurret2.materials[numUpgradeMaterial];
                if (materialUpgrade)
                {
                    materialUpgrade.color = GetColorNow();
                    renderTurret2.materials[numUpgradeMaterial] = materialUpgrade;
                }
            }

            Color GetColorNow() {
                Color result = new Color();

                //Получаем коофицент прогресса
                float progressCoof = 1 - (((GameplayCTRL.main.cellsData[cellPos.x, cellPos.y].timeLastBuilding + timeChangeColor) - GameplayCTRL.main.timeGamePlay)/timeChangeColor);
                Color need = new Color();
                Color start = new Color();

                int numColorNeed = (GameplayCTRL.main.cellsData[cellPos.x, cellPos.y].getCountTech()+2)/2;
                int plusHalf = (GameplayCTRL.main.cellsData[cellPos.x, cellPos.y].getCountTech()+2) % 2;

                //Если необходимый номер ниже количества цветов
                if (numColorNeed < GameplayCTRL.main.EnemyColors.Length) {
                    if (plusHalf == 0) {
                        //получаем смещение назад
                        if (numColorNeed > 0)
                        {
                            start = Color.Lerp(GameplayCTRL.main.EnemyColors[numColorNeed - 1], GameplayCTRL.main.EnemyColors[numColorNeed], 0.5f);
                            need = GameplayCTRL.main.EnemyColors[numColorNeed];
                        }
                        else
                        {
                            start = GameplayCTRL.main.EnemyColors[numColorNeed];
                            need = GameplayCTRL.main.EnemyColors[numColorNeed];
                        }

                    }
                    else {
                        //получаем смещение вперед
                        if (numColorNeed < GameplayCTRL.main.EnemyColors.Length)
                        {
                            start = GameplayCTRL.main.EnemyColors[numColorNeed];
                            need = Color.Lerp(GameplayCTRL.main.EnemyColors[numColorNeed], GameplayCTRL.main.EnemyColors[numColorNeed + 1], 0.5f);
                        }
                        else
                        {
                            start = GameplayCTRL.main.EnemyColors[GameplayCTRL.main.EnemyColors.Length - 1];
                            need = GameplayCTRL.main.EnemyColors[GameplayCTRL.main.EnemyColors.Length - 1];
                        }
                    }

                    result = Color.Lerp(start, need, progressCoof);
                }
                else result = GameplayCTRL.main.EnemyColors[GameplayCTRL.main.EnemyColors.Length - 1];

                result = result*0.85f;

                return result;
            }
        }
    }
}
