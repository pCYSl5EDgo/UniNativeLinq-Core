using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class DistinctDefaultHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition @static;
            @static = new TypeDefinition(NameSpace,
                nameof(DistinctDefaultHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);

            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                Distinct(@static, type);
            }
        }

        private static void Distinct(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(Distinct), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);

            GenericParameter Element;
            {
                var _element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
                var findIndex = added.FindIndex(x => x.Name == _element.Name);
                if (findIndex == -1)
                    return;
                Element = added[findIndex];
            }
            Element.Constraints.Add(MainModule.ImportReference(typeof(IEquatable<>)).MakeGenericInstanceType(Element));
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            var DefaultEqualityComparer = MainModule.GetType(NameSpace, "DefaultEqualityComparer`1").MakeGenericInstanceType(Element);
            var DefaultGetHashCodeFunc = MainModule.GetType(NameSpace, "DefaultGetHashCodeFunc`1").MakeGenericInstanceType(Element);

            var @return = MainModule.GetType(NameSpace, nameof(Distinct) + "Enumerable`5").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                DefaultEqualityComparer,
                DefaultGetHashCodeFunc,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            method.Parameters.Add(new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            });

            var variables = method.Body.Variables;
            var DefaultEqualityComparerVariable = new VariableDefinition(DefaultEqualityComparer);
            var DefaultGetHashCodeFuncVariable = new VariableDefinition(DefaultGetHashCodeFunc);
            variables.Add(DefaultEqualityComparerVariable);
            variables.Add(DefaultGetHashCodeFuncVariable);

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.LdLocaS(0);
            processor.LdLocaS(1);
            processor.Do(OpCodes.Ldarg_1);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
