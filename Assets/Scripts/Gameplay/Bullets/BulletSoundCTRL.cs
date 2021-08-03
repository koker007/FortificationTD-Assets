using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSoundCTRL : MonoBehaviour
{

    [Header("Audio Live Sources")]
    [SerializeField]
    AudioSource ASLiveNearest;
    [SerializeField]
    AudioSource ASLiveMedium;
    [SerializeField]
    AudioSource ASLiveFar;

    [Header("Audio Die Sources")]
    [SerializeField]
    AudioSource ASDieNearest;
    [SerializeField]
    AudioSource ASDieMedium;
    [SerializeField]
    AudioSource ASDieFar;

    [Header("Audio Live Clips")]
    [SerializeField]
    AudioClip[] ACLiveNearest;
    [SerializeField]
    AudioClip[] ACLiveMedium;
    [SerializeField]
    AudioClip[] ACLiveFar;

    [Header("Audio Die Clips")]
    [SerializeField]
    AudioClip[] ACDieNearest;
    [SerializeField]
    AudioClip[] ACDieMedium;
    [SerializeField]
    AudioClip[] ACDieFar;

    [Header("Priority")]
    [SerializeField]
    float MinDistPriority = 0;
    [SerializeField]
    float MedDistPriority = 0;
    [SerializeField]
    float MaxDistPriority = 0;

    [SerializeField]
    float PitchBias = 0;

    const float MaxPriority = 256;

    BulletCtrl bulletCtrl;

    // Start is called before the first frame update
    void Start()
    {
        bulletCtrl = GetComponent<BulletCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        TestSoundPlay();
    }

    bool LiveOld = true;
    void TestSoundPlay() {
        if (bulletCtrl && Setings.main && Setings.main.game != null && MainCamera.main)
        {
            //получаем растояние до камеры
            float distToCam = Vector3.Distance(transform.position, MainCamera.main.transform.position);
            if (distToCam > MaxPriority / 2)
                distToCam = MaxPriority / 2;

            float volumeDist = 1 - (distToCam / 60);
            if (volumeDist < 0)
                volumeDist = 0;
            else if (volumeDist > 1)
                volumeDist = 1;


            if (!bulletCtrl.die)
            {

                //близкий
                if (ASLiveNearest)
                {
                    float volumeMin = 1 - distToCam / ASLiveNearest.maxDistance;
                    float PriorityMin = (MaxPriority * 0.75f) + MinDistPriority - Calc.Sound.getCoofPriory(10, distToCam) * 20 + distToCam / 10;
                    if (volumeMin <= 0)
                        PriorityMin = 256;

                    ASLiveNearest.priority = (int)PriorityMin;
                    ASLiveNearest.volume = volumeMin * volumeDist * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    if (!ASLiveNearest.isPlaying)
                    {
                        ASLiveNearest.clip = ACLiveNearest[Random.Range(0, ACLiveNearest.Length)];
                        ASLiveNearest.pitch = 1.0f + Random.Range(-PitchBias, PitchBias);
                        ASLiveNearest.Play();
                    }
                }
                //средний
                if (ASLiveMedium)
                {
                    float volumeMedium = 1 - distToCam / ASLiveMedium.volume;
                    float PriorityMedium = (MaxPriority * 0.75f) + MedDistPriority - Calc.Sound.getCoofPriory(20, distToCam) * 20 + distToCam / 10;
                    if (volumeMedium <= 0)
                        PriorityMedium = 256;

                    ASLiveMedium.priority = (int)PriorityMedium;
                    ASLiveMedium.volume = volumeMedium * volumeDist * Setings.main.game.volume_all * Setings.main.game.volume_sound;

                    if (!ASLiveMedium.isPlaying)
                    {
                        ASLiveMedium.clip = ACLiveMedium[Random.Range(0, ACLiveMedium.Length)];
                        ASLiveMedium.pitch = 1.0f + Random.Range(-PitchBias, PitchBias);
                        ASLiveMedium.Play();
                    }

                }
                //Далекий
                if (ASLiveFar)
                {
                    float volumeMax = 1 - distToCam / ASLiveFar.maxDistance;
                    float PriorityMax = (MaxPriority * 0.75f) + MaxDistPriority - Calc.Sound.getCoofPriory(60, distToCam) * 20 + distToCam / 10;
                    if (volumeMax <= 0)
                        PriorityMax = 256;

                    ASLiveFar.priority = (int)PriorityMax;
                    ASLiveFar.volume = volumeMax * volumeDist * Setings.main.game.volume_all * Setings.main.game.volume_sound;

                    if (!ASLiveFar.isPlaying)
                    {
                        ASLiveFar.clip = ACLiveFar[Random.Range(0, ACLiveFar.Length)];
                        ASLiveFar.pitch = 1.0f + Random.Range(-PitchBias, PitchBias);
                        ASLiveFar.Play();
                    }
                }
            }
            else if (bulletCtrl.die && LiveOld)
            {
                LiveOld = false;

                //близкий
                if (ASLiveNearest)
                {
                    if (ASLiveNearest.isPlaying)
                        ASLiveNearest.Stop();
                    ASLiveNearest.loop = false;
                }
                if (ASDieNearest && ACDieNearest.Length > 0)
                {
                    float volumeMin = 1 - distToCam / ASDieNearest.maxDistance;
                    float PriorityMin = (MaxPriority * 0.75f) + MinDistPriority - Calc.Sound.getCoofPriory(10, distToCam) * 20 + distToCam / 10;
                    if (volumeMin <= 0)
                        PriorityMin = 256;

                    ASDieNearest.priority = (int)PriorityMin;
                    ASDieNearest.volume = volumeMin * volumeDist * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASDieNearest.clip = ACDieNearest[Random.Range(0, ACDieNearest.Length)];
                    ASDieNearest.pitch = 1.0f + Random.Range(-PitchBias, PitchBias);
                    ASDieNearest.Play();
                }

                //средний
                if (ASLiveMedium)
                {
                    if (ASLiveMedium.isPlaying)
                        ASLiveMedium.Stop();
                    ASLiveMedium.loop = false;
                }
                if (ASDieMedium && ACDieMedium.Length > 0)
                {
                    float volumeMin = 1 - distToCam / ASDieMedium.maxDistance;
                    float PriorityMin = (MaxPriority * 0.75f) + MinDistPriority - Calc.Sound.getCoofPriory(20, distToCam) * 20 + distToCam / 10;
                    if (volumeMin <= 0)
                        PriorityMin = 256;

                    ASDieMedium.priority = (int)PriorityMin;
                    ASDieMedium.volume = volumeMin * volumeDist * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASDieMedium.clip = ACDieMedium[Random.Range(0, ACDieMedium.Length)];
                    ASDieMedium.pitch = 1.0f + Random.Range(-PitchBias, PitchBias);
                    ASDieMedium.Play();
                }
                //Далекий
                if (ASLiveFar)
                {
                    if (ASLiveFar.isPlaying)
                        ASLiveFar.Stop();
                    ASLiveFar.loop = false;
                }

                if (ASDieFar && ACDieFar.Length > 0)
                {
                    float volumeMin = 1 - distToCam / ASDieFar.maxDistance;
                    float PriorityMin = (MaxPriority * 0.75f) + MinDistPriority - Calc.Sound.getCoofPriory(20, distToCam) * 20 + distToCam / 10;
                    if (volumeMin <= 0)
                        PriorityMin = 256;

                    ASDieFar.priority = (int)PriorityMin;
                    ASDieFar.volume = volumeMin * volumeDist * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASDieFar.clip = ACDieFar[Random.Range(0, ACDieFar.Length)];
                    ASDieFar.pitch = 1.0f + Random.Range(-PitchBias, PitchBias);
                    ASDieFar.Play();
                }
            }
        }

    }
}
