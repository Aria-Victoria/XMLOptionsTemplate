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
        private static float height_modifier = 3000f;
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect outerRect = new Rect(0f, 30f, inRect.width, inRect.height - 30);
            Rect viewRect = new Rect(0f, 0f, inRect.width - 60f, inRect.height + height_modifier);
            Widgets.BeginScrollView(outerRect, ref scrollPosition, viewRect);
            Listing_Standard ls = new Listing_Standard();
            ls.maxOneColumn = true;
            ls.Begin(viewRect);
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            //
            ls.Gap(6f);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            if (settings.OptionBEnabled) //Only shows OptionBValues if OptionB is enabled
            {
                settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            }
            else
            {
                settings.OptionBValue = 24; //Returns the value to the default. Doesn't really matter as the patch is never applied.
            }
            //
                ls.Gap(6f);
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            if (settings.OptionCEnabled)
            {
                settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            }
            else
            {
                settings.OptionCValue = 18; //Returns the value to the default. Doesn't really matter as the patch is never applied.
            }
            //
                ls.Gap(6f);
            if (ModLister.HasActiveModWithName("Harmony"))
            {
                ls.Label("You should only see this if Harmony is loaded.");
                ls.CheckboxLabeled("Renames wood to bark.", ref settings.OptionDEnabled);
            }
            else
            {
                settings.OptionDEnabled = false; //If Harmony (or whatever mod you replace harmony with) isn't loaded, this option will ALWAYS be false, meaning the patch WILL NEVER apply.
            }
            ls.End();
            Widgets.EndScrollView();
            /*

            ######################################### UI LAYOUT EXAMPLES

            ##### UI Example 1 ##### 
            Single column with scroll bar. (This is what the template has active by default.)

            Rect outerRect = new Rect(0f, 30f, inRect.width, inRect.height - 30);
            Rect viewRect = new Rect(0f, 0f, inRect.width - 60f, inRect.height + height_modifier);
            Widgets.BeginScrollView(outerRect, ref scrollPosition, viewRect);
            Listing_Standard ls = new Listing_Standard();
            ls.maxOneColumn = true;
            ls.Begin(viewRect);
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            //
            ls.Gap(6f);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            if (settings.OptionBEnabled)
            {
                settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            }
            else
            {
                settings.OptionBValue = 24; //Returns the value to the default. Doesn't really matter as the patch is never applied.
            }
            //
                ls.Gap(6f);
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            if (settings.OptionCEnabled)
            {
                settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            }
            else
            {
                settings.OptionCValue = 18; //Returns the value to the default. Doesn't really matter as the patch is never applied.
            }
            //
                ls.Gap(6f);
            if (ModLister.HasActiveModWithName("Harmony"))
            {
                ls.Label("You should only see this if Harmony is loaded.");
                ls.CheckboxLabeled("Renames wood to bark.", ref settings.OptionDEnabled);
            }
            else
            {
                settings.OptionDEnabled = false; //If Harmony (or whatever mod you replace harmony with) isn't loaded, this option will ALWAYS be false, meaning the patch WILL NEVER apply.
            }
            ls.End();
            Widgets.EndScrollView();

            ##### UI Example 2 #####
            Start with a single column then switch into two columns. 
            
            This approch has serious issues as options that show / hide more options can (and will) screw up the way the menu looks in game.
            This approch also requires you to figure out exactly how high the single column is and adjust the gap after creating the second column.
            
            Rect outerRect = new Rect(0f, 30f, inRect.width, inRect.height - 30);
            Rect viewRect = new Rect(0f, 0f, inRect.width - 60f, inRect.height + height_modifier);
            Widgets.BeginScrollView(outerRect, ref scrollPosition, viewRect);
            Listing_Standard ls = new Listing_Standard();
            ls.maxOneColumn = true;
            ls.Begin(viewRect);
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.ColumnWidth = (viewRect.width / 2.05f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.NewColumn();
            ls.Gap(200f); //You will need to figure out how high you need it to be exacl
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.End();
            Widgets.EndScrollView();            
            
            ##### UI Example 3 #####
            Starts with duel columns then switches to a single column, before switching back to duel columns.

            This approch also has issues.

            Rect outerRect = new Rect(0f, 30f, inRect.width, inRect.height - 30);
            Rect viewRect = new Rect(0f, 0f, inRect.width - 60f, inRect.height + height_modifier);
            Widgets.BeginScrollView(outerRect, ref scrollPosition, viewRect);
            Listing_Standard ls = new Listing_Standard();
            ls.maxOneColumn = true;
            ls.ColumnWidth = (viewRect.width / 2.05f);
            ls.Begin(viewRect);
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            //#############################################################################################################################
            //############################# Split into single coloumn
            //#############################################################################################################################
            ls.ColumnWidth = (viewRect.width);
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            //#############################################################################################################################
            //############################# return to duel column and move to a new column
            //#############################################################################################################################
            ls.ColumnWidth = (viewRect.width / 2.05f);
            ls.NewColumn();
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.Label("A restart is required to apply any changed settings."); //XML Patches are only applied upon loading the game.
            ls.GapLine();
            ls.Gap(6f);
            ls.Label("Actual Patch Examples.");
            ls.CheckboxLabeled("Rename Luciferium to Chile Powder.", ref settings.OptionAEnabled);
            ls.CheckboxLabeled("Change packed meal value.", ref settings.OptionBEnabled);
            settings.OptionBValue = (int)ls.SliderLabeled("Option B Value: " + settings.OptionBValue + ".", settings.OptionBValue, 0f, 100f, 0.5f, "Tooltip");
            ls.CheckboxLabeled("Change industrial medicine value.", ref settings.OptionCEnabled);
            settings.OptionCValue = (int)ls.SliderLabeled("Option C Value: " + settings.OptionCValue + ".", settings.OptionCValue, 0f, 10000f, 0.5f, "Tooltip");
            ls.End();
            Widgets.EndScrollView();
            
            
            */
        }
    }

    public class XMLOptionsTemplateSettings : ModSettings
    {
        public bool OptionAEnabled = false;
        public bool OptionBEnabled = false;
        public int OptionBValue = 24;
        public bool OptionCEnabled = false;
        public int OptionCValue = 2324;
        public bool OptionDEnabled = false;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref OptionAEnabled, "OptionAEnabled", false);
            Scribe_Values.Look(ref OptionBEnabled, "OptionBEnabled", false);
            Scribe_Values.Look(ref OptionBValue, "OptionBValue", 24);
            Scribe_Values.Look(ref OptionCEnabled, "OptionCEnabled", false);
            Scribe_Values.Look(ref OptionCValue, "OptionCValue", 2324);
            Scribe_Values.Look(ref OptionDEnabled, "OptionDEnabled", false);
        }

        public IEnumerable<string> GetEnabledSettings
        {
            get
            {
                return GetType().GetFields().Where(p => p.FieldType == typeof(bool) && (bool)p.GetValue(this)).Select(p => p.Name);
            }

        }
    }

    public class PatchOperation_Mod_SettingActive : PatchOperation
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
    public class PatchOperation_Mod_ReplaceWithSetting : PatchOperationPathed
    {
        private string settingValue;
        private XmlContainer value;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode node = value.node;
            bool result = false;
            XmlNode[] array = xml.SelectNodes(xpath).Cast<XmlNode>().ToArray();
            foreach (XmlNode xmlNode in array)
            {
                result = true;
                XmlNode parentNode = xmlNode.ParentNode;
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.InnerText.Contains("{VALUE}"))
                    {
                        childNode.InnerText = childNode.InnerText.Replace("{VALUE}", XMLOptionsTemplateMod.settings.GetType().GetField(settingValue).GetValue(XMLOptionsTemplateMod.settings).ToString());
                    }
                    parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(childNode, deep: true), xmlNode);
                }

                parentNode.RemoveChild(xmlNode);
            }

            return result;
        }
    }
}
