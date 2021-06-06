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
            if (intensity < 2.5f)
            {
                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].active && projectile.DistanceSQ(Main.player[i].Center) <= 600 * 600)
                    {
                        var distanceSQ = projectile.DistanceSQ(Main.player[i].Center);
                        if (distanceSQ == 0) continue;
                        if (distanceSQ <= 1000 * 1000 && distanceSQ > 900)
                        {
                            var accle = projectile.DirectionFrom(Main.player[i].Center)
                                * Math.Min(9000 * intensity / distanceSQ, 1f);//need further testing

                            Main.player[i].velocity += accle;
                        }
                    }
                }
                intensity += 0.01f;
                projectile.scale = intensity;
            }
            else
            {
                //let us assume that there is always a LumiteDestroyer Alive
                if (Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[0]))
                {
                    NPC head = Main.npc[(int)projectile.ai[0]];
                    if (projectile.ai[1] == 45)
                    {
                        Projectile ray = Projectile.NewProjectileDirect(projectile.Center, -Vector2.UnitY, ModContent.ProjectileType<DestroyerDeathRay>(),
                        projectile.damage * 2, 0f, projectile.owner, 135, projectile.ai[0]);
                        ray.localAI[1] = 1f;
                        ray.netUpdate = true;
                    }
                    if (projectile.ai[1] >= 210)
                    {
                        projectile.scale -= 0.08f;
                        if (projectile.scale < 0.05f)
                        {
                            projectile.Kill();
                        }
                    }
                }
                else
                {
                    projectile.scale -= 0.08f;
                    if (projectile.scale < 0.05f)
                    {
                        projectile.Kill();
                    }
                }

                projectile.ai[1]++;
            }
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
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale*1.1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
