using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public static ButtonSounds main;

    [SerializeField]
    float volumeButtons = 1;

    [SerializeField]
    AudioSource ASButton;
    [SerializeField]
    AudioSource ASSong;

    [SerializeField]
    AudioClip ACSelect;
    [SerializeField]
    AudioClip ACDown;
    [SerializeField]
    AudioClip ACUp;

    [SerializeField]
    AudioClip ACLobbyFound;
    [SerializeField]
    AudioClip ACPlayerConnected;
    [SerializeField]
    AudioClip ACPlayerDisconnect;


    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestPlayersCount();
    }

    int countPlayersOld = 0;
    bool GameplayOld = false;
    void TestPlayersCount() {
        if (GameplayCTRL.main)
        {
            if (!GameplayOld)
            {
                GameplayOld = true;
            }
            else {
                if (countPlayersOld != GameplayCTRL.main.players.Count) {
                    if (countPlayersOld < GameplayCTRL.main.players.Count) {
                        PlayerConnected();
                    }
                    else {
                        PlayerDisconneted();
                    }

                    countPlayersOld = GameplayCTRL.main.players.Count;
                }
            }
        }
        else {
            GameplayOld = false;
            countPlayersOld = 0;
        }
    }

    public static void Down() {
        if (main && main.ASButton && main.ACDown && Setings.main && Setings.main.game != null) {
            main.ASButton.pitch = Random.Range(0.95f, 1.05f);
            main.ASButton.volume = Setings.main.game.volume_all * Setings.main.game.volume_sound * main.volumeButtons;
            main.ASButton.PlayOneShot(main.ACDown);
        }
    }
    public static void Up()
    {
        if (main && main.ASButton && main.ACUp && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = Random.Range(0.95f, 1.05f);
            main.ASButton.volume = Setings.main.game.volume_all * Setings.main.game.volume_sound * main.volumeButtons;
            main.ASButton.PlayOneShot(main.ACUp);
        }
    }
    public static void Select() {
        if (main && main.ASButton && main.ACSelect && Setings.main && Setings.main.game != null) {
            main.ASButton.pitch = Random.Range(0.95f, 1.05f);
            main.ASButton.volume = Setings.main.game.volume_all * Setings.main.game.volume_sound * main.volumeButtons;
            main.ASButton.PlayOneShot(main.ACSelect);
        }
    }

    public static void LobbyFound() {
        if (main && main.ASButton && main.ACLobbyFound && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = Random.Range(0.95f, 1.05f);
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons;
            main.ASButton.PlayOneShot(main.ACLobbyFound);
        }
    }

    public static void PlayerConnected() {
        if (main && main.ASButton && main.ACPlayerConnected && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = Random.Range(0.95f, 1.05f);
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons;
            main.ASButton.PlayOneShot(main.ACPlayerConnected);
        }
    }
    public static void PlayerDisconneted() {
        if (main && main.ASButton && main.ACPlayerDisconnect && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = Random.Range(0.95f, 1.05f);
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons;
            main.ASButton.PlayOneShot(main.ACPlayerDisconnect);
        }
    }


    [SerializeField]
    AudioClip Defeat;
    [SerializeField]
    AudioClip DefeatSong;
    [SerializeField]
    AudioClip Victory;
    [SerializeField]
    AudioClip VictorySong;

    [SerializeField]
    AudioClip[] ACTechs;
    [SerializeField]
    AudioClip[] ACBuilds;
    [SerializeField]
    AudioClip[] ACDestroy;

    [SerializeField]
    AudioClip[] ACNotAccept;

    public static void GameDefeated() {
        if (main && main.ASButton && main.Defeat && main.DefeatSong && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = 1;
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons;
            main.ASButton.PlayOneShot(main.Defeat);

            if (main.ASSong && main.DefeatSong) {
                main.ASSong.pitch = 1;
                main.ASSong.volume = Setings.main.game.volume_all * main.volumeButtons * Setings.main.game.volume_music;
                main.ASSong.PlayOneShot(main.DefeatSong);
            }
        }
    }
    public static void GameVictory() {
        if (main && main.ASButton && main.Victory && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = 1;
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons;
            main.ASButton.PlayOneShot(main.Victory);

            if (main.ASSong && main.VictorySong)
            {
                main.ASSong.pitch = 1;
                main.ASSong.volume = Setings.main.game.volume_all * main.volumeButtons * Setings.main.game.volume_music;
                main.ASSong.PlayOneShot(main.VictorySong);
            }
        }
    }

    public static void SelectTech() {
        if (main && main.ASButton && main.ACTechs.Length > 0 && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = 1;
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons * Setings.main.game.volume_sound;
            main.ASButton.PlayOneShot(main.ACTechs[Random.Range(0, main.ACTechs.Length)]);
        }
    }

    public static void SelectBuild() {
        if (main && main.ASButton && main.ACBuilds.Length > 0 && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = Random.Range(0.95f, 1.05f);
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons * Setings.main.game.volume_sound;
            main.ASButton.PlayOneShot(main.ACBuilds[Random.Range(0, main.ACBuilds.Length)]);
        }
    }

    public static void SelectDestroy() {
        if (main && main.ASButton && main.ACDestroy.Length > 0 && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = Random.Range(0.95f, 1.05f);
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons * Setings.main.game.volume_sound;
            main.ASButton.PlayOneShot(main.ACDestroy[Random.Range(0, main.ACDestroy.Length)]);
        }
    }

    public static void PlayNotAccept()
    {
        if (main && main.ASButton && main.ACNotAccept.Length > 0 && Setings.main && Setings.main.game != null)
        {
            main.ASButton.pitch = 1;
            main.ASButton.volume = Setings.main.game.volume_all * main.volumeButtons * Setings.main.game.volume_sound;
            main.ASButton.PlayOneShot(main.ACNotAccept[Random.Range(0, main.ACNotAccept.Length)]);
        }
    }
}
