using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.Projectiles.DecimatorOfPlanets;

namespace SunksBossChallenges.Items
{
	public class Tester : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Diamond"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("This is a basic modded sword.");
		}

		public override void SetDefaults() 
		{
			item.damage = 2000;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<LaserBarrage>();
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			const float length = 600f;
			for(int i = 0; i < 5; i++)
            {
				Vector2 pos = Main.rand.NextVector2Unit(MathHelper.Pi / 3, MathHelper.Pi / 3) * length;
				Projectile.NewProjectile(player.Center + pos, Vector2.Zero, ModContent.ProjectileType<LaserBarrage>(), 60, 0f, Main.myPlayer, player.Center.X, player.Center.Y);
			}
			return true;
		}

        public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}