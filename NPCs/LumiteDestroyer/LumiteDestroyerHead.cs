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
        internal int DynDRTimer = 0;
        internal int lastHealth = 0;
        internal float DynDR;
        internal bool DivideChooser = false;

        internal Vector2 healBarPos;//this one doesn't need to be synchronized.
        int Length => 80;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nova Annihilator");
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
            npc.damage = 120;
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
            return npc.life < npc.lifeMax * 0.65f;
        }
        public bool CanBeTransparent()
        {
            return (npc.ai[1] == -1 || npc.ai[1] == DivideAttackStart || npc.ai[1] == DivideAttackStart + DivideAILength
                || npc.ai[1] == DeathStruggleStart || npc.ai[1] == DeathStruggleStart + 5);
        }
        public bool AllowSpin()
        {
            return IsPhase3() && (npc.ai[1] < DivideAttackStart || npc.ai[1] > DivideAttackStart + DivideAILength) &&
                (npc.ai[1] != ChronoDash) && (npc.ai[1] != StarCard) && (npc.ai[1] != SigilStar) && (npc.ai[1] != PlanetAurora);
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
            if ((Main.rand.NextFloat() < possibility && randomCorrecter > 3) || randomCorrecter == 8)//at least one divide attack every 9 attacks
            {
                randomCorrecter = 0;
                SwitchTo(randomAI1);
            }
            else
            {
                SwitchTo(normalAI1);
            }
        }
        void DeathStruggle(Player player,Vector2 targetModifier,float maxSpeed,float turnAcc,float ramAcc)
        {
            #region Death Struggle
            if (npc.ai[1] == DeathStruggleStart)
            {
                npc.WormMovement(player.Center + targetModifier, maxSpeed / 2, turnAcc, ramAcc / 2);
                npc.ai[2]++;
                ForeachSegment((tmpNPC, counter) =>
                {
                    tmpNPC.alpha += 3;
                    if (tmpNPC.alpha > 255) tmpNPC.alpha = 255;
                });

                if (npc.ai[2] == 15)
                {
                    npc.NewOrBoardcastText("OVERLOADED MODE ON", Color.BlueViolet, false);
                }

                if (npc.ai[2] >= 200)
                {
                    for(int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].active && (Main.projectile[i].hostile
                            || Main.projectile[i].type == ModContent.ProjectileType<LMSigilStar>()
                            || Main.projectile[i].type == ModContent.ProjectileType<LMDoublePlanet>()
                            || Main.projectile[i].type == ModContent.ProjectileType<LMStarSigil>()))
                        {
                            Main.projectile[i].active = false;
                        }
                    }

                    Vector2 dist = Main.rand.NextVector2Unit() * LumiteDestroyerArguments.R * 1.8f;
                    npc.Center = player.Center + player.velocity * 60f + dist;
                    npc.velocity = Vector2.Normalize(player.Center - npc.Center) * maxSpeed / 3;
                    ForeachSegment((tmpNPC, counter) =>
                    {
                        tmpNPC.Center = player.Center + dist;
                        tmpNPC.velocity = Vector2.Zero;
                        tmpNPC.localAI[0] = 0;
                        tmpNPC.localAI[1] = 0;
                        tmpNPC.localAI[2] = 0;
                        tmpNPC.frame.Y = 0;
                        tmpNPC.netUpdate = true;
                    });
                    SwitchTo(DeathStruggleStart + 2);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 1)
            {
                npc.ai[2]++;
                if (npc.ai[2] == 120 || npc.ai[2] == 500 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int direction = Main.rand.Next(4);
                    /*if (npc.ai[2] == 120)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<LMStarSigilEx>(),
                            npc.damage / 8, 0f, Main.myPlayer, 0f, npc.target);*/
                    if (npc.ai[2] == 500)
                        Projectile.NewProjectile(player.Center - Vector2.UnitX.RotatedBy(Math.PI / 2 * direction) * 1080,
                            Vector2.Zero, ModContent.ProjectileType<LMLaserMatrix>(),
                            npc.damage / 6, 0f, Main.myPlayer, 180, direction);
                }
                if (npc.ai[2] < 500)
                    npc.WormMovementEx(player.Center, maxSpeed, turnAcc, ramAcc);
                else npc.WormMovementEx(player.Center + npc.DirectionFrom(player.Center) * LumiteDestroyerArguments.R * 1.5f,
                    maxSpeed, turnAcc, ramAcc);

                if (npc.ai[2] >= 675)
                {
                    var center = player.Center + player.velocity * 10;
                    spinCenter = center;
                    SwitchTo(DeathStruggleStart + 2);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 2)
            {
                float r = (float)(LumiteDestroyerArguments.SpinSpeed / LumiteDestroyerArguments.SpinRadiusSpeed);
                var center = spinCenter;
                if (Vector2.Distance(npc.Center, center) <= r)//has moved to the desired position
                {
                    if (npc.Distance(center) < r)//modify it to retain accuracy
                        npc.position += center + Vector2.Normalize(npc.Center - center) * r - npc.Center;
                    npc.direction = Main.rand.NextBool() ? -1 : 1;
                    npc.velocity = Vector2.Normalize(npc.Center - center)
                        .RotatedBy(-Math.PI / 2 * npc.direction) * LumiteDestroyerArguments.SpinSpeed;
                    npc.rotation = npc.velocity.ToRotation();
                    SwitchTo(DeathStruggleStart + 3);
                }
                else
                {
                    npc.WormMovement(center, maxSpeed * 1.5f, 0.5f, 0.75f);
                }
                if (Vector2.Distance(player.Center, center) > LumiteDestroyerArguments.R)
                {
                    player.Center = center + Vector2.Normalize(player.Center - center) * LumiteDestroyerArguments.R;
                }
                var pivot = spinCenter;
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2 * Math.PI;
                    offset.X += (float)(Math.Cos(angle) * r);
                    offset.Y += (float)(Math.Sin(angle) * r);
                    Dust dust = Main.dust[Dust.NewDust(pivot + offset, 0, 0, DustID.Clentaminator_Purple, 0, 0, 100, Color.White)];
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.Next(3) == 0)
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 3)
            {
                int direction = npc.direction;
                var center = spinCenter;
                if (npc.Distance(center) < LumiteDestroyerArguments.R)
                {
                    npc.Center = center + npc.DirectionFrom(center) * LumiteDestroyerArguments.R;
                }
                //player.wingTime = 100;
                npc.velocity = npc.velocity.RotatedBy(-LumiteDestroyerArguments.SpinRadiusSpeed * direction);
                //npc.rotation -= (float)(LumiteDestroyerArguments.SpinRadiusSpeed * direction);
                if (Vector2.Distance(player.Center, center) > LumiteDestroyerArguments.R)
                {
                    player.Center = center + Vector2.Normalize(player.Center - center) * LumiteDestroyerArguments.R;
                }
                var pivot = spinCenter;
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2 * Math.PI;
                    offset.X += (float)(Math.Cos(angle) * LumiteDestroyerArguments.R);
                    offset.Y += (float)(Math.Sin(angle) * LumiteDestroyerArguments.R);
                    Dust dust = Main.dust[Dust.NewDust(pivot + offset, 0, 0, DustID.Clentaminator_Purple, 0, 0, 100, Color.White)];
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.Next(3) == 0)
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                }
                npc.ai[2]++;

                if (npc.ai[2] % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var target = spinCenter;
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(),
                        npc.damage / 3, 0f, Main.myPlayer, target.X, target.Y);
					Projectile.NewProjectile((npc.Center - target).RotatedBy(Math.PI / 2) + target, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(),
                        npc.damage / 3, 0f, Main.myPlayer, target.X, target.Y);
                }

                if (npc.ai[2] >= 360)
                {
                    SwitchTo(DeathStruggleStart + 4);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 4)
            {
                int direction = npc.direction;
                var center = spinCenter;
                if (npc.Distance(center) < LumiteDestroyerArguments.R)
                {
                    npc.Center = center + npc.DirectionFrom(center) * LumiteDestroyerArguments.R;
                }
                //player.wingTime = 100;
                npc.velocity = npc.velocity.RotatedBy(-LumiteDestroyerArguments.SpinRadiusSpeed * direction);
                //npc.rotation -= (float)(LumiteDestroyerArguments.SpinRadiusSpeed * direction);
                if (Vector2.Distance(player.Center, center) > LumiteDestroyerArguments.R)
                {
                    player.Center = center + Vector2.Normalize(player.Center - center) * LumiteDestroyerArguments.R;
                }
                var pivot = spinCenter;
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2 * Math.PI;
                    offset.X += (float)(Math.Cos(angle) * LumiteDestroyerArguments.R);
                    offset.Y += (float)(Math.Sin(angle) * LumiteDestroyerArguments.R);
                    Dust dust = Main.dust[Dust.NewDust(pivot + offset, 0, 0, DustID.Clentaminator_Purple, 0, 0, 100, Color.White)];
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.Next(3) == 0)
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                }
                if (npc.ai[2] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[3] = Projectile.NewProjectile(spinCenter, Vector2.Zero, ModContent.ProjectileType<LMBlackHole>(),
                        npc.damage, 0f, Main.myPlayer, npc.whoAmI);
                }

                npc.ai[2]++;
                if (Util.CheckProjAlive<LMBlackHole>((int)npc.ai[3]))
                {
                    Projectile blackhole = Main.projectile[(int)npc.ai[3]];

                    if (blackhole.ai[1] >= 1080)
                    {
                        npc.velocity = Vector2.Zero;
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            tmpNPC.localAI[0] = 0;
                            tmpNPC.localAI[1] = 0;
                            tmpNPC.localAI[2] = 0;
                            tmpNPC.localAI[3] = counter * 1;
                            tmpNPC.frame.Y = 0;
                            tmpNPC.netUpdate = true;
                        });
                        SwitchTo(DeathStruggleStart + 5);
                    }
                }

                if (npc.ai[2] >= 3000)
                {
                    npc.velocity = Vector2.Zero;
                    ForeachSegment((tmpNPC, counter) =>
                    {
                        tmpNPC.localAI[0] = 0;
                        tmpNPC.localAI[1] = 0;
                        tmpNPC.localAI[2] = 0;
                        tmpNPC.localAI[3] = counter * 1;
                        tmpNPC.frame.Y = 0;
                        tmpNPC.netUpdate = true;
                    });
                    SwitchTo(DeathStruggleStart + 5);
                }
            }
            #endregion
        }
        public override void PostAI()
        {
            if (DynDRTimer == 0 && npc.ai[1] >= 0)
            {
                if (npc.life < lastHealth - npc.lifeMax / 4800)
                {
                    DynDR = Math.Max(DynDR - 0.01f, 1 - ((float)npc.lifeMax / 4800 / (lastHealth - npc.life)));
                    DynDR = Math.Max(DynDR, 0.6f);
                }
                else
                {
                    DynDR = Math.Max(DynDR - 0.0075f, 0.6f);
                }
                lastHealth = npc.life;
            }
            DynDR = Math.Max(DynDR, 0.35f);
            DynDRTimer++;
            if (DynDRTimer > 3) DynDRTimer = 0;
            //Main.NewText(DynDR.ToString());
            base.PostAI();
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
            base.SendExtraAI(writer);
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
            base.ReceiveExtraAI(reader);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
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

        public override void BossHeadSlot(ref int index)
        {
            if (CanBeTransparent()) 
                index = -1;
            else 
                base.BossHeadSlot(ref index);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (npc.alpha > 0)
                return false;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return !CanBeTransparent() && (npc.ai[1] != DeathStruggleStart + 5);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.alpha > 0 || npc.ai[1] >= SpinAttackStart)
                damage *= (1 - 0.99);
            damage *= (1 - DynDR);
            return base.StrikeNPC(ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override bool CheckDead()
        {
            if (npc.ai[1] < DeathStruggleStart)
            {
                if (npc.ai[1] == ChronoDash)//chrono dash
                {
                    Projectile clock = Main.projectile[(int)npc.localAI[0]];
                    Main.fastForwardTime = false;
                    if (clock.active && clock.type == ModContent.ProjectileType<LMClockFace>())
                    {
                        clock.active = false;
                        Main.dayTime = false;
                        Main.time = 16200;
                    }
                }
                if (npc.ai[1] == PlanetAurora)
                {
                    if (Util.CheckProjAlive<LMDoublePlanetAurora>((int)npc.ai[3]))
                    {
                        Main.projectile[(int)npc.ai[3]].localAI[1] = 1;
                    }
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
            /*if (npc.ai[1] == 3)
            {
                if(npc.ai[2] >= 240)
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
            }*/
            if ((npc.ai[1] == DivideAttackStart + 5 )|| (npc.ai[1] == DivideAttackStart + 6))
            {
                /*if (npc.ai[2] >= 10)
                {
                    if (npc.ai[1] == DivideAttackStart + 5)
                        npc.DrawAim(spriteBatch, npc.Center + new Vector2(LumiteDestroyerArguments.R, 0) * 5 * npc.ai[3], Color.Red);
                    if (npc.ai[1] == DivideAttackStart + 6)
                        npc.DrawAim(spriteBatch, npc.Center + new Vector2(0, LumiteDestroyerArguments.R) * 5 * npc.ai[3], Color.Red);
                }*/
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
