using DR_Tools.Models;
using EDAP_Waypoint_Editor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

                FavFoldersDataGrid.ItemsSource = Waypoints.Waypoints;
                DataGridGlobalShoppingList.ItemsSource = Waypoints.globalshoppinglist.BuyCommodities;
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
                    waypoint.BuyCommodities.Add(shopitem.Name, shopitem.Quantity);
                }

                foreach (var shopitem in item.SellCommodities)
                {
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
            LoadWaypointFile("C:\\Users\\shuttle\\OneDrive\\Programming\\Python\\EDAPGui - Stumpii-Main\\waypoints\\waypoints.json");
        }

        private void FavFoldersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var wp = (InternalWaypoint)FavFoldersDataGrid.SelectedItem;
            DataGridBuyShoppingList.ItemsSource = wp.BuyCommodities;
            DataGridSellShoppingList.ItemsSource = wp.SellCommodities;
        }
    }
}