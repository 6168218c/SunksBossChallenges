using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SunksBossChallenges.NPCs.LumiteDestroyer;
using System.Collections.Generic;
using SunksBossChallenges.Effects;

namespace SunksBossChallenges.Projectiles.LumiteDestroyer
{
    public class LMDashTrail:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dash Trail");
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
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
            if (projectile.localAI[1] == 1)
            {
                projectile.velocity = Vector2.Zero;
                projectile.alpha += 25;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
                return;
            }
            projectile.Loomup();
            if (Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[1]))
            {
                NPC head = Main.npc[(int)projectile.ai[1]];
                if (head.ai[1] == LumiteDestroyerSegment.PlanetAurora
                && Util.CheckProjAlive<LMDoublePlanetAurora>((int)head.ai[3]))
                {
                    Projectile aurora = Main.projectile[(int)head.ai[3]];

                    if (aurora.localAI[1] != 1)
                        projectile.Center = head.Center
                            + head.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * projectile.ai[0]) * head.width / 2;
                    else
                    {
                        projectile.localAI[1] = 1;
                    }
                }
                else if (head.ai[1] == LumiteDestroyerSegment.ChronoDash)
                {
                    projectile.Center = head.Center
                            + head.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * projectile.ai[0]) * head.width / 2;
                }
            }
            else
            {
                projectile.localAI[1] = 1;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            List<VertexStripInfo> vertecies = new List<VertexStripInfo>();
            if (Util.CheckNPCAlive<LumiteDestroyerHead>((int)projectile.ai[1]))
            {
                NPC head = Main.npc[(int)projectile.ai[1]];
                if (head.ai[1] == LumiteDestroyerSegment.PlanetAurora
                && Util.CheckProjAlive<LMDoublePlanetAurora>((int)head.ai[3]))
                {
                    Projectile aurora = Main.projectile[(int)head.ai[3]];
                    if ((head.Center - aurora.Center).Compare(LumiteDestroyerArguments.R) <= 0 || projectile.localAI[1] == 1)
                    {
                        for (int i = 1; i < projectile.oldPos.Length; i++)
                        {
                            if (projectile.oldPos[i] == Vector2.Zero) break;

                            var dir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                            dir = dir.SafeNormalize(Vector2.Zero).RotatedBy(Math.PI / 2);

                            var factor = i / (float)projectile.oldPos.Length;
                            var alpha = MathHelper.SmoothStep(1f, 0.5f, factor);

                            float width = MathHelper.SmoothStep(0, 20, Math.Min(1, 2.5f * factor));

                            if (i > 15)
                            {
                                width *= (float)(25 - i) / 10;
                            }
                            if (projectile.localAI[1] == 1)
                            {
                                alpha *= (float)(255 - projectile.alpha) / 25;
                                width *= (float)(255 - projectile.alpha) / 25;
                            }

                            Vector2 d = projectile.oldPos[i - 1] - projectile.oldPos[i];
                            vertecies.Add(new VertexStripInfo((projectile.oldPos[i] - d * i * 0.4f) + dir * width, new Vector3((float)Math.Sqrt(factor), 1, alpha)));
                            vertecies.Add(new VertexStripInfo((projectile.oldPos[i] - d * i * 0.35f) + dir * -width, new Vector3((float)Math.Sqrt(factor), 0, alpha)));
                        }

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                        var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                        var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

                        SunksBossChallenges.Trail.Parameters["uTransform"].SetValue(model * projection);
                        SunksBossChallenges.Trail.Parameters["uTime"].SetValue(projectile.timeLeft * 0.04f);

                        Main.graphics.GraphicsDevice.Textures[0] = mod.GetTexture("Images/Trail");
                        Main.graphics.GraphicsDevice.Textures[1] = mod.GetTexture("Images/YellowGrad/img_color");
                        Main.graphics.GraphicsDevice.Textures[2] = mod.GetTexture("Images/YellowGrad/img_color");

                        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
                        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

                        SunksBossChallenges.Trail.CurrentTechnique.Passes[0].Apply();

                        if (vertecies.Count >= 3)
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertecies.ToArray(), 0, vertecies.Count - 2);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    }
                }
                else if (head.ai[1] == LumiteDestroyerSegment.ChronoDash)
                {
                    for (int i = 1; i < projectile.oldPos.Length; i++)
                    {
                        if (projectile.oldPos[i] == Vector2.Zero) break;

                        var dir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                        dir = dir.SafeNormalize(Vector2.Zero).RotatedBy(Math.PI / 2);

                        var factor = i / (float)projectile.oldPos.Length;
                        var alpha = MathHelper.SmoothStep(1f, 0.5f, factor);

                        float width = MathHelper.SmoothStep(0, 20, Math.Min(1, 2.5f * factor));

                        if (i > 15)
                        {
                            width *= (float)(25 - i) / 10;
                        }
                        if (projectile.localAI[1] == 1)
                        {
                            alpha *= (float)(255 - projectile.alpha) / 25;
                            width *= (float)(255 - projectile.alpha) / 25;
                        }

                        Vector2 d = projectile.oldPos[i - 1] - projectile.oldPos[i];
                        vertecies.Add(new VertexStripInfo((projectile.oldPos[i] - d * i * 0.4f) + dir * width, new Vector3((float)Math.Sqrt(factor), 1, alpha)));
                        vertecies.Add(new VertexStripInfo((projectile.oldPos[i] - d * i * 0.35f) + dir * -width, new Vector3((float)Math.Sqrt(factor), 0, alpha)));
                    }

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

                    SunksBossChallenges.Trail.Parameters["uTransform"].SetValue(model * projection);
                    SunksBossChallenges.Trail.Parameters["uTime"].SetValue(projectile.timeLeft * 0.04f);

                    Main.graphics.GraphicsDevice.Textures[0] = mod.GetTexture("Images/Trail");
                    Main.graphics.GraphicsDevice.Textures[1] = mod.GetTexture("Images/YellowGrad/img_color");
                    Main.graphics.GraphicsDevice.Textures[2] = mod.GetTexture("Images/YellowGrad/img_color");

                    Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
                    Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

                    SunksBossChallenges.Trail.CurrentTechnique.Passes[0].Apply();

                    if (vertecies.Count >= 3)
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertecies.ToArray(), 0, vertecies.Count - 2);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }

            base.PostDraw(spriteBatch, lightColor);
        }
    }
}
