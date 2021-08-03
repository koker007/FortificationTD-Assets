using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicators : MonoBehaviour
{
    static public EnemyIndicators main;

    [SerializeField]
    public GameObject PrefabAIHealth;

    public AIHealthSlider[] aIHealthSliders = new AIHealthSlider[50000];

    static public void CreateIndicator(AICTRL aICTRL) {
        if (main && !main.aIHealthSliders[aICTRL.ID]) {
            main.aIHealthSliders[aICTRL.ID] = Instantiate(main.PrefabAIHealth).GetComponent<AIHealthSlider>();
            main.aIHealthSliders[aICTRL.ID].iniAIHealth(aICTRL);
            main.aIHealthSliders[aICTRL.ID].transform.parent = main.transform;
        }
    }

    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
