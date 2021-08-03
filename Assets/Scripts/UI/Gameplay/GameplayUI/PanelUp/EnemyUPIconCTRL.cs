using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUPIconCTRL : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TestTime();
    }

    public GameplayCTRL gameplayCTRL;

    [SerializeField]
    AICTRL.TypeEnemy typeEnemy = AICTRL.TypeEnemy.Pehota;

    [SerializeField]
    Image mainFon;
    [SerializeField]
    Image obvodka;
    [SerializeField]
    Text timeText;

    public void setObvodkaVisual(bool enable) {
        if (obvodka != null && mainFon != null) {
            mainFon.gameObject.active = enable;

            if (gameplayCTRL != null) {
                if (typeEnemy == AICTRL.TypeEnemy.Pehota)
                    obvodka.fillAmount = 1 - (gameplayCTRL.timeEpicSpawnInfantry / 10);
                else if (typeEnemy == AICTRL.TypeEnemy.Car)
                    obvodka.fillAmount = 1 - (gameplayCTRL.timeEpicSpawnAutomobile / 10);
                else if (typeEnemy == AICTRL.TypeEnemy.Crab)
                    obvodka.fillAmount = 1 - (gameplayCTRL.timeEpicSpawnCrab / 10);
            }
        }
    }
    void TestTime() {
        if (timeText != null && gameplayCTRL != null && mainFon != null) {
            timeText.gameObject.active = !mainFon.gameObject.active;

            if (timeText.gameObject.active) {
                if (typeEnemy == AICTRL.TypeEnemy.Pehota)
                    timeText.text = System.Convert.ToString((int)gameplayCTRL.timeEpicSpawnInfantry);
                else if (typeEnemy == AICTRL.TypeEnemy.Car)
                    timeText.text = System.Convert.ToString((int)gameplayCTRL.timeEpicSpawnAutomobile);
                else if (typeEnemy == AICTRL.TypeEnemy.Crab)
                    timeText.text = System.Convert.ToString((int)gameplayCTRL.timeEpicSpawnCrab);
            }
        }
    }
}
