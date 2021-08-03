using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLanguage : MonoBehaviour
{
    [SerializeField]
    bool NeedInvoke = false;

    [SerializeField]
    string TextKeyLanguage = "";

    Setings mainParam;
    [SerializeField]
    Text[] texts;

    [SerializeField]
    float zaderzka = 0.1f;
    [SerializeField]
    float speed = 0.1f;
    [SerializeField]
    bool numeric = false;
    [SerializeField]
    bool refzesh = true;
    TimeText timeText;

    void iniTextLanguage() {
        if (mainParam == null) {
            mainParam = GameObject.FindGameObjectWithTag("Setings").GetComponent<Setings>();
        }
        if (texts.Length == 0) {
            Text textParent = gameObject.GetComponent<Text>();
            if (textParent != null)
            {
                texts = new Text[1];
                texts[0] = textParent;
            }


            if (texts.Length == 0) {
                texts = gameObject.GetComponentsInChildren<Text>();
            }
        }

        if (mainParam != null && texts.Length > 0) {
            string textStr = "";

            if (TextKeyLanguage != "") {
                textStr = mainParam.LangugeText.get_text_from_key(TextKeyLanguage);
            }

            if (textStr == "")
                textStr = texts[0].text;

            timeText.Set(zaderzka, speed, textStr, numeric, refzesh);
        }

        if (texts.Length > 0) {
            int size = texts[0].fontSize;
            foreach (Text text in texts) {
                text.fontSize = size;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(NeedInvoke)
            Invoke("iniTextLanguage", 0.01f);
        else
            iniTextLanguage();
    }

    // Update is called once per frame
    void Update()
    {
        if (texts.Length > 0) {
            foreach (Text text in texts)
            {
                text.text = timeText.Get();
            }
        }
    }
}
