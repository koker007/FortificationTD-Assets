using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;

public class StringKeyFileLoader
{

    public Font font = null;
   
    //Хранит связь ключа и текста
    public struct KeyAndText {
        public string Key;
        public string Text;
    }

    //Количество ключей начинающихся на один символ
    static public int MaximumKeyOneSumbol = 250;

    //Хранит все ключи и текст
    public KeyAndText[] KaT = new KeyAndText[char.MaxValue * MaximumKeyOneSumbol];
    public int KaTCount = 0;

    //Главный ключ для поиска в тексте
    string main_key = ":=";
    //Проверка текста на ключ и запомнить
    void test_set_text_to_key(string new_str)
    {
        bool main_key_found_yn = false;
        int num_simbol_start_main_key = 0;
        int num_simbol_end_main_key = 0;

        //Ищем в тексте главный ключ
        int num_key_time = 0;
        for (int num_sumbol_now = 0; num_sumbol_now < new_str.Length && !main_key_found_yn; num_sumbol_now++)
        {
            KaTCount++;
            //проверяем символ с символом ключа
            if (new_str[num_sumbol_now] == main_key[num_key_time])
            {
                //совпадение!
                //переключаемся на следующий символ
                num_key_time++;

                //Если это первый символ ключа
                if (num_key_time == 1)
                {
                    //запоминаем позицию старта
                    num_simbol_start_main_key = num_sumbol_now;
                }
                //если это последний символ ключа
                if (num_key_time == main_key.Length)
                {
                    //Говорим что ключ был найден;
                    main_key_found_yn = true;
                    //Запоминаем позицию конца
                    num_simbol_end_main_key = num_sumbol_now + 1;
                }
            }
            else
            {
                //Сбрасываем счетчик совпадений ключа
                num_key_time = 0;
            }
        }

        //Если ключ был найден
        if (main_key_found_yn)
        {
            //Разделяем ключ и значение
            string text_key = "";
            string text_value = "";

            //заполняем ключ
            for (int num = 0; num < num_simbol_start_main_key; num++)
            {
                text_key = text_key + new_str[num];
            }
            //заполняем ключ
            for (int num = num_simbol_end_main_key; num < new_str.Length; num++)
            {
                text_value = text_value + new_str[num];
            }

            //Запоминаем
            int startPos = (int)text_key[0] * MaximumKeyOneSumbol;
            for (int num = startPos; num < startPos + MaximumKeyOneSumbol * 4 && num < KaT.Length; num++) {
                //Если позиция свободна
                if (KaT[num].Text == null || KaT[num].Text == "") {
                    KaT[num].Key = text_key;
                    KaT[num].Text = text_value;
                    break;
                }
            }

        }
    }

    //Получить текст по ключу
    public string get_text_from_key(string text_key) {
        string text_value = "";

        if (text_key.Length > 0) {
            //стартовая позиция поиска
            int startPos = (int)text_key[0] * MaximumKeyOneSumbol;

            //Если номер не превышает поисковый максимум и общий диапазон
            for (int num = startPos; num < startPos + MaximumKeyOneSumbol * 4 && num < KaT.Length; num++)
            {
                //Если ключ совпадает и текст есть
                if (KaT[num].Key == text_key && KaT[num].Text != null)
                {
                    text_value = KaT[num].Text;
                    break;
                }
            }
        }

        return text_value;
    }

    //получить текст из файла
    public void GetLanguage(string folder) {
        //создаем путь к файлу
        string path = folder + "/text.txt";

        if (File.Exists(path))
        {
            //string[] fileText = File.ReadAllLines(path, System.Text.Encoding.GetEncoding(1251));
            string encodeText = "Test Тест テスト 測試 تست mitä";
            string[] fileText = File.ReadAllLines(path, System.Text.Encoding.GetEncoding(1201));

            //Получили строки файла, теперь перебираем и ищем ключи
            foreach (string textFull in fileText) {
                test_set_text_to_key(textFull);
            }

            //Теперь надо получить файл шрифта
            string pathFont = folder + "/font.ttf";
            if (File.Exists(pathFont)) {
                
                font = (Font)Resources.Load(pathFont);
            }
        }
        else {
            Debug.LogError("File " + path + " Not Found");
        }

    }
}
