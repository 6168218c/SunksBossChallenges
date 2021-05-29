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
    public class LMClockFace:ModProjectile
    {
        public int ClockR => 720;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clock Face");
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
            NPC head = Main.npc[(int)projectile.ai[0]];
            if (!head.active || head.type != ModContent.NPCType<LumiteDestroyerHead>())//this will heavily affect the head's behavior so we used it
            {
                projectile.Kill();
                return;
            }
            Player player = Main.player[head.target];
            projectile.alpha -= 25;
            if (projectile.alpha < 0) projectile.alpha = 0;
            //Player player = Main.player[(int)projectile.ai[0]];
            projectile.Center = (player.Center + projectile.Center * 64) / 65;
            if (player.DistanceSQ(projectile.Center) > ClockR * ClockR)
            {
                player.Center = projectile.Center + player.DirectionFrom(projectile.Center) * ClockR;
            }

            //update timer rotations
            double fullTime = GetTime();
            float hourHand = (float)(fullTime / (3600 * 24) * MathHelper.TwoPi * 2);
            float minHand = (float)(fullTime % 3600 / 3600 * MathHelper.TwoPi);
            projectile.localAI[0] = MathHelper.WrapAngle(hourHand - MathHelper.PiOver2);
            projectile.localAI[1] = MathHelper.WrapAngle(minHand - MathHelper.PiOver2);
        }
        protected double GetTime()
        {
            double FullTime = Main.dayTime ? Main.time + 3600 * 4.5 : Main.time + 54000 + 3600 * 4.5;
            double timeMax = 3600 * 24;
            if (FullTime >= timeMax) FullTime -= timeMax;
            return FullTime;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D aimTexture = SunksBossChallenges.Instance.GetTexture("Projectiles/AimLine");
            var color = Color.BlueViolet * projectile.Opacity;
            for(int i = 0; i < 750; i++)//draw the circle
            {
                float rotation = i * MathHelper.TwoPi / 750;
                spriteBatch.Draw(Main.magicPixel, projectile.Center + rotation.ToRotationVector2() * ClockR - Main.screenPosition,
                    new Rectangle(0, 0, 1, 1), color, rotation, Vector2.Zero, 3, SpriteEffects.None, 0);
            }
            Vector2 unit = Vector2.UnitY;
            for (int i = 0; i < 12; i++)//draw the face 
            {
                float length = i % 3 == 0 ? 250 : 120;
                for (int k = 0; k <= length; k+=4)
                {
                    Vector2 drawPos = projectile.Center - unit * ClockR + unit * k - Main.screenPosition;
                    Color alphaCenter = color * 0.8f;
                    spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
                }
                unit = unit.RotatedBy(-MathHelper.Pi / 6);
            }
            for(int i = 0; i < 2; i++)
            {
                unit = projectile.localAI[i].ToRotationVector2();
                float length = i == 0 ? 350 : 600;
                for (int k = 0; k <= length; k++)
                {
                    Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                    Color alphaCenter = color * 0.8f;
                    spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 unit = -Vector2.UnitY;
                for (int i = 0; i < 12; i++)
                {
                    Projectile ray = Projectile.NewProjectileDirect(projectile.Center + unit, unit, ModContent.ProjectileType<DestroyerDeathRay>(),
                        projectile.damage * 2, 0f, projectile.owner, 135, projectile.ai[0]);
                    ray.localAI[1] = 1f;
                    ray.netUpdate = true;
                    unit = unit.RotatedBy(-MathHelper.Pi / 6);
                }
            }
            base.Kill(timeLeft);
        }
    }
}
