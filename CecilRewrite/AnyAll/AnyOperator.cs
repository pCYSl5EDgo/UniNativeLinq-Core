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
            var parameterTypeTPredicate0Ref = (ByReferenceType)method.Parameters[1].ParameterType;
            body.Variables.Add(new VariableDefinition(parameterTypeTPredicate0Ref));
            var typeReferenceEnumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            body.Variables.Add(new VariableDefinition(typeReferenceEnumerator));
            var typeReferenceElement = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            body.Variables.Add(new VariableDefinition(typeReferenceElement.MakeByReferenceType()));
            body.Variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));

            var processor = body.GetILProcessor();
            var il000E = Instruction.Create(OpCodes.Ldloca_S, body.Variables[1]);
            var il0024 = Instruction.Create(OpCodes.Ldloc_0);

            processor.Do(OpCodes.Ldarg_1);
            var methodDefinitionAsRef = new GenericInstanceMethod(MainModule.ImportReference(SystemUnsafeType.Methods.Single(x => x.Name == "AsRef" && x.Parameters.First().ParameterType.Name != "Void*")));
            methodDefinitionAsRef.GenericArguments.Add(parameterTypeTPredicate0Ref.ElementType);
            processor.Call(methodDefinitionAsRef);
            processor.Do(OpCodes.Stloc_0);
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(@this.FindMethod("GetEnumerator", x => x.Parameters.Count == 0));
            processor.Do(OpCodes.Stloc_1);
            processor.Append(il000E);
            processor.Do(OpCodes.Ldloca_S, body.Variables[3]);
            if (typeReferenceEnumerator is GenericInstanceType genericInstanceEnumerator)
            {
                var methodReferenceTryGetNext = genericInstanceEnumerator.FindMethod("TryGetNext");
                processor.Call(methodReferenceTryGetNext);
                processor.Do(OpCodes.Stloc_2);
                processor.Do(OpCodes.Ldloc_3);
                processor.Append(Instruction.Create(OpCodes.Brtrue_S, il0024));
                processor.Do(OpCodes.Ldloca_S, body.Variables[1]);
                var methodReferenceDispose = genericInstanceEnumerator.FindMethod("Dispose", x => x.Parameters.Count == 0);
                processor.Call(methodReferenceDispose);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Do(OpCodes.Ret);
                processor.Append(il0024);
                processor.Do(OpCodes.Ldloc_2);
                processor.Append(Instruction.Create(OpCodes.Constrained, ((ByReferenceType)method.Parameters[1].ParameterType).ElementType));
                processor.Append(Instruction.Create(OpCodes.Callvirt, MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[] { typeReferenceElement, MainModule.TypeSystem.Boolean }).FindMethod("Calc")));
                processor.Append(Instruction.Create(OpCodes.Brfalse_S, il000E));
                processor.Do(OpCodes.Ldloca_S, body.Variables[1]);
                processor.Call(methodReferenceDispose);
            }
            else
            {
                throw new Exception();
            }
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Do(OpCodes.Ret);
        }
    }
}
