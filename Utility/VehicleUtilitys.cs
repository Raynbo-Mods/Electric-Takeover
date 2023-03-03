﻿// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Shared.Utils;
    using Eco.World.Blocks;

    // A place for misc vehicle utilities
    public class ModVehicleUtilities
    {
        // Mapping for custom stack sizes in vehicles by vehicle type as key
        // We can have different stack sizes in different vehicles with this
        public static Dictionary<Type, StackLimitTypeRestriction> AdvancedVehicleStackSizeMap = new Dictionary<Type, StackLimitTypeRestriction>();

        static ModVehicleUtilities() => CreateBlockStackSizeMaps();

        private static void CreateBlockStackSizeMaps()
        {
            var blockItems = Item.AllItems.Where(x => x is BlockItem).Cast<BlockItem>().ToList();

            // Excavator
            var excavatorMap = new StackLimitTypeRestriction(true, 30);

            excavatorMap.AddListRestriction(blockItems.GetItemsByBlockAttribute<Diggable>(), 20);
            excavatorMap.AddListRestriction(blockItems.GetItemsByBlockAttribute<Minable>(), 80);

            AdvancedVehicleStackSizeMap.Add(typeof(ElectricExcavatorObject), excavatorMap);

            // Skidsteer (same as excavator currently)
            AdvancedVehicleStackSizeMap.Add(typeof(ElectricSkidSteerObject), excavatorMap);

            // Tractor
            var tractorMap = new StackLimitTypeRestriction();
            tractorMap.AddListRestriction(ItemUtils.GetItemsByTag("Seeds", "Crop"), 500);
            AdvancedVehicleStackSizeMap.Add(typeof(ElectricSteamTractorObject), tractorMap);
        }

        public static StackLimitTypeRestriction GetInventoryRestriction(object obj) => AdvancedVehicleStackSizeMap.GetOrDefault(obj.GetType());
    }
}