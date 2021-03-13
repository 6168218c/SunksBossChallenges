using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.Projectiles.DecimatorOfPlanets;
namespace SunksBossChallenges.NPCs.DecimatorOfPlanets
{
    public class StellarDestroyerHead:ModNPC
    {
        int Length => 80;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Decimator of Planets");
            NPCID.Sets.TrailingMode[npc.type] = 3;
            NPCID.Sets.TrailCacheLength[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.boss = true;
            npc.npcSlots = 1f;
            npc.width = npc.height = 50;
            npc.defense = 0;
            npc.damage = 100;
            npc.lifeMax = 120000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 0f;
            npc.netAlways = true;
            npc.alpha = 255;
            npc.scale = 1.2f;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            music = MusicID.Boss3;
            musicPriority = MusicPriority.BossMedium;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax / 2 * bossLifeScale);
        }

        public override void AI()
        {
            if (NPC.FindFirstNPC(ModContent.NPCType<StellarDestroyerHead>()) != npc.whoAmI)
            {
                npc.active = false;
                return;
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
                if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
                {
                    npc.velocity = new Vector2(0, -18f);
                    npc.rotation = npc.velocity.ToRotation() + (float)(Math.PI / 2) * npc.direction;
                    if (npc.target >= 0 && npc.target < 255)
                        if (npc.Distance(Main.player[npc.target].position) > 3000f)
                        {
                            npc.active = false;
                        }
                    return;
                }
            }

            Player player = Main.player[npc.target];
            if (player.immuneTime > 50)
            {
                player.immuneTime = 50;
            }
            if (npc.alpha != 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 229, 0f, 0f, 100, default, 2f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].noLight = true;
                    Main.dust[num].color = Color.LightBlue;
                }
            }
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }

            if (npc.ai[0] == 0f)
            {
                int previous = npc.whoAmI;
                for (int j = 1; j <= Length; j++)
                {
                    int npcType = ModContent.NPCType<StellarDestroyerBody>();
                    if (j == Length)
                    {
                        npcType = ModContent.NPCType<StellarDestroyerTail>();
                    }
                    int current = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, npcType, npc.whoAmI);
                    Main.npc[current].ai[3] = npc.whoAmI;
                    Main.npc[current].realLife = npc.whoAmI;
                    Main.npc[current].ai[1] = previous;
                    Main.npc[previous].ai[0] = current;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, current);

                    previous = current;
                }

                if (!NPC.AnyNPCs(ModContent.NPCType<StellarDestroyerTail>()))
                {
                    npc.active = false;
                    return;
                }
            }

            var maxSpeed = 18f;
            if (Main.expertMode)
                maxSpeed *= 1.25f;

            WormMovement(Main.player[npc.target].Center, maxSpeed);
            npc.rotation = npc.velocity.ToRotation() + (float)(Math.PI / 2) * npc.direction;
            Lighting.AddLight(npc.Center, 0.3f, 0.3f, 0.5f);
        }

        protected void WormMovement(Vector2 position, float maxSpeed, float turnAccle = 0.1f, float ramAccle = 0.15f)
        {
            Vector2 targetVector = position - npc.Center;
            targetVector = Vector2.Normalize(targetVector) * maxSpeed;
            if ((targetVector.X * npc.velocity.X > 0f) && (targetVector.Y * npc.velocity.Y > 0f)) //acclerate
            {
                npc.velocity.X += Math.Sign(targetVector.X - npc.velocity.X) * ramAccle;
                npc.velocity.Y += Math.Sign(targetVector.Y - npc.velocity.Y) * ramAccle;
            }
            if ((targetVector.X * npc.velocity.X > 0f) || (targetVector.Y * npc.velocity.Y > 0f)) //turn
            {
                npc.velocity.X += Math.Sign(targetVector.X - npc.velocity.X) * turnAccle;
                npc.velocity.Y += Math.Sign(targetVector.Y - npc.velocity.Y) * turnAccle;

                if (Math.Abs(targetVector.Y) < maxSpeed * 0.2 && targetVector.X * npc.velocity.X < 0)
                {
                    npc.velocity.Y += Math.Sign(npc.velocity.Y) * turnAccle * 2f;
                }

                if (Math.Abs(targetVector.X) < maxSpeed * 0.2 && targetVector.Y * npc.velocity.Y < 0)
                {
                    npc.velocity.X += Math.Sign(npc.velocity.X) * turnAccle * 2f;
                }
            }
            else if (Math.Abs(targetVector.X) > Math.Abs(targetVector.Y))
            {
                npc.velocity.X += Math.Sign(targetVector.X - npc.velocity.X) * turnAccle * 1.1f;
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5)
                {
                    npc.velocity.Y += Math.Sign(npc.velocity.Y) * turnAccle;
                }
            }
            else
            {
                npc.velocity.Y += Math.Sign(targetVector.Y - npc.velocity.Y) * turnAccle * 1.1f;
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5)
                {
                    npc.velocity.X += Math.Sign(npc.velocity.X) * turnAccle;
                }
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (npc.alpha > 0)
                return false;
            return null;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.alpha > 0)
                damage *= (1 - 0.99);
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<FinalPhaseSummoner>());
            return true;
        }
    }
}
