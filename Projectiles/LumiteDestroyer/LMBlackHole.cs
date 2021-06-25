using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SunksBossChallenges.NPCs.LumiteDestroyer;
using Terraria.Graphics.Effects;

namespace SunksBossChallenges.Projectiles.LumiteDestroyer
{
    public class LMBlackHole:ModProjectile
    {
        public override string Texture => "SunksBossChallenges/Projectiles/GlowRing";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Black Hole");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 80;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.hostile = true;
        }
        public override void AI()
        {
            projectile.alpha -= 25;
            if (projectile.alpha < 0) projectile.alpha = 0;
            float intensity = projectile.scale;
            if (intensity < 6f && projectile.localAI[0] == 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    /*if (!Filters.Scene["BlackHoleDistort"].IsActive())
                        Filters.Scene.Activate("BlackHoleDistort", projectile.Center)
                            .GetShader()
                            .UseTargetPosition(projectile.Center);
                    Filters.Scene["BlackHoleDistort"].GetShader().UseIntensity(intensity / 10);*/
                }
                Vector2 offset = Main.rand.NextVector2Circular(1000, 1000);
                Dust dust = Main.dust[Dust.NewDust(projectile.Center + offset, 0, 0, DustID.Clentaminator_Purple, 0, 0, 100, Color.White)];
                dust.velocity = -offset * 1000 / offset.LengthSquared();
                if (Main.rand.Next(3) == 0)
                    dust.velocity += Vector2.Normalize(offset) * 5f;
                dust.noGravity = true;
                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].active && projectile.DistanceSQ(Main.player[i].Center) <= 600 * 600)
                    {
                        var distanceSQ = projectile.DistanceSQ(Main.player[i].Center);
                        if (distanceSQ == 0) continue;
                        if (distanceSQ <= 900 * 900 && distanceSQ > 900)
                        {
                            var accle = projectile.DirectionFrom(Main.player[i].Center)
                                * Math.Min(9000 * intensity / distanceSQ, 0.9f);//need further testing

                            Main.player[i].velocity += accle;
                        }
                        else if (distanceSQ <= 1800 * 1800)
                        {
                            var accle = projectile.DirectionFrom(Main.player[i].Center)
                                * Math.Min(16000 * intensity / distanceSQ, 0.9f);//need further testing

                            Main.player[i].velocity += accle;
                        }
                    }
                }
                intensity += 0.045f;
                projectile.scale = intensity;
                if (intensity >= 6f) projectile.localAI[0]++;
            }
            else
            {
                //let us assume that there is always a LumiteDestroyer Alive
                if (Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[0]))
                {
                    NPC head = Main.npc[(int)projectile.ai[0]];
                    if (projectile.ai[1] >= 45 && projectile.ai[1] <= 600 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (projectile.ai[1] == 45)
                            projectile.localAI[1] = 0;

                        projectile.localAI[1] += 0.001f;
                        projectile.rotation = MathHelper.WrapAngle(projectile.rotation + projectile.localAI[1]);

                        if (projectile.ai[1] % 8 == 0)
                        {
                            Vector2 unit = projectile.rotation.ToRotationVector2();
                            for (int i = 0; i < 5; i++)
                            {
                                Projectile star = Projectile.NewProjectileDirect(projectile.Center + unit*LumiteDestroyerArguments.R, -unit*6, ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(),
                                    projectile.damage/3, 0f, projectile.owner);
                                star.timeLeft = (int)(LumiteDestroyerArguments.R / 6);
                                star.scale = 0.5f;
                                star.netUpdate = true;
                                unit = unit.RotatedBy(-MathHelper.TwoPi / 5);
                            }
                        }
                    }
                    else if(projectile.ai[1]==640 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 unit = Main.rand.NextVector2Unit();
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile ray = Projectile.NewProjectileDirect(projectile.Center + unit, unit, ModContent.ProjectileType<DestroyerDeathRay>(),
                                (int)(projectile.damage * 0.8), 0f, projectile.owner, 480, projectile.ai[0]);
                            ray.localAI[1] = 1f;
                            ray.netUpdate = true;
                            unit = unit.RotatedBy(-MathHelper.TwoPi / 5);
                        }
                    }
                    else if (projectile.ai[1] >= 1200)
                    {
                        /*if (!Filters.Scene["BlackHoleDistort"].IsActive())
                            Filters.Scene.Activate("BlackHoleDistort", projectile.Center)
                                .GetShader()
                                .UseTargetPosition(projectile.Center);
                        Filters.Scene["BlackHoleDistort"].GetShader().UseIntensity(intensity / 10);*/
                        projectile.scale -= 0.08f;
                        if (projectile.scale < 0.05f)
                        {
                            projectile.Kill();
                        }
                    }
                }
                else
                {
                    /*if (!Filters.Scene["BlackHoleDistort"].IsActive())
                        Filters.Scene.Activate("BlackHoleDistort", projectile.Center)
                            .GetShader()
                            .UseTargetPosition(projectile.Center);
                    Filters.Scene["BlackHoleDistort"].GetShader().UseIntensity(intensity / 10);*/
                    projectile.scale -= 0.08f;
                    if (projectile.scale < 0.05f)
                    {
                        projectile.Kill();
                    }
                }

                projectile.ai[1]++;
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Filters.Scene["BlackHoleDistort"].Deactivate();
            }
            base.Kill(timeLeft);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Color.Black;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.Orange), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(color), projectile.rotation, origin2, projectile.scale*1.1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
