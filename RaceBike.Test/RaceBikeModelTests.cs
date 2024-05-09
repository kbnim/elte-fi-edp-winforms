using Moq;
using RaceBike.Model;
using RaceBike.Model.Classes;
using RaceBike.Persistence;

namespace RaceBike.Test
{
    [TestClass]
    public class RaceBikeModelTests
    {
        private RaceBikeModel _model = null!;
        private TimeSpan _timeSpan;
        private Mock<IRaceBikeDataAccess> _mock = null!;

        [TestInitialize]
        public void Initialize()
        {
            _timeSpan = TimeSpan.Zero;

            _mock = new Mock<IRaceBikeDataAccess>();
            
            _model = new RaceBikeModel(_mock.Object);
            _model.GameContinues += Model_GameContinues;
            _model.GameOver += Model_GameOver;
        }

        private async Task LoadFileContents(string contents)
        {
            _timeSpan = TimeSpan.Parse(contents);
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>()))
                 .Returns(() => Task.FromResult(_timeSpan));
            await _model.LoadGameAsync(string.Empty);
        }

        [TestMethod]
        public async Task ValidInputFileTest()
        {
            await LoadFileContents("00:00:30.8101404");
            Assert.AreEqual(_timeSpan, _model.LatestBestTime);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public async Task InvalidInputFileTest01()
        {
            await LoadFileContents("00:00:30.8101404û");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public async Task InvalidInputFileTest02()
        {
            await LoadFileContents(string.Empty);
        }

        [TestMethod]
        public void GamePauseResumeTest01()
        {
            _model.GameTimePauseResume(); // start
            _model.GameTimePauseResume(); // stop
            Assert.AreNotEqual(TimeSpan.Zero, _model.CurrentTime);
            Assert.AreNotEqual(_model.LatestBestTime, _model.CurrentTime);
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
        }

        [TestMethod]
        public async Task GamePauseResumeTest02()
        {
            await LoadFileContents("00:00:00.0000001");
            _model.GameTimePauseResume(); // start
            _model.GameTimePauseResume(); // stop
            Assert.AreNotEqual(TimeSpan.Zero, _model.CurrentTime);
            Assert.AreNotEqual(_model.LatestBestTime, _model.CurrentTime);
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
        }

        [TestMethod]
        public void ChangeSpeedTest()
        {
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
            Assert.AreEqual("Slow", _model.CurrentSpeed.ToString());
            _model.SpeedUp();
            Assert.AreEqual(2, (int)_model.CurrentSpeed);
            Assert.AreEqual("Medium", _model.CurrentSpeed.ToString());
            _model.SpeedUp();
            Assert.AreEqual(3, (int)_model.CurrentSpeed);
            Assert.AreEqual("Fast", _model.CurrentSpeed.ToString());
            _model.SpeedUp();
            Assert.AreEqual(3, (int)_model.CurrentSpeed);
            Assert.AreEqual("Fast", _model.CurrentSpeed.ToString());
            _model.SlowDown();
            Assert.AreEqual(2, (int)_model.CurrentSpeed);
            Assert.AreEqual("Medium", _model.CurrentSpeed.ToString());
            _model.SlowDown();
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
            Assert.AreEqual("Slow", _model.CurrentSpeed.ToString());
            _model.SlowDown();
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
            Assert.AreEqual("Slow", _model.CurrentSpeed.ToString());
        }

        [TestMethod]
        public void RunningOutOfGasTest()
        {
            Assert.IsTrue(_model.IsPaused);
            _model.GameTimePauseResume(); // resumes, i.e. starts the game

            while (!_model.IsGameOver)
            {
                // Assert.IsFalse(_model.IsGameOver);
                _model.GameTimeElapsing();
            }

            Assert.AreEqual(0, _model.CurrentTankLevel);
            Assert.IsTrue(_model.IsGameOver);
            Assert.IsFalse(_model.IsPaused);
        }

        [TestMethod]
        public void FuelConsumption()
        {
            Assert.IsTrue(_model.IsPaused);
            _model.GameTimePauseResume();

            for (int i = 0; i < 100; i++)
            {
                _model.GameTimeElapsing();
            }

            _model.GenerateNewFuelItem();
            _model.IncreaseTankLevel();

            Assert.AreEqual(100, _model.CurrentTankLevel);
            Assert.IsFalse(_model.IsGameOver);
            Assert.IsFalse(_model.IsPaused);
        }

        [TestMethod]
        public void FuelConsumptionEmptyQueue()
        {
            for (int i = 0; i < 100; i++) // (1000 - 100) ÷ 10 = 90
            {
                _model.GameTimeElapsing();
            }

            Assert.AreEqual(90, _model.CurrentTankLevel);
            _model.IncreaseTankLevel(); // no fuel items have been generated
            Assert.AreEqual(90, _model.CurrentTankLevel);
        }

        [TestMethod]
        public void LoseFuelTest()
        {
            for (int i = 0; i < 100; i++) // (1000 - 100) ÷ 10 = 90
            {
                _model.GameTimeElapsing();
            }

            int levelBefore = _model.CurrentTankLevel;
            _model.GenerateNewFuelItem();
            _model.GenerateNewFuelItem();
            _model.LoseFuelItem(); // loses the first
            _model.IncreaseTankLevel(); // catches the last
            Assert.AreNotEqual(levelBefore, _model.CurrentTankLevel);
        }

        [TestMethod]
        public void LoseEachFuelTest()
        {
            for (int i = 0; i < 100; i++) // (1000 - 100) ÷ 10 = 90
            {
                _model.GameTimeElapsing();
            }

            int levelBefore = _model.CurrentTankLevel;
            _model.GenerateNewFuelItem();
            _model.LoseFuelItem(); // loses the first
            _model.IncreaseTankLevel(); // nothing to catch
            Assert.AreEqual(levelBefore, _model.CurrentTankLevel);
        }

        [TestMethod]
        public void TestReset()
        {
            _model.GameTimePauseResume(); // start

            for (int i = 0; i < 100; i++)
            {
                _model.SpeedUp();
                _model.GameTimeElapsing();
            }

            TimeSpan current = _model.CurrentTime;
            ImmutableSpeed speed = _model.CurrentSpeed;
            int level = _model.CurrentTankLevel;
            _model.Reset();
            Assert.AreNotEqual(current, _model.CurrentTime);
            Assert.AreNotEqual(speed, _model.CurrentSpeed);
            Assert.AreNotEqual(level, _model.CurrentTankLevel);
        }

        private void Model_GameOver(object? sender, EventArgs e)
        {
            Assert.IsTrue(_model.IsPaused);
        }

        private void Model_GameContinues(object? sender, EventArgs e)
        {
            Assert.IsTrue(_model.CurrentTime >= TimeSpan.Zero);
        }
    }
}