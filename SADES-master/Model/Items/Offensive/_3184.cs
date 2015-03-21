using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Model.Items
{
    internal class _3184 : Item
    {
        internal override int Id
        {
            get { return 3184; }
        }

        internal override string Name
        {
            get { return "Entropy"; }
        }

        public override void Use()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }


            if (target.IsValidTarget() && ObjectManager.Player.AttackRange <= target.Distance(ObjectManager.Player.Position))
            {
                LeagueSharp.Common.Items.UseItem(Id, ObjectManager.Player);
            }
        }
    }
}
