using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using eMapy.Utils;

namespace eMapy.Core
{
    public class KeyUpBinding : InputBinding
    {
        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyUpBinding), new PropertyMetadata(Key.A, KeyChanged));

        private static void KeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var keybinding = (d as KeyUpBinding);

            Keyboard.AddKeyUpHandler(App.Current.MainWindow, (s, ku) =>
            {
                if (keybinding.Command != null && ku.Key == keybinding.Key && ku.IsUp)
                {
                    //KeyBinding.Command.CanExecute(keybinding.can)
                    keybinding.Command.ExecuteAsync(keybinding.CommandParameter);
                }
            });
        }


        public AsyncCommand Command
        {
            get { return (AsyncCommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(AsyncCommand), typeof(KeyUpBinding), new PropertyMetadata(null));



    }
}
