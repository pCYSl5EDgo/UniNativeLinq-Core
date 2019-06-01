using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    using static Program;
    public static class TryGetMaxOperatorHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetMaxOperatorHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                @static.TryGetMax(type);
            }
        }

        private static void TryGetMax(this TypeDefinition @static, TypeDefinition type)
        {
            static void WithType(TypeDefinition @static, TypeDefinition type, TypeReference fillType)
            {
                var method = new MethodDefinition(nameof(TryGetMax), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
                {
                    DeclaringType = @static,
                    AggressiveInlining = true,
                };
                method.CustomAttributes.Add(ExtensionAttribute);
                method.Parameters.Capacity = 3;
                method.GenericParameters.Capacity = type.GenericParameters.Count;
                method.TryGetMaxMethodFillTypeArgument(type, fillType);
                @static.Methods.Add(method);
            }
            WithType(@static, type, MainModule.TypeSystem.Byte);
            WithType(@static, type, MainModule.TypeSystem.SByte);
            WithType(@static, type, MainModule.TypeSystem.Int16);
            WithType(@static, type, MainModule.TypeSystem.UInt16);
            WithType(@static, type, MainModule.TypeSystem.Int32);
            WithType(@static, type, MainModule.TypeSystem.UInt32);
            WithType(@static, type, MainModule.TypeSystem.Int64);
            WithType(@static, type, MainModule.TypeSystem.UInt64);
            WithType(@static, type, MainModule.TypeSystem.Single);
            WithType(@static, type, MainModule.TypeSystem.Double);
        }

        private static void TryGetMaxMethodFillTypeArgument(this MethodDefinition method, TypeDefinition collectionTypeDefinition, TypeReference fillTypeReference)
        {
            var addedParams = method.FromTypeToMethodParam(collectionTypeDefinition.GenericParameters);
            var @this = collectionTypeDefinition.MakeGenericInstanceType(addedParams);
            FillParameter(@this, method, fillTypeReference);
            FillBody(@this, method, fillTypeReference, OpCodes.Ble_S);
        }

        internal static void FillParameter(GenericInstanceType @this, MethodDefinition method, TypeReference fillTypeReference)
        {
            var @thisParameter = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType())
            {
                IsIn = true,
            };
            @thisParameter.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(@thisParameter);
            var typeReference = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var genericParameter = new GenericParameter("TFunc0", method)
            {
                HasNotNullableValueTypeConstraint = true,
                HasDefaultConstructorConstraint = true,
                IsNonVariant = true,
            };
            genericParameter.Constraints.Add(MainModule.ImportReference(typeof(ValueType)));
            genericParameter.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[]
            {
                typeReference,
                fillTypeReference
            }));
            method.GenericParameters.Add(genericParameter);
            var funcParameter = new ParameterDefinition("func", ParameterAttributes.In, genericParameter.MakeByReferenceType());
            funcParameter.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(funcParameter);
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, fillTypeReference.MakeByReferenceType())
            {
                IsOut = true,
            });
        }

        internal static void FillBody(GenericInstanceType @this, MethodDefinition method, TypeReference fillTypeReference, OpCode jump)
        {
            var body = method.Body;
            var typeReferenceEnumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            body.Variables.Add(new VariableDefinition(typeReferenceEnumerator));
            var typeReferenceElement = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            body.Variables.Add(new VariableDefinition(typeReferenceElement.MakeByReferenceType()));
            body.Variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(fillTypeReference));

            var il0020 = Instruction.Create(OpCodes.Ldarg_2);
            var il0030 = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
            var il0046 = Instruction.Create(OpCodes.Ldarg_1);

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(@this.FindMethod("GetEnumerator", Helper.NoParameter));
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.LdLocaS(2);
            if (!(typeReferenceEnumerator is GenericInstanceType enumerator))
                throw new Exception();
            var methodReferenceTryGetNext = enumerator.FindMethod("TryGetNext");
            processor.Call(methodReferenceTryGetNext);
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.True(il0020);
            processor.LdLocaS(0);
            var methodReferenceDispose = enumerator.FindMethod("Dispose", Helper.NoParameter);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Stind_I1);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Ret);
            processor.Append(il0020);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldloc_1);
            var typeReferenceFunc0 = method.Parameters[1].ParameterType;
            processor.Constrained(typeReferenceFunc0);
            var methodReferenceCalc = MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[] { typeReferenceElement, fillTypeReference }).FindMethod("Calc");
            processor.CallVirtual(methodReferenceCalc);
            processor.Do(OpCodes.Stind_I1);
            processor.Append(il0030);
            processor.LdLocaS(2);
            processor.Call(methodReferenceTryGetNext);
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.True(il0046);
            processor.LdLocaS(0);
            processor.Call(methodReferenceDispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Do(OpCodes.Ret);
            processor.Append(il0046);
            processor.Do(OpCodes.Ldloc_1);
            processor.Constrained(typeReferenceFunc0);
            processor.CallVirtual(methodReferenceCalc);
            processor.Do(OpCodes.Stloc_3);
            processor.Do(OpCodes.Ldloc_3);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldind_U1);
            processor.Append(Instruction.Create(jump, il0030));
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldloc_3);
            processor.Do(OpCodes.Stind_I1);
            processor.Append(Instruction.Create(OpCodes.Br_S, il0030));
        }
    }
}
