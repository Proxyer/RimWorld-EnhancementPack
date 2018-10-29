﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.Planet;
using Harmony;

namespace TD_Enhancement_Pack
{
	[StaticConstructorOnStartup]
	class FertilityOverlay : BaseOverlay
	{
		public static Dictionary<Map, FertilityOverlay> fertilityOverlays = new Dictionary<Map, FertilityOverlay>();

		public static void DirtyAll()
		{
			foreach(var kvp in fertilityOverlays)
			{
				kvp.Value.SetDirty();
			}
		}

		public static readonly Color noneColor = new Color(1, 0, 0);
		public static readonly Color lightColor = new Color(.8f, .8f, 0);

		public FertilityOverlay(Map m) : base(m) { }

		public override bool GetCellBool(int index)
		{
			float f = FertilityAt(map, index);
			return f != 1
				&& !map.fogGrid.IsFogged(index);
		}

		public override Color GetCellExtraColor(int index)
		{
			float f = FertilityAt(map, index);
			return f < 1 ? Color.Lerp(Color.red, Color.yellow, f)
				: Color.Lerp(Color.green, Color.white, f-1);
		}

		public static float FertilityAt(Map map, int index)
		{
			if (Settings.Get().cheatFertilityUnderGrid)
			{
				FieldInfo underGridInfo = AccessTools.Field(typeof(TerrainGrid), "underGrid");
				if ((underGridInfo.GetValue(map.terrainGrid) as TerrainDef[])[index] is TerrainDef def)
					return def.fertility; 
			}
			return map.terrainGrid.TerrainAt(index).fertility;
		}

		public override bool ShouldDraw() => PlaySettings_Patch_Fertility.showFertilityOverlay;
		public override bool ShouldAutoDraw() => Settings.Get().autoOverlayFertility;
		public override Type AutoDesignator() => typeof(Designator_ZoneAdd_Growing);
	}

	[HarmonyPatch(typeof(MapInterface), "MapInterfaceUpdate")]
	static class MapInterfaceUpdate_Patch_Fertility
	{
		public static void Postfix()
		{
			if (Find.CurrentMap == null || WorldRendererUtility.WorldRenderedNow)
				return;

			if (!FertilityOverlay.fertilityOverlays.TryGetValue(Find.CurrentMap, out FertilityOverlay fertilityOverlay))
			{
				fertilityOverlay = new FertilityOverlay(Find.CurrentMap);
				FertilityOverlay.fertilityOverlays[Find.CurrentMap] = fertilityOverlay;
			}
			fertilityOverlay.Draw();
		}
	}

	[HarmonyPatch(typeof(TerrainGrid), "DoTerrainChangedEffects")]
	static class DoTerrainChangedEffects_Patch_Fertility
	{
		public static void Postfix(TerrainGrid __instance, Map ___map)
		{
			Map map = ___map;

			if (!FertilityOverlay.fertilityOverlays.TryGetValue(map, out FertilityOverlay fertilityOverlay))
			{
				fertilityOverlay = new FertilityOverlay(map);
				FertilityOverlay.fertilityOverlays[map] = fertilityOverlay;
			}
			fertilityOverlay.SetDirty();
		}
	}

	[HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
	[StaticConstructorOnStartup]
	public static class PlaySettings_Patch_Fertility
	{
		public static bool showFertilityOverlay;
		private static Texture2D icon = ContentFinder<Texture2D>.Get("CornPlantIcon", true);// or WallBricks_MenuIcon;

		[HarmonyPostfix]
		public static void AddButton(WidgetRow row, bool worldView)
		{
			if (!Settings.Get().showOverlayFertility) return;
			if (worldView) return;

			row.ToggleableIcon(ref showFertilityOverlay, icon, "TD.ToggleFertility".Translate());
		}
	}

}