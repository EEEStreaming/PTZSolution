using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PTZPadController.PresentationLayer
{
    /// <summary>
    /// Logique d'interaction pour PresetItemView.xaml
    /// </summary>
    public partial class PresetItemView : UserControl
    {

        private Popup m_Popup;
        private bool m_ButtonDown;
        private ListBox m_ListPresetIcon;
        

        public string PresetId
        {
            get { return (string)GetValue(PresetIdProperty); }
            set { SetValue(PresetIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PresetId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresetIdProperty =
            DependencyProperty.Register("PresetId", typeof(string), typeof(PresetItemView), new PropertyMetadata(String.Empty));



        public ICommand PresetUpCommand
        {
            get { return (ICommand)GetValue(PresetUpCommandProperty); }
            set { SetValue(PresetUpCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PresetUpCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresetUpCommandProperty =
            DependencyProperty.Register("PresetUpCommand", typeof(ICommand), typeof(PresetItemView), new UIPropertyMetadata(null));



        public ICommand PresetDownCommand
        {
            get { return (ICommand)GetValue(PresetDownCommandProperty); }
            set { SetValue(PresetDownCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PresetDownCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresetDownCommandProperty =
            DependencyProperty.Register("PresetDownCommand", typeof(ICommand), typeof(PresetItemView), new UIPropertyMetadata(null));

        public ICommand PresetChangeImageCommand
        {
            get { return (ICommand)GetValue(PresetChangeImageCommandProperty); }
            set { SetValue(PresetChangeImageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PresetDownCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresetChangeImageCommandProperty =
            DependencyProperty.Register("PresetChangeImageCommand", typeof(ICommand), typeof(PresetItemView), new UIPropertyMetadata(null));



        public ImageSource ImgSource
        {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImgSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgSourceProperty =
            DependencyProperty.Register("ImgSource", typeof(ImageSource), typeof(PresetItemView), new UIPropertyMetadata(null));





        public PresetItemView()
        {
            InitializeComponent();
            m_Popup = new Popup();
            m_Popup.Placement = PlacementMode.MousePoint;
            m_Popup.MaxHeight = 400;
            m_Popup.MaxWidth = 500;
            m_Popup.StaysOpen = false;
            m_ListPresetIcon = FindResource("PopupView") as ListBox;
            m_ListPresetIcon.ItemsSource = PresetIconList.Icons;
            m_ListPresetIcon.SelectionChanged += PopupSelectionChanged;
           
            m_Popup.Child = m_ListPresetIcon;
           // m_Popup.Child = new StackPanel { Orientation = Orientation.Vertical, Children = { new Button() { Content = new TextBlock() { Text = "Copy full Data to clipboard" } }, new Button() { Content = new TextBlock() { Text = "Copy current Data to clipboards" } } } };
            m_Popup.MouseDown += M_Popup_MouseDown;
            m_ButtonDown = false;
        }

        private void PopupSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_Popup.IsOpen)
            {
                Debug.WriteLine("Preset {0}, Index {1}", PresetId, m_ListPresetIcon.SelectedIndex);
                var cmd = GetValue(PresetChangeImageCommandProperty) as ICommand;
                PresetEventArgs args = new PresetEventArgs();
                args.PresetId = PresetId;
                args.PresetIcon = m_ListPresetIcon.SelectedItem as PresetIcon;
                if (cmd != null && cmd.CanExecute(args))
                {
                    cmd.Execute(args);
                    e.Handled = true;
                }
            }
            m_Popup.IsOpen = false;

        }

        private void M_Popup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Popup.IsOpen = false;
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            m_ListPresetIcon.SelectedIndex = -1;
            m_Popup.IsOpen = true;
            m_ButtonDown = false;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (!m_ButtonDown)
            {
                m_ButtonDown = true;
                var cmd = GetValue(PresetDownCommandProperty) as ICommand;
                if (cmd != null && cmd.CanExecute(PresetId))
                {
                    cmd.Execute(PresetId);
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            if (m_ButtonDown)
            {
                m_ButtonDown = false;
                var cmd = GetValue(PresetUpCommandProperty) as ICommand;
                if (cmd != null && cmd.CanExecute(PresetId))
                {
                    cmd.Execute(PresetId);
                    e.Handled = true;
                }
            }
        }
    }

    public class PresetEventArgs
    {
        public string PresetId { get; internal set; }
        public PresetIcon PresetIcon { get; internal set; }
    }
}
