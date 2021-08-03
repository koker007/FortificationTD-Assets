using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoGTRL : MonoBehaviour
{
    [SerializeField]
    Image Fon;
    [SerializeField]
    RawImage Logo;
    [SerializeField]
    Text Text;

    [SerializeField]
    Texture[] Logos;
    [SerializeField]
    string[] Names;
    [SerializeField]
    Color[] Fons;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TestWorking();
        TestEnd();
    }

    float startTime = 0;
    [SerializeField]
    float endTime = 4;

    [SerializeField]
    RectTransform posCenter;
    [SerializeField]
    RectTransform posCenterText;

    float zaderzka = 0.25f;
    void TestWorking()
    {
        zaderzka -= Time.deltaTime;
        if (startTime == 0 && zaderzka < 0)
        {
            startTime = Time.unscaledTime;
            endTime += startTime;
        }
        else if (zaderzka > 0)
        {
            RectTransform rect = Logo.GetComponent<RectTransform>();
            rect.transform.position = new Vector3(3000, posCenter.position.y, posCenter.position.z);

            RectTransform rectText = Text.GetComponent<RectTransform>();
            rectText.transform.position = new Vector3(3000, rectText.position.y, rectText.position.z);
        }
        else
        {

            //считаем общее время
            float allTime = endTime - startTime;
            //Узнаем текущий прогрес
            float progressNow = (Time.unscaledTime - startTime) / allTime;
            //узнаем прогресс одного показа
            float progressOne = (float)(1.0f / Logos.Length);

            //номер текущего лого
            int numLogoNow = (int)(progressNow / progressOne);
            //прогресс текущего лого
            float progLogoNow = (progressNow % progressOne) / progressOne;

            Debug.Log(numLogoNow + " " + progLogoNow);

            if (Logo && numLogoNow < Logos.Length)
            {
                //Ставим картинку лого который надо
                Logo.texture = Logos[numLogoNow];
                Text.text = Names[numLogoNow];

                RectTransform rect = Logo.GetComponent<RectTransform>();
                RectTransform rectText = Text.GetComponent<RectTransform>();

                if (rect)
                {
                    Vector3 posNew = rect.transform.position;
                    Vector3 posNewText = rectText.transform.position;

                    if (progLogoNow < 0.1f)
                    {
                        float coof = (progLogoNow / 0.1f);
                        posNew.x = (1 - coof) * 1000 + posCenter.position.x;
                        posNewText.x = (1 - coof) * -1000 + posCenterText.position.x;
                        
                    }
                    else if (progLogoNow < 0.9f)
                    {
                        posNew.x = posCenter.position.x;
                        posNewText.x = posCenterText.position.x;
                    }
                    else if (progLogoNow < 1f)
                    {
                        float coof = ((progLogoNow - 0.9f) / 0.1f);
                        posNew.x = (coof * -1000) + posCenter.position.x;
                        posNewText.x = (coof * 1000) + posCenterText.position.x;
                    }
                    rect.transform.position = posNew;
                    rectText.transform.position = posNewText;
                }
            }
        }
    }

    AsyncOperation asyncScene;
    void TestEnd() {
        if (startTime != 0 && zaderzka <= 0 && endTime < Time.unscaledTime) {
            Debug.Log("EndLogoTime");
            SceneManager.LoadScene(1);
        }
    }
}
