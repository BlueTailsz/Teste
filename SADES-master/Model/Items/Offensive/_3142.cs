using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Model.Items
{
    internal class _3142 : Item
    {
        internal override int Id
        {
            get { return 3142; }
        }

        internal override string Name
        {
            get { return "Youmuu's Ghostblade"; }
        }

        public override void Use()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            var targetHero = (Obj_AI_Hero)target;

            if (targetHero.IsValidTarget())
            {
                LeagueSharp.Common.Items.UseItem(Id, ObjectManager.Player);
            }
        }
    }
}
