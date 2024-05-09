using RaceBike.Model;
using RaceBike.Model.Classes;

namespace RaceBike
{
    internal class Program
    {
        static void Main()
        {
            string s = "    (46, 36)  \n";
            SimplePoint p = SimplePoint.Parse(s);
            Console.WriteLine(p);
            string s1 = "Medium";
            ImmutableSpeed speed = ImmutableSpeed.Parse(s1);
            Console.WriteLine(speed);
        }
    }
}