using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Flow.Player.ViewModels;
namespace Flow.Player.Views;

public partial class MainWindow : Window
{
	private Slider? _slider;
	
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainWindowViewModel();

		Init();
	}
	public MainWindow(string filePath) 
	{
		InitializeComponent();
		DataContext = new MainWindowViewModel(filePath);

		Init();
	}

	private void Init()
	{
		Loaded += (s, e) =>
		{
			_slider = this.FindControl<Slider>("SeekSlider");
			if (_slider == null)
				return;

			_slider.AddHandler(PointerPressedEvent, OnSeekBarPointerPressed, RoutingStrategies.Tunnel);
			_slider.AddHandler(PointerReleasedEvent, OnSeekBarPointerReleased, RoutingStrategies.Tunnel);
		};

		KeyDown += OnKeyDown;
	}
	
	private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			return;

		Visual? element = e.Source as Visual;
		while (element != null)
		{
			if (element is MenuItem)
				return;
				
			if (element == sender)
				break;
				
			element = element.Parent as Visual;
		}
        
		BeginMoveDrag(e);
		e.Handled = true;
	}

	private void OnSeekBarPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (DataContext is MainWindowViewModel vm)
		{
			vm.IsSeeking = true;
		}
	}

	private void OnSeekBarPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (DataContext is not MainWindowViewModel vm || _slider == null || !vm.IsSeeking)
			return;

		vm.IsSeeking = false;
		vm.SeekTo((long)_slider.Value);
	}

	private void OnKeyDown(object? sender, KeyEventArgs e)
	{
		if (DataContext is not MainWindowViewModel vm)
			return;

		switch (e.Key)
		{
			case Key.Space or Key.MediaPlayPause:
				vm.IsPlaying = !vm.IsPlaying;
				break;
			case Key.Right:
				vm.Seek(5);
				break;
			case Key.Left:
				vm.Seek(-5);
				break;
		}
	}
	private void VolumeSliderScroll(object? sender, PointerWheelEventArgs e)
	{
		if (DataContext is not MainWindowViewModel vm)
			return;

		switch (e.Delta.Y)
		{
			case > 0:
				vm.Volume += 5;
				break;
			case < 0:
				vm.Volume -= 5;
				break;
		}
	}
}