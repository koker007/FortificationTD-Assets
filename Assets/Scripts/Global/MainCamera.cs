using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    static public MainCamera main;
    static public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        mainCamera = gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        TestChainsVelosity();
    }

    public struct TimeFloat
    {
        public float time;
        public float value;
    }

    Chains<TimeFloat> chainsVelosity;
    public float nowVelosity = 0;
    Vector3 oldPos = new Vector3();
    void TestChainsVelosity() {
        if (GameplayCTRL.main != null) {
            if (chainsVelosity == null)
            {
                chainsVelosity = new Chains<TimeFloat>();


                TimeFloat timeFloat;
                timeFloat.time = Time.time;
                timeFloat.value = 0;

                chainsVelosity.AddChain(timeFloat);
            }
            else if (GameplayCTRL.main.gamemode == 2)
            {
                //Добавляем звено в цепь
                TimeFloat newdata = new TimeFloat();
                newdata.time = Time.time;
                newdata.value = Vector3.Distance(oldPos, transform.position);
                chainsVelosity.AddChain(newdata);
                oldPos = transform.position;

                //Начинаем перебор цепи
                int chainCount = 0;
                float needVelosity = 0;
                float oldChainTime = chainsVelosity.start.data.time;

                Chains<TimeFloat>.Chain<TimeFloat> next;
                next = chainsVelosity.start;
                bool firstFound = false;
                while (next != null)
                {
                    //Проверяем время в цепи
                    if (Time.time - 1 < next.data.time)
                    {
                        //Передвигаем цепь на следующее звено если надо
                        if (!firstFound)
                        {
                            firstFound = true;
                            chainsVelosity.start = next;
                        }

                        //считаем разницу во времени относительно предыдущего кадра
                        float timeShag = next.data.time - oldChainTime;
                        

                        chainCount++;
                        needVelosity += next.data.value * timeShag;
                    }

                    //Переключаемся на следующее звено
                    if (next.next != null)
                    {
                        next = next.next;
                    }
                    else
                    {
                        next = null;
                    }
                }

                //Перебор звеньев закончен считаем деньги
                if(chainCount > 0)
                    needVelosity = needVelosity/chainCount;

                //Подгоняем значения
                nowVelosity += (needVelosity - nowVelosity) * 0.6f;
                //Debug.Log("CamVelosity " + nowVelosity + " chainCount " + chainCount);
            }
            else {
                //Подгоняем значения
                nowVelosity += (0 - nowVelosity) * Time.unscaledDeltaTime;
            }
        }
    }
}
