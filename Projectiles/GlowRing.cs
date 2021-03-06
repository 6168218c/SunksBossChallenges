using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Graphics.Effects;
using Terraria.Localization;

namespace SunksBossChallenges.Projectiles
{
    //Original Author:FargoWilta(https://github.com/FargoWilta)
    public class GlowRing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow Ring");
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.alpha = 0;
            projectile.timeLeft = 300;
        }

        public Color color = Color.White;

        public override void AI()
        {
            int ai0 = (int)projectile.ai[0];
            if (ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].HasPlayerTarget)
            {
                projectile.Center = Main.npc[ai0].Center;
            }

            float scale = 12f;
            int maxTime = 30;

            switch ((int)projectile.ai[1])
            {
                case -12: //nature shroomite blue
                    color = new Color(0, 0, 255);
                    break;

                case -11: //nature chlorophyte green
                    color = new Color(0, 255, 0);
                    break;

                case -10: //nature frost cyan
                    color = new Color(0, 255, 255);
                    break;

                case -9: //nature rain yellow
                    color = new Color(255, 255, 0);
                    break;

                case -8: //nature molten orange
                    color = new Color(255, 127, 40);
                    break;

                case -7: //nature crimson red
                    color = new Color(255, 0, 0);
                    break;

                case -6: //will, spirit champ yellow
                    color = new Color(255, 255, 0);
                    break;

                case -5: //shadow champ purple
                    color = new Color(200, 0, 255);
                    break;

                case -4: //life champ yellow
                    color = new Color(255, 255, 0);
                    scale = 16f;
                    maxTime = 60;
                    break;

                case -3: //earth champ orange
                    color = new Color(255, 100, 0);
                    scale = 16f;
                    maxTime = 60;
                    break;

                case -2: //ml teal cyan
                    color = new Color(51, 255, 191);
                    scale = 16f;
                    break;

                case -1: //purple shadowbeam
                    color = new Color(200, 0, 200);
                    maxTime = 60;
                    break;

                default:
                    break;
            }

            if (++projectile.localAI[0] > maxTime)
            {
                projectile.Kill();
                return;
            }

            projectile.scale = scale * (float)Math.Sin(Math.PI / 2 * projectile.localAI[0] / maxTime);
            projectile.alpha = (int)(255f * projectile.localAI[0] / maxTime * 0.75f);

            if (projectile.alpha > 255)
                projectile.alpha = 255;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * projectile.Opacity;
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
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }

    public class GlowRingLegacy : ModProjectile
    {
        public Color color = Color.White;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Legacy Glow Ring");
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

            projectile.timeLeft = 20;
        }
        public override void AI()
        {
            float scale = 12f;
            switch ((int)projectile.ai[1])
            {
                case -12: //nature shroomite blue
                    color = new Color(0, 0, 255);
                    break;

                case -11: //nature chlorophyte green
                    color = new Color(0, 255, 0);
                    break;

                case -10: //nature frost cyan
                    color = new Color(0, 255, 255);
                    break;

                case -9: //nature rain yellow
                    color = new Color(255, 255, 0);
                    break;

                case -8: //nature molten orange
                    color = new Color(255, 127, 40);
                    break;

                case -7: //nature crimson red
                    color = new Color(255, 0, 0);
                    break;

                case -6: //will, spirit champ yellow
                    color = new Color(255, 255, 0);
                    break;

                case -5: //shadow champ purple
                    color = new Color(200, 0, 255);
                    break;

                case -4: //life champ yellow
                    color = new Color(255, 255, 0);
                    break;

                case -3: //earth champ orange
                    color = new Color(255, 100, 0);
                    scale = 16f;
                    break;

                case -2: //ml teal cyan
                    color = new Color(51, 255, 191);
                    scale = 16f;
                    break;

                case -1: //purple shadowbeam
                    color = new Color(200, 0, 200);
                    break;

                default:
                    break;
            }

            int veloFactor = 1;
            if (projectile.ai[0] == 1) veloFactor = -1;
            for(int i = 0; i < 22; i++)
            {
                var velo = -Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 22 * i) * veloFactor;
                int num = Dust.NewDust(projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 22 * i) * 120 * scale,
                    projectile.width, projectile.height, DustID.Vortex, velo.X, velo.Y, 0, projectile.GetAlpha(color), scale);
                Main.dust[num].noGravity = true;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return color * projectile.Opacity;
        }
    }
}
