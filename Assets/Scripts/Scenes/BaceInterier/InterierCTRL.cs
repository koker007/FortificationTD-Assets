using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterierCTRL : MonoBehaviour
{
    [SerializeField]
    MeshRenderer RenderInterier;
    


    [SerializeField]
    GameObject AlarmLight;
    [SerializeField]
    TextMesh TowerInfoText;
    [SerializeField]
    Camera interierCamera;

    [SerializeField]
    float HeightCamMax = 38;

    [SerializeField]
    string[] TowerInfoKeys;
    string TowerInfo0 = "Enemy Detected";
    string TowerInfo1 = "Tower Defence";
    string TowerInfo2 = "Attention";
    string TowerInfo3 = "Enemy invasion";
    string TowerInfo4 = "Facility Damage";
    string TowerInfo5 = "Mission threatened";
    string TowerInfo6 = "Alarm";
    string TowerInfo7 = "Critical Damage";
    string TowerInfo8 = "FIRE";
    string TowerInfo9 = "System ERROR";

    [SerializeField]
    ParticleSystem ShakeSmoke;

    [SerializeField]
    GameObject TerraObj;
    [SerializeField]
    float posUp = 0;
    [SerializeField]
    float posDown = 60;

    static public float interierVolume = 0;

    void iniTowerInfoText() {
        if (Setings.main && Setings.main.LangugeText != null) {
            for (int num = 0; num < TowerInfoKeys.Length; num++) {
                string textNew = Setings.main.LangugeText.get_text_from_key(TowerInfoKeys[num]);
                if (textNew != "") {
                    if (num == 1) {
                        TowerInfo1 = textNew;
                    }
                    else if (num == 2) {
                        TowerInfo2 = textNew;
                    }
                    else if (num == 3)
                    {
                        TowerInfo3 = textNew;
                    }
                    else if (num == 4)
                    {
                        TowerInfo4 = textNew;
                    }
                    else if (num == 5)
                    {
                        TowerInfo5 = textNew;
                    }
                    else if (num == 6)
                    {
                        TowerInfo6 = textNew;
                    }
                    else if (num == 7)
                    {
                        TowerInfo7 = textNew;
                    }
                    else if (num == 8)
                    {
                        TowerInfo8 = textNew;
                    }
                    else if (num == 9)
                    {
                        TowerInfo9 = textNew;
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("iniTowerInfoText", 1f);
        TowerInfotimeText.Set(0.1f, 0.01f, " ", false, false);
    }

    // Update is called once per frame
    void Update()
    {
        TestInterirer();

        TestAlarmLight();
        TestTowerInfoText();
        TestTerra();

        TestSounds();
    }

    void TestAlarmLight() {
        if (GameplayCTRL.main && AlarmLight) {
            if (GameplayCTRL.main.BaceHealth < 35 || (GameplayCTRL.main.BaceHealth < 50 && Time.unscaledTime - GameplayCTRL.main.timeDamage < 22))
            {
                AlarmLight.active = true;
                AlarmLight.transform.Rotate(0, 360 * Time.unscaledDeltaTime, 0);
            }
            else {
                AlarmLight.active = false;
            }
        }
    }

    TimeText TowerInfotimeText;
    void TestTowerInfoText() {
        if (TowerInfoText && GameplayCTRL.main) {
            TowerInfoText.text = TowerInfotimeText.Get();

            if (TowerInfotimeText.TimeDone > 3)
            {
                SetNewText();
                TowerInfoText.color = new Color(1, 0, 0);
            }
            else if (TowerInfotimeText.TimeDone > 2.5f)
            {
                TowerInfoText.color = new Color(1, 0, 0, 1);
            }
            else if (TowerInfotimeText.TimeDone > 2.0f) {
                TowerInfoText.color = new Color(1, 0, 0, 0);
            }
            else if (TowerInfotimeText.TimeDone > 1.5f)
            {
                TowerInfoText.color = new Color(1, 0, 0, 1);
            }
            else if (TowerInfotimeText.TimeDone > 1.0f)
            {
                TowerInfoText.color = new Color(1, 0, 0, 0);
            }
            else if (TowerInfotimeText.TimeDone > 0.5f)
            {
                TowerInfoText.color = new Color(1, 0, 0, 1);
            }
            else if (TowerInfotimeText.TimeDone > 0.0f)
            {
                TowerInfoText.color = new Color(1, 0, 0, 0);
            }
            else if (TowerInfotimeText.TimeDone == 0)
            {
                TowerInfoText.color = new Color(1, 0, 0, 1);
            }

            void SetNewText()
            {
                List<string> ListText = new List<string>();
                if (GameplayCTRL.main.BaceHealth < 90 && GameplayCTRL.main.BaceHealth > 30)
                {
                    ListText.Add(TowerInfo1);
                }
                if (GameplayCTRL.main.BaceHealth < 80 && GameplayCTRL.main.BaceHealth > 20)
                {
                    ListText.Add(TowerInfo2);
                }
                if (GameplayCTRL.main.BaceHealth < 70 && GameplayCTRL.main.BaceHealth > 10)
                {
                    ListText.Add(TowerInfo3);
                }
                if (GameplayCTRL.main.BaceHealth < 60 && GameplayCTRL.main.BaceHealth > 5)
                {
                    ListText.Add(TowerInfo4);
                }
                if (GameplayCTRL.main.BaceHealth < 50 && GameplayCTRL.main.BaceHealth > 5)
                {
                    ListText.Add(TowerInfo5);
                }
                if (GameplayCTRL.main.BaceHealth < 40 && GameplayCTRL.main.BaceHealth > 5)
                {
                    ListText.Add(TowerInfo6);
                }
                if (GameplayCTRL.main.BaceHealth < 30)
                {
                    ListText.Add(TowerInfo7);
                }
                if (GameplayCTRL.main.BaceHealth < 20)
                {
                    ListText.Add(TowerInfo8);
                }
                if (GameplayCTRL.main.BaceHealth < 10)
                {
                    ListText.Add(TowerInfo9);
                }
                if (GameplayCTRL.main.BaceHealth < 5)
                {
                    ListText.Add("#124%ER!");
                }

                if (ListText.Count > 0)
                {
                    string text = ListText[Random.Range(0, ListText.Count)];
                    TowerInfotimeText.Set(0.1f, 0.05f, text, false, false);

                    if (BaceVoiceCTRL.main != null) {
                        if (TowerInfo0 == text) {
                            
                        }
                        else if (TowerInfo1 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(90);
                        }
                        else if (TowerInfo2 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(80);
                        }
                        else if (TowerInfo3 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(70);
                        }
                        else if (TowerInfo4 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(60);
                        }
                        else if (TowerInfo5 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(50);
                        }
                        else if (TowerInfo6 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(40);
                        }
                        else if (TowerInfo7 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(30);
                        }
                        else if (TowerInfo8 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(20);
                        }
                        else if (TowerInfo9 == text)
                        {
                            BaceVoiceCTRL.main.PlayVoice(10);
                        }
                    }
                }
            }
        }
    }

    float HealtBaceOld = 100;
    float VisioOld = 0;
    float VisioTimeNeed = 1; //Как долго показывать
    void TestInterirer() {
        if (GameplayCTRL.main && RenderInterier && interierCamera) {
            if (HealtBaceOld != GameplayCTRL.main.BaceHealth) {

                if (HealtBaceOld > GameplayCTRL.main.BaceHealth)
                {
                    ShakeSmokePlay();
                    if (GameplayCTRL.main.BaceHealth < 5)
                    {
                        VisioOld = Time.unscaledTime;
                        VisioTimeNeed = 90;
                    }
                    if (GameplayCTRL.main.BaceHealth < 10) {
                        VisioOld = Time.unscaledTime;
                        VisioTimeNeed = 5;
                    }
                    else if (GameplayCTRL.main.BaceHealth < 20)
                    {
                        VisioOld = Time.unscaledTime;
                        VisioTimeNeed = 3;
                    }
                    else if (GameplayCTRL.main.BaceHealth < 30)
                    {
                        VisioOld = Time.unscaledTime;
                        VisioTimeNeed = 1;
                    }
                    else if (GameplayCTRL.main.BaceHealth < 40)
                    {
                        VisioOld = Time.unscaledTime;
                        VisioTimeNeed = 0.9f;
                    }
                    else if (GameplayCTRL.main.BaceHealth < 50)
                    {
                        VisioOld = Time.unscaledTime;
                        VisioTimeNeed = 0.5f;
                    }
                    else if (GameplayCTRL.main.BaceHealth < 60)
                    {
                        VisioOld = Time.unscaledTime;
                        VisioTimeNeed = 0.2f;
                    }
                    else if (GameplayCTRL.main.BaceHealth < 70)
                    {
                        VisioOld = Time.unscaledTime;
                        VisioTimeNeed = 0.1f;
                    }
                }
                else {

                }
                HealtBaceOld = GameplayCTRL.main.BaceHealth;
            }

            if (GameplayCTRL.main.gamemode == 2)
            {
                if ((VisioOld + VisioTimeNeed) > Time.unscaledTime
                    || (Player.me && Player.me.controlsMouse.ScrollHeight > HeightCamMax)
                    || (MainCamera.main && Vector2.Distance(new Vector2(MainCamera.main.transform.position.x, MainCamera.main.transform.position.z), new Vector2(30,30)) > 45f))
                {
                    interierOn();
                }
                else
                {
                    InterierOff();
                }
            }
            else if (GameplayCTRL.main.gamemode == 3 && GameplayCTRL.main.BaceHealth <= 0)
            {
                interierOn();
                Debug.Log("end scene " + GameplayCTRL.main.timeEndScene);
            }
            else {
                InterierOff();
            }


            void interierOn() {
                interierCamera.gameObject.active = true;
                RenderInterier.gameObject.active = true;
                if (GameplayCTRL.main.SeaOff) GameplayCTRL.main.SeaOff.active = false;

                if (interierCamera.targetTexture.width != Screen.width) {
                    interierCamera.targetTexture = new RenderTexture(Screen.width, Screen.width,1);
                    Material material = RenderInterier.material;
                    material.mainTexture = interierCamera.targetTexture;
                    RenderInterier.material = material;
                }
            }
            void InterierOff() {
                interierCamera.gameObject.active = false;
                RenderInterier.gameObject.active = false;
                if (GameplayCTRL.main.SeaOff) GameplayCTRL.main.SeaOff.active = true;
            }

            void ShakeSmokePlay()
            {
                if (ShakeSmoke)
                {
                    ShakeSmoke.Play();
                }
            }
        }
        else if (!GameplayCTRL.main && interierCamera) {
            interierCamera.gameObject.active = false;
        }
    }

    void TestTerra() {
        if (TerraObj && GameplayCTRL.main) {
            if (GameplayCTRL.main.gamemode == 2) {
                TerraObj.transform.localPosition = new Vector3(0, 0, 0);

                float smehenie = posDown - posUp;

                float time = GameplayCTRL.main.timeGamePlay % (60*10);
                float timeSec = time % 60;
                float timeMin = time / 60;

                //Опускаем
                if (timeMin <= 1 && timeSec < 50 && GameplayCTRL.main.timeGamePlay > 60)
                {
                    TerraObj.transform.localPosition = new Vector3(0, posUp + (smehenie*(timeSec/50)),0);
                }
                //Поднимаем
                else if (timeMin >= 8 && timeMin <= 9)
                {
                    TerraObj.transform.localPosition = new Vector3(0, posUp + (smehenie * (1-(timeSec / 60))), 0);
                }
                //Вверху
                else if (timeMin >= 9)
                {
                    TerraObj.transform.localPosition = new Vector3(0, 0, 0);
                }
                //Опушен
                else {
                    TerraObj.transform.localPosition = new Vector3(0, posDown, 0);
                }
            }
            else if (GameplayCTRL.main.gamemode == 3 && RocketCTRL.main) {
                TerraObj.transform.localPosition = new Vector3(0, (RocketCTRL.main.transform.localPosition.y- 0.6982098f) * -40,0);
            }
        }
    }

    float BaceSoundNeed = 1;
    float BaceSoundNow = 1;

    [Header("Sounds")]
    [SerializeField]
    AudioSource ASAlarm1;
    [SerializeField]
    AudioSource ASAlarm2;
    [SerializeField]
    AudioSource ASAlarm3;
    [SerializeField]
    AudioSource ASAlarm4;

    [SerializeField]
    AudioSource ASClock1;
    [SerializeField]
    AudioSource ASClock2;
    [SerializeField]
    AudioSource ASClock3;
    [SerializeField]
    AudioSource ASClock4;

    [SerializeField]
    AudioSource ASRocketLift;
    [SerializeField]
    AudioSource ASRocketLiftShot;
    [SerializeField]
    AudioClip ACRocketLiftStart;
    [SerializeField]
    AudioClip ACRocketLiftEnd;
    [SerializeField]
    AudioClip ACRocketLiftSEngine;

    [SerializeField]
    AudioSource ASAmbientMap;

    bool New4Seconds = false;
    bool rosketLiftSound = false;
    void TestSounds() {

        if (GameplayCTRL.main && GameplayCTRL.main.gamemode == 2 && ASAlarm1 && ASAlarm2 && ASAlarm3 && ASAlarm4 && ASAmbientMap) {
            if (GameplayCTRL.main.timeGamePlay % 4 < 2)
            {
                if (!New4Seconds) {
                    TestPlayOrStopAllAlarm();
                    TestPlayOrStopAllClock();
                }
                New4Seconds = true;
            }
            else {
                New4Seconds = false;
            }

            TestSetings();

            void TestSetings() {
                interierVolume = 0.06f;
                if (MainCamera.main && RenderInterier && !RenderInterier.gameObject.active) {
                    float dist = Vector3.Distance(new Vector3(30, 10, 30), MainCamera.main.transform.position);
                    if (dist - 30 > 0) {
                        if (dist - 29 < 15)
                            interierVolume = (dist - 29) / 15f;
                        else interierVolume = 1;
                    }
                }
                else if (RenderInterier.gameObject.active) {
                    interierVolume = 1;
                }

                if (Setings.main && Setings.main.game != null) {
                    ASAlarm1.volume = 0.5f * interierVolume * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASAlarm2.volume = 0.5f * interierVolume * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASAlarm3.volume = 0.5f * interierVolume * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASAlarm4.volume = 0.5f * interierVolume * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;

                    ASClock1.volume = 0.2f * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASClock2.volume = 0.5f * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASClock3.volume = 0.5f * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASClock4.volume = 0.5f * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;

                    ASAmbientMap.volume = 0.2f * interierVolume * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    if (!ASAmbientMap.isPlaying) ASAmbientMap.Play();
                }
            }
            void TestPlayOrStopAllAlarm()
            {
                if (GameplayCTRL.main.BaceHealth < 50 && Time.unscaledTime - GameplayCTRL.main.timeDamage < 28)
                {
                    if (!ASAlarm1.isPlaying)
                        ASAlarm1.Play();
                }
                else
                {
                    if (ASAlarm1.isPlaying)
                        ASAlarm1.Stop();
                }
                if ((GameplayCTRL.main.BaceHealth < 40 && Time.unscaledTime - GameplayCTRL.main.timeDamage < 22) || GameplayCTRL.main.BaceHealth < 35)
                {
                    if (!ASAlarm2.isPlaying)
                        ASAlarm2.Play();
                }
                else
                {
                    if (ASAlarm2.isPlaying)
                        ASAlarm2.Stop();
                }
                if (GameplayCTRL.main.BaceHealth < 30 && Time.unscaledTime - GameplayCTRL.main.timeDamage < 14)
                {
                    if (!ASAlarm3.isPlaying)
                        ASAlarm3.Play();
                }
                else
                {
                    if (ASAlarm3.isPlaying)
                        ASAlarm3.Stop();
                }
                if (GameplayCTRL.main.BaceHealth < 20 && Time.unscaledTime - GameplayCTRL.main.timeDamage < 8)
                {
                    if (!ASAlarm4.isPlaying)
                        ASAlarm4.Play();
                }
                else
                {
                    if (ASAlarm4.isPlaying)
                        ASAlarm4.Stop();
                }
            }

            void TestPlayOrStopAllClock()
            {
                float LVLSec = GameplayCTRL.main.timeGamePlay % 600;
                float LVLNow = (int)(GameplayCTRL.main.timeGamePlay / 600);

                if ((LVLSec >= 535 && LVLSec <= 543) || (LVLSec >= 569 && LVLSec <= 573) || (LVLSec >= 583 && LVLSec <= 600))
                {
                    if (!ASClock1.isPlaying)
                        ASClock1.Play();
                }
                else
                {
                    if (ASClock1.isPlaying)
                        ASClock1.Stop();
                }
                if (LVLNow > 0 && (LVLSec <= 4 || (LVLSec >= 540 && LVLSec <= 543) || (LVLSec >= 587 && LVLSec <= 600)))
                {
                    if (!ASClock2.isPlaying)
                        ASClock2.Play();
                }
                else
                {
                    if (ASClock2.isPlaying)
                        ASClock2.Stop();
                }
                if (LVLSec >= 591)
                {
                    if (!ASClock3.isPlaying)
                        ASClock3.Play();
                }
                else
                {
                    if (ASClock3.isPlaying)
                        ASClock3.Stop();
                }
                if (LVLSec >= 595)
                {
                    if (!ASClock4.isPlaying)
                        ASClock4.Play();
                }
                else
                {
                    if (ASClock4.isPlaying)
                        ASClock4.Stop();
                }
            }

        }

        else if (GameplayCTRL.main && (GameplayCTRL.main.gamemode < 2 || GameplayCTRL.main.gamemode > 3) && ASAlarm1 && ASAlarm2 && ASAlarm3 && ASAlarm4 && ASAmbientMap) {
            StopAllAlarm();
            StopAllClock();

            void StopAllAlarm()
            {
                ASAlarm1.Stop();
                ASAlarm2.Stop();
                ASAlarm3.Stop();
                ASAlarm4.Stop();
            }
            void StopAllClock()
            {
                ASClock1.Stop();
                ASClock2.Stop();
                ASClock3.Stop();
                ASClock4.Stop();
            }

            ASAmbientMap.Stop();
        }

        if (GameplayCTRL.main && GameplayCTRL.main.gamemode == 2)
        {

            TestRosketList();
            TestSetings();

            void TestRosketList()
            {
                int LVL = (int)(GameplayCTRL.main.timeGamePlay / 600);
                float LVLTime = GameplayCTRL.main.timeGamePlay % 600;
                if (ASRocketLift && ACRocketLiftSEngine)
                {
                    //подем
                    if (LVLTime > 480 && LVLTime < 540)
                    {
                        if (!ASRocketLift.isPlaying)
                        {
                            ASRocketLift.pitch = 0f;
                            ASRocketLift.clip = ACRocketLiftSEngine;
                            ASRocketLift.loop = true;
                            ASRocketLift.Play();
                        }

                        ASRocketLift.pitch += Time.deltaTime * 0.10f;
                        if (ASRocketLift.pitch > 1)
                        {
                            ASRocketLift.pitch = 1;
                        }

                        if (!rosketLiftSound) {
                            rosketLiftSound = true;
                            PlayShotStart();
                        }
                    }
                    else if (LVLTime < 50 && LVL > 0)
                    {
                        if (!ASRocketLift.isPlaying)
                        {
                            ASRocketLift.pitch = 0f;
                            ASRocketLift.clip = ACRocketLiftSEngine;
                            ASRocketLift.loop = true;
                            ASRocketLift.Play();
                        }

                        ASRocketLift.pitch += Time.deltaTime * 0.3f;
                        if (ASRocketLift.pitch > 1)
                        {
                            ASRocketLift.pitch = 1;
                        }

                        if (!rosketLiftSound)
                        {
                            rosketLiftSound = true;
                            PlayShotStart();
                        }
                    }
                    else
                    {
                        if (ASRocketLift.isPlaying)
                        {
                            ASRocketLift.pitch -= Time.deltaTime * 0.1f;
                            if (ASRocketLift.pitch < 0.01f)
                            {
                                ASRocketLift.Stop();
                            }
                        }

                        if (rosketLiftSound) {
                            rosketLiftSound = false;
                            PlayShotStop();
                        }
                    }
                }
            }
            void TestSetings()
            {
                float volumeDist = 0.25f;
                if (MainCamera.main && RenderInterier && !RenderInterier.gameObject.active)
                {
                    float dist = Vector3.Distance(new Vector3(30, 10, 30), MainCamera.main.transform.position);
                    if (dist - 30 > 0)
                    {
                        if (dist - 26 < 15)
                            volumeDist = (dist - 26) / 15f;
                        else volumeDist = 1;
                    }
                }
                else if (RenderInterier.gameObject.active)
                {
                    volumeDist = 1;
                }

                if (Setings.main && Setings.main.game != null)
                {
                    ASRocketLift.volume = 0.5f * volumeDist * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                    ASRocketLiftShot.volume = 1f * volumeDist * BaceSoundNow * Setings.main.game.volume_all * Setings.main.game.volume_sound;
                }
            }

            void PlayShotStart()
            {
                if (ASRocketLiftShot && ACRocketLiftStart)
                {
                    ASRocketLiftShot.PlayOneShot(ACRocketLiftStart);
                }
            }
            void PlayShotStop() {
                if (ASRocketLiftShot && ACRocketLiftEnd) {
                    ASRocketLiftShot.PlayOneShot(ACRocketLiftEnd);
                }
            }
        }
        else {
            if (ASRocketLift && ASRocketLift.isPlaying) {
                ASRocketLift.Stop();
            }
        }
    }
}
