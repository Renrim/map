using System.Windows;
using System.Windows.Controls;

namespace eMapy.Views
{
    /// <summary>
    /// Interaction logic for ToolTipxaml.xaml
    /// </summary>
    public partial class ToolTip : UserControl
    {
        public ToolTip()
        {
            InitializeComponent();
        }



        public string ConfidenceLevel
        {
            get { return (string)GetValue(ConfidenceLevelProperty); }
            set { SetValue(ConfidenceLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConfidenceLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfidenceLevelProperty =
            DependencyProperty.Register("ConfidenceLevel", typeof(string), typeof(ToolTip), new PropertyMetadata(null));


    }
}