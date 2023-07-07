using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFUI.Controls
{
    public partial class NumericUpDown : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(int?),
                typeof(NumericUpDown),
                new PropertyMetadata(
                    null,
                    new PropertyChangedCallback(OnNumericValueChanged),
                    new CoerceValueCallback(CoerceNumericValue)));

        public int? Value
        {
            get { return (int?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static object CoerceNumericValue(DependencyObject d, object baseValue)
        {
            NumericUpDown control = (NumericUpDown)d;
            int? newValue = (int?)baseValue;

            if (newValue == null && !control.AllowNullValue)
                newValue = 0;

            if (newValue < control.MinimumValue)
                newValue = control.MinimumValue;

            if (newValue > control.MaximumValue)
                newValue = control.MaximumValue;

            return newValue;
        }

        private static void OnNumericValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown control = (NumericUpDown)d;

            if (control.Value == null)
                control.Text = control.NullValueText;
            else
                control.Text = control.Value.ToString();
        }

        public static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.Register(
                nameof(MinimumValue),
                typeof(int?),
                typeof(NumericUpDown),
                new PropertyMetadata(
                    null,
                    new PropertyChangedCallback(OnMinimumValueChanged)));

        public int? MinimumValue
        {
            get { return (int?)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        private static void OnMinimumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown control = (NumericUpDown)d;

            if (control.MaximumValue < control.MinimumValue)
                control.MaximumValue = control.MinimumValue;

            control.CoerceValue(ValueProperty);
        }

        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register(
                nameof(MaximumValue),
                typeof(int?),
                typeof(NumericUpDown),
                new PropertyMetadata(
                    null,
                    new PropertyChangedCallback(OnMaximumValueChanged)));

        public int? MaximumValue
        {
            get { return (int?)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        private static void OnMaximumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown control = (NumericUpDown)d;

            if (control.MinimumValue > control.MaximumValue)
                control.MinimumValue = control.MaximumValue;

            control.CoerceValue(ValueProperty);
        }

        public static readonly DependencyProperty AllowNullValueProperty =
            DependencyProperty.Register(
                nameof(AllowNullValue),
                typeof(bool),
                typeof(NumericUpDown),
                new PropertyMetadata(
                    true,
                    new PropertyChangedCallback(OnAllowNullValueChanged)));

        public bool AllowNullValue
        {
            get { return (bool)GetValue(AllowNullValueProperty); }
            set { SetValue(AllowNullValueProperty, value); }
        }

        private static void OnAllowNullValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
                d.CoerceValue(ValueProperty);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(NumericUpDown),
                new PropertyMetadata(
                    "<null>",
                    new PropertyChangedCallback(OnTextChanged),
                    new CoerceValueCallback(CoerceText)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static object CoerceText(DependencyObject d, object baseValue)
        {
            NumericUpDown control = (NumericUpDown)d;
            string newValue = (string)baseValue;

            if (int.TryParse(newValue, out int numericValue))
                newValue = numericValue.ToString();
            else if (control.inputBox.IsKeyboardFocused)
                newValue = string.Empty;
            else
                newValue = control.NullValueText;

            return newValue;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown control = (NumericUpDown)d;

            if (int.TryParse(control.Text, out int numericValue))
            {
                control.Value = numericValue;
                control.Text = control.Value.ToString();
            }
            else
            {
                control.Value = null;
                control.Text = control.NullValueText;
            }
        }

        public static readonly DependencyProperty NullValueTextProperty =
            DependencyProperty.Register(
                nameof(NullValueText),
                typeof(string),
                typeof(NumericUpDown),
                new PropertyMetadata(
                    "<null>",
                    new PropertyChangedCallback(OnNullValueTextChanged)));

        public string NullValueText
        {
            get { return (string)GetValue(NullValueTextProperty); }
            set { SetValue(NullValueTextProperty, value); }
        }

        private static void OnNullValueTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(TextProperty);
        }

        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register(
                nameof(TextMargin),
                typeof(Thickness),
                typeof(NumericUpDown));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public static readonly DependencyProperty TextPaddingProperty =
            DependencyProperty.Register(
                nameof(TextPadding),
                typeof(Thickness),
                typeof(NumericUpDown));

        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(
                nameof(TextAlignment),
                typeof(TextAlignment),
                typeof(NumericUpDown));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty ButtonSizeProperty =
            DependencyProperty.Register(
                nameof(ButtonSize),
                typeof(double),
                typeof(NumericUpDown),
                new PropertyMetadata(20.0));

        public double ButtonSize
        {
            get { return (double)GetValue(ButtonSizeProperty); }
            set { SetValue(ButtonSizeProperty, value); }
        }

        public static readonly DependencyProperty ArrowFontSizeProperty =
            DependencyProperty.Register(
                nameof(ArrowFontSize),
                typeof(double),
                typeof(NumericUpDown),
                new PropertyMetadata(12.0));

        public double ArrowFontSize
        {
            get { return (double)GetValue(ArrowFontSizeProperty); }
            set { SetValue(ArrowFontSizeProperty, value); }
        }

        public NumericUpDown()
        {
            InitializeComponent();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Value == null)
            {
                if (this.MinimumValue.HasValue)
                    this.Value = this.MinimumValue;
                else if (this.MaximumValue < 0)
                    this.Value = this.MaximumValue;
                else
                    this.Value = 0;
            }
            else if (this.Value < this.MaximumValue || this.MaximumValue == null)
            {
                this.Value++;
            }
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Value == this.MinimumValue)
                this.Value = null;

            else if (this.Value > this.MinimumValue || this.MinimumValue == null)
                this.Value--;
        }

        private void inputBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (inputBox.Text == this.NullValueText)
                this.Text = string.Empty;

            inputBox.SelectAll();
        }

        private void inputBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (inputBox.Text == string.Empty)
                this.Text = this.NullValueText;
        }
    }
}
