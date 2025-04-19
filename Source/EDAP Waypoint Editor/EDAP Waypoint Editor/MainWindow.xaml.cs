﻿using DR_Tools.Models;
using EDAP_Waypoint_Editor.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

namespace EDAP_Waypoint_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private const string ProductName = "EDAP Waypoint Editor";

        private ProgramSettings programSettings = new ProgramSettings();

        private List<string> GalaxyMapBookmarkTypesList = new List<string>() { "", "Favorite", "System", "Body", "Station", "Settlement" };
        private List<string> SystemMapBookmarkTypesList = new List<string>() { "", "Favorite", "Body", "Station", "Settlement", "Navigation Panel" };
        private Dictionary<string, List<string>> commodities = new Dictionary<string, List<string>>();
        public static List<string> AllCommodities = new List<string>();

        #endregion Fields

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            // Load settings
            SettingsLoad();
        }

        #endregion Constructors

        #region Properties

        //public WaypointsFile Waypoints { get; private set; }
        public Dictionary<string, Waypoint> RawWaypoints { get; set; }

        public InternalWaypoints Waypoints { get; set; } = new InternalWaypoints();

        #endregion Properties

        #region Methods

        private void Serializer_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            Debug.WriteLine(e.ErrorContext.Error.Message);
            MessageBox.Show("Serializer_Error.", ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsLoad()
        {
            try
            {
                string AppDataFolder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), ProductName);
                if (!Directory.Exists(AppDataFolder))
                    Directory.CreateDirectory(AppDataFolder);

                // deserialize JSON directly from a file
                string settingsFilename = Path.Combine(AppDataFolder, "Program Settings.json");
                if (File.Exists(settingsFilename))
                {
                    using (StreamReader file = File.OpenText(settingsFilename))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Error += Serializer_Error;
                        programSettings = (ProgramSettings)serializer.Deserialize(file, typeof(ProgramSettings));

                        this.Left = programSettings.MainFormX;
                        this.Top = programSettings.MainFormY;
                        this.Width = programSettings.MainFormWidth;
                        this.Height = programSettings.MainFormHeight;

                        this.DataContext = programSettings;
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        public bool LoadWaypointFile(string filepath)
        {
            // deserialize JSON directly from a file
            if (File.Exists(filepath))
            {
                using (StreamReader file = File.OpenText(filepath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Error += Serializer_Error;
                    RawWaypoints = (Dictionary<string, Waypoint>)serializer.Deserialize(file, typeof(Dictionary<string, Waypoint>));
                    //Waypoints = (WaypointsFile)serializer.Deserialize(file, typeof(WaypointsFile));

                    // Convert to internal
                    Waypoints.Waypoints.Clear();
                    foreach (var item in RawWaypoints)
                    {
                        if (item.Key == "GlobalShoppingList")
                        {
                            InternaGlobalshoppinglist globalshoppinglist = Waypoints.globalshoppinglist;
                            globalshoppinglist.Name = item.Key;
                            globalshoppinglist.UpdateCommodityCount = item.Value.UpdateCommodityCount;
                            globalshoppinglist.Skip = item.Value.Skip;
                            globalshoppinglist.Completed = item.Value.Completed;

                            globalshoppinglist.BuyCommodities.Clear();
                            foreach (var item1 in item.Value.BuyCommodities)
                            {
                                ShoppingItem commod = new ShoppingItem();
                                commod.Name = item1.Key;
                                commod.Quantity = item1.Value;

                                globalshoppinglist.BuyCommodities.Add(commod);
                            }
                        }
                        else
                        {
                            InternalWaypoint waypoint = new InternalWaypoint();
                            waypoint.Name = item.Key;
                            waypoint.SystemName = item.Value.SystemName;
                            waypoint.GalaxyBookmarkType = item.Value.GalaxyBookmarkType;
                            waypoint.GalaxyBookmarkNumber = item.Value.GalaxyBookmarkNumber;
                            waypoint.StationName = item.Value.StationName;
                            waypoint.SystemBookmarkType = item.Value.SystemBookmarkType;
                            waypoint.SystemBookmarkNumber = item.Value.SystemBookmarkNumber;
                            waypoint.UpdateCommodityCount = item.Value.UpdateCommodityCount;
                            waypoint.FleetCarrierTransfer = item.Value.FleetCarrierTransfer;
                            waypoint.Skip = item.Value.Skip;
                            waypoint.Completed = item.Value.Completed;

                            waypoint.BuyCommodities.Clear();
                            foreach (var item1 in item.Value.BuyCommodities)
                            {
                                ShoppingItem commod = new ShoppingItem();
                                commod.Name = item1.Key;
                                commod.Quantity = item1.Value;

                                waypoint.BuyCommodities.Add(commod);
                            }

                            waypoint.SellCommodities.Clear();
                            foreach (var item1 in item.Value.SellCommodities)
                            {
                                ShoppingItem commod = new ShoppingItem();
                                commod.Name = item1.Key;
                                commod.Quantity = item1.Value;

                                waypoint.SellCommodities.Add(commod);
                            }

                            Waypoints.Waypoints.Add(waypoint);
                        }
                    }
                }

                this.DataContext = Waypoints.Waypoints;

                DataGridWaypoints.ItemsSource = Waypoints.Waypoints;
                DataGridGlobalShoppingList.ItemsSource = Waypoints.globalshoppinglist.BuyCommodities;

                GridGlobalShopping.DataContext = Waypoints.globalshoppinglist;
                Griditems.DataContext = null;
            }

            return true;
        }

        private void SettingsSave()
        {
            string AppDataFolder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), ProductName);
            if (!Directory.Exists(AppDataFolder))
                Directory.CreateDirectory(AppDataFolder);

            programSettings.MainFormX = this.Left;
            programSettings.MainFormY = this.Top;
            programSettings.MainFormWidth = this.Width;
            programSettings.MainFormHeight = this.Height;

            // serialize JSON directly to a file
            string settingsFilename = Path.Combine(AppDataFolder, "Program Settings.json");
            using (StreamWriter file = File.CreateText(settingsFilename))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, programSettings);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWaypointFile("C:\\Users\\shuttle\\OneDrive\\Programming\\Python\\EDAPGui - Stumpii-Main\\waypoints\\waypoints1.json");
            SettingsSave();
        }

        private void SaveWaypointFile(string filepath)
        {
            RawWaypoints.Clear();

            Waypoint waypointg = new Waypoint();
            waypointg.Skip = Waypoints.globalshoppinglist.Skip;
            waypointg.UpdateCommodityCount = Waypoints.globalshoppinglist.UpdateCommodityCount;
            waypointg.Completed = Waypoints.globalshoppinglist.Completed;

            foreach (var shopitem in Waypoints.globalshoppinglist.BuyCommodities)
            {
                if (!waypointg.BuyCommodities.ContainsKey(shopitem.Name))
                    waypointg.BuyCommodities.Add(shopitem.Name, shopitem.Quantity);
            }

            RawWaypoints.Add("GlobalShoppingList", waypointg);

            int i = 1;
            foreach (var item in Waypoints.Waypoints)
            {
                Waypoint waypoint = new Waypoint();
                waypoint.SystemName = item.SystemName;
                waypoint.GalaxyBookmarkType = item.GalaxyBookmarkType;
                waypoint.GalaxyBookmarkNumber = item.GalaxyBookmarkNumber;
                waypoint.StationName = item.StationName;
                waypoint.SystemBookmarkType = item.SystemBookmarkType;
                waypoint.SystemBookmarkNumber = item.SystemBookmarkNumber;
                waypoint.UpdateCommodityCount = item.UpdateCommodityCount;
                waypoint.FleetCarrierTransfer = item.FleetCarrierTransfer;
                waypoint.Skip = item.Skip;
                waypoint.Completed = item.Completed;

                foreach (var shopitem in item.BuyCommodities)
                {
                    if (!waypoint.BuyCommodities.ContainsKey(shopitem.Name))
                        waypoint.BuyCommodities.Add(shopitem.Name, shopitem.Quantity);
                }

                foreach (var shopitem in item.SellCommodities)
                {
                    if (!waypoint.SellCommodities.ContainsKey(shopitem.Name))
                        waypoint.SellCommodities.Add(shopitem.Name, shopitem.Quantity);
                }

                RawWaypoints.Add(i.ToString(), waypoint);
                i++;
            }

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(filepath))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(file, RawWaypoints);
            }
        }

        #endregion Methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Add some data to the dictionary
            commodities["Chemicals"] = new List<string> { "Agronomic Treatment", "Explosives", "Hydrogen Fuel", "Hydrogen Peroxide", "Liquid Oxygen", "Mineral Oil", "Nerve Agents", "Pesticides", "Rockforth Fertiliser", "Surface Stabilisers", "Synthetic Reagents", "Tritium", "Water" };
            commodities["Consumer Items"] = new List<string> { "Clothing", "Consumer Technology", "Domestic Appliances", "Evacuation Shelter", "Survival Equipment" };
            commodities["Foods"] = new List<string> { "Algae", "Animal Meat", "Coffee", "Fish", "Food Cartridges", "Fruit and Vegetables", "Grain", "Synthetic Meat", "Tea" };
            commodities["Industrial Materials"] = new List<string> { "Ceramic Composites", "CMM Composite", "Insulating Membrane", "Meta-Alloys", "Micro-Weave Cooling Hoses", "Neofabric Insulation", "Polymers", "Semiconductors", "Superconductors" };
            commodities["Legal Drugs"] = new List<string> { "Beer", "Bootleg Liquor", "Liquor", "Narcotics", "Onionhead Gamma Strain", "Tobacco", "Wine" };
            commodities["Machinery"] = new List<string> { "Articulation Motors", "Atmospheric Processors", "Building Fabricators", "Crop Harvesters", "Emergency Power Cells", "Energy Grid Assembly", "Exhaust Manifold", "Geological Equipment", "Heatsink Interlink", "HN Shock Mount", "Magnetic Emitter Coil", "Marine Equipment", "Microbial Furnaces", "Mineral Extractors", "Modular Terminals", "Power Converter", "Power Generators", "Power Transfer Bus", "Radiation Baffle", "Reinforced Mounting Plate", "Skimmer Components", "Thermal Cooling Units", "Water Purifiers" };
            commodities["Medicines"] = new List<string> { "Advanced Medicines", "Agri-Medicines", "Basic Medicines", "Combat Stabilisers", "Performance Enhancers", "Progenitor Cells" };
            commodities["Metals"] = new List<string> { "Aluminium", "Beryllium", "Bismuth", "Cobalt", "Copper", "Gallium", "Gold", "Hafnium 178", "Indium", "Lanthanum", "Lithium", "Osmium", "Palladium", "Platinum", "Platinum Alloy", "Praseodymium", "Samarium", "Silver", "Steel", "Tantalum", "Thallium", "Thorium", "Titanium", "Uranium" };
            commodities["Minerals"] = new List<string> { "Alexandrite", "Bauxite", "Benitoite", "Bertrandite", "Bromellite", "Coltan", "Cryolite", "Gallite", "Goslarite", "Grandidierite", "Indite", "Jadeite", "Lepidolite", "Lithium Hydroxide", "Low Temperature Diamonds", "Methane Clathrate", "Methanol Monohydrate Crystals", "Moissanite", "Monazite", "Musgravite", "Painite", "Pyrophyllite", "Rhodplumsite", "Rutile", "Serendibite", "Taaffeite", "Uraninite", "Void Opals" };
            commodities["Salvage"] = new List<string> { "AI Relics", "Ancient Artefact", "Ancient Key", "Anomaly Particles", "Antimatter Containment Unit", "Antique Jewellery", "Antiquities", "Assault Plans", "Black Box", "Commercial Samples", "Damaged Escape Pod", "Data Core", "Diplomatic Bag", "Earth Relics", "Encrypted Correspondence", "Encrypted Data Storage", "Experimental Chemicals", "Fossil Remnants", "Gene Bank", "Geological Samples", "Guardian Casket", "Guardian Orb", "Guardian Relic", "Guardian Tablet", "Guardian Totem", "Guardian Urn", "Hostage", "Large Survey Data Cache", "Military Intelligence", "Military Plans", "Mollusc Brain Tissue", "Mollusc Fluid", "Mollusc Membrane", "Mollusc Mycelium", "Mollusc Soft Tissue", "Mollusc Spores", "Mysterious Idol", "Occupied Escape Pod", "Personal Effects", "Pod Core Tissue", "Pod Dead Tissue", "Pod Mesoglea", "Pod Outer Tissue", "Pod Shell Tissue", "Pod Surface Tissue", "Pod Tissue", "Political Prisoner", "Precious Gems", "Prohibited Research Materials", "Prototype Tech", "Rare Artwork", "Rebel Transmissions", "SAP 8 Core Container", "Scientific Research", "Scientific Samples", "Small Survey Data Cache", "Space Pioneer Relics", "Tactical Data", "Technical Blueprints", "Thargoid Basilisk Tissue Sample", "Thargoid Biological Matter", "Thargoid Bio-Storage Capsule", "Thargoid Cyclops Tissue Sample", "Thargoid Glaive Tissue Sample", "Thargoid Heart", "Thargoid Hydra Tissue Sample", "Thargoid Link", "Thargoid Orthrus Tissue Sample", "Thargoid Probe", "Thargoid Resin", "Thargoid Sensor", "Thargoid Medusa Tissue Sample", "Thargoid Scout Tissue Sample", "Thargoid Technology Samples", "Time Capsule", "Titan Deep Tissue Sample", "Titan Maw Deep Tissue Sample", "Titan Maw Partial Tissue Sample", "Titan Maw Tissue Sample", "Titan Partial Tissue Sample", "Titan Tissue Sample", "Trade Data", "Trinkets of Hidden Fortune", "Unclassified Relic", "Unoccupied Escape Pod", "Unstable Data Core", "Wreckage Components" };
            commodities["Slavery"] = new List<string> { "Imperial Slaves", "Slaves" };
            commodities["Technology"] = new List<string> { "Advanced Catalysers", "Animal Monitors", "Aquaponic Systems", "Auto Fabricators", "Bioreducing Lichen", "Computer Components", "H.E. Suits", "Hardware Diagnostic Sensor", "Ion Distributor", "Land Enrichment Systems", "Medical Diagnostic Equipment", "Micro Controllers", "Muon Imager", "Nanobreakers", "Resonating Separators", "Robotics", "Structural Regulators", "Telemetry Suite" };
            commodities["Textiles"] = new List<string> { "Conductive Fabrics", "Leather", "Military Grade Fabrics", "Natural Fabrics", "Synthetic Fabrics" };
            commodities["Waste"] = new List<string> { "Biowaste", "Chemical Waste", "Scrap", "Toxic Waste" };
            commodities["Weapons"] = new List<string> { "Battle Weapons", "Landmines", "Non Lethal Weapons", "Personal Weapons", "Reactive Armour" };

            foreach (var commodity in commodities)
            {
                foreach (var item in commodity.Value)
                {
                    AllCommodities.Add(item);
                }
            }
            AllCommodities.Sort();

            if (programSettings.LastOpenFilepath != "")
            {
                LoadWaypointFile(programSettings.LastOpenFilepath);
            }
            // Load default combos, etc/
            ComboGalaxyBookmarkType.ItemsSource = GalaxyMapBookmarkTypesList;
            ComboSystemBookmarkType.ItemsSource = SystemMapBookmarkTypesList;
        }

        private void FavFoldersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;
                Griditems.DataContext = wp;
                DataGridBuyShoppingList.ItemsSource = wp.BuyCommodities;
                DataGridSellShoppingList.ItemsSource = wp.SellCommodities;
            }
            else
            {
                Griditems.DataContext = null;
                DataGridBuyShoppingList.ItemsSource = null;
                DataGridSellShoppingList.ItemsSource = null;
            }
        }

        private void MenuFileExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuFileSave_Click(object sender, RoutedEventArgs e)
        {
            if (programSettings.LastOpenFilepath != "")
                SaveWaypointFile(programSettings.LastOpenFilepath);
            else
                MenuFileSaveAs(null, new RoutedEventArgs());
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json file (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                LoadWaypointFile(openFileDialog.FileName);
                programSettings.LastOpenFilepath = openFileDialog.FileName;
            }
        }

        private void MenuFileNew_Click(object sender, RoutedEventArgs e)
        {
            RawWaypoints = new Dictionary<string, Waypoint>();
            Waypoints = new InternalWaypoints();
        }

        private void MenuFileSaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json file (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveWaypointFile(saveFileDialog.FileName);
                programSettings.LastOpenFilepath = saveFileDialog.FileName;
            }
        }

        private void ButtonWaypointUp_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                int currindex = Waypoints.Waypoints.IndexOf(wp);
                Waypoints.Waypoints.Move(currindex, MoveDirection.Up);

                DataGridWaypoints.Items.Refresh();
            }
        }

        private void ButtonWaypointDown_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                int currindex = Waypoints.Waypoints.IndexOf(wp);
                Waypoints.Waypoints.Move(currindex, MoveDirection.Down);

                DataGridWaypoints.Items.Refresh();
            }
        }

        private void ButtonBuyUp_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                if (DataGridBuyShoppingList.SelectedItem is ShoppingItem)
                {
                    var item = (ShoppingItem)DataGridBuyShoppingList.SelectedItem;

                    int currindex = wp.BuyCommodities.IndexOf(item);
                    wp.BuyCommodities.Move(currindex, MoveDirection.Up);

                    DataGridBuyShoppingList.Items.Refresh();
                }
            }
        }

        private void ButtonBuyDown_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                if (DataGridBuyShoppingList.SelectedItem is ShoppingItem)
                {
                    var item = (ShoppingItem)DataGridBuyShoppingList.SelectedItem;

                    int currindex = wp.BuyCommodities.IndexOf(item);
                    wp.BuyCommodities.Move(currindex, MoveDirection.Down);

                    DataGridBuyShoppingList.Items.Refresh();
                }
            }
        }

        private void ButtonSellUp_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                if (DataGridSellShoppingList.SelectedItem is ShoppingItem)
                {
                    var item = (ShoppingItem)DataGridSellShoppingList.SelectedItem;

                    int currindex = wp.SellCommodities.IndexOf(item);
                    wp.SellCommodities.Move(currindex, MoveDirection.Up);

                    DataGridSellShoppingList.Items.Refresh();
                }
            }
        }

        private void ButtonSellDown_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                if (DataGridSellShoppingList.SelectedItem is ShoppingItem)
                {
                    var item = (ShoppingItem)DataGridSellShoppingList.SelectedItem;

                    int currindex = wp.SellCommodities.IndexOf(item);
                    wp.SellCommodities.Move(currindex, MoveDirection.Down);

                    DataGridSellShoppingList.Items.Refresh();
                }
            }
        }

        private void ButtonGlobalBuyUp_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridGlobalShoppingList.SelectedItem is ShoppingItem)
            {
                var item = (ShoppingItem)DataGridGlobalShoppingList.SelectedItem;

                int currindex = Waypoints.globalshoppinglist.BuyCommodities.IndexOf(item);
                Waypoints.globalshoppinglist.BuyCommodities.Move(currindex, MoveDirection.Up);

                DataGridGlobalShoppingList.Items.Refresh();
            }
        }

        private void ButtonGlobalBuyDown_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridGlobalShoppingList.SelectedItem is ShoppingItem)
            {
                var item = (ShoppingItem)DataGridGlobalShoppingList.SelectedItem;

                int currindex = Waypoints.globalshoppinglist.BuyCommodities.IndexOf(item);
                Waypoints.globalshoppinglist.BuyCommodities.Move(currindex, MoveDirection.Down);

                DataGridGlobalShoppingList.Items.Refresh();
            }
        }

        private void ButtonWaypointAdd_Click(object sender, RoutedEventArgs e)
        {
            Waypoints.Waypoints.Add(new InternalWaypoint("New System", ""));
            DataGridWaypoints.Items.Refresh();
        }

        private void ButtonBuyAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                wp.BuyCommodities.Add(new ShoppingItem("New Item"));
                DataGridBuyShoppingList.Items.Refresh();
            }
        }

        private void ButtonSellAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                wp.SellCommodities.Add(new ShoppingItem("New Item"));
                DataGridSellShoppingList.Items.Refresh();
            }
        }

        private void ButtonGlobalBuyAdd_Click(object sender, RoutedEventArgs e)
        {
            Waypoints.globalshoppinglist.BuyCommodities.Add(new ShoppingItem("New Item"));
            DataGridGlobalShoppingList.Items.Refresh();
        }

        private void ButtonWaypointDel_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;
                Waypoints.Waypoints.Remove(wp);
                DataGridWaypoints.Items.Refresh();
            }
        }

        private void ButtonBuyDel_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                if (DataGridBuyShoppingList.SelectedItem is ShoppingItem)
                {
                    var item = (ShoppingItem)DataGridBuyShoppingList.SelectedItem;

                    wp.BuyCommodities.Remove(item);

                    DataGridBuyShoppingList.Items.Refresh();
                }
            }
        }

        private void ButtonSellDel_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                if (DataGridSellShoppingList.SelectedItem is ShoppingItem)
                {
                    var item = (ShoppingItem)DataGridSellShoppingList.SelectedItem;

                    wp.SellCommodities.Remove(item);

                    DataGridSellShoppingList.Items.Refresh();
                }
            }
        }

        private void ButtonGlobalBuyDel_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridGlobalShoppingList.SelectedItem is ShoppingItem)
            {
                var item = (ShoppingItem)DataGridGlobalShoppingList.SelectedItem;

                Waypoints.globalshoppinglist.BuyCommodities.Remove(item);

                DataGridGlobalShoppingList.Items.Refresh();
            }
        }

        private void ButtonSellAddAll_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWaypoints.SelectedItem is InternalWaypoint)
            {
                var wp = (InternalWaypoint)DataGridWaypoints.SelectedItem;

                wp.SellCommodities.Add(new ShoppingItem("ALL"));
                DataGridSellShoppingList.Items.Refresh();
            }
        }

        private void ToolBarButtonNew_Click(object sender, RoutedEventArgs e)
        {
            MenuFileNew_Click(this, new RoutedEventArgs());
        }

        private void ToolBarButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            MenuFileOpen_Click(this, new RoutedEventArgs());
        }

        private void ToolBarButtonSave_Click(object sender, RoutedEventArgs e)
        {
            MenuFileSave_Click(this, new RoutedEventArgs());
        }

        private void buttonInaraAdd_Click(object sender, RoutedEventArgs e)
        {
            var fromWaypoint = new InternalWaypoint();
            var toWaypoint = new InternalWaypoint();

            bool fromBuy = true;
            bool fromSell = true;
            for (int i = 0; i < multilineTextBox.LineCount - 1; i++)
            {
                string line = multilineTextBox.GetLineText(i);
                if (line != null)
                {
                    if (line.StartsWith("From"))
                    {
                        string detailLine = line.Replace("From", "");
                        var fromData = detailLine.Split('|');
                        var station = fromData[0].Trim();
                        var system = fromData[1].Trim();

                        // Strip out non-ASCII chars
                        station = Regex.Replace(station, @"[^\u0000-\u007F]+", string.Empty);
                        system = Regex.Replace(system, @"[^\u0000-\u007F]+", string.Empty);

                        fromWaypoint.SystemName = system;
                        fromWaypoint.StationName = station;
                    }
                    else if (line.StartsWith("To"))
                    {
                        string detailLine = line.Replace("To", "");
                        var toData = detailLine.Split('|');
                        var station = toData[0].Trim();
                        var system = toData[1].Trim();

                        // Strip out non-ASCII chars
                        station = Regex.Replace(station, @"[^\u0000-\u007F]+", string.Empty);
                        system = Regex.Replace(system, @"[^\u0000-\u007F]+", string.Empty);

                        toWaypoint.SystemName = system;
                        toWaypoint.StationName = station;
                    }
                    else if (line.StartsWith("Buy") && !line.StartsWith("Buy price"))
                    {
                        string detailLine = line.Replace("Buy", "").Trim();

                        if (fromBuy)
                            fromWaypoint.BuyCommodities.Add(new ShoppingItem(detailLine, 9999));
                        else
                            toWaypoint.BuyCommodities.Add(new ShoppingItem(detailLine, 9999));

                        fromBuy = false;
                    }
                    else if (line.StartsWith("Sell") && !line.StartsWith("Sell price"))
                    {
                        string detailLine = line.Replace("Sell", "").Trim();

                        if (fromSell)
                            fromWaypoint.SellCommodities.Add(new ShoppingItem(detailLine, 9999));
                        else
                            toWaypoint.SellCommodities.Add(new ShoppingItem(detailLine, 9999));

                        fromSell = false;
                    }
                }
            }

            Waypoints.Waypoints.Add(fromWaypoint);
            Waypoints.Waypoints.Add(toWaypoint);
            DataGridWaypoints.Items.Refresh();

            MessageBox.Show("Finished.\nRemember to add bookmark data for the stations!", ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}