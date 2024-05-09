using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaceBike.Model;

namespace RaceBike.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Private fields
        private readonly RaceBikeModel _model;
        #endregion

        #region Properties of the model
        public string CurrentTime => _model.CurrentTime.ToString("mm\\:ss\\.ff");
        public string LatestBestTime => _model.LatestBestTime.ToString("mm\\:ss\\.ff");
        public int TankStatus => _model.CurrentTankLevel;
        public string CurrentSpeed => _model.CurrentSpeed.ToString();
        public bool IsPaused => _model.IsPaused;
        public bool IsGameOver => _model.IsGameOver;
        public int LoadedBikePosition => _model.LoadedBikePosition;
        public ImmutableArray<SimplePoint> LoadedFuels => _model.LoadedFuels;
        #endregion

        
        #region Delegate commands of the model
        public DelegateCommand GameContinuesCommand { get; private set; }
        public DelegateCommand GameOverCommand { get; private set; }
        #endregion

        #region Event handlers
        public event EventHandler? Model_GameContinues;
        public event EventHandler? Model_GameOver;
        #endregion

        #region Constructor
        public MainViewModel(RaceBikeModel model)
        {
            _model = model;
            // GameContinuesCommand = new DelegateCommand(param => _model);
        }
        #endregion
    }
}
