using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterierTV : MonoBehaviour
{

    [SerializeField]
    MeshRenderer MainMashRenderer;

    [SerializeField]
    Texture2D[] TVshow;
    [SerializeField]
    float showSpeed = 0.5f;

    [SerializeField]
    Texture2D[] TVDamage;

    [SerializeField]
    Light[] TVLight;


    // Update is called once per frame
    void Update()
    {
        TestTVshow();
    }

    void TestTVshow() {
        if (MainMashRenderer && TVshow.Length > 0) {
            float showTimeNow = Time.unscaledTime%(TVshow.Length*showSpeed);
            int showNum = (int)(showTimeNow / showSpeed);

            if (TVshow.Length > showNum && TVshow[showNum] != null) {
                Material material = MainMashRenderer.material;
                material.mainTexture = TVshow[showNum];
                material.color = new Color(1,1,1, Random.Range(0.7f, 0.75f));

                MainMashRenderer.material = material;
            }
        }
    }
}
