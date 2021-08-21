using System.Windows;
using System.Windows.Input;
using PixelPalette.Color;

namespace PixelPalette.Control.MainWindow
{
    public partial class HexTabContent
    {
        public GlobalState? GlobalState
        {
            get => GetValue(GlobalStateProperty) as GlobalState;
            set => SetValue(GlobalStateProperty, value);
        }

        public static readonly DependencyProperty GlobalStateProperty =
            DependencyProperty.Register(
                "GlobalState",
                typeof(GlobalState),
                typeof(HexTabContent),
                new FrameworkPropertyMetadata(GlobalState.Instance)
            );

        public HexTabContent()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {
                if (GlobalState == null) return;

                var vm = new HexTabViewModel(GlobalState);
                DataContext = vm;

                EventUtil.HandleKey(Key.Up, HexRed, () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithRed(GlobalState.Hex.Red + 1)); });
                EventUtil.HandleKey(Key.Up, HexGreen, () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithGreen(GlobalState.Hex.Green + 1)); });
                EventUtil.HandleKey(Key.Up, HexBlue, () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithBlue(GlobalState.Hex.Blue + 1)); });

                EventUtil.HandleKey(Key.Down, HexRed, () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithRed(GlobalState.Hex.Red - 1)); });
                EventUtil.HandleKey(Key.Down, HexGreen, () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithGreen(GlobalState.Hex.Green - 1)); });
                EventUtil.HandleKey(Key.Down, HexBlue, () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithBlue(GlobalState.Hex.Blue - 1)); });

                EventUtil.HandleMouseWheel(
                    HexRed,
                    () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithRed(GlobalState.Hex.Red + 1)); },
                    () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithRed(GlobalState.Hex.Red - 1)); }
                );
                EventUtil.HandleMouseWheel(
                    HexGreen,
                    () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithGreen(GlobalState.Hex.Green + 1)); },
                    () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithGreen(GlobalState.Hex.Green - 1)); }
                );
                EventUtil.HandleMouseWheel(
                    HexBlue,
                    () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithBlue(GlobalState.Hex.Blue + 1)); },
                    () => { GlobalState.RefreshFromHex(GlobalState.Hex.WithBlue(GlobalState.Hex.Blue - 1)); }
                );

                var rThrottler = new Throttler();
                var gThrottler = new Throttler();
                var bThrottler = new Throttler();

                EventUtil.HandleSliderChange(HexRedSlider, value => { rThrottler.Throttle(1, _ => GlobalState.RefreshFromRgb(GlobalState.Rgb.WithRed(value))); });
                EventUtil.HandleSliderChange(HexGreenSlider, value => { gThrottler.Throttle(1, _ => GlobalState.RefreshFromRgb(GlobalState.Rgb.WithGreen(value))); });
                EventUtil.HandleSliderChange(HexBlueSlider, value => { bThrottler.Throttle(1, _ => GlobalState.RefreshFromRgb(GlobalState.Rgb.WithBlue(value))); });

                // The box actively ignores 3-char hex strings while typing so that 6-chars may be entered without interruption.
                // However, 3-char hex strings can still be entered by pressing Enter or dropping focus:
                EventUtil.HandleInputEnterOrFocusLost(HexText, _ =>
                {
                    var nullableHex = Hex.FromString(vm.HexText);
                    if (nullableHex.HasValue) GlobalState.RefreshFromHex(nullableHex.Value);
                });

                vm.PropertyChangedByUser += (_, ev) =>
                {
                    switch (ev.PropertyName)
                    {
                        case nameof(HexTabViewModel.HexText):
                            var nullableHex = Hex.From6CharString(vm.HexText);
                            if (nullableHex.HasValue) GlobalState.RefreshFromHex(nullableHex.Value);
                            break;
                        case nameof(HexTabViewModel.HexRed):
                            // Length comparison for better UX. Stops converting "0" to "00" and moving user cursor.
                            if (!Hex.IsValidHexPart(vm.HexRed) || vm.HexRed.Length != 2) return;
                            GlobalState.RefreshFromHex(GlobalState.Hex.WithRed(vm.HexRed));
                            break;
                        case nameof(HexTabViewModel.HexGreen):
                            // Length comparison for better UX. Stops converting "0" to "00" and moving user cursor.
                            if (!Hex.IsValidHexPart(vm.HexGreen) || vm.HexGreen.Length != 2) return;
                            GlobalState.RefreshFromHex(GlobalState.Hex.WithGreen(vm.HexGreen));
                            break;
                        case nameof(HexTabViewModel.HexBlue):
                            // Length comparison for better UX. Stops converting "0" to "00" and moving user cursor.
                            if (!Hex.IsValidHexPart(vm.HexBlue) || vm.HexBlue.Length != 2) return;
                            GlobalState.RefreshFromHex(GlobalState.Hex.WithBlue(vm.HexBlue));
                            break;
                    }
                };

                HexTextClipboardButton.ButtonClicked += (_, _) => { Clipboard.Set(vm.HexText); };
            };
        }
    }
}