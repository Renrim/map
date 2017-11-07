using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace eMapy.Views
{
    /// <summary>
    /// Interaction logic for ButtonBase.xaml
    /// </summary>
    public partial class ButtonBase : UserControl
    {
        public ButtonBase()
        {
            InitializeComponent();
        }


        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconPathProperty =
    DependencyProperty.Register("IconPath", typeof(string), typeof(ButtonBase), new PropertyMetadata(null));

        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set { SetValue(RightTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightTextProperty =
    DependencyProperty.Register("RightText", typeof(string), typeof(ButtonBase), new PropertyMetadata(null));

        public ICommand MyCommand
        {
            get { return (ICommand)GetValue(MyCommandProperty); }
            set { SetValue(MyCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyCommandProperty =
    DependencyProperty.Register("MyCommand", typeof(ICommand), typeof(ButtonBase), new PropertyMetadata(null));

        public int PathSize
        {
            get { return (int)GetValue(PathSizeProperty); }
            set { SetValue(PathSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathSizeProperty =
    DependencyProperty.Register("PathSize", typeof(int), typeof(ButtonBase), new PropertyMetadata(null));





     


    }
}