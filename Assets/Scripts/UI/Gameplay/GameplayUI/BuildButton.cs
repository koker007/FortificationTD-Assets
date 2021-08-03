using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    public RawImage rawImagePresent;

    [SerializeField]
    public GameObject PrefabCamPresent;
    public Camera cameraComponet;
    public RenderTexture renderTexture;
    KeyCode keyPress = KeyCode.None;
    public void SetKeyPress(int numKey) {
        if (numKey == 1) keyPress = KeyCode.Alpha1;
        else if (numKey == 2) keyPress = KeyCode.Alpha2;
        else if (numKey == 3) keyPress = KeyCode.Alpha3;
        else if (numKey == 4) keyPress = KeyCode.Alpha4;
        else if (numKey == 5) keyPress = KeyCode.Alpha5;
        else if (numKey == 6) keyPress = KeyCode.Alpha6;
        else if (numKey == 7) keyPress = KeyCode.Alpha7;
        else if (numKey == 8) keyPress = KeyCode.Alpha8;
        else if (numKey == 9) keyPress = KeyCode.Alpha9;
    }

    [SerializeField]
    public Text NameText;
    [SerializeField]
    public Text PriceText;
    [SerializeField]
    public Image Lock;
    [SerializeField]
    public Image CoolDown;

    public enum Type {
        none,
        building,
        upgrade
    }
    public Type type;

    [SerializeField]
    public GameObject buildPresent;
    [SerializeField]
    public Building building;

    InfoTextCTRL infoTextCTRL;
    public string KeyText = "";

    //Для вывода информации при наведении мыши
    public void OnPointerEnter(PointerEventData eventData) {
        if (infoTextCTRL == null) {
            GameObject infoTextObj = GameObject.FindGameObjectWithTag("InfoText");
            if (infoTextObj != null) {
                infoTextCTRL = infoTextObj.GetComponent<InfoTextCTRL>();
            }
        }
        
        if (infoTextCTRL != null) {
            infoTextCTRL.textKeyNow = KeyText;
        }
    }

    private void Update()
    {
        TestKeyBoardPress();
    }

    void TestKeyBoardPress() {
        if (Input.GetKeyDown(keyPress)) {
            //Вытаскиваем компонент кнопки
            Button button = gameObject.GetComponent<Button>();
            if (button && button.onClick != null) {
                button.onClick.Invoke();
            }
        }
    }
}
