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
            projectile.width = projectile.height = 270;
            projectile.scale = 0.5f;
        }
        public override void AI()
        {                
            projectile.Loomup(4);
			if (projectile.alpha != 0)
			{
				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Vortex, 0f, 0f, 100, default, 2f);
					dust.noGravity = true;
					dust.noLight = true;
					dust.color = Color.Pink;
				}
			}
            projectile.localAI[0]++;

            Projectile parent = Main.projectile[(int)projectile.ai[1]];

            if (projectile.localAI[0] <= 90)
            {
                projectile.Center = parent.Center;
                if (projectile.localAI[0] == 30 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile shockwave = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(),
                        0, 0f, Main.myPlayer, 1);
                    shockwave.timeLeft = 90;
                }
            }
            else if (projectile.localAI[0] <= 120)
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


            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                const int max = 36;
                const float rotationInterval = 2f * (float)Math.PI / max;
                Vector2 speed = new Vector2(0f, 8f + 4f).RotatedBy(projectile.rotation);
                for (int i = 0; i < max; i++)
                    Projectile.NewProjectile(projectile.Center, speed.RotatedBy(rotationInterval * i),
                        ModContent.ProjectileType<LMRetractStar>(), projectile.damage * 2 / 3, 0f, Main.myPlayer, projectile.ai[1], 1);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            /*if (projectile.localAI[0] >= 200)
            {
                float timer = projectile.localAI[0] - 200;
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
                    const int max = 30;
                    const float rotationInterval = 2f * (float)Math.PI / max;
                    Vector2 speed = new Vector2(0f, 8f + 4f).RotatedBy(projectile.rotation);
                    for (int i = 0; i < max; i++)
                        projectile.DrawAim(spriteBatch, speed.RotatedBy(rotationInterval * i) * (i % 2 == 0 ? 1 : -1) * 200, alpha);
                }
            }*/
            return base.PreDraw(spriteBatch, lightColor);
        }
        public override bool CanHitPlayer(Player target)
        {
            return projectile.localAI[1] > 90;
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
            if (!Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[0]))
            {
                projectile.localAI[1] = 1;
                return;
            }
            NPC head = Main.npc[(int)projectile.ai[0]];
            Player player = Main.player[head.target];
            //Player player = Main.player[0];

            if (Util.CheckProjAlive<LMJevilSigil>((int)head.localAI[0]))
            {
                projectile.Center = Main.projectile[(int)head.localAI[0]].Center;
                return;
            }
            /*foreach(Projectile proj in Main.projectile)
            {
                if (Util.CheckProjAlive<LMJevilSigil>(proj.whoAmI))
                {
                    projectile.Center = proj.Center;
                    return;//wait until it dies
                }
            }*/

            if (projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LMGigaMoon>(),
                    projectile.damage, 0f, Main.myPlayer, 0, projectile.whoAmI);
            }
            projectile.localAI[0]++;

            if (projectile.localAI[0] <= 90)
            {
                if(projectile.localAI[0] >= 45)
                    projectile.FastMovement(player.Center - Vector2.UnitY * 375, 15);
            }
            else if (projectile.localAI[0] <= 120)
            {
				projectile.Loomup();
                projectile.velocity = Vector2.Zero;
            }
            else if (projectile.localAI[0] <= 210)
            {
                projectile.scale += 3f / 90;
            }
            else if (projectile.localAI[0] <= 390)
            {
                Vector2 offset = Main.rand.NextVector2Circular(1000, 1000);
                Dust dust = Main.dust[Dust.NewDust(projectile.Center + offset, 0, 0, DustID.Clentaminator_Purple, 0, 0, 100, Color.White)];
                dust.velocity = -offset * 1000 / offset.LengthSquared();
                if (Main.rand.Next(3) == 0)
                    dust.velocity += Vector2.Normalize(offset) * 5f;
                dust.noGravity = true;
                if (projectile.localAI[0] == 270 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile shockwave = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockwaveCenter>(),
                        0, 0f, Main.myPlayer, 1);
                    shockwave.timeLeft = 90;
                }
                projectile.ai[1]++;
            }
            else
            {
                projectile.localAI[1] = 1;
            }
        }
        public override bool CanHitPlayer(Player target)
        {
            return projectile.localAI[0] > 90;
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

            if (Util.CheckProjAlive<LMMoonHole>((int)projectile.ai[0]))
            {
                Projectile hole = Main.projectile[(int)projectile.ai[0]];
                if (hole.ai[1] != 0)
                {
                    //gravitational one
                    /*if (projectile.DistanceSQ(hole.Center) <= 150 * 150)
                    {
                        projectile.Kill();
                        return;
                    }
                    var distanceSQ = projectile.DistanceSQ(hole.Center);
                    float intensity = (hole.scale + hole.ai[1] / 15);
                    if (distanceSQ <= 900 * 900 && distanceSQ > 150*150)
                    {
                        var accle = projectile.DirectionTo(hole.Center)
                            * Math.Min(9000 * intensity / distanceSQ, 1.35f);//need further testing

                        projectile.velocity += accle;
                    }
                    else if (distanceSQ <= 4800 * 4800)
                    {
                        var accle = projectile.DirectionTo(hole.Center)
                            * Math.Min(16000 * intensity * 2 / distanceSQ, 1.8f);//need further testing

                        projectile.velocity += accle;
                    }
					projectile.velocity = projectile.velocity.SafeNormalize(Vector2.Zero) * Math.Min(projectile.velocity.Length(), 30f);*/
                    if (projectile.ai[1] == 0 || projectile.ai[1] == 1)
                    {
                        projectile.SlowDown(0.8f);
                        if (projectile.velocity == Vector2.Zero)
                        {
                            projectile.velocity = projectile.DirectionTo(hole.Center);
                            projectile.localAI[1] = 0;
                            projectile.ai[1] = 2;
                        }
                    }
                    else
                    {
                        if (projectile.velocity.Compare(48f) < 0)
                            projectile.velocity *= 1.08f;
                        projectile.localAI[1]++;
                        if (projectile.DistanceSQ(hole.Center) <= 100 * 100)
                        {
                            projectile.Kill();
                        }
                    }
                }
                else
                {
                    if (projectile.ai[1] == 1 && projectile.velocity.Compare(90f) < 0)
                    {
                        projectile.localAI[1]++;
                        if (projectile.localAI[1] >= 25)
                            projectile.velocity *= 1.08f;
                    }
                }
            }
			else
			{
				projectile.Kill();
			}
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D glow = mod.GetTexture("Projectiles/DecimatorOfPlanets/DarkStar_Glow");
            int rect1 = glow.Height / Main.projFrames[projectile.type];
            int rect2 = rect1 * projectile.frame;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(180, 100, 180, 150), Color.Transparent, 0.8f);
            Vector2 drawCenter = projectile.Center - (projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);

            Main.spriteBatch.Draw(glow, drawCenter - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),//create small, non transparent trail texture
                   projectile.GetAlpha(lightColor), projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, projectile.scale / 2, SpriteEffects.None, 0f);

            for (int i = 0; i < 3; i++) //create multiple transparent trail textures ahead of the projectile
            {
                Vector2 drawCenter2 = drawCenter + (projectile.velocity.SafeNormalize(Vector2.UnitX) * 12).RotatedBy(MathHelper.Pi / 5 - (i * MathHelper.Pi / 5)); //use a normalized version of the projectile's velocity to offset it at different angles
                drawCenter2 -= (projectile.velocity.SafeNormalize(Vector2.UnitX) * 12); //then move it backwards
                Main.spriteBatch.Draw(glow, drawCenter2 - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, projectile.scale, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++) //reused betsy fireball scaling trail thing
            {
                Color color27 = glowcolor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i] - (projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);
                Main.spriteBatch.Draw(glow, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale, SpriteEffects.None, 0f);
            }

            if (projectile.ai[1] == 1|| projectile.ai[1] == 2)
            {
                int timer = (int)projectile.localAI[1];
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
                    projectile.DrawAim(spriteBatch, projectile.Center + projectile.velocity.SafeNormalize(Vector2.Zero) * 3600, alpha);
                }
            }
            return false;
        }
    }
}
