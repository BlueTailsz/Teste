using LeagueSharp;
using LeagueSharp.Common;
using SADES.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Controller
{
    internal static class PotionHandler
    {
        private const string HpPotName = "RegenerationPotion";
        private const string MpPotName = "FlaskOfCrystalWater";
        private const int Hpid = 2003;
        private const int Mpid = 2004;

        static PotionHandler()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        public static void Load()
        {
            ItemsSubMenu.AddItem(new MenuItem("useHP", "Use Health Pot").SetValue(true));
            ItemsSubMenu.AddItem(new MenuItem("useHPPercent", "Health %").SetValue(new Slider(35, 1)));
            ItemsSubMenu.AddItem(new MenuItem("sseperator", "       "));
            ItemsSubMenu.AddItem(new MenuItem("useMP", "Use Mana Pot").SetValue(true));
            ItemsSubMenu.AddItem(new MenuItem("useMPPercent", "Mana %").SetValue(new Slider(35, 1)));

        }

        private static Menu ItemsSubMenu
        {
            get { return GameControl.Config.SubMenu("Potions"); }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var useHp = GameControl.Config.Item("useHP").GetValue<bool>();
            var useMp = GameControl.Config.Item("useMP").GetValue<bool>();

            if (useHp && ObjectManager.Player.HealthPercentage() <= GameControl.Config.Item("useHPPercent").GetValue<Slider>().Value &&
                !HasHealthPotBuff())
            {
                if (Items.CanUseItem(Hpid) && Items.HasItem(Hpid))
                {
                    Items.UseItem(Hpid);
                }
            }

            if (!useMp ||
                !(ObjectManager.Player.ManaPercentage() <= GameControl.Config.Item("useMPPercent").GetValue<Slider>().Value) ||
                HasMannaPutBuff()) return;

            if (Items.CanUseItem(Mpid) && Items.HasItem(Mpid))
            {
                Items.UseItem(Mpid);
            }
        }

        private static bool HasHealthPotBuff()
        {
            return ObjectManager.Player.HasBuff(HpPotName, true);
        }

        private static bool HasMannaPutBuff()
        {
            return ObjectManager.Player.HasBuff(MpPotName, true);
        }
    }
}