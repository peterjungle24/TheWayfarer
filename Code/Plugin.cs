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
            On.Player.FreeHand += Player_FreeHand;
        }

        private int Player_FreeHand(On.Player.orig_FreeHand orig, Player self)
        {
            int result = orig.Invoke(self);

            if (self.grasps[0] != null || self.grasps[1] != null)
            {
                result = -1;
            }

            return result;
        }

        #region One Arm

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
            /*
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self.player.slugcatStats.name == slgWayfare)
            {
                // Hand 1
                sLeaser.sprites[5].isVisible = false;
                sLeaser.sprites[7].isVisible = false;

                // Hand 2
                sLeaser.sprites[6].isVisible = false;
                sLeaser.sprites[8].isVisible = false;
            }
            */

            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            
            DrawSpritesHandler(self.player, sLeaser, rCam, timeStacker, camPos);
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
        public void DrawSpritesHandler(Player player, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (player.graphicsModule != null || sLeaser != null)
            {
                PlayerGraphics self = player.graphicsModule as PlayerGraphics;
                int grasp = 0;

                if (player.grasps[1] != null) grasp = 1;
                if (sLeaser.sprites.Length >= 9)
                {
                    for (int i = 5; i <= 8; i++)
                        if (i == 6 - grasp || i == 8 - grasp)
                            sLeaser.sprites[i].isVisible = false;
                }
            }
        }

        #endregion

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