using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionIndicator : MonoBehaviour
{
    
    public float sizeNeed = 1;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        TestSize();
    }

    void TestSize() {
        //Требование постепенно уменьшается
        if (sizeNeed >= 1)
        {
            sizeNeed -= Time.deltaTime * 4;
        }
        else {
            sizeNeed = 1;
        }

        //изменяем размер плавно
        float sizeNow = gameObject.transform.localScale.x;

        sizeNow += (sizeNeed - sizeNow)*Time.deltaTime * 2;

        gameObject.transform.localScale = new Vector3(sizeNow, sizeNow, sizeNow);

    }
    
}
