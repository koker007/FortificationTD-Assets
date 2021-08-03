using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoTextSeter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    string KeyInfoText = "";
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InfoTextCTRL.main) {
            InfoTextCTRL.main.textKeyNow = KeyInfoText;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InfoTextCTRL.main && InfoTextCTRL.main.textKeyNow == KeyInfoText)
        {
            InfoTextCTRL.main.textKeyNow = "";
        }
    }
}
