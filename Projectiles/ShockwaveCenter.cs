using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace SunksBossChallenges.Projectiles
{
    public class ShockwaveCenter:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockwave Center");
            base.SetStaticDefaults();
        }
		public override void SetDefaults()
		{
			projectile.hostile = false;
			projectile.magic = true;
			projectile.width = 10;
			projectile.height = 10;
			projectile.friendly = false;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.timeLeft = 180;
			projectile.tileCollide = false;
		}
		//i wonder why my code didn't work so i used spirit mod's
		//int counter = -720;
		bool boom = false;
		private float distortStrength = 300f;
		public override bool PreAI()
		{
			if (!boom)
			{
				if (!Main.dedServ && !Filters.Scene["SunksBossChallenges:Shockwave"].IsActive())
				{
					Filters.Scene.Activate("SunksBossChallenges:Shockwave", projectile.Center).GetShader().UseColor(2, 5, 15).UseTargetPosition(projectile.Center);
				}
				boom = true;
			}
			if (!Main.dedServ && Filters.Scene["SunksBossChallenges:Shockwave"].IsActive())
			{
				float progress = (180 - projectile.timeLeft) / 60f;// Will range from -3 to 3, 0 being the point where the bomb explodes.
				if (projectile.ai[0] == 1)
					progress = projectile.timeLeft / 60f;  //reversed
				Filters.Scene["SunksBossChallenges:Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 8f));
			}
			return false;
		}
		public override void Kill(int timeLeft)
		{
			if (!Main.dedServ && Filters.Scene["SunksBossChallenges:Shockwave"].IsActive())
			{
				Filters.Scene["SunksBossChallenges:Shockwave"].Deactivate();
			}
		}
	}
}
