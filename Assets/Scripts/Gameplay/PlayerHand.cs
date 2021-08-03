using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{

    [SerializeField]
    public Player MyPlayer;
    [SerializeField]
    public Camera MainCamera;
    [SerializeField]
    public Transform PosForIndicator;

    [SerializeField]
    public MeshRenderer HandRender;
    [SerializeField]
    Material HandMaterial;

    // Start is called before the first frame update
    void Start()
    {
    }



    // Update is called once per frame
    void Update()
    {
        testLive();
        testPos();
        testColor();
    }

    //Удалить если пропал игрок или нету меша
    void testLive() {
        if (MyPlayer == null || HandRender == null) {
            Destroy(gameObject);
        }
    }

    //Перемещаем и вращаем
    void testPos() {
        if (MyPlayer != null)
        {
            if (!MyPlayer.CursorInUI && !MyPlayer.isLocalPlayer)
            {
                HandRender.gameObject.SetActive(true);
                if (Time.unscaledDeltaTime < 1f/10f)
                    transform.position += (MyPlayer.CursorPos - transform.position)*(Time.unscaledDeltaTime*10);
                else transform.position += (MyPlayer.CursorPos - transform.position) * Time.unscaledDeltaTime;

                Quaternion rotNeed = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(MyPlayer.CursorPos - MyPlayer.MyCameraPos), Time.unscaledDeltaTime);
                transform.rotation = rotNeed;
            }
            else {
                HandRender.gameObject.SetActive(false);
            }
        }
    }

    void testColor()
    {
        if (MyPlayer != null && HandRender != null && MainCamera != null)
        {
            Material material = HandRender.material;
            float alpha = 1;
            float minDist = 3;
            float alphaDist = 3;
            float distToCursor = Vector3.Distance(MainCamera.transform.position, transform.position);
            if (distToCursor < (alphaDist + minDist) && distToCursor > alphaDist)
                alpha = (distToCursor - minDist) / alphaDist;
            else if(distToCursor <= minDist) alpha = 0;

            material.color = new Color(MyPlayer.colorVec.x * 0.7f, MyPlayer.colorVec.y * 0.7f, MyPlayer.colorVec.z * 0.7f, alpha);

            HandRender.material = material;
        }
    }

}
