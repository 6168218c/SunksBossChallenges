using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Graphics.Shaders;

namespace SunksBossChallenges.Projectiles
{
    public class DeathLaserEx:ModProjectile
    {
        protected float maxAccle => 0.2f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death Laser Ex");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 16;
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.light = 0.8f;
			projectile.hostile = true;
            projectile.penetrate = -1;

            cooldownSlot = 1;
			projectile.scale = 0.8f;
            projectile.timeLeft = 240;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (projectile.timeLeft <= 15)
            {
                projectile.velocity = Vector2.Zero;
                projectile.alpha += 25;
                if (projectile.alpha > 255) projectile.alpha = 255;
                return;
            }
            projectile.alpha -= 42;
            if (projectile.alpha < 0) projectile.alpha = 0;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = new Vector2(0, -1);
            }
            float length = projectile.velocity.Length();
            if (projectile.ai[1] != -1)
            {
                projectile.WormMovement(Main.player[(int)projectile.ai[1]].Center, projectile.ai[0], maxAccle);
                if (projectile.timeLeft <= 120) projectile.Kill();
            }
            else
            {
                if (length < projectile.ai[0])
                {
                    if (length + maxAccle > projectile.ai[0])
                    {
                        projectile.velocity = Vector2.Normalize(projectile.velocity) * projectile.ai[0];
                    }
                    else
                    {
                        projectile.velocity = Vector2.Normalize(projectile.velocity) * (length + maxAccle * 2.5f);
                    }
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() - 1.57f;
            for (int num25 = projectile.oldPos.Length - 1; num25 > 0; num25--)
            {
                projectile.oldPos[num25] = projectile.oldPos[num25 - 1];
                projectile.oldRot[num25] = projectile.oldRot[num25 - 1];
                projectile.oldSpriteDirection[num25] = projectile.oldSpriteDirection[num25 - 1];
            }

            projectile.oldPos[0] = projectile.position;
            projectile.oldRot[0] = projectile.rotation;
            projectile.oldSpriteDirection[0] = projectile.spriteDirection;
            float amount = 0.65f;
            int num26 = 1;
            for (int num27 = 0; num27 < num26; num27++)
            {
                for (int num28 = projectile.oldPos.Length - 1; num28 > 0; num28--)
                {
                    if (!(projectile.oldPos[num28] == Vector2.Zero))
                    {
                        if (Vector2.Distance(projectile.oldPos[num28],projectile.oldPos[num28 - 1]) > 2f)
                            projectile.oldPos[num28] = Vector2.Lerp(projectile.oldPos[num28], projectile.oldPos[num28 - 1], amount);

                        projectile.oldRot[num28] = (projectile.oldPos[num28 - 1] - projectile.oldPos[num28]).SafeNormalize(Vector2.Zero).ToRotation() - 1.57f;
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Texture2D TrailTex = mod.GetTexture("Projectiles/Trail");
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            //Color glow = new Color(Main.DiscoR + 210, Main.DiscoG + 210, Main.DiscoB + 210);
            //Color glow2 = new Color(Main.DiscoR + 50, Main.DiscoG + 50, Main.DiscoB + 50);
			Color glow2 = Color.Yellow;
            int y12 = 0;
            Microsoft.Xna.Framework.Rectangle rectangle4 = new Microsoft.Xna.Framework.Rectangle(0, y12, TrailTex.Width, TrailTex.Height);
            Vector2 origin5 = rectangle4.Size() / 2f;
            Vector2 zero = Vector2.Zero;
            float num145 = 0f;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            int num149 = 18;
            int num147 = 0;
            int num148 = -2;
            float value15 = 1.3f;
            float num150 = 15f;
            float num151 = 0f;
            Rectangle value16 = rectangle4;
            for (int num152 = num149; (num148 > 0 && num152 < num147) || (num148 < 0 && num152 > num147); num152 += num148)
            {
                if (num152 >= projectile.oldPos.Length)
                    continue;
                Color color32 = glow2;
                float num157 = num147 - num152;
                if (num148 < 0)
                    num157 = num149 - num152;

                color32 *= num157 / ((float)ProjectileID.Sets.TrailCacheLength[projectile.type] * 1.5f);
                Vector2 value18 = projectile.oldPos[num152];
                float num158 = projectile.rotation;
                SpriteEffects effects2 = spriteEffects;
                if (ProjectileID.Sets.TrailingMode[projectile.type] == 2 || ProjectileID.Sets.TrailingMode[projectile.type] == 3 || ProjectileID.Sets.TrailingMode[projectile.type] == 4)
                {
                    num158 = projectile.oldRot[num152];
                    effects2 = ((projectile.oldSpriteDirection[num152] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                }
                if (value18 == Vector2.Zero)
                    continue;
                Vector2 position3 = value18 + Vector2.Zero + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                spriteBatch.Draw(TrailTex, position3, value16, color32, num158 + num145 + projectile.rotation * num151 * (float)(num152 - 1) * (float)(-spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt()), origin5, MathHelper.Lerp(projectile.scale, value15, (float)num152 / num150), effects2, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0f, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, 0f, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}
