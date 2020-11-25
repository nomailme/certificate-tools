using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace certificate_tools
{
    public static class CommandExtensions
    {
        public static Command ConfigureFromClass<T>(this Command command, Action<T> execute)
            where T : new()
        {
            var commandType = typeof(T);

            command.Description = commandType
                .GetCustomAttributes()
                .OfType<DescriptionAttribute>()
                .FirstOrDefault()
                ?.Description;

            var props = commandType.GetProperties();
            foreach (var prop in props)
            {
                var name = prop.GetAttribute<NameAttribute>()?.Name ?? prop.Name;
                var description = prop.GetAttribute<DescriptionAttribute>()?.Description;
                var required = prop.GetAttribute<RequiredAttribute>() != null;

                var option = new Option($"-{name}", description)
                {
                    IsRequired = required,
                    Argument = new Argument
                    {
                        ArgumentType = prop.PropertyType,
                    }
                };

                command.AddOption(option);
            }

            command.Handler = CommandHandler.Create((InvocationContext context) =>
            {
                var modelBinder = new ModelBinder<T>();
                var cmd = new T();
                modelBinder.UpdateInstance(cmd, context.BindingContext);
                execute(cmd);
            });
            return command;
        }

        private static T GetAttribute<T>(this PropertyInfo prop, bool inherit = true)
        {
            return prop
                .GetCustomAttributes(inherit)
                .OfType<T>()
                .FirstOrDefault();
        }
    }

    public class NameAttribute : Attribute
    {
        public NameAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}
