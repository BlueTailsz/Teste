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
    internal class SmiteHandler
    {
        private const int smiteRange = 700;
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static Spell smite;

        static SmiteHandler()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        public static void Load()
        {
            ItemsSubMenu.AddItem(new MenuItem("smiteCombo", "Use in Combo").SetValue(true));
            ItemsSubMenu.AddItem(new MenuItem("smiteKill", "Smite Only if Kills").SetValue(true));
            ItemsSubMenu.AddItem(new MenuItem("smiteKS", "Use Smite KS").SetValue(true));
        }

        private static Menu ItemsSubMenu
        {
            get { return GameControl.Config.SubMenu("Smite"); }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!smitetype().Equals("s5_summonersmiteplayerganker") || !smitetype().Equals("s5_summonersmiteduel") || !smiteSlot.IsReady())
                return;

            var smiteKill = GameControl.Config.Item("smiteKill").GetValue<bool>();
            var smiteCombo = GameControl.Config.Item("smiteCombo").GetValue<bool>();
            var smiteKs = GameControl.Config.Item("smiteKS").GetValue<bool>();

            if (smiteKs)
            {
                var smiteKillableEnemy =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(ObjectManager.Player.Position) <= smiteRange)
                        .FirstOrDefault(
                            x => getSmiteDamage() > x.Health);

                if(smiteKillableEnemy != null && MiscControl.isValidTarget(smiteKillableEnemy) && smiteKillableEnemy.Distance(ObjectManager.Player.Position) < smiteRange )
                    ObjectManager.Player.Spellbook.CastSpell(smiteSlot, smiteKillableEnemy);
            }

            if (smiteCombo)
            {
                var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.True);

                if (MiscControl.OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if(smiteKill){
                        if(target.Health < getSmiteDamage())
                            ObjectManager.Player.Spellbook.CastSpell(smiteSlot, target);
                    }
                    else
                    {
                        if (target.Distance(ObjectManager.Player.Position) > ObjectManager.Player.AttackRange)
                            ObjectManager.Player.Spellbook.CastSpell(smiteSlot, target);
                    }
                }
            }
        }

        //Start Credits to Kurisu
        public static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        public static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        public static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };

        public static string smitetype()
        {
            if (SmiteBlue.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(id => Items.HasItem(id)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }
        //End credits


        public static void setSmiteSlot()
        {
            foreach (var spell in ObjectManager.Player.Spellbook.Spells.Where(spell => String.Equals(spell.Name, smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {
                smiteSlot = spell.Slot;
                smite = new Spell(smiteSlot, 700);
                return;
            }
        }

        public static float getSmiteDamage()
        {
            switch (smitetype())
            {
                case "s5_summonersmiteplayerganker":
                    return getBlueSmiteDamage();
                case "s5_summonersmiteduel":
                    return getRedSmiteDamage();
            }
            return 0f;
        }

        public static float getBlueSmiteDamage()
        {
            return 20 + 8 * ObjectManager.Player.Level;
        }

        public static float getRedSmiteDamage()
        {
            return 54 + 6 * ObjectManager.Player.Level;
        }
    }
}