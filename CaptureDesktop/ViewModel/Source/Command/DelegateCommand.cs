using System;
using System.Windows.Input;

namespace Common.WPF.Tools.Command
{
    /// <summary>
    /// Делегат исполняемого действия.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="execute">Исполняемое действие</param>
        /// <param name="canExecute">Проверка доступности действия</param>
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Исполняемое действие.
        /// </summary>
        private readonly Action<object> _execute;
        /// <summary>
        /// Проверка доступности действия.
        /// </summary>
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Событие уведомлений о доступности действия.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Выполнить.
        /// </summary>
        /// <param name="parameter">Параметр</param>
        public virtual void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
        /// <summary>
        /// Проверить доступность.
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <returns>В случае успеха операции вернет true, в противном случае - false</returns>
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }
    }
}
