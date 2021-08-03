using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VisualConnectionIndicator();
        testPortInfo();
    }

    [SerializeField]
    ConnectionIndicator[] connectionIndicators;

    float VisualTime = 0;
    void VisualConnectionIndicator()
    {
        if (Time.unscaledTime - LastViewConnect > 2)
        {
            LastViewConnect = Time.unscaledTime;
            lastinfotest = 0;
        }
        else {
            LastViewConnect = Time.unscaledTime;
        }

        VisualTime += Time.deltaTime;
        float sizeNeed = 2;
        for (int num = 0; num < connectionIndicators.Length; num++)
        {
            if (connectionIndicators[num] != null && VisualTime > num * 0.5f && VisualTime < (num * 0.5f + 0.5f) && connectionIndicators[num].sizeNeed <= sizeNeed)
            {
                connectionIndicators[num].sizeNeed = sizeNeed;
            }
        }

        if (VisualTime > 2.5f)
            VisualTime = 0;
    }

    float lastinfotest = -100;
    float LastViewConnect = -100;
    [SerializeField]
    GameObject portInfoText;

    void testPortInfo() {
        lastinfotest += Time.unscaledDeltaTime;

        if (lastinfotest > 5)
        {
            if (!portInfoText.activeSelf)
            {

                portInfoText.SetActive(true);
            }
        }
        else {
            if (portInfoText.activeSelf)
            {
                portInfoText.SetActive(false);
            }
        }
    }
}
