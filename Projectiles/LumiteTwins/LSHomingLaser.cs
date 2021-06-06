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

namespace SunksBossChallenges.Projectiles.LumiteTwins
{
    public class LSHomingLaser:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.DeathLaser;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death Laser");
            ProjectileID.Sets.TrailingMode[projectile.type] = ProjectileID.Sets.TrailingMode[ProjectileID.DeathLaser];
            ProjectileID.Sets.TrailCacheLength[projectile.type] = ProjectileID.Sets.TrailCacheLength[ProjectileID.DeathLaser];
        }
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.DeathLaser);
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
        }
        public override void AI()
        {
            projectile.alpha = 0;
            if (projectile.timeLeft >= 60)
            {
                int playerId = (int)projectile.ai[0];
                if (playerId != 255 && playerId >= 0)
                {
                    var vecToPlayer = Main.player[playerId].Center - projectile.Center;
                    var targetVector = Vector2.Normalize(vecToPlayer) * projectile.velocity.Length();
                    float turnAccle = projectile.ai[1];
                    if ((targetVector.X * projectile.velocity.X > 0f) || (targetVector.Y * projectile.velocity.Y > 0f)) //turn
                    {
                        projectile.velocity.X += Math.Sign(targetVector.X - projectile.velocity.X) * turnAccle;
                        projectile.velocity.Y += Math.Sign(targetVector.Y - projectile.velocity.Y) * turnAccle;

                        if (targetVector.X * projectile.velocity.X < 0)
                        {
                            projectile.velocity.Y += Math.Sign(projectile.velocity.Y) * turnAccle * 2f;
                        }

                        if (targetVector.Y * projectile.velocity.Y < 0)
                        {
                            projectile.velocity.X += Math.Sign(projectile.velocity.X) * turnAccle * 2f;
                        }
                    }
                    else if (Math.Abs(targetVector.X) > Math.Abs(targetVector.Y))
                    {
                        projectile.velocity.X += Math.Sign(targetVector.X - projectile.velocity.X) * turnAccle * 1.1f;
                        projectile.velocity.Y += Math.Sign(projectile.velocity.Y) * turnAccle;
                    }
                    else
                    {
                        projectile.velocity.Y += Math.Sign(targetVector.Y - projectile.velocity.Y) * turnAccle * 1.1f;
                        projectile.velocity.X += Math.Sign(projectile.velocity.X) * turnAccle;
                    }
                    projectile.velocity = Vector2.Normalize(projectile.velocity) * targetVector.Length();
                }
            }
            projectile.rotation = projectile.velocity.ToRotation()-1.57f;
        }
    }
}
