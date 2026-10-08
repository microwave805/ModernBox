using System;
using System.Collections.Generic;
using HarmonyLib;

namespace ModernBoxM2Rewrite
{
    /// <summary>
    /// Per-unit max_age, knockback, targets and knockback_reduction from the original
    /// M2 assets (last assignment, or the value inherited from the cloned asset).
    /// </summary>
    internal static class M2OriginalStats
    {
        // Old engine: tiles/s = speed * 0.1. 0.51.2: speed * 0.4 * unit_speed_multiplier (0.5) = speed * 0.2.
        // Every M2 speed value and speed bonus is halved so units move as fast as in the original.
        internal const float SpeedScale = 0.5f;

        // id -> { max_age, knockback, targets, knockback_reduction }; 0 means not set.
        private static readonly Dictionary<string, float[]> Stats = new Dictionary<string, float[]>(StringComparer.Ordinal)
        {
            { "AT9000", new[] { 50f, 0f, 0f, 300f } },
            { "AbramTank", new[] { 50f, 0f, 0f, 300f } },
            { "Assimilatus", new[] { 100f, 1f, 0f, 20f } },
            { "Cocytuswalker", new[] { 100f, 1f, 10f, 20f } },
            { "Drone", new[] { 5f, 0f, 0f, 300f } },
            { "Duneworm", new[] { 100f, 0f, 0f, 0.8f } },
            { "EliteBomber", new[] { 50f, 0f, 0f, 300f } },
            { "EliteP9000", new[] { 50f, 0f, 0f, 300f } },
            { "EliteZeppelin", new[] { 50f, 0f, 0f, 300f } },
            { "F55FighterJet", new[] { 50f, 0f, 0f, 300f } },
            { "F55FighterJet1", new[] { 1f, 0f, 0f, 300f } },
            { "FighterJet", new[] { 50f, 0f, 0f, 300f } },
            { "FighterJet1", new[] { 1f, 0f, 0f, 300f } },
            { "Gunship", new[] { 50f, 0f, 0f, 0f } },
            { "Heli", new[] { 50f, 0f, 0f, 300f } },
            { "HeliELite", new[] { 50f, 0f, 0f, 300f } },
            { "HumanTitan", new[] { 10000f, 0f, 0f, 300f } },
            { "HumanTitanElite", new[] { 10000f, 0f, 0f, 300f } },
            { "Humvee", new[] { 50f, 0f, 0f, 300f } },
            { "MA9000", new[] { 200f, 0f, 0f, 300f } },
            { "MIRVBomber", new[] { 50f, 0f, 0f, 300f } },
            { "MissileSystem", new[] { 50f, 0f, 0f, 300f } },
            { "OmegaRailgun", new[] { 50f, 0f, 0f, 320f } },
            { "P9000", new[] { 50f, 0f, 0f, 300f } },
            { "Railgun", new[] { 50f, 0f, 0f, 320f } },
            { "SpaceMarine", new[] { 500f, 0f, 0f, 0f } },
            { "TIEfighter", new[] { 50f, 0f, 0f, 300f } },
            { "Tank", new[] { 50f, 0f, 0f, 300f } },
            { "Terlanius", new[] { 1000f, 0.1f, 1f, 0.1f } },
            { "Terran", new[] { 50f, 0f, 0f, 300f } },
            { "Zeppelin", new[] { 50f, 0f, 0f, 300f } },
            { "alienwisp", new[] { 50f, 0f, 0f, 0f } },
            { "americanbomberww", new[] { 50f, 0f, 0f, 300f } },
            { "armoredwolf", new[] { 50f, 0.8f, 0f, 0f } },
            { "artilleryatst", new[] { 50f, 0f, 0f, 300f } },
            { "assimilatrax", new[] { 0f, 2f, 4f, 0f } },
            { "atst", new[] { 50f, 0f, 0f, 300f } },
            { "atstsniper", new[] { 50f, 0f, 0f, 300f } },
            { "balloonunit", new[] { 50f, 0f, 0f, 0f } },
            { "baseMA9000", new[] { 50f, 0f, 0f, 300f } },
            { "basecrusader", new[] { 10f, 2f, 5f, 0f } },
            { "batteringram", new[] { 50f, 3f, 6f, 10f } },
            { "bigfaerydragon", new[] { 50f, 0f, 0f, 0f } },
            { "bigtankww", new[] { 50f, 0f, 0f, 300f } },
            { "biplane", new[] { 50f, 0f, 0f, 300f } },
            { "buffrost", new[] { 0f, 2f, 4f, 0f } },
            { "catapulta", new[] { 50f, 0f, 0f, 10f } },
            { "crusaderHeli", new[] { 10f, 0f, 0f, 300f } },
            { "crusaderdreadnaught", new[] { 10f, 0f, 0f, 3f } },
            { "crusadermaus", new[] { 10f, 0f, 0f, 300f } },
            { "davincitank", new[] { 50f, 0f, 0f, 3f } },
            { "dreadnaught", new[] { 5000f, 0f, 0f, 3f } },
            { "dwarfcannon", new[] { 50f, 0f, 0f, 1f } },
            { "dwarfdoctor", new[] { 50f, 0f, 1f, 0f } },
            { "elfcannon", new[] { 50f, 0f, 0f, 1f } },
            { "eliteAT9000", new[] { 400f, 0f, 0f, 300f } },
            { "eliteGunship", new[] { 50f, 0f, 0f, 300f } },
            { "fairelf", new[] { 50f, 0f, 1f, 0f } },
            { "fairydragon", new[] { 50f, 0f, 1f, 0f } },
            { "fighterww", new[] { 50f, 0f, 0f, 300f } },
            { "fishing_boat_industrial", new[] { 25f, 0.8f, 0f, 6f } },
            { "fishing_boat_modern", new[] { 25f, 0.8f, 0f, 6f } },
            { "fishing_boat_renaissance", new[] { 25f, 0.8f, 0f, 6f } },
            { "geckoid", new[] { 200f, 0f, 0f, 0f } },
            { "genericwwtank", new[] { 50f, 0f, 0f, 300f } },
            { "glitchdrake", new[] { 1000f, 0f, 0f, 4f } },
            { "glitchtarantula", new[] { 200f, 0f, 0f, 1f } },
            { "golemgem", new[] { 50f, 0f, 4f, 0f } },
            { "hashbrowncat", new[] { 100f, 0f, 0f, 0f } },
            { "human_industrial_battleship", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_industrial_corvette1", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_industrial_corvette2", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_industrial_trading", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_modern_battleship", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_modern_corvette1", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_modern_corvette2", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_modern_gunboat", new[] { 4f, 0f, 0f, 3f } },
            { "human_modern_submarine", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_modern_trading", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_renaissance_battleship", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_renaissance_corvette1", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_renaissance_corvette2", new[] { 25f, 0.8f, 0f, 6f } },
            { "human_renaissance_trading", new[] { 25f, 0.8f, 0f, 6f } },
            { "humancannon", new[] { 50f, 0f, 0f, 1f } },
            { "humancavalry", new[] { 50f, 0f, 2f, 0f } },
            { "humanpaladin", new[] { 50f, 0f, 1f, 0f } },
            { "icedracoid", new[] { 0f, 0f, 1f, 0f } },
            { "landship", new[] { 50f, 0f, 0f, 300f } },
            { "modernhumvee", new[] { 50f, 0f, 0f, 300f } },
            { "modernsupporttruck", new[] { 50f, 0f, 0f, 300f } },
            { "newwalker", new[] { 0f, 1f, 2f, 0f } },
            { "normalwalker", new[] { 0f, 1f, 2f, 0f } },
            { "ogreunit", new[] { 50f, 0f, 4f, 0f } },
            { "orcatapulta", new[] { 50f, 0f, 0f, 10f } },
            { "orccannon", new[] { 50f, 0f, 0f, 1f } },
            { "orcwarlock", new[] { 50f, 0f, 1f, 0f } },
            { "orcwarturtle", new[] { 25f, 0.8f, 0f, 6f } },
            { "pantherax", new[] { 200f, 0f, 6f, 2f } },
            { "peones", new[] { 100f, 0f, 0f, 0f } },
            { "pterax", new[] { 100f, 0f, 0f, 0f } },
            { "rhinokinglor", new[] { 200f, 1.2f, 3f, 0.3f } },
            { "santaguin", new[] { 50f, 0f, 0f, 10f } },
            { "scandid", new[] { 100f, 0f, 0f, 0.8f } },
            { "shermanww", new[] { 50f, 0f, 0f, 300f } },
            { "spaceork", new[] { 200f, 0f, 0f, 0f } },
            { "supportatst", new[] { 50f, 0f, 0f, 300f } },
            { "tankie", new[] { 50f, 0f, 0f, 300f } },
            { "teslatruckgun", new[] { 50f, 0f, 0f, 300f } },
            { "treant", new[] { 50f, 0f, 1f, 0f } },
            { "wheeledtank", new[] { 50f, 0f, 0f, 300f } },
            { "woolyrhino", new[] { 50f, 3f, 6f, 10f } },
            { "wwartillery", new[] { 50f, 0f, 0f, 300f } },
            { "wwsupporttruck", new[] { 50f, 0f, 0f, 300f } },
            { "xenodogo", new[] { 100f, 0f, 0f, 0f } },
            { "zombieabomination", new[] { 200f, 3f, 0f, 5f } },
            { "zombieacid", new[] { 200f, 1f, 0f, 1f } },
            { "zombieacidman", new[] { 200f, 1f, 0f, 1f } },
            { "zombieballoon", new[] { 200f, 6f, 0f, 5f } },
            { "zombieclawed", new[] { 200f, 6f, 0f, 5f } },
            { "zombiedemon", new[] { 200f, 1f, 0f, 1f } },
            { "zombiedoctor", new[] { 200f, 1f, 1f, 1f } },
            { "zombiedruid", new[] { 200f, 1f, 0f, 1f } },
            { "zombieevilhorseman", new[] { 200f, 1f, 1f, 1f } },
            { "zombiefairy", new[] { 200f, 1f, 0f, 1f } },
            { "zombiefiremaniac", new[] { 200f, 1f, 0f, 1f } },
            { "zombiehulk", new[] { 200f, 3f, 0f, 5f } },
            { "zombieicelich", new[] { 200f, 1f, 1f, 1f } },
            { "zombiemother", new[] { 200f, 8f, 0f, 10f } },
            { "zombiepoison", new[] { 200f, 0f, 0f, 1f } },
            { "zombiespeed", new[] { 200f, 1f, 0f, 1f } },
            { "zombiespikes", new[] { 200f, 0f, 0f, 1f } },
            { "zombiestalker", new[] { 200f, 2f, 0f, 3f } },
            { "zombietarantula", new[] { 200f, 1f, 0f, 1f } },
            { "zombietentacle", new[] { 200f, 1f, 0f, 1f } },
        };

        internal static void Apply(ActorAsset actor, string id)
        {
            float[] stats;
            if (!Stats.TryGetValue(id, out stats)) return;
            if (stats[0] > 0f) actor.base_stats["lifespan"] = stats[0];
            if (stats[1] > 0f) actor.base_stats["knockback"] = stats[1];
            if (stats[2] > 0f) actor.base_stats["targets"] = stats[2];
        }

        // 0.51.2 dropped knockback_reduction; the old engine scaled every push by (1 - value).
        internal static float ForceMultiplier(ActorAsset asset)
        {
            float[] stats;
            if (asset == null || !Stats.TryGetValue(asset.id, out stats) || stats[3] <= 0f) return 1f;
            return Math.Max(0f, 1f - stats[3]);
        }
    }

