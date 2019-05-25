using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace CecilRewrite
{
    internal static class Helper
    {
        public static GenericInstanceType MakeGenericType(this TypeReference self, IEnumerable<TypeReference> arguments)
        {
            var instance = new GenericInstanceType(self);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);
            return instance;
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, IEnumerable<TypeReference> arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };
            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            foreach (var genericParameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));
            return reference;
        }

        public static MethodReference FindMethod(this GenericInstanceType type, string name)
            => type.Resolve().Methods.Single(x => x.Name == name).MakeHostInstanceGeneric(type.GenericArguments);

        public static MethodReference FindMethodAndImport(this GenericInstanceType type, string name, ModuleDefinition module)
            => module.ImportReference(type.Resolve().Methods.Single(x => x.Name == name)).MakeHostInstanceGeneric(type.GenericArguments);

        public static MethodReference FindMethod(this GenericInstanceType type, string name, Func<MethodDefinition, bool> predicate)
            => type.Resolve().Methods.Single(x => x.Name == name && predicate(x)).MakeHostInstanceGeneric(type.GenericArguments);

        public static GenericInstanceType FindNested(this GenericInstanceType type, string name)
        {
            var nestedType = new GenericInstanceType(((TypeDefinition)type.ElementType).NestedTypes.First(x => x.Name.EndsWith(name)));
            foreach (var argument in type.GenericArguments)
                nestedType.GenericArguments.Add(argument);
            return nestedType;
        }
    }
}
