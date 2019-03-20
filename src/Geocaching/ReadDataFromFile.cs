using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Geocaching;

namespace Geocaching
{
    private AppDbContext db = new AppDbContext();




    public class ReadDataFromFile
    {

        ClearDatabase();
        PopulateDatabase();

        private static void ClearDatabase()
        {

        }

        private static void PopulateDatabase()
        {

        }


        private static Dictionary<int, Person> ReadPerson(Dictionary<int, Geocache> geocache)
        {
            var songs = new Dictionary<int, Song>();

            string[] lines = File.ReadAllLines("Geocaches.txt").Skip(1).ToArray();
            foreach (string line in lines)
            {
                try
                {
                    string[] values = line.Split('|').Select(v => v.Trim()).ToArray();

                    int id = int.Parse(values[0]);
                    byte trackNumber = byte.Parse(values[1]);
                    string title = values[2];

                    string[] lengthParts = values[3].Split(':');
                    int minutes = int.Parse(lengthParts[0]);
                    int seconds = int.Parse(lengthParts[1]);
                    Int16 length = Convert.ToInt16(minutes * 60 + seconds);

                    bool hasMusicVideo;
                    if (values[4].ToUpper() == "Y") hasMusicVideo = true;
                    else if (values[4].ToUpper() == "N") hasMusicVideo = false;
                    else throw new FormatException("Boolean string must be either Y or N.");

                    int albumId = int.Parse(values[5]);

                    // If there are lyrics, add them, otherwise let them be null.
                    string lyrics = null;
                    if (values.Length == 7)
                    {
                        lyrics = values[6];
                    }

                    songs[id] = new Song
                    {
                        TrackNumber = trackNumber,
                        Title = title,
                        Length = length,
                        HasMusicVideo = hasMusicVideo,
                        Lyrics = lyrics,
                        Album = albums[albumId]
                    };
                }
                catch
                {
                    Console.WriteLine("Could not read song: " + line);
                }
            }

            return songs;
        }

    }



}
