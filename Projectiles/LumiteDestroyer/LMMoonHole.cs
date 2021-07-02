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
    public class LMGigaMoon:LMPlanetMoon
    {
        public override string Texture => "SunksBossChallenges/Projectiles/LumiteDestroyer/LMGigaMoon";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.scale = 0.5f;
        }
        public override void AI()
        {                
            projectile.Loomup(3);
            projectile.localAI[0]++;

            Projectile parent = Main.projectile[(int)projectile.ai[1]];

            if (projectile.localAI[0] <= 210)
            {
                projectile.Center = parent.Center;
                if (projectile.localAI[0] == 150 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile shockwave = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(),
                        0, 0f, Main.myPlayer, 1);
                    shockwave.timeLeft = 90;
                }
            }
            else if (projectile.localAI[0] <= 240)
            {
                projectile.velocity = Vector2.Zero;
                projectile.scale -= (0.5f / 32);
            }
            else
            {
                projectile.Kill();
            }
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item89, projectile.position);

            Vector2 size = new Vector2(40, 40);
            Vector2 spawnPos = projectile.Center;
            spawnPos.X -= size.X / 2;
            spawnPos.Y -= size.Y / 2;

            for (int num615 = 0; num615 < 45; num615++)
            {
                int num616 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num616].velocity *= 1.4f;
            }

            for (int num617 = 0; num617 < 60; num617++)
            {
                int num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[num618].noGravity = true;
                Main.dust[num618].velocity *= 7f;
                num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num618].velocity *= 3f;
            }

            for (int num619 = 0; num619 < 3; num619++)
            {
                float scaleFactor9 = 0.4f;
                if (num619 == 1) scaleFactor9 = 0.8f;
                int num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore97 = Main.gore[num620];
                gore97.velocity.X++;
                Gore gore98 = Main.gore[num620];
                gore98.velocity.Y++;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore99 = Main.gore[num620];
                gore99.velocity.X--;
                Gore gore100 = Main.gore[num620];
                gore100.velocity.Y++;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore101 = Main.gore[num620];
                gore101.velocity.X++;
                Gore gore102 = Main.gore[num620];
                gore102.velocity.Y--;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore103 = Main.gore[num620];
                gore103.velocity.X--;
                Gore gore104 = Main.gore[num620];
                gore104.velocity.Y--;
            }


            for (int k = 0; k < 40; k++) //make visual dust
            {
                Vector2 dustPos = spawnPos;
                dustPos.X += Main.rand.Next((int)size.X);
                dustPos.Y += Main.rand.Next((int)size.Y);

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(dustPos, 32, 32, DustID.Smoke, 0f, 0f, 100, default(Color), 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                float scaleFactor9 = 0.5f;
                for (int j = 0; j < 4; j++)
                {
                    int gore = Gore.NewGore(dustPos, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[gore].velocity *= scaleFactor9;
                    Main.gore[gore].velocity.X += 1f;
                    Main.gore[gore].velocity.Y += 1f;
                }
            }


            const int num226 = 80;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 40f;
                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Fire, 0f, 0f, 0, default(Color), 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
            base.Kill(timeLeft);


            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                const int max = 8;
                const float rotationInterval = 2f * (float)Math.PI / max;
                Vector2 speed = new Vector2(0f, 8f + 4f).RotatedBy(projectile.rotation);
                for (int i = 0; i < max; i++)
                    Projectile.NewProjectile(projectile.Center, speed.RotatedBy(rotationInterval * i),
                        ModContent.ProjectileType<LMRetractStar>(), projectile.damage / 2, 0f, Main.myPlayer,projectile.ai[1]);
            }
        }
    }
    public class LMMoonHole : LMBlackHole
    {
        public override void AI()
        {
            if (projectile.localAI[1] == 1)
            {
                projectile.scale -= 0.06f;
                if (projectile.scale <= 0.02f)
                {
                    projectile.scale = 0.02f;
                    projectile.Kill();
                }
                return;
            }
            projectile.Loomup();
            Player player = Main.player[(int)projectile.ai[0]];
            if (projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LMGigaMoon>(),
                    0, 0f, Main.myPlayer, 0, projectile.whoAmI);
            }

            projectile.localAI[0]++;

            if (projectile.localAI[0] <= 210)
            {
                projectile.FastMovement(player.Center - Vector2.UnitY * 375, 15);
            }
            else if (projectile.localAI[0] <= 240)
            {
                projectile.velocity = Vector2.Zero;
            }
            else if (projectile.localAI[0] <= 330)
            {
                projectile.scale += 3f / 90;
            }
            else if (projectile.localAI[0] <= 560)
            {
                projectile.ai[1] = 1;
            }
            else
            {
                projectile.localAI[1] = 1;
            }
        }
    }
    public class LMRetractStar : DecimatorOfPlanets.DarkStar
    {
        public override void AI()
        {
            if (projectile.localAI[0] == 0)
                projectile.localAI[0] = 1f;
            projectile.alpha += (int)(25.0 * projectile.localAI[0]);
            if (projectile.alpha > 200)
            {
                projectile.alpha = 200;
                projectile.localAI[0] = -1f;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
                projectile.localAI[0] = 1f;
            }

            projectile.rotation = projectile.rotation + (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;

            if (Main.rand.Next(30) == 0)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Enchanted_Pink, (float)(projectile.velocity.X * 0.5), (float)(projectile.velocity.Y * 0.5), 150, default, 1.2f);

            Lighting.AddLight(projectile.Center, 0.9f, 0.8f, 0.1f);

            if (Util.CheckProjAlive<LMMoonHole>((int)projectile.ai[0], true))
            {
                Projectile hole = Main.projectile[(int)projectile.ai[0]];
                if (hole.ai[1] == 1)
                {
                    if (projectile.ai[1] == 0)
                    {
                        projectile.SlowDown(0.95f);
                        if (projectile.velocity == Vector2.Zero)
                        {
                            projectile.velocity = (hole.Center - projectile.Center).SafeNormalize(Vector2.UnitY);
                            projectile.ai[1] = 1;
                        }
                    }
                    else
                    {
                        projectile.localAI[1]++;
                        if (projectile.velocity.Compare(48f) < 0) projectile.velocity *= 1.05f;
                        if (projectile.DistanceSQ(hole.Center) <= 150 * 150)
                        {
                            projectile.Kill();
                        }
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Util.CheckProjAlive<LMMoonHole>((int)projectile.ai[0], true))
            {
                /*float timer = projectile.localAI[1] - 1;
                if (timer <= 45)
                {
                    Color alpha = Color.BlueViolet;
                    if (timer <= 10)
                    {
                        alpha *= timer / 10f;
                    }
                    else if (timer >= 30 && timer <= 45)
                    {
                        alpha *= (45 - timer) / 15f;
                    }
                    projectile.DrawAim(spriteBatch, Main.projectile[(int)projectile.ai[0]].Center, alpha);
                }*/
            }  
            return base.PreDraw(spriteBatch, lightColor);
        }
    }
}
