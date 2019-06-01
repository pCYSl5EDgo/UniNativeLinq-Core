using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    using static Program;

    public static class TryGetMaxFuncHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetMaxFuncHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
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
            var elementTypeOfCollectionType = @this.GetElementTypeOfCollectionType();
            var typeReference = elementTypeOfCollectionType.Replace(method.GenericParameters);
            var funcType = typeof(Func<,>).ImportGenericType(MainModule, new[] {typeReference, fillTypeReference});
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, funcType));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, fillTypeReference.MakeByReferenceType())
            {
                IsOut = true,
            });
        }

        internal static void FillBody(GenericInstanceType @this, MethodDefinition method, TypeReference fillTypeReference, OpCode jump)
        {
            var body = method.Body;
            body.Variables.Clear();
            var enumeratorType = @this.FindNested("Enumerator");
            body.Variables.Add(new VariableDefinition(enumeratorType));
            var typeReferenceElement = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            body.Variables.Add(new VariableDefinition(typeReferenceElement.MakeByReferenceType()));
            body.Variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(fillTypeReference));

            var il = body.GetILProcessor();
            il.Append(Instruction.Create(OpCodes.Ldarg_0));
            il.Append(Instruction.Create(OpCodes.Call, @this.FindMethod("GetEnumerator", x => x.Parameters.Count == 0)));
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
            var methodReferenceDispose = enumeratorType.FindMethod("Dispose", x => x.Parameters.Count == 0);
            il.Append(Instruction.Create(OpCodes.Call, methodReferenceDispose));
            il.Append(Instruction.Create(OpCodes.Ldarg_2));
            il.Append(Instruction.Create(OpCodes.Ldc_I4_0));
            il.Append(Instruction.Create(OpCodes.Stind_I1));
            il.Append(Instruction.Create(OpCodes.Ldc_I4_0));
            il.Append(Instruction.Create(OpCodes.Ret));
            il.Append(il0020);
            il.Append(Instruction.Create(OpCodes.Ldarg_1));
            il.Append(Instruction.Create(OpCodes.Ldloc_1));
            il.Append(Instruction.Create(OpCodes.Ldobj, typeReferenceElement));
            var methodReferenceFuncInvoke = typeof(Func<,>).FindMethodImportGenericType(MainModule, "Invoke", new[] { typeReferenceElement, fillTypeReference });
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
            il.Append(Instruction.Create(OpCodes.Ldobj, typeReferenceElement));
            il.Append(Instruction.Create(OpCodes.Callvirt, methodReferenceFuncInvoke));
            il.Append(Instruction.Create(OpCodes.Stloc_3));
            il.Append(Instruction.Create(OpCodes.Ldloc_3));
            il.Append(Instruction.Create(OpCodes.Ldarg_2));
            il.Append(Instruction.Create(OpCodes.Ldind_U1));
            il.Append(Instruction.Create(jump, il002E));
            il.Append(Instruction.Create(OpCodes.Ldarg_2));
            il.Append(Instruction.Create(OpCodes.Ldloc_3));
            il.Append(Instruction.Create(OpCodes.Stind_I1));
            il.Append(Instruction.Create(OpCodes.Br_S, il002E));
            il.Append(il0052);
            il.Append(Instruction.Create(OpCodes.Call, methodReferenceDispose));
            il.Append(Instruction.Create(OpCodes.Ldc_I4_1));
            il.Append(Instruction.Create(OpCodes.Ret));
        }
    }
}