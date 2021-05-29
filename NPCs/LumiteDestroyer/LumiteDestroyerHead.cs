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
    [AutoloadBossHead]
    public partial class LumiteDestroyerHead : LumiteDestroyerSegment
    {
        internal Vector2 spinCenter;
        internal int spinTimer = 900;
        internal int[] chaosPlanets = new int[3];
        internal int randomCorrecter = 0;

        internal Vector2 healBarPos;//this one doesn't need to be synchronized.
        int Length => 80;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LM-002 \"Annhilation\"");
            NPCID.Sets.TrailingMode[npc.type] = 3;
            NPCID.Sets.TrailCacheLength[npc.type] = 16;
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.TheDestroyer];
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.boss = true;
            npc.npcSlots = 1f;
            npc.width = npc.height = 50;
            npc.defense = 0;
            npc.damage = 100;
            npc.lifeMax = 360000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 0f;
            npc.netAlways = true;
            npc.alpha = 255;
            npc.scale = LumiteDestroyerArguments.Scale;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            music = MusicID.Boss3;
            musicPriority = MusicPriority.BossMedium;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax / 2 * bossLifeScale);
        }
        #region AIHelper
        public bool IsDeathStruggling()
        {
            return npc.ai[1] >= DeathStruggleStart && npc.ai[1] < DeathStruggleStart + 5;
        }
        public bool IsPhase3()
        {
            return npc.life < npc.lifeMax * 0.5f;
        }
        public bool CanBeTransparent()
        {
            return (npc.ai[1] == -1 || npc.ai[1] == DivideAttackStart || npc.ai[1] == DivideAttackStart + DivideAILength
                || npc.ai[1] == DeathStruggleStart);
        }
        public bool AllowSpin()
        {
            return IsPhase3() && (npc.ai[1] < DivideAttackStart || npc.ai[1] > DivideAttackStart + DivideAILength) &&
                (npc.ai[1] != 1) && (npc.ai[1] != 3) && (npc.ai[1] != 4);
        }
        void SwitchTo(float ai1, bool resetCounter = true, bool resetAllTimer = true)
        {
            npc.ai[1] = ai1;
            npc.ai[2] = 0;
            if (resetCounter)
            {
                npc.ai[3] = 0;
                npc.localAI[2] = 0;
            }
            npc.localAI[0] = 0;
            if (resetAllTimer)
            {
                npc.localAI[1] = 0;
            }
            npc.netUpdate = true;
        }
        void SwitchToRandomly(float normalAI1, float randomAI1, float possibility)
        {
            randomCorrecter++;
            if ((Main.rand.NextFloat() < possibility && randomCorrecter > 2) || randomCorrecter == 6)//at least one divide attack every 6 attacks
            {
                randomCorrecter = 0;
                SwitchTo(randomAI1);
            }
            else
            {
                SwitchTo(normalAI1);
            }
        }
        #endregion
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
            writer.Write(spinCenter.X);
            writer.Write(spinCenter.Y);
            writer.Write(spinTimer);
            writer.Write(chaosPlanets[0]);
            writer.Write(chaosPlanets[1]);
            writer.Write(chaosPlanets[2]);
            writer.Write(randomCorrecter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
            spinCenter.X = reader.ReadSingle();
            spinCenter.Y = reader.ReadSingle();
            spinTimer = reader.ReadInt32();
            chaosPlanets[0] = reader.ReadInt32();
            chaosPlanets[1] = reader.ReadInt32();
            chaosPlanets[2] = reader.ReadInt32();
            randomCorrecter = reader.ReadInt32();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            if (npc.ai[1] >= DivideAttackStart && npc.ai[1] <= DivideAttackStart + DivideAILength)
            {
                Vector2 value = default(Vector2);
                float num2 = 999999f;
                for(int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && 
                        (Main.npc[i].type==ModContent.NPCType<LumiteDestroyerHead>()
                        || Main.npc[i].type == ModContent.NPCType<LumiteDestroyerTail>()
                        || Main.npc[i].type == ModContent.NPCType<LumiteDestroyerBody>()))
                    {
                        Vector2 vector = Main.player[Main.myPlayer].Center - Main.npc[i].Center;
                        if (vector.Length() < num2 && Collision.CanHit(Main.player[Main.myPlayer].Center, 1, 1, Main.npc[i].Center, 1, 1))
                        {
                            num2 = vector.Length();
                            value = Main.npc[i].Center;
                        }
                    }
                }
                if (!Main.gamePaused)
                {
                    if (num2 < (float)Main.screenWidth)
                    {
                        if (healBarPos.X < 100f && healBarPos.Y < 100f)
                        {
                            healBarPos = value;
                        }
                        else
                        {
                            healBarPos = (healBarPos * 49f + value) / 50f;
                        }
                        position = healBarPos;
                    }
                    else
                    {
                        healBarPos = new Vector2(0f, 0f);
                    }
                }
                else
                {
                    position = healBarPos;
                }
                return true;
            } 
            return base.DrawHealthBar(hbPosition, ref scale, ref position);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (npc.alpha > 0)
                return false;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !CanBeTransparent();
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.alpha > 0 || npc.ai[1] >= 101)
                damage *= (1 - 0.99);
            return true;
        }

        public override bool CheckDead()
        {
            if (npc.ai[1] < DeathStruggleStart)
            {
                if (npc.ai[1] == 3)//chrono dash
                {
                    Main.fastForwardTime = false;
                }
                npc.life = 1;
                npc.ai[1] = DeathStruggleStart;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                npc.netUpdate = true;
                return false;
            }
            else if (npc.ai[1] < DeathStruggleStart + 5)
            {
                npc.life = 1;
                return false;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture2D = Main.npcTexture[npc.type];
            Texture2D DestTexture = mod.GetTexture("NPCs/LumiteDestroyer/LumiteDestroyerHead_Glow");
            Color glowColor = npc.ai[1] != 1 ? Color.White : Color.Lerp(Color.White, Color.Black, (float)Math.Sin(MathHelper.Pi / 14 * npc.localAI[2]));
            SpriteEffects effects = (npc.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = drawColor;
            if (npc.ai[1] == 2)
            {
                if(npc.ai[2] >= 200)
                {
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                    {
                        float k = 1 - (float)i / NPCID.Sets.TrailCacheLength[npc.type];
                        spriteBatch.Draw(texture2D, npc.oldPos[i] + npc.Size / 2 - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), drawColor * k * npc.Opacity, npc.oldRot[i] + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                        spriteBatch.Draw(DestTexture, npc.oldPos[i] + npc.Size / 2 - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), glowColor * 0.75f * npc.Opacity * k, npc.oldRot[i] + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
                    }
                }
                else
                {
                    mainColor *= 0.5f;
                }
            }
            else if ((npc.ai[1] == DivideAttackStart + 5 )|| (npc.ai[1] == DivideAttackStart + 6))
            {
                if (npc.ai[2] >= 10)
                {
                    if (npc.ai[1] == DivideAttackStart + 5)
                        npc.DrawAim(spriteBatch, npc.Center + new Vector2(LumiteDestroyerArguments.R, 0) * 5 * npc.ai[3], Color.Red);
                    if (npc.ai[1] == DivideAttackStart + 6)
                        npc.DrawAim(spriteBatch, npc.Center + new Vector2(0, LumiteDestroyerArguments.R) * 5 * npc.ai[3], Color.Red);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 4)
            {
                if (npc.ai[2] >= 360)
                {
                    float alpha = 1 - (npc.ai[2] - 360) / 360;
                    if (alpha < 0) alpha = 0;
                    glowColor *= alpha;
                    mainColor = Color.Lerp(mainColor, Color.Black, alpha / 2);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 5)
            {
                glowColor *= 0;
                mainColor = Color.Lerp(mainColor, Color.Black, 0.5f);
            }
            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), mainColor * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            spriteBatch.Draw(DestTexture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), glowColor * 0.75f * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            return false;
        }
    }
}
