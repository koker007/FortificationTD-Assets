using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISoundCTRL : MonoBehaviour
{

    [SerializeField]
    AICTRL aICTRL;
    [SerializeField]
    bool PlayNonStop = false;
    AudioClip[] ClipsNonStop;

    [SerializeField]
    int MinPriorityStep;
    [SerializeField]
    AudioSource ASStep;
    [SerializeField]
    AudioClip[] ACSteps;
    [SerializeField]
    AudioClip[] ACStepsSlope;

    // Update is called once per frame
    void Update()
    {
        testDistance();
        TestPlayNonStop();
    }

    void TestPlayNonStop()
    {
        if (PlayNonStop && aICTRL && aICTRL.healthNow > 0 && distance < ASStep.maxDistance && ASStep != null && !ASStep.isPlaying && ClipsNonStop != null && ClipsNonStop.Length > 0)
        {
            ASStep.volume = Setings.main.game.volume_all * Setings.main.game.volume_sound;

            //вытаскиваем следующий клип
            AudioClip nextClip = ClipsNonStop[Random.Range(0, ClipsNonStop.Length)];
            //рандомим еще если клипы совпадают
            if (nextClip == ASStep.clip) nextClip = ClipsNonStop[Random.Range(0, ClipsNonStop.Length)];

            ASStep.clip = nextClip;
            ASStep.Play();
        }
    }

    float distance = 999999;
    void testDistance() {
        if (MainCamera.main) {
            distance = Vector3.Distance(MainCamera.main.transform.position, transform.position);
        }
    }

    const float MaxPriority = 256;
    float getCoofPriory(float distForMax)
    {
        //Приоритет для дальнего звука
        float distPrior = 0;
        if (distance < distForMax && distance > 0)
            distPrior = distance / distForMax;
        else if (distance > distForMax && distance < distForMax * 2)
            distPrior = 1 - ((distance - distForMax) / distForMax);

        return distPrior;
    }

    public void PlayStep() {
        if (ASStep && distance < ASStep.maxDistance && Setings.main && Setings.main.game != null && ACSteps.Length > 0)
        {
            if (!PlayNonStop)
            {
                ASStep.priority = (int)((MaxPriority * 0.75f) + MinPriorityStep - getCoofPriory(ASStep.maxDistance) * 10 + distance / 10);
                ASStep.volume = Setings.main.game.volume_all * Setings.main.game.volume_sound;
                ASStep.pitch = Random.Range(0.95f, 1.05f);
                ASStep.PlayOneShot(ACSteps[Random.Range(0, ACSteps.Length)]);
            }
            else {
                ClipsNonStop = ACSteps;
                ASStep.priority = (int)((MaxPriority * 0.75f) + MinPriorityStep - getCoofPriory(ASStep.maxDistance) * 10 + distance / 10);
            }
        }
    }
    public void PlayStepSlope()
    {
        if (ASStep && distance < ASStep.maxDistance && Setings.main && Setings.main.game != null && ACStepsSlope.Length > 0)
        {
            if (!PlayNonStop)
            {
                ASStep.priority = (int)((MaxPriority * 0.75f) + MinPriorityStep - getCoofPriory(ASStep.maxDistance) * 10 + distance / 10);
                ASStep.volume = Setings.main.game.volume_all * Setings.main.game.volume_sound;
                ASStep.pitch = Random.Range(0.95f, 1.05f);
                ASStep.PlayOneShot(ACStepsSlope[Random.Range(0, ACStepsSlope.Length)]);
            }
            else
            {
                ClipsNonStop = ACStepsSlope;
                ASStep.pitch = Random.Range(0.97f, 1.03f);
                ASStep.priority = (int)((MaxPriority * 0.75f) + MinPriorityStep - getCoofPriory(ASStep.maxDistance) * 10 + distance / 10);
            }
        }
    }
}
