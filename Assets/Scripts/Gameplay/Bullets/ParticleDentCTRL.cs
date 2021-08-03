using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDentCTRL : MonoBehaviour
{

    [SerializeField]
    ParticleSystem mainParticle;

    public Vector3 normal;

    [SerializeField]
    float liveTimeMax = 30;
    float liveTimeNow = 0;

    // Start is called before the first frame update
    void Start()
    {
        liveTimeNow = liveTimeMax;
        StartPlay();
    }

    void StartPlay() {
        if (mainParticle == null) {
            mainParticle = gameObject.GetComponent<ParticleSystem>();
        }

        if (mainParticle != null)
        {
            mainParticle.startLifetime = liveTimeMax;
            mainParticle.Play();
            gameObject.transform.rotation = Quaternion.LookRotation(normal);
        }
        else {
            liveTimeNow = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        liveCtlr();
    }

    void liveCtlr() {
        liveTimeNow -= Time.deltaTime;

        if (liveTimeNow < 0) {
            Destroy(gameObject);
        }
    }
}
