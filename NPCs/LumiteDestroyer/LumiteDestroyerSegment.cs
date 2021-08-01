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
                int iHead = NPC.FindFirstNPC(ModContent.NPCType<LumiteDestroyerHead>());
                if (iHead != -1)
                {
                    NPC head = Main.npc[iHead];
                    if (head.ai[1] >= LumiteDestroyerSegment.DeathStruggleStart)
                    {
                        Speed /= 1.2f;
                    }
                }
                return Speed;
            }
        }

        public static float R => (float)(SpinSpeed / SpinRadiusSpeed);
        public static float Scale => 1.5f;
        public static float Phase2HealthFactor => 0.75f;
        public static int TeleportDistance => 750;
        public static void Blink(this NPC npc,Vector2 dest)
        {
			if(npc.alpha==0)
			{
				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Vortex, 0f, 0f, 100, default, 2f);
					dust.noGravity = true;
					dust.noLight = true;
					dust.color = Color.LightBlue;
				}
			}
            npc.Center = dest;
        }
    }
    public abstract class LumiteDestroyerSegment:ModNPC
    {
        public static int LaserMatrix => 0;
        public static int StarFall => 1;
        public static int PlanetAurora => 2;
        public static int StarCard => 3;
        public static int ChronoDash => 4;
        public static int SigilStar => 5;

        // should have used Split,but used it in order not to confuse with SpinAttack
        public static int DivideAttackStart => 10;
        public static int DivideAILength => 10;
        public static int SpinAttackStart => 101;
        public static int DeathStruggleStart => 200;
        /// <summary>
        /// Sync attack state ai,shouldn't be used on head if head.ai[1]!=<see cref="DivideAttackStart"/>+1
        /// </summary>
        protected float SyncAttackState { get => npc.localAI[0]; set => npc.localAI[0] = value; }
        protected float SyncAttackTimer { get => npc.localAI[1]; set => npc.localAI[1] = value; }

        internal int ImmuneTimer;

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
            if (npc.realLife != -1 && Main.npc[npc.realLife].type == ModContent.NPCType<LumiteDestroyerHead>() && Main.npc[npc.realLife].active)
            {
                if (Main.npc[npc.realLife].ai[1] <= DeathStruggleStart + 5)
                {
                    //prevent from dying
                    npc.life = 1;
                    return;
                }
            }
            else if (npc.realLife != -1 && Main.npc[npc.realLife].type == ModContent.NPCType<LumiteDestroyerHead>())
            {
                //if head is really dead
                if (Main.npc[npc.realLife].ai[1] >= DeathStruggleStart + 5)
                {
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
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ImmuneTimer);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ImmuneTimer = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (ImmuneTimer != 0) damage *= (1 - 0.80);
            if (npc.alpha > 0) damage *= (1 - 0.60);
            return true;
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitByProjectile(projectile, ref damage, ref knockback, ref crit, ref hitDirection);
            /*if (projectile.type == ProjectileID.DD2BetsyArrow)
            {
                if(this is LumiteDestroyerHead head)
                {
                    head.DynDR = Math.Max(head.DynDR, 0.8f);
                }
                else if (Util.CheckNPCAlive<LumiteDestroyerHead>(npc.realLife))
                {
                    var npchead = (Main.npc[npc.realLife].modNPC as LumiteDestroyerHead);
                    npchead.DynDR = Math.Max(npchead.DynDR, 0.8f);
                }
            }*/
            if (!projectile.minion)
            {
                //projectile.penetrate = 0;
                if (projectile.penetrate == -1) damage = (int)(damage * 0.15f);
                else if (projectile.penetrate != 0) damage /= projectile.penetrate;
                if (projectile.type == ProjectileID.DD2BetsyArrow) damage = (int)(damage * 0.6f);
                if (projectile.type == ProjectileID.StardustDragon1) damage = (int)(damage * 0.25f);
                if (projectile.type == ProjectileID.StardustDragon2) damage = (int)(damage * 0.25f);
                if (projectile.type == ProjectileID.StardustDragon3) damage = (int)(damage * 0.25f);
                if (projectile.type == ProjectileID.StardustDragon4) damage = (int)(damage * 0.25f);
            }
        }
    }
}
