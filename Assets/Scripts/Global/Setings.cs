using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

using System;
using System.IO;

public class Setings : MonoBehaviour
{

    public static Setings main;
    Setings() {
        main = this;
    }

    //Хранит текстовую информацию
    public StringKeyFileLoader LangugeText;
    void GetLanguage()
    {
        //Если язык не английский то пытаемся поставить
        if (game != null /*&& game.Language != "English"*/)
        {
            string FolderPath = Application.dataPath + "/Language/" + game.Language;
            LangugeText.GetLanguage(FolderPath);
        }
        Debug.Log("Language " + game.Language);
    }

    PostProcessVolume postProcess;
    void iniPostProcess()
    {
        if (postProcess == null)
        {
            GameObject cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraObj != null)
            {
                postProcess = cameraObj.GetComponent<PostProcessVolume>();
            }
        }
    }

    class stack_string
    {
        public string text;
        public stack_string stack_next;

        public stack_string(string new_text)
        {
            text = new_text;
        }

        public void set_new_stack_text(string new_text)
        {
            stack_string now = this;

            bool end_stack_yn = false;
            while (!end_stack_yn)
            {
                //Проверяем есть ли следующее звено
                if (now.stack_next != null)
                {
                    now = now.stack_next;
                }
                //Если звена нет то создаем новое и записываем текст
                else
                {
                    now.stack_next = new stack_string(new_text);
                    end_stack_yn = true;
                }
            }
        }

        //Получить количество звеньев
        public int get_stack_max()
        {
            stack_string now = this;
            int return_int = 1;

            //цикл пока не достигнем последнего звена
            bool end_stack_yn = false;
            while (!end_stack_yn)
            {
                //Проверяем есть ли следующее звено
                if (now.stack_next != null)
                {
                    now = now.stack_next;
                    return_int++;

                }
                else
                {
                    end_stack_yn = true;
                }
            }

            return return_int;
        }
    }

    public class Game_setings
    {

        string path_setings = Application.dataPath + "/setings.txt";
        string[] setings_str;

        //Главный ключ для поиска в тексте
        const string main_key = ":=";

        //Говорит о том есть ли новые настройки?
        public bool new_data_setings_yn = true;

        const string Language_key = "Language";
        public string Language = "English";

        const string playerColorKey = "PlayerColor";
        public float playerColor = 0;

        const string volume_all_key = "volume_all";
        public float volume_all = 0.5f;
        const string volume_music_key = "volume_music";
        public float volume_music = 1f;
        const string volume_sound_key = "volume_sound";
        public float volume_sound = 1f;

        //Параметры сервера
        const string serverNameKey = "server_name";
        public string serverName = "";
        const string serverPasswordKey = "server_password";
        public string serverPassword = "";
        const string serverPlayersMaxKey = "server_players_max";
        public int serverPlayersMax = 6;

        const string grafic_lvl_key = "grafic_lvl";
        public int grafic_lvl = 0;

        const string shader_key = "shader";
        public bool shader = true;

        const string speedMouseKey = "speed_controller";
        public float speedMouse = 2f;

        const string ConnectIpKey = "ConnectIP";
        public string ConnectIp = "";

        const string ResearchTechKey = "ResearchTech";
        public string ResearchTech = "";

        //вынести данные из файла в найстройки
        void get_setings_from_file()
        {
            //пытаемся получить файл
            setings_str = File.ReadAllLines(path_setings);
            //Если не пустое то вытаскиваем значения
            if (setings_str != null && setings_str.Length > 0)
            {
                foreach (string text in setings_str)
                    test_text_to_key(text);

            }
            //зануляем значения
            setings_str = null;

        }
        public void set_setings_to_file()
        {
            //Занулили старые данные
            if (setings_str != null)
                setings_str = null;

            stack_string stack_start = new stack_string("=====Main setings=====");

            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text(Language_key + main_key + Language);

            //Цвет игрока
            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text(playerColorKey + main_key + playerColor);


            //Заполняем новыми данными
            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text("          Volume");
            //громкость обшая
            stack_start.set_new_stack_text(volume_all_key + main_key + volume_all);
            //громкость музыки
            stack_start.set_new_stack_text(volume_music_key + main_key + volume_music);
            //громкость звуков
            stack_start.set_new_stack_text(volume_sound_key + main_key + volume_sound);

            //Заполняем новыми данными
            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text("          Video");
            stack_start.set_new_stack_text(grafic_lvl_key + main_key + grafic_lvl);
            //Включен ли шейдер
            stack_start.set_new_stack_text(shader_key + main_key + shader);


            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text("          Controller");
            //Скороcть мыши или контроллера
            stack_start.set_new_stack_text(speedMouseKey + main_key + speedMouse);

            //Заполняем новыми данными
            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text("          Server");
            stack_start.set_new_stack_text(serverNameKey + main_key + serverName);
            stack_start.set_new_stack_text(serverPasswordKey + main_key + serverPassword);
            stack_start.set_new_stack_text(serverPlayersMaxKey + main_key + serverPlayersMax);

            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text("          Connect");
            //IP подключения
            stack_start.set_new_stack_text(ConnectIpKey + main_key + ConnectIp);

            stack_start.set_new_stack_text(" ");
            stack_start.set_new_stack_text("          Gameplay");
            stack_start.set_new_stack_text(ResearchTechKey + main_key + ResearchTech);

            //стек готов - узнаем число звеньев
            int stack_lenght = stack_start.get_stack_max();
            setings_str = new string[stack_lenght];
            //перебираем все звенья
            stack_string now_stack = stack_start;
            for (int num_now = 0; num_now < stack_lenght; num_now++)
            {
                //Записываем
                setings_str[num_now] = now_stack.text;
                //переключаем
                now_stack = now_stack.stack_next;
            }

            //Запись в файл того что есть
            File.WriteAllLines(path_setings, setings_str);



        }

        public void Reloading()
        {
            set_setings_to_file();
            get_setings_from_file();
        }

        //Проверка текста на ключ и значение
        void test_text_to_key(string new_str)
        {
            bool main_key_found_yn = false;
            int num_simbol_start_main_key = 0;
            int num_simbol_end_main_key = 0;

            //Ищем в тексте главный ключ
            int num_key_time = 0;
            for (int num_sumbol_now = 0; num_sumbol_now < new_str.Length && !main_key_found_yn; num_sumbol_now++)
            {
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

                //Обшая громкость
                if (volume_all_key == text_key)
                {
                    volume_all = (float)Convert.ToDouble(text_value);
                }
                //Громкость музыки
                else if (volume_music_key == text_key)
                {
                    volume_music = (float)Convert.ToDouble(text_value);
                }
                //Громкость звуков
                else if (volume_sound_key == text_key)
                {
                    volume_sound = (float)Convert.ToDouble(text_value);
                }
                //уровень графики
                else if (grafic_lvl_key == text_key)
                {
                    grafic_lvl = Convert.ToInt32(text_value);
                }
                //шейдер
                else if (shader_key == text_key)
                {
                    shader = Convert.ToBoolean(text_value);
                }
                //Скорость мыши
                else if (speedMouseKey == text_key)
                {
                    speedMouse = (float)Convert.ToDouble(text_value);
                }

                //язык
                else if (Language_key == text_key)
                {
                    Language = text_value;
                }

                //сервер
                else if (serverNameKey == text_key) serverName = text_value;
                else if (serverPasswordKey == text_key) serverPassword = text_value;
                else if (serverPlayersMaxKey == text_key) serverPlayersMax = Convert.ToInt32(text_value);

                //IP последний введенный
                else if (ConnectIpKey == text_key)
                {
                    ConnectIp = text_value;
                }
                //Цвет игрока
                else if (playerColorKey == text_key) {
                    playerColor = (float)Convert.ToDouble(text_value);
                }
                //Изучаемая технология
                else if (ResearchTechKey == text_key) {
                    ResearchTech = text_value;
                }

            }
        }

        void test_new_setings()
        {

        }

        //Конструктор пытается вытащить данные из файла если нету, то создает файл
        public Game_setings()
        {
            get_setings_from_file();
        }
        ~Game_setings()
        {
            set_setings_to_file();
        }
    }

    public Game_setings game;

    // Use this for initialization
    void Start()
    {
        game = new Game_setings();

        LangugeText = new StringKeyFileLoader();
        GetLanguage();

        iniPostProcess();
    }

    // Update is called once per frame

    public float timeToSave = 0;
    void testAutoSaveParam()
    {
        timeToSave -= Time.deltaTime;
        if (timeToSave <= 0)
        {
            timeToSave = 60;

            if (game != null)
            {
                Debug.Log("AutoSAveParam");
                game.set_setings_to_file();
            }
        }
    }

    void Update()
    {
        testAutoSaveParam();

        setSetings();

    }


    void setSetings() {
        if (game != null) {
            if (game.grafic_lvl < 3) {
                QualitySettings.SetQualityLevel(game.grafic_lvl);
            }

            if (postProcess != null) {
                postProcess.enabled = game.shader;


            }
        }
    }



}
