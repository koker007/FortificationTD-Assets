using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterierVisualizatorCTRL : MonoBehaviour
{
    static public InterierVisualizatorCTRL main;

    [SerializeField]
    Camera InterierCamera;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        //InterierCamTransform();
    }

    Vector3 smeshenie = new Vector3(30, 0, 30);
    public void InterierCamTransform() {
        if (InterierCamera && MainCamera.main) {
            InterierCamera.transform.localPosition = (MainCamera.main.transform.position - smeshenie)/20f;
            InterierCamera.transform.rotation = MainCamera.main.transform.rotation;
        }
    }
}
