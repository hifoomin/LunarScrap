using System.Collections;
using UnityEngine;

namespace LunarScrap.Scrap
{
    public static class InactiveLandmine
    {
        public static Item inactiveLandmine;
        public static GameObject idleSFX;
        public static GameObject primeSFX;
        public static GameObject explosionSFX;
        public static GameObject vfx;

        public static void Init()
        {
            primeSFX = Main.assetBundle.LoadAsset<GameObject>("InactiveLandminePrimeSound.prefab");
            explosionSFX = Main.assetBundle.LoadAsset<GameObject>("InactiveLandmineExplosionSound.prefab");
            idleSFX = Main.assetBundle.LoadAsset<GameObject>("InactiveLandmineIdleSound.prefab");

            // vfx = Main.assetBundle.LoadAsset<GameObject>("InactiveLandmineEffect.prefab");

            inactiveLandmine = Utils.CreateScrap("InactiveLandmine", 70, true, idleSFX);

            On.GameNetcodeStuff.PlayerControllerB.DamagePlayer += PlayerControllerB_DamagePlayer;
        }

        private static void PlayerControllerB_DamagePlayer(On.GameNetcodeStuff.PlayerControllerB.orig_DamagePlayer orig, GameNetcodeStuff.PlayerControllerB self, int damageNumber, bool hasDamageSFX, bool callRPC, CauseOfDeath causeOfDeath, int deathAnimation, bool fallDamage, Vector3 force)
        {
            orig(self, damageNumber, hasDamageSFX, callRPC, causeOfDeath, deathAnimation, fallDamage, force);
            if (damageNumber > 0 && self.currentlyHeldObjectServer && self.currentlyHeldObjectServer.itemProperties == inactiveLandmine)
            {
                self.StartCoroutine(Explode(self));
                self.currentlyHeldObjectServer = null;
                return;
            }
        }

        private static IEnumerator Explode(GameNetcodeStuff.PlayerControllerB self)
        {
            Utils.Shake(ScreenShakeType.VeryStrong);

            var duration = Utils.PlaySound(primeSFX, self.gameObject);
            yield return new WaitForSeconds(duration);

            self.DamagePlayer(int.MaxValue, true, true, CauseOfDeath.Blast, 0, false, Vector3.one * 5f);
            Utils.PlaySound(explosionSFX, self.gameObject);
            // Utils.SpawnEffect(vfx, 3f, self.gameObject);
        }
    }
}