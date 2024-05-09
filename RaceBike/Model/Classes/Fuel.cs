using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public class Fuel
    {
        #region Properties
        public int Volume { get; private set; }
        #endregion

        #region Constructors
        public Fuel(int volume = 100)
        {
            if (volume <= 0) throw new EmptyOrNegativeVolumeException();
            Volume = volume;
        }
        #endregion

        #region Exceptions
        public class EmptyOrNegativeVolumeException : Exception { }
        #endregion
    }
}