    [HarmonyPatch(typeof(Actor), nameof(Actor.calculateForce))]
    internal static class M2KnockbackReductionPatch
    {
        private static bool Prefix(Actor __instance, ref float pForceAmountDirection, ref float pForceHeight)
        {
            float multiplier = M2OriginalStats.ForceMultiplier(__instance.asset);
            if (multiplier >= 1f) return true;
            if (multiplier <= 0f) return false;
            pForceAmountDirection *= multiplier;
            pForceHeight *= multiplier;
            return true;
        }
    }

    // M2 unit stat fixes after each recalculation:
    // - 0.51.2 takes base stats from the subspecies and ignores the asset's own values. The port
    //   gives humanoid M2 soldiers their city's subspecies, so swap the subspecies base back.
    // - Level bonuses follow the old engine.
    [HarmonyPatch(typeof(Actor), nameof(Actor.updateStats))]
    internal static class M2UnitStatsPatch
    {
        private static readonly string[] Keys =
        {
            "health", "damage", "speed", "armor", "attack_speed", "range", "lifespan",
            "targets", "knockback", "projectiles", "accuracy"
        };

        // updateStats returns early when the stats are not dirty; only correct a fresh recalculation.
        private static void Prefix(Actor __instance, out bool __state)
        {
            __state = __instance != null && __instance.isStatsDirty();
        }

