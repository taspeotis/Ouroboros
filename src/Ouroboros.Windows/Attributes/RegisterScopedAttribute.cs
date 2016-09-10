using System;

namespace Ouroboros.Windows.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class RegisterScopedAttribute : Attribute
    {
        public RegisterScopedAttribute(Type implementationType, Type interfaceType)
        {
            ImplementationType = implementationType;
            InterfaceType = interfaceType;
        }

        public Type ImplementationType { get; }

        public Type InterfaceType { get; }
    }
}