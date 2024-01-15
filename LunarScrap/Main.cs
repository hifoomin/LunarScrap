using BepInEx;
using BepInEx.Logging;
using LunarScrap.Scrap;
using System.Reflection;
using UnityEngine;

namespace LunarScrap
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class Main : BaseUnityPlugin
    {
        public const string GUID = "HIFU.LunarScrap";
        public const string Name = "LunarScrap";
        public const string Version = "0.0.1";

        public static ManualLogSource LSLogger;

        public static AssetBundle assetBundle;

        private void Awake()
        {
            LSLogger = base.Logger;
            assetBundle = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("LunarScrap.dll", "lunarscrap"));

            Initialize.Init();
        }
    }
}