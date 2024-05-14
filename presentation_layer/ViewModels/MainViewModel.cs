﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using presentation_layer.Models;
using data_layer;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace presentation_layer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private SimulationModel _Simulation_Model;
        private String _Amount_Of_Balls;
        private DispatcherTimer _Update_Timer;
        private bool _Is_Start_Button_Active;
        private bool _Is_Stop_Button_Active;
        private bool _Is_Text_Field_Active;
        public ICommand Start_Simulation_Command { get; }
        public ICommand Stop_Simulation_Command { get; }

        // hard coded value of Billard Table size
        private int _Billard_Table_Width = 1000;
        private int _Billard_Table_Height = 500;
        public int Billard_Table_Width => _Billard_Table_Width;
        public int Billard_Table_Height => _Billard_Table_Height;

        private ObservableCollection<BallWithTimer> _ballsWithTimer = new ObservableCollection<BallWithTimer>();
        public ObservableCollection<BallWithTimer> BallsWithTimer => _ballsWithTimer;

        public MainViewModel()
        {
            Start_Simulation_Command = new RelayCommand(Start_Simulation, () => _Is_Start_Button_Active);
            Stop_Simulation_Command = new RelayCommand(Stop_Simulation, () => Is_Stop_Button_Enable);
            _Simulation_Model = new SimulationModel(_Billard_Table_Width, _Billard_Table_Height);
            _Amount_Of_Balls = "47";

            Is_Start_Button_Enable = true;
            Is_Stop_Button_Enable = false;
            Is_Text_Field_Enable = true;

            _Update_Timer = new DispatcherTimer();
            _Update_Timer.Interval = TimeSpan.FromMilliseconds(1);
            _Update_Timer.Tick += Update_Simulation;
        }

        public string Amount_Of_Balls
        {
            get => _Amount_Of_Balls;
            set
            {
                _Amount_Of_Balls = value;
                Is_Start_Button_Enable = int.TryParse(value, out int number) && number > 0;
                NotifyPropertyChanged();
                NotifyPropertyChanged("CanStart");
            }
        }

        public void Start_Simulation()
        {
            Is_Start_Button_Enable = false;
            Is_Stop_Button_Enable = true;
            Is_Text_Field_Enable = false;
            _Simulation_Model.GenerateBalls(int.Parse(Amount_Of_Balls));

            foreach (Ball ball in _Simulation_Model.GetBalls()) {
                _ballsWithTimer.Add(new BallWithTimer(ball,Billard_Table_Width,Billard_Table_Height));
            }
            NotifyPropertyChanged("CanStart");
            NotifyPropertyChanged("CanStop");
            NotifyPropertyChanged("CanEdit");
        }
        public void Stop_Simulation()
        {
            Is_Start_Button_Enable = true;
            Is_Stop_Button_Enable = false;
            Is_Text_Field_Enable = true;

            foreach (BallWithTimer ballWithTimer in _ballsWithTimer) {
                ballWithTimer.Stop();
            }
            _ballsWithTimer.Clear();
            _Simulation_Model.ClearAllBalls();
            NotifyPropertyChanged("CanStart");
            NotifyPropertyChanged("CanStop");
            NotifyPropertyChanged("CanEdit");
        }
        private void Update_Simulation(object sender, EventArgs e)
        {
            _Simulation_Model.UpdateBalls();
            NotifyPropertyChanged("Balls");
        }
        //public IBall[]? Balls => _Simulation_Model.GetBalls().ToArray();
        public bool Is_Start_Button_Enable
        {
            get => _Is_Start_Button_Active;
            set
            {
                _Is_Start_Button_Active = value;
                NotifyPropertyChanged();
            }
        }

        public bool Is_Stop_Button_Enable
        {
            get => _Is_Stop_Button_Active;
            set
            {
                _Is_Stop_Button_Active = value;
                NotifyPropertyChanged();
            }
        }

        public bool Is_Text_Field_Enable
        {
            get => _Is_Text_Field_Active;
            set
            {
                _Is_Text_Field_Active = value;
                NotifyPropertyChanged();
            }
        }
    }


}
