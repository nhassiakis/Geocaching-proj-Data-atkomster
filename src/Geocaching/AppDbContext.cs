using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Geocaching.Models;
using System.IO;

namespace Geocaching
{
    //public AppDbContext db = new AppDbContext();

    public class AppDbContext : DbContext
    {
        public DbSet<Person> Person { get; set; }
        public DbSet<Geocache> Geocache { get; set; }
        public DbSet<FoundGeocache> FoundGeocache { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(local)\SQLEXPRESS;Initial Catalog=Geocaching;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FoundGeocache>()
                .HasKey(fg => new { fg.PersonID, fg.GeocacheID });
            modelBuilder.Entity<FoundGeocache>()
                .HasOne(fg => fg.Geocache)
                .WithMany(g => g.FoundGeocaches)
                .HasForeignKey(fg => fg.GeocacheID);
            modelBuilder.Entity<FoundGeocache>()
                .HasOne(fg => fg.Person)
                .WithMany(p => p.FoundGeocaches)
                .HasForeignKey(fg => fg.PersonID);
        }

        //private static void ClearDatabase(AppDbContext database)
        //{
        //    database.
        //    database.Song.RemoveRange(database.Song);
        //    database.Album.RemoveRange(database.Album);
        //    database.Artist.RemoveRange(database.Artist);
        //    database.SaveChanges();
        //}

        public void ReadFromFile(string path)
        {
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

                        if (line.StartsWith("Found:"))
                        {
                            char[] trimString = { 'F', 'o', 'u', 'n', 'd', ':', ' ' };
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

        public void PopulateDatabase(AppDbContext db, string path)
        {
            var person = ReadPerson(path);
            var geocache = ReadGeocahe(path);
            var foundGeocache = ReadFoundGeoCache(path);
            foreach (var p in person)
            {
                db.Add(p.Value);
                db.SaveChanges();
            }
        }


        private static Dictionary<int, FoundGeocache> ReadFoundGeoCache(string path)
        {
            var foundGeocache = new Dictionary<int, FoundGeocache>();
            int counter = 0;


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

                        if (line.StartsWith("Found:"))
                        {
                            char[] trimString = { 'F', 'o', 'u', 'n', 'd', ':', ' ' };
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
                    }

                }

                catch
                {
                    Console.WriteLine("Could not read file." + line);
                }
            }
                return foundGeocache;

        }
        private static Dictionary<int, Geocache> ReadGeocahe(string path)
        {
            var geocache = new Dictionary<int, Geocache>();


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
                return geocache;
        }

        private static Dictionary<int, Person> ReadPerson(string path)
        {
            int counter = 0;

            var person = new Dictionary<int, Person>();

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
                        if (line.StartsWith("Found:"))
                        {

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

                            counter++;
                        }
                    }
                }


                catch
                {
                    Console.WriteLine("Could not read file." + line);
                }


            }
                return person;
        }

    }
}
