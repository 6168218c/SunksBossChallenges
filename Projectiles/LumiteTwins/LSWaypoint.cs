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
    public class LSWaypointLauncher : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waypoint Launcher");
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
            Player player = Main.player[(int)projectile.ai[0]];
            projectile.Loomup();
            //spawn 6 waypoints
            if (projectile.localAI[0] % 45 == 0 && Main.netMode != NetmodeID.MultiplayerClient && projectile.localAI[0] / 45 < 6)
            {
                if (projectile.localAI[0] == 0)
                {
                    projectile.ai[1] = projectile.localAI[1] =
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LSWaypoint>(),
                    0, 0f, projectile.owner, -1, -1);
                }
                else
                {
                    projectile.localAI[1] = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LSWaypoint>(),
                    0, 0f, projectile.owner, projectile.localAI[1], -1);
                }
                projectile.netUpdate = true;
            }
            if (Util.CheckProjAlive<LSWaypoint>((int)projectile.localAI[1])&& projectile.localAI[0] / 45 <= 6)
            {
                Projectile proj = Main.projectile[(int)projectile.localAI[1]];
                proj.Center = player.Center + player.velocity * 30;
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
    public class LSWaypoint:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.StardustTowerMark;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waypoint");
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
            //ai[0]:prev,ai[1]:next
            projectile.Loomup();
            if (projectile.ai[0] != -1 && Main.projectile[(int)projectile.ai[0]].active
                && Main.projectile[(int)projectile.ai[0]].type == ModContent.ProjectileType<LSWaypoint>())
            {
                Main.projectile[(int)projectile.ai[0]].ai[1] = projectile.whoAmI;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[1] != -1 && Main.projectile[(int)projectile.ai[1]].active
                && Main.projectile[(int)projectile.ai[1]].type == ModContent.ProjectileType<LSWaypoint>())
            {
                projectile.DrawAim(spriteBatch, Main.projectile[(int)projectile.ai[1]].Center, Color.Green);
            }
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            lightColor = Color.Lerp(lightColor, Color.Green, 0.5f);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
