using RaceBike.Model.Classes;
using RaceBike.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model
{
    public class RaceBikeModel
    {
        #region Properties
        public TimeSpan LatestBestTime { get => _statistics.LatestBestTime; set => _statistics.LatestBestTime = value; }
        public int LoadedBikePosition => _statistics.BikePosition;
        public ImmutableArray<SimplePoint> LoadedFuels => _statistics.FuelPositions.ToImmutableArray();
        public TimeSpan CurrentTime => _stopwatch.Elapsed;
        public ImmutableSpeed CurrentSpeed => _bike.Speed;
        public int CurrentTankLevel => CalculateTankLevelPercent();
        public bool IsPaused => !_stopwatch.IsRunning;
        public bool IsGameOver => _bike.IsOutOfGas;
        #endregion

        #region Private fields
        private readonly Bike _bike;
        private readonly Queue<Fuel> _fuels;
        private readonly IRaceBikeDataAccess? _dataAccess;
        private readonly SmartStopwatch _stopwatch;
        private RaceBikeRecordStats _statistics;
        //private TimeSpan latestBestTime;
        #endregion

        #region Public events
        public event EventHandler? GameContinues;
        public event EventHandler? GameOver;
        #endregion

        #region Constructors
        public RaceBikeModel(IRaceBikeDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _stopwatch = new SmartStopwatch();
            _bike = new Bike();
            _fuels = new Queue<Fuel>();
            _statistics = new RaceBikeRecordStats();
        }
        #endregion

        #region Public methods
        public void Reset()
        {
            _bike.Reset();
            _fuels.Clear();
            _stopwatch.Restart(); // megj.: .Reset() != .Restart()
        }

        public async Task LoadGameAsync(string path)
        {
            if (_dataAccess is null)
            {
                throw new InvalidOperationException("No data access was provided.");
            }

            _statistics.FuelPositions.Clear();
            _statistics = await _dataAccess.LoadAsync(path);

            _bike.SetSpeed(_statistics.Speed);
            _stopwatch.Add(_statistics.LatestBestTime);
        }

        public void SaveGame(string path, int bikePos, Queue<SimplePoint> points)
        {

            _statistics.Speed = _bike.Speed;
            _statistics.BikePosition = bikePos;
            _statistics.FuelPositions.Clear();

            foreach (SimplePoint p in points)
            {
                _statistics.FuelPositions.Enqueue(p);
            }

            _dataAccess?.SaveAsync(path, _statistics);
        }

        public void GameTimeElapsing()
        {
            if (_bike.IsOutOfGas)
            {
                _stopwatch.Stop();

                if (_stopwatch.Elapsed > LatestBestTime)
                {
                    LatestBestTime = _stopwatch.Elapsed;
                }

                OnGameOver();
            }
            else
            {
                DecreaseTankLevel();
                OnGameContinues();
            }
        }

        public void GameTimePauseResume()
        {
            if (IsPaused)
            {
                _stopwatch.Start();
            }
            else
            {
                _stopwatch.Stop();

                if (_stopwatch.Elapsed > _statistics.LatestBestTime)
                {
                    _statistics.LatestBestTime = _stopwatch.Elapsed;
                }
            }
        }

        public void IncreaseTankLevel()
        {
            if (_fuels.Count != 0)
            {
                _bike.IncreaseTankLevel(_fuels.Dequeue()); // Peek() != Dequeue(); mivel Dequeue() jelenti a pop() műveletet
            }
        }

        public void GenerateNewFuelItem() { _fuels.Enqueue(new Fuel()); } // Enqueue() == push()

        public void LoseFuelItem()
        {
            if (_fuels.Count != 0)
            {
                _fuels.Dequeue();
            }
        }

        public void SpeedUp() { _bike.SpeedUp(); }

        public void SlowDown() { _bike.SlowDown(); }
        #endregion

        #region Private methods
        private void DecreaseTankLevel() { _bike.DecreaseTankLevel(); }

        private int CalculateTankLevelPercent()
        {
            double percent = (double)_bike.TankLevel / _bike.MaxCapacity * 100;
            return (int)percent;
        }
        #endregion

        #region Private event methods
        private void OnGameContinues()
        {
            GameContinues?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
