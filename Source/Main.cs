using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;
using System.Xml;
using System.Configuration;

// *Uncomment for Harmony*
// using System.Reflection;
// using HarmonyLib;

namespace XMLOptionsTemplate
{
    public class XMLOptionsTemplateMod : Mod
    {
        public static XMLOptionsTemplateMod mod;
        public static XMLOptionsTemplateSettings settings;

        public XMLOptionsTemplateMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<XMLOptionsTemplateSettings>();
            mod = this;
        }
        public override string SettingsCategory()
        {
            return "XMLOptionsTemplate";
        }
        private static UnityEngine.Vector2 scrollPosition = UnityEngine.Vector2.zero;
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect scrollViewRect = inRect.AtZero();
            scrollViewRect.width -= 30f;
            scrollViewRect.height = 3000f;

            Widgets.BeginScrollView(inRect, ref scrollPosition, scrollViewRect);
            Listing_Standard ls = new Listing_Standard
            {
                ColumnWidth = scrollViewRect.width - 30f
            };
            ls.Begin(inRect);
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " +settings.OptionBValue+".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            
            ls.End();
            Widgets.EndScrollView();
            base.DoSettingsWindowContents(inRect);
        }
    }

    public class XMLOptionsTemplateSettings : ModSettings
    {
        public bool OptionAEnabled = false;
        public bool OptionBEnabled = false;
        public int OptionBValue = 24;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref OptionAEnabled, "OptionAEnabled", false);
            Scribe_Values.Look(ref OptionBEnabled, "OptionBEnabled", false);
            Scribe_Values.Look(ref OptionBValue, "OptionBValue", 24);
        }

        public IEnumerable<string> GetEnabledSettings
        {
            get
            {
                return GetType().GetFields().Where(p => p.FieldType == typeof(bool) && (bool)p.GetValue(this)).Select(p => p.Name);
            }

        }
    }

    public class PatchOperation_SettingsActive : PatchOperation
    {
        private List<string> settings;
        private PatchOperation active;
        private PatchOperation inactive;
        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool flag = false;
            for (int i = 0; i < settings.Count(); i++)
            {
                if (XMLOptionsTemplateMod.settings.GetEnabledSettings.Contains(settings[i]))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                if (active != null)
                {
                    return active.Apply(xml);
                }
            }
            else if (inactive != null)
            {
                return inactive.Apply(xml);
            }
            return true;
        }
    }
}
