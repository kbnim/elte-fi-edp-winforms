using RaceBike.Model.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model
{
    public struct SimplePoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public SimplePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override readonly string ToString()
        {
            return string.Format($"({ X }, { Y })");
        }

        public static SimplePoint Parse(string s)
        {
            int x, y;
            int i = 0;

            while (char.IsWhiteSpace(s[i]) && i < s.Length) i++;

            if (s[i] != '(') throw new FormatException("Character '(' was not found");
            i++;

            var builder = new StringBuilder();

            while (s[i] != ',' && i < s.Length)
            {
                builder.Append(s[i]);
                i++;
            }

            try
            {
                x = Convert.ToInt32(builder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            builder.Clear();

            while ((s[i] == ',' || char.IsWhiteSpace(s[i])) && i < s.Length) i++;

            while (s[i] != ')' && i < s.Length)
            {
                builder.Append(s[i]);
                i++;
            }

            try
            {
                y = Convert.ToInt32(builder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return new SimplePoint(x, y);
        }
    }

    public struct RaceBikeRecordStats
    {
        public TimeSpan LatestBestTime { get; set; }
        public ImmutableSpeed Speed { get; set; }
        public int BikePosition { get; set; }
        public Queue<SimplePoint> FuelPositions { get; set; }

        public RaceBikeRecordStats()
        {
            LatestBestTime = TimeSpan.Zero;
            Speed = new ImmutableSpeed();
            BikePosition = (480 / 2) - (40 / 2);
            FuelPositions = new Queue<SimplePoint>();
        }
    }
}
