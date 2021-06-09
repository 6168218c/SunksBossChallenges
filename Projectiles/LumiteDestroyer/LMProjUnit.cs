using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SunksBossChallenges.NPCs.DecimatorOfPlanets;


namespace SunksBossChallenges.Projectiles.LumiteDestroyer
{
    public abstract class LMProjUnit:ModProjectile
    {
        protected Texture2D TrailTex;
        public override string Texture => "SunksBossChallenges/Projectiles/LumiteDestroyer/LMProjUnit";
        public abstract override void AI();
        public virtual bool NeedSyncLocalAI => false;
        public int DeathAnimationTimer { get; set; }
        protected abstract int CacheLen { get; }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death Star");
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
            projectile.alpha = 255;

            cooldownSlot = 1;
            projectile.scale = 0.75f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
        }
        protected void LoomUp(int rate=42)
        {
            projectile.alpha -= rate;
            if (projectile.alpha < 0) projectile.alpha = 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            if (TrailTex == null)
                TrailTex = mod.GetTexture("Projectiles/DeathLaserTrail");
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            //Color glow = new Color(Main.DiscoR + 210, Main.DiscoG + 210, Main.DiscoB + 210);
            //Color glow2 = new Color(Main.DiscoR + 50, Main.DiscoG + 50, Main.DiscoB + 50);
            Color glow2 = Color.Yellow;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            /*for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }*/
            for (int i = 0; i < CacheLen - 1; i++)
            {
                int cacheLen = CacheLen;
                //int i2 = (int)(i + projectile.ai[0]) % (ProjectileID.Sets.TrailCacheLength[projectile.type] - 1);
                Rectangle rect = new Rectangle(0, 5 * i, 84, 5 * (i + 1));  //10 84
                float len = (projectile.oldPos[i + 1] - projectile.oldPos[i]).Length();
                float rotation = (projectile.oldPos[i] - projectile.oldPos[i + 1]).ToRotation() + MathHelper.PiOver2;
                if (projectile.oldPos[i + 1] == Vector2.Zero || projectile.oldPos[i] == Vector2.Zero) continue;
                Vector2 scale = new Vector2(0.2f * (cacheLen - i) / cacheLen, len / 10);
                float ops = (50f - i) / 50f;
                Vector2 MidCenter = (projectile.oldPos[i] + projectile.oldPos[i + 1]) / 2 + projectile.Size / 2;
                spriteBatch.Draw(TrailTex, MidCenter - Main.screenPosition, rect, glow2 * ops, rotation, rect.Size() / 2, scale, SpriteEffects.None, 0);
            }
            if (DeathAnimationTimer == 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (NeedSyncLocalAI)
            {
                writer.Write(projectile.localAI[0]);
                writer.Write(projectile.localAI[1]);
            }
            writer.Write(DeathAnimationTimer);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if(NeedSyncLocalAI)
            {
                projectile.localAI[0] = reader.ReadSingle();
                projectile.localAI[1] = reader.ReadSingle();
            }
            DeathAnimationTimer = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
    }
}
