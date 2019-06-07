using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    using static Program;
    internal static class AnyOperatorHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AnyOperatorHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                @static.Any(type);
            }
        }

        private static void Any(this TypeDefinition @static, TypeDefinition type)
        {
            var method = new MethodDefinition(nameof(Any), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            method.Parameters.Capacity = 1;
            var argumentsFromTypeToMethodParam = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(argumentsFromTypeToMethodParam);
            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);
            var genericParameter = new GenericParameter("TPredicate0", method)
            {
                HasNotNullableValueTypeConstraint = true,
                HasDefaultConstructorConstraint = true,
                IsNonVariant = true,
            };
            genericParameter.Constraints.Add(MainModule.ImportReference(typeof(ValueType)));
            genericParameter.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[]
            {
                @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters),
                MainModule.TypeSystem.Boolean
            }));
            method.GenericParameters.Add(genericParameter);
            var predicateParameterDefinition = new ParameterDefinition("predicate", ParameterAttributes.In, genericParameter.MakeByReferenceType());
            predicateParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(predicateParameterDefinition);

            var body = method.Body;
            var parameterTypeTPredicate0Ref = (ByReferenceType)method.Parameters[1].ParameterType;
            var enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);

            var processor = body.GetILProcessor();

            @static.Methods.Add(method);
        }
    }
}
