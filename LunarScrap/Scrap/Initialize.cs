using UnityEngine;

namespace LunarScrap.Scrap
{
    public static class Initialize
    {
        public static int girlIndex = -1;
        public static int eyelessDogIndex = -1;

        public static void Init()
        {
            On.QuickMenuManager.Start += QuickMenuManager_Start;
            InactiveLandmine.Init();
            DogBone.Init();
        }

        /*
         * [Error  :LunarScrap] enemy name is Flowerman
[Error  :LunarScrap] enemy name is Crawler
[Error  :LunarScrap] enemy name is Hoarding bug
[Error  :LunarScrap] enemy name is Centipede
[Error  :LunarScrap] enemy name is Bunker Spider
[Error  :LunarScrap] enemy name is Puffer
[Error  :LunarScrap] enemy name is Jester
[Error  :LunarScrap] enemy name is Jester
[Error  :LunarScrap] enemy name is Blob
[Error  :LunarScrap] enemy name is Girl
[Error  :LunarScrap] DAYTIME enemy name is Red Locust Bees
[Error  :LunarScrap] DAYTIME enemy name is Docile Locust Bees
[Error  :LunarScrap] DAYTIME enemy name is Manticoil
[Error  :LunarScrap] OUTSIDE enemy name is MouthDog
[Error  :LunarScrap] OUTSIDE enemy name is Earth Leviathan
[Error  :LunarScrap] OUTSIDE enemy name is ForestGiant
[Error  :LunarScrap] OUTSIDE enemy name is Baboon hawk

        */

        private static void QuickMenuManager_Start(On.QuickMenuManager.orig_Start orig, QuickMenuManager self)
        {
            orig(self);
            var level = self.testAllEnemiesLevel;
            for (int i = 0; i < level.Enemies.Count; i++)
            {
                var enemy = level.Enemies[i];
                var enemyType = enemy.enemyType;
                if (enemyType)
                {
                    var enemyName = enemyType.enemyName;
                    // Main.LSLogger.LogError("enemy name is " + enemyName);
                    if (enemyName == "Girl")
                    {
                        girlIndex = i;
                        break;
                    }
                }
            }
            /*
            for (int i = 0; i < level.DaytimeEnemies.Count; i++)
            {
                var enemy = level.DaytimeEnemies[i];
                var enemyType = enemy.enemyType;
                if (enemyType)
                {
                    var enemyName = enemyType.enemyName;
                    Main.LSLogger.LogError("DAYTIME enemy name is " + enemyName);
                }
            }
            */

            for (int i = 0; i < level.OutsideEnemies.Count; i++)
            {
                var enemy = level.OutsideEnemies[i];
                var enemyType = enemy.enemyType;
                if (enemyType)
                {
                    var enemyName = enemyType.enemyName;
                    if (enemyName == "MouthDog")
                    {
                        eyelessDogIndex = i;
                        break;
                    }
                }
            }
        }

        /*
        [Error  :LunarScrap] enemy name is Centipede
        [Error  :LunarScrap] enemy name is Bunker Spider
        [Error  :LunarScrap] enemy name is Hoarding bug
        [Error  :LunarScrap] enemy name is Flowerman
        [Error  :LunarScrap] enemy name is Crawler
        [Error  :LunarScrap] enemy name is Blob
        [Error  :LunarScrap] enemy name is Girl
        [Error  :LunarScrap] enemy name is Puffer
        [Error  :LunarScrap] enemy name is Nutcracker
        */
    }
}