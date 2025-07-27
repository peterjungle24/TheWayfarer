using UnityEngine;
using BepInEx;
using BepInEx.Logging;

namespace Code
{
    [BepInPlugin(ID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        // cool thing that logs to "mods.log" file (with LogManager)
        public static ManualLogSource logger;
        // needs to match with "modinfo.json" properties
        private const string ID = "mod.the_wayfarer";
        private const string NAME = "The Wayfarer";
        private const string VERSION = "0.1.0";

        public void OnEnable()
        {
            // assign logger to the base logger
            logger = base.Logger;

            On.RainWorld.OnModsInit += Initialize;
        }

        /// <summary>
        /// called when the mods are initializing
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void Initialize(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            // make sure that the code its working
            logger.LogInfo("Wayfare is here! Literally.");

            orig(self);
        }
    }
}