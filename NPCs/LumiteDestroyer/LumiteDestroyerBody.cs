using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.Projectiles.DecimatorOfPlanets;
using SunksBossChallenges.Projectiles;

namespace SunksBossChallenges.NPCs.LumiteDestroyer
{
    public class LumiteDestroyerBody : LumiteDestroyerSegment
    {
        readonly int segDistance = 44;
        int PreviousIndex => (int)npc.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LD-002 \"Destruction\"");
            NPCID.Sets.TrailingMode[npc.type] = 3;
            NPCID.Sets.TrailCacheLength[npc.type] = 2;
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.boss = true;
            npc.aiStyle = -1;
            npc.npcSlots = 1f;
            npc.width = npc.height = 38;
            npc.defense = 0;
            npc.damage = 60;
            npc.lifeMax = 360000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 0f;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.alpha = 255;
            npc.scale = LumiteDestroyerArguments.Scale;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            music = MusicID.Boss3;
            musicPriority = MusicPriority.BossMedium;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * 0.8f);
        }
        public override void FindFrame(int frameHeight)
        {
            if (npc.localAI[0] >= DivideAttackStart && npc.localAI[0] <= DivideAttackStart + DivideAILength)
            {
                npc.frame.Y = frameHeight;
            }
            else if (npc.ai[0] >= 0 && Main.npc[(int)npc.ai[0]].life > 0 &&
                Main.npc[(int)npc.ai[0]].type == ModContent.NPCType<LumiteDestroyerBody>()
                && Main.npc[(int)npc.ai[0]].active)
            {
                if (Main.npc[(int)npc.ai[0]].localAI[0] >= DivideAttackStart && Main.npc[(int)npc.ai[0]].localAI[0] <= DivideAttackStart + DivideAILength)
                    npc.frame.Y = frameHeight * 2;
            }
            else npc.frame.Y = 0;
        }

        public override void AI()
        {
            if (npc.ai[3] > 0f)//set head
                npc.realLife = (int)npc.ai[3];

            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
                npc.TargetClosest();

            bool dead = false;
            if (npc.ai[1] < 0f)
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
            Player player = Main.player[npc.target];
            Vector2 targetModifier = player.velocity;
            NPC previousSegment = Main.npc[PreviousIndex];
            NPC head = Main.npc[npc.realLife];

            #region Loom up
            //if head is ready to be invisible,this should also be
            if(!(head.modNPC as LumiteDestroyerHead).CanBeTransparent())
            {
                if(npc.localAI[0] < DivideAttackStart)
                {
                    if (previousSegment.alpha < 128)
                    {
                        if (npc.alpha != 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Vortex, 0f, 0f, 100, default, 2f);
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
                }
                else
                {
                    if (npc.alpha != 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Vortex, 0f, 0f, 100, default, 2f);
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
            }

            #endregion
            #region Music
            if (head.ai[1] >= 0)
            {
                if (music != mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Crystar"))
                    music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Crystar");
            }
            #endregion

            if (Main.rand.Next(10) > 8 && head.life <= head.lifeMax * LumiteDestroyerArguments.Phase2HealthFactor
                && !(head.modNPC as LumiteDestroyerHead).CanBeTransparent())
            {
                int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Electric, 0f, 0f, 100, default, 2f);
                Main.dust[num].noGravity = true;
                Main.dust[num].noLight = true;
                Main.dust[num].scale = 0.4f;
                Main.dust[num].velocity = Main.rand.NextVector2Unit(npc.rotation - 0.001f, npc.rotation + 0.001f) * 9f;
            }

            npc.dontTakeDamage = (head.modNPC as LumiteDestroyerHead).CanBeTransparent();
            if (this.SyncAttackState < DivideAttackStart)//normal state
            {
                if (npc.ai[1] >= 0f && npc.ai[1] < Main.npc.Length)
                {
                    if (head.ai[1] >= SpinAttackStart)//spinning or preparing spinning
                    {
                        npc.chaseable = false;
                        Vector2 pivot = (head.modNPC as LumiteDestroyerHead).spinCenter;
                        if (npc.Distance(pivot) < LumiteDestroyerArguments.R)
                        {
                            npc.Center = pivot + npc.DirectionFrom(pivot) * LumiteDestroyerArguments.R;
                        }

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
                            npc.rotation = offset.ToRotation();// + (float)(Math.PI / 2) * npc.direction;
                            offset -= Vector2.Normalize(offset) * dist;
                            npc.velocity = Vector2.Zero;
                            npc.position += offset;
                        }
                    }
                    else
                    {
                        npc.chaseable = true;
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
                            npc.rotation = offset.ToRotation();
                            offset -= Vector2.Normalize(offset) * dist;
                            npc.velocity = Vector2.Zero;
                            npc.position += offset;
                        }

                        if (npc.localAI[1] > 0)
                        {
                            npc.localAI[1]--;
                            if (npc.localAI[1] <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (npc.localAI[0] == 0)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeathLaserEx>(), npc.damage / 5, 0f, Main.myPlayer, 36f, npc.target);
                            }
                        }
                    }
                }
            }
            else 
            {
                if (this.SyncAttackState >= DivideAttackStart)//acting as head
                {
                    var maxSpeed = 18f + player.velocity.Length() / 2;
                    float turnAcc = 0.15f;
                    float ramAcc = 0.15f;
                    if (Main.expertMode)
                        maxSpeed *= 1.125f;
                    //if (Main.getGoodWorld)
                    //    maxSpeed *= 1.25f;
                    maxSpeed = maxSpeed * 0.9f + maxSpeed * ((npc.lifeMax - npc.life) / (float)npc.lifeMax) * 0.2f;
                    maxSpeed = Math.Max(player.velocity.Length() * 1.5f, maxSpeed);
                    if (npc.localAI[0] == DivideAttackStart + 1)
                    {
                        npc.WormMovement(player.Center + targetModifier, maxSpeed * 0.75f, turnAcc * 1.25f, ramAcc);
                    }

                    npc.rotation = npc.velocity.ToRotation();
                }
            }
            
            //Lighting.AddLight(npc.Center, 0.3f, 0.3f, 0.5f);
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture2D = Main.npcTexture[npc.type];
            Texture2D DestTexture = (npc.type == ModContent.NPCType<LumiteDestroyerBody>()) ? 
                mod.GetTexture("NPCs/LumiteDestroyer/LumiteDestroyerBody_Glow") 
                : mod.GetTexture("NPCs/LumiteDestroyer/LumiteDestroyerTail_Glow");
            NPC head = Main.npc[npc.realLife];
            Color color = head.ai[1] != 1 ? Color.White : Color.Lerp(Color.White, Color.Red, (float)Math.Sin(MathHelper.Pi / 14 * npc.localAI[2]));
            SpriteEffects effects = (npc.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = drawColor;
            if (head.ai[1] == 2)
            {
                if (head.ai[2] >= 200)
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                    {
                        float k = 1 - (float)i / NPCID.Sets.TrailCacheLength[npc.type];
                        spriteBatch.Draw(texture2D, npc.oldPos[i] + npc.Size / 2 - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor * k * npc.Opacity, npc.oldRot[i] + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                        spriteBatch.Draw(DestTexture, npc.oldPos[i] + npc.Size / 2 - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity * k, npc.oldRot[i] + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                    }
                }
                else
                {
                    mainColor *= 0.5f;
                }
            }
            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), mainColor * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            spriteBatch.Draw(DestTexture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), color * 0.75f * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            return false;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (npc.localAI[0] >= DivideAttackStart && npc.localAI[0] <= DivideAttackStart + DivideAILength)
            {
                index = ModContent.GetModBossHeadSlot("SunksBossChallenges/NPCs/LumiteDestroyer/LumiteDestroyerHead_Head_Boss");
            }
            else
            {
                index = -1;
            }
            base.BossHeadSlot(ref index);
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (npc.realLife != -1 && Main.npc[npc.realLife].type == ModContent.NPCType<LumiteDestroyerHead>())
            {
                return !(Main.npc[npc.realLife].modNPC as LumiteDestroyerHead).CanBeTransparent();
            }
            return true;
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (this.SyncAttackState < DivideAttackStart)
            {
                if (npc.realLife != -1 && Main.npc[npc.realLife].type == ModContent.NPCType<LumiteDestroyerHead>())
                {
                    NPC head = Main.npc[npc.realLife];
                    if (head.life <= head.lifeMax * LumiteDestroyerArguments.Phase2HealthFactor)
                    {
                        if (head.ai[1] >= DivideAttackStart && head.ai[1] <= DivideAttackStart + DivideAILength)
                        {
                            damage *= (1 - 0.875);
                        }
                        else
                        {
                            damage *= (1 - 0.80);
                        }
                    }
                    if (npc.alpha > 0 || Main.npc[npc.realLife].ai[1] >= SpinAttackStart)
                        damage *= (1 - 0.99);
                }
            }
            else
            {
                if (npc.alpha > 0)
                    damage *= (1 - 0.99);
            }
            return true;
        }
    }
}
