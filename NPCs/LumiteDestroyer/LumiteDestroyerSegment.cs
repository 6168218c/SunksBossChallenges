using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SunksBossChallenges.Projectiles.LumiteDestroyer;
using SunksBossChallenges.Projectiles;
using System.IO;
using Terraria.Graphics.Effects;

namespace SunksBossChallenges.NPCs.LumiteDestroyer
{
    public static class LumiteDestroyerArguments
    {
        public static float SpinSpeed
        {
            get
            {
                float Speed = 20f;
                if (Main.expertMode)
                    Speed *= 1.125f;
                //if (Main.getGoodWorld)
                //    Speed *= 1.25f;
                return Speed;
            }
        }
        public static double SpinRadiusSpeed
        {
            get
            {
                double Speed = Math.PI * 2f / 180;
                return Speed;
            }
        }

        public static float R => (float)(SpinSpeed / SpinRadiusSpeed);
        public static float Scale => 1.25f;
        public static float Phase2HealthFactor => 0.75f;
    }
    public abstract class LumiteDestroyerSegment:ModNPC
    {
        // should have used Split,but used it in order not to confuse with SpinAttack
        protected int DivideAttackStart => 10;
        protected int DivideAILength => 5;
        protected int SpinAttackStart => 101;
        /// <summary>
        /// Sync attack state ai,shouldn't be used on head if head.ai[1]!=<see cref="DivideAttackStart"/>+1
        /// </summary>
        protected float SyncAttackState { get => npc.localAI[0]; set => npc.localAI[0] = value; }
        protected float SyncAttackTimer { get => npc.localAI[1]; set => npc.localAI[1] = value; }

        protected void ForeachSegment(Action<NPC,int> actionIdCount)
        {
            int i = npc.whoAmI;
            int counter = 0;
            while (i != -1)
            {
                counter++;
                actionIdCount?.Invoke(Main.npc[i], counter);
                i = (int)Main.npc[i].ai[0];
            }
        }

        public override bool CheckActive()
        {
            return false;
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            base.OnHitByProjectile(projectile, damage, knockback, crit);
            if (!projectile.minion)
            {
                projectile.Kill();
            }
        }
    }
}
