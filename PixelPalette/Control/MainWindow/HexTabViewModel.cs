using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PixelPalette.Control.MainWindow
{
    public sealed class HexTabViewModel : INotifyPropertyChanged
    {
        public GlobalState GlobalState { get; }

        private string _hexText = "";
        private string _hexRed = "";
        private string _hexGreen = "";
        private string _hexBlue = "";

        private LinearGradientBrush? _redGradientFill;
        private LinearGradientBrush? _greenGradientFill;
        private LinearGradientBrush? _blueGradientFill;

        public HexTabViewModel(GlobalState globalState)
        {
            GlobalState = globalState;
            GlobalState.PropertyChanged += (_, ev) =>
            {
                if (ev.PropertyName == "Hex") RefreshValues();
            };
            RefreshValues();
        }

        public string HexText
        {
            get => _hexText;
            set => SetField(ref _hexText, value);
        }

        public string HexRed
        {
            get => _hexRed;
            set => SetField(ref _hexRed, value);
        }

        public string HexGreen
        {
            get => _hexGreen;
            set => SetField(ref _hexGreen, value);
        }

        public string HexBlue
        {
            get => _hexBlue;
            set => SetField(ref _hexBlue, value);
        }

        public LinearGradientBrush? RedGradientFill
        {
            get => _redGradientFill;
            private set => SetField(ref _redGradientFill, value);
        }

        public LinearGradientBrush? GreenGradientFill
        {
            get => _greenGradientFill;
            private set => SetField(ref _greenGradientFill, value);
        }

        public LinearGradientBrush? BlueGradientFill
        {
            get => _blueGradientFill;
            private set => SetField(ref _blueGradientFill, value);
        }

        private void RefreshValues()
        {
            _isUserUpdate = false;

            HexText = GlobalState.Hex.ToString();
            HexRed = GlobalState.Hex.RedPart;
            HexGreen = GlobalState.Hex.GreenPart;
            HexBlue = GlobalState.Hex.BluePart;

            var redGradientFill = Window.MainWindow.NewBrush();
            var greenGradientFill = Window.MainWindow.NewBrush();
            var blueGradientFill = Window.MainWindow.NewBrush();

            redGradientFill.GradientStops.Add(new GradientStop(GlobalState.Rgb.WithRed(0.0).ToMediaColor(), 0.0));
            redGradientFill.GradientStops.Add(new GradientStop(GlobalState.Rgb.WithRed(1.0).ToMediaColor(), 1.0));
            greenGradientFill.GradientStops.Add(new GradientStop(GlobalState.Rgb.WithGreen(0.0).ToMediaColor(), 0.0));
            greenGradientFill.GradientStops.Add(new GradientStop(GlobalState.Rgb.WithGreen(1.0).ToMediaColor(), 1.0));
            blueGradientFill.GradientStops.Add(new GradientStop(GlobalState.Rgb.WithBlue(0.0).ToMediaColor(), 0.0));
            blueGradientFill.GradientStops.Add(new GradientStop(GlobalState.Rgb.WithBlue(1.0).ToMediaColor(), 1.0));

            RedGradientFill = redGradientFill;
            GreenGradientFill = greenGradientFill;
            BlueGradientFill = blueGradientFill;

            _isUserUpdate = true;
        }

#region boilerplate

        private bool _isUserUpdate = true;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<PropertyChangedEventArgs>? PropertyChangedByUser;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (_isUserUpdate) PropertyChangedByUser?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }

#endregion
    }
}