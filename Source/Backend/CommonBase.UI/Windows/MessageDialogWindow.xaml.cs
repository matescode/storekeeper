using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

using CommonBase.Application.Messages;
using CommonBase.Resources;
using CommonBase.UI.MessageDialogs;

namespace CommonBase.UI.Windows
{
    /// <summary>
    /// Interaction logic for MessageDialogWindow.xaml
    /// </summary>
    public partial class MessageDialogWindow : Window
    {
        private QuestionResult _result;
        private MessageType _messageType;

        public MessageDialogWindow()
        {
            InitializeComponent();
            _result = QuestionResult.Cancel;
        }

        #region MessageDialogWindow Features

        public string Caption
        {
            set
            {
                Title = value;
            }
        }

        public MessageType MessageType
        {
            set
            {
                _messageType = value;
            }
        }

        public string Text
        {
            set
            {
                _mainText.Text = value;
            }
        }

        public IEnumerable<string> Detail
        {
            set
            {
                if (value == null)
                {
                    _detailsExpander.Visibility = Visibility.Hidden;
                    return;
                }

                _detailsText.Text = "";

                StringBuilder sb = new StringBuilder();
                foreach (string str in value)
                {
                    sb.AppendLine(str);
                }
                _detailsText.Text = sb.ToString();
            }
        }

        public QuestionResult Result
        {
            get { return _result; }
        }

        public bool QuestionWithCancel
        {
            private get;
            set;
        }

        public void ShowMessageWindow()
        {
            InitWindow();
            ShowDialog();
        }

        #endregion

        private void InitWindow()
        {
            IResourceProvider provider = UIApplication.Service<IResourceProvider>();
            ILocalizationProvider localization = UIApplication.Service<ILocalizationProvider>();

            _button1.Visibility = Visibility.Hidden;
            _button2.Visibility = Visibility.Hidden;
            _button3.Visibility = Visibility.Hidden;

            if (_messageType == MessageType.Error)
            {
                _iconImage.Source = provider.GetResource<ImageSource>("IconError");
                _button3.Visibility = Visibility.Visible;
                _button3.Content = localization.Translate("Ok");
                _button3.Click += OkButton_Click;
                _button3.Focus();
            }
            else if (_messageType == MessageType.Info)
            {
                _iconImage.Source = provider.GetResource<ImageSource>("IconInfo");
                _button3.Visibility = Visibility.Visible;
                _button3.Content = localization.Translate("Ok");
                _button3.Click += OkButton_Click;
                _button3.Focus();
            }
            else if (_messageType == MessageType.Warning)
            {
                _iconImage.Source = provider.GetResource<ImageSource>("IconWarning");
                _button3.Visibility = Visibility.Visible;
                _button3.Content = localization.Translate("Ok");
                _button3.Click += OkButton_Click;
                _button3.Focus();
            }
            else if (_messageType == MessageType.Question)
            {
                _iconImage.Source = provider.GetResource<ImageSource>("IconQuestion");
                if (QuestionWithCancel)
                {
                    _button1.Visibility = Visibility.Visible;
                    _button2.Visibility = Visibility.Visible;
                    _button3.Visibility = Visibility.Visible;

                    _button1.Content = localization.Translate("Yes");
                    _button2.Content = localization.Translate("No");
                    _button3.Content = localization.Translate("Cancel");

                    _button1.Click += YesButton_Click;
                    _button2.Click += NoButton_Click;
                    _button3.Click += CancelButton_Click;

                    _button1.Focus();
                }
                else
                {
                    _button2.Visibility = Visibility.Visible;
                    _button3.Visibility = Visibility.Visible;

                    _button2.Content = localization.Translate("Yes");
                    _button3.Content = localization.Translate("No");

                    _button2.Click += YesButton_Click;
                    _button3.Click += NoButton_Click;

                    _button2.Focus();
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = QuestionResult.Positive;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            _result = QuestionResult.Positive;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            _result = QuestionResult.Negative;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _result = QuestionResult.Cancel;
            Close();
        }

        private void _detailsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            Height += 180;
            _detailsBorder.Height = 180;
            _detailsText.Visibility = Visibility.Visible;
        }

        private void _detailsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            Height -= 180;
            _detailsBorder.Height = 0;
            _detailsText.Visibility = Visibility.Hidden;
        }
    }
}