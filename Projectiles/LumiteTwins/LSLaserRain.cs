using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SunksBossChallenges.NPCs.LumiteTwins;

namespace SunksBossChallenges.Projectiles.LumiteTwins
{
    public class LSLaserRain:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Rain");
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
            projectile.hostile = true;
        }
        public override void AI()
        {
            Vector2 velocity = projectile.ai[1].ToRotationVector2();
            if (projectile.ai[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)//death laser
            {
                if (projectile.localAI[0] % 10 == 0)
                {
                    Projectile.NewProjectile(projectile.Center, velocity * 18f, ProjectileID.DeathLaser, projectile.damage,
                        0f, projectile.owner);
                }
                if (projectile.localAI[0] >= 90) projectile.Kill();
            }
            else if(projectile.ai[0] == 1 && Main.netMode != NetmodeID.MultiplayerClient)//deathray
            {
                int index = -1;
                if ((index = NPC.FindFirstNPC(ModContent.NPCType<LumiteRetinazer>())) != -1 && projectile.localAI[0] == 0)
                {
                    Projectile proj1= Projectile.NewProjectileDirect(projectile.Center, velocity, ModContent.ProjectileType<RetinDeathRay>(),
                        projectile.damage, 0f, projectile.owner, 90, index);
                    Projectile proj2 = Projectile.NewProjectileDirect(projectile.Center, -velocity, ModContent.ProjectileType<RetinDeathRay>(),
                        projectile.damage, 0f, projectile.owner, 90, index);
                    proj1.localAI[1] = proj2.localAI[1] = 1;
                    proj1.netUpdate = proj2.netUpdate = true;
                }

                if (projectile.localAI[0] >= 90) projectile.Kill();
            }
            projectile.localAI[0]++;
        }
    }
}
