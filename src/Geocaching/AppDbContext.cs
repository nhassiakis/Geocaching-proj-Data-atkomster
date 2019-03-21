using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Geocaching.Models;

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

        //private static void PopulateDatabase()
        //{
        //    var artists = ReadArtists();
        //    var albums = ReadAlbums(artists);
        //    var songs = ReadSongs(albums);
        //    foreach (var song in songs.Values)
        //    {
        //        database.Add(song);
        //        database.SaveChanges();
        //    }
        //}




    }
}
