using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsCtrl : MonoBehaviour
{
    [SerializeField]
    Text[] textUp;
    [SerializeField]
    Text[] textDown;

    [SerializeField]
    string[] thanksList;


    [SerializeField]
    string developerTextKey;
    string developerText = "A game by";
    
    [SerializeField]
    string artistTextKey;
    string artistText = "Artist";

    [SerializeField]
    string translatedPersonNameKey;
    string translatedPerson = "";

    [SerializeField]
    string translaterTextKey;
    string translaterText = "Translated by";
    
    [SerializeField]
    string thanksTextKey;
    string thanksText = "Thanks";


    // Start is called before the first frame update
    void Start()
    {
        main = this;
        Invoke("GetTexts", 0.1f);
    }

    static public CreditsCtrl main;
    // Update is called once per frame
    void Update()
    {
        TestCredits();
    }


    public bool playTitle = false;

    void GetTexts() {
        if (Setings.main != null && Setings.main.game != null && Setings.main.LangugeText != null && Setings.main.LangugeText.KaTCount > 0) {
            string value;
            value = Setings.main.LangugeText.get_text_from_key(developerTextKey);
            if (value != "") {
                developerText = value;
            }

            value = Setings.main.LangugeText.get_text_from_key(artistTextKey);
            if (value != "")
            {
                artistText = value;
            }

            value = Setings.main.LangugeText.get_text_from_key(translaterTextKey);
            if (value != "")
            {
                translaterText = value;
            }
            value = Setings.main.LangugeText.get_text_from_key(translatedPersonNameKey);
            if (value != "")
            {
                translatedPerson = value;
            }

            value = Setings.main.LangugeText.get_text_from_key(thanksTextKey);
            if (value != "")
            {
                thanksText = value;
            }
        }
    }

    enum TypeView {
        start,
        developer,
        artist,
        translater,
        thanks,
        end
    }

    int thanksNum = 0;
    TypeView typeView = TypeView.start;

    float timeView = 0;
    TimeText timeTextUP;
    TimeText timeTextDown;

    void TestCredits() {
        if (textUp.Length > 0 && textDown.Length > 0) {
            timeView += Time.deltaTime;
            if (timeView > 10 && typeView != TypeView.end && playTitle) {
                NextView();
            }
            VisualizeText();

            void NextView() {
                timeView = 0;
                //4
                if (typeView == TypeView.artist)
                {
                    typeView = TypeView.developer;
                    timeTextUP.Set(0, 0.05f, developerText, false, false);
                    timeTextDown.Set(2f, 0.05f, "Koker 007", false, false);
                }
                //5
                else if (typeView == TypeView.developer)
                {
                    typeView = TypeView.end;
                    timeView = 10;
                }
                //1
                else if (typeView == TypeView.start)
                {
                        typeView = TypeView.thanks;
                        timeTextUP.Set(0, 0.05f, thanksText, false, false);
                        thanksNum = 0;
                        timeTextDown.Set(2f, 0.05f, thanksList[thanksNum], false, false);
                }
                //3
                else if (typeView == TypeView.translater)
                {
                    typeView = TypeView.artist;
                    timeTextUP.Set(0, 0.05f, artistText, false, false);
                    timeTextDown.Set(2f, 0.05f, "WLF ULM", false, false);
                }

                //2
                else if (typeView == TypeView.thanks)
                {
                    thanksNum++;
                    if (thanksNum >= thanksList.Length)
                    {
                        if (translatedPerson != "")
                        {
                            typeView = TypeView.translater;
                            timeTextUP.Set(0, 0.05f, translaterText, false, false);
                            timeTextDown.Set(2f, 0.05f, translatedPerson, false, false);
                        }
                        else
                        {
                            typeView = TypeView.artist;
                            timeTextUP.Set(0, 0.05f, artistText, false, false);
                            timeTextDown.Set(2f, 0.05f, "WLF ULM", false, false);
                        }
                    }
                    else {
                        timeTextUP.Set(0, 0.05f, thanksText, false, false);
                        timeTextDown.Set(2f, 0.05f, thanksList[thanksNum], false, false);
                    }
                }
            }
            void VisualizeText() {
                float alpha = 1;

                if (timeView > 7) {
                    alpha -= (1 - (10 - timeView)/3);
                }

                foreach (Text text in textUp) {
                    text.text = timeTextUP.Get();
                    Color color = text.color;
                    color.a = alpha;
                    text.color = color;
                }
                foreach (Text text in textDown) {
                    text.text = timeTextDown.Get();
                    Color color = text.color;
                    color.a = alpha;
                    text.color = color;
                }
            }
        }
    }
}
