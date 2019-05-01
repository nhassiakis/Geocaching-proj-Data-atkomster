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
        public Person currentPerson = null;
        private AppDbContext db = new AppDbContext();


        // Contains the ID string needed to use the Bing map.
        // Instructions here: https://docs.microsoft.com/en-us/bingmaps/getting-started/bing-maps-dev-center-help/getting-a-bing-maps-key
        private const string applicationId = "ArrPoarKmhwzYYvnD3Ws7Cl1Cj14XE9y95ylBEc6i2MW26Ty77PgjgIpV85CoU5D";

        private MapLayer layer;

        // Contains the location of the latest click on the map.
        // The Location object in turn contains information like longitude and latitude.
        private Location latestClickLocation;
        int CurrentPerID;
        Color color = new Color();
        private List<Pin> pin = new List<Pin>();
        double opacity = 1;

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



            // Load data from database and populate map here.
            foreach (var personobj in db.Person.Include(x => x.FoundGeocaches))
            {
                Person person = new Person
                {
                    ID = personobj.ID,
                    FirstName = personobj.FirstName,
                    LastName = personobj.LastName,
                    Country = personobj.Country,
                    City = personobj.City,
                    StreetName = personobj.StreetName,
                    StreetNumber = personobj.StreetNumber,
                    Longitude = personobj.Longitude,
                    Latitude = personobj.Latitude
                };
                Location locationPer = new Location(longitude: person.Longitude, latitude: person.Latitude);

                string messageTooltipPer = TooltipMessagePer(person);
                pin.Add(AddPersonPin(locationPer, tooltip: messageTooltipPer, Colors.Blue, 1, person));
                //pinPer.MouseDown += (s, a) =>
                //{

                //    SelectedPerson(s, a);
                //    // Handle click on person pin here.
                //    MessageBox.Show("You clicked a person");
                //    currentPerson = person;
                //    UpdateMap();

                //    // Prevent click from being triggered on map.
                //    a.Handled = true;
                //};

                foreach (var geo in db.Geocache.Include(g => g.Person).Include(fg => fg.FoundGeocaches))
                {
                    Geocache geocache = new Geocache
                    {
                        ID = geo.ID,
                        Latitude = geo.Latitude,
                        Longitude = geo.Longitude,
                        Contents = geo.Contents,
                        Message = geo.Message,
                        Person = geo.Person

                    };

                    Location location = new Location(longitude: geocache.Longitude, latitude: geocache.Latitude);

                    string messageTooltipGeo = TooltipMessageGeo(geocache);
                    pin.Add(AddGeoPin(location, tooltip: messageTooltipGeo, Colors.Gray, 1, geocache));
                    //pinGeo.MouseDown += (s, a) =>
                    //{
                    //    // Handle click on person pin here.
                    //    MessageBox.Show("You clicked a person");

                    //    UpdateMap();

                    //    // Prevent click from being triggered on map.
                    //    a.Handled = true;
                    //};
                }



            }



        }
        #region OnClick
        private void SelectedPerson(object s, MouseButtonEventArgs a)
        {
            //throw new NotImplementedException(); // select person, ändra opacity

        }

        private void LeftClickOnMap()
        {
            currentPerson = null;
            CurrentPerID = 0;
            UpdateMap();
        }
        #endregion
        #region TooltipMessage
        private string TooltipMessageGeo(Geocache geo)
        {
            string message = geo.Latitude + ", " + geo.Longitude + "\n" + geo.Message + "\n" + geo.Contents + "\n" + geo.Person.FirstName + " " + geo.Person.LastName;

            return message;
        }


        private string TooltipMessagePer(Person person)
        {
            string message = person.FirstName + " " + person.LastName + "\n" + person.StreetName + " " + person.StreetNumber;

            return message;
        }

        #endregion

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

            //pasasdasd();

            //ColorsPersonGeoPin();

            ColorGeoPin();
            ColorPersonPin();
        }

        #region Color Pins
        private void pasasdasd()
        {
            IEnumerable<Pin> PersonList = new List<Pin>();
            Person currentPer = new Person();
            //PersonList = db.Person.Include(p => p.FoundGeocaches);

            foreach (Pin pin in pin)
            {

                if (pin.Person != null)
                {
                    currentPer = pin.Person;
                    Location location = new Location(pin.Person.Latitude, pin.Person.Longitude);
                    string perToolTip = TooltipMessagePer(pin.Person);

                    if (CurrentPerID == pin.Person.ID || CurrentPerID == 0)
                    {
                        color = Colors.Red;
                    }
                    else
                    {
                        pin.Opacity = 0.5;
                    }
                    //var pin = AddPersonPin(location, perToolTip, color, currentPer);
                    pin.Background = new SolidColorBrush(color);
                    pin.MouseDown += (s, a) =>
                    {
                        currentPerson = pin.Person;
                        UpdateMap();
                        a.Handled = true;
                    };
                }
            }
        }

        private void ColorsPersonGeoPin()
        {
            IEnumerable<Pin> PersonList = new List<Pin>();
            Person currentPer = new Person();
            //PersonList = db.Person.Include(p => p.FoundGeocaches);

            foreach (var Pin in pin)
            {
                if (Pin.Person != null)
                {
                    currentPer = Pin.Person;
                    Location location = new Location(Pin.Person.Latitude, Pin.Person.Longitude);
                    string perToolTip = TooltipMessagePer(Pin.Person);

                    if (currentPerson == null || currentPerson.ID == Pin.Person.ID)
                    {
                        color = Colors.Blue;
                    }
                    else
                    {
                        Pin.Opacity = 0.5;
                    }

                    var pin = AddPersonPin(location, perToolTip, color, 1, currentPer);
                    Pin.MouseDown += (s, a) =>
                    {
                        currentPerson = Pin.Person;
                        UpdateMap();
                        a.Handled = true;
                    };

                }

                if (Pin.Geocache != null)
                {
                    Location location = new Location(Pin.Geocache.Latitude, Pin.Geocache.Longitude);
                    string geoToolTip = TooltipMessageGeo(Pin.Geocache);

                    if (currentPerson == null)
                    {
                        color = Colors.Gray;
                    }
                    // if geocache was placed by player, color should be black
                    else if (Pin.Person.ID == currentPerson.ID)
                    {
                        color = Colors.Black;
                    }
                    else
                    {
                        color = Colors.Red;
                    }
                    var pinGeo = AddGeoPin(location, geoToolTip, color, 1, Pin.Geocache);
                    foreach (var fg in Pin.Geocache.FoundGeocaches)
                    {
                        if (fg.PersonID == currentPerson.ID)
                        {
                            pinGeo = AddGeoPin(location, geoToolTip, Colors.Green, 1, Pin.Geocache);
                        }
                    }
                    // On click, change color on Geocache-pins.
                    Pin.MouseDown += (s, a) =>
                    {
                        a.Handled = true;
                        ChangeColorGeoPins(Pin, geoToolTip);
                    };
                }
            }
        }

        private void ChangeColorGeoPins(Pin pin, string geoToolTip)
        {
            Geocache currentGeo = new Geocache();
            List<int> currentFGeoIDs = new List<int>();

            currentGeo = pin.Geocache;
            //currentGeo = db.Geocache.Include(geo => geo.Person).First(geoC => geoC.Latitude == pin.Geocache.Latitude && geoC.Longitude == pin.Geocache.Longitude);
            currentFGeoIDs = db.FoundGeocache.Where(fg => fg.PersonID == currentPerson.ID).Select(fg => fg.GeocacheID).ToList();


            if (currentPerson.ID != currentGeo.Person.ID && currentPerson != null)
            {
                if (currentFGeoIDs.Contains(currentGeo.ID))
                {
                    pin = AddGeoPin(pin.Location, geoToolTip, Colors.Red, 1, pin.Geocache);
                    var fgToDelete = db.FoundGeocache.First(fg => (fg.PersonID == currentPerson.ID) && (fg.GeocacheID == currentGeo.ID));
                    db.Remove(db.FoundGeocache.Single(fg => fg.GeocacheID == fgToDelete.GeocacheID && fg.PersonID == currentPerson.ID));
                }

                else
                {
                    pin = AddGeoPin(pin.Location, geoToolTip, Colors.Green, 1, pin.Geocache);
                    var foundFG = new FoundGeocache
                    {
                        GeocacheID = currentGeo.ID,
                        PersonID = currentPerson.ID
                    };
                    db.Add(foundFG);
                }
                db.SaveChanges();
                UpdateMap();
            }
        }

        private void ColorPersonPin()
        {
            IEnumerable<Person> PersonList = new List<Person>();

            PersonList = db.Person.Include(p => p.FoundGeocaches);
            foreach (var p in PersonList)
            {
                Location location = new Location(p.Latitude, p.Longitude);
                string perToolTip = TooltipMessagePer(p);

                if (CurrentPerID == p.ID || CurrentPerID == 0)
                {
                    color = Colors.Blue;
                    opacity = 1;
                }
                else opacity = Opacity = 0.5;

                var pinPerson = AddPersonPin(location, perToolTip, color, Opacity, p);

                pinPerson.MouseDown += (s, a) =>
                 {
                     CurrentPerID = p.ID;
                     UpdateMap();
                     a.Handled = true;
                 };
            }
        }

        private void ColorGeoPin()
        {
            IEnumerable<Geocache> GeocacheList = new List<Geocache>();
            GeocacheList = db.Geocache.Include(geo => geo.Person).Include(geo => geo.FoundGeocaches);

            foreach (var g in GeocacheList)
            {
                Location location = new Location(g.Latitude, g.Longitude);
                string geoToolTip = TooltipMessageGeo(g);
                if (CurrentPerID == 0)
                {
                    color = Colors.Gray;
                }
                else if (g.Person.ID == CurrentPerID)
                {
                    color = Colors.Black;
                }
                else color = Colors.Red;

                var pinGeo = AddGeoPin(location, geoToolTip, color, 1, g);

                foreach (var fg in g.FoundGeocaches)
                {
                    if (fg.PersonID == CurrentPerID)
                    {
                        pinGeo = AddGeoPin(location, geoToolTip, Colors.Green, 1, g);
                    }
                }

                pinGeo.MouseDown += (s, a) =>
                 {
                     a.Handled = true;
                     Geocache currentGeo = new Geocache();
                     List<int> CurrentPersonFoundIDs = new List<int>();

                     currentGeo = db.Geocache.Include(geo => geo.Person)
                     .First(geo => geo.Latitude == location.Latitude && geo.Longitude == location.Longitude);
                     // To find out each geocache ID that the currently selected person has found and add all of them to a list.
                     CurrentPersonFoundIDs = db.FoundGeocache.Where(fg => fg.PersonID == CurrentPerID).Select(fg => fg.GeocacheID).ToList();


                 };
            }


        }

        private void UpdatePin(Pushpin pin, Color color, double opacity)
        {
            pin.Background = new SolidColorBrush(color);
            pin.Opacity = opacity;
        }

        //private Pushpin AddPin(GeoCoordinate loc, string tooltip, Color color, double opacity, object obj)
        //{
        //    var location = new Location { Latitude = loc.Latitude, Longitude = loc.Longitude };
        //    var pin = new Pushpin();
        //    pin.Cursor = Cursors.Hand;
        //    pin.Background = new SolidColorBrush(color);
        //    pin.Opacity = opacity;
        //    pin.Location = location;
        //    pin.Tag = obj;
        //    ToolTipService.SetToolTip(pin, tooltip);
        //    ToolTipService.SetInitialShowDelay(pin, 0);
        //    layer.AddChild(pin, location);
        //    return pin;
        //}
        #endregion

        private void OnMapLeftClick()
        {
            // Handle map click here.
            CurrentPerID = 0;
            currentPerson = null;
            UpdateMap();
        }

        private void OnAddGeocacheClick(object sender, RoutedEventArgs args)
        {

            if (currentPerson != null)
            {


                var dialog = new GeocacheDialog();
                dialog.Owner = this;
                dialog.ShowDialog();
                if (dialog.DialogResult == false)
                {


                    return;
                }

                double latitude = latestClickLocation.Latitude;
                double longitude = latestClickLocation.Longitude;
                string contents = dialog.GeocacheContents;
                string message = dialog.GeocacheMessage;


                Geocache geocache = new Geocache
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Contents = contents,
                    Message = message,
                    Person = currentPerson
                };

                db.Add(geocache);
                db.SaveChanges();

                // Add geocache to map and database here.
                string messageGeo = TooltipMessageGeo(geocache);
                pin.Add(AddGeoPin(latestClickLocation, messageGeo, Colors.Gray, 1, geocache));

                UpdateMap();

                //pin.MouseDown += (s, a) =>
                //{
                //    // Handle click on geocache pin here.


                //    MessageBox.Show("You clicked a geocache");
                //    UpdateMap();

                //    // Prevent click from being triggered on map.
                //    a.Handled = true;
                //};
            }
            else
            {
                MessageBox.Show("You need to select a person");
            }
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
            string firstName = dialog.PersonFirstName;
            string lastName = dialog.PersonLastName;
            double latitude = latestClickLocation.Latitude;
            double longitude = latestClickLocation.Longitude;
            string city = dialog.AddressCity;
            string country = dialog.AddressCountry;
            string streetName = dialog.AddressStreetName;
            int streetNumber = dialog.AddressStreetNumber;

            Person person = new Person
            {
                FirstName = firstName,
                LastName = lastName,
                Latitude = latitude,
                Longitude = longitude,
                City = city,
                Country = country,
                StreetName = streetName,
                StreetNumber = (short)streetNumber
            };
            db.Add(person);
            db.SaveChanges();

            string message = TooltipMessagePer(person);
            // Add person to map and database here.
            //var pin = AddPersonPin(latestClickLocation, message, Colors.Blue, person);
            Location location = new Location(person.Latitude, person.Longitude);
            pin.Add(AddPersonPin(location, message, color: Colors.Blue, 1, person));
            CurrentPerID = person.ID;
            currentPerson = person;
            UpdateMap();
            //pin.MouseDown += (s, a) =>
            //{
            //    // Handle click on person pin here.
            //    currentPerson = person;
            //    //MessageBox.Show("You clicked a person");
            //    //var locLat = latestClickLocation.Latitude;
            //    //var locLong = latestClickLocation.Longitude;
            //    //if (pinper.Location.Latitude == locLat && pinper.Location.Longitude == locLong)
            //    //{

            //    //}
            //    UpdateMap();

            //    // Prevent click from being triggered on map.
            //    a.Handled = true;
            //};
        }
        #region pin
        private Pin AddGeoPin(Location location, string tooltip, Color color, double opacity, Geocache geocache)
        {
            var pin = new Pin();
            pin.Cursor = Cursors.Hand;
            pin.Background = new SolidColorBrush(color);
            pin.Geocache = geocache;
            pin.Opacity = opacity;
            pin.Tag = geocache;
            ToolTipService.SetToolTip(pin, tooltip);
            ToolTipService.SetInitialShowDelay(pin, 0);
            layer.AddChild(pin, new Location(location.Latitude, location.Longitude));
            return pin;
        }

        private Pin AddPersonPin(Location location, string tooltip, Color color, double opacity, Person person)
        {
            var pin = new Pin();
            pin.Cursor = Cursors.Hand;
            pin.Background = new SolidColorBrush(color);
            pin.Person = person;
            pin.Opacity = opacity;
            pin.Tag = person;
            ToolTipService.SetToolTip(pin, tooltip);
            ToolTipService.SetInitialShowDelay(pin, 0);
            layer.AddChild(pin, new Location(location.Latitude, location.Longitude));
            return pin;
        }
        #endregion

        #region Save/Load from file

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

            db.ClearDatabase(db);
            db.ReadFromFile(path, db);

            UpdateMap();
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
            using (TextWriter tw = new StreamWriter(path))
            {
                List<string> fileToText = new List<string>();
                string foundGeo = "Found: ";
                //foreach (var item in Person) Hämta från databas först
                foreach (var person in db.Person.Include(p => p.FoundGeocaches))
                {

                    fileToText.Add(person.FirstName + " | " + person.LastName + " | " + person.Country + " | " + person.City + " | " + person.StreetName + " | " + person.StreetNumber + " | " + person.Latitude + " | " + person.Longitude);
                    foreach (var geocache in db.Geocache.Where(s => s.Person.ID == person.ID))
                    {
                        fileToText.Add(geocache.ID + " | " + geocache.Latitude + " | " + geocache.Longitude + " | " + geocache.Contents + " | " + geocache.Message);
                        //fileToText.Add("Found: " + person.FoundGeocaches.Select(fg => fg.GeocacheID));
                    }
                    foreach (var item in person.FoundGeocaches)
                    {

                        foundGeo += item.GeocacheID + " , ";
                    }
                    string formattedFG = foundGeo.Remove(foundGeo.Length - 3);
                    fileToText.Add(formattedFG);
                    fileToText.Add(Environment.NewLine);
                }
                foreach (var text in fileToText)
                {
                    tw.WriteLine(text);
                }
            }


        }
        #endregion

    }
}
