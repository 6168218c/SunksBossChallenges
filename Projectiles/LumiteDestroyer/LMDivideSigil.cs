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
    public class LMDivideSigil:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.CultistRitual;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Divide Sigil");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = (int)LumiteDestroyerArguments.TeleportDistance * 2;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }
        public override void AI()
        {
            projectile.alpha -= 25;
            if (projectile.alpha < 0) projectile.alpha = 0;
            if (projectile.localAI[1] > 0)
            {
                projectile.velocity = Vector2.Zero;
                projectile.scale -= 0.02f;
                if (projectile.scale < 0.03f)
                {
                    projectile.Kill();
                }
                return;
            }
            if (NPC.FindFirstNPC(ModContent.NPCType<LumiteDestroyerHead>()) == -1)
            {
                projectile.localAI[1] = 1;
            }
            Player player = Main.player[(int)projectile.ai[0]];
            projectile.Center = player.Center;
            projectile.rotation = MathHelper.WrapAngle(projectile.rotation + 0.015f);
            if (projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                projectile.rotation = Main.rand.NextFloatDirection();
                for (int i = 0; i < 3; i++)
                {
                    float rotate = MathHelper.Pi * 2 / 3 * i;
                    Projectile.NewProjectile(projectile.Center + rotate.ToRotationVector2() * LumiteDestroyerArguments.TeleportDistance,
                        Vector2.Zero, ModContent.ProjectileType<LMSubDivideSigil>(), 0, 0f, projectile.owner, rotate, projectile.whoAmI);
                }
                projectile.netUpdate = true;
            }
            projectile.localAI[0]++;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            Texture2D aimTexture = mod.GetTexture("Projectiles/AimLine");
            float lineCenterDist = 80;
            float lineHalfLen = lineCenterDist * (float)Math.Tan(Math.PI / 2.5);
            Vector2 baseUnit = (-projectile.rotation).ToRotationVector2();
            Vector2 baseVector = baseUnit * lineCenterDist;

            if (projectile.ai[1] == 1 && projectile.localAI[1] == 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (projectile.localAI[0] < i * 20)
                        break;
                    var start = projectile.Center + baseVector + baseUnit.RotatedBy(Math.PI / 2) * lineHalfLen;
                    var end = projectile.Center + baseVector + baseUnit.RotatedBy(-Math.PI / 2) * lineHalfLen;
                    var scale = new Vector2(1);
                    Vector2 unit = end - start;
                    float length = unit.Length();
                    unit.Normalize();
                    for (int k = 0; k <= length; k += 4)
                    {
                        Vector2 drawPos = start + unit * k - Main.screenPosition;
                        Color alphaCenter = Color.Lerp(Color.Yellow, Color.LightGoldenrodYellow, (float)Math.Sin(MathHelper.Pi / 15 * projectile.localAI[0]))
                            * Math.Min(1f, ((projectile.localAI[0] - i * 20) / 20f));
                        spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), scale, SpriteEffects.None, 0f);
                    }
                    baseVector = baseVector.RotatedBy(Math.PI / 2.5);
                    baseUnit = baseUnit.RotatedBy(Math.PI / 2.5);
                }
            }

            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }
    }
    public class LMSubDivideSigil : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.CultistRitual;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Divide Sigil");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = (int)LumiteDestroyerArguments.R * 2;
            projectile.scale = 0.3f;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }
        public override void AI()
        {
            projectile.alpha -= 25;
            if (projectile.alpha < 0) projectile.alpha = 0;
            if (Util.CheckProjAlive<LMDivideSigil>((int)projectile.ai[1]))
            {
                projectile.rotation = MathHelper.WrapAngle(projectile.rotation - 0.025f);
                projectile.Center = Main.projectile[(int)projectile.ai[1]].Center +
                    Main.projectile[(int)projectile.ai[1]].rotation.ToRotationVector2().RotatedBy(projectile.ai[0])
                    * LumiteDestroyerArguments.TeleportDistance;
            }
            else
            {
                projectile.localAI[0]++;
            }
            if (projectile.localAI[0] >= 120)
            {
                projectile.scale *= 0.9f;
                if (projectile.scale < 0.01f)
                    projectile.Kill();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }
    }
}
