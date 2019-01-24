using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace BurstMaster
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    ///

    // TODO: Save class file to XML file in Settings folder
    // TODO: Load Settings from XML folder.

    public partial class Window1
    {
        #region "SETTINGS CLASS"

        [Serializable]
        public class MySettings
        {
            // BASE NEEDED FOR CONFIRMATION
            public bool settingsSet
            {
                get;
                set;
            }

            // JOB ABILITIES SETTINGS

            // RED MAGE JOB ABILITIES
            public bool Composure
            {
                get;
                set;
            }

            public bool Convert
            {
                get;
                set;
            }

            public bool Spontaneity
            {
                get;
                set;
            }

            // BLACK MAGE JOB ABILITIES
            public bool ManaWall
            {
                get;
                set;
            }

            public bool Cascade
            {
                get;
                set;
            }

            public bool Manawell
            {
                get;
                set;
            }

            // SCHOLAR JOB ABILITIES
            public bool Sublimation
            {
                get;
                set;
            }

            public bool LightArts
            {
                get;
                set;
            }

            public bool AddendumWhite
            {
                get;
                set;
            }

            public bool DarkArts
            {
                get;
                set;
            }

            public bool AddendumBlack
            {
                get;
                set;
            }

            public bool MPReduction
            {
                get;
                set;
            }

            public bool CastingReduction
            {
                get;
                set;
            }

            public bool AreaOfEffect
            {
                get;
                set;
            }

            public bool PotencyIncrease
            {
                get;
                set;
            }

            // GEOMANCER JOB ABILITIES
            public bool FullCircle
            {
                get;
                set;
            }

            public bool LastingEmanation
            {
                get;
                set;
            }

            public bool EclipticAttrition
            {
                get;
                set;
            }

            public bool LifeCircle
            {
                get;
                set;
            }

            public bool BlazeOfGlory
            {
                get;
                set;
            }

            public bool Dematerialize
            {
                get;
                set;
            }

            public bool RadialArcana
            {
                get;
                set;
            }

            public bool Entrust
            {
                get;
                set;
            }

            // GEOMANCY MAGIC SETTINGS
            public int IndiSpell
            {
                get;
                set;
            }

            public int GeoSpell
            {
                get;
                set;
            }

            public int EntrustedSpell
            {
                get;
                set;
            }

            //DEFENSIVE MAGIC SETTINGS
            public bool ReraiseToggle
            {
                get;
                set;
            }

            public int ReraiseTier
            {
                get;
                set;
            }

            public bool ProtectToggle
            {
                get;
                set;
            }

            public int ProtectTier
            {
                get;
                set;
            }

            public bool ShellToggle
            {
                get;
                set;
            }

            public int ShellTier
            {
                get;
                set;
            }

            public bool AquaveilToggle
            {
                get;
                set;
            }

            public bool BlinkToggle
            {
                get;
                set;
            }

            public bool PhalanxToggle
            {
                get;
                set;
            }

            public bool StoneskinToggle
            {
                get;
                set;
            }

            public bool HasteToggle
            {
                get;
                set;
            }

            public int HasteTier
            {
                get;
                set;
            }

            public bool GainToggle
            {
                get;
                set;
            }

            public int GainTier
            {
                get;
                set;
            }

            public bool StormToggle
            {
                get;
                set;
            }

            public int StormTier
            {
                get;
                set;
            }

            public bool EnmityToggle
            {
                get;
                set;
            }

            public int EnmityTier
            {
                get;
                set;
            }

            public bool RegenToggle
            {
                get;
                set;
            }

            public int RegenTier
            {
                get;
                set;
            }

            public bool RefreshToggle
            {
                get;
                set;
            }

            public int RefreshTier
            {
                get;
                set;
            }

            public bool CureToggle
            {
                get;
                set;
            }

            public int CurePotency
            {
                get;
                set;
            }

            public bool Cure2Toggle
            {
                get;
                set;
            }

            public int Cure2Potency
            {
                get;
                set;
            }

            public bool Cure3Toggle
            {
                get;
                set;
            }

            public int Cure3Potency
            {
                get;
                set;
            }

            public bool Cure4Toggle
            {
                get;
                set;
            }

            public int Cure4Potency
            {
                get;
                set;
            }

            // OFFENSIVE MAGIC SETTINGS
            public int DarknessType
            {
                get;
                set;
            }

            public int LightType
            {
                get;
                set;
            }

            public int GravitationType
            {
                get;
                set;
            }

            public int DistortionType
            {
                get;
                set;
            }

            public int FusionType
            {
                get;
                set;
            }

            public int FragmentationType
            {
                get;
                set;
            }

            public bool AspirToggle
            {
                get;
                set;
            }

            public bool MyrkrToggle
            {
                get;
                set;
            }

            public bool DaganToggle
            {
                get;
                set;
            }

            // PROGRAM OPTIONS
            public bool GeomancerOnlyInCombat
            {
                get;
                set;
            }


        }

        #endregion "SETTINGS CLASS"

        #region "CREATE A CONFIG CLASS INSTANCE"

        public static MySettings config = new MySettings();

        #endregion "CREATE A CONFIG CLASS INSTANCE"

        #region "LOAD SETTINGS FUNCTION"

        private void LoadSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = " Extensible Markup Language (*.xml)|*.xml",
                FilterIndex = 2,
                InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings")
            };

            if (openFileDialog1.ShowDialog() == true)
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(MySettings));

                StreamReader reader = new StreamReader(openFileDialog1.FileName);
                config = (MySettings)mySerializer.Deserialize(reader);

                reader.Close();
                reader.Dispose();
                UpdateSettings();
                SaveSettingsClose_Click(sender, e);
            }
        }

        #endregion "LOAD SETTINGS FUNCTION"

        #region "SAVE SETTINGS BUTTON CLICK AND CLOSE"

        private void SaveSettingsClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // JOB ABILITIES SETTINGS

            // RED MAGE JOB ABILITIES
            config.Composure = Composure.IsChecked ?? false;
            config.Convert = Convert.IsChecked ?? false;
            config.Spontaneity = Spontaneity.IsChecked ?? false;

            // BLACK MAGE JOB ABILITIES
            config.ManaWall = ManaWall.IsChecked ?? false;
            config.Cascade = Cascade.IsChecked ?? false;
            config.Manawell = Manawell.IsChecked ?? false;

            // SCHOLAR JOB ABILITIES
            config.Sublimation = Sublimation.IsChecked ?? false;

            config.LightArts = LightArts.IsChecked ?? false;
            config.AddendumWhite = AddendumWhite.IsChecked ?? false;

            config.DarkArts = DarkArts.IsChecked ?? false;
            config.AddendumBlack = AddendumBlack.IsChecked ?? false;

            config.MPReduction = MPReduction.IsChecked ?? false;
            config.CastingReduction = CastingReduction.IsChecked ?? false;
            config.AreaOfEffect = AreaOfEffect.IsChecked ?? false;
            config.PotencyIncrease = PotencyIncrease.IsChecked ?? false;

            // GEOMANCER JOB ABILITIES
            config.FullCircle = FullCircle.IsChecked ?? false;
            config.LastingEmanation = LastingEmanation.IsChecked ?? false;
            config.EclipticAttrition = EclipticAttrition.IsChecked ?? false;
            config.LifeCircle = LifeCircle.IsChecked ?? false;
            config.BlazeOfGlory = BlazeOfGlory.IsChecked ?? false;
            config.Dematerialize = Dematerialize.IsChecked ?? false;
            config.RadialArcana = RadialArcana.IsChecked ?? false;
            config.Entrust = Entrust.IsChecked ?? false;

            // GEOMANCY MAGIC SETTINGS
            config.IndiSpell = IndiSpell.SelectedIndex;
            config.GeoSpell = GeoSpell.SelectedIndex;
            config.EntrustedSpell = EntrustedSpell.SelectedIndex;

            //DEFENSIVE MAGIC SETTINGS
            config.ReraiseToggle = ReraiseToggle.IsChecked ?? false;
            config.ReraiseTier = ReraiseTier.SelectedIndex;

            config.ProtectToggle = ProtectToggle.IsChecked ?? false;
            config.ProtectTier = ProtectTier.SelectedIndex;

            config.ShellToggle = ShellToggle.IsChecked ?? false;
            config.ShellTier = ShellTier.SelectedIndex;

            config.AquaveilToggle = AquaveilToggle.IsChecked ?? false;

            config.BlinkToggle = BlinkToggle.IsChecked ?? false;

            config.PhalanxToggle = PhalanxToggle.IsChecked ?? false;

            config.StoneskinToggle = StoneskinToggle.IsChecked ?? false;

            config.HasteToggle = HasteToggle.IsChecked ?? false;
            config.HasteTier = HasteTier.SelectedIndex;

            config.GainToggle = GainToggle.IsChecked ?? false;
            config.GainTier = GainTier.SelectedIndex;

            config.StormToggle = StormToggle.IsChecked ?? false;
            config.StormTier = StormTier.SelectedIndex;

            config.EnmityToggle = EnmityToggle.IsChecked ?? false;
            config.EnmityTier = EnmityTier.SelectedIndex;

            config.RegenToggle = RegenToggle.IsChecked ?? false;
            config.RegenTier = RegenTier.SelectedIndex;

            config.RefreshToggle = RefreshToggle.IsChecked ?? false;
            config.RefreshTier = RefreshTier.SelectedIndex;

            config.CureToggle = CureToggle.IsChecked ?? false;
            config.CurePotency = (int)CurePotency.Value;

            config.Cure2Toggle = Cure2Toggle.IsChecked ?? false;
            config.Cure2Potency = (int)Cure2Potency.Value;

            config.Cure3Toggle = Cure3Toggle.IsChecked ?? false;
            config.Cure3Potency = (int)Cure3Potency.Value;

            config.Cure4Toggle = Cure4Toggle.IsChecked ?? false;
            config.Cure4Potency = (int)Cure4Potency.Value;

            // OFFENSIVE MAGIC SETTINGS

            config.DarknessType = DarknessType.SelectedIndex;

            config.LightType = LightType.SelectedIndex;

            config.GravitationType = GravitationType.SelectedIndex;

            config.DistortionType = DistortionType.SelectedIndex;

            config.FusionType = FusionType.SelectedIndex;

            config.FragmentationType = FragmentationType.SelectedIndex;

            config.AspirToggle = AspirToggle.IsChecked ?? false;

            config.MyrkrToggle = MyrkrToggle.IsChecked ?? false;

            config.DaganToggle = DaganToggle.IsChecked ?? false;

            // PROGRAM SETTINGS

            config.GeomancerOnlyInCombat = GeomancerOnlyInCombat.IsChecked ?? false;

            config.settingsSet = true;

            this.Close();
        }

        #endregion "SAVE SETTINGS BUTTON CLICK AND CLOSE"

        #region "SAVE SETTINGS FILE FUNCTION"

        private void SaveSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();

            savefile.FileName = "Settings.xml";

            savefile.Filter = " Extensible Markup Language (*.xml)|*.xml";
            savefile.FilterIndex = 2;
            savefile.InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings");

            if (savefile.ShowDialog() == true)
            {
                SaveSettingsClose_Click(sender, e);

                XmlSerializer mySerializer = new XmlSerializer(typeof(MySettings));
                StreamWriter myWriter = new StreamWriter(savefile.FileName);
                mySerializer.Serialize(myWriter, config);
                myWriter.Close();
                myWriter.Dispose();
            }
        }

        #endregion "SAVE SETTINGS FILE FUNCTION"

        #region "UPDATE THE FORM WITH THE CURRENT CONFIG DATA FUNCTION"

        private void UpdateSettings()
        {
            // JOB ABILITIES SETTINGS

            // RED MAGE JOB ABILITIES
            Composure.IsChecked = config.Composure;
            Convert.IsChecked = config.Convert;
            Spontaneity.IsChecked = config.Spontaneity;

            // BLACK MAGE JOB ABILITIES
            ManaWall.IsChecked = config.ManaWall;
            Cascade.IsChecked = config.Cascade;
            Manawell.IsChecked = config.Manawell;

            // SCHOLAR JOB ABILITIES
            Sublimation.IsChecked = config.Sublimation;

            LightArts.IsChecked = config.LightArts;
            AddendumWhite.IsChecked = config.AddendumWhite;

            DarkArts.IsChecked = config.DarkArts;
            AddendumBlack.IsChecked = config.AddendumBlack;

            MPReduction.IsChecked = config.MPReduction;
            CastingReduction.IsChecked = config.CastingReduction;
            AreaOfEffect.IsChecked = config.AreaOfEffect;
            PotencyIncrease.IsChecked = config.PotencyIncrease;

            // GEOMANCER JOB ABILITIES
            FullCircle.IsChecked = config.FullCircle;
            LastingEmanation.IsChecked = config.LastingEmanation;
            EclipticAttrition.IsChecked = config.EclipticAttrition;
            LifeCircle.IsChecked = config.LifeCircle;
            BlazeOfGlory.IsChecked = config.BlazeOfGlory;
            Dematerialize.IsChecked = config.Dematerialize;
            RadialArcana.IsChecked = config.RadialArcana;
            Entrust.IsChecked = config.Entrust;

            // GEOMANCY MAGIC SETTINGS
            IndiSpell.SelectedIndex = config.IndiSpell;
            GeoSpell.SelectedIndex = config.GeoSpell;
            EntrustedSpell.SelectedIndex = config.EntrustedSpell;

            //DEFENSIVE MAGIC SETTINGS
            ReraiseToggle.IsChecked = config.ReraiseToggle;
            ReraiseTier.SelectedIndex = config.ReraiseTier;

            ProtectToggle.IsChecked = config.ProtectToggle;
            ProtectTier.SelectedIndex = config.ProtectTier;

            ShellToggle.IsChecked = config.ShellToggle;
            ShellTier.SelectedIndex = config.ShellTier;

            AquaveilToggle.IsChecked = config.AquaveilToggle;

            BlinkToggle.IsChecked = config.BlinkToggle;

            PhalanxToggle.IsChecked = config.PhalanxToggle;

            StoneskinToggle.IsChecked = config.StoneskinToggle;

            HasteToggle.IsChecked = config.HasteToggle;
            HasteTier.SelectedIndex = config.HasteTier;

            GainToggle.IsChecked = config.GainToggle;
            GainTier.SelectedIndex = config.GainTier;

            StormToggle.IsChecked = config.StormToggle;
            StormTier.SelectedIndex = config.StormTier;

            EnmityToggle.IsChecked = config.EnmityToggle;
            EnmityTier.SelectedIndex = config.EnmityTier;

            RegenToggle.IsChecked = config.RegenToggle;
            RegenTier.SelectedIndex = config.RegenTier;

            RefreshToggle.IsChecked = config.RefreshToggle;
            RefreshTier.SelectedIndex = config.RefreshTier;

            CureToggle.IsChecked = config.CureToggle;
            CurePotency.Value = config.CurePotency;

            Cure2Toggle.IsChecked = config.Cure2Toggle;
            Cure2Potency.Value = config.Cure2Potency;

            Cure3Toggle.IsChecked = config.Cure3Toggle;
            Cure3Potency.Value = config.Cure3Potency;

            Cure4Toggle.IsChecked = config.Cure4Toggle;
            Cure4Potency.Value = config.Cure4Potency;

            // OFFENSIVE MAGIC SETTINGS

            DarknessType.SelectedIndex = config.DarknessType;

            LightType.SelectedIndex = config.LightType;

            GravitationType.SelectedIndex = config.GravitationType;

            DistortionType.SelectedIndex = config.DistortionType;

            FusionType.SelectedIndex = config.FusionType;

            FragmentationType.SelectedIndex = config.FragmentationType;

            AspirToggle.IsChecked = config.AspirToggle;

            MyrkrToggle.IsChecked = config.MyrkrToggle;

            DaganToggle.IsChecked = config.DaganToggle;

            // PROGRAM OPTIONS

            GeomancerOnlyInCombat.IsChecked = config.GeomancerOnlyInCombat;

            config.settingsSet = true;
        }

        #endregion "UPDATE THE FORM WITH THE CURRENT CONFIG DATA FUNCTION"

        #region "MAIN WINDOW FUNCTION, WHEN IT'S INITIALIZED"

        public Window1()
        {
            #region "INITIALIZE THE WINDOW AND SET THE WINDOW TITLE"

            InitializeComponent();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            MainWindow2.Title = "BurstMaster" + " v" + version + " - Settings ";

            #endregion "INITIALIZE THE WINDOW AND SET THE WINDOW TITLE"

            #region "SET THE DEFAULT SETTINGS FOR THE BOT"

            if (config.settingsSet != true)
            {
                // JOB ABILITIES SETTINGS

                // RED MAGE JOB ABILITIES
                config.Composure = false;
                config.Convert = false;
                config.Spontaneity = false;

                // BLACK MAGE JOB ABILITIES
                config.ManaWall = false;
                config.Cascade = false;
                config.Manawell = false;

                // SCHOLAR JOB ABILITIES
                config.Sublimation = false;

                config.LightArts = false;
                config.AddendumWhite = false;

                config.DarkArts = false;
                config.AddendumBlack = false;

                config.MPReduction = false;
                config.CastingReduction = false;
                config.AreaOfEffect = false;
                config.PotencyIncrease = false;

                // GEOMANCER JOB ABILITIES
                config.FullCircle = false;
                config.LastingEmanation = false;
                config.EclipticAttrition = false;
                config.LifeCircle = false;
                config.BlazeOfGlory = false;
                config.Dematerialize = false;
                config.RadialArcana = false;
                config.Entrust = false;

                // GEOMANCY MAGIC SETTINGS
                config.IndiSpell = -1;
                config.GeoSpell = -1;
                config.EntrustedSpell = -1;

                //DEFENSIVE MAGIC SETTINGS
                config.ReraiseToggle = false;
                config.ReraiseTier = 0;

                config.ProtectToggle = false;
                config.ProtectTier = 0;

                config.ShellToggle = false;
                config.ShellTier = 0;

                config.AquaveilToggle = false;

                config.BlinkToggle = false;

                config.PhalanxToggle = false;

                config.StoneskinToggle = false;

                config.HasteToggle = false;
                config.HasteTier = 0;

                config.GainToggle = false;
                config.GainTier = 0;

                config.StormToggle = false;
                config.StormTier = 0;

                config.EnmityToggle = false;
                config.EnmityTier = 0;

                config.RegenToggle = false;
                config.RegenTier = 0;

                config.RefreshToggle = false;
                config.RefreshTier = 0;

                config.CureToggle = false;
                config.CurePotency = 0;

                config.Cure2Toggle = false;
                config.Cure2Potency = 0;

                config.Cure3Toggle = false;
                config.Cure3Potency = 0;

                config.Cure4Toggle = false;
                config.Cure4Potency = 0;

                // OFFENSIVE MAGIC SETTINGS

                config.DarknessType = 0;

                config.LightType = 0;

                config.GravitationType = 0;

                config.DistortionType = 0;

                config.FusionType = 0;

                config.FragmentationType = 0;

                config.AspirToggle = false;

                config.MyrkrToggle = false;

                config.DaganToggle = false;

                // PROGRAM OPTIONS

                config.GeomancerOnlyInCombat = false;

                config.settingsSet = true;
            }
            else
            {
                UpdateSettings();
            }

            #endregion "SET THE DEFAULT SETTINGS FOR THE BOT"
        }

        #endregion "MAIN WINDOW FUNCTION, WHEN IT'S INITIALIZED"

        #region "MOVE UP MOVE DOWN BUTTONS"

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            MoveItemDown(SpellOrder);
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            MoveItemUp(SpellOrder);
        }

        void MoveItemUp(ListBox myListBox)
        {
            if (myListBox.SelectedItem == null)
                return;

            if (myListBox.SelectedIndex == 0)
                return;

            var idx = myListBox.SelectedIndex;
            var elem = myListBox.SelectedItem;
            myListBox.Items.RemoveAt(idx);
            myListBox.Items.Insert(idx - 1, elem);
            myListBox.SelectedIndex = idx - 1;
        }

        void MoveItemDown(ListBox myListBox)
        {
            if (myListBox.SelectedItem == null)
                return;

            if (myListBox.SelectedIndex == 5)
                return;

            var idx = myListBox.SelectedIndex;
            var elem = myListBox.SelectedItem;
            myListBox.Items.RemoveAt(idx);
            myListBox.Items.Insert(idx + 1, elem);
            myListBox.SelectedIndex = idx + 1;
        }

        #endregion
    }
}