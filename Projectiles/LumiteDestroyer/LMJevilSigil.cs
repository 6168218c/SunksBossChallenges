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
    public class LMJevilSigil : ModProjectile
    {
        public static int MoonDistance => 500;
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
            if (projectile.localAI[1] == 1)
            {
                projectile.localAI[0]++;
                projectile.SlowDown(0.8f);
                if (projectile.localAI[0] >= 20)
                {
                    projectile.Kill();
                }
                return;
            }
            if (!Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[0]))
            {
                projectile.localAI[1] = 1;
                projectile.localAI[0] = 0;
                return;
            }
            NPC head = Main.npc[(int)projectile.ai[0]];
            Player player = Main.player[head.target];
            //Player player = Main.player[0];//test
            if (projectile.localAI[0] == 0&&Main.netMode!=NetmodeID.MultiplayerClient)
            {
                projectile.direction = Main.rand.NextBool() ? -1 : 1;
                projectile.netUpdate = true;
                var baseVector = projectile.rotation.ToRotationVector2() * MoonDistance;
                for(int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(projectile.Center + baseVector.RotatedBy(MathHelper.PiOver2 * i),
                        Vector2.Zero, ModContent.ProjectileType<LMJevilMoon>(), projectile.damage, 0f, Main.myPlayer,
                        projectile.whoAmI, i);
                }
            }
            projectile.localAI[0]++;
            if (projectile.localAI[0] < 240)
            {
                projectile.Center = player.Center;
            }
            projectile.rotation = MathHelper.WrapAngle(projectile.rotation + 0.0075f * projectile.direction);
            if (projectile.localAI[0] >= 280)
            {
                projectile.localAI[0] = 0;
                projectile.localAI[1] = 1;
            }
        }
    }
    public class LMJevilMoon : LMPlanetMoon
    {
        int headIndex = 0;
        Vector2 center;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void AI()
        {
            projectile.Loomup(3);

            if (Util.CheckProjAlive<LMJevilSigil>((int)projectile.ai[0],true))
            {
                Projectile parent = Main.projectile[(int)projectile.ai[0]];
                if (projectile.localAI[0] == 0)
                {
                    projectile.rotation = (parent.Center - projectile.Center).ToRotation();
                    headIndex = (int)parent.ai[0];
                }
                if (projectile.localAI[0] <= 240)
                {
                    projectile.rotation = MathHelper.WrapAngle(projectile.rotation - 0.05f * parent.direction);
                    projectile.Center = parent.Center + parent.rotation.ToRotationVector2()
                        .RotatedBy(MathHelper.PiOver2 * projectile.ai[1]) * LMJevilSigil.MoonDistance
                        + projectile.rotation.ToRotationVector2() * LMJevilSigil.MoonDistance * 0.375f;
                    projectile.velocity = Vector2.Zero;
                }

                if (projectile.localAI[0] % 80 == 0 && projectile.localAI[0] > 90 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    /*if (Util.CheckNPCAlive<LumiteDestroyerHead>(headIndex) && parent.localAI[0] >= 300)
                    {
                        NPC head = Main.npc[headIndex];
                        for (int i = 0; i < 9; i++)
                        {
                            Projectile.NewProjectile(projectile.Center, Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 9 * i) * 6,
                                ModContent.ProjectileType<LMRetractStar>(), projectile.damage * 2 / 3, 0f, Main.myPlayer, head.ai[3]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            Projectile.NewProjectile(projectile.Center, Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 9 * i) * 10.5f,
                                ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(), projectile.damage * 2 / 3, 0f, Main.myPlayer);
                        }
                    }*/
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 6 * i) * 10.5f,
                            ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(), projectile.damage * 2 / 3, 0f, Main.myPlayer, 3);
                    }
                }
            }
            else
            {
                if (projectile.localAI[1] == 0)
                {
                    projectile.localAI[1] = 1;
                    projectile.localAI[0] = 0;
                    center = Main.projectile[(int)projectile.ai[0]].Center;
                }
                projectile.HoverMovement(center, 24f, 0.75f);
                if (projectile.DistanceSQ(center) <= 30 * 30)
                    projectile.Kill();
            }
            projectile.localAI[0]++;
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
        public override void Kill(int timeLeft)
        {
        }
        public override bool CanDamage()
        {
            return false;
        }
    }
}
