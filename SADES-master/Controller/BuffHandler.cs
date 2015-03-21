using LeagueSharp;
using LeagueSharp.Common;
using SADES.Model;
using SADES.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Controller
{
    internal class BuffHandler
    {
        static Spell spell;

        static String[] ccList = 
        {
            "Blind",                    
            "Charm",
            "CombatDehancer",
            "Fear", 
            "Frenzy",
            "Flee", 
            "Polymorph",
            "Silence", 
            "Slow",
            "Suppresion",
            "Suspension",
            "Snare",
            "Stun",
            "Taunt"
        };


        public static void Load()
        {
            if (ChampionSpell.supportedChamp()) 
            { 
                ItemsSubMenu.AddItem(new MenuItem("useSpell", "Use Shield Spell").SetValue(true));
                ItemsSubMenu.AddItem(new MenuItem("useSpellGC", "Use On Gap Closers").SetValue(true));
            }

            var QSSMenu = new Menu("QSS - CC Type", "QSS");
            {
                QSSMenu.AddItem(new MenuItem("useQSS", "Use QSS").SetValue(true));
                QSSMenu.AddItem(new MenuItem("useQSSCombo", "Only QSS While Combo").SetValue(false));
                QSSMenu.AddItem(new MenuItem("QSSesparator", "        "));
                QSSMenu.AddItem(new MenuItem("qssZed", "Zed Mark").SetValue(false));

                foreach(string s in ccList){
                    QSSMenu.AddItem(new MenuItem("qss"+s, s).SetValue(false));
                }
            }

            ItemsSubMenu.AddSubMenu(QSSMenu);

            var MCMenu = new Menu("Mikael's Crucible - CC Type", "MikaelC");
            {
                MCMenu.AddItem(new MenuItem("useMikaelC", "Use Mikael's Crucible").SetValue(true));
                MCMenu.AddItem(new MenuItem("useMikaelCCombo", "Only Use Mikael's Crucible While Combo").SetValue(false));
                MCMenu.AddItem(new MenuItem("MikaelCseparator", "        "));
                MCMenu.AddItem(new MenuItem("mcZed", "Zed Mark").SetValue(false));

                foreach (string s in ccList)
                {
                    MCMenu.AddItem(new MenuItem("mc" + s, s).SetValue(false));
                }
            }

            ItemsSubMenu.AddSubMenu(MCMenu);

            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static void buffCast()
        {
            spell = ChampionSpell.usableSpell();

            spell.Cast();
        }

        private static Menu ItemsSubMenu
        {
            get { return GameControl.Config.SubMenu("Spell Help"); }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var player = ObjectManager.Player;

            if (MiscControl.GetBool("useQSS"))
            {
                QSS(player);
            }

            if (MiscControl.GetBool("useMikaelC"))
            {
                MC(player);
            }
        }

        static void MC(Obj_AI_Base player)
        {
            if (MiscControl.GetBool("useMikaelC"))
            {
                if (MiscControl.GetBool("useMikaelCCombo") && MiscControl.OrbwalkerMode != Orbwalking.OrbwalkingMode.Combo)
                    return;

                var id = 0;

                if (Items.CanUseItem(3222) && Items.HasItem(3222))
                    id = 3222;

                if (id == 0)
                    return;

                if (MiscControl.GetBool("mcZed"))
                {
                    if (player.HasBuff("zedultexecute", false, true))
                    {
                        LeagueSharp.Common.Items.UseItem(id, ObjectManager.Player);
                    }
                }

                foreach (string s in ccList)
                {
                    if (player.HasBuffOfType((BuffType)Enum.Parse(typeof(BuffType), s)))
                    {
                        if (MiscControl.GetBool("mc" + s))
                        {
                            LeagueSharp.Common.Items.UseItem(id, ObjectManager.Player);
                        }
                    }
                }
            }
        }

        static void QSS(Obj_AI_Base player){

            if (MiscControl.GetBool("useQSS"))
            {
                if (MiscControl.GetBool("useQSSCombo") && MiscControl.OrbwalkerMode != Orbwalking.OrbwalkingMode.Combo)
                    return;

                var id = 0;
                if (Items.CanUseItem(3140) && Items.HasItem(3140))
                    id = 3140;
                if (Items.CanUseItem(3137) && Items.HasItem(3137))
                    id = 3137;
                if (Items.CanUseItem(3139) && Items.HasItem(3139))
                    id = 3139;
                if (id == 0)
                    return;

                if (MiscControl.GetBool("qssZed"))
                {
                    if (player.HasBuff("zedultexecute", false, true))
                    {
                        LeagueSharp.Common.Items.UseItem(id, ObjectManager.Player);
                    }
                }

                foreach (string s in ccList)
                {
                    if (player.HasBuffOfType((BuffType)Enum.Parse(typeof(BuffType), s)))
                    {
                        if (MiscControl.GetBool("qss" + s))
                        {
                            LeagueSharp.Common.Items.UseItem(id, ObjectManager.Player);
                        }
                    }
                }
            }
        }

    }
}
