﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;
using HarmonyLib;
using UnityEngine;
using RimWorld;

namespace TD_Enhancement_Pack
{
	[StaticConstructorOnStartup]
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	public static class StopGizmo
	{
		private static Texture2D StopIcon = ContentFinder<Texture2D>.Get("Stop", true);

		//public override IEnumerable<Gizmo> GetGizmos()
		public static void Postfix(ref IEnumerable<Gizmo> __result, Pawn __instance)
		{
			if (__instance.Drafted ? !Mod.settings.showStopGizmoDrafted : !Mod.settings.showStopGizmo) return;

			if (RimWorld.Planet.WorldRendererUtility.WorldRenderedNow) return;

			if (!DebugSettings.godMode && !
				(__instance.IsColonistPlayerControlled && (__instance.CurJobDef?.playerInterruptible ?? true))) return;

			List<Gizmo> result = __result.ToList();
			
			result.Add(new Command_Action()
			{
				defaultLabel = "TD.StopGizmo".Translate(),
				icon = StopIcon,
				defaultDesc = (__instance.Drafted ? "TD.StopDescDrafted".Translate() : "TD.StopDescUndrafted".Translate()) + "\n\n" + "TD.AddedByTD".Translate(),
				action = delegate
				{
					foreach (Pawn pawn in Find.Selector.SelectedObjects.Where(o => o is Pawn).Cast<Pawn>())
					{
						pawn.jobs.StopAll(false);
					}
				},
				hotKey = KeyBindingDefOf.Designator_Deconstruct,
				Order = -30f
			});

			__result = result;
		}
	}
}
