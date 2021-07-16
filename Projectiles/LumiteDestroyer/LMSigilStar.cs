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
    public class LMSigilStarWave : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sigil Star Wave");
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

            projectile.timeLeft = 600;
        }
        public override void AI()
        {
            projectile.ai[1]++;
            if (projectile.ai[1] >= 175)
            {
                projectile.SlowDown(0.8f);
                if (projectile.velocity == Vector2.Zero || projectile.velocity.Compare(projectile.ai[0]) < 0)
                {
                    projectile.Kill();
                }
            }
            projectile.rotation = projectile.velocity.ToRotation();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float timer = projectile.ai[1];
            if (timer <= 45)
            {
                Color alpha = Color.Yellow;
                if (timer <= 10)
                {
                    alpha *= timer / 10f;
                }
                else if (timer >= 30 && timer <= 45)
                {
                    alpha *= (45 - timer) / 15f;
                }
                projectile.DrawAim(spriteBatch, projectile.Center + projectile.velocity.SafeNormalize(Vector2.Zero) * 3600, alpha);
            }
            return base.PreDraw(spriteBatch, lightColor);
        }
    }
    public class LMSigilStar:ModProjectile
    {
        float Speed => 24f;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public static int LaunchTime => 280;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Sigil Star");
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
            projectile.rotation -= 0.02f;
            projectile.Loomup();
            if (projectile.ai[0] == 0)
            {
                Player player = Main.player[(int)projectile.ai[1]];
                projectile.localAI[0]++;

                if (projectile.localAI[0] >= LaunchTime / 3 && projectile.localAI[0] <= 200)
                {
                    projectile.WormMovementEx(player.Center, Speed * 0.8f, angleLimit: MathHelper.Pi / 6);
                }
                else if (projectile.localAI[0] == LaunchTime - 45)
                {
                    projectile.localAI[1] = 1;//set this to inactive
                    projectile.velocity = Vector2.Zero;
                }
                else if (projectile.localAI[0] >= LaunchTime)
                {
                    projectile.Kill();
                }
            }
            else if (projectile.ai[0] == 1)
            {
                Player player = Main.player[(int)projectile.ai[1]];
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
                        var tarPos = projectile.Center + Main.rand.NextVector2Circular(150f, 100f);
                        Projectile.NewProjectile(tarPos, Vector2.Zero, ModContent.ProjectileType<LMStarSigilExUnit>(),
                            projectile.damage, 0f, projectile.owner, count + i * 6, projectile.whoAmI);
                        baseVector = baseVector.RotatedBy(Math.PI / 2.5);
                        baseUnit = baseUnit.RotatedBy(Math.PI / 2.5);
                    }
                }
                if (projectile.localAI[0] == LaunchTime - 50)
                {
                    projectile.velocity = (player.Center - projectile.Center).SafeNormalize(Vector2.Zero) * Speed;
                }
                if (projectile.localAI[0] >= LaunchTime)
                {
                    projectile.Kill();
                }
            }
            else if (projectile.ai[0] == 2)
            {
                float Omega = MathHelper.TwoPi / 90;
                int direction = Math.Sign(projectile.ai[1]);
                Projectile parent = Main.projectile[Math.Abs((int)projectile.ai[1])];//Wave
                projectile.localAI[0]++;
                if (projectile.localAI[0] > 90 && projectile.localAI[0] < LaunchTime - 45)
                {
                    projectile.localAI[0] = 0;
                }

                if (parent.active && parent.type == ModContent.ProjectileType<LMSigilStarWave>())
                {
                    projectile.Center = parent.Center
                        + parent.velocity.RotatedBy(MathHelper.PiOver2 * direction) * (float)Math.Sin(projectile.localAI[0] * Omega) * 50;
                }
                else
                {
                    projectile.localAI[1] = 1;//set this to inactive
                    projectile.velocity = Vector2.Zero;
                    projectile.localAI[0] = LaunchTime - 45;
                }
                if (projectile.localAI[0] >= LaunchTime)
                {
                    projectile.Kill();
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] == 0)
            {
                if (projectile.localAI[0] >= LaunchTime - 45 && projectile.localAI[0] < LaunchTime)
                {
                    int timer = (int)projectile.localAI[0] - LaunchTime + 45;
                    Color alpha = Color.Yellow;
                    if (timer <= 10)
                    {
                        alpha *= timer / 10f;
                    }
                    else if (timer >= 30 && timer <= 45)
                    {
                        alpha *= (45 - timer) / 15f;
                    }
                    projectile.DrawAim(spriteBatch, 
                        projectile.Center + (Main.player[(int)projectile.ai[1]].Center-projectile.Center).SafeNormalize(Vector2.Zero) * 3600, alpha);
                }
            }
            return base.PreDraw(spriteBatch, lightColor);
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
    public class LMSigilStarUnit : LMProjUnit
    {
        protected bool hasBeenLaunched = false;
        protected int AIState = 0;
        protected float launchRotation;
        protected override int CacheLen
        {
            get
            {
                Projectile parent = Main.projectile[(int)projectile.ai[1]];
                if (parent.active && parent.type == ModContent.ProjectileType<LMSigilStar>())
                    return 5;
                else if (hasBeenLaunched && AIState == 0 && projectile.ai[0] % 6 != 0)
                    return 8;
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
                projectile.localAI[1] = parent.ai[1];
                AIState = (int)parent.ai[0];
            }
            if (AIState == 0)
            {
                if (projectile.localAI[0] <= 30)
                {
                    projectile.localAI[0]++;
                    projectile.SlowDown(0.98f);
                }
                else if (parent.active && parent.type == ModContent.ProjectileType<LMSigilStar>() && parent.localAI[1] == 0)
                {
                    projectile.localAI[0]++;
                    projectile.HoverMovement(parent.Center, 24f, 0.75f);
                    if (projectile.localAI[0] >= 80 && parent.type == ModContent.ProjectileType<LMSigilStar>())
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

                        if (projectile.localAI[0] <=90)
                        {
                            projectile.FastMovement(tarPos);
                        }
                        else
                        {
                            projectile.velocity = Vector2.Zero;
                            projectile.Center = tarPos;
                        }
                    }
                }
                else
                {
                    if (!hasBeenLaunched)
                    {
                        hasBeenLaunched = true;
                        projectile.localAI[0] = 0;
                        launchRotation = (Main.player[Math.Abs((int)projectile.localAI[1])].Center - parent.Center).ToRotation();
                        if (parent.active && parent.type == ModContent.ProjectileType<LMSigilStar>())
                            projectile.velocity = -(parent.Center - projectile.Center) / 60f;
                    }
                    if (Util.CheckProjAlive<LMSigilStar>((int)projectile.ai[1]))
                    {
                        launchRotation = (Main.player[Math.Abs((int)projectile.localAI[1])].Center - parent.Center).ToRotation();
                    }
                    projectile.localAI[0]++;
                    var targetVector = launchRotation.ToRotationVector2();
                    if (projectile.velocity.Compare(25f) < 0)
                    {
                        projectile.velocity *= 1.035f;
                    }
                    float angle = MathHelper.WrapAngle(targetVector.ToRotation() - projectile.velocity.ToRotation());
                    if (Math.Abs(angle) >= 0.03f)
                    {
                        projectile.velocity = projectile.velocity.RotatedBy(0.03f * Math.Sign(angle));
                    }
                    else
                    {
                        projectile.velocity = projectile.velocity.RotatedBy(angle);
                    }
                    if (projectile.localAI[0] >= 180)
                    {
                        DeathAnimationTimer = 15;
                        return;
                    }
                }
            }
            else if (AIState == 1)
            {
                if (parent.active && parent.type == ModContent.ProjectileType<LMSigilStar>())
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
            else if (AIState == 2)
            {
                if (projectile.localAI[0] <= 30)
                {
                    projectile.localAI[0]++;
                    projectile.SlowDown(0.98f);
                }
                else if (parent.active && parent.type == ModContent.ProjectileType<LMSigilStar>() && parent.localAI[1] == 0)
                {
                    projectile.localAI[0]++;
                    projectile.HoverMovement(parent.Center, 24f, 0.75f);
                    if (projectile.localAI[0] >= 80 && parent.type == ModContent.ProjectileType<LMSigilStar>())
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

                        if (projectile.localAI[0] <= 90)
                        {
                            projectile.FastMovement(tarPos);
                        }
                        else
                        {
                            projectile.velocity = Vector2.Zero;
                            projectile.Center = tarPos;
                        }
                    }
                }
                else
                {
                    if (!hasBeenLaunched)
                    {
                        hasBeenLaunched = true;
                        projectile.localAI[0] = 0;
                        if (parent.active && parent.type == ModContent.ProjectileType<LMSigilStar>())
                            projectile.velocity = -(parent.Center - projectile.Center) / 60f;
                    }
                    projectile.localAI[0]++;
                    if (projectile.velocity.Compare(60) < 0)
                    {
                        projectile.velocity *= 1.05f;
                    }
                    if (projectile.localAI[0] >= 240)
                    {
                        DeathAnimationTimer = 15;
                        return;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (AIState == 2)
            {
                if (hasBeenLaunched && projectile.velocity != Vector2.Zero && projectile.ai[0] % 6 == 0&& projectile.localAI[0] <= 45)
                {
                    Color alpha = Color.Yellow * projectile.Opacity;
                    if (projectile.localAI[0] <= 10)
                    {
                        alpha *= projectile.localAI[0] / 10f;
                    }
                    else if (projectile.localAI[0] >= 30)
                    {
                        alpha *= (45 - projectile.localAI[0]) / 15f;
                    }
                    projectile.DrawAim(spriteBatch, projectile.Center + projectile.velocity.SafeNormalize(Vector2.Zero) * 1500, alpha);
                }
            }
            return base.PreDraw(spriteBatch, lightColor);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasBeenLaunched);
            writer.Write(AIState);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasBeenLaunched = reader.ReadBoolean();
            AIState = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
    }
}
