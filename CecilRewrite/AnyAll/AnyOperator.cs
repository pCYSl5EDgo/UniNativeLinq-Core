using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming

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

            var argumentsFromTypeToMethodParam = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(argumentsFromTypeToMethodParam);
            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);

            GenericParameter TPredicate0;
            TPredicate0 = new GenericParameter(nameof(TPredicate0), method)
            {
                HasNotNullableValueTypeConstraint = true,
                HasDefaultConstructorConstraint = true,
                IsNonVariant = true,
            };
            TPredicate0.Constraints.Add(MainModule.ImportReference(typeof(ValueType)));
            TPredicate0.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[]
            {
                @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters),
                MainModule.TypeSystem.Boolean
            }));
            method.GenericParameters.Add(TPredicate0);

            var predicateParameterDefinition = new ParameterDefinition("predicate", ParameterAttributes.In, TPredicate0.MakeByReferenceType());
            predicateParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(predicateParameterDefinition);

            var body = method.Body;
            var variables = body.Variables;
            var Enumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);

            var processor = body.GetILProcessor();

            variables.Add(new VariableDefinition(Enumerator));
            variables.Add(new VariableDefinition(Element));

            var il0007 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var il002B = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

            processor.Do(OpCodes.Ldarg_0);
            processor.GetEnumerator(@this);
            processor.Do(OpCodes.Stloc_0);
            processor.Append(il0007);
            processor.LdLocaS(1);
            processor.Call(Enumerator.FindMethod("TryMoveNext"));
            processor.False(il002B);
            processor.Do(OpCodes.Ldarg_1);
            processor.LdLocaS(1);
            processor.Constrained(TPredicate0);
            processor.CallVirtual(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[] { Element, MainModule.TypeSystem.Boolean }).FindMethod("Calc"));
            processor.False(il0007);
            processor.LdLocaS(0);
            var Dispose = Enumerator.FindMethod("Dispose");
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Ret();
            processor.Append(il002B);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
