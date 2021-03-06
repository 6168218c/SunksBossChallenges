using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SunksBossChallenges.NPCs.DecimatorOfPlanets
{
    public class DecimatorOfPlanetsBody:ModNPC
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
            npc.damage = 80;
            npc.lifeMax = 120000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 0f;
            npc.scale = 1f;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.alpha = 255;
            npc.scale = 1.2f;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void AI()
        {
            ///shared contents:
            ///  ai[0]:rear segment
            ///  ai[1]:previous segment
            ///  ai[2]:mode/state
            ///     0 - normal,11 - spin setup, 12 - spinning
            ///  ai[3]:head
            ///
            if (npc.ai[3] > 0f)//life is more than 0
                npc.realLife = (int)npc.ai[3];

            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
                npc.TargetClosest();

            bool dead = false;
            if (npc.ai[1] <= 0f)
            {
                dead = true;
            }
            else if (Main.npc[(int)npc.ai[1]].life <= 0 && !Main.npc[(int)npc.ai[1]].active)
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
                if (npc.ai[2] == 12)//spinning
                {
                    npc.velocity.RotatedBy(DecimatorOfPlanetsArguments.SpinRadiusSpeed);
                }
                else
                {

                    //previous segment has completed setting up,time to setup itself
                    if (npc.ai[2] == 11 && previousSegment.ai[2] == 12)
                    {
                        npc.ai[2] = 12;
                        var posx = npc.localAI[2] = previousSegment.localAI[2];
                        var posy = npc.localAI[3] = previousSegment.localAI[3];
                        int direction = (int)previousSegment.localAI[4];
                        npc.localAI[4] = previousSegment.localAI[4];

                        Vector2 spinCenter = new Vector2(posx, posy);
                        Vector2 center2Prev = previousSegment.Center - spinCenter;
                        var len = center2Prev.Length();
                        var dist = segDistance * npc.scale;
                        var angle = Math.Acos((2 * len * len - dist * dist) / (2 * len * len));
                        Vector2 center2Cur = center2Prev.RotatedBy(angle * direction);
                        npc.position = center2Cur + spinCenter;
                        npc.rotation = (center2Prev - center2Cur).ToRotation();
                        npc.velocity = Vector2.Normalize(center2Prev - center2Cur) * DecimatorOfPlanetsArguments.SpinSpeed;
                    }
                    else//not preparing spinning,meaning everything is in control
                    {
                        if (previousSegment.ai[2] > 10)//not in normal state
                        {
                            npc.ai[2] = 11;//start to setup,wait for previous segment to complete
                        }
                        else
                        {
                            npc.ai[2] = 0f;
                        }

                        if (npc.Distance(previousSegment.Center) > 6) 
                        {
                            try//default behavior
                            {
                                Vector2 offset = previousSegment.Center - npc.Center;
                                if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                                var dist = segDistance * npc.scale;
                                npc.rotation = offset.ToRotation() - (float)(Math.PI / 2) * npc.direction;
                                offset -= Vector2.Normalize(offset) * dist;
                                npc.velocity = Vector2.Zero;
                                npc.position += offset;
                            }
                            catch { }
                        }
                    }


                }
            }   

            /*if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }

            bool dead = false;
            if (npc.ai[1] <= 0f)
            {
                dead = true;
            }
            else if (Main.npc[(int)npc.ai[1]].life <= 0 && !Main.npc[(int)npc.ai[1]].active)
            {
                dead = true;
            }
            if (dead)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.checkDead();
            }
            NPC head = Main.npc[(int)npc.ai[3]];
            NPC PreviousSeg = Main.npc[(int)npc.ai[1]];

            if (PreviousSeg.alpha < 128)
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
                    if (npc.Distance(PreviousSeg.Center) > 5)
                    {
                        Vector2 SegVect = Vector2.Zero;
                        try
                        {
                            SegVect = PreviousSeg.Center - npc.Center;
                        }
                        catch
                        {
                        }
                        if (SegVect == Vector2.Zero || SegVect.HasNaNs()) SegVect = new Vector2(0, 1f);
                        npc.rotation = SegVect.ToRotation();
                        int dist = (int)(segDistance * npc.scale);
                        SegVect -= dist * Vector2.Normalize(SegVect);
                        npc.velocity = Vector2.Zero;
                        npc.position += SegVect;
                    }

                }*/

        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (npc.alpha > 0)
                return false;
            return null;
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage = (int)(damage * (1 - 0.60));
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * (1 - 0.60));
        }
    }
}
