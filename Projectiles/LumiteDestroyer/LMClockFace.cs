using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SunksBossChallenges.NPCs.LumiteDestroyer;

namespace SunksBossChallenges.Projectiles.LumiteDestroyer
{
    public class LMClockFace:ModProjectile
    {
        protected int selectedArea = 0;
        protected int lastArea = -1;
        protected int areaTimer;
        public static int ClockR => 640;
        protected float lastLaserRotation = float.MinValue;
        public override string Texture => "SunksBossChallenges/Projectiles/LumiteDestroyer/ClockFace";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clock Face");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 4;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }
        public override void AI()
        {
            NPC head = Main.npc[(int)projectile.ai[0]];
            if (!head.active || head.type != ModContent.NPCType<LumiteDestroyerHead>() || head.ai[1] >= LumiteDestroyerSegment.DeathStruggleStart)//this will heavily affect the head's behavior so we used it
            {
                projectile.Kill();
                return;
            }
            Player player = Main.player[head.target];
            projectile.alpha -= 25;
            if (projectile.alpha < 0) projectile.alpha = 0;
            if (head.ai[1] == LumiteDestroyerSegment.ChronoDash && !player.dead)
            {
                if(head.ai[2] <= 180)
                    projectile.Center = (player.Center + projectile.Center * 64) / 65;
                if (head.ai[2] == 231 && head.ai[3] < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 unit;
                    int direction = Main.rand.NextBool() ? -1 : 1;
                    if (Main.rand.NextBool())
                    {
                        /*Vector2 unit = -Vector2.UnitY;
                        for (int i = 0; i < 12; i += 2)
                        {
                            Vector2 projPos = projectile.Center + unit * ClockR;
                            Projectile.NewProjectile(projPos, unit * 8f, ModContent.ProjectileType<DeathLaserEx>(),
                                projectile.damage /5, 0f, projectile.owner, 36f, head.target);
                            unit = unit.RotatedBy(-MathHelper.Pi / 6);
                        }*/
						unit = -Vector2.UnitY;
                        for (int i = 1; i < 12; i += 2)
                        {
                            Vector2 projPos = player.Center + unit * ClockR * direction;
                            Projectile.NewProjectile(projPos, Vector2.Zero, ModContent.ProjectileType<DecimatorOfPlanets.LaserBarrage>(),
                                projectile.damage / 3, 0f, projectile.owner, player.Center.X, player.Center.Y);
                            unit = unit.RotatedBy(-MathHelper.Pi / 6);
                        }
                    }
                    else
                    {
                        unit = -Vector2.UnitY;
                        for (int i = 0; i < 12; i += 2)
                        {
                            Vector2 projPos = player.Center + unit * ClockR * direction;
                            Projectile.NewProjectile(projPos, Vector2.Zero, ModContent.ProjectileType<DecimatorOfPlanets.LaserBarrage>(),
                                projectile.damage / 3, 0f, projectile.owner, player.Center.X, player.Center.Y);
                            unit = unit.RotatedBy(-MathHelper.Pi / 6);
                        }
                    }
                    unit = lastLaserRotation.ToRotationVector2();
                    Projectile ray = Projectile.NewProjectileDirect(projectile.Center, unit, ModContent.ProjectileType<DestroyerDeathRay>(),
                        projectile.damage, 0f, projectile.owner, 135, projectile.ai[0]);
                    ray.localAI[1] = 2f;
                    ray.netUpdate = true;
                    ray = Projectile.NewProjectileDirect(projectile.Center, -unit, ModContent.ProjectileType<DestroyerDeathRay>(),
                        projectile.damage, 0f, projectile.owner, 135, projectile.ai[0]);
                    ray.localAI[1] = 2f;
                    ray.netUpdate = true;
                    lastLaserRotation = projectile.localAI[0];
                }
                if (head.ai[2] == 235 && head.ai[3] < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 baseUnit;
                    switch (selectedArea)
                    {
                        case 0:
                            baseUnit = (player.Center - projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.Pi / 9);
                            for(int i = 0; i < 3; i++)
                            {
                                Projectile.NewProjectile(projectile.Center,
                                baseUnit * 12f,
                                ProjectileID.CultistBossFireBall,
                                projectile.damage / 3, 0f, Main.myPlayer);
                                baseUnit = baseUnit.RotatedBy(MathHelper.Pi / 18);
                            }
                            break;
                        case 1:
                            baseUnit = Vector2.UnitY.RotatedBy(Math.PI / 12);
                            for (int i = 0; i < 12; i++)
                            {
                                Projectile.NewProjectile(projectile.Center,
                                baseUnit * 12f,
                                ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(),
                                projectile.damage / 3, 0f, Main.myPlayer);
                                baseUnit = baseUnit.RotatedBy(MathHelper.Pi / 6);
                            }
                            break;
                        case 2:
                            baseUnit = -Vector2.UnitY;
                            for(int i = 0; i < 6; i++)
                            {
                                Projectile.NewProjectile(projectile.Center,
                                baseUnit * 36f,
                                ProjectileID.NebulaSphere,
                                projectile.damage / 3, 0f, Main.myPlayer, player.whoAmI);
                                baseUnit = baseUnit.RotatedBy(MathHelper.Pi / 3);
                            }
                            break;
                        case 3:
                            baseUnit = Vector2.UnitY.RotatedBy(Math.PI / 12);
                            for (int i = 0; i < 12; i++)
                            {
                                Projectile.NewProjectile(projectile.Center,
                                baseUnit*18f,
                                ProjectileID.CultistBossLightningOrbArc,
                                projectile.damage / 3, 0f, Main.myPlayer, baseUnit.ToRotation());
                                baseUnit = baseUnit.RotatedBy(MathHelper.Pi / 4);
                            }
                            break;
                    }
                }
                //head.localAI[0] = projectile.whoAmI;
            }
            /*projectile.alpha -= 25;
            if (projectile.alpha < 0) projectile.alpha = 0;
            Player player = Main.player[(int)projectile.ai[0]];*/
            if (player.DistanceSQ(projectile.Center) > ClockR * ClockR)
            {
                player.Center = projectile.Center + player.DirectionFrom(projectile.Center) * ClockR;
            }

            
            //update timer rotations
            double fullTime = GetTime();
            selectedArea = (int)(fullTime / 3600 % 12) / 3;
            //visuals
            if (head.ai[3] >= 5)
            {
                selectedArea = -1;
                areaTimer++;
                if (areaTimer > 45)
                {
                    areaTimer = 45;
                }
            }
            else if (selectedArea != lastArea)
            {
                areaTimer++;
                if (areaTimer > 45)
                {
                    lastArea = selectedArea;
                    areaTimer = 0;
                }
            }

            float hourHand = (float)(fullTime / (3600 * 24) * MathHelper.TwoPi * 2);
            float minHand = (float)(fullTime % 3600 / 3600 * MathHelper.TwoPi);
            projectile.localAI[0] = MathHelper.WrapAngle(hourHand - MathHelper.PiOver2);
            projectile.localAI[1] = MathHelper.WrapAngle(minHand - MathHelper.PiOver2);
            if (lastLaserRotation == float.MinValue)
            {
                lastLaserRotation = projectile.localAI[0];
            }
        }
        protected static double GetTime()
        {
            double FullTime = Main.dayTime ? Main.time + 3600 * 4.5 : Main.time + 54000 + 3600 * 4.5;
            double timeMax = 3600 * 24;
            if (FullTime >= timeMax) FullTime -= timeMax;
            return FullTime;
        }
        protected Color ColorByArea(int area)
        {
            switch (area)
            {
                case 0:return Color.Orange;
                case 1:return Color.DeepSkyBlue;
                case 2:return Color.HotPink;
                case 3:return Color.LightSeaGreen;
                default:return Color.White;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.White;
            if (Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[0]))
            {
                NPC head = Main.npc[(int)projectile.ai[0]];
                if (head.ai[1] == LumiteDestroyerSegment.ChronoDash)
                {
                    if (head.ai[3] < 5)
                    {
                        color = Color.Lerp(ColorByArea(lastArea), ColorByArea(selectedArea), (float)areaTimer / 45);
                    }
                    else
                    {
                        color = Color.Lerp(ColorByArea(lastArea), Color.White, (float)areaTimer / 45);
                    }
                }
            }
            return color * projectile.Opacity;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int num156;
            int y3;
            Rectangle rectangle;
            Vector2 origin2;
            var color = projectile.GetAlpha(lightColor);
            //maybe ForceField?
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            //draw minute hand
            Texture2D minuteTexture = mod.GetTexture("Projectiles/LumiteDestroyer/MinuteHand");
            rectangle = new Rectangle(0, 0, minuteTexture.Width, minuteTexture.Height);
            origin2 = new Vector2(minuteTexture.Width / 2, 223f);
            Main.spriteBatch.Draw(minuteTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color, projectile.localAI[1] - MathHelper.PiOver2, origin2, projectile.scale * 1.1f, SpriteEffects.None, 0f);

            //draw hour hand
            Texture2D hourTexture = mod.GetTexture("Projectiles/LumiteDestroyer/HourHand");
            rectangle = new Rectangle(0, 0, hourTexture.Width, hourTexture.Height);
            origin2 = new Vector2(hourTexture.Width / 2, 22f);
            Main.spriteBatch.Draw(hourTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color, projectile.localAI[0] - MathHelper.PiOver2, origin2, projectile.scale * 1.1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                new Microsoft.Xna.Framework.Rectangle?(rectangle), color, projectile.rotation, origin2, projectile.scale * 1.25f, SpriteEffects.None, 0f);
            /*Texture2D bkgTexture = mod.GetTexture("Projectiles/GlowRing");
            num156 = bkgTexture.Height;
            y3 = num156;
            rectangle = new Rectangle(0, y3, bkgTexture.Width, num156);
            origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(bkgTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color, projectile.rotation, origin2, projectile.scale * 2f, SpriteEffects.None, 0f);*/
            Texture2D aimTexture = SunksBossChallenges.Instance.GetTexture("Projectiles/AimLine");
            for(int i = 0; i < 750; i++)//draw the circle
            {
                float rotation = i * MathHelper.TwoPi / 750;
                spriteBatch.Draw(aimTexture, projectile.Center + rotation.ToRotationVector2() * ClockR - Main.screenPosition,
                    new Rectangle(0, 0, 1, 1), color, rotation, Vector2.Zero, 3, SpriteEffects.None, 0);
            }
            
            /*Vector2 unit = Vector2.UnitY;
            for (int i = 0; i < 12; i++)//draw the face 
            {
                float length = i % 3 == 0 ? 250 : 120;
                for (int k = 0; k <= length; k+=4)
                {
                    Vector2 drawPos = projectile.Center - unit * ClockR + unit * k - Main.screenPosition;
                    Color alphaCenter = color * 0.8f;
                    spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
                }
                unit = unit.RotatedBy(-MathHelper.Pi / 6);
            }
            for(int i = 0; i < 2; i++)
            {
                unit = projectile.localAI[i].ToRotationVector2();
                float length = i == 0 ? 350 : 600;
                for (int k = 0; k <= length; k++)
                {
                    Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                    Color alphaCenter = color * 0.8f;
                    spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
                }
            }*/
            if (Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[0]))
            {
                NPC head = Main.npc[(int)projectile.ai[0]];
                if (head.ai[1] == LumiteDestroyerSegment.ChronoDash)
                {
                    /*if (head.ai[2] >= 225 && head.ai[2] <= 270)
                    {
                        float timer = head.ai[2] - 225;
                        Color alpha = Color.BlueViolet * projectile.Opacity;
                        if (timer <= 10)
                        {
                            alpha *= timer / 10f;
                        }
                        else if (timer >= 30 && timer <= 45)
                        {
                            alpha *= (45 - timer) / 15f;
                        }
                        projectile.DrawAim(spriteBatch, projectile.Center + projectile.localAI[0].ToRotationVector2() * 1000, alpha);
                    }*/
                    //head.localAI[0] = projectile.whoAmI;
                }
            }
            

            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(),
                    0, 0f, Main.myPlayer);
                Vector2 unit = -Vector2.UnitY;
                for (int i = 0; i < 12; i++)
                {
                    Projectile ray = Projectile.NewProjectileDirect(projectile.Center + unit, unit, ModContent.ProjectileType<DestroyerDeathRay>(),
                        projectile.damage * 2, 0f, projectile.owner, 135, projectile.ai[0]);
                    ray.localAI[1] = 1f;
                    ray.netUpdate = true;
                    unit = unit.RotatedBy(-MathHelper.Pi / 6);
                }
            }
            base.Kill(timeLeft);
        }
    }
}
