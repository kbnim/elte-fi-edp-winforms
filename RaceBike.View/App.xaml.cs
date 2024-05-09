using Microsoft.Win32;
using RaceBike.Model;
using RaceBike.Persistence;
using RaceBike.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace RaceBike.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IRaceBikeDataAccess _dataAccess = null!;
        private RaceBikeModel _model = null!;

        private MainViewModel _gameViewModel = null!;
        private MainMenuViewModel _menuViewModel = null!;

        private MainWindow _gameWindow = null!;
        private MenuWindow _menuWindow = null!;

        private OpenFileDialog? _openFileDialog;
        private SaveFileDialog? _saveFileDialog;

        private DispatcherTimer _gameTimer;
        private DispatcherTimer _fuelTimer;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // init layers of persistence and model
            _dataAccess = new RaceBikeTxtAccess();
            _model = new RaceBikeModel(_dataAccess);

            // init view and viewmodel (game window)
            _gameViewModel = new MainViewModel(_model);
            _gameWindow = new MainWindow();
            _gameWindow.DataContext = _gameViewModel;
            _gameWindow.Show();

            // init view and viewmodel (menu window)
            _menuViewModel = new MainMenuViewModel();
            _menuViewModel.NewResumeContinueButtonCommand += MenuViewModel_NewResumeContinueButtonCommand;
            _menuViewModel.LoadButtonCommand += MenuViewModel_LoadButtonCommand;
            _menuViewModel.SaveButtonCommand += MenuViewModel_SaveButtonCommand;
            _menuViewModel.HelpButtonCommand += MenuViewModel_HelpButtonCommand;
            _menuViewModel.QuitButtonCommand += MenuViewModel_QuitButtonCommand;
            _menuWindow = new MenuWindow();
            _menuWindow.DataContext = _menuViewModel;
            _menuWindow.Show();
            
            // init game timer
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = new TimeSpan(20);
            _gameTimer.Tick += GameTimer_Tick;

            // init fuel timer
            _fuelTimer = new DispatcherTimer();
            _fuelTimer.Interval = new TimeSpan(2000);
            _fuelTimer.Tick += FuelTimer_Tick;
        }

        private void MenuViewModel_QuitButtonCommand(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuViewModel_HelpButtonCommand(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuViewModel_SaveButtonCommand(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuViewModel_LoadButtonCommand(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MenuViewModel_NewResumeContinueButtonCommand(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #region Timer event handlers
        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            _model.GameTimeElapsing();
        }

        private void FuelTimer_Tick(object? sender, EventArgs e)
        {
            _model.GenerateNewFuelItem();
            //_gamePanel.Controls.Add(new FuelPictureBox(_gamePanel.Width));
        }
        #endregion
    }

}
