using LeagueSharp;
using LeagueSharp.Common;
using SADES.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Model.Items
{
    internal class _3128 : Item
    {
        internal override int Id
        {
            get { return 3128; }
        }

        internal override string Name
        {
            get { return "Deathfire Grasp"; }
        }

        internal override float Range
        {
            get { return 750; }
        }

        public override void Use()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            if (MiscControl.isValidTarget(target))
            {
                LeagueSharp.Common.Items.UseItem(Id, target);
            }
        }
    }
}
