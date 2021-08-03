using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonCTRL : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonSounds.Down();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonSounds.Up();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ButtonSounds.Select();
    }
}
