using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualFireDist : MonoBehaviour
{
    static public VisualFireDist main;
    [SerializeField]
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestCTRL();
    }

    bool oldsetNull = false;
    static public void SetTransform(Vector3 position, float range) {
        if (main) {
            main.transform.position = position;
            main.transform.localScale = new Vector3(range, range, range);
            if (main.oldsetNull) {
                main.oldsetNull = false;
                main.OldUpdateTime = Time.unscaledTime;
            }
        }
    }
    static public void SetTransformNull() {
        if (main)
        {
            main.transform.position = new Vector3(29f, -600, 29f);
            main.transform.localScale = new Vector3(0, 0, 0);
            main.oldsetNull = true;
            main.OldUpdateTime = Time.unscaledTime;
        }
    }

    float OldUpdateTime = 0;
    Vector2 SelectPosOld = new Vector2();
    void TestCTRL() {
        TestPos();
        TestColor();


        void TestPos() {
            if (Player.me)
            {
                if (Player.me.controlsMouse.SelectCellCTRL && GameplayCTRL.main && GameplayCTRL.main.cellsData != null &&
                    GameplayCTRL.main.buildings[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY] != null)
                {
                    //Если тип строения туррель
                    if (GameplayCTRL.main.cellsData[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].build == Building.getTypeName(Building.Type.PillBox) ||
                        GameplayCTRL.main.cellsData[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].build == Building.getTypeName(Building.Type.Turret) ||
                        GameplayCTRL.main.cellsData[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].build == Building.getTypeName(Building.Type.Artillery) ||
                        GameplayCTRL.main.cellsData[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].build == Building.getTypeName(Building.Type.Minigun) ||
                        GameplayCTRL.main.cellsData[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].build == Building.getTypeName(Building.Type.Laser) ||
                        GameplayCTRL.main.cellsData[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].build == Building.getTypeName(Building.Type.Thunder) ||
                        GameplayCTRL.main.cellsData[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].build == Building.getTypeName(Building.Type.Rocket))
                    {
                        if (SelectPosOld.x != Player.me.controlsMouse.SelectCellCTRL.posX || SelectPosOld.y != Player.me.controlsMouse.SelectCellCTRL.posY)
                        {
                            SelectPosOld = new Vector2(Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY);
                            ReTime();
                        }
                        SetTransform(GameplayCTRL.main.buildings[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].GetTurretPos(), GameplayCTRL.main.buildings[Player.me.controlsMouse.SelectCellCTRL.posX, Player.me.controlsMouse.SelectCellCTRL.posY].GetFireDistance());
                    }
                    else
                    {
                        SetTransformNull();
                    }
                }
                else
                {
                    SetTransformNull();
                }
            }
            else
            {
                SetTransformNull();
            }
        }
        void TestColor() {
            if (MainCamera.main && meshRenderer && transform.localScale.x > 0) {
                float distToCam = Vector3.Distance(MainCamera.main.transform.position, transform.position);

                //Коофицент времени
                float timecoof = (Time.unscaledTime - OldUpdateTime - 5f) / 2f;
                if (timecoof > 1)
                    timecoof = 1;
                else if (timecoof < 0)
                    timecoof = 0;

                timecoof = 1 - timecoof;

                //Каков процент от маштаба визуализатора?
                float percent = distToCam / transform.localScale.x;

                Material material = meshRenderer.material;

                Color colorNew = material.color;
                if (percent > 1.75f) {
                    colorNew.a = 0.25f * timecoof;
                }
                else if (percent > 0.75f) {
                    colorNew.a = (percent - 0.75f) * 0.25f * timecoof;
                }
                else {
                    colorNew.a = 0;
                }

                material.color = colorNew;
                meshRenderer.material = material;
            }
        }

    }

    static public void ReTime() {
        if (main) {
            main.OldUpdateTime = Time.unscaledTime;
        }
    }
}
