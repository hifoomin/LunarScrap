using UnityEngine;

namespace LunarScrap.Scrap
{
    public static class DogBone
    {
        public static Item dogBone;

        public static void Init()
        {
            dogBone = Utils.CreateScrap("DogBone", 70, false);
            On.GameNetcodeStuff.PlayerControllerB.BeginGrabObject += PlayerControllerB_BeginGrabObject;
            dogBone.spawnPrefab.AddComponent<DogBoneController>();
        }

        private static void PlayerControllerB_BeginGrabObject(On.GameNetcodeStuff.PlayerControllerB.orig_BeginGrabObject orig, GameNetcodeStuff.PlayerControllerB self)
        {
            orig(self);

            if (self.currentlyGrabbingObject && self.currentlyGrabbingObject.itemProperties == dogBone)
            {
                Main.LSLogger.LogError("currently grabbing object is " + self.currentlyGrabbingObject);
                Main.LSLogger.LogError("dogbone spawn prefab is " + dogBone.spawnPrefab);

                var dogBoneController = self.currentlyGrabbingObject.GetComponent<DogBoneController>();
                Main.LSLogger.LogError("currently garbbing object dog bone controller is " + dogBoneController);

                if (dogBoneController && !dogBoneController.hasPickedUp)
                {
                    // Utils.SpawnEnemyIgnoreCap(Initialize.eyelessDogIndex, false);
                    Utils.SpawnEnemyIgnoreCap(-1, false);
                    dogBoneController.hasPickedUp = true;
                    // loses its properties on pickup unfortunately, this lags
                }
            }
        }
    }

    public class DogBoneController : MonoBehaviour
    {
        public bool hasPickedUp = false;
    }
}