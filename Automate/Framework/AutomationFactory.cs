using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate.Framework.Machines.Buildings;
using Pathoschild.Stardew.Automate.Framework.Machines.Objects;
using Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures;
using Pathoschild.Stardew.Automate.Framework.Machines.Tiles;
using Pathoschild.Stardew.Automate.Framework.Models;
using Pathoschild.Stardew.Automate.Framework.Storage;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.Automate.Framework
{
    /// <summary>Constructs machines, containers, or connectors which can be added to a machine group.</summary>
    internal class AutomationFactory : IAutomationFactory
    {
        /*********
        ** Fields
        *********/
        /// <summary>Encapsulates monitoring and logging.</summary>
        private readonly IMonitor Monitor;

        /// <summary>Simplifies access to private game code.</summary>
        private readonly IReflectionHelper Reflection;

        /// <summary>The object names through which machines can connect, but which have no other automation properties.</summary>
        private readonly HashSet<string> Connectors;

        /// <summary>Whether to treat the shipping bin as a machine that can be automated.</summary>
        private readonly bool AutomateShippingBin;

        /// <summary>The tile area on the farm matching the shipping bin.</summary>
        private readonly Rectangle ShippingBinArea = new Rectangle(71, 14, 2, 1);

        /// <summary>The internal Automate data that can't be derived automatically.</summary>
        private readonly DataModel Data;

        /// <summary>Whether to enable compatibility with the Better Junimos mod.</summary>
        private readonly bool BetterJunimosCompat;

        /// <summary>Whether to enable compatibility with Auto-Grabber Mod.</summary>
        private readonly bool AutoGrabberModCompat;

        /// <summary>Whether to pull gemstones out of Junimo huts.</summary>
        public bool PullGemstonesFromJunimoHuts { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="connectors">The objects through which machines can connect, but which have no other automation properties.</param>
        /// <param name="automateShippingBin">Whether to treat the shipping bin as a machine that can be automated.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="reflection">Simplifies access to private game code.</param>
        /// <param name="data">The internal Automate data that can't be derived automatically.</param>
        /// <param name="betterJunimosCompat">Whether to enable compatibility with the Better Junimos mod.</param>
        /// <param name="autoGrabberModCompat">Whether to enable compatibility with Auto-Grabber Mod.</param>
        /// <param name="pullGemstonesFromJunimoHuts">Whether to pull gemstones out of Junimo huts.</param>
        public AutomationFactory(string[] connectors, bool automateShippingBin, IMonitor monitor, IReflectionHelper reflection, DataModel data, bool betterJunimosCompat, bool autoGrabberModCompat, bool pullGemstonesFromJunimoHuts)
        {
            this.Connectors = new HashSet<string>(connectors, StringComparer.InvariantCultureIgnoreCase);
            this.AutomateShippingBin = automateShippingBin;
            this.Monitor = monitor;
            this.Reflection = reflection;
            this.Data = data;
            this.BetterJunimosCompat = betterJunimosCompat;
            this.AutoGrabberModCompat = autoGrabberModCompat;
            this.PullGemstonesFromJunimoHuts = pullGemstonesFromJunimoHuts;
        }

        /// <summary>Get a machine, container, or connector instance for a given object.</summary>
        /// <param name="obj">The in-game object.</param>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile)
        {
            // chest container 
            if (obj is Chest chest)
                return new ChestContainer(chest, location, tile, this.Reflection);

            // machine
            if (obj.ParentSheetIndex == 165)
                return new AutoGrabberMachine(obj, location, tile, ignoreSeedOutput: this.AutoGrabberModCompat, ignoreFertilizerOutput: this.AutoGrabberModCompat);
            if (obj.name == "Bee House")
                return new BeeHouseMachine(obj, location, tile);
            if (obj is Cask cask)
                return new CaskMachine(cask, location, tile);
            if (obj.name == "Charcoal Kiln")
                return new CharcoalKilnMachine(obj, location, tile);
            if (obj.name == "Cheese Press")
                return new CheesePressMachine(obj, location, tile);
            if (obj is CrabPot pot)
                return new CrabPotMachine(pot, location, tile, this.Monitor, this.Reflection);
            if (obj.Name == "Crystalarium")
                return new CrystalariumMachine(obj, location, tile, this.Reflection);
            if (obj.name == "Feed Hopper")
                return new FeedHopperMachine(location, tile);
            if (obj.Name == "Furnace")
                return new FurnaceMachine(obj, location, tile);
            if (obj.name == "Incubator")
                return new CoopIncubatorMachine(obj, location, tile);
            if (obj.Name == "Keg")
                return new KegMachine(obj, location, tile);
            if (obj.name == "Lightning Rod")
                return new LightningRodMachine(obj, location, tile);
            if (obj.name == "Loom")
                return new LoomMachine(obj, location, tile);
            if (obj.name == "Mayonnaise Machine")
                return new MayonnaiseMachine(obj, location, tile);
            if (obj.Name == "Mushroom Box")
                return new MushroomBoxMachine(obj, location, tile);
            if (obj.name == "Oil Maker")
                return new OilMakerMachine(obj, location, tile);
            if (obj.name == "Preserves Jar")
                return new PreservesJarMachine(obj, location, tile);
            if (obj.name == "Recycling Machine")
                return new RecyclingMachine(obj, location, tile);
            if (obj.name == "Seed Maker")
                return new SeedMakerMachine(obj, location, tile);
            if (obj.name == "Slime Egg-Press")
                return new SlimeEggPressMachine(obj, location, tile);
            if (obj.name == "Slime Incubator")
                return new SlimeIncubatorMachine(obj, location, tile);
            if (obj.name == "Soda Machine")
                return new SodaMachine(obj, location, tile);
            if (obj.name == "Statue Of Endless Fortune")
                return new StatueOfEndlessFortuneMachine(obj, location, tile);
            if (obj.name == "Statue Of Perfection")
                return new StatueOfPerfectionMachine(obj, location, tile);
            if (obj.name == "Tapper")
            {
                if (location.terrainFeatures.TryGetValue(tile, out TerrainFeature terrainFeature) && terrainFeature is Tree tree)
                    return new TapperMachine(obj, location, tile, tree.treeType.Value);
            }
            if (obj is WoodChipper woodChipper)
                return new WoodChipperMachine(woodChipper, location, tile);
            if (obj.name == "Worm Bin")
                return new WormBinMachine(obj, location, tile);

            // connector
            if (this.IsConnector(obj))
                return new Connector(location, tile);

            return null;
        }

        /// <summary>Get a machine, container, or connector instance for a given terrain feature.</summary>
        /// <param name="feature">The terrain feature.</param>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
        {
            // machine
            if (feature is FruitTree fruitTree)
                return new FruitTreeMachine(fruitTree, location, tile);
            if (feature is Bush bush && BushMachine.CanAutomate(bush))
                return new BushMachine(bush, location);

            // connector
            if (this.IsConnector(feature))
                return new Connector(location, tile);

            return null;
        }

        /// <summary>Get a machine, container, or connector instance for a given building.</summary>
        /// <param name="building">The building.</param>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        public IAutomatable GetFor(Building building, BuildableGameLocation location, in Vector2 tile)
        {
            // machine
            if (building is FishPond pond)
                return new FishPondMachine(pond, location);
            if (building is JunimoHut hut)
                return new JunimoHutMachine(hut, location, ignoreSeedOutput: this.BetterJunimosCompat, ignoreFertilizerOutput: this.BetterJunimosCompat, pullGemstonesFromJunimoHuts: this.PullGemstonesFromJunimoHuts);
            if (building is Mill mill)
                return new MillMachine(mill, location);
            if (this.AutomateShippingBin && building is ShippingBin bin)
                return new ShippingBinMachine(bin, location, Game1.getFarm());
            if (building.buildingType.Value == "Silo")
                return new FeedHopperMachine(building, location);
            return null;
        }

        /// <summary>Get a machine, container, or connector instance for a given tile position.</summary>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile position to check.</param>
        /// <returns>Returns an instance or <c>null</c>.</returns>
        /// <remarks>Shipping bin logic from <see cref="Farm.leftClick"/>, garbage can logic from <see cref="Town.checkAction"/>.</remarks>
        public IAutomatable GetForTile(GameLocation location, in Vector2 tile)
        {
            // shipping bin
            if (this.AutomateShippingBin && location is Farm farm && (int)tile.X == this.ShippingBinArea.X && (int)tile.Y == this.ShippingBinArea.Y)
            {
                return new ShippingBinMachine(farm, this.ShippingBinArea);
            }

            // garbage can
            if (location is Town town)
            {
                string action = town.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Action", "Buildings");
                if (!string.IsNullOrWhiteSpace(action) && action.StartsWith("Garbage ") && int.TryParse(action.Split(' ')[1], out int trashCanIndex))
                    return new TrashCanMachine(town, tile, trashCanIndex, this.Reflection);
            }

            return null;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether a given in-game entity should be treated as a connector.</summary>
        /// <param name="entity">The in-game entity.</param>
        private bool IsConnector(object entity)
        {
            switch (entity)
            {
                case Item item:
                    return this.Connectors.Contains(item.Name);

                case Flooring floor:
                    return
                        this.Data?.FloorNames != null
                        && this.Data.FloorNames.TryGetValue(floor.whichFloor.Value, out string name)
                        && this.Connectors.Contains(name);

                default:
                    return false;
            }
        }
    }
}
