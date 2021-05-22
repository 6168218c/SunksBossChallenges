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
    public class LMLaserMatrix:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Matrix");
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

            projectile.timeLeft = 120;
        }

        public override void AI()
        {
            // ai[0]:half distance of each border
            // ai[1]:direction

            Vector2 unitVect = Vector2.UnitX;
            unitVect = unitVect.RotatedBy(Math.PI / 2 * (int)projectile.ai[1]);

            unitVect *= projectile.ai[0];
            if (projectile.localAI[0] % 20 == 5 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                var startPos = projectile.Center;
                startPos += unitVect.RotatedBy(-MathHelper.PiOver2) * 8;
                //move to the new center
                startPos += unitVect.RotatedBy(MathHelper.PiOver2);
                for (int i = 0; i < 9; i++)
                {
                    startPos += unitVect.RotatedBy(MathHelper.PiOver2) * 2;

                    var target = startPos + unitVect;
                    Projectile.NewProjectile(startPos + unitVect.RotatedBy(-MathHelper.PiOver2), Vector2.Zero,
                        ModContent.ProjectileType<DecimatorOfPlanets.LaserBarrage>(), projectile.damage, 0f, Main.myPlayer, target.X, target.Y);
                    Projectile.NewProjectile(startPos + unitVect.RotatedBy(MathHelper.PiOver2), Vector2.Zero,
                        ModContent.ProjectileType<DecimatorOfPlanets.LaserBarrage>(), projectile.damage, 0f, Main.myPlayer, target.X, target.Y);
                }
            }

            projectile.localAI[0]++;

            if (projectile.localAI[0] % 20 == 0)
            {
                projectile.Center += unitVect * 2;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            base.ReceiveExtraAI(reader);
        }
    }
}
