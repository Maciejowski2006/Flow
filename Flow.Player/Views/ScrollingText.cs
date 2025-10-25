using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Flow.Player.Views;

public class ScrollingTextBlock : Control
{
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<ScrollingTextBlock, string>(nameof(Text), string.Empty);
    public static readonly StyledProperty<IBrush?> ForegroundProperty = AvaloniaProperty.Register<ScrollingTextBlock, IBrush?>(nameof(Foreground), Brushes.Black);
    public static readonly StyledProperty<double> FontSizeProperty = AvaloniaProperty.Register<ScrollingTextBlock, double>(nameof(FontSize), 12.0);
    public static readonly StyledProperty<double> ScrollSpeedProperty = AvaloniaProperty.Register<ScrollingTextBlock, double>(nameof(ScrollSpeed), 50.0);
    public static readonly StyledProperty<double> GapWidthProperty = AvaloniaProperty.Register<ScrollingTextBlock, double>(nameof(GapWidth), 50.0);
    private static readonly StyledProperty<FontFamily> FontFamilyProperty = AvaloniaProperty.Register<ScrollingTextBlock, FontFamily>(nameof(FontFamily), FontFamily.Default);

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public double ScrollSpeed
    {
        get => GetValue(ScrollSpeedProperty);
        set => SetValue(ScrollSpeedProperty, value);
    }

    public double GapWidth
    {
        get => GetValue(GapWidthProperty);
        set => SetValue(GapWidthProperty, value);
    }

    private FormattedText? _formattedText;
    private double _offset;
    private DispatcherTimer? _timer;
    private DateTime _lastUpdate = DateTime.Now;
    private double _textWidth;

    static ScrollingTextBlock()
    {
        AffectsRender<ScrollingTextBlock>(TextProperty, ForegroundProperty, FontFamilyProperty);
        AffectsMeasure<ScrollingTextBlock>(TextProperty, FontFamilyProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty || change.Property == FontFamilyProperty || change.Property == FontSizeProperty)
        {
            UpdateFormattedText();
            _offset = 0;
            UpdateScrolling();
        }

        if (change.Property == BoundsProperty)
        {
            UpdateScrolling();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateFormattedText();
        UpdateScrolling();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopScrolling();
    }

    private void UpdateFormattedText()
    {
        if (string.IsNullOrEmpty(Text))
        {
            _formattedText = null;
            _textWidth = 0;
            return;
        }

        _formattedText = new(
            Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new(FontFamily.Default),
            FontSize,
            Foreground);

        _textWidth = _formattedText.Width;
        
        InvalidateVisual();
    }

    private void UpdateScrolling()
    {
        if (_formattedText == null || Bounds.Width <= 0)
        {
            StopScrolling();
            return;
        }

        if (_textWidth > Bounds.Width)
        {
            StartScrolling();
        }
        else
        {
            StopScrolling();
            _offset = 0;
            InvalidateVisual();
        }
    }

    private void StartScrolling()
    {
        if (_timer != null) return;

        _timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
        };
        _timer.Tick += OnTimerTick;
        _lastUpdate = DateTime.Now;
        _timer.Start();
    }

    private void StopScrolling()
    {
        if (_timer == null)
            return;

        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _timer = null;
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        DateTime now = DateTime.Now;
        double elapsed = (now - _lastUpdate).TotalSeconds;
        _lastUpdate = now;

        _offset += ScrollSpeed * elapsed;

        // Reset offset when we've scrolled past the text + gap
        double cycleWidth = _textWidth + GapWidth;
        if (_offset >= cycleWidth)
        {
            _offset -= cycleWidth;
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_formattedText == null || Bounds.Width <= 0 || Bounds.Height <= 0)
            return;

        // Clip to bounds
        context.PushClip(new(Bounds.Size));

        double yOffset = (Bounds.Height - _formattedText.Height) / 2;

        if (_textWidth <= Bounds.Width)
        {
            context.DrawText(_formattedText, new(0, yOffset));
        }
        else
        {
            double cycleWidth = _textWidth + GapWidth;
            
            double text1 = -_offset;
            context.DrawText(_formattedText, new(text1, yOffset));

            double text2 = text1 + cycleWidth;
            context.DrawText(_formattedText, new(text2, yOffset));

            if (!(text2 + _textWidth < Bounds.Width))
                return;

            double text3 = text2 + cycleWidth;
            context.DrawText(_formattedText, new(text3, yOffset));
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_formattedText == null)
            return new(0, 0);

        return new(
            double.IsInfinity(availableSize.Width) ? _textWidth : availableSize.Width,
            _formattedText.Height);
    }
}