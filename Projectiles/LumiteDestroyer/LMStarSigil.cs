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
    public class LMStarSigil:ModProjectile
    {
        protected int baseUnitLen => 875;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Sigil");
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
            Player player = Main.player[(int)projectile.ai[0]];
            if (projectile.localAI[0] < 120)
            {
                projectile.rotation += 0.015f;
                projectile.localAI[0]++;
                projectile.Center = player.Center;
            }

            /*if (projectile.localAI[0] == 200 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                float lineCenterDist = projectile.ai[0] * (float)Math.Sin(Math.PI / 2.5);
                float lineHalfLen = lineCenterDist * (float)Math.Tan(Math.PI / 2.5);
                Vector2 baseUnit = projectile.rotation.ToRotationVector2();
                Vector2 baseVector = baseUnit * lineCenterDist;

                for (int i = 0; i < projectile.ai[1]; i++)
                {
                    var start = projectile.Center + baseVector + baseUnit.RotatedBy(Math.PI / 2) * lineHalfLen;
                    var target = projectile.Center + baseVector;
                    Projectile.NewProjectile(start, Vector2.Zero, ModContent.ProjectileType<DecimatorOfPlanets.LaserBarrage>(),
                        projectile.damage, 0f, projectile.owner, target.X, target.Y);
                    baseVector = baseVector.RotatedBy(Math.PI / 2.5);
                    baseUnit = baseUnit.RotatedBy(Math.PI / 2.5);
                }
                projectile.Kill();
            }*/
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D aimTexture = mod.GetTexture("Projectiles/AimLine");
            float angleCenterDist = baseUnitLen;
            float lineHalfLen = angleCenterDist * (float)Math.Sin(Math.PI / 2.5);
            Vector2 baseUnit = projectile.rotation.ToRotationVector2();
            Vector2 baseVector = baseUnit * angleCenterDist * Math.Abs((float)Math.Cos(Math.PI / 2.5));

            for (int i = 0; i < 5; i++)
            {
                var start = projectile.Center + baseVector + baseUnit.RotatedBy(Math.PI / 2) * lineHalfLen;
                var end = projectile.Center + baseVector + baseUnit.RotatedBy(-Math.PI / 2) * lineHalfLen;
                var scale = new Vector2(1);
                Vector2 unit = end - start;
                float length = unit.Length();
                unit.Normalize();
                for (int k = 0; k <= length; k += 4)
                {
                    Vector2 drawPos = start + unit * k - Main.screenPosition;
                    Color alphaCenter = Color.Lerp(Color.CornflowerBlue, Color.BlueViolet, (float)Math.Sin(MathHelper.Pi / 15 * projectile.localAI[0]));
                    spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), scale, SpriteEffects.None, 0f);
                }
                baseVector = baseVector.RotatedBy(Math.PI / 2.5);
                baseUnit = baseUnit.RotatedBy(Math.PI / 2.5);
            }

            for(int i = 1; i <= projectile.ai[1]; i++)
            {
                Vector2 target = projectile.Center + projectile.rotation.ToRotationVector2()
                            .RotatedBy(Math.PI * 4 / 5 * i) * baseUnitLen;
                Texture2D texture2D13 = mod.GetTexture("Projectiles/GlowRing");
                Rectangle rectangle = new Rectangle(0, 0, texture2D13.Width, texture2D13.Height);
                Vector2 origin2 = rectangle.Size() / 2f;
                Main.spriteBatch.Draw(texture2D13, target - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                    projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
