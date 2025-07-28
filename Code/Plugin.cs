namespace Code
{
    [BepInPlugin(ID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        // the Wayfare scug id
        public static readonly SlugcatStats.Name slgWayfare = new SlugcatStats.Name("theresAFareOnMyWay");

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

            // removing arm
            On.PlayerGraphics.DrawSprites += RemovesArm;
            On.Player.Update += Player_Update;
            On.Player.Grabability += Player_Grabability;
        }

        private Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (self.slugcatStats.name == slgWayfare)
            {
                PlayerGrab result = orig.Invoke(self, obj);
                result = Grabability(self, result);

                return result;
            }

            return orig(self, obj);
        }
        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.slugcatStats.name == slgWayfare)
            {
                if (self.grasps[0] != null && self.grasps[1] != null)
                {
                    self.ReleaseGrasp(1);
                }
            }

            orig(self, eu);
        }
        private void RemovesArm(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self.player.slugcatStats.name == slgWayfare)
            {
                sLeaser.sprites[6].isVisible = false;
                sLeaser.sprites[8].isVisible = false;
            }
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

        /***********************************************************/
        public Player.ObjectGrabability Grabability(Player self, PlayerGrab result)
        {
            PlayerGrab result2;
            
            result2 = result;
            if (result == PlayerGrab.OneHand) result = PlayerGrab.BigOneHand;
            else
            {
                if (result == PlayerGrab.BigOneHand) result = PlayerGrab.BigOneHand;
                else
                    if (result == PlayerGrab.TwoHands) result = PlayerGrab.Drag;
                    else if (result == PlayerGrab.Drag) result = PlayerGrab.Drag;
            }
            result2 = result;

            return result2;
        }
    }
}