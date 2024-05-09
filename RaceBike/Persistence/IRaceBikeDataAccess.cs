using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Model;

namespace RaceBike.Persistence
{
    public interface IRaceBikeDataAccess
    {
        #region Methods
        Task<RaceBikeRecordStats> LoadAsync(string path);
        Task SaveAsync(string path, RaceBikeRecordStats fileContents);
        #endregion
    }
}
