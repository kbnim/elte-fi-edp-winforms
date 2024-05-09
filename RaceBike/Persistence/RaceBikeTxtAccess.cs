using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Model;
using RaceBike.Model.Classes;

namespace RaceBike.Persistence
{
    public class RaceBikeTxtAccess : IRaceBikeDataAccess
    {
        #region Public methods
        public async Task<RaceBikeRecordStats> LoadAsync(string path)
        {
            try
            {
                using StreamReader reader = new(path); // fájl megnyitása
                RaceBikeRecordStats fileContents = new();

                List<string> lines = new();

                while (!reader.EndOfStream)
                {
                    lines.Add(await reader.ReadLineAsync() ?? string.Empty);
                }

                try
                {
                    var info = new DateTimeFormatInfo();
                    // time
                    fileContents.LatestBestTime = TimeSpan.Parse(lines[0], new DateTimeFormatInfo());
                    // speed
                    fileContents.Speed = ImmutableSpeed.Parse(lines[1]);
                    // bike's coordinates
                    fileContents.BikePosition = Convert.ToInt32(lines[2]);
                }
                catch (Exception ex)
                {
                    throw new RaceBikeDataException(ex.Message, ex);
                }

                // read remaining fuel items

                for (int i = 3; i < lines.Count; i++)
                {
                    try
                    {
                        fileContents.FuelPositions.Enqueue(SimplePoint.Parse(lines[i]));
                    }
                    catch (Exception ex)
                    {
                        throw new RaceBikeDataException(ex.Message, ex);
                    }
                }

                return fileContents;
            }
            catch (Exception ex)
            {
                throw new RaceBikeDataException(ex.Message, ex);
            }
        }

        public async Task SaveAsync(string path, RaceBikeRecordStats fileContents)
        {
            try
            {
                using StreamWriter writer = new(path);
                await writer.WriteLineAsync(fileContents.LatestBestTime.ToString());
                await writer.WriteLineAsync(fileContents.Speed.ToString());
                await writer.WriteLineAsync(fileContents.BikePosition.ToString());

                foreach (SimplePoint point in fileContents.FuelPositions)
                {
                    await writer.WriteLineAsync(point.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new RaceBikeDataException(ex.Message, ex);
            }
        }
        #endregion
    }
}
