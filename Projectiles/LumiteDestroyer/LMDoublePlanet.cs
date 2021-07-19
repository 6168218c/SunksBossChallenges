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
    public class LMDoublePlanet:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Double Planet");
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

            projectile.timeLeft = 600;
        }
        public override void AI()
        {
            if (projectile.ai[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LMPlanetMoon>(),
                    projectile.damage, 0f, projectile.owner, -1, projectile.whoAmI);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LMPlanetMoon>(),
                    projectile.damage, 0f, projectile.owner, 1, projectile.whoAmI);
            }
            projectile.ai[1]++;
            if (projectile.ai[1] >= 150)
            {
                projectile.SlowDown(0.98f);
                if (projectile.velocity == Vector2.Zero || projectile.velocity.Compare(projectile.ai[0]) < 0)
                {
                    projectile.Kill();
                }
            }
            projectile.rotation = projectile.velocity.ToRotation();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float timer = projectile.ai[1];
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
                projectile.DrawAim(spriteBatch, projectile.Center + projectile.rotation.ToRotationVector2() * 3600, alpha);
            }
            return base.PreDraw(spriteBatch, lightColor);
        }
    }
    public class LMPlanetMoon : ModProjectile
    {
        public override string Texture => "SunksBossChallenges/Projectiles/LumiteDestroyer/LMChaosMoon";
        public float Omega => 0.1f;
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Moon");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.width = 41;
            projectile.height = 41;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
			projectile.alpha = 255;

            //projectile.timeLeft = 450;
            projectile.extraUpdates = 0;
            cooldownSlot = 1;
            projectile.penetrate = -1;

            projectile.scale = 1.5f;
        }
        public override void AI()
        {
			projectile.Loomup(3);
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
            Projectile parent = Main.projectile[(int)projectile.ai[1]];
            if (parent.active && parent.type == ModContent.ProjectileType<LMDoublePlanet>())
            {
                projectile.localAI[0]++;
                projectile.Center = parent.Center
                    + parent.velocity.RotatedBy(MathHelper.PiOver2 * projectile.ai[0]) * (float)Math.Sin(projectile.localAI[0] * Omega) * 10;
            }
            else
            {
                projectile.Kill();
            }
        }
        public override bool CanDamage()
        {
            return projectile.alpha == 0;
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
                        ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(), projectile.damage / 2, 0f, Main.myPlayer);
            }
        }
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color glow = new Color(Main.DiscoR + 210, Main.DiscoG + 210, Main.DiscoB + 210);
            Color glow2 = new Color(Main.DiscoR + 50, Main.DiscoG + 50, Main.DiscoB + 50) * projectile.Opacity;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }

    public class LMDoublePlanetAurora : ModProjectile
    {
        public static float maxSpeedBuff => 1.35f;
        public static float ramAccBuff => 3f;
        public static float radiusSpeedBuff => 1.2f;
        public override string Texture => "SunksBossChallenges/Projectiles/GlowRing";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Double Planet Ex");
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

            //projectile.timeLeft = 600;
        }
        public override void AI()
        {
			if (!Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[1]))
			{
				projectile.localAI[1] = 1;
			}
            if (projectile.localAI[1] == 1)
            {
                projectile.scale -= 0.8f;
                if (projectile.scale < 0.01f) projectile.Kill();
				return;
            }
            Player player = Main.player[(int)projectile.ai[0]];
            projectile.Loomup();
            if (projectile.localAI[0] == 0)
            {
                Vector2 closest = new Vector2(999999, 999999);
                foreach(NPC nPC in Main.npc)
                {
                    if (nPC.type == ModContent.NPCType<LumiteDestroyerBody>())
                    {
                        if ((nPC.Center - projectile.Center).LengthSquared() < (closest - projectile.Center).LengthSquared())
                        {
                            closest = nPC.Center;
                        }
                    }
                }
                for(int i = 0; i < 3; i++)
                {
                    var velo = (player.Center - projectile.Center).SafeNormalize(-Vector2.UnitY).RotatedBy(-MathHelper.Pi / 9) * 8;
                    Projectile.NewProjectile(closest,
                    velo, ModContent.ProjectileType<LMPlanetMoonEx>(),
                    projectile.damage, 0f, projectile.owner, i, projectile.whoAmI);
                }
                projectile.direction = Main.rand.NextBool() ? -1 : 1;
            }

            projectile.localAI[0]++;
            if (projectile.localAI[0] >= 240 && projectile.localAI[0]<=270)
            {
                projectile.scale += 0.5f;
                if (projectile.scale > 16f) projectile.scale = 16f;
            }

            projectile.rotation += 0.02f * projectile.direction;
            projectile.Center = (player.Center + projectile.Center * 19) / 20;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Yellow * projectile.Opacity * 0.2f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class LMPlanetMoonEx : LMPlanetMoon
    {
        Vector2 center;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Moon");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void AI()
        {
            projectile.localAI[0]++;
			projectile.Loomup(3);
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
            if (Util.CheckProjAlive<LMDoublePlanetAurora>((int)projectile.ai[1]) && Main.projectile[(int)projectile.ai[1]].localAI[1] == 0)
            {
                Projectile parent = Main.projectile[(int)projectile.ai[1]];
                if (projectile.localAI[0] <= 30)
                {
                    projectile.SlowDown(0.98f);
                }
                else if (projectile.localAI[0] <= 80)
                {
                    float R = LumiteDestroyerArguments.R - 600;
                    projectile.HoverMovement(parent.Center - parent.rotation.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 3 * projectile.ai[0]) * R,
                        24f, 0.75f);
                }
                else if (projectile.localAI[0] <= 90)
                {
                    float R = LumiteDestroyerArguments.R - 600;
                    projectile.FastMovement(parent.Center - parent.rotation.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 3 * projectile.ai[0]) * R);
                }
                else if (projectile.localAI[0] <= 270)
                {
                    projectile.velocity = Vector2.Zero;
                    if (projectile.localAI[0] % 60 == 0 && Main.netMode!=NetmodeID.MultiplayerClient)
                    {
                        var baseUnit = (parent.Center - projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(Math.PI / 12);
                        for (int i = 0; i < 3; i++)
                            Projectile.NewProjectile(projectile.Center, baseUnit.RotatedBy(Math.PI / 18 * i),
                                ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(), projectile.damage * 2 / 3, 0f, projectile.owner, 1, 36f);
                    }

                    float R = LumiteDestroyerArguments.R - 600 + 600 * (float)Math.Sin(Math.PI / 2 * (projectile.localAI[0] - 90) / 180);
                    projectile.Center = parent.Center - parent.rotation.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 3 * projectile.ai[0]) * R;
                }
                else
                {
                    float R = LumiteDestroyerArguments.R;
                    if (projectile.localAI[0] % 120 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var baseUnit = (parent.Center - projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.NewProjectile(projectile.Center, baseUnit,
                            ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(), projectile.damage * 2 / 3, 0f, projectile.owner, 1, 36f);
                    }
                    projectile.Center = parent.Center - parent.rotation.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 3 * projectile.ai[0]) * R;
                }
            }
            else
            {
                if (projectile.localAI[1] == 0)
                {
                    projectile.localAI[1] = 1;
                    projectile.localAI[0] = 0;
                    center = Main.projectile[(int)projectile.ai[1]].Center;
                }
                projectile.HoverMovement(center, 24f, 0.75f);
                if (projectile.DistanceSQ(center) <= 450 * 450)
                    projectile.Kill();
            }
        }
		public override bool CanHitPlayer(Player target)
        {
            if (projectile.localAI[0] <= 120) 
            {
                return false;
            }
            return true;
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
                const int max = 6;
                const float rotationInterval = 2f * (float)Math.PI / max;
                Vector2 speed = new Vector2(0f, 2f).RotatedBy(projectile.rotation);
                for (int i = 0; i < max; i++)
                    Projectile.NewProjectile(projectile.Center, speed.RotatedBy(rotationInterval * i),
                        ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(), projectile.damage * 2 / 3, 0f, Main.myPlayer, 1, 36f);
            }
        }
    }
}
