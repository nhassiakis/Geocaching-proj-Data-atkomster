using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Geocaching.Models;

namespace Geocaching
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {



        // Contains the ID string needed to use the Bing map.
        // Instructions here: https://docs.microsoft.com/en-us/bingmaps/getting-started/bing-maps-dev-center-help/getting-a-bing-maps-key
        private const string applicationId = "ArrPoarKmhwzYYvnD3Ws7Cl1Cj14XE9y95ylBEc6i2MW26Ty77PgjgIpV85CoU5D";

        private MapLayer layer;

        // Contains the location of the latest click on the map.
        // The Location object in turn contains information like longitude and latitude.
        private Location latestClickLocation;

        private Location gothenburg = new Location(57.719021, 11.991202);

       

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            if (applicationId == null)
            {
                MessageBox.Show("Please set the applicationId variable before running this program.");
                Environment.Exit(0);
            }

            CreateMap();

            using (var db = new AppDbContext())
            {
                // Load data from database and populate map here.
            }
        }

        private void CreateMap()
        {
            map.CredentialsProvider = new ApplicationIdCredentialsProvider(applicationId);
            map.Center = gothenburg;
            map.ZoomLevel = 12;
            layer = new MapLayer();
            map.Children.Add(layer);

            MouseDown += (sender, e) =>
            {
                var point = e.GetPosition(this);
                latestClickLocation = map.ViewportPointToLocation(point);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    OnMapLeftClick();
                }
            };

            map.ContextMenu = new ContextMenu();

            var addPersonMenuItem = new MenuItem { Header = "Add Person" };
            map.ContextMenu.Items.Add(addPersonMenuItem);
            addPersonMenuItem.Click += OnAddPersonClick;

            var addGeocacheMenuItem = new MenuItem { Header = "Add Geocache" };
            map.ContextMenu.Items.Add(addGeocacheMenuItem);
            addGeocacheMenuItem.Click += OnAddGeocacheClick;
        }

        private void UpdateMap()
        {
            // It is recommended (but optional) to use this method for setting the color and opacity of each pin after every user interaction that might change something.
            // This method should then be called once after every significant action, such as clicking on a pin, clicking on the map, or clicking a context menu option.
        }

        private void OnMapLeftClick()
        {
            // Handle map click here.
            UpdateMap();
        }

        private void OnAddGeocacheClick(object sender, RoutedEventArgs args)
        {
            var dialog = new GeocacheDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
            if (dialog.DialogResult == false)
            {
                return;
            }

            string contents = dialog.GeocacheContents;
            string message = dialog.GeocacheMessage;
            // Add geocache to map and database here.
            var pin = AddPin(latestClickLocation, "Person", Colors.Gray);

            pin.MouseDown += (s, a) =>
            {
                // Handle click on geocache pin here.
                MessageBox.Show("You clicked a geocache");
                UpdateMap();

                // Prevent click from being triggered on map.
                a.Handled = true;
            };
        }

        private void OnAddPersonClick(object sender, RoutedEventArgs args)
        {
            var dialog = new PersonDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
            if (dialog.DialogResult == false)
            {
                return;
            }

            string city = dialog.AddressCity;
            string country = dialog.AddressCountry;
            string streetName = dialog.AddressStreetName;
            int streetNumber = dialog.AddressStreetNumber;
            // Add person to map and database here.
            var pin = AddPin(latestClickLocation, "Person", Colors.Blue);

            pin.MouseDown += (s, a) =>
            {
                // Handle click on person pin here.
                MessageBox.Show("You clicked a person");
                UpdateMap();

                // Prevent click from being triggered on map.
                a.Handled = true;
            };
        }

        private Pushpin AddPin(Location location, string tooltip, Color color)
        {
            var pin = new Pushpin();
            pin.Cursor = Cursors.Hand;
            pin.Background = new SolidColorBrush(color);
            ToolTipService.SetToolTip(pin, tooltip);
            ToolTipService.SetInitialShowDelay(pin, 0);
            layer.AddChild(pin, new Location(location.Latitude, location.Longitude));
            return pin;
        }

        private void OnLoadFromFileClick(object sender, RoutedEventArgs args)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";
            bool? result = dialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            string path = dialog.FileName;
            // Read the selected file here.

            int counter = 0;

             var person = new Dictionary<int, Person>();
             var geocache = new Dictionary<int, Geocache>();
             var foundGeocache = new Dictionary<int, FoundGeocache>();

            string[] lines = File.ReadAllLines(path).ToArray();
            foreach (string line in lines)
            {
                
                try
                {
                    string checkNumbers = "0123456789";
                    bool CheckInt = false;
                    if (line == string.Empty)
                    {

                    }
                    else if (checkNumbers.Contains(line.First()))
                    {
                        CheckInt = true;
                    }
                    else if (CheckInt == false)
                    {
                        
                        if(line.StartsWith("Found:"))
                        {
                            char[] trimString = { 'F', 'o', 'u', 'n', 'd', ':', ' '};
                            string[] values = line.Split(',').Select(v => v.Trim(trimString)).ToArray();
                           
                            foreach (var item in values)
                            {
                                if (item == "")
                                {

                                }
                                else
                                {
                                    foundGeocache[counter] = new FoundGeocache
                                    {
                                        PersonID = counter,
                                        GeocacheID = int.Parse(item)
                                    };

                                }

                            }

                        counter++;
                        }
                        else
                        {
                            string[] values = line.Split('|').Select(v => v.Trim()).ToArray();
                            string firstName = values[0];
                            string lastName = values[1];
                            string country = values[2];
                            string city = values[3];
                            string streetName = values[4];
                            Int16 streetNumber = Int16.Parse(values[5]);
                            double latitude = double.Parse(values[6]);
                            double longitude = double.Parse(values[7]);

                            person[counter] = new Person
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                Country = country,
                                City = city,
                                StreetName = streetName,
                                StreetNumber = streetNumber,
                                Latitude = latitude,
                                Longitude = longitude
                            };
                        }


                    }

                    if (CheckInt == true)
                    {
                        string[] values = line.Split('|').Select(v => v.Trim()).ToArray();
                        int id = int.Parse(values[0]);
                        double latitude = double.Parse(values[1]);
                        double longitude = double.Parse(values[2]);
                        string contents = values[3];
                        string message = values[4];

                        geocache[id] = new Geocache
                        {
                            ID = id,
                            Latitude = latitude,
                            Longitude = longitude,
                            Contents = contents,
                            Message = message
                        };
                    }
                }
                catch
                {
                    Console.WriteLine("Could not read file." + line);
                }
            }

        }

        private void OnSaveToFileClick(object sender, RoutedEventArgs args)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";
            dialog.FileName = "Geocaches";
            bool? result = dialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            string path = dialog.FileName;
            // Write to the selected file here.
        }
    }
}
