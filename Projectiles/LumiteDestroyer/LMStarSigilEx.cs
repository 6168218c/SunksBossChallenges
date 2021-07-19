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
    public class LMStarSigilEx : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Sigil Ex");
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

            projectile.timeLeft = LMSigilSubStar.LaunchTime + 200;
        }
        public override void AI()
        {
            projectile.rotation -= 0.015f * Math.Sign(projectile.ai[0]);
            if (projectile.localAI[0] <= LMSigilSubStar.LaunchTime - 75)
                projectile.Center = Main.player[(int)projectile.ai[1]].Center;

            if (projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                projectile.rotation = MathHelper.WrapAngle(projectile.ai[0]);
                Vector2 baseVector = projectile.rotation.ToRotationVector2();

                for (int i = 0; i < 5; i++)
                {
                    var center = projectile.Center + baseVector * LumiteDestroyerArguments.R * 0.8f;
                    Projectile.NewProjectile(center, Vector2.Zero, ModContent.ProjectileType<LMSigilSubStar>(), projectile.damage,
                        0f, projectile.owner, baseVector.ToRotation(), projectile.whoAmI);
                    baseVector = baseVector.RotatedBy(MathHelper.TwoPi / 5);
                }
            }

            projectile.localAI[0]++;
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
    public class LMSigilSubStar:ModProjectile
    {
        float Speed => 24f;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public static int LaunchTime => 280;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Sigil Substar");
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
            Projectile parent = Main.projectile[(int)projectile.ai[1]];
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[1] = parent.ai[1];
            }
            if (projectile.localAI[0] < 150)
            {
                projectile.Center = parent.Center + parent.rotation.ToRotationVector2().RotatedBy(projectile.ai[0]) * LumiteDestroyerArguments.R * 0.8f;
            }
            projectile.rotation -= 0.025f;
            projectile.localAI[0]++;

            if (projectile.localAI[0] % 20 == 0 && projectile.localAI[0] / 20 <= 7 && Main.netMode != NetmodeID.MultiplayerClient) 
            {
                float lineCenterDist = 60;
                float lineHalfLen = lineCenterDist * (float)Math.Tan(Math.PI / 2.5);
                Vector2 baseUnit = projectile.rotation.ToRotationVector2();
                Vector2 baseVector = baseUnit * lineCenterDist;

                for (int i = 0; i < 5; i++)
                {
                    var start = projectile.Center + baseVector + baseUnit.RotatedBy(Math.PI / 2) * lineHalfLen;
                    var end = projectile.Center + baseVector + baseUnit.RotatedBy(-Math.PI / 2) * lineHalfLen;
                    //we will have five stars per line
                    int count = (int)projectile.localAI[0] / 20 - 1;
                    //var tarPos = Vector2.Lerp(start, end, 1.0f / 6 * count);//the end will not be covered
                    var tarPos = projectile.Center + (projectile.Center - parent.Center);
                    Projectile.NewProjectile(tarPos, projectile.DirectionTo(parent.Center) * 20f, ModContent.ProjectileType<LMStarSigilExUnit>(),
                        projectile.damage, 0f, projectile.owner, count + i * 6, projectile.whoAmI);
                    baseVector = baseVector.RotatedBy(Math.PI / 2.5);
                    baseUnit = baseUnit.RotatedBy(Math.PI / 2.5);
                }
            }
            if (projectile.localAI[0] == LaunchTime - 50)
            {
                projectile.velocity = (parent.Center - projectile.Center).SafeNormalize(Vector2.Zero) * Speed;
            }
            if (projectile.localAI[0] >= LaunchTime)
            {
                projectile.Kill();
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
    public class LMStarSigilExUnit : LMProjUnit
    {
        protected bool hasBeenLaunched = false;
        protected override int CacheLen
        {
            get
            {
                Projectile parent = Main.projectile[(int)projectile.ai[1]];
                if (parent.active && parent.type == ModContent.ProjectileType<LMSigilSubStar>())
                    return 5;
                else
                    return ProjectileID.Sets.TrailCacheLength[projectile.type];
            }
        }
        public override bool NeedSyncLocalAI => true;
        public override void AI()
        {
            LoomUp(25);
            Projectile parent = Main.projectile[(int)projectile.ai[1]];
            if (DeathAnimationTimer > 0)
            {
                projectile.velocity = Vector2.Zero;
                DeathAnimationTimer--;
                if (DeathAnimationTimer == 0) projectile.Kill();
                return;
            }
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[1] = parent.localAI[1];
            }
            if (parent.active && parent.type == ModContent.ProjectileType<LMSigilSubStar>())
            {
                int counter = (int)projectile.ai[0] / 6;
                int pos = (int)projectile.ai[0] % 6;
                float lineCenterDist = 60;
                float lineHalfLen = lineCenterDist * (float)Math.Tan(Math.PI / 2.5);
                Vector2 baseUnit = parent.rotation.ToRotationVector2().RotatedBy(counter * Math.PI / 2.5);
                Vector2 baseVector = baseUnit * lineCenterDist;

                var start = parent.Center + baseVector + baseUnit.RotatedBy(Math.PI / 2) * lineHalfLen;
                var end = parent.Center + baseVector + baseUnit.RotatedBy(-Math.PI / 2) * lineHalfLen;
                var tarPos = Vector2.Lerp(start, end, 1.0f / 6 * pos);//the end will not be covered

                if (projectile.localAI[0] < 30)
                {
                    projectile.SlowDown(0.95f);
                }
                else if (projectile.localAI[0] < 60)
                {
                    projectile.FastMovement(tarPos);
                }
                else
                    projectile.Center = tarPos;
            }
            else
            {
                if (!hasBeenLaunched)
                {
                    hasBeenLaunched = true;
                    projectile.localAI[0] = 0;
                }
                Player player = Main.player[(int)projectile.localAI[1]];
                projectile.WormMovementEx(player.Center, 36f, distLimit: 200, angleLimit: MathHelper.Pi / 6);
            }
            projectile.localAI[0]++;
            if ((projectile.localAI[0] >= 180 || projectile.ai[0] % 6 <= 3) && hasBeenLaunched)
            {
                //projectile.Kill();
                DeathAnimationTimer = 15;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenLaunched);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenLaunched = reader.ReadBoolean();
            base.ReceiveExtraAI(reader);
        }
    }
}
