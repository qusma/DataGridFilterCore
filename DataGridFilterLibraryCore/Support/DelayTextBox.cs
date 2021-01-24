using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace DataGridFilterLibrary.Support
{
    /// <summary>
    /// WPF port of windows forms version: http://www.codeproject.com/KB/miscctrl/CustomTextBox.aspx
    /// </summary>
    public class DelayTextBox : TextBox
    {
        #region private globals

        private Timer DelayTimer; // used for the delay
        private bool TimerElapsed = false; // if true OnTextChanged is fired.
        private bool KeysPressed = false; // makes event fire immediately if it wasn't a keypress
        private const int DELAY_TIME = 100; //for now best empiric value

        public static readonly DependencyProperty DelayTimeProperty =
            DependencyProperty.Register("DelayTime", typeof(int), typeof(DelayTextBox));

        #endregion private globals

        #region ctor

        public DelayTextBox()
            : base()
        {
            // Initialize Timer
            DelayTimer = new Timer(DELAY_TIME);
            DelayTimer.Elapsed += DelayTimer_Elapsed;

            _previousTextChangedEventArgs = null;

            AddHandler(PreviewKeyDownEvent, new System.Windows.Input.KeyEventHandler(DelayTextBox_PreviewKeyDown));

            PreviousTextValue = String.Empty;
        }

        private void DelayTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!DelayTimer.Enabled)
                DelayTimer.Enabled = true;
            else
            {
                DelayTimer.Enabled = false;
                DelayTimer.Enabled = true;
            }

            KeysPressed = true;
        }

        #endregion ctor

        #region event handlers

        private void DelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DelayTimer.Enabled = false;// stop timer.

            TimerElapsed = true;// set timer elapsed to true, so the OnTextChange knows to fire

            Dispatcher.Invoke(new DelayOverHandler(DelayOver), null);// use invoke to get back on the UI thread.
        }

        #endregion event handlers

        #region overrides

        private TextChangedEventArgs _previousTextChangedEventArgs;

        public string PreviousTextValue { get; private set; }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            // if the timer elapsed or text was changed by something besides a keystroke
            // fire base.OnTextChanged
            if (TimerElapsed || !KeysPressed)
            {
                TimerElapsed = false;
                KeysPressed = false;
                base.OnTextChanged(e);

                System.Windows.Data.BindingExpression be = GetBindingExpression(TextProperty);
                if (be != null && be.Status == System.Windows.Data.BindingStatus.Active) be.UpdateSource();

                PreviousTextValue = Text;
            }

            _previousTextChangedEventArgs = e;
        }

        #endregion overrides

        #region delegates

        public delegate void DelayOverHandler();

        #endregion delegates

        #region private helpers

        private void DelayOver()
        {
            if (_previousTextChangedEventArgs != null)
                OnTextChanged(_previousTextChangedEventArgs);
        }

        #endregion private helpers
    }
}