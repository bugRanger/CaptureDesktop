namespace Common.Utils.Command
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Делегат условия доступности обратного вызова команды.
    /// </summary>
    /// <typeparam name="T">Тип команды</typeparam>
    /// <param name="sender">Владелец</param>
    /// <param name="command">Исполняемое действие</param>
    /// <param name="precommand">Исполняемое действие до <c>command</c></param>
    /// <returns>В случае успеха операции вернет true, в противном случае - false</returns>
    public delegate bool? DelegateRequiredExecute<T>(object sender, T command, T precommand) where T : ICommand;

    /// <summary>
    /// Команда выполняемая перед.
    /// </summary>
    public interface IPreCommand : ICommand
    {
        /// <summary>
        /// Команда обратного вызова.
        /// </summary>
        ICommand Callback { get; set; }
        /// <summary>
        /// Выполнить.
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <param name="callback">Команда обратного вызова</param>
        void Execute(object parameter, ICommand callback);
        /// <summary>
        /// Условие необходимости вызова.
        /// </summary>
        DelegateRequiredExecute<IPreCommand> IsRequired { get; set; }
    }

    /// <summary>
    /// Делегат команды исполняемой до исполняемого действия.
    /// </summary>
    public class DelegatePreCommand : DelegateCommand, IPreCommand
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="execute">Исполняемое действие</param>
        /// <param name="canExecute">Проверка доступности действия</param>
        public DelegatePreCommand(Action<object> execute, Func<object, bool> canExecute = null) : base(execute, canExecute) { }
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="precommands">Исполняемые пред действие</param>
        /// <param name="execute">Исполняемое действие</param>
        /// <param name="canExecute">Проверка доступности действия</param>
        public DelegatePreCommand(IPreCommand[] precommands, Action<object> execute, Func<object, bool> canExecute = null) : base(execute, canExecute)
        {
            _precommands = precommands;
        }

        /// <summary>
        /// Набор команд исполняемых команд до исполняемого действия.
        /// </summary>
        private IPreCommand[] _precommands;

        /// <summary>
        /// Команда обратного вызова.
        /// </summary>
        public ICommand Callback { get; set; }

        /// <summary>
        /// Условие необходимости вызова.
        /// </summary>
        public DelegateRequiredExecute<IPreCommand> IsRequired { get; set; }

        /// <summary>
        /// Выполнить до первой усл. доступной команды выполняемой до исполняемого действия.
        /// <para>В случае успеха выполнения в обратный вызов будет назначено - исполняемое действие.</para>
        /// </summary>
        /// <param name="parameter">Параметр</param>
        public override void Execute(object parameter)
        {
            //Проверка наличия.
            if (_precommands?.Any() ?? false)
            {
                foreach (var item in _precommands)
                {
                    //Проверка усл. доступности.
                    var result = item.IsRequired?.Invoke(parameter, this, item);
                    if (result != null)
                    {
                        //Проверка результата.
                        if (result == true)
                        {
                            //Проверка доступности.
                            if (item.CanExecute(parameter))
                            {
                                //Назначаем.
                                item.Callback = this;
                                //Выполняем.
                                item.Execute(parameter);
                            }
                        }
                        //Выходим.
                        return;
                    }
                }
            }
            //Выполняем.
            base.Execute(parameter);
        }
        /// <summary>
        /// Выполнить до первой усл. доступной команды выполняемой до исполняемого действия.
        /// <para>В случае успеха выполнения в обратный вызов будет назначено - исполняемое действие.</para>
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <param name="callback">Команда обратного вызова</param>
        public virtual void Execute(object parameter, ICommand callback)
        {
            //Назначаем.
            Callback = callback;
            //Выполняем.
            Execute(parameter);
        }
    }
}
