using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.Projectiles.LumiteDestroyer;
using SunksBossChallenges.Projectiles.DecimatorOfPlanets;
using SunksBossChallenges.Projectiles;

namespace SunksBossChallenges.NPCs.LumiteDestroyer
{
    public class LumiteDestroyerBody : LumiteDestroyerSegment
    {
        readonly int segDistance = 38;
        int PreviousIndex => (int)npc.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nova Annihilator");
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
            npc.damage = 100;
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
                if (Main.npc[(int)npc.realLife].type != ModContent.NPCType<LumiteDestroyerHead>() || !Main.npc[(int)npc.realLife].active
                    || Main.npc[(int)npc.realLife].ai[1] >= DeathStruggleStart + 5)
                    dead = true;
            }
            if (Main.npc[(int)npc.realLife].type != ModContent.NPCType<LumiteDestroyerHead>() || !Main.npc[(int)npc.realLife].active)
            {
                dead = true;
            }
            
            if (dead)
            {
                npc.ai[2]++;
				
                if (npc.ai[2] >= npc.localAI[3])
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.checkDead();
                }
                return;
            }
            if (Main.npc[(int)npc.realLife].target < 0 || Main.npc[(int)npc.realLife].target == 255 || Main.player[Main.npc[(int)npc.realLife].target].dead)
            {
                if (npc.localAI[0] >= DivideAttackStart)//acting as head
                {
                    npc.WormMovement(npc.Center + new Vector2(0, -900f), 25f);
                    npc.rotation = npc.velocity.ToRotation();
                }
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
                            ImmuneTimer = 180;
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
                        ImmuneTimer = 180;
                    }
                    npc.alpha -= 42;
                    if (npc.alpha < 0)
                    {
                        npc.alpha = 0;
                    }
                }
            }
            if (ImmuneTimer > 0) --ImmuneTimer;
            #endregion
            #region Music
            if (head.ai[1] >= 0)
            {
                if (music != mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Frontier"))
                    music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Frontier");
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

            npc.dontTakeDamage = (head.modNPC as LumiteDestroyerHead).CanBeTransparent()
                || (head.modNPC as LumiteDestroyerHead).IsDeathStruggling();
            if (npc.localAI[0] < DivideAttackStart)//normal state
            {
                if (npc.ai[1] >= 0f && npc.ai[1] < Main.npc.Length)
                {
                    if ((head.ai[1] == HalfCircleDash && head.ai[2] >= 180)
                        || (head.ai[1] >= SpinAttackStart && head.ai[1] < DeathStruggleStart) 
                        || (head.ai[1] >= DeathStruggleStart + 2 && head.ai[1] <= DeathStruggleStart + 4))//spinning or preparing spinning
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

                        if (npc.localAI[1] > 0)
                        {
                            npc.localAI[1]--;
                            if (npc.localAI[1] <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                npc.localAI[1] = 0;
                                if (head.ai[1] == HalfCircleDash)
                                {
                                    var target = (npc.Center - pivot) * 3 + pivot;
                                    Projectile.NewProjectile(pivot, (player.Center - npc.Center).SafeNormalize(Vector2.Zero)
                                        , ModContent.ProjectileType<LaserBarrage>(), npc.damage / 5, 0f, Main.myPlayer, target.X, target.Y);
                                }
                            }
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
                                npc.localAI[1] = 0;
                                if (head.ai[1] == -2)
                                {
                                    Projectile.NewProjectile(npc.Center, npc.localAI[0].ToRotationVector2() * 20, ModContent.ProjectileType<DarkStar>(), npc.damage / 5, 0f, Main.myPlayer);
                                }
                                else if(head.ai[1]==SigilStar)
                                {
                                    Projectile.NewProjectile(npc.Center, (player.Center - npc.Center).SafeNormalize(Vector2.UnitY) * 18f,
                                         ModContent.ProjectileType<LMSigilStarUnit>(), npc.damage / 5, 0f, Main.myPlayer, npc.localAI[3], npc.localAI[2]);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (npc.localAI[0] >= DivideAttackStart)//acting as head
                {
                    var maxSpeed = 15f + player.velocity.Length() / 3;
                    float turnAcc = 0.15f;
                    float ramAcc = 0.15f;
                    if (Main.expertMode)
                        maxSpeed *= 1.125f;
                    //if (Main.getGoodWorld)
                    //    maxSpeed *= 1.25f;
                    maxSpeed = maxSpeed * 0.9f + maxSpeed * ((head.lifeMax - head.life) / (float)head.lifeMax) * 0.2f;
                    maxSpeed = Math.Max(player.velocity.Length() * 1.5f, maxSpeed);
                    if (npc.localAI[0] == DivideAttackStart + 1)
                    {
                        npc.WormMovement(npc.Center + npc.localAI[1].ToRotationVector2() * 600f, maxSpeed * 0.75f, turnAcc * 1.25f, ramAcc);
                    }
                    #region CoAttack Pattern1
                    else if (npc.localAI[0] == DivideAttackStart + 2)
                    {
                        npc.WormMovementEx(player.Center + targetModifier, maxSpeed * 0.75f, turnAcc * 1.25f, ramAcc);
                    }
                    else if (npc.localAI[0] == DivideAttackStart + 3)
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
                    else if (npc.localAI[0] == DivideAttackStart + 4)
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
                                targetPos.Y = heightOffset + 600;
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
                    else if (npc.localAI[0] == DivideAttackStart + 5)
                    {
                        npc.WormMovementEx(player.Center + targetModifier, maxSpeed * 0.6f, turnAcc * 1.25f, ramAcc);
                    }
                    else if (npc.localAI[0] == DivideAttackStart + 6)
                    {
                        npc.localAI[2]++;//the head will create the aim
                        if (npc.localAI[2] < 144)
                        {
                            npc.WormMovementEx(player.Center + targetModifier, maxSpeed * 0.6f, turnAcc * 1.25f, ramAcc);
                        }
                        else if (npc.localAI[2] == 144)
                        {
                            Projectile aim = Main.projectile[(int)npc.localAI[1]];
                            Vector2 endpoint = new Vector2(aim.ai[0], aim.ai[1]);
                            npc.Center = aim.Center;
                            npc.velocity = (endpoint - npc.Center).SafeNormalize(Vector2.Zero) * maxSpeed;
                            int i = npc.whoAmI;
                            int counter = 0;
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
                        else if (npc.localAI[2] > 144)
                        {
                            if (npc.velocity.Compare(maxSpeed * 1.5f) < 0) npc.velocity *= 1.2f;
                            if (npc.localAI[2] % 6 == 2 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                var direction = npc.velocity.SafeNormalize(Vector2.UnitY);
                                Vector2 target = npc.Center +
                                    Vector2.Lerp(direction, Vector2.UnitX * Math.Sign(player.Center.X - npc.Center.X), 0.8f) * Math.Max(1800, Math.Abs(player.Center.X - npc.Center.X));
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.DecimatorOfPlanets.LaserBarrage>(),
                                    head.damage / 6, 0f, Main.myPlayer, target.X, target.Y);

                            }
                        }
                        if (npc.localAI[2] >= 180)
                        {
                            npc.localAI[0] = DivideAttackStart + 5;
                            npc.localAI[1] = 0;
                            npc.localAI[2] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    #endregion
                    else if (npc.localAI[0] == DivideAttackStart + DivideAILength - 1)
                    {
                        npc.localAI[2]++;
                        var fakeCenter = (head.modNPC as LumiteDestroyerHead).spinCenter;
                        var center = fakeCenter + head.ai[3].ToRotationVector2().RotatedBy(npc.localAI[1])
                            * LumiteDestroyerArguments.R * 1.25f;
                        npc.HoverMovementEx(center, maxSpeed, 0.6f);
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[2]>=150 && npc.localAI[2] % 5 == 0)
                        {
                            Projectile.NewProjectile(npc.Center, (fakeCenter - npc.Center).SafeNormalize(Vector2.Zero) * 5f, ModContent.ProjectileType<DeathLaserEx>(), head.damage / 5, 0f, Main.myPlayer, 90f, -1);
                        }
                    }
                    else if (npc.localAI[0] == DivideAttackStart + DivideAILength)
                    {
                        npc.WormMovement(player.Center + targetModifier, maxSpeed / 2, turnAcc, ramAcc / 2);
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
            Color glowColor = Color.White;
            SpriteEffects effects = (npc.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = drawColor;
            /*if (head.ai[1] == 3)
            {
                if (head.ai[2] >= 240)
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
            if ((head.ai[1] == DivideAttackStart + 5) || (head.ai[1] == DivideAttackStart + 6))
            {
                /*if (head.ai[2] >= 10)
                {
                    if(npc.localAI[0]==DivideAttackStart+5)
                        npc.DrawAim(spriteBatch, npc.Center + new Vector2(LumiteDestroyerArguments.R, 0) * 5 * (-head.ai[3]), Color.Red);
                    if(npc.localAI[0]==DivideAttackStart+6)
                        npc.DrawAim(spriteBatch, npc.Center + new Vector2(0, LumiteDestroyerArguments.R) * 5 * (-head.ai[3]), Color.Red);
                }*/
            }
            else if (head.ai[1] == DeathStruggleStart + 4)
            {
                if (head.ai[2] >= 360)
                {
                    float alpha = 1 - (head.ai[2] - 360) / 360;
                    if (alpha < 0) alpha = 0;
                    glowColor *= alpha;
                    mainColor = Color.Lerp(mainColor, Color.Black, alpha / 2);
                }
            }
            else if (head.ai[1] == DeathStruggleStart + 5)
            {
                glowColor *= 0;
                mainColor = Color.Lerp(mainColor, Color.Black, 0.5f);
            }
            spriteBatch.Draw(texture2D, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), mainColor * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
            spriteBatch.Draw(DestTexture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Rectangle?(npc.frame), glowColor * 0.75f * npc.Opacity, npc.rotation + MathHelper.Pi / 2, npc.frame.Size() / 2f, npc.scale, effects, 0f);
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
        public override bool CheckDead()
        {
            if (npc.realLife != -1 && Main.npc[npc.realLife].type == ModContent.NPCType<LumiteDestroyerHead>() && Main.npc[npc.realLife].active)
            {
                return Main.npc[npc.realLife].ai[1] >= DeathStruggleStart + 5;
            }
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
            base.ReceiveExtraAI(reader);
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
                return !(Main.npc[npc.realLife].modNPC as LumiteDestroyerHead).CanBeTransparent() 
                    && (Main.npc[npc.realLife].ai[1] != DeathStruggleStart + 5);
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
                    /*if (head.life <= head.lifeMax * LumiteDestroyerArguments.Phase2HealthFactor)
                    {
                        if (head.ai[1] >= DivideAttackStart && head.ai[1] <= DivideAttackStart + DivideAILength)
                        {
                            damage *= (1 - 0.80);
                        }
                        else
                        {
                            damage *= (1 - 0.75);
                        }
                    }*/
                    damage *= (1 - (head.modNPC as LumiteDestroyerHead).DynDR);
                    if (npc.alpha > 0 || Main.npc[npc.realLife].ai[1] >= SpinAttackStart)
                        damage *= (1 - 0.99);
                }
            }
            else
            {
                if (npc.alpha > 0)
                    damage *= (1 - 0.99);
            }
            return base.StrikeNPC(ref damage, defense, ref knockback, hitDirection, ref crit);
        }
    }
}
