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
            if (head.ai[1] == LumiteDestroyerSegment.ChronoDash)
            {
                if(head.ai[2] <= 180)
                    projectile.Center = (player.Center + projectile.Center * 64) / 65;
                if (head.ai[2] == 270 && head.ai[3] < 5 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 unit;
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
                            Vector2 projPos = projectile.Center + unit * ClockR;
                            Projectile.NewProjectile(projPos, Vector2.Zero, ModContent.ProjectileType<DecimatorOfPlanets.LaserBarrage>(),
                                projectile.damage / 3, 0f, projectile.owner, projectile.Center.X, projectile.Center.Y);
                            unit = unit.RotatedBy(-MathHelper.Pi / 6);
                        }
                    }
                    else
                    {
                        unit = -Vector2.UnitY;
                        for (int i = 0; i < 12; i += 2)
                        {
                            Vector2 projPos = projectile.Center + unit * ClockR;
                            Projectile.NewProjectile(projPos, Vector2.Zero, ModContent.ProjectileType<DecimatorOfPlanets.LaserBarrage>(),
                                projectile.damage / 3, 0f, projectile.owner, projectile.Center.X, projectile.Center.Y);
                            unit = unit.RotatedBy(-MathHelper.Pi / 6);
                        }
                    }
                    unit = lastLaserRotation.ToRotationVector2();
                    Projectile ray = Projectile.NewProjectileDirect(projectile.Center, unit, ModContent.ProjectileType<DestroyerDeathRay>(),
                        projectile.damage * 2, 0f, projectile.owner, 135, projectile.ai[0]);
                    ray.localAI[1] = 2f;
                    ray.netUpdate = true;
                    ray = Projectile.NewProjectileDirect(projectile.Center, -unit, ModContent.ProjectileType<DestroyerDeathRay>(),
                        projectile.damage * 2, 0f, projectile.owner, 135, projectile.ai[0]);
                    ray.localAI[1] = 2f;
                    ray.netUpdate = true;
                    lastLaserRotation = projectile.localAI[0];
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
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //maybe ForceField?

            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            var color = Color.Yellow * projectile.Opacity;
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
            
            //draw minute hand
            Texture2D minuteTexture = mod.GetTexture("Projectiles/LumiteDestroyer/MinuteHand");
            rectangle = new Rectangle(0, 0, minuteTexture.Width, minuteTexture.Height);
            origin2 = new Vector2(minuteTexture.Width / 2, 223f);
            Main.spriteBatch.Draw(minuteTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                projectile.GetAlpha(color), projectile.localAI[1]-MathHelper.PiOver2, origin2, projectile.scale*1.1f, SpriteEffects.None, 0f);

            //draw hour hand
            Texture2D hourTexture = mod.GetTexture("Projectiles/LumiteDestroyer/HourHand");
            rectangle = new Rectangle(0, 0, hourTexture.Width, hourTexture.Height);
            origin2 = new Vector2(hourTexture.Width / 2, 22f);
            Main.spriteBatch.Draw(hourTexture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                projectile.GetAlpha(color), projectile.localAI[0] - MathHelper.PiOver2, origin2, projectile.scale*1.1f, SpriteEffects.None, 0f);
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
                    if (head.ai[2] >= 225 && head.ai[2] <= 270)
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
                        projectile.DrawAim(spriteBatch, projectile.Center + lastLaserRotation.ToRotationVector2() * 1000, alpha);
                    }
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
