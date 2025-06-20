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
            //###### Example taken from Bradson's Plant Settings.
            //ls.CheckboxLabeled("Allow everything to be sown, including plants that aren't meant to regrow at all", ref settings.addSowTags, "Disabled by default. Respects the above settings in regards to wild plants and hydroponics. Generally bloats the sowing list quite heavily.");
            //###### Example taken from Bradson's Plant Settings.
            ls.Label("Example A: Checkboxes");
            ls.Gap(6f);
            ls.CheckboxLabeled("Option 1 description.", ref settings.Option1Enabled, "Tool tip description.");
            ls.CheckboxLabeled("Option 2 description.", ref settings.Option2Enabled); //Tooltips aren't required.
            ls.Gap(6f);
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Example B: Show / Hide Options on check");
            ls.Gap(6f);
            ls.CheckboxLabeled("Show More Options?", ref settings.Option3Enabled);
            if (settings.Option3Enabled)
            {
                ls.CheckboxLabeled("Option 4 description", ref settings.Option4Enabled);
            }
            else
            {
                settings.Option4Enabled = false; //if you cant see the option, it should be disabled.
            }
            ls.Gap(6f);
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Example C: Sliders");
            settings.Option5Value = (int)ls.SliderLabeled("Option 5 Value: " +settings.Option5Value+".", settings.Option5Value, 0f, 100f, 0.5f, "Tooltip");
            ls.Label("Slider Example with Toggle");
            ls.CheckboxLabeled("Enable Option 6", ref settings.Option6Enabled);
            if (settings.Option6Enabled)
            {
                settings.Option6Value /*The setting you want to change*/ = (int)ls.SliderLabeled("Option 6 Value: " + settings.Option6Value + "" /*The label name, followed by the setting*/, settings.Option6Value /*setting value again. Tells the slider what it's adjusting*/, 0f/*minimum slider value*/, 100f/*max slider value*/, 0.5f/*Do not touch. Anything aside from 0.5 breaks the slider. No idea why.*/, "Tooltip"/*The tooltip. Obviously.*/);
            }
            ls.Gap(6f);
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Example D: Only showing options if a mod is detected.");
            if (ModLister.HasActiveModWithName("Harmony")) //Replace Harmony with the you want to detects NAME. Not the mod.id (For some reason.)
            {
                ls.Label("Settings for mod (Insert name here)");
                ls.CheckboxLabeled("Enable Option 8", ref settings.Option8Enabled);
            }
            else
            {
                settings.Option8Enabled = false; //If the mod is not found, the setting is automatically disabled.
            }
            ls.Gap(6f);
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.End();
            Widgets.EndScrollView();
            base.DoSettingsWindowContents(inRect);
        }
    }

    public class XMLOptionsTemplateSettings : ModSettings
    {
        public bool Option1Enabled = false;
        public bool Option2Enabled = false;
        public bool Option3Enabled = false;
        public bool Option4Enabled = false;
        public bool Option5Enabled = false;
        public bool Option6Enabled = false;
        public bool Option7Enabled = false;
        public bool Option8Enabled = false;

        public bool OptionAEnabled = false;
        public bool OptionBEnabled = false;
        public bool OptionCEnabled = false;

        //Slider values

        public int Option5Value = 10;
        public int Option6Value = 10;
        public int Option7Value = 10;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref Option1Enabled /*ref (Setting name)*/, "Option1Enabled" /*setting label*/, false /*default value*/);
            Scribe_Values.Look(ref Option2Enabled, "Option2Enabled", false);
            Scribe_Values.Look(ref Option3Enabled, "Option3Enabled", false);
            Scribe_Values.Look(ref Option4Enabled, "Option4Enabled", false);
            Scribe_Values.Look(ref Option5Enabled, "Option5Enabled", false);
            Scribe_Values.Look(ref Option6Enabled, "Option6Enabled", false);
            Scribe_Values.Look(ref Option7Enabled, "Option7Enabled", false);
            Scribe_Values.Look(ref Option8Enabled, "Option8Enabled", false);
            Scribe_Values.Look(ref Option5Value, "Option5Value", 10);
            Scribe_Values.Look(ref Option6Value, "Option6Value", 10);
            Scribe_Values.Look(ref Option7Value, "Option7Value", 10);
            Scribe_Values.Look(ref OptionAEnabled, "OptionAEnabled", false);
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
