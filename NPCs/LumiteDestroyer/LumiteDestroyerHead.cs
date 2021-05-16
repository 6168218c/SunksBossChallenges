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
    public class LumiteDestroyerHead : LumiteDestroyerSegment
    {
        internal Vector2 spinCenter;
        internal int spinTimer = 0;
        internal int[] chaosPlanets = new int[3];

        internal Vector2 healBarPos;//this one doesn't need to be synchronized.
        int Length => 80;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LD-002 \"Destruction\"");
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

        public override void AI()
        {
            //string aiDump = $"ai:{string.Join(",", npc.ai.Select(fl => $"{fl}"))}";
            //aiDump += $" localAI:{string.Join(",", npc.localAI.Select(fl => $"{fl}"))}";
            //Main.NewText($"{aiDump}");
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
            void SwitchToRandomly(float normalAI1,float randomAI1,float possibility)
            {
                if (Main.rand.NextFloat() < possibility)
                {
                    SwitchTo(randomAI1);
                }
                else
                {
                    SwitchTo(normalAI1);
                }
            }
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
                    npc.WormMovement(npc.Center + new Vector2(0, -900f), 25f);
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

                npc.ai[1] = -1f;//not in phase 2
            }

            int spinMaxTime = 2700;
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

            if (Main.rand.Next(10) > 8 && isPhase2 &&!CanBeTransparent())
            {
                int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Electric, 0f, 0f, 100, default, 2f);
                Main.dust[num].noGravity = true;
                Main.dust[num].noLight = true;
                Main.dust[num].scale = 0.4f;
                Main.dust[num].velocity = Main.rand.NextVector2Unit(npc.rotation - 0.001f, npc.rotation + 0.001f) * 9f;
            }

            Vector2 targetModifier = player.velocity;
            if (spinTimer >= spinMaxTime - 300 && (npc.ai[1] < DivideAttackStart || npc.ai[1] >= SpinAttackStart))
            {
                if (spinTimer == spinMaxTime - 300)
                {
                    Main.PlaySound(SoundID.Roar, npc.Center);
                }
                targetModifier = npc.DirectionFrom(player.Center) * LumiteDestroyerArguments.R * 1.5f;
            }

            if (isPhase2 && (npc.ai[1] < DivideAttackStart || npc.ai[1] >= SpinAttackStart))//ensure not in split attack
            {
                spinTimer++;
            }
            int chaosPlanetsCount = 3;

            /*if (npc.ai[1] == SpinAttackStart + 1 && spinTimer == spinMaxTime + 180 && Main.netMode != NetmodeID.MultiplayerClient)
            {//spawn chaos system
                for (int i = 0; i < chaosPlanets.Length; i++) chaosPlanets[i] = -1;
                var center = spinCenter;
                Vector2 offset = new Vector2(450, 0);
                for (int i = 0; i < chaosPlanetsCount; i++)
                {
                    chaosPlanets[i] = Projectile.NewProjectile(center + offset, Vector2.Normalize(offset.RotatedBy(MathHelper.TwoPi - MathHelper.TwoPi / chaosPlanetsCount)) * 6f,
                        ModContent.ProjectileType<ChaosMoon>(), npc.damage / 3, 0f, Main.myPlayer);
                    offset = offset.RotatedBy(MathHelper.TwoPi / chaosPlanetsCount);
                    Main.projectile[chaosPlanets[i]].ai[1] = Main.rand.NextFloat(1, 12);
                }
            }
            else if (npc.ai[1] == SpinAttackStart + 1 && spinTimer >= spinMaxTime + 180 && ((spinTimer - spinMaxTime) % 120) == 0)
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
            }*/

            if (Vector2.Distance(npc.Center, player.Center) >= 4500f)
            {
                maxSpeed *= 5f;
                turnAcc *= 5f;
            }
            else if (npc.velocity.Length() > maxSpeed * 2.4 && npc.ai[1] != 2 && npc.ai[1] < DivideAttackStart)
            {
                npc.SlowDown(0.9f);
            }

            if(spinTimer >= spinMaxTime)
            {
                if((npc.ai[1] < DivideAttackStart) || (npc.ai[1] >= SpinAttackStart))//ensure not in split attack
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
                        //Vector2 destination = Vector2.Normalize(npc.Center - center) * r;
                        //if (destination == Vector2.Zero || destination.HasNaNs())
                        //    destination = center + new Vector2(0, r);
                        //npc.velocity = (destination-npc.Center) / 2;//arrive in two ticks,leaving time for other segments to react
                    }
                }
            }

            npc.dontTakeDamage = CanBeTransparent() || IsDeathStruggling();

            if (spinTimer <= 240 && isPhase2)
            {
                targetModifier = npc.DirectionFrom(player.Center) * LumiteDestroyerArguments.R * 1.5f;
                npc.WormMovement(player.Center + targetModifier, maxSpeed * 0.9f, turnAcc, ramAcc);
                npc.rotation = npc.velocity.ToRotation();
                return;
            }

            if (npc.ai[1] < SpinAttackStart)
            {
                #region Normal Attack
                if (npc.ai[1] == -1)
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
                        SwitchTo(0);
                        SkyManager.Instance.Activate("SunksBossChallenges:LumiteDestroyer");
                        music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CyberMeteoroid");
                    }
                }
                else if (npc.ai[1] == 0)
                {
                    if (npc.ai[2] % 300 == 0)
                    {
                        double baseRotation = Main.rand.NextBool() ? Math.PI / 2 : Math.PI * 3 / 2;
                        const float length = 1200f;
                        var target = player.Center + player.velocity * 45f;
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 pos = Main.rand.NextVector2Unit(MathHelper.Pi / 3, MathHelper.Pi / 3) * length;
                            pos = pos.RotatedBy(baseRotation);
                            Projectile.NewProjectile(target + pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(), 60, 0f, Main.myPlayer, target.X, target.Y);
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            target += Main.rand.NextVector2Unit() * 100f;
                            Vector2 pos = Main.rand.NextVector2Unit(MathHelper.Pi / 3, MathHelper.Pi / 3) * length;
                            pos = pos.RotatedBy(baseRotation);
                            Projectile.NewProjectile(target + pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(), 60, 0f, Main.myPlayer, target.X, target.Y);
                        }
                        if (Main.rand.Next(5) == 3)
                        {
                            baseRotation = Main.rand.NextBool() ? Math.PI : 0;
                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 pos = Main.rand.NextVector2Unit(MathHelper.Pi / 3, MathHelper.Pi / 3) * length;
                                pos = pos.RotatedBy(baseRotation);
                                Projectile.NewProjectile(target + pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(), 60, 0f, Main.myPlayer, target.X, target.Y);
                            }
                            for (int i = 0; i < 5; i++)
                            {
                                target += Main.rand.NextVector2Unit() * 100f;
                                Vector2 pos = Main.rand.NextVector2Unit(MathHelper.Pi / 3, MathHelper.Pi / 3) * length;
                                pos = pos.RotatedBy(baseRotation);
                                Projectile.NewProjectile(target + pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(), 60, 0f, Main.myPlayer, target.X, target.Y);
                            }
                        }
                    }

                    npc.ai[2]++;
                    npc.WormMovementEx(player.Center + targetModifier, maxSpeed, turnAcc, ramAcc, 0.05f, 600);
                    if (npc.ai[2] >= 1200)
                    {
                        SwitchToRandomly(1, DivideAttackStart, 0.25f);
                    }
                }
                else if (npc.ai[1] == 1)
                {
                    maxSpeed *= 0.8f;
                    turnAcc *= 1.5f;

                    if (npc.ai[2] % 450 == 200)
                    {
                        if (spinTimer <= 300 || spinTimer >= spinMaxTime - 300)
                        {
                            npc.ai[2]--;
                        }
                        else
                        {
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
                            int counter = 0;
                            while (i != -1)
                            {
                                Main.npc[i].localAI[0] = 0;
                                Main.npc[i].localAI[1] = generator(counter++);
                                i = (int)Main.npc[i].ai[0];
                            }
                        }
                    }

                    npc.ai[2]++;

                    if (npc.localAI[1] > 0)
                    {
                        npc.localAI[1]--;
                        if (npc.localAI[1] <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.localAI[1] = 0;
                            if (npc.localAI[0] == 0)
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DeathLaserEx>(), npc.damage / 5, 0f, Main.myPlayer, 36f, npc.target);
                        }
                    }

                    npc.WormMovementEx(player.Center + targetModifier, maxSpeed, turnAcc, ramAcc, 0.1f, 900);

                    if (npc.ai[2] >= 900)
                    {
                        SwitchToRandomly(2, DivideAttackStart, 0.45f);
                    }
                }
                else if (npc.ai[1] == 2)
                {
                    maxSpeed *= 0.6f;
                    turnAcc *= 2f;

                    npc.ai[2]++;

                    if (npc.ai[2] == 150 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var proj = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, Main.myPlayer, -1);
                        (proj.modProjectile as GlowRing).color = Color.Purple;
                    }

                    if (npc.ai[2] >= 200)
                    {
                        if (npc.ai[2] == 200)
                        {
                            npc.velocity = Vector2.Normalize(player.Center - npc.Center) * npc.velocity.Length();
                        }
                        npc.WormMovement(player.Center + targetModifier, maxSpeed * (6f + Vector2.Distance(npc.Center, player.Center) / 1500f) + player.velocity.Length(), turnAcc / 10f, ramAcc * 10f);
                        if (npc.ai[2] >= 235)
                        {
                            if (npc.velocity.Compare(maxSpeed) > 0)
                                npc.SlowDown(0.98f);
                        }
                        if (npc.ai[2] >= 250)
                        {
                            npc.ai[2] = 0;
                            npc.ai[3]++;
                        }
                    }
                    else
                    {
                        if (Vector2.Distance(npc.Center, player.Center) >= 4500f)
                        {
                            maxSpeed /= 5f;
                            turnAcc /= 5f;
                        }
                        else if (Vector2.Distance(npc.Center, player.Center) >= 2000f)
                        {
                            turnAcc *= 2f;
                        }
                        npc.WormMovement(player.Center + targetModifier, maxSpeed, turnAcc, ramAcc);
                        npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
                    }

                    if (npc.ai[3] >= 5 && npc.ai[2] >= 100)
                    {
                        SwitchToRandomly(0, DivideAttackStart, 0.6f);
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
                        npc.Center = player.Center + player.velocity * 60f + dist;
                        npc.velocity = -dist.SafeNormalize(Vector2.Zero) * maxSpeed / 6;
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            int factor = counter / 28;
                            var offset = dist.RotatedBy(MathHelper.TwoPi / 3 * factor);
                            tmpNPC.Center = player.Center + offset;
                            tmpNPC.velocity = Vector2.Zero;
                            if (counter % 28 == 0)
                            {
                                tmpNPC.localAI[0] = DivideAttackStart + 1;
                                tmpNPC.localAI[1] = 0;
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
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var proj = Projectile.NewProjectileDirect(player.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, Main.myPlayer, -1);
                            proj.scale = 2f;
                            (proj.modProjectile as GlowRing).color = Color.Purple;
                        }
                        SwitchTo(DivideAttackStart + 1);
                    }
                }
                else if (npc.ai[1] == DivideAttackStart + 1)
                {
                    npc.ai[2]++;
                    npc.WormMovement(player.Center + targetModifier, maxSpeed * 0.75f, turnAcc * 1.25f, ramAcc);

                    if (npc.ai[2] >= 600)
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
                        SwitchTo(Main.rand.NextBool() ? DivideAttackStart + 2 : DivideAttackStart + DivideAILength - 1);
                    }
                }
                #region CoAttack Pattern1
                else if (npc.ai[1] == DivideAttackStart + 2)
                {
                    npc.ai[2]++;
                    npc.WormMovementEx(player.Center + targetModifier, maxSpeed * 0.75f, turnAcc * 1.25f, ramAcc);
                    if (npc.ai[2] >= 900)
                    {
                        SwitchTo(DivideAttackStart + 5);
                    }
                }
                else if (npc.ai[1] == DivideAttackStart + 3)
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
                            targetPos.Y = heightOffset - 200;
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
                else if (npc.ai[1] == DivideAttackStart + 4)
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
                            targetPos.Y = heightOffset + 1200;
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
                #region CoAttack Pattern2
                else if (npc.ai[1] == DivideAttackStart + 5)
                {
                    if (npc.ai[2] == 0)
                    {
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            int factor = counter / 28;
                            if (counter % 28 == 0)
                            {
                                tmpNPC.localAI[0] = DivideAttackStart + 5;
                                tmpNPC.localAI[1] = factor == 1 ? 1 : -1;
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
                        npc.ai[3] = Main.rand.NextBool() ? 1 : -1;
                    }
                    Vector2 dest = player.Center + Vector2.UnitX * (-npc.ai[3]) * LumiteDestroyerArguments.R * 1.8f;
                    if (npc.ai[2] <= 10)
                    {
                        npc.FastMovement(dest);
                    }
                    else if (npc.ai[2] < 75)
                    {
                        npc.HoverMovementEx(dest, maxSpeed, 0.8f);
                    }
                    else if (npc.ai[2] < 225)
                    {
                        npc.velocity.Y = 0;
                        if (npc.ai[2] == 135 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var velocity = Vector2.UnitX * Math.Sign(player.Center.X - npc.Center.X);
                            Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<DestroyerDeathRay>(), npc.damage, 0f, Main.myPlayer, 90, npc.whoAmI);
                        }
                        npc.WormMovement(npc.Center + new Vector2(1000, 0) * npc.ai[3], maxSpeed * 1.35f, turnAcc, ramAcc);
                    }
                    npc.ai[2]++;
                    if (npc.ai[2] == 225)
                    {
                        SwitchTo(DivideAttackStart + 6);
                    }
                }
                else if (npc.ai[1] == DivideAttackStart + 6)
                {
                    if (npc.ai[2] == 0)
                    {
                        ForeachSegment((tmpNPC, counter) =>
                        {
                            int factor = counter / 28;
                            if (counter % 28 == 0)
                            {
                                tmpNPC.localAI[0] = DivideAttackStart + 6;
                                tmpNPC.localAI[1] = factor == 1 ? 1 : -1;
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
                        npc.ai[3] = Main.rand.NextBool() ? 1 : -1;
                    }
                    Vector2 dest = player.Center + Vector2.UnitY * (-npc.ai[3]) * LumiteDestroyerArguments.R * 1.8f;
                    if (npc.ai[2] <= 10)
                    {
                        npc.FastMovement(dest);
                    }
                    else if (npc.ai[2] < 75)
                    {
                        npc.HoverMovementEx(dest, maxSpeed, 0.8f);
                    }
                    else if (npc.ai[2] < 225)
                    {
                        npc.velocity.X = 0;
                        if (npc.ai[2] == 135 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var velocity = Vector2.UnitY * Math.Sign(player.Center.Y - npc.Center.Y);
                            Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<DestroyerDeathRay>(), npc.damage, 0f, Main.myPlayer, 90, npc.whoAmI);
                        }
                        npc.WormMovement(npc.Center + new Vector2(0, 1000) * npc.ai[3], maxSpeed * 1.35f, turnAcc, ramAcc);
                    }
                    npc.ai[2]++;
                    if (npc.ai[2] == 225)
                    {
                        SwitchTo(DivideAttackStart + DivideAILength);
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

                    if (npc.ai[2] >= 900)
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
                        SwitchTo(Main.rand.Next(0, 3));
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
                          (Main.projectile[item].type != ModContent.ProjectileType<ChaosMoon>() || (Main.projectile[item].ai[0] == 1 || Main.projectile[item].active == false))
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
                                ModContent.ProjectileType<ChaosMoon>(), npc.damage / 3, 0f, Main.myPlayer);
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

                if (npc.ai[2] == 120)
                {
                    npc.NewOrBoardcastText("OVERLOADED MODE ON", Color.Purple, false);
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
            else if(npc.ai[1] == DeathStruggleStart + 3)
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

                if (npc.ai[2] >= 720)
                {
                    SwitchTo(DeathStruggleStart + 4);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 4)
            {
                npc.ai[2]++;
                if (npc.ai[2] >= 240)
                {
                    maxSpeed *= (1 - (npc.ai[2] - 240) / 240);
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

                if (npc.ai[2] == 500)
                {
                    npc.NewOrBoardcastText("LOW POWER", Color.Purple);
                }
                if (npc.ai[2] >= 540)
                {
                    SwitchTo(DeathStruggleStart + 5);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 5)
            {
                //do nothing here.
            }
            #endregion

            npc.rotation = npc.velocity.ToRotation();
            //Lighting.AddLight(npc.Center, 0.3f, 0.3f, 0.5f);
        }
        public bool IsDeathStruggling()
        {
            return npc.ai[1] >= DeathStruggleStart && npc.ai[1] < DeathStruggleStart + 5;
        }
        public bool CanBeTransparent()
        {
            return (npc.ai[1] == DivideAttackStart || npc.ai[1] == DivideAttackStart + DivideAILength
                || npc.ai[1] == DeathStruggleStart);
        }

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
            Color glowColor = npc.ai[1] != 1 ? Color.White : Color.Lerp(Color.White, Color.Red, (float)Math.Sin(MathHelper.Pi / 14 * npc.localAI[2]));
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
                        npc.DrawAim(spriteBatch, npc.Center + new Vector2(LumiteDestroyerArguments.R, 0) * 2 * npc.ai[3], Color.Red);
                    if (npc.ai[1] == DivideAttackStart + 6)
                        npc.DrawAim(spriteBatch, npc.Center + new Vector2(0, LumiteDestroyerArguments.R) * 2 * npc.ai[3], Color.Red);
                }
            }
            else if (npc.ai[1] == DeathStruggleStart + 4)
            {
                if (npc.ai[2] >= 240)
                {
                    float alpha = 1 - (npc.ai[2] - 240) / 240;
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
