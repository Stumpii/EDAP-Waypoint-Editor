using DR_Tools.Models;
using EDAP_Waypoint_Editor.Models;
using Microsoft.Win32;
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

        private List<string> GalaxyMapBookmarkTypesList = new List<string>() { "", "Favorite", "System", "Body", "Station", "Settlement" };
        private List<string> SystemMapBookmarkTypesList = new List<string>() { "", "Favorite", "Body", "Station", "Settlement", "Navigation Panel" };

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

        private void MenuFileSave(object sender, RoutedEventArgs e)
        {
            if (programSettings.LastOpenFilepath != "")
                SaveWaypointFile(programSettings.LastOpenFilepath);
            else
                MenuFileSaveAs(null, new RoutedEventArgs());
        }

        private void MenuFileOpen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json file (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                LoadWaypointFile(openFileDialog.FileName);
                programSettings.LastOpenFilepath = openFileDialog.FileName;
            }
        }

        private void MenuFileNew(object sender, RoutedEventArgs e)
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
    }
}