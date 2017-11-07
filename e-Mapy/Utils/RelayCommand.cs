using eMapy.Models;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace eMapy.Utils
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }
        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            execute(parameter);
        }


    }
}