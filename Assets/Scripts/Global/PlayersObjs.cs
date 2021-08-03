using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Скрипт метка чтобы игроки могли сгрупироваться
public class PlayersObjs : MonoBehaviour
{
    static public PlayersObjs main;
    private void Start()
    {
        main = this;
    }
}
