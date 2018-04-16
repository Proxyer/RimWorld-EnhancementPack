﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using TD.Utilities;

namespace TD_Enhancement_Pack
{
	class Settings : ModSettings
	{
		public bool showOverlayBuildable = true;
		public bool showOverlayFertility = true;
		public bool showOverlayLighting = true;
		public bool cheatFertilityUnderGrid = true;

		public bool changeSpeedAfterTrader = true;
		public int afterTraderSpeed = 0;

		public bool skillArrows = true;
		public bool skillUpArrows = true;
		public bool skillDownArrows = true;

		public bool showZoneSize = true;

		public static Settings Get()
		{
			return LoadedModManager.GetMod<TD_Enhancement_Pack.Mod>().GetSettings<Settings>();
		}

		public void DoWindowContents(Rect wrect)
		{
			var options = new Listing_Standard();
			options.Begin(wrect);
			options.CheckboxLabeled("TD.SettingOverlayBuildable".Translate(), ref showOverlayBuildable, "TD.SettingOverlayBuildableDesc".Translate());
			options.CheckboxLabeled("TD.SettingOverlayFertility".Translate(), ref showOverlayFertility);
			bool before = cheatFertilityUnderGrid;
			options.CheckboxLabeled("TD.SettingOverlayFertilityUnder".Translate(), ref cheatFertilityUnderGrid);
			if (before != cheatFertilityUnderGrid)
				FertilityOverlay.DirtyAll();
			options.CheckboxLabeled("TD.SettingOverlayLighting".Translate(), ref showOverlayLighting, "TD.SettingOverlayLightingDesc".Translate());
			options.Gap();

			options.CheckboxLabeled("TD.SettingTradeClose".Translate(), ref changeSpeedAfterTrader);
			options.SliderLabeled("TD.SettingTradeCloseSpeed".Translate(), ref afterTraderSpeed, "{0}x", 0, 4);
			options.Gap();

			options.CheckboxLabeled("TD.SettingSkillGainArrow".Translate(), ref skillUpArrows, "TD.SettingSkillGainArrowDesc".Translate());
			options.CheckboxLabeled("TD.SettingSkillLossArrow".Translate(), ref skillDownArrows, "TD.SettingSkillLossArrowDesc".Translate());
			skillArrows = skillUpArrows || skillDownArrows;
			options.Gap();

			options.CheckboxLabeled("TD.SettingZoneSize".Translate(), ref showZoneSize);
			options.Gap();

			options.Label("TD.OtherFeatures".Translate());
			options.Label("TD.AreaEditing".Translate());
			options.Label("TD.NeverHome".Translate());
			options.Label("TD.VariousCount".Translate());
			options.Label("TD.RottedAwayClickable".Translate());
			options.Label("TD.PopupMessageRealtime".Translate());
			options.Label("TD.CameraPanningFixes".Translate());

			options.End();
		}

		public override void ExposeData()
		{
			Scribe_Values.Look(ref showOverlayBuildable, "showOverlayBuildable", true);
			Scribe_Values.Look(ref showOverlayFertility, "showOverlayFertility", true);
			Scribe_Values.Look(ref showOverlayLighting, "showOverlayLighting", true);
			Scribe_Values.Look(ref cheatFertilityUnderGrid, "cheatFertilityUnderGrid", true);

			Scribe_Values.Look(ref changeSpeedAfterTrader, "changeSpeedAfterTrader", true);
			Scribe_Values.Look(ref afterTraderSpeed, "afterTraderSpeed", 0);

			Scribe_Values.Look(ref skillArrows, "skillArrows", true);
			Scribe_Values.Look(ref skillUpArrows, "skillUpArrows", true);
			Scribe_Values.Look(ref skillDownArrows, "skillDownArrows", true);

			Scribe_Values.Look(ref showZoneSize, "showZoneSize", true);
		}
	}
}