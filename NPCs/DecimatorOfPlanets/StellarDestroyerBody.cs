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
    public class StellarDestroyerBody:ModNPC
    {
        readonly int segDistance = 44;
        int PreviousIndex => (int)npc.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Decimator of Planets");
            NPCID.Sets.TrailingMode[npc.type] = 3;
            NPCID.Sets.TrailCacheLength[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.boss = true;
            npc.aiStyle = -1;
            npc.npcSlots = 1f;
            npc.width = npc.height = 38;
            npc.defense = 0;
            npc.damage = 60;
            npc.lifeMax = 120000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 0f;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.alpha = 255;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            music = MusicID.Boss3;
            musicPriority = MusicPriority.BossMedium;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 0.8f);
        }
        public override void AI()
        {
            if (npc.ai[3] > 0f)//life is more than 0
                npc.realLife = (int)npc.ai[3];

            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
                npc.TargetClosest();

            bool dead = false;
            if (npc.ai[1] <= 0f)
            {
                dead = true;
            }
            else if (Main.npc[(int)npc.ai[1]].life <= 0 || !Main.npc[(int)npc.ai[1]].active)
            {
                dead = true;
            }
            if (dead)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
            }

            NPC previousSegment = Main.npc[PreviousIndex];
            NPC head = Main.npc[npc.realLife];

            if (previousSegment.alpha < 128)
            {
                if (npc.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 229, 0f, 0f, 100, default, 2f);
                        dust.noGravity = true;
                        dust.noLight = true;
                        dust.color = Color.LightBlue;
                    }
                }
                npc.alpha -= 42;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }

            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            { 
                if (npc.Distance(previousSegment.Center) > 6)
                {
                    Vector2 offset = new Vector2(0, 1f);
                    try//default behavior
                    {
                        offset = previousSegment.Center - npc.Center;
                    }
                    catch { }
                    if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                    var dist = segDistance * npc.scale;
                    npc.rotation = offset.ToRotation() + (float)(Math.PI / 2) * npc.direction;
                    offset -= Vector2.Normalize(offset) * dist;
                    npc.velocity = Vector2.Zero;
                    npc.position += offset;
                }
            }

            npc.localAI[0] += Main.rand.Next(4);
            if (npc.localAI[0] >= (float)Main.rand.Next(1400, 26000))
            {
                npc.localAI[0] = 0f;
                npc.TargetClosest();
                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                    float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                    float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                    float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
                    num8 = 8f / num8;
                    num6 *= num8;
                    num7 *= num8;
                    num6 += (float)Main.rand.Next(-20, 21) * 0.05f;
                    num7 += (float)Main.rand.Next(-20, 21) * 0.05f;
                    int num9 = 22;
                    if (Main.expertMode)
                    {
                        num9 = 18;
                    }
                    int num10 = ModContent.ProjectileType<DarkStar>();
                    vector.X += num6 * 5f;
                    vector.Y += num7 * 5f;
                    int num11 = Projectile.NewProjectile(vector.X, vector.Y, num6, num7, num10, num9, 0f, Main.myPlayer);
                    Main.projectile[num11].timeLeft = 300;
                    npc.netUpdate = true;
                }
            }
            Lighting.AddLight(npc.Center, 0.3f, 0.3f, 0.5f);
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
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
    }
}
