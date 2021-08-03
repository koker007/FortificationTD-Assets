using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkCTRL : MonoBehaviour
{
    static public NetworkManager networkManager;
    static public Mirror.FizzySteam.FizzySteamworks fizzySteamworks;

    [SerializeField]
    public bool ThisServerBuild = false;

    [SerializeField]
    GameObject _GameplayPrefab;
    GameObject _GameplayObj = null;

    // Start is called before the first frame update
    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        fizzySteamworks = GetComponent<Mirror.FizzySteam.FizzySteamworks>();

    }

    void Update()
    {
        serverTest();
    }

    void serverTest() {
        if (networkManager.isNetworkActive && Player.me && Player.me.isServer && _GameplayObj == null) {
            if (!GameplayCTRL.main && _GameplayPrefab) {
                GameObject gameplayObj = Instantiate(_GameplayPrefab);
                if (gameplayObj) {
                    _GameplayObj = gameplayObj;
                    NetworkServer.Spawn(gameplayObj);
                }
            }
        }
    }

}
