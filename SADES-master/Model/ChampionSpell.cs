using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Model
{
    internal static class ChampionSpell
    {
        private static String[] supportedChampions =  {"Nocturne", "Sivir", "Morgana"};

        public static Spell usableSpell()
        {
            if (supportedChampions.Contains(ObjectManager.Player.ChampionName))
            {
                switch (ObjectManager.Player.ChampionName)
                {
                    case "Morgana":
                        return new Spell(SpellSlot.E, 750f);
                    case "Nocturne":
                        return new Spell(SpellSlot.W);
                    case "Sivir":
                        return new Spell(SpellSlot.E);
                }
            }
            return null;
        }

        public static bool supportedChamp()
        {
            if (usableSpell() != null)
                return true;

            return false;
        }

    }
}
