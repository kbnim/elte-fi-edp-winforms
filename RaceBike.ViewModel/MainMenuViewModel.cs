using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.ViewModel
{
    public class MainMenuViewModel : ViewModelBase
    {
        private string _titleText;
        private string _description01Text;
        private string _description02Text;
        private string _newResumeText;
        private bool _saveButtonEnablement;

        public string TitleText
        {
            get { return _titleText; }
            set
            {
                _titleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }

        public string Description01Text
        {
            get { return _description01Text; }
            set
            {
                _description01Text = value;
                OnPropertyChanged(nameof(Description01Text));
            }
        }

        public string Description02Text
        {
            get { return _description02Text; }
            set
            {
                _description02Text = value;
                OnPropertyChanged(nameof(Description02Text));
            }
        }

        public string NewResumeText
        {
            get { return _newResumeText; }
            set
            {
                _newResumeText = value;
                OnPropertyChanged(nameof(NewResumeText));
            }
        }

        public bool SaveButtonEnablement
        {
            get { return _saveButtonEnablement; }
            set
            {
                _saveButtonEnablement = value;
                OnPropertyChanged(nameof(SaveButtonEnablement));
            }
        }

        public event EventHandler? NewResumeContinueButtonCommand;
        public event EventHandler? LoadButtonCommand;
        public event EventHandler? SaveButtonCommand;
        public event EventHandler? HelpButtonCommand;
        public event EventHandler? QuitButtonCommand;

        public MainMenuViewModel()
        {
            _titleText = "RaceBike 2000";
            _description01Text = "Press 'space' or";
            _description02Text = "click to start";
            _newResumeText = "New";
            _saveButtonEnablement = false;
        }

        private void OnNewResumeContinueButtonCommand()
        {
            NewResumeContinueButtonCommand?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadButtonCommand()
        {
            LoadButtonCommand?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveButtonCommand()
        {
            SaveButtonCommand?.Invoke(this, EventArgs.Empty);
        }

        private void OnHelpButtonCommand()
        {
            HelpButtonCommand?.Invoke(this, EventArgs.Empty);
        }

        private void OnQuitButtoneCommand()
        {
            QuitButtonCommand?.Invoke(this, EventArgs.Empty);
        }

    }
}
