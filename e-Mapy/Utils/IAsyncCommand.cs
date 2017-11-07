using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMapy.Utils
{
    using System.Windows.Input;

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }

    public abstract class AsyncCommandBase : IAsyncCommand
    {
        public abstract bool CanExecute(object parameter);

        public abstract Task ExecuteAsync(object parameter);

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class AsyncCommand : AsyncCommandBase
    {
        private readonly Func<object, Task> commandWithParam;
        private readonly Func<Task> _command;

        private Func<object, bool> canExecute;

        public AsyncCommand(Func<object, Task> commandWithParam, Func<object, bool> canExecute = null)

        {
            this.canExecute = canExecute;
            this.commandWithParam = commandWithParam;
        }

        public AsyncCommand(Func<Task> command, Func<object, bool> canExecute = null)
        {
            this.canExecute = canExecute;
            _command = command;
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute == null || this.canExecute(parameter);
        }

        public override Task ExecuteAsync(object parameter)
        {
            if (_command == null)
            {
                Param = parameter;
                return this.commandWithParam(parameter);
            }
            return _command();
        }

        public object Param { get; set; }
    }
}