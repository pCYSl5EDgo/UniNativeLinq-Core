using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CecilRewrite
{
    using static Program;
    static class AllOperatorHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AllOperatorHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                @static.All(type);
            }
        }

        private static void All(this TypeDefinition @static, TypeDefinition type)
        {
            var method = new MethodDefinition(nameof(All), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            method.Parameters.Capacity = 1;
            var argumentsFromTypeToMethodParam = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(argumentsFromTypeToMethodParam);
            FillParameter(@this, method);
            FillBody(@this, method);
            @static.Methods.Add(method);
        }

        private static void FillParameter(GenericInstanceType @this, MethodDefinition method)
        {
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
        }

        private static void FillBody(GenericInstanceType @this, MethodDefinition method)
        {
            var body = method.Body;

            var typeReferenceEnumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var typeReferenceElement = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var typeReferencePredicate = ((ByReferenceType)method.Parameters[1].ParameterType).ElementType;

            body.Variables.Add(new VariableDefinition(typeReferenceEnumerator));
            body.Variables.Add(new VariableDefinition(typeReferenceElement.MakeByReferenceType()));
            body.Variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));

            var il001D = Instruction.Create(OpCodes.Ldarg_1);
            var il0036 = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
            var il004C = Instruction.Create(OpCodes.Ldarg_1);

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(@this.FindMethod("GetEnumerator", Helper.NoParameter));
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.LdLocaS(2);
            var TryGetNext = typeReferenceEnumerator.FindMethod("TryGetNext");
            processor.Call(TryGetNext);
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.True(il001D);
            processor.LdLocaS(0);
            var Dispose = typeReferenceEnumerator.FindMethod("Dispose");
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Do(OpCodes.Ret);
            processor.Append(il001D);
            processor.Do(OpCodes.Ldloc_1);
            processor.Constrained(typeReferencePredicate);
            var Calc = MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[] { typeReferenceElement, MainModule.TypeSystem.Boolean }).FindMethod("Calc");
            processor.CallVirtual(Calc);
            processor.True(il0036);
            processor.LdLocaS(0);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Ret);
            processor.Append(il0036);
            processor.LdLocaS(2);
            processor.Call(TryGetNext);
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.True(il004C);
            processor.LdLocaS(0);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Do(OpCodes.Ret);
            processor.Append(il004C);
            processor.Do(OpCodes.Ldloc_1);
            processor.Constrained(typeReferencePredicate);
            processor.CallVirtual(Calc);
            processor.True(il0036);
            processor.LdLocaS(0);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Ret);
        }
    }
}
