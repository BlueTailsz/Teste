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
    internal static class IgniteHander
    {
        private const int IgniteRange = 600;
        private static SpellSlot _igniteSlot;

        static IgniteHander()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        public static void Load()
        {
            ItemsSubMenu.AddItem(new MenuItem("igniteKill", "Use Ignite/Incendiar if Killable").SetValue(true));
            ItemsSubMenu.AddItem(new MenuItem("igniteMinRange", "Min Range to cast ignite").SetValue(new Slider(400, 0, 600)));
            ItemsSubMenu.AddItem(new MenuItem("igniteKS", "Use Ignite KS").SetValue(true));

            _igniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

        }

        private static Menu ItemsSubMenu
        {
            get { return GameControl.Config.SubMenu("Ignite"); }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var igniteKill = GameControl.Config.Item("igniteKill").GetValue<bool>();
            var igniteKs = GameControl.Config.Item("igniteKS").GetValue<bool>();
            var igniteRange = GameControl.Config.Item("igniteMinRange").GetValue<Slider>().Value;

            if (!_igniteSlot.IsReady())
                return;

            if (igniteKill)
            {
                var igniteKillableEnemy =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(ObjectManager.Player.Position) <= IgniteRange)
                        .FirstOrDefault(
                            x => ObjectManager.Player.GetSummonerSpellDamage(x, Damage.SummonerSpell.Ignite) > x.Health);

                if (igniteKillableEnemy == null)
                    return;

                if (igniteKillableEnemy.Distance(GameControl.MyHero.Position) < igniteRange)
                    return;

                if (igniteKillableEnemy.IsValidTarget())
                    ObjectManager.Player.Spellbook.CastSpell(_igniteSlot, igniteKillableEnemy);
            }

            if (!igniteKs) return;

            var enemy =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(x => x.IsEnemy)
                    .Where(x => x.Distance(ObjectManager.Player.Position) <= IgniteRange)
                    .FirstOrDefault(
                        x => x.Health <= ObjectManager.Player.GetSummonerSpellDamage(x, Damage.SummonerSpell.Ignite) / 5);

            if (enemy.IsValidTarget())
                ObjectManager.Player.Spellbook.CastSpell(_igniteSlot, enemy);
        }
    }
}