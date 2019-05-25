using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    using static Program;
    public static class TryGetMinHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetMinHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                if (type.Name == "GroupByEnumerable`8") continue;
                @static.TryGetMin(type);
            }
        }

        private static void TryGetMin(this TypeDefinition @static, TypeDefinition type)
        {
            static void WithType(TypeDefinition @static, TypeDefinition type, TypeReference fillType)
            {
                var method = new MethodDefinition(nameof(TryGetMin), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
                {
                    DeclaringType = @static,
                    AggressiveInlining = true,
                };
                method.CustomAttributes.Add(ExtensionAttribute);
                method.Parameters.Capacity = 2;
                method.GenericParameters.Capacity = type.GenericParameters.Count - 1;
                method.TryGetMinMethodFillTypeArgument(type, fillType);
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

        private static void TryGetMinMethodFillTypeArgument(this MethodDefinition method, TypeDefinition collectionTypeDefinition, TypeReference fillTypeReference)
        {
            var @this = new GenericInstanceType(collectionTypeDefinition);
            var typeGenericParameters = collectionTypeDefinition.GenericParameters;
            var parameterDictionary = new Dictionary<string, GenericParameter>();

            static void FillGenericParameters(GenericInstanceType @this, Collection<GenericParameter> typeGenericParameters, Dictionary<string, GenericParameter> parameterDictionary, MethodDefinition method, TypeReference fillTypeReference)
            {
                foreach (var typeGenericParameter in typeGenericParameters)
                {
                    if (typeGenericParameter.Name == "T")
                    {
                        @this.GenericArguments.Add(fillTypeReference);
                        continue;
                    }
                    var genericParameter = new GenericParameter(typeGenericParameter.Name, method)
                    {
                        HasNotNullableValueTypeConstraint = typeGenericParameter.HasNotNullableValueTypeConstraint,
                        HasDefaultConstructorConstraint = typeGenericParameter.HasDefaultConstructorConstraint,
                        IsContravariant = typeGenericParameter.IsContravariant,
                        IsValueType = typeGenericParameter.IsValueType,
                    };
                    foreach (var customAttribute in typeGenericParameter.CustomAttributes)
                        genericParameter.CustomAttributes.Add(customAttribute);
                    genericParameter.Constraints.Clear();
                    method.GenericParameters.Add(genericParameter);
                    @this.GenericArguments.Add(genericParameter);
                    parameterDictionary.Add(genericParameter.Name, genericParameter);
                }
            }
            static void FillConstraint(Collection<GenericParameter> typeGenericParameters, Dictionary<string, GenericParameter> parameterDictionary, TypeReference fillTypeReference)
            {
                foreach (var typeGenericParameter in typeGenericParameters)
                {
                    if (!parameterDictionary.TryGetValue(typeGenericParameter.Name, out var genericParameter) || !typeGenericParameter.HasConstraints)
                        continue;
                    foreach (var constraint in typeGenericParameter.Constraints)
                    {
                        if (!constraint.IsGenericInstance)
                        {
                            genericParameter.Constraints.Add(constraint);
                            continue;
                        }
                        var newConstraint = new GenericInstanceType(MainModule.GetType(constraint.Namespace, constraint.Name));
                        foreach (var genericParameterConstraint in ((GenericInstanceType)constraint).GenericArguments)
                        {
                            var reference = parameterDictionary.TryGetValue(genericParameterConstraint.Name, out var constraintGenericParameter) ? constraintGenericParameter : fillTypeReference;
                            newConstraint.GenericArguments.Add(reference);
                        }
                        genericParameter.Constraints.Add(newConstraint);
                    }
                }
            }
            static void FillParameter(GenericInstanceType @this, MethodDefinition method, TypeReference fillTypeReference)
            {
                var @thisParameter = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType())
                {
                    IsIn = true,
                };
                @thisParameter.CustomAttributes.Add(IsReadOnlyAttribute);
                method.Parameters.Add(@thisParameter);
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, fillTypeReference.MakeByReferenceType())
                {
                    IsOut = true,
                });
            }
            static void FillBody(GenericInstanceType @this, MethodDefinition method, TypeReference fillTypeReference)
            {
                var body = method.Body;
                body.Variables.Clear();
                var enumeratorType = new GenericInstanceType(((TypeDefinition)@this.ElementType).NestedTypes.First(x => x.Name.EndsWith("Enumerator")));
                foreach (var argument in @this.GenericArguments)
                    enumeratorType.GenericArguments.Add(argument);
                body.Variables.Add(new VariableDefinition(enumeratorType));
                body.Variables.Add(new VariableDefinition(fillTypeReference));

                var il = body.GetILProcessor();
                il.Append(Instruction.Create(OpCodes.Ldarg_0));
                il.Append(Instruction.Create(OpCodes.Call, @this.FindMethod("GetEnumerator")));
                il.Append(Instruction.Create(OpCodes.Stloc_0));
                il.Append(Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]));
                il.Append(Instruction.Create(OpCodes.Ldarg_1));
                var methodReferenceTryMoveNext = enumeratorType.FindMethod("TryMoveNext");
                il.Append(Instruction.Create(OpCodes.Call, methodReferenceTryMoveNext));
                var il001A = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
                il.Append(Instruction.Create(OpCodes.Brtrue_S, il001A));
                il.Append(Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]));
                var methodReferenceDispose = enumeratorType.FindMethod("Dispose", x => !x.Parameters.Any());
                il.Append(Instruction.Create(OpCodes.Call, methodReferenceDispose));
                il.Append(Instruction.Create(OpCodes.Ldc_I4_0));
                il.Append(Instruction.Create(OpCodes.Ret));
                il.Append(il001A);
                il.Append(Instruction.Create(OpCodes.Ldloca_S, body.Variables[1]));
                il.Append(Instruction.Create(OpCodes.Call, methodReferenceTryMoveNext));
                var il002F = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
                il.Append(Instruction.Create(OpCodes.Brfalse_S, il002F));
                il.Append(Instruction.Create(OpCodes.Ldloc_1));
                il.Append(Instruction.Create(OpCodes.Ldarg_1));
                il.Append(Instruction.Create(OpCodes.Ldind_U1));
                il.Append(Instruction.Create(OpCodes.Bge_S, il001A));
                il.Append(Instruction.Create(OpCodes.Ldarg_1));
                il.Append(Instruction.Create(OpCodes.Ldloc_1));
                il.Append(Instruction.Create(OpCodes.Stind_I1));
                il.Append(Instruction.Create(OpCodes.Br_S, il001A));
                il.Append(il002F);
                il.Append(Instruction.Create(OpCodes.Call, methodReferenceDispose));
                il.Append(Instruction.Create(OpCodes.Ldc_I4_1));
                il.Append(Instruction.Create(OpCodes.Ret));
            }

            FillGenericParameters(@this, typeGenericParameters, parameterDictionary, method, fillTypeReference);
            FillConstraint(typeGenericParameters, parameterDictionary, fillTypeReference);
            FillParameter(@this, method, fillTypeReference);
            FillBody(@this, method, fillTypeReference);
        }
    }
}
