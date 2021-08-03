using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class OptionsCTRL : MonoBehaviour
{
    float OpenTimeSettings = 0;

    [SerializeField]
    Setings setings;
    void iniSettingsGame() {
        if (setings == null) {
            GameObject setObj = GameObject.FindGameObjectWithTag("Setings");
            if (setObj != null) {
                setings = setObj.GetComponent<Setings>();
            }
        }
    }

    [SerializeField]
    Dropdown dropdownSize;

    [SerializeField]
    Toggle toggleFullscreen;

    [SerializeField]
    Dropdown dropdownSetings;

    [SerializeField]
    Toggle toggleShader;

    // Start is called before the first frame update
    void Start()
    {
        OpenTimeSettings = Time.unscaledTime;

        iniSettingsGame();

        iniSetVizual();
        iniVideoSize();
        iniSetings();

        iniVolumeAll();
        iniVolumeMusic();
        iniVolumeSound();

        iniLanguage();

    }

    // Update is called once per frame
    void Update()
    {
        iniSettingsGame();
        TestInfoPanel();
    }

    public void iniSetVizual() {
        if (setings != null && setings.game != null) {
            if (toggleShader != null) {
                if (setings.game.shader) {
                    toggleShader.isOn = true;
                }
                else {
                    toggleShader.isOn = false;
                }
            }
        }
    }

    public void iniVideoSize() {
        if (dropdownSize) {
            //Узнаем текущее разрешение экрана
            int width = Screen.width;
            int height = Screen.height;

            string sizeNowStr = width + "x" + height;
            Debug.Log(sizeNowStr);

            //Ищем это разрешение в списке
            bool found = false;
            for (int num = 0; num < dropdownSize.options.Count; num++) {
                if (dropdownSize.options[num].text == sizeNowStr) {
                    dropdownSize.value = num;
                    found = true;
                    break;
                }
            }
            if (found) {
                Debug.Log("Size found");
            }
            else {
                Debug.Log("Size not found");
            }
        }
    }
    public void setVideoSize() {
        if (dropdownSize != null) {
            //получаем выбранный текст
            string textSize = dropdownSize.options[dropdownSize.value].text;

            string textWight = "";
            string textHeight = "";
            bool xFound = false;

            bool ok = true;

            //перебираем каждый символ
            foreach (char s in textSize) {
                //Если цифра
                if (s == '0' || s == '1' || s == '2' || s == '3' || s == '4' || s == '5' || s == '6' || s == '7' || s == '8' || s == '9')
                {
                    if (!xFound)
                    {
                        textWight += s;
                    }
                    else
                    {
                        textHeight += s;
                    }
                }
                else {
                    if (s == 'x') {
                        if (!xFound) {
                            xFound = true;
                        }
                        else
                        {
                            ok = false;
                            break;
                        }
                    }
                    else {
                        ok = false;
                        break;
                    }
                }
            }

            if (ok && xFound) {
                Screen.SetResolution(System.Convert.ToInt32(textWight), System.Convert.ToInt32(textHeight), Screen.fullScreenMode);
                Debug.Log("New screen size " + textWight + " " + textHeight);
            }
        }        
    }

    public void iniSetings() {
        if (dropdownSetings != null && setings != null && setings.game != null)
        {
            //Узнаем текущие настройки
            dropdownSetings.value = setings.game.grafic_lvl;
        }
    }
    public void setSetings()
    {
        if (dropdownSetings != null && setings != null && setings.game != null)
        {
            setings.game.grafic_lvl = dropdownSetings.value;
            setings.game.set_setings_to_file();

        }
    }
    public void clickFullscreen() {
        if (toggleFullscreen != null) {
            if (Screen.fullScreen)
            {
                Screen.SetResolution(Screen.width, Screen.height, false);
                toggleFullscreen.isOn = false;
                Debug.Log("Fullscreen " + false);
            }
            else
            {
                Screen.SetResolution(Screen.width, Screen.height, true);
                toggleFullscreen.isOn = true;
                Debug.Log("Fullscreen " + true);
            }
        }
    }

    public void clickShader() {
        if (toggleShader != null && setings != null && setings.game != null) {
            if (setings.game.shader)
            {
                toggleShader.isOn = false;

                setings.game.shader = false;

                setings.game.set_setings_to_file();
            }
            else {
                toggleShader.isOn = true;

                setings.game.shader = true;

                setings.game.set_setings_to_file();
            }
        }
    }

    void iniVolumeAll() {
        if (setings && setings.game != null && sliderVolumeAll) {
            sliderVolumeAll.value = setings.game.volume_all * sliderVolumeAll.maxValue;
        }
    }
    public void clickVolumeAll() {
        if (setings && setings.game != null && sliderVolumeAll) {
            float volume = sliderVolumeAll.value / sliderVolumeAll.maxValue;
            setings.game.volume_all = volume;
            setings.timeToSave = 0;
        }
    }
    [SerializeField]
    Slider sliderVolumeAll;

    void iniVolumeMusic()
    {
        if (setings && setings.game != null && sliderVolumeMusic)
        {
            sliderVolumeMusic.value = setings.game.volume_music * sliderVolumeMusic.maxValue;
        }
    }
    public void clickVolumeMusic()
    {
        if (setings && setings.game != null && sliderVolumeMusic)
        {
            float volume = sliderVolumeMusic.value/ sliderVolumeMusic.maxValue;
            setings.game.volume_music = volume;
            setings.timeToSave = 0;
        }
    }
    [SerializeField]
    Slider sliderVolumeMusic;

    void iniVolumeSound()
    {
        if (setings && setings.game != null && sliderVolumeSound)
        {
            sliderVolumeSound.value = setings.game.volume_sound * sliderVolumeSound.maxValue;
        }
    }
    public void clickVolumeSound()
    {
        if (setings && setings.game != null && sliderVolumeSound)
        {
            float volume = sliderVolumeSound.value / sliderVolumeSound.maxValue;
            setings.game.volume_sound = volume;
            setings.timeToSave = 0;
        }
    }
    [SerializeField]
    Slider sliderVolumeSound;

    [SerializeField]
    Dropdown dropdownLanguage;
    void iniLanguage() {
        if (dropdownLanguage && setings && setings.game != null) {
            //Получаем все папки в папке language
            string LanguagePath = Application.dataPath + "/Language/";

            //получаем списки языков
            DirectoryInfo folderInfo = new System.IO.DirectoryInfo(LanguagePath);
            DirectoryInfo[] languageFolders = folderInfo.GetDirectories();

            //Добавляем новые варианты выбора в дроп бокс, для всех найденных языков
            foreach (DirectoryInfo languageFolder in languageFolders) {
                //Перебираем все варианты дропбокса
                bool found = false;
                for (int num = 0; num < dropdownLanguage.options.Count; num++)
                {
                    //Если имя совпадает с именем папки то завершаем поиск
                    if (dropdownLanguage.options[num].text == languageFolder.Name)
                    {
                        found = true;
                        break;
                    }
                }

                //Если не нашлось создаем вариант выбора
                if (!found) {
                    Dropdown.OptionData valueNew = new Dropdown.OptionData();
                    valueNew.text = languageFolder.Name;

                    dropdownLanguage.options.Add(valueNew);
                }
            }

            //Перебираем варианты в поисках того что в настройках
            for (int num = 0; num < dropdownLanguage.options.Count; num++) {
                if (dropdownLanguage.options[num].text == setings.game.Language) {
                    dropdownLanguage.value = num;
                    break;
                }
            }

        }
    }
    public void setLanguage() {
        if (dropdownLanguage && setings && setings.game != null) {
            //Проверяем выбранный язык на то что его можно поставить
            string languageNew = dropdownLanguage.options[dropdownLanguage.value].text;


            DirectoryInfo directorySelectLanguage = new DirectoryInfo(Application.dataPath + "/Language/" + languageNew);
            FileInfo[] filesSelectLanguage = directorySelectLanguage.GetFiles();
            //Проверяем что файл с переводом существует в этой папке
            bool foundTranslate = false;
            foreach (FileInfo file in  filesSelectLanguage) {
                if (file.Name == "text.txt") {
                    foundTranslate = true;
                    break;
                }
            }

            if (foundTranslate) {
                setings.game.Language = dropdownLanguage.options[dropdownLanguage.value].text;
                setings.game.set_setings_to_file();

                if(Time.unscaledTime - OpenTimeSettings > 1)
                    changesAfterRestartGame = true;
            }
        }
    }

    [SerializeField]
    Button OptionsInfoPanel;

    [SerializeField]
    Text changesAfterRestartGameText;
    bool changesAfterRestartGame = false;

    void TestInfoPanel() {
        if (OptionsInfoPanel) {
            if (changesAfterRestartGame) {
                OptionsInfoPanel.interactable = true;
                changesAfterRestartGameText.gameObject.active = true;
            }
            else {
                OptionsInfoPanel.interactable = false;
                changesAfterRestartGameText.gameObject.active = false;
            }
        }
    }

    public void ClickButtonSettingsInfo() {
        if (changesAfterRestartGame && !GameplayCTRL.main) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
