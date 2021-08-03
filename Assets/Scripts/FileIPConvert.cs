using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

public class FileIPConvert : MonoBehaviour
{
    [SerializeField]
    bool convertNeed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ConvertorFileIp();
    }

    void ConvertorFileIp() {
        if (convertNeed) {

            //Сперва получаем файл с Ip
            string pathIn = Application.dataPath + "/textIP.txt";
            string[] fileText = File.ReadAllLines(pathIn, System.Text.Encoding.GetEncoding(1201));

            string[] fileTextNew = new string[fileText.Length];

            //Перебираем текст и фильтруем
            for (int x = 0; x < fileText.Length; x++) {

                //ищем точки
                int pointsFound = 0;
                bool tireFound = false;

                string lastNum = "";

                string TextNew = "";
                bool done = false;

                //Перебираем символы
                foreach (char s in fileText[x]) {
                    if (s == '.')
                    {
                        pointsFound++;
                        TextNew += lastNum;
                        TextNew += '.';
                    }
                    if (s == '-') {
                        tireFound = true;
                        TextNew += lastNum;
                        TextNew += '-';
                    }

                    if (s == ' ') {
                        TextNew += lastNum;
                    }


                    if (s == '0' || s == '1' || s == '2' || s == '3' || s == '4' || s == '5' || s == '6' || s == '7' || s == '8' || s == '9')
                    {
                        lastNum += s;
                    }
                    else {
                        //Проверка на завершенность
                        if (pointsFound == 6 && tireFound && s != '.')
                        {
                            TextNew += lastNum;
                            done = true;


                            break;
                        }

                        lastNum = "";
                    }
                }

                //Если готово, добавляем
                if (done) {
                    fileTextNew[x] = TextNew;
                }
            }

            string pathOut = Application.dataPath + "/textIPResult.txt";
            //нужно записать результат в файл
            File.WriteAllLines(pathOut, fileTextNew);

            convertNeed = false;
        }
    }
}
