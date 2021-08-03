using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTurretCTRL : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField]
    AudioSource ASMinDist;
    [SerializeField]
    AudioSource ASMediumDist;
    [SerializeField]
    AudioSource ASMaxDist;

    [Header("Audio Clips")]
    [SerializeField]
    AudioClip[] ACMinDist;
    [SerializeField]
    AudioClip[] ACMediumDist;
    [SerializeField]
    AudioClip[] ACMaxDist;

    [Header("Priority")]
    [SerializeField]
    float MinDistPriority = 0;
    [SerializeField]
    float MedDistPriority = 0;
    [SerializeField]
    float MaxDistPriority = 0;

    const float MaxPriority = 256;


    GameObject MainCam;
    void iniMainCam() {
        if (MainCam == null) {
            MainCam = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    Setings setings;
    void iniSetings() {
        if (setings == null) {
            GameObject setingsObj = GameObject.FindGameObjectWithTag("Setings");
            if (setingsObj != null) {
                setings = setingsObj.GetComponent<Setings>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("iniMainCam", 0.3f);
        Invoke("iniSetings", 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        TestAudioSource();
    }

    float distListener = 0;
    /// <summary>
    /// Задать приоритет и настроить громкость
    /// </summary>
    void TestAudioSource() {
        if (setings != null && setings.game != null && MainCam != null) {
            distListener = Vector3.Distance(MainCam.transform.position, gameObject.transform.position);
            
            if (distListener > MaxPriority / 2)
                distListener = MaxPriority / 2;

            float volumeDist = 1 - (distListener / 60);
            if (volumeDist < 0)
                volumeDist = 0;
            else if (volumeDist > 1)
                volumeDist = 1;

            float volumeMin = 1 - distListener/20;
            float volumeMedium = 1 - distListener/40;
            float volumeMax = 1 - distListener/120;

            //float myPriority = MaxDistPriority/2 + ma

            if (ASMinDist != null) {
                float PriorityMin = (MaxPriority * 0.75f) + MinDistPriority - getCoofPriory(10) * 20 + distListener/10;
                if (volumeMin <= 0)
                    PriorityMin = 256;

                ASMinDist.priority = (int)PriorityMin;
                ASMinDist.volume = volumeMin * volumeDist * setings.game.volume_all * setings.game.volume_sound;

            }
            if (ASMediumDist != null) {
                float PriorityMedium = (MaxPriority * 0.75f) + MedDistPriority - getCoofPriory(20) * 20 + distListener/10;
                if (volumeMedium <= 0)
                    PriorityMedium = 256;

                ASMediumDist.priority = (int)PriorityMedium;
                ASMediumDist.volume = volumeMedium * volumeDist * setings.game.volume_all * setings.game.volume_sound;
            }
            if (ASMaxDist != null) {

                float PriorityMax = (MaxPriority * 0.75f) + MaxDistPriority - getCoofPriory(60) * 20 + distListener/10;
                if (volumeMax <= 0)
                    PriorityMax = 256;

                ASMaxDist.priority = (int)PriorityMax;
                ASMaxDist.volume = volumeMax * volumeDist * setings.game.volume_all * setings.game.volume_sound;
            }

            float getCoofPriory(float distForMax) {
                //Приоритет для дальнего звука
                float distPrior = 0;
                if (distListener < distForMax && distListener > 0)
                    distPrior = distListener / distForMax;
                else if (distListener > distForMax && distListener < distForMax * 2)
                    distPrior = 1 - ((distListener - distForMax) / distForMax);

                return distPrior;
            }
        }
    }

    /// <summary>
    /// Воспроизвести звук стрельбы турели
    /// </summary>
    public void Fire() {
        if (SoundMap != null) {
            SoundMap.SetData(new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.z));
        }

        //Invoke("PlayMinSound", distListener/30f);
        //Invoke("PlayMediumSound", distListener / 30f);
        //Invoke("PlayMaxSound", distListener / 30f);

        PlayMinSound();
        PlayMediumSound();
        PlayMaxSound();

        void PlayMinSound() {
            if (ASMinDist != null && ACMinDist != null && ACMinDist.Length > 0) {
                if (ASMinDist.isPlaying) ASMinDist.Stop();
                ASMinDist.pitch = Random.Range(0.95f, 1.05f);
                ASMinDist.PlayOneShot(ACMinDist[Random.Range(0, ACMinDist.Length)]);
            }
        }
        void PlayMediumSound() {
            if (ASMediumDist != null && ACMediumDist != null && ACMediumDist.Length > 0)
            {
                if (ASMediumDist.isPlaying) ASMediumDist.Stop();
                ASMediumDist.pitch = Random.Range(0.95f, 1.05f);
                ASMediumDist.PlayOneShot(ACMediumDist[Random.Range(0, ACMediumDist.Length)]);
            }
        }
        void PlayMaxSound() {
            if (ASMaxDist != null && ACMaxDist != null && ACMaxDist.Length > 0)
            {
                if (ASMaxDist.isPlaying) ASMaxDist.Stop();
                ASMaxDist.pitch = Random.Range(0.95f, 1.05f);
                ASMaxDist.PlayOneShot(ACMaxDist[Random.Range(0, ACMaxDist.Length)]);
            }
        }
        
    }

    public static GameplayCTRL.InfoMap SoundMap;
}
