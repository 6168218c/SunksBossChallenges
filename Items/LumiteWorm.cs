using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.NPCs.LumiteDestroyer;

namespace SunksBossChallenges.Items
{
    public class LumiteWorm : ModItem
    {
        public override string Texture => "Terraria/Item_" + ItemID.MechanicalWorm;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lumite Worm");
            Tooltip.SetDefault("You've defeated the mech bosses.But who said they are the FINAL version?\nSummons Lumite Destroyer");
        }
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 30;
            item.maxStack = 1;
            item.rare = ItemRarityID.Purple;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<LumiteDestroyerHead>());
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MechanicalWorm);
            recipe.AddIngredient(ItemID.FragmentSolar);
            recipe.AddIngredient(ItemID.FragmentNebula);
            recipe.AddIngredient(ItemID.FragmentStardust);
            recipe.AddIngredient(ItemID.FragmentVortex);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool CanUseItem(Player player)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<LumiteDestroyerHead>()))
                return false;
            return true;
        }
    }
}
