using EliteMMO.API;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BurstMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region " CUSTOM LIST CLASS : SkillchainData "

        public class SkillchainData : List<SkillchainData>
        {
            public string Skillchain_name
            {
                get; set;
            }

            public List<string> Burst_elements
            {
                get; set;
            }
        }

        #endregion

        #region " CUSTOM LIST CLASS : StatusBuffsActive "

        public class StatusBuffsActive
        {
            public String Source { get; set; }
        }

        #endregion

        #region " PUBLIC METHODS "

        public ListBox processids = new ListBox();

        public static EliteAPI api;

        public int firstSelect = 0;

        private static BackgroundWorker backgroundWorker;
        private static BackgroundWorker ChatLogReader;

        public List<SkillchainData> skillchains = new List<SkillchainData>();

        public string WindowerMode = "Windower";

        private int lastCommand = 0;

        private bool BurstPossible = false;
        private bool InCombat = false;
        private bool IsRunning = false;
        private bool CanCast = true;
        private string skillchainLocated = String.Empty;
        private DateTime timeDated;

        private bool CombatStatus = false;

        private DispatcherTimer StatusEffectsTimer = new DispatcherTimer();
        private DispatcherTimer BasicStatsTimer = new DispatcherTimer();
        private DispatcherTimer SkillchainBurstTimer = new DispatcherTimer();
        private DispatcherTimer PerformBuffingTimer = new DispatcherTimer();
        private DispatcherTimer CastLockTimer = new DispatcherTimer();
        private DispatcherTimer CombatCheckerTimer = new DispatcherTimer();

        private DispatcherTimer _timer;
        private TimeSpan _time;

        #endregion

        #region "MainWindow Initialization ETC"

        public MainWindow()
        {
            #region "Initialize Window and set Version"

            InitializeComponent();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            MainWindow1.Title = "BurstMaster" + " v" + version;

            Window1 mainWindow = new Window1();

            #endregion

            #region " Generate SKILLCHAIN List "

            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Radiance",
                Burst_elements = new List<string>
                {
                    "Light", "Fire", "Thunder", "Wind"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Umbra",
                Burst_elements = new List<string>
                {
                    "Dark", "Earth", "Water", "Ice"
                }
            });

            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Light",
                Burst_elements = new List<string>
                {
                    "Light", "Fire", "Thunder", "Wind"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Dark",
                Burst_elements = new List<string>
                {
                    "Dark", "Earth", "Water", "Ice"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Gravitation",
                Burst_elements = new List<string>
                {
                    "Dark", "Earth"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Fragmentation",
                Burst_elements = new List<string>
                {
                    "Thunder", "Wind"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Distortion",
                Burst_elements = new List<string>
                {
                    "Water", "Ice"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Fusion",
                Burst_elements = new List<string>
                {
                    "Fire", "Light"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Compression",
                Burst_elements = new List<string>
                {
                    "Dark"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Liquefaction",
                Burst_elements = new List<string>
                {
                    "Fire"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Induration",
                Burst_elements = new List<string>
                {
                    "Ice"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Reverberation",
                Burst_elements = new List<string>
                {
                    "Water"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Transfixion",
                Burst_elements = new List<string>
                {
                    "Light"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Scission",
                Burst_elements = new List<string>
                {
                    "Earth"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Detonation",
                Burst_elements = new List<string>
                {
                    "Wind"
                }
            });
            skillchains.Add(new SkillchainData
            {
                Skillchain_name = "Impaction",
                Burst_elements = new List<string>
                {
                    "Thunder"
                }
            });

            #endregion

            #region "All Background Timer starters"

            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            ChatLogReader = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            BasicStatsTimer.Tick += new EventHandler(BasicStatsTimer_Tick);
            BasicStatsTimer.Interval = TimeSpan.FromSeconds(0.5);
            BasicStatsTimer.Start();

            StatusEffectsTimer.Tick += new EventHandler(StatusEffectsTimer_Tick);
            StatusEffectsTimer.Interval = TimeSpan.FromSeconds(2.0);
            StatusEffectsTimer.Start();

            SkillchainBurstTimer.Tick += new EventHandler(SkillchainBurstTimer_Tick);
            SkillchainBurstTimer.Interval = TimeSpan.FromSeconds(0.5);
            SkillchainBurstTimer.Start();

            PerformBuffingTimer.Tick += new EventHandler(PerformBuffingTimer_Tick);
            PerformBuffingTimer.Interval = TimeSpan.FromSeconds(0.5);
            PerformBuffingTimer.Start();

            CombatCheckerTimer.Tick += new EventHandler(CombatCheckerTimer_Tick);
            CombatCheckerTimer.Interval = TimeSpan.FromSeconds(2.0);
            CombatCheckerTimer.Start();

            CastLockTimer.Tick += new EventHandler(CastLockTimer_TickAsync);
            CastLockTimer.Interval = TimeSpan.FromSeconds(0.5);


            #endregion

            #region "Check for the Elite API and POL instances"

            if (File.Exists("eliteapi.dll") && File.Exists("elitemmo.api.dll"))
            {
                var pol = Process.GetProcessesByName("pol");

                if (pol.Length < 1)
                {
                    ErrorDialog("Notice", "No POL instances were able to be located." +
                            "\n\n" +
                            "Please note: If you use a private server make sure the program used to access it has been renamed to POL " +
                            "otherwise this bot will not be able to locate it.");
                }
                else
                {
                    for (var i = 0; i < pol.Length; i++)
                    {
                        this.POLID.Items.Add(pol[i].MainWindowTitle);
                        this.processids.Items.Add(pol[i].Id);
                    }
                    this.POLID.SelectedIndex = 0;
                    this.processids.SelectedIndex = 0;
                }
            }
            else
            {
                MessageBox.Show("This program can not function without EliteMMO.API.dll and EliteAPI.dll");
                Application.Current.Shutdown();
            }

            #endregion
        }

        #endregion

        #region "A POLID was selected so create an API instance and run the Addon"

        private void SelectPOLIDButton_Click(object sender, RoutedEventArgs e)
        {
            this.processids.SelectedIndex = this.POLID.SelectedIndex;
            api = new EliteAPI((int)this.processids.SelectedItem);
            CharacterName.Content = api.Player.Name;
            this.SelectPOLID.Content = "SELECTED";

            foreach (var dats in Process.GetProcessesByName("pol").Where(dats => POLID.Text == dats.MainWindowTitle))
            {
                for (int i = 0; i < dats.Modules.Count; i++)
                {
                    if (dats.Modules[i].FileName.Contains("Ashita.dll"))
                    {
                        WindowerMode = "Ashita";
                    }
                    else if (dats.Modules[i].FileName.Contains("Hook.dll"))
                    {
                        WindowerMode = "Windower";
                    }
                }
            }

            if (firstSelect == 0)
            {

                EliteAPI.ChatEntry cl = api.Chat.GetNextChatLine();
                while (cl != null) cl = api.Chat.GetNextChatLine();

                backgroundWorker.RunWorkerAsync();

                if (WindowerMode == "Windower")
                {
                    api.ThirdParty.SendString("//lua load BurstMaster_addon");
                }
                else if (WindowerMode == "Ashita")
                {
                    api.ThirdParty.SendString("/addon load BurstMaster_addon");
                }

                lastCommand = api.ThirdParty.ConsoleIsNewCommand();

                firstSelect = 1;
            }
        }

        #endregion "A POLID was selected so create an API instance and run the Addon"

        #region "Check if BuffActive: IsBuffActive(#BuffID, #SpellType)"

        public bool IsBuffActive(int buffID, string spellType)
        {
            if (spellType != "None")
            {
                if (spellType == "Gain")
                {
                    // grab tier
                    int Tier = Window1.config.ReraiseTier;
                    // known tiers
                    var KnownTiers = new List<int> { 119, 120, 125, 122, 121, 123, 124 };
                    // Return the data requested
                    if (api.Player.GetPlayerInfo().Buffs.Any(b => b == KnownTiers[Tier]))
                        return true;
                    else
                        return false;
                }
                else if (spellType == "Animus")
                {
                    // grab tier
                    int Tier = Window1.config.EnmityTier;
                    // known tiers
                    var KnownTiers = new List<int> { 289, 291 };
                    // Return the data requested
                    if (api.Player.GetPlayerInfo().Buffs.Any(b => b == KnownTiers[Tier]))
                        return true;
                    else
                        return false;
                }
                else if (spellType == "Storm")
                {
                    // grab tier
                    int Tier = Window1.config.ReraiseTier;
                    // known tiers
                    var KnownTiers = new List<int> { 178, 181, 183, 180, 179, 182, 184, 185, 589, 592, 594, 591, 590, 593, 595, 596, };
                    // Return the data requested
                    if (api.Player.GetPlayerInfo().Buffs.Any(b => b == KnownTiers[Tier]))
                        return true;
                    else
                        return false;
                }
                return false;
            }
            else
            {
                if (api.Player.GetPlayerInfo().Buffs.Any(b => b == buffID))
                    return true;
                else
                    return false;
            }
        }

        #endregion "Check if BuffActive: IsBuffActive(#BuffID)"

        #region "Check all requirements to cast a spell / Job Ability"

        public bool CanUseJA(string JobAbilityName)
        {
            if (api.Player.GetPlayerInfo().Buffs.Any(b => b == 261)) // IF YOU HAVE INPAIRMENT THEN BLOCK JOB ABILITY CASTING
            {
                return false;
            }
            else if (api.Player.HasAbility(api.Resources.GetAbility(JobAbilityName, 0).ID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool CanCastSpell(string spell)
        {
            string SpellName = String.Empty;

            // FIRST GET THE SPELL NAME IF NOT SPECIFIED
            if (spell == "Reraise")
            {
                // grab tier
                int Tier = Window1.config.ReraiseTier;
                // known tiers
                var KnownTiers = new List<string> { "Reraise", "Reraise II", "Reraise III" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else if (spell == "Protect")
            {
                // grab tier
                int Tier = Window1.config.ProtectTier;
                // known tiers
                var KnownTiers = new List<string> { "Protect", "Protect II", "Protect III", "Protect IV", "Protect V" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else if (spell == "Shell")
            {
                // grab tier
                int Tier = Window1.config.ShellTier;
                // known tiers
                var KnownTiers = new List<string> { "Shell", "Shell II", "Shell III", "Shell IV", "Shell V" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else if (spell == "Refresh")
            {
                // grab tier
                int Tier = Window1.config.RefreshTier;
                // known tiers
                var KnownTiers = new List<string> { "Refresh", "Refresh II", "Refresh III" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else if (spell == "Regen")
            {
                // grab tier
                int Tier = Window1.config.RegenTier;
                // known tiers
                var KnownTiers = new List<string> { "Regen", "Regen II", "Regen III", "Regen IV", "Regen V" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else if (spell == "Haste")
            {
                // grab tier
                int Tier = Window1.config.HasteTier;
                // known tiers
                var KnownTiers = new List<string> { "Haste", "Haste II" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else if (spell == "Gain")
            {
                // grab tier
                int Tier = Window1.config.GainTier;
                // known tiers
                var KnownTiers = new List<string> { "Gain-STR", "Gain-DEX", "Gain-CHR", "Gain-AGI", "Gain-VIT", "Gain-INT", "Gain-MND" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];

            }
            else if (spell == "Storm")
            {
                // grab tier
                int Tier = Window1.config.StormTier;
                // known tiers
                var KnownTiers = new List<string> { "Firestorm", "Sandstorm", "Rainstorm", "Windstorm", "Hailstorm", "Thunderstorm", "Aurorastorm", "Voidstorm",
                                                        "Firestorm II", "Sandstorm II", "Rainstorm II", "Windstorm II", "Hailstorm II", "Thunderstorm II", "Aurorastorm II", "Voidstorm II", };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else if (spell == "Animus")
            {
                // grab tier
                int Tier = Window1.config.HasteTier;
                // known tiers
                var KnownTiers = new List<string> { "Animus Augeo", "Animus Minuo" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else if (spell == "Indi")
            {
                // grab tier
                int Tier = Window1.config.IndiSpell;
                // known tiers
                var KnownTiers = new List<string> {"Indi-Voidance", "Indi-Precision", "Indi-Regen", "Indi-Haste", "Indi-Attunement", "Indi-Focus", "Indi-Barrier",
                                                       "Indi-Refresh", "Indi-CHR", "Indi-MND", "Indi-Fury", "Indi-INT", "Indi-AGI", "Indi-Fend", "Indi-VIT", "Indi-DEX",
                                                       "Indi-Acumen", "Indi-STR", "Indi-Poison", "Indi-Slow", "Indi-Torpor", "Indi-Slip","Indi-Languor", "Indi-Paralysis",
                                                       "Indi-Vex","Indi-Frailty","Indi-Wilt","Indi-Malaise", "Indi-Gravity", "Indi-Fade" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            } 
            else if (spell == "Geo")
            {
                // grab tier
                int Tier = Window1.config.GeoSpell;
                // known tiers
                var KnownTiers = new List<string> {"Geo-Voidance", "Geo-Precision", "Geo-Regen", "Geo-Haste", "Geo-Attunement", "Geo-Focus", "Geo-Barrier",
                                                       "Geo-Refresh", "Geo-CHR", "Geo-MND", "Geo-Fury", "Geo-INT", "Geo-AGI", "Geo-Fend", "Geo-VIT", "Geo-DEX",
                                                       "Geo-Acumen", "Geo-STR", "Geo-Poison", "Geo-Slow", "Geo-Torpor", "Geo-Slip","Geo-Languor", "Geo-Paralysis",
                                                       "Geo-Vex","Geo-Frailty","Geo-Wilt","Geo-Malaise", "Geo-Gravity", "Geo-Fade" };
                // Generated Spell Name
                SpellName = KnownTiers[Tier];
            }
            else
                SpellName = spell;

            // ONCE THE SPELL NAME IS KNOWN CONTINUE ON OTHERWISE RETURN FALSE
            if (SpellName != String.Empty)
            {
                var magic = api.Resources.GetSpell(SpellName, 0);

                int mainjobLevelRequired = magic.LevelRequired[(api.Player.MainJob)];
                int subjobLevelRequired = magic.LevelRequired[(api.Player.SubJob)];

                var JobPoints = api.Player.GetJobPoints(api.Player.MainJob);

                // FIRST AND FOREMOST CHECK YOU HAVE THE MP TO EVEN CAST THE SPELL
                if (api.Player.MP >= magic.MPCost)
                {
                    // CHECK IF YOU'RE THE REQUIRED LEVEL TO CAST ON EITHER MAIN JOB OR SUB JOB
                    if ((mainjobLevelRequired <= api.Player.MainJobLevel && mainjobLevelRequired != -1) || (subjobLevelRequired <= api.Player.SubJobLevel && subjobLevelRequired != -1))
                    {
                        // IF THE SPELL IS A JOB POINT SPELL THEN CHECK THAT AS WELL
                        if (SpellName == "Refresh III")
                        {
                            if (api.Player.MainJob == 5 && api.Player.MainJobLevel == 99 && JobPoints.SpentJobPoints >= 1200) // YOU HAVE ACCESS TO REFRESH III
                            {
                                if (api.Recast.GetSpellRecast(magic.Index) == 0)
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else if (SpellName.Contains("storm II"))
                        {
                            if (api.Player.MainJob == 20 && api.Player.MainJobLevel == 99 && JobPoints.SpentJobPoints >= 100) // YOU HAVE ACCESS TO TIER II STORM SPELLS
                            {
                                if (api.Recast.GetSpellRecast(magic.Index) == 0)
                                    return true;
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            if (api.Recast.GetSpellRecast(magic.Index) == 0)
                                return true;
                            else
                                return false;
                        }
                    }
                    return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private bool GeoSpell_CombatRequirement(int Tier)
        {
            // known tiers
            var KnownTiers = new List<string> {"Geo-Voidance", "Geo-Precision", "Geo-Regen", "Geo-Haste", "Geo-Attunement", "Geo-Focus", "Geo-Barrier",
                                                       "Geo-Refresh", "Geo-CHR", "Geo-MND", "Geo-Fury", "Geo-INT", "Geo-AGI", "Geo-Fend", "Geo-VIT", "Geo-DEX",
                                                       "Geo-Acumen", "Geo-STR", "Geo-Poison", "Geo-Slow", "Geo-Torpor", "Geo-Slip","Geo-Languor", "Geo-Paralysis",
                                                       "Geo-Vex","Geo-Frailty","Geo-Wilt","Geo-Malaise", "Geo-Gravity", "Geo-Fade" };

            // Grab spell 
            string SpellName = KnownTiers[Tier];

            if (api.Resources.GetSpell(SpellName, 0).ValidTargets == 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        #endregion

        #region "Casting Lock"

        private void CastLockMethod()
        {
            CastLockTimer.Start();
        }

        private async void JALockMethodAsync()
        {
            await Task.Delay(1000);
        }

        private async void CastLockTimer_TickAsync(object sender, EventArgs e)
        {

            var count = 0;
            float lastPercent = 0;

            while (api.CastBar.Percent <= 1)
            {
                await Task.Delay(250);

                if (lastPercent != api.CastBar.Percent)
                {
                    count = 0;
                    lastPercent = api.CastBar.Percent;
                }
                else if (count == 10)
                {

                    break;
                }
                else
                {
                    count++;
                    lastPercent = api.CastBar.Percent;
                }
            }

            this.CastingPossible.Content = "Cast Timer reset.";
            CanCast = true;

        }
        #endregion

        #region "Cast a Spell / Job Ability"

        private void CastSpell(string spellName, string targetName)
        {

            api.ThirdParty.SendString("/ma \"" + spellName + "\" " + targetName);
            this.ActionBeingPerformed.Content = "Casting: " + spellName;
            this.CanCast = false;
            CastLockMethod();
        }

        private void UseJA(string JobAbilityName)
        {
            api.ThirdParty.SendString("/ja \"" + JobAbilityName + "\" <me>");
            this.ActionBeingPerformed.Content = "Using Ability: " + JobAbilityName;
            JALockMethodAsync();
        }

        #endregion

        #region "Perform buffing"

        private void PerformBuffingTimer_Tick(object sender, EventArgs e)
        {
            if (BurstPossible == false && IsRunning == true && CanCast == true && (api.Player.Status == 1 || api.Player.Status == 0))
            {
                // CURING
                if ((api.Player.HP <= api.Player.HPMax - Window1.config.Cure4Potency) && CanCastSpell("Cure IV") && Window1.config.Cure4Toggle)
                {
                    CastSpell("Cure IV", "<me>");
                }
                else if ((api.Player.HP <= api.Player.HPMax - Window1.config.Cure3Potency) && CanCastSpell("Cure III") && Window1.config.Cure3Toggle)
                {
                    CastSpell("Cure III", "<me>");
                }
                else if ((api.Player.HP <= api.Player.HPMax - Window1.config.Cure2Potency) && CanCastSpell("Cure II") && Window1.config.Cure2Toggle)
                {
                    CastSpell("Cure II", "<me>");
                }
                else if ((api.Player.HP <= api.Player.HPMax - Window1.config.CurePotency) && CanCastSpell("Cure") && Window1.config.CureToggle)
                {
                    CastSpell("Cure", "<me>");
                }
                // JOB ABILITIES THAT IMPROVE SPELLS
                // COMPOSURE
                else if (IsBuffActive(419, "None") == false && Window1.config.Composure == true && CanUseJA("Composure"))
                {
                    UseJA("Composure");
                }
                // LIGHT ARTS
                else if (IsBuffActive(358, "None") == false && IsBuffActive(401, "None") && Window1.config.LightArts == true && CanUseJA("Light Arts"))
                {
                    UseJA("Light Arts");
                }
                // ADDENDUM: WHITE
                else if (IsBuffActive(401, "None") == false && IsBuffActive(358, "None") == true && Window1.config.AddendumWhite == true && CanUseJA("Addendum: White"))
                {
                    UseJA("Addendum: White");
                }
                // RERAISE
                else if (IsBuffActive(113, "None") == false && Window1.config.ReraiseToggle == true && CanCastSpell("Reraise"))
                {
                    // grab tier
                    int Tier = Window1.config.ReraiseTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Reraise", "Reraise II", "Reraise III" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // PROTECT
                else if (IsBuffActive(40, "None") == false && Window1.config.ProtectToggle == true && CanCastSpell("Protect"))
                {
                    // grab tier
                    int Tier = Window1.config.ProtectTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Protect", "Protect II", "Protect III", "Protect IV", "Protect V" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // SHELL
                else if (IsBuffActive(41, "None") == false && Window1.config.ShellToggle == true && CanCastSpell("Shell"))
                {
                    // grab tier
                    int Tier = Window1.config.ShellTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Shell", "Shell II", "Shell III", "Shell IV", "Shell V" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // REFRESH
                else if (IsBuffActive(43, "None") == false && Window1.config.RefreshToggle == true && CanCastSpell("Refresh"))
                {
                    // grab tier
                    int Tier = Window1.config.ShellTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Refresh", "Refresh  II", "Refresh III" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // REGEN
                else if (IsBuffActive(42, "None") == false && Window1.config.RegenToggle == true && CanCastSpell("Regen"))
                {
                    // grab tier
                    int Tier = Window1.config.RegenTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Regen", "Regen II", "Regen III", "Regen IV", "Regen V" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // AQUAVEIL
                else if (IsBuffActive(39, "None") == false && Window1.config.AquaveilToggle == true && CanCastSpell("Aquaveil"))
                {
                    // Cast the spell
                    CastSpell("Aquaveil", "<me>");
                }
                // BLINK
                else if (IsBuffActive(36, "None") == false && Window1.config.BlinkToggle == true && CanCastSpell("Blink"))
                {
                    // Cast the spell
                    CastSpell("Blink", "<me>");
                }
                // PHALANX
                else if (IsBuffActive(116, "None") == false && Window1.config.PhalanxToggle == true && CanCastSpell("Phalanx"))
                {
                    // Cast the spell
                    CastSpell("Phalanx", "<me>");
                }
                // STONESKIN
                else if (IsBuffActive(37, "None") == false && Window1.config.StoneskinToggle == true && CanCastSpell("Stoneskin"))
                {
                    // Cast the spell
                    CastSpell("Stoneskin", "<me>");
                }
                // HASTE 
                else if (IsBuffActive(33, "None") == false && IsBuffActive(13, "None") == false && Window1.config.HasteToggle == true && CanCastSpell("Haste"))
                {
                    // grab tier
                    int Tier = Window1.config.HasteTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Haste", "Haste II" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // GAIN 
                else if (IsBuffActive(0, "Gain") == false && Window1.config.HasteToggle == true && CanCastSpell("Gain"))
                {
                    // grab tier
                    int Tier = Window1.config.GainTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Gain-STR", "Gain-DEX", "Gain-CHR", "Gain-AGI", "Gain-VIT", "Gain-INT", "Gain-MND" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // STORM SPELL
                else if (IsBuffActive(0, "Storm") == false && Window1.config.StormToggle == true && CanCastSpell("Storm"))
                {
                    // grab tier
                    int Tier = Window1.config.StormTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Firestorm", "Sandstorm", "Rainstorm", "Windstorm", "Hailstorm", "Thunderstorm", "Aurorastorm", "Voidstorm",
                                                        "Firestorm II", "Sandstorm II", "Rainstorm II", "Windstorm II", "Hailstorm II", "Thunderstorm II", "Aurorastorm II", "Voidstorm II" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // ANIMUS / ENMITY  SPELL
                else if (IsBuffActive(0, "Animus") == false && Window1.config.EnmityToggle == true && CanCastSpell("Animus"))
                {
                    // grab tier
                    int Tier = Window1.config.GainTier;
                    // known tiers
                    var KnownTiers = new List<string> { "Animus Augeo", "Animus Minuo" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // GEOMANCY SPELLS - INDI
                else if (CanCastSpell("Indi") && IsBuffActive(612, "None") && ((Window1.config.GeomancerOnlyInCombat == true && CombatStatus == true) || Window1.config.GeomancerOnlyInCombat == false))
                {
                    // grab tier
                    int Tier = Window1.config.IndiSpell;
                    // known tiers
                    var KnownTiers = new List<string> {"Indi-Voidance", "Indi-Precision", "Indi-Regen", "Indi-Haste", "Indi-Attunement", "Indi-Focus", "Indi-Barrier",
                                                       "Indi-Refresh", "Indi-CHR", "Indi-MND", "Indi-Fury", "Indi-INT", "Indi-AGI", "Indi-Fend", "Indi-VIT", "Indi-DEX",
                                                       "Indi-Acumen", "Indi-STR", "Indi-Poison", "Indi-Slow", "Indi-Torpor", "Indi-Slip","Indi-Languor", "Indi-Paralysis",
                                                       "Indi-Vex","Indi-Frailty","Indi-Wilt","Indi-Malaise", "Indi-Gravity", "Indi-Fade" };
                    // Cast the spell
                    CastSpell(KnownTiers[Tier], "<me>");
                }
                // GEOMANCY SPELLS - GEO
                else if (CanCastSpell("Geo") && ((Window1.config.GeomancerOnlyInCombat == true && CombatStatus == true) || Window1.config.GeomancerOnlyInCombat == false && GeoSpell_CombatRequirement(Window1.config.GeoSpell)) == true && api.Player.Pet.HealthPercent < 1)
                {

                    // grab tier
                    int Tier = Window1.config.GeoSpell;
                    // known tiers
                    var KnownTiers = new List<string> {"Geo-Voidance", "Geo-Precision", "Geo-Regen", "Geo-Haste", "Geo-Attunement", "Geo-Focus", "Geo-Barrier",
                                                       "Geo-Refresh", "Geo-CHR", "Geo-MND", "Geo-Fury", "Geo-INT", "Geo-AGI", "Geo-Fend", "Geo-VIT", "Geo-DEX",
                                                       "Geo-Acumen", "Geo-STR", "Geo-Poison", "Geo-Slow", "Geo-Torpor", "Geo-Slip","Geo-Languor", "Geo-Paralysis",
                                                       "Geo-Vex","Geo-Frailty","Geo-Wilt","Geo-Malaise", "Geo-Gravity", "Geo-Fade" };


                    // PARTY SPELL
                    if (GeoSpell_CombatRequirement(Window1.config.GeoSpell) == true)
                    {
                        CastSpell(KnownTiers[Tier], "<me>");
                    }
                    // ENEMY SPELL
                    else if (InCombat == true)
                    {
                        CastSpell(KnownTiers[Tier], "<bt>");
                    }
                    else
                    {
                        // DO NOTHING
                    }

                }
            }
        }

        #endregion



        #region "Perform Combat Checker"

        private void CombatCheckerTimer_Tick(object sender, EventArgs e)
        {
            if (api != null)
            {

                // Set a variable simply to read if the status has changed
                bool CombatStatusChanged = false;

                // Grab the current party, we'll use tohis to check engaged status.
                var CurrentParty = api.Party.GetPartyMembers().Where(p => p.Active != 0 && p.Zone == api.Player.ZoneId);

                // Run through each PARTY member, grab their enity and check engaged status
                foreach (var pData in CurrentParty)
                {
                    if (pData.TargetIndex != 0)
                    {
                        if (api.Entity.GetEntity((int)pData.TargetIndex).Status == 2)
                        {
                            InCombat = true;
                            CombatStatusChanged = true;
                            break;
                        }
                    }
                }

                // If CombatStatusChanged was not changed to true then you're not engaged so set it to false.
                if (CombatStatusChanged == false)
                {
                    InCombat = false;
                }
            }
        }

        #endregion


        #region "Backgrounds worker to check for commands"
        public void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (api == null)
            {
                return;
            }
            if (api.Player.LoginStatus != (int)LoginStatus.LoggedIn)
            {
                return;
            }
            if (api.Player.LoginStatus == (int)LoginStatus.Loading)
            {
                System.Threading.Thread.Sleep(17000);
            }

            string lastCmd = string.Empty;

            int cmdTime = api.ThirdParty.ConsoleIsNewCommand();

            if (lastCommand != cmdTime)
            {
                lastCommand = cmdTime;
                lastCmd = string.Empty;

                if (api.ThirdParty.ConsoleGetArg(0) == "burstmaster")
                {
                    int argCount = api.ThirdParty.ConsoleGetArgCount();
                    if (argCount > 2)
                    {
                        string skillchainLocated = api.ThirdParty.ConsoleGetArg(1);
                        timeDated = DateTime.Parse(api.ThirdParty.ConsoleGetArg(2));

                        Task.Factory.StartNew(() =>
                        {
                            Dispatcher.BeginInvoke(new Action(() => { this.SkillchainRecieved.Content = api.ThirdParty.ConsoleGetArg(1); }));
                            Dispatcher.BeginInvoke(new Action(() => { this.TimeRecieved.Content = api.ThirdParty.ConsoleGetArg(2); }));
                        });

                        RunBurstAction(api.ThirdParty.ConsoleGetArg(1), api.ThirdParty.ConsoleGetArg(2));
                    }
                    else if (argCount == 2 && api.ThirdParty.ConsoleGetArg(1) == "finished")
                    {
                        System.Threading.Thread.Sleep(2500);
                        Task.Factory.StartNew(() =>
                        {
                            Dispatcher.BeginInvoke(new Action(() => { this.CastingPossible.Content = "Casting is possible."; }));
                            Dispatcher.BeginInvoke(new Action(() => { this.ActionBeingPerformed.Content = String.Empty; }));
                        });

                        CanCast = true;
                    }
                    else if (argCount == 2 && api.ThirdParty.ConsoleGetArg(1) == "NOTfinished")
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Dispatcher.BeginInvoke(new Action(() => { this.CastingPossible.Content = "Casting is NOT possible."; }));
                        });
                    }
                    else if (argCount == 2 && api.ThirdParty.ConsoleGetArg(1) == "interruption")
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Dispatcher.BeginInvoke(new Action(() => { this.CastingPossible.Content = "Casting was interrupted."; }));
                        });

                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2.5));

                        Task.Factory.StartNew(() =>
                        {
                            Dispatcher.BeginInvoke(new Action(() => { this.CastingPossible.Content = "Casting is possible."; }));
                            Dispatcher.BeginInvoke(new Action(() => { this.ActionBeingPerformed.Content = String.Empty; }));
                        });
                        CanCast = true;
                    }
                }
            }
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(2000 / 500));

        }


        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            backgroundWorker.RunWorkerAsync();
        }

        #endregion

        #region "Skillchain privates"

        private void RunBurstAction(string SkillchainLocated, string TimeLocated)
        {
            _time = TimeSpan.FromSeconds(9);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    this.TimeRemaining.Content = _time.ToString("c");
                    if (_time == TimeSpan.Zero) _timer.Stop();
                    _time = _time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

            _timer.Start();

            BurstPossible = true;
        }

        private void SkillchainBurstTimer_Tick(object sender, EventArgs e)
        {
            if (BurstPossible == true && skillchainLocated != String.Empty && api != null)
            {
                var skillchainData = skillchains.Find(x => x.Skillchain_name == skillchainLocated);

                DateTime windowGone = timeDated.AddSeconds(10);

                if (DateTime.Now > windowGone)
                {
                    // Skillchain widow is gone, move along.
                    Task.Factory.StartNew(() =>
                    {
                        Dispatcher.BeginInvoke(new Action(() => { this.SkillchainRecieved.Content = String.Empty; }));
                        Dispatcher.BeginInvoke(new Action(() => { this.TimeRecieved.Content = String.Empty; }));
                        Dispatcher.BeginInvoke(new Action(() => { this.TimeRemaining.Content = String.Empty; }));
                        Dispatcher.BeginInvoke(new Action(() => { this.ActionBeingPerformed.Content = "Skillchain burst window has gone."; }));
                    });

                    skillchainLocated = String.Empty;
                    BurstPossible = false;

                    return;
                }
                else
                {
                    Task.Factory.StartNew(() =>
                    {
                        Dispatcher.BeginInvoke(new Action(() => { this.ActionBeingPerformed.Content = "Skillchain window is open."; }));
                    });
                }
            }
        }

        #endregion

        #region "Metro Window Closing Event"

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (api != null)
            {
                if (WindowerMode == "Ashita")
                {
                    api.ThirdParty.SendString("/addon unload BurstMaster_addon");
                }
                else if (WindowerMode == "Windower")
                {
                    api.ThirdParty.SendString("//lua unload BurstMaster_addon");
                }
            }
        }

        #endregion

        #region "Settings button event"

        private Window settings = null;

        private void SettingsButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (settings == null)
            {
                Window settings = new Window1();
                settings.ShowDialog();
            }
        }

        #endregion

        #region "Error Dialog"

        // ERROR DIALOG WILL BE SHOWN WHENEVER AN ERROR IS ENCOUNTERED.
        public async void ErrorDialog(string ErrorTitle, string ErrorMessage)
        {
            await Task.Delay(10);
            await this.ShowMessageAsync(ErrorTitle, ErrorMessage);
        }

        #endregion

        #region "HP, MP, TP and Status Buffs area"

        private void BasicStatsTimer_Tick(object sender, EventArgs e)
        {
            if (api != null)
            {
                this.HPBar.Maximum = api.Player.HPMax;
                this.HPBar.Value = api.Player.HP;

                this.HPBar.Maximum = api.Player.HPMax;
                this.HPBar.Value = api.Player.HP;

                this.MPBar.Maximum = api.Player.MPMax;
                this.MPBar.Value = api.Player.MP;

                this.TPBar.Maximum = 3000;
                this.TPBar.Value = api.Player.TP;

                this.HP.Content = api.Player.HP + "/" + api.Player.HPMax;
                this.MP.Content = api.Player.MP + "/" + api.Player.MPMax;
                this.TP.Content = api.Player.TP + "/3000";

                // HP BAR
                HPBar.Background = Brushes.LightGray;
                HPBar.Foreground = Brushes.Green;

                // MP BAR
                MPBar.Background = Brushes.LightGray;
                MPBar.Foreground = Brushes.MediumPurple;

                // TP BAR
                TPBar.Background = Brushes.LightGray;
                TPBar.Foreground = Brushes.Orange;
            }
        }

        private void StatusEffectsTimer_Tick(object sender, EventArgs e)
        {
            if (api != null)
            {
                var All = new ObservableCollection<BitmapImage>();

                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "status_images");

                foreach (int plEffect in api.Player.Buffs)
                {
                    if (System.IO.File.Exists(path + "\\" + plEffect + ".png"))
                    {
                        String pathed = path + "\\" + plEffect + ".png";
                        BitmapImage image = new BitmapImage(new Uri(pathed, UriKind.RelativeOrAbsolute));
                        All.Add(image);
                    }
                }

                StatusBuffs.ItemsSource = All;
            }
        }

        #endregion

        #region "Pause and Unpause button"

        private void RunPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (api != null)
            {
                if (IsRunning == false)
                {
                    IsRunning = true;
                    RunPauseButton.Content = "PAUSE";
                    RunPauseButton.Background = Brushes.Green;
                    RunPauseButton.BorderBrush = Brushes.Green;
                }
                else
                {
                    IsRunning = false;
                    RunPauseButton.Content = "UNPAUSE";
                    RunPauseButton.BorderBrush = Brushes.DarkRed;
                    RunPauseButton.Background = Brushes.DarkRed;
                }
            }
        }


        #endregion

        #region "Make sure the Window closes properly."

        private void MainWindow1_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

    }
}