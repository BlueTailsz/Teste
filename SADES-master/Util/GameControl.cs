using LeagueSharp;
using LeagueSharp.Common;
using SADES.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Util
{
    class GameControl
    {
        public static String version = "2 Revision 1";
        public static Obj_AI_Hero MyHero = ObjectManager.Player;

        internal static Menu Config;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static Obj_AI_Hero Player;

        internal static void Load()
        {
            try
            {
                Player = ObjectManager.Player;

                Config = new Menu("SADES", "SADES", true);

                BuffHandler.Load();
                IgniteHander.Load();
                SmiteHandler.Load();
                PotionHandler.Load();
                ItemHandler.Load();
                RecallHandler.Load();

                Config.AddToMainMenu();
            }
            catch (Exception e)
            {
            }
        }
    }
}
