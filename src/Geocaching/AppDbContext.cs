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

            modelBuilder.Entity<Geocache>(b =>
            {
                b.HasKey(e => e.ID);
                b.Property(e => e.ID).ValueGeneratedOnAdd();
            }); ;
        }

        public void ClearDatabase(AppDbContext db)
        {
            db.Geocache.RemoveRange(db.Geocache);
            db.FoundGeocache.RemoveRange(db.FoundGeocache);
            foreach (var p in db.Person)
            {

            }
            db.Person.RemoveRange(db.Person.ToArray());
            try
            {
                db.SaveChanges();
            }
            catch { }
        }

        public void ReadFromFile(string path, AppDbContext db)
        {
            var person = new List<Person>();
            var found = new Dictionary<Person, int[]>();
            var geocache = new Dictionary<int, Geocache>();

            found.Clear();

            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                string[] split = line.Split('|').Select(v => v.Trim()).ToArray();

                if (split[0] != "")
                {

                    if (split.Length == 8)
                    {
                        string firstName = split[0];
                        string lastName = split[1];
                        string country = split[2];
                        string city = split[3];
                        string streetName = split[4];
                        Int16 streetNumber = Int16.Parse(split[5]);
                        double latitude = double.Parse(split[6]);
                        double longitude = double.Parse(split[7]);

                        var newPerson = new Person
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




                        if (person.Count == 0)
                        {
                            person.Add(newPerson);

                        }
                        else
                        {
                            person.Clear();
                            person.Add(newPerson);
                        }
                    }

                    else if (split.Length == 5)
                    {
                        int id = int.Parse(split[0]);
                        double latitude = double.Parse(split[1]);
                        double longitude = double.Parse(split[2]);
                        string contents = split[3];
                        string message = split[4];

                        var newGeo = new Geocache
                        {
                            Latitude = latitude,
                            Longitude = longitude,
                            Contents = contents,
                            Message = message,
                            Person = person[0]

                        };

                        geocache.Add(id, newGeo);

                    }
                    else
                    {
                        string foundString = split[0].Substring(6);

                        int[] intSplit = foundString.Split(',').Select(s => int.Parse(s.Trim())).ToArray();

                        if (foundString != "")
                        {
                            found.Add(person[0], intSplit);


                        }
                        else
                        {
                            db.Add(person[0]);


                        }

                    }
                }



            }

            foreach (var item in found)
            {
                foreach (var foundGeo in item.Value)
                {
                    var newfg = new FoundGeocache()
                    {
                        Person = item.Key,
                        Geocache = geocache.Where(s => s.Key == foundGeo).Select(fg => fg.Value).First()
                    };
                    db.Add(newfg);
                    try
                    {
                    db.SaveChanges();
                    }
                    catch
                    {
                    }
                }
            }
        }


    }
}

