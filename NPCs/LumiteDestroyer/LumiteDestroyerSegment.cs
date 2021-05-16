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
        protected int DivideAILength => 8;
        protected int SpinAttackStart => 101;
        protected int DeathStruggleStart => 200;
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

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                return;
            }
            if (npc.realLife != -1 && Main.npc[npc.realLife].type == ModContent.NPCType<LumiteDestroyerHead>())
            {
                if( Main.npc[npc.realLife].ai[1] < DeathStruggleStart + 5)
                {
                    npc.life = 1;
                    return;
                }
            }
            Gore.NewGore(npc.position, npc.velocity, 156);
            if (Main.rand.Next(2) == 0)
            {
                for (int num668 = 0; num668 < 10; num668++)
                {
                    int num669 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                    Dust dust146 = Main.dust[num669];
                    Dust dust2 = dust146;
                    dust2.velocity *= 1.4f;
                }
                for (int num670 = 0; num670 < 5; num670++)
                {
                    int num671 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Fire, 0f, 0f, 100, default(Color), 2.5f);
                    Main.dust[num671].noGravity = true;
                    Dust dust147 = Main.dust[num671];
                    Dust dust2 = dust147;
                    dust2.velocity *= 5f;
                    num671 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                    dust147 = Main.dust[num671];
                    dust2 = dust147;
                    dust2.velocity *= 3f;
                }
                int num672 = Gore.NewGore(new Vector2(npc.position.X, npc.position.Y), default(Vector2), Main.rand.Next(61, 64));
                Gore gore20 = Main.gore[num672];
                Gore gore2 = gore20;
                gore2.velocity *= 0.4f;
                Main.gore[num672].velocity.X += 1f;
                Main.gore[num672].velocity.Y += 1f;
                num672 = Gore.NewGore(new Vector2(npc.position.X, npc.position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore20 = Main.gore[num672];
                gore2 = gore20;
                gore2.velocity *= 0.4f;
                Main.gore[num672].velocity.X -= 1f;
                Main.gore[num672].velocity.Y += 1f;
                num672 = Gore.NewGore(new Vector2(npc.position.X, npc.position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore20 = Main.gore[num672];
                gore2 = gore20;
                gore2.velocity *= 0.4f;
                Main.gore[num672].velocity.X += 1f;
                Main.gore[num672].velocity.Y -= 1f;
                num672 = Gore.NewGore(new Vector2(npc.position.X, npc.position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore20 = Main.gore[num672];
                gore2 = gore20;
                gore2.velocity *= 0.4f;
                Main.gore[num672].velocity.X -= 1f;
                Main.gore[num672].velocity.Y -= 1f;
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
