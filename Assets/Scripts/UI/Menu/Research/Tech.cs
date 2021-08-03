using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tech: MonoBehaviour
{
    [SerializeField]
    string techSTR;
    [SerializeField]
    string nameKey;
    TimeText nameTime;
    [SerializeField]
    string infoKey;
    TimeText infoTime;

    [SerializeField]
    int cost = 0;


    [SerializeField]
    Image imageIcon;
    [SerializeField]
    Text nameText;
    [SerializeField]
    Text infoText;

    [SerializeField]
    GameObject blockObj;
    [SerializeField]
    Text costText;
    [SerializeField]
    Text costText2;

    [SerializeField]
    colors colorTech;

    enum colors {
        basic,
        good,
        rare
    }
    

    // Start is called before the first frame update
    void Start()
    {
        iniText();
        iniColor();
    }

    // Update is called once per frame
    void Update()
    {
        testText();
        testBlockImage();
    }

    void iniText() {
        if (Setings.main) {
            nameTime.Set(0, 0.1f, Setings.main.LangugeText.get_text_from_key(nameKey), false, true);
            infoTime.Set(0.5f, 0.1f, Setings.main.LangugeText.get_text_from_key(infoKey), false, true);
        }
    }

    void iniColor() {
        Image image = GetComponent<Image>();
        if (colorTech != colors.basic && image) {
            if (colorTech == colors.good) {
                image.color = new Color(1, 0.75f, 0.50f);
            }
            else if (colorTech == colors.rare) {
                image.color = new Color(0.77f, 0.47f, 1);
            }
        }
    }

    void testText() {
        if (nameText && infoText && Setings.main) {
            nameText.text = nameTime.Get();
            infoText.text = infoTime.Get();
        }
    }

    public void buttonReserchTech() {
        if (techSTR != "" && !TechIsBlock()) {
            GPResearch.main.StartResearchTech(techSTR, cost);
        }
    }

    bool TechIsBlock() {
        bool isBlock = true;
        if (GPResearch.main != null && cost < GPResearch.main.stats.neirodata) {
            isBlock = false;
        }
        return isBlock;
    }
    void testBlockImage() {
        if (blockObj && costText && costText2) {
            bool isBlock = TechIsBlock();
            if (isBlock && !blockObj.active) {
                blockObj.active = true;
            }
            else if (!isBlock && blockObj.active) {
                blockObj.active = false;
            }

            costText.text = System.Convert.ToString(cost);
            costText2.text = System.Convert.ToString(cost);
        }
    }


    static public int getMaxCountTechThisGameplayTime() {
        int count = 0;

        if (GameplayCTRL.main != null) {
            int gameplayLVL = (int)(GameplayCTRL.main.timeGamePlay / (60*10)); //уровень каждые 10 минут
            count += gameplayLVL * 2; //Каждый уровень по 2 улучщения
        }

        return count;
    }
}
