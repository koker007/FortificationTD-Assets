using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaceVoiceCTRL : MonoBehaviour
{
    static public BaceVoiceCTRL main;

    [SerializeField]
    AudioSource ASBaceVoice;

    [SerializeField]
    AudioClip[] AC90;
    [SerializeField]
    AudioClip[] AC80;
    [SerializeField]
    AudioClip[] AC70;
    [SerializeField]
    AudioClip[] AC60;
    [SerializeField]
    AudioClip[] AC50;
    [SerializeField]
    AudioClip[] AC40;
    [SerializeField]
    AudioClip[] AC30;
    [SerializeField]
    AudioClip[] AC20;
    [SerializeField]
    AudioClip[] AC10;
    [SerializeField]
    AudioClip[] AC05;

    public void PlayVoice(int healt) {
        if (Setings.main && Setings.main.game != null && ASBaceVoice && !ASBaceVoice.isPlaying && GameplayCTRL.main && (GameplayCTRL.main.gamemode == 2 || GameplayCTRL.main.gamemode == 3)) {

            if (AC90 != null && AC90.Length > 0 && healt == 90) {
                ASBaceVoice.PlayOneShot(AC90[Random.Range(0, AC90.Length)]);
            }
            else if (AC80 != null && AC80.Length > 0 && healt == 80)
            {
                ASBaceVoice.PlayOneShot(AC80[Random.Range(0, AC80.Length)]);
            }
            else if (AC70 != null && AC70.Length > 0 && healt == 70)
            {
                ASBaceVoice.PlayOneShot(AC70[Random.Range(0, AC70.Length)]);
            }
            else if (AC60 != null && AC60.Length > 0 && healt == 60)
            {
                ASBaceVoice.PlayOneShot(AC60[Random.Range(0, AC60.Length)]);
            }
            else if (AC50 != null && AC50.Length > 0 && healt == 50)
            {
                ASBaceVoice.PlayOneShot(AC50[Random.Range(0, AC50.Length)]);
            }
            else if (AC40 != null && AC40.Length > 0 && healt == 40)
            {
                ASBaceVoice.PlayOneShot(AC40[Random.Range(0, AC40.Length)]);
            }
            else if (AC30 != null && AC30.Length > 0 && healt == 30)
            {
                ASBaceVoice.PlayOneShot(AC30[Random.Range(0, AC30.Length)]);
            }
            else if (AC20 != null && AC20.Length > 0 && healt == 20)
            {
                ASBaceVoice.PlayOneShot(AC20[Random.Range(0, AC20.Length)]);
            }
            else if (AC10 != null && AC10.Length > 0 && healt == 10)
            {
                ASBaceVoice.PlayOneShot(AC10[Random.Range(0, AC10.Length)]);
            }
            else if (AC05 != null && AC05.Length > 0 && healt == 5)
            {
                ASBaceVoice.PlayOneShot(AC05[Random.Range(0, AC05.Length)]);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        volume();
    }

    void volume() {
        if (ASBaceVoice && Setings.main && Setings.main.game != null) {
            ASBaceVoice.volume = (InterierCTRL.interierVolume - 0.1f) * 0.75f * Setings.main.game.volume_all * Setings.main.game.volume_sound;
        }
        else if (ASBaceVoice) {
            ASBaceVoice.volume = 0;
        }
    }
}
