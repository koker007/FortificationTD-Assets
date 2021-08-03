using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCTRL : MonoBehaviour
{

    float timeEnd = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetAllParticles();
    }

    void GetAllParticles() {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        float durationMax = 0;
        foreach (ParticleSystem particleSystem in particleSystems) {
            if (durationMax < particleSystem.main.duration) {
                durationMax = particleSystem.main.duration;
            }
        }

        timeEnd = durationMax + Time.time;


    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeEnd) {
            Destroy(gameObject);
        }
    }
}