        private static void Postfix(Actor __instance, bool __state)
        {
            if (!__state || __instance == null || __instance.asset == null || __instance.data == null || !__instance.isAlive()) return;
            ModernUnitSpec spec = ContentRegistry.FindUnit(__instance.asset.id);
            if (spec == null) return;
            BaseStats stats = __instance.stats;
            Subspecies subspecies = __instance.subspecies;
            if (spec.Humanoid && subspecies != null)
            {
                BaseStats sexed = __instance.isSexMale() ? subspecies.base_stats_male : subspecies.base_stats_female;
                foreach (string key in Keys)
                {
                    float fromSubspecies = subspecies.base_stats[key] + (sexed == null ? 0f : sexed[key]);
                    float delta = __instance.asset.base_stats[key] - fromSubspecies;
                    if (delta != 0f) stats[key] += delta;
                }
            }

            // The game's own traits, statuses, clans, languages and cultures add attack_speed
            // in old-engine numbers (fast is +5 in both engines). On 0.51.2's 1/s scale that
            // is +5 shots a second, so an M2 vehicle with fast fired 16x as often. Convert
            // those bonuses the way the old engine would have applied them.
            float vanilla = VanillaAttackSpeed(__instance);
            if (vanilla != 0f)
            {
                float oldBase = M2AttackSpeed.OldTotal(spec.AttackSpeed, spec.Attack);
                float scale = 1f + stats["multiplier_attack_speed"];
                stats["attack_speed"] += (M2AttackSpeed.Delta(vanilla, oldBase) - vanilla) * scale;
            }

            // Old Actor.updateStats: per level above 1, +20 health, +1/2 damage, +1/3 armor and
            // +1 old-scale attack_speed. 0.51.2 gives level * 5% health instead; swap it back.
            int level = __instance.data.level;
            if (level > 0)
            {
                float newBonus = 1f + level * SimGlobals.m.level_mod_bonus_health;
                if (newBonus > 0f) stats["health"] = stats["health"] / newBonus + (level - 1) * 20;
                stats["damage"] += (level - 1) / 2;
                stats["armor"] += (level - 1) / 3;
                float oldAttack = M2AttackSpeed.OldTotal(spec.AttackSpeed, spec.Attack);
                stats["attack_speed"] += M2AttackSpeed.ToNew(oldAttack + level - 1) - M2AttackSpeed.ToNew(oldAttack);
            }
            stats.normalize();
            if (__instance.getHealth() > __instance.getMaxHealth()) __instance.setMaxHealth();
        }

