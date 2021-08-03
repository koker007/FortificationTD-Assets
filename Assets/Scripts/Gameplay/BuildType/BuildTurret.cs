using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTurret : MonoBehaviour
{
    [SerializeField]
    Building building;
    void iniBuildPrefab() {
        if (building == null)
            building = gameObject.GetComponent <Building>();
    }

    [SerializeField]
    Animator AnimTurrel;
    [SerializeField]
    Animator AnimPlatform;

    [SerializeField]
    GameObject FireMainPos;
    [SerializeField]
    GameObject FireMainPrefab;

    [SerializeField]
    bool fire = false;
    public void fireTest()
    {
        if (AnimTurrel != null) {
            if (fire)
            {
                fire = false;
                AnimTurrel.SetBool("fire", true);
                if (FireMainPrefab != null && FireMainPos != null)
                {
                    GameObject particleFireObj = Instantiate(FireMainPrefab);
                    particleFireObj.GetComponent<ParticleSystem>().Play();
                    particleFireObj.transform.parent = FireMainPos.transform.parent;
                    particleFireObj.transform.localPosition = FireMainPos.transform.localPosition;
                }
            }
            else {
                AnimTurrel.SetBool("fire", false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fireTest();
    }

}
