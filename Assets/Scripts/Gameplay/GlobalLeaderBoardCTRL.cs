using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLeaderBoardCTRL : MonoBehaviour
{
    public static GlobalLeaderBoardCTRL main;
    public const string keySeedKill = "SeedKill:";

    public SteamLeaderboard.Leaderboard SeedsVerified = new SteamLeaderboard.Leaderboard("!SeedsVerified", true, 99999); //Таблица используемая для сохранения результатов
    public SteamLeaderboard.Leaderboard SeedsFavorite = new SteamLeaderboard.Leaderboard("!SeedsFavorite", true, 99999); //Таблица для записи понравившихся карт игрокам
    public SteamLeaderboard.Leaderboard SeedsPopular = new SteamLeaderboard.Leaderboard("!SeedsPopular", true, 99999); //таблица для записи популярных карт

    public SteamLeaderboard.Leaderboard ThisSeedMap;

    public void TestLoadingSeeds() {
        SeedsVerified.UpdateLeadersBoard();
        SeedsFavorite.UpdateLeadersBoard();
        SeedsPopular.UpdateLeadersBoard();
    }

    //Проверка и загрузка текущей карты
    void TestLeaderBoardThisSeed() {
        if (GameplayCTRL.main != null && GameplayCTRL.main.KeyGen != "" && GameplayCTRL.main.KeyGen != null) {
            string keySeedNow = keySeedKill + GameplayCTRL.main.KeyGen;
            if (ThisSeedMap == null)
                ThisSeedMap = new SteamLeaderboard.Leaderboard(keySeedNow, false, 3);

            //Пересоздаем таблицу для новой карты
            if (ThisSeedMap.Key != keySeedNow) {
                ThisSeedMap = new SteamLeaderboard.Leaderboard(keySeedNow, false, 3);
            }

            if (ThisSeedMap != null) {
                ThisSeedMap.UpdateLeadersBoard();
            }
        }
    }

    void Start()
    {
        main = this;    
    }

    // Update is called once per frame
    void Update()
    {
        TestLoadingSeeds();
        TestLeaderBoardThisSeed();
    }
}
