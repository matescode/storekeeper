using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonBase.Application;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.Controls
{
    /// <summary>
    /// Interaction logic for CodeCompleteControl.xaml
    /// </summary>
    public partial class CodeCompleteControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register("Code", typeof(string), typeof(CodeCompleteControl), new PropertyMetadata(string.Empty, OnCodePropertyChanged));

        public static readonly DependencyProperty CodeTypeProperty = DependencyProperty.Register("CodeType", typeof(ArticleCodeType), typeof(CodeCompleteControl), new PropertyMetadata(ArticleCodeType.All));

        public CodeCompleteControl()
        {
            InitializeComponent();
        }

        public string Code
        {
            get
            {
                return (string)GetValue(CodeProperty);
            }
            set
            {
                SetValue(CodeProperty, value);
            }
        }

        public ArticleCodeType CodeType
        {
            get
            {
                return (ArticleCodeType)GetValue(CodeTypeProperty);
            }
            set
            {
               SetValue(CodeTypeProperty, value); 
            }
        }

        #region Handlers

        private void EditBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string codeText = EditBox.Text;
            Code = codeText;

            if (codeText.Length >= AppContext.Config.SeekCodeCharLimit)
            {
                if (!Popup.IsOpen)
                {
                    ShowList();
                }
            }
            else
            {
                HideList();
            }

            if (Popup.IsOpen)
            {
                PossibleValuesListBox.Items.Filter = (item => (item as CodeCompleteListBoxItem).Text.ToString().ToUpper().StartsWith(codeText.ToUpper()));

                if (PossibleValuesListBox.Items.Count == 0)
                {
                    HideList();
                }
            }
        }

        private void TextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CodeCompleteListBoxItem item = (sender as TextBlock).DataContext as CodeCompleteListBoxItem;
            Code = item.Text;
            HideList();
        }

        #endregion

        #region Internals and Helpers

        private static void OnCodePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CodeCompleteControl autoCompleteControl = dependencyObject as CodeCompleteControl;
            autoCompleteControl.OnPropertyChanged("Code");
            autoCompleteControl.OnCodePropertyChanged(e);
        }

        private void OnCodePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                EditBox.Text = Code;
            }
        }

        private void ShowList()
        {
            Popup.IsOpen = true;
            InitValues();
        }

        private void HideList()
        {
            Popup.IsOpen = false;
            PossibleValuesListBox.Items.Filter = null;
        }

        private void InitValues()
        {
            List<CodeCompleteListBoxItem> items = new List<CodeCompleteListBoxItem>();

            try
            {
                IDataAccess dataAcces = ApplicationContext.Service<IDataAccess>();
                IEnumerable<IArticleCode> matches = dataAcces.GetMatchingArticleCodes(Code, CodeType);
                items.AddRange(matches.Select(articleCode => new CodeCompleteListBoxItem(articleCode.Code, articleCode.Description)));
            }
            catch (Exception ex)
            {
                ApplicationContext.Log.Error(GetType(), ex);
            }

            PossibleValuesListBox.ItemsSource = items;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Private Classes

        private class CodeCompleteListBoxItem
        {
            public CodeCompleteListBoxItem(string text, string tooltip)
            {
                Text = text;
                ToolTip = tooltip;
            }

            public string Text { get; private set; }

            public string ToolTip { get; private set; }

            public override string ToString()
            {
                return Text;
            }
        }

        #endregion
    }
}