        private static float VanillaAttackSpeed(Actor actor)
        {
            float total = 0f;
            foreach (ActorTrait trait in actor.getTraits())
            {
                if (trait.only_active_on_era_flag && ((trait.era_active_moon && !World.world_era.flag_moon) ||
                    (trait.era_active_night && !World.world_era.overlay_darkness))) continue;
                if (M2AttackSpeed.IsVanillaTrait(trait)) total += trait.base_stats["attack_speed"];
            }
            if (actor.hasAnyStatusEffect())
                foreach (Status status in actor.getStatuses())
                    total += status.asset.base_stats["attack_speed"];
            if (actor.hasClan())
            {
                total += actor.clan.base_stats["attack_speed"];
                total += (actor.isSexMale() ? actor.clan.base_stats_male : actor.clan.base_stats_female)["attack_speed"];
            }
            if (actor.hasLanguage()) total += actor.language.base_stats["attack_speed"];
            if (actor.hasCulture()) total += actor.culture.base_stats["attack_speed"];
            return total;
        }
    }

    // Old Actor.updateAge ran once a year and killed a unit past max_age with a 13% roll.
    // 0.51.2's checkNaturalDeath runs many times a second, so M2 units would die almost
    // exactly at their lifespan. M2 units get the old yearly roll instead.
    [HarmonyPatch(typeof(Actor), nameof(Actor.checkNaturalDeath))]
    internal static class M2OldAgePatch
    {
        private const string LastRollKey = "modernbox_m2_age_roll";

        private static bool Prefix(Actor __instance, ref bool __result)
        {
            if (__instance == null || __instance.asset == null || __instance.data == null ||
                ContentRegistry.FindUnit(__instance.asset.id) == null) return true;
            __result = false;
            if (!WorldLawLibrary.world_law_old_age.isEnabled() || __instance.hasTrait("immortal")) return false;
            float lifespan = __instance.stats["lifespan"];
            int age = __instance.getAge();
            if (lifespan == 0f || age < lifespan) return false;
            float lastRoll;
            __instance.data.get(LastRollKey, out lastRoll, -1f);
            if (lastRoll >= age) return false;
            __instance.data.set(LastRollKey, (float)age);
            if (!Randy.randomChance(0.13f)) return false;
            __instance.getHitFullHealth(AttackType.Age);
            __result = true;
            return false;
        }
    }
}
