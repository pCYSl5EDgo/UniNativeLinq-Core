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
    public static class TryGetMinFuncHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetMinFuncHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
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
                method.Parameters.Capacity = 3;
                method.GenericParameters.Capacity = type.GenericParameters.Count;
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

            static void FillGenericParameters(GenericInstanceType @this, Collection<GenericParameter> typeGenericParameters, Dictionary<string, GenericParameter> parameterDictionary, MethodDefinition method)
            {
                foreach (var typeGenericParameter in typeGenericParameters)
                {
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
                        var newConstraint = new GenericInstanceType(constraint.GetElementType().Resolve());
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
                var funcType = MainModule.ImportReference(typeof(Func<,>)).MakeGenericType(new[] { @this.GenericArguments.First(x => x.Name == "T"), fillTypeReference });
                method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, funcType));
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, fillTypeReference.MakeByReferenceType())
                {
                    IsOut = true,
                });
            }
            static void FillBody(GenericInstanceType @this, MethodDefinition method, TypeReference fillTypeReference)
            {
                var body = method.Body;
                body.Variables.Clear();
                var enumeratorType = @this.FindNested("Enumerator");
                body.Variables.Add(new VariableDefinition(enumeratorType));
                var typeReferenceT = @this.GenericArguments.Single(x => x.Name == "T");
                body.Variables.Add(new VariableDefinition(typeReferenceT.MakeByReferenceType()));
                body.Variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));
                body.Variables.Add(new VariableDefinition(fillTypeReference));

                var il = body.GetILProcessor();
                il.Append(Instruction.Create(OpCodes.Ldarg_0));
                il.Append(Instruction.Create(OpCodes.Call, @this.FindMethod("GetEnumerator")));
                il.Append(Instruction.Create(OpCodes.Stloc_0));
                il.Append(Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]));
                il.Append(Instruction.Create(OpCodes.Ldloca_S, body.Variables[2]));
                var methodReferenceTryGetNext = enumeratorType.FindMethod("TryGetNext");
                il.Append(Instruction.Create(OpCodes.Call, methodReferenceTryGetNext));
                il.Append(Instruction.Create(OpCodes.Stloc_1));
                il.Append(Instruction.Create(OpCodes.Ldloc_2));
                var il0020 = Instruction.Create(OpCodes.Ldarg_2);
                il.Append(Instruction.Create(OpCodes.Brtrue_S, il0020));
                il.Append(Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]));
                var methodReferenceDispose = enumeratorType.FindMethod("Dispose");
                il.Append(Instruction.Create(OpCodes.Call, methodReferenceDispose));
                il.Append(Instruction.Create(OpCodes.Ldarg_2));
                il.Append(Instruction.Create(OpCodes.Ldc_I4_0));
                il.Append(Instruction.Create(OpCodes.Stind_I1));
                il.Append(Instruction.Create(OpCodes.Ldc_I4_0));
                il.Append(Instruction.Create(OpCodes.Ret));
                il.Append(il0020);
                il.Append(Instruction.Create(OpCodes.Ldarg_1));
                il.Append(Instruction.Create(OpCodes.Ldloc_1));
                il.Append(Instruction.Create(OpCodes.Ldobj, typeReferenceT));
                var methodReferenceFuncInvoke = MainModule.ImportReference(typeof(Func<,>)).MakeGenericType(new[] {@this.GenericArguments.First(x => x.Name == "T"), fillTypeReference}).FindMethodAndImport("Invoke", MainModule);
                il.Append(Instruction.Create(OpCodes.Callvirt, methodReferenceFuncInvoke));
                il.Append(Instruction.Create(OpCodes.Stind_I1));
                var il002E = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
                il.Append(il002E);
                il.Append(Instruction.Create(OpCodes.Ldloca_S, body.Variables[2]));
                il.Append(Instruction.Create(OpCodes.Call, methodReferenceTryGetNext));
                il.Append(Instruction.Create(OpCodes.Stloc_1));
                il.Append(Instruction.Create(OpCodes.Ldloc_2));
                var il0052 = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
                il.Append(Instruction.Create(OpCodes.Brfalse_S, il0052));
                il.Append(Instruction.Create(OpCodes.Ldarg_1));
                il.Append(Instruction.Create(OpCodes.Ldloc_1));
                il.Append(Instruction.Create(OpCodes.Ldobj, typeReferenceT));
                il.Append(Instruction.Create(OpCodes.Callvirt, methodReferenceFuncInvoke));
                il.Append(Instruction.Create(OpCodes.Stloc_3));
                il.Append(Instruction.Create(OpCodes.Ldloc_3));
                il.Append(Instruction.Create(OpCodes.Ldarg_2));
                il.Append(Instruction.Create(OpCodes.Ldind_U1));
                il.Append(Instruction.Create(OpCodes.Bge_S, il002E));
                il.Append(Instruction.Create(OpCodes.Ldarg_2));
                il.Append(Instruction.Create(OpCodes.Ldloc_3));
                il.Append(Instruction.Create(OpCodes.Stind_I1));
                il.Append(Instruction.Create(OpCodes.Br_S, il002E));
                il.Append(il0052);
                il.Append(Instruction.Create(OpCodes.Call, methodReferenceDispose));
                il.Append(Instruction.Create(OpCodes.Ldc_I4_1));
                il.Append(Instruction.Create(OpCodes.Ret));
            }

            FillGenericParameters(@this, typeGenericParameters, parameterDictionary, method);
            FillConstraint(typeGenericParameters, parameterDictionary, fillTypeReference);
            FillParameter(@this, method, fillTypeReference);
            FillBody(@this, method, fillTypeReference);
        }
    }
}
