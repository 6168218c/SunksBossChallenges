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
    public partial class LumiteDestroyerHead : LumiteDestroyerSegment
    {
        public override void AI()
        {
            //string aiDump = $"ai:{string.Join(",", npc.ai.Select(fl => $"{fl}"))}";
            //aiDump += $" localAI:{string.Join(",", npc.localAI.Select(fl => $"{fl}"))}";
            //Main.NewText($"{aiDump}");
            if (NPC.FindFirstNPC(ModContent.NPCType<LumiteDestroyerHead>()) != npc.whoAmI)
            {
                npc.active = false;
                return;
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
                if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
                {
                    if (npc.ai[1] == 3)//chrono dash
                    {
                        Main.fastForwardTime = false;
                    }
                    npc.WormMovement(npc.Center + new Vector2(0, -900f), 25f);
                    npc.rotation = npc.velocity.ToRotation();
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
            if (!CanBeTransparent())
            {
                if (npc.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Electric, 0f, 0f, 100, default, 2f);
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
            }

            if (npc.ai[0] == 0f)
            {
                int previous = npc.whoAmI;
                npc.direction = 1;
                for (int j = 1; j <= Length; j++)
                {
                    int npcType = ModContent.NPCType<LumiteDestroyerBody>();
                    if (j == Length)
                    {
                        npcType = ModContent.NPCType<LumiteDestroyerTail>();
                    }
                    int current = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, npcType, npc.whoAmI);
                    Main.npc[current].ai[3] = npc.whoAmI;
                    Main.npc[current].realLife = npc.whoAmI;
                    Main.npc[current].ai[1] = previous;
                    Main.npc[previous].ai[0] = current;
                    if (j == Length)
                        Main.npc[current].ai[0] = -1;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, current);

                    previous = current;
                }

                if (!NPC.AnyNPCs(ModContent.NPCType<LumiteDestroyerTail>()))
                {
                    npc.active = false;
                    return;
                }

                npc.ai[1] = -2f;//not in phase 2
            }

            int spinMaxTime = 2100;
            var maxSpeed = 18f + player.velocity.Length() / 2;
            float turnAcc = 0.15f;
            float ramAcc = 0.15f;
            if (Main.expertMode)
                maxSpeed *= 1.125f;
            //if (Main.getGoodWorld)
            //    maxSpeed *= 1.25f;
            maxSpeed = maxSpeed * 0.9f + maxSpeed * ((npc.lifeMax - npc.life) / (float)npc.lifeMax) * 0.2f;
            maxSpeed = Math.Max(player.velocity.Length() * 1.5f, maxSpeed);
            bool isPhase2 = npc.life <= npc.lifeMax * LumiteDestroyerArguments.Phase2HealthFactor;
            bool specialAttacks = npc.life <= npc.lifeMax * 0.5;

            #region CommonHelper
            void CrawlipedeMove(int offset = 200)
            {
                var targetPos = player.Center;
                int playerTileX = (int)(targetPos.X / 16f);
                int playerTileY = (int)(targetPos.Y / 16f);
                int OffgroundTile = -1;
                for (int i = playerTileX - 2; i <= playerTileX + 2; i++)
                {
                    for (int j = playerTileY; j <= playerTileY + 18; j++)
                    {
                        if (WorldGen.SolidTile2(i, j))
                        {
                            OffgroundTile = j;
                            break;
                        }
                    }
                    if (OffgroundTile > 0)
                    {
                        break;
                    }
                }
                if (OffgroundTile > 0)
                {
                    OffgroundTile *= 16;
                    float heightOffset = OffgroundTile - 600;
                    if (player.Center.Y > heightOffset)
                    {
                        targetPos.Y = heightOffset - offset;
                        if (Math.Abs(npc.Center.X - player.Center.X) < 500f)
                        {
                            targetPos.X = targetPos.X + Math.Sign(npc.velocity.X) * 600f;
                        }
                        turnAcc *= 1.5f;
                    }
                    else
                    {
                        turnAcc *= 1.2f;
                    }
                }
                else
                {
                    maxSpeed *= 1.125f;//charge
                    turnAcc *= 2f;
                }
                float speed = npc.velocity.Length();
                if (OffgroundTile > 0)
                {
                    float num47 = maxSpeed * 1.3f;
                    float num48 = maxSpeed * 0.7f;
                    float num49 = npc.velocity.Length();
                    if (num49 > 0f)
                    {
                        if (num49 > num47)
                        {
                            npc.velocity.Normalize();
                            npc.velocity *= num47;
                        }
                        else if (num49 < num48)
                        {
                            npc.velocity.Normalize();
                            npc.velocity *= num48;
                        }
                    }
                }
                npc.WormMovementEx(targetPos, maxSpeed, turnAcc);
            }
            #endregion

            if (Main.rand.Next(10) > 8 && isPhase2 && !CanBeTransparent())
            {
                int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Electric, 0f, 0f, 100, default, 2f);
                Main.dust[num].noGravity = true;
                Main.dust[num].noLight = true;
                Main.dust[num].scale = 0.4f;
                Main.dust[num].velocity = Main.rand.NextVector2Unit(npc.rotation - 0.001f, npc.rotation + 0.001f) * 9f;
            }

            Vector2 targetModifier = player.velocity;
            if (spinTimer >= spinMaxTime - 300 && AllowSpin()) 
            {
                if (spinTimer == spinMaxTime - 300)
                {
                    Main.PlaySound(SoundID.Roar, npc.Center);
                }
                targetModifier = npc.DirectionFrom(player.Center) * LumiteDestroyerArguments.R * 1.5f;
            }

            if (isPhase2 && AllowSpin())//ensure not in split attack
            {
                spinTimer++;
            }
            int chaosPlanetsCount = 3;

            if (Vector2.Distance(npc.Center, player.Center) >= 4500f)
            {
                maxSpeed *= 5f;
                turnAcc *= 5f;
            }
            else if (npc.velocity.Length() > maxSpeed * 2.4 && npc.ai[1] != 2 && npc.ai[1] < DivideAttackStart)
            {
                npc.SlowDown(0.9f);
            }

            if (spinTimer >= spinMaxTime)
            {
                if (AllowSpin())//ensure not in split attack
                {
                    if (npc.ai[1] < SpinAttackStart)//not even have set up
                    {
                        npc.localAI[2] = npc.ai[1];
                        npc.ai[1] = SpinAttackStart;
                        var center = player.Center + player.velocity * 10;
                        spinCenter = center;
                        Vector2 dist = npc.DirectionFrom(center);
                        dist *= LumiteDestroyerArguments.R * 1.75f;
                        dist = center + dist;
                        npc.WormMovement(dist, maxSpeed * 5f, turnAcc * 5f);
                    }
                }
                else
                {
                    spinTimer = spinMaxTime - 1;
                }
            }

            npc.dontTakeDamage = CanBeTransparent() || IsDeathStruggling();

            if (spinTimer <= 240 && npc.ai[1] >= 0 && AllowSpin())
            {
                targetModifier = npc.DirectionFrom(player.Center) * LumiteDestroyerArguments.R * 1.5f;
                npc.WormMovement(player.Center + targetModifier, maxSpeed * 0.9f, turnAcc, ramAcc);
                npc.rotation = npc.velocity.ToRotation();
                return;
            }

            if (npc.ai[1] < SpinAttackStart)
            {
                #region Normal Attack
                if (npc.ai[1] == -2)
                {
                    npc.ai[2]++;
                    npc.WormMovement(player.Center + targetModifier, maxSpeed * 0.9f, turnAcc, ramAcc);

                    if (npc.ai[2] >= 300)
                    {
                        npc.ai[2] = 0;

                        int mode = Main.rand.Next(3);
                        Func<int, int> generator = null;
                        switch (mode)
                        {
                            case 0:
                                generator = index => 210 - Math.Abs(index - 40) * 5;
                                break;
                            case 1:
                                generator = index => Math.Abs(index - 40) * 5;
                                break;
                            case 2:
                                generator = index => (index % 40) * 5;
                                break;
                            default:
                                generator = _ => 30;
                                break;
                        }
                        int i = npc.whoAmI;
                        var rotate = (player.Center - npc.Center).ToRotation();
                        int counter = 0;
                        while (i != -1)
                        {
                            Main.npc[i].localAI[0] = rotate;
                            Main.npc[i].localAI[1] = generator(counter++);
                            if (counter == 40)
                            {
                                rotate = (player.Center - Main.npc[i].Center).ToRotation();
                            }
                            i = (int)Main.npc[i].ai[0];
                        }
                    }

                    if (npc.localAI[1] > 0)
                    {
                        npc.localAI[1]--;
                        if (npc.localAI[1] <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.localAI[1] = 0;
                            Projectile.NewProjectile(npc.Center, npc.localAI[0].ToRotationVector2() * 20, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.DarkStar>(), npc.damage / 5, 0f, Main.myPlayer);
                        }
                    }

                    if (npc.life <= npc.lifeMax * LumiteDestroyerArguments.Phase2HealthFactor)
                    {
                        SwitchTo(-1);
                        SkyManager.Instance.Activate("SunksBossChallenges:LumiteDestroyer");
                        music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Frontier");
                    }
                }
                else if (npc.ai[1] == -1)
                {
                    npc.WormMovement(player.Center + targetModifier, maxSpeed / 2, turnAcc, ramAcc / 2);
                    npc.ai[2]++;
                    ForeachSegment((tmpNPC, counter) =>
                    {
                        tmpNPC.alpha += 3;
                        if (tmpNPC.alpha > 255) tmpNPC.alpha = 255;
                    });

                    if (npc.ai[2] >= 120)
                    {
                        Vector2 dist = Main.rand.NextVector2Unit() * 1000;
                        npc.Center = player.Center + player.velocity * 60f + dist;
                        npc.velocity = Vector2.Normalize(player.Center - npc.Center) * maxSpeed / 3;
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            tmpNPC.Center = player.Center + player.velocity * 60f + dist;
                            tmpNPC.velocity = Vector2.Zero;
                            tmpNPC.localAI[0] = 0;
                            tmpNPC.localAI[1] = 0;
                            tmpNPC.localAI[2] = 0;
                            tmpNPC.frame.Y = 0;
                            tmpNPC.netUpdate = true;
                        });
                        SwitchTo(0);
                    }
                }
                else if (npc.ai[1] == 0)//laser matrix
                {
                    if ((npc.ai[2] + 60) % 240 == 0)
                    {
                        int direction = Main.rand.Next(4);
                        Projectile.NewProjectile(player.Center + player.velocity * 45 - Vector2.UnitX.RotatedBy(Math.PI / 2 * direction) * 720,
                            Vector2.Zero, ModContent.ProjectileType<LMLaserMatrix>(),
                            npc.damage / 6, 0f, Main.myPlayer, 240, direction);
                    }

                    if (npc.ai[2] % 450 == 0 && Main.netMode != NetmodeID.MultiplayerClient && specialAttacks)
                    {
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<LMStarSigil>(),
                            npc.damage / 6, 0f, Main.myPlayer, 105, npc.target);
                    }

                    npc.ai[2]++;

                    npc.WormMovementEx(player.Center + targetModifier, maxSpeed, turnAcc, ramAcc, 0.05f, 500, MathHelper.Pi * 2 / 5);

                    if (npc.ai[2] >= 750)
                    {
                        SwitchToRandomly(1, DivideAttackStart, 0.25f);
                    }
                }
                else if (npc.ai[1] == 1)//star sigil Ex
                {
                    maxSpeed *= 0.8f;
                    turnAcc *= 1.5f;

                    if ((npc.ai[2] + 200) % 500 == 0 && Main.netMode!=NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<LMStarSigilEx>(),
                            npc.damage / 8, 0f, Main.myPlayer,
                            Main.rand.NextBool().ToDirectionInt(), npc.target);
                    }

                    npc.ai[2]++;
                    CrawlipedeMove();

                    if (npc.ai[2] >= 1200)
                    {
                        SwitchToRandomly(2, DivideAttackStart, 0.3f);
                    }
                }
                else if (npc.ai[1] == 2)
                {
                    maxSpeed *= 0.8f;

                    npc.ai[2]++;

                    if (npc.ai[2] % 75 == 0 && npc.ai[2] <= 450)
                    {
                        int tarCount = Main.rand.Next(1, 80);
                        Vector2 center = npc.Center;
                        int i = npc.whoAmI;
                        int counter = 0;
                        while (i != -1)
                        {
                            counter++;
                            NPC tmpNPC = Main.npc[i];
                            if (counter == tarCount)
                            {
                                center = tmpNPC.Center;
                                break;
                            }
                            i = (int)Main.npc[i].ai[0];
                        }

                        Projectile.NewProjectile(center, (player.Center - center).SafeNormalize(Vector2.UnitX) * maxSpeed * 0.8f,
                            ModContent.ProjectileType<LMDoublePlanet>(), npc.damage / 3, 0f, Main.myPlayer, 5f);
                    }

                    npc.WormMovementEx(player.Center + targetModifier, maxSpeed * 0.9f, turnAcc, ramAcc);

                    if (npc.ai[2] >= 675)
                    {
                        SwitchToRandomly(IsPhase3() ? 3 : 0, DivideAttackStart, 0.3f);
                    }
                }
                //the below are phase 3 exclusive
                else if (npc.ai[1] == 3)//chrono dash
                {
                    if (npc.ai[2] == 0 && npc.ai[3] == 0 && Main.netMode!=NetmodeID.MultiplayerClient)//spawn the clock
                    {
                        npc.localAI[0] = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, ModContent.ProjectileType<LMClockFace>(),
                            npc.damage / 2, 0f, Main.myPlayer, npc.whoAmI).whoAmI;
                    }

                    Projectile clock = Main.projectile[(int)npc.localAI[0]];
                    var target = clock.Center + clock.localAI[1].ToRotationVector2() * LumiteDestroyerArguments.R * 1.8f;
                    npc.ai[2]++;
                    if (npc.ai[2] <= 190)
                    {
                        Main.fastForwardTime = true;
                        npc.localAI[1] = (float)Main.time;
                        npc.WormMovementEx(player.Center + npc.DirectionFrom(player.Center) * LumiteDestroyerArguments.R * 1.5f,
                            maxSpeed, turnAcc, ramAcc, radiusSpeed: 0.05f, distLimit: 100, angleLimit: MathHelper.Pi / 6);
                    }
                    else if (npc.ai[2] < 250)
                    {
                        if (npc.ai[2] == 200)
                        {
                            var proj = Projectile.NewProjectileDirect(clock.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, Main.myPlayer, -1, -4);
                            proj.scale = 2f;
                        }
                        if (npc.ai[3] == 5)
                        {
                            Main.fastForwardTime = false;
                            Main.dayTime = false;
                            Main.time = 16200;
                            npc.localAI[1] = 16200;
                            if (npc.ai[2] == 0) npc.ai[2] += 90;//wait shorter for it
                        }
                        else
                            Main.time = npc.localAI[1];
                        npc.WormMovementEx(player.Center + npc.DirectionFrom(player.Center) * LumiteDestroyerArguments.R * 1.5f, maxSpeed, turnAcc, ramAcc, radiusSpeed: 0.1f, distLimit: 100, angleLimit: MathHelper.Pi / 6);
                    }
                    else if (npc.ai[2] <= 280)
                    {
                        Main.time = npc.localAI[1];
                        if (npc.ai[2] == 250)
                        {
                            int i = npc.whoAmI;
                            int counter = 0;
                            while (i != -1)
                            {
                                counter++;
                                NPC tmpNPC = Main.npc[i];
                                tmpNPC.Center = target;
                                tmpNPC.velocity = Vector2.Zero;
                                i = (int)Main.npc[i].ai[0];
                            }
                            npc.velocity = (clock.Center - npc.Center).SafeNormalize(Vector2.Zero) * maxSpeed / 3;
                        }
                        if (npc.velocity.Compare(maxSpeed * 2) < 0)
                            npc.velocity *= 1.2f;
                    }
                    else if (npc.ai[2] > 280)
                    {
                        if (npc.ai[3] == 5)
                        {
                            clock.Kill();//and blow it up
                        }
                        npc.ai[3]++;
                        npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
                        if (npc.ai[3] > 5)
                        {
                            if (npc.ai[2] == 360)
                                SwitchToRandomly(0, DivideAttackStart, 0.3f);
                        }
                        else
                        {
                            int clockId = (int)npc.localAI[0];
                            SwitchTo(3, false);
                            npc.localAI[0] = clockId;
                        }
                    }
                }
                else if (npc.ai[1] == 4)//V charge
                {
                    if (npc.ai[2] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var unit = Vector2.UnitY.RotatedBy(-Math.PI / 12).RotatedByRandom(Math.PI / 6);
                        if (Main.rand.NextBool()) unit = unit.RotatedBy(Math.PI);
                        Vector2 start = player.Center + Vector2.UnitX * (-Math.Sign(unit.X)) * 750
                            + Vector2.UnitY * (-Math.Sign(unit.Y)) * 800;
                        Vector2 end = start + unit * 2000;
                        int index = Projectile.NewProjectile(start, Vector2.Zero, ModContent.ProjectileType<LMAimLine>(),
                                    0, 0f, Main.myPlayer, end.X, end.Y);
                        npc.localAI[0] = index;
                        npc.netUpdate = true;
                    }
                    npc.ai[2]++;
                    if (npc.ai[2] <= 150)
                    {
                        npc.WormMovementEx(player.Center + npc.DirectionFrom(player.Center) * LumiteDestroyerArguments.R * 1.5f,
                            maxSpeed, turnAcc, ramAcc, radiusSpeed: 0.05f, distLimit: 600);
                        if (npc.ai[2] == 150)
                        {
                            npc.Center = Main.projectile[(int)npc.localAI[0]].Center;
                            Vector2 end = new Vector2(Main.projectile[(int)npc.localAI[0]].ai[0], Main.projectile[(int)npc.localAI[0]].ai[1]);
                            npc.velocity = (end - npc.Center).SafeNormalize(Vector2.UnitY) * maxSpeed / 5;
                        }
                    }
                    else if (npc.ai[2] <= 180)
                    {
                        if (npc.velocity.Compare(maxSpeed * 1.5f) < 0) npc.velocity *= 1.1f;
                        if (npc.ai[2] % 6 == 2 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var direction = npc.velocity.SafeNormalize(Vector2.UnitY);
                            Vector2 target = npc.Center +
                                Vector2.Lerp(direction, Vector2.UnitX * Math.Sign(player.Center.X - npc.Center.X), 0.8f) * Math.Abs(player.Center.X - npc.Center.X);
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(),
                                npc.damage / 6, 0f, Main.myPlayer, target.X, target.Y);

                        }
                    }
                    else if (npc.ai[2] >= 200)
                    {
                        npc.ai[3]++;
                        if (npc.ai[3] < 5)
                            SwitchTo(4, false);
                        else
                            SwitchToRandomly(0, DivideAttackStart, 0.3f);
                    }
                }
                #endregion
                #region Divide Attack
                else if (npc.ai[1] == DivideAttackStart)
                {
                    npc.WormMovement(player.Center + targetModifier, maxSpeed / 2, turnAcc, ramAcc / 2);
                    npc.ai[2]++;
                    ForeachSegment((tmpNPC, counter) =>
                    {
                        tmpNPC.alpha += 3;
                        if (tmpNPC.alpha > 255) tmpNPC.alpha = 255;
                    });

                    if (npc.ai[2] >= 120)
                    {
                        Vector2 dist = Main.rand.NextVector2Unit() * 1500;
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            int factor = counter / 28;
                            var offset = dist.RotatedBy(MathHelper.TwoPi / 3 * factor);
                            tmpNPC.Center = player.Center + offset;
                            tmpNPC.velocity = Vector2.Zero;
                            if (counter % 28 == 0)
                            {
                                tmpNPC.localAI[0] = DivideAttackStart + 1;
                                tmpNPC.localAI[1] = (-offset).ToRotation();
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.velocity = -offset.SafeNormalize(Vector2.Zero) * maxSpeed / 6;
                                tmpNPC.netUpdate = true;
                            }
                            else
                            {
                                tmpNPC.localAI[0] = 0;
                                tmpNPC.localAI[1] = 0;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                            }
                        });
                        npc.Center = player.Center + player.velocity * 60f + dist;
                        npc.velocity = -dist.SafeNormalize(Vector2.Zero) * maxSpeed / 6;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var proj = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, Main.myPlayer, -1, -5);
                            proj.scale = 2f;
                        }
                        SwitchTo(DivideAttackStart + 1);
                        npc.localAI[1] = (-dist).ToRotation();
                    }
                }
                else if (npc.ai[1] == DivideAttackStart + 1)
                {
                    npc.ai[2]++;
                    npc.WormMovement(npc.Center + npc.localAI[1].ToRotationVector2() * 600f, maxSpeed * 0.75f, turnAcc * 1.25f, ramAcc);

                    if (npc.ai[2] >= 120)
                    {
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            int factor = counter / 28;
                            if (counter % 28 == 0)
                            {
                                tmpNPC.localAI[0] = DivideAttackStart + factor + 2;
                                tmpNPC.localAI[1] = 0;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                            }
                            else
                            {
                                tmpNPC.localAI[0] = 0;
                                tmpNPC.localAI[1] = 0;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                            }
                        });
                        SwitchTo(DivideAttackStart + 2);
                    }
                }
                #region CoAttack Pattern1
                else if (npc.ai[1] == DivideAttackStart + 2)
                {
                    npc.ai[2]++;
                    npc.WormMovementEx(player.Center + targetModifier, maxSpeed * 0.75f, turnAcc * 1.25f, ramAcc);
                    if (npc.ai[2] >= 450)
                    {
                        if (Main.rand.NextBool())
                        {
                            ForeachSegment((tmpNPC, counter) =>
                            {
                                int factor = counter / 28;
                                if (counter % 28 == 0)
                                {
                                    tmpNPC.localAI[0] = DivideAttackStart + 5;
                                    tmpNPC.localAI[1] = 0;
                                    tmpNPC.localAI[2] = 0;
                                    tmpNPC.netUpdate = true;
                                }
                                else
                                {
                                    tmpNPC.localAI[0] = 0;
                                    tmpNPC.localAI[1] = 0;
                                    tmpNPC.localAI[2] = 0;
                                    tmpNPC.netUpdate = true;
                                }
                            });
                            SwitchTo(DivideAttackStart + 5);
                        }
                        else
                        {
                            SwitchTo(DivideAttackStart + DivideAILength - 1);
                        }
                    }
                }
                else if (npc.ai[1] == DivideAttackStart + 3)
                {
                    CrawlipedeMove();
                }
                else if (npc.ai[1] == DivideAttackStart + 4)
                {
                    CrawlipedeMove(-600);
                }
                #endregion
                #region CoAttack Pattern2
                else if (npc.ai[1] == DivideAttackStart + 5 || npc.ai[1] == DivideAttackStart + 6)
                {
                    //Common code
                    int i = npc.whoAmI;
                    int counter = 0;
                    if (npc.ai[1] == DivideAttackStart + 5)
                    {
                        npc.WormMovementEx(player.Center + targetModifier, maxSpeed * 0.6f, turnAcc * 1.25f, ramAcc);
                        if (npc.ai[3] > 9)
                        {
                            SwitchTo(DivideAttackStart + DivideAILength);
                        }
                    }
                    else if (npc.ai[1] == DivideAttackStart + 6)
                    {
                        //reserved
                        npc.ai[2]++;//the head will create the aim
                        if (npc.ai[2] < 144)
                        {
                            npc.WormMovementEx(player.Center + targetModifier, maxSpeed * 0.6f, turnAcc * 1.25f, ramAcc);
                        }
                        else if (npc.ai[2] == 144)
                        {
                            Projectile aim = Main.projectile[(int)npc.localAI[1]];
                            Vector2 endpoint = new Vector2(aim.ai[0], aim.ai[1]);
                            npc.Center = aim.Center;
                            npc.velocity = (endpoint - npc.Center).SafeNormalize(Vector2.Zero) * maxSpeed / 3;
                            i = npc.whoAmI;
                            counter = 0;
                            while (i != -1)
                            {
                                counter++;
                                NPC tmpNPC = Main.npc[i];
                                if (tmpNPC.localAI[0] >= DivideAttackStart && tmpNPC.whoAmI != npc.whoAmI)
                                {
                                    break;
                                }
                                tmpNPC.Center = aim.Center;
                                i = (int)Main.npc[i].ai[0];
                            }
                            aim.Kill();
                        }
                        else if (npc.ai[2] > 144)
                        {
                            if (npc.velocity.Compare(maxSpeed * 1.5f) < 0) npc.velocity *= 1.2f;
                            if (npc.ai[2] % 6 == 2 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                var direction = npc.velocity.SafeNormalize(Vector2.UnitY);
                                Vector2 target = npc.Center +
                                    Vector2.Lerp(direction, Vector2.UnitX * Math.Sign(player.Center.X - npc.Center.X), 0.8f) * Math.Max(1800,Math.Abs(player.Center.X - npc.Center.X));
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(),
                                    npc.damage / 6, 0f, Main.myPlayer, target.X, target.Y);

                            }
                        }
                        if (npc.ai[2] >= 180)
                        {
                            npc.ai[1] = DivideAttackStart + 5;
                            npc.ai[2] = 0;
                            npc.netUpdate = true;
                            //SwitchTo(DivideAttackStart + 5, false);
                        }
                    }
                    npc.localAI[2]++;
                    while (i != -1)
                    {
                        counter++;
                        NPC tmpNPC = Main.npc[i];
                        if (counter % 28 == 0)
                        {
                            if (tmpNPC.localAI[0] == DivideAttackStart + 5 && Main.rand.Next(60) > 48 && npc.ai[3] <= 9 && npc.localAI[2] >= 45)
                            {
                                var start = player.Center + Main.rand.NextVector2Circular(1200, 1800);
                                var end = player.Center + Main.rand.NextVector2Circular(1200, 1800);
                                if ((end - start).Compare(1200) < 0)
                                {
                                    end = start + (end - start).SafeNormalize(Vector2.UnitY) * 1200;
                                }
                                int index = Projectile.NewProjectile(start, Vector2.Zero, ModContent.ProjectileType<LMAimLine>(),
                                    0, 0f, Main.myPlayer, end.X, end.Y);
                                tmpNPC.localAI[0] = DivideAttackStart + 6;
                                tmpNPC.localAI[1] = index;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                                npc.localAI[2] = 0;
                                npc.ai[3]++;
                            }
                        }
                        else if (i == npc.whoAmI)
                        {
                            if (tmpNPC.ai[1] == DivideAttackStart + 5 && Main.rand.Next(60) > 48 && npc.ai[3] <= 9)
                            {
                                var start = player.Center + Main.rand.NextVector2Circular(1800, 1200);
                                var end = player.Center + Main.rand.NextVector2Circular(1800, 1200);
                                if ((end - start).Compare(1200) < 0)
                                {
                                    end = start + (end - start).SafeNormalize(Vector2.UnitY) * 1200;
                                }
                                int index = Projectile.NewProjectile(start, Vector2.Zero, ModContent.ProjectileType<LMAimLine>(),
                                    0, 0f, Main.myPlayer, end.X, end.Y);
                                tmpNPC.ai[1] = DivideAttackStart + 6;
                                tmpNPC.localAI[1] = index;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                                npc.ai[3]++;
                            }
                        }
                        i = (int)Main.npc[i].ai[0];
                    }
                }
                #endregion
                else if (npc.ai[1] == DivideAttackStart + DivideAILength - 1)
                {
                    if (npc.ai[2] == 0)//set up
                    {
                        npc.ai[3] = Main.rand.NextFloatDirection();
                        npc.velocity = Vector2.Zero;
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            tmpNPC.velocity = Vector2.Zero;
                            int factor = counter / 28;
                            float offsetRotation = MathHelper.TwoPi / 3 * factor;
                            if (counter % 28 == 0)
                            {
                                tmpNPC.localAI[0] = DivideAttackStart + DivideAILength - 1;
                                tmpNPC.localAI[1] = offsetRotation;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                            }
                            else
                            {
                                tmpNPC.localAI[0] = 0;
                                tmpNPC.localAI[1] = 0;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                            }
                        });
                        spinCenter = player.Center + player.velocity * 10;
                    }
                    var fakeCenter = player.Center + player.velocity * 10;
                    spinCenter = (spinCenter * 99f + fakeCenter) / 100f;

                    npc.ai[2]++;
                    var center = spinCenter + npc.ai[3].ToRotationVector2() * LumiteDestroyerArguments.R * 1.25f;
                    npc.HoverMovementEx(center, maxSpeed, 0.6f);

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= 150 && npc.ai[2] % 5 == 0)
                    {
                        Projectile.NewProjectile(npc.Center, (spinCenter - npc.Center).SafeNormalize(Vector2.Zero) * 5f, ModContent.ProjectileType<DeathLaserEx>(), npc.damage / 5, 0f, Main.myPlayer, 90f, -1);
                    }

                    npc.ai[3] += 0.025f;
                    if (npc.ai[3] >= MathHelper.TwoPi)
                    {
                        npc.ai[3] -= MathHelper.TwoPi;
                    }

                    if (npc.ai[2] >= 450)
                    {
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            if (counter % 28 == 0)
                            {
                                tmpNPC.localAI[0] = DivideAttackStart + DivideAILength;
                                tmpNPC.localAI[1] = 0;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                            }
                            else
                            {
                                tmpNPC.localAI[0] = 0;
                                tmpNPC.localAI[1] = 0;
                                tmpNPC.localAI[2] = 0;
                                tmpNPC.netUpdate = true;
                            }
                        });
                        SwitchTo(DivideAttackStart + DivideAILength);
                    }
                }
                else if (npc.ai[1] == DivideAttackStart + DivideAILength)//fade back
                {
                    npc.WormMovement(player.Center + targetModifier, maxSpeed / 2, turnAcc, ramAcc / 2);
                    npc.ai[2]++;
                    ForeachSegment((tmpNPC, counter) =>
                    {
                        tmpNPC.alpha += 3;
                        if (tmpNPC.alpha > 255) tmpNPC.alpha = 255;
                    });

                    if (npc.ai[2] >= 120)
                    {
                        Vector2 dist = Main.rand.NextVector2Unit() * 1000;
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
                        SwitchTo(IsPhase3() ? Main.rand.Next(1, 4) : Main.rand.Next(0, 3));
                    }
                }
                #endregion
            }
            else if (npc.ai[1] < DeathStruggleStart)
            {
                #region Spin Attack
                float r = (float)(LumiteDestroyerArguments.SpinSpeed / LumiteDestroyerArguments.SpinRadiusSpeed);
                if (npc.ai[1] == SpinAttackStart)
                {
                    var center = spinCenter;
                    if (Vector2.Distance(npc.Center, center) <= r)//has moved to the desired position
                    {
                        if (npc.Distance(center) < r)//modify it to retain accuracy
                            npc.position += center + Vector2.Normalize(npc.Center - center) * r - npc.Center;
                        npc.direction = Main.rand.NextBool() ? -1 : 1;
                        npc.velocity = Vector2.Normalize(npc.Center - center)
                            .RotatedBy(-Math.PI / 2 * npc.direction) * LumiteDestroyerArguments.SpinSpeed;
                        npc.rotation = npc.velocity.ToRotation();
                        chaosPlanets[0] = chaosPlanets[1] = chaosPlanets[2] = -1;
                        npc.ai[1] = SpinAttackStart + 1;
                    }
                    else
                    {
                        spinTimer--;//prevent it from spawning chaotic system before performing spin.
                        npc.WormMovement(center, maxSpeed * 1.5f, 0.5f, 0.75f);
                    }
                    if (Vector2.Distance(player.Center, center) > LumiteDestroyerArguments.R)
                    {
                        player.Center = center + Vector2.Normalize(player.Center - center) * LumiteDestroyerArguments.R;
                    }
                }
                else if (npc.ai[1] == SpinAttackStart + 1)
                {
                    npc.chaseable = false;
                    int direction = npc.direction;
                    var center = spinCenter;
                    if (npc.Distance(center) < LumiteDestroyerArguments.R)
                    {
                        npc.Center = center + npc.DirectionFrom(center) * LumiteDestroyerArguments.R;
                    }
                    //player.wingTime = 100;
                    npc.velocity = npc.velocity.RotatedBy(-LumiteDestroyerArguments.SpinRadiusSpeed * direction);
                    npc.rotation -= (float)(LumiteDestroyerArguments.SpinRadiusSpeed * direction);
                    //check all dead
                    if (spinTimer >= spinMaxTime + 200 && chaosPlanets.All(item => item != -1 ?
                          (Main.projectile[item].type != ModContent.ProjectileType<LMChaosMoon>() || (Main.projectile[item].ai[0] == 1 || Main.projectile[item].active == false))
                          : true))
                    {
                        npc.chaseable = true;
                        spinTimer = 0;
                        npc.ai[1] = npc.localAI[2];//reset to normal
                        npc.netUpdate = true;
                    }
                    else if (Vector2.Distance(player.Center, center) > LumiteDestroyerArguments.R)
                    {
                        player.Center = center + Vector2.Normalize(player.Center - center) * LumiteDestroyerArguments.R;
                    }

                    if (spinTimer == spinMaxTime + 180 && Main.netMode != NetmodeID.MultiplayerClient)
                    {//spawn chaos system
                        for (int i = 0; i < chaosPlanets.Length; i++) chaosPlanets[i] = -1;
                        Vector2 offset = new Vector2(450, 0);
                        for (int i = 0; i < chaosPlanetsCount; i++)
                        {
                            chaosPlanets[i] = Projectile.NewProjectile(center + offset, Vector2.Normalize(offset.RotatedBy(MathHelper.TwoPi - MathHelper.TwoPi / chaosPlanetsCount)) * 6f,
                                ModContent.ProjectileType<LMChaosMoon>(), npc.damage / 3, 0f, Main.myPlayer);
                            offset = offset.RotatedBy(MathHelper.TwoPi / chaosPlanetsCount);
                            Main.projectile[chaosPlanets[i]].ai[1] = Main.rand.NextFloat(1, 12);
                        }
                    }
                    else if (spinTimer >= spinMaxTime + 180 && ((spinTimer - spinMaxTime) % 120) == 0)
                    {
                        double baseRotation = Main.rand.NextBool() ? Math.PI / 2 : Math.PI * 3 / 2;
                        const float length = 1200f;
                        var target = spinCenter;
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 pos = Main.rand.NextVector2Unit(MathHelper.Pi / 3, MathHelper.Pi / 3) * length;
                            pos = pos.RotatedBy(baseRotation);
                            Projectile.NewProjectile(target + pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(), 60, 0f, Main.myPlayer, target.X, target.Y);
                        }
                        baseRotation = Main.rand.NextBool() ? Math.PI : 0;
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 pos = Main.rand.NextVector2Unit(MathHelper.Pi / 3, MathHelper.Pi / 3) * length;
                            pos = pos.RotatedBy(baseRotation);
                            Projectile.NewProjectile(target + pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(), 60, 0f, Main.myPlayer, target.X, target.Y);
                        }
                    }
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
                #endregion
            }
            #region Death Struggle
            else if (npc.ai[1] == DeathStruggleStart)
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
                    Vector2 dist = Main.rand.NextVector2Unit() * 1000;
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
                    SwitchTo(DeathStruggleStart + 1);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 1)
            {
                var center = player.Center + player.velocity * 10;
                spinCenter = center;
                Vector2 dist = npc.DirectionFrom(spinCenter);
                dist *= LumiteDestroyerArguments.R * 1.75f;
                dist = center + dist;
                npc.WormMovement(dist, maxSpeed * 5f, turnAcc * 5f);
                SwitchTo(DeathStruggleStart + 2);
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
                    spinTimer--;//prevent it from spawning chaotic system before performing spin.
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
                npc.rotation -= (float)(LumiteDestroyerArguments.SpinRadiusSpeed * direction);
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
                }

                if (npc.ai[2] >= 360)
                {
                    SwitchTo(DeathStruggleStart + 4);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 4)
            {
                npc.ai[2]++;
                if (npc.ai[2] == 120 || npc.ai[2] == 480 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int direction = Main.rand.Next(4);
                    if (npc.ai[2] == 120)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<LMStarSigilEx>(),
                            npc.damage / 8, 0f, Main.myPlayer, 0f, npc.target);
                    if (npc.ai[2] == 480)
                        Projectile.NewProjectile(player.Center + player.velocity * 45 - Vector2.UnitX.RotatedBy(Math.PI / 2 * direction) * 720,
                            Vector2.Zero, ModContent.ProjectileType<LMLaserMatrix>(),
                            npc.damage / 6, 0f, Main.myPlayer, 240, direction);
                }
                if (npc.ai[2] >= 360)
                {
                    maxSpeed *= (1 - (npc.ai[2] - 360) / 360);
                    if (maxSpeed < 0.01f)
                    {
                        maxSpeed = 0.01f;
                    }
                }
                npc.WormMovementEx(player.Center, maxSpeed, turnAcc, ramAcc);
                if (npc.velocity.Compare(maxSpeed) > 0)
                {
                    npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
                }

                if (npc.ai[2] == 720)
                {
                    npc.NewOrBoardcastText("LOW POWER", Color.BlueViolet);
                }
                if (npc.ai[2] >= 750)
                {
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
            else if (npc.ai[1] == DeathStruggleStart + 5)
            {
                npc.SlowDown(0.9f);
                return;
                //do nothing here.
            }
            #endregion

            npc.rotation = npc.velocity.ToRotation();
            //Lighting.AddLight(npc.Center, 0.3f, 0.3f, 0.5f);
        }
    }
}
