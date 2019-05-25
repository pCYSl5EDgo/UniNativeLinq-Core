#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using CecilRewrite;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
// ReSharper disable VariableHidesOuterVariable

namespace CecilRewriteAndAddExtensions
{
    static class Program
    {
        private const string NameSpace = "UniNativeLinq";

        private const TypeAttributes StaticExtensionClassTypeAttributes = TypeAttributes.AnsiClass | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Abstract;
        private const MethodAttributes StaticMethodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        private static readonly ModuleDefinition MainModule;
        private static readonly ModuleDefinition UnsafeModule;
        private static readonly CustomAttribute ExtensionAttribute;
        private static readonly CustomAttribute IsReadOnlyAttribute;
        private static readonly ModuleDefinition UnityModule;
        private static readonly AssemblyDefinition Assembly;

        static Program()
        {
            var location = @"C:\Users\conve\source\repos\pcysl5edgo\CecilRewrite\bin\Release\UniNativeLinq.dll";
            Console.WriteLine(location);
            Assembly = AssemblyDefinition.ReadAssembly(location);
            MainModule = Assembly.MainModule;
            var nativeEnumerable = MainModule.GetType("UniNativeLinq.NativeEnumerable");
            ExtensionAttribute = nativeEnumerable.CustomAttributes.Single();
            var negateMethodDefinition = MainModule.GetType("UniNativeLinq.NegatePredicate`2").GetConstructors().First();
            IsReadOnlyAttribute = negateMethodDefinition.Parameters.First().CustomAttributes.First();
            UnsafeModule = ExtensionAttribute.AttributeType.Module;
            UnityModule = nativeEnumerable.Methods.First(x => x.Parameters.First().ParameterType.IsValueType).Parameters.First().ParameterType.Module;
        }

        private static void Main(string[] args)
        {
            RewriteThrow(MainModule);
            MinMaxHelper(MainModule);
            Assembly.Write(@"C:\Users\conve\source\repos\pcysl5edgo\UniNativeLinq\bin\Release\UniNativeLinq.dll");
            Console.ReadLine();
        }

        private static void MinMaxHelper(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(MinMaxHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            var z = MainModule.GetType("UniNativeLinq.NativeEnumerable").Methods.First();
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
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
                Console.WriteLine(type);
                method.CustomAttributes.Add(ExtensionAttribute);
                method.Parameters.Capacity = 2;
                method.GenericParameters.Capacity = type.GenericParameters.Count - 1;
                method.TryGetMinMethodFillTypeArgument(type, fillType);
                @static.Methods.Add(method);
            }
            WithType(@static, type, MainModule.TypeSystem.Byte);
            //WithType(@static, type, MainModule.TypeSystem.SByte);
            //WithType(@static, type, MainModule.TypeSystem.Int16);
            //WithType(@static, type, MainModule.TypeSystem.UInt16);
            //WithType(@static, type, MainModule.TypeSystem.Int32);
            //WithType(@static, type, MainModule.TypeSystem.UInt32);
            //WithType(@static, type, MainModule.TypeSystem.Int64);
            //WithType(@static, type, MainModule.TypeSystem.UInt64);
            //WithType(@static, type, MainModule.TypeSystem.Single);
            //WithType(@static, type, MainModule.TypeSystem.Double);
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
                    genericParameter.Constraints.Clear();
                    method.GenericParameters.Add(genericParameter);
                    @this.GenericArguments.Add(genericParameter);
                    parameterDictionary.Add(genericParameter.Name, genericParameter);
                }
            }
            static void FillConstraint(GenericInstanceType @this, Collection<GenericParameter> typeGenericParameters, Dictionary<string, GenericParameter> parameterDictionary, TypeReference fillTypeReference)
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
                var @thisParameter = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
                @thisParameter.CustomAttributes.Add(IsReadOnlyAttribute);
                method.Parameters.Add(@thisParameter);
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, fillTypeReference.MakeByReferenceType()));
            }
            static void FillBody(GenericInstanceType @this, MethodDefinition method, TypeReference fillTypeReference)
            {
                var body = method.Body;
                body.Variables.Clear();
                var enumeratorType = new GenericInstanceType(((TypeDefinition)@this.ElementType).NestedTypes.First(x => x.Name.EndsWith("Enumerator")));
                foreach (var argument in @this.GenericArguments)
                    enumeratorType.GenericArguments.Add(argument);
                body.Variables.Add(new VariableDefinition(enumeratorType));
                var typeSystemBoolean = MainModule.TypeSystem.Boolean;
                body.Variables.Add(new VariableDefinition(typeSystemBoolean));
                body.Variables.Add(new VariableDefinition(typeSystemBoolean));
                body.Variables.Add(new VariableDefinition(fillTypeReference));
                body.Variables.Add(new VariableDefinition(typeSystemBoolean));
                body.Variables.Add(new VariableDefinition(typeSystemBoolean));
                var il = body.GetILProcessor();

                var loopStart = il.Create(OpCodes.Ldloca_S, body.Variables[0]);
                var end = il.Create(OpCodes.Ldloc_2);
                var dispose = il.Create(OpCodes.Ldloca_S, body.Variables[0]);
                var loopEnd = il.Create(OpCodes.Br_S, loopStart);

                il.Append(il.Create(OpCodes.Ldarg_0));
                il.Append(il.Create(OpCodes.Call, new MethodReference("GetEnumerator", enumeratorType, @this)));
                il.Append(il.Create(OpCodes.Stloc_0));
                il.Append(il.Create(OpCodes.Ldloca_S, body.Variables[0]));
                il.Append(il.Create(OpCodes.Ldarg_1));
                var tryMoveNext = enumeratorType.ElementType.Resolve().Methods.First(x => x.Name == "TryMoveNext").MakeHostInstanceGeneric(enumeratorType.GenericArguments);
                il.Append(il.Create(OpCodes.Call, tryMoveNext));
                il.Append(il.Create(OpCodes.Ldc_I4_0));
                il.Append(il.Create(OpCodes.Ceq));
                il.Append(il.Create(OpCodes.Stloc_1));
                il.Append(il.Create(OpCodes.Ldloc_1));
                il.Append(il.Create(OpCodes.Brfalse_S, loopStart));
                il.Append(il.Create(OpCodes.Ldloca_S, body.Variables[0]));
                var disposeMethodReference = new MethodReference("Dispose", MainModule.TypeSystem.Void, enumeratorType);
                il.Append(il.Create(OpCodes.Call, disposeMethodReference));
                il.Append(il.Create(OpCodes.Ldc_I4_0));
                il.Append(il.Create(OpCodes.Stloc_2));
                il.Append(il.Create(OpCodes.Br_S, end));
                il.Append(loopStart);
                il.Append(il.Create(OpCodes.Ldloca_S, body.Variables[3]));
                il.Append(il.Create(OpCodes.Call, tryMoveNext));
                il.Append(il.Create(OpCodes.Stloc_S, body.Variables[4]));
                il.Append(il.Create(OpCodes.Ldloc_S, body.Variables[4]));
                il.Append(il.Create(OpCodes.Brfalse_S, dispose));
                il.Append(il.Create(OpCodes.Ldloc_3));
                il.Append(il.Create(OpCodes.Ldarg_1));
                il.Append(il.Create(OpCodes.Ldind_U1));
                il.Append(il.Create(OpCodes.Clt));
                il.Append(il.Create(OpCodes.Stloc_S, body.Variables[5]));
                il.Append(il.Create(OpCodes.Ldloc_S, body.Variables[5]));
                il.Append(il.Create(OpCodes.Brfalse_S, loopEnd));
                il.Append(il.Create(OpCodes.Ldarg_1));
                il.Append(il.Create(OpCodes.Ldloc_3));
                il.Append(il.Create(OpCodes.Stind_I1));
                il.Append(loopEnd);
                il.Append(dispose);
                il.Append(il.Create(OpCodes.Call, disposeMethodReference));
                il.Append(il.Create(OpCodes.Ldc_I4_1));
                il.Append(il.Create(OpCodes.Stloc_2));
                il.Append(il.Create(OpCodes.Br_S, end));
                il.Append(end);
                il.Append(il.Create(OpCodes.Ret));
            }

            FillGenericParameters(@this, typeGenericParameters, parameterDictionary, method, fillTypeReference);
            FillConstraint(@this, typeGenericParameters, parameterDictionary, fillTypeReference);
            FillParameter(@this, method, fillTypeReference);
            FillBody(@this, method, fillTypeReference);
        }

        private static void RewriteThrow(ModuleDefinition module)
        {
            var enumeratorTypes = module.GetTypes()
                .Where(x => x.IsPublic && x.IsValueType && x.HasNestedTypes)
                .SelectMany(x => x.NestedTypes)
                .Where(x => x.HasCustomAttributes && x.CustomAttributes.Any(y => y.AttributeType.Name == "LocalRefReturnAttribute"));
            Console.WriteLine("rewrite 'throw new NotImplemented' to return ref element;");
            foreach (var enumeratorType in enumeratorTypes)
            {
                //Console.WriteLine(enumeratorType.FullName);
                ReWriteRefReturn(module, enumeratorType);
            }
        }

        private static void ReWriteRefReturn(ModuleDefinition module, TypeDefinition type)
        {
            var element = type.Fields.First(x => x.Name == "element").FillGenericParams();
            ReWrite(type.Methods.First(x => x.Name == "TryGetNext"), element);
            static bool Predicate(PropertyDefinition x)
                => x.Name == "Current" && x.PropertyType.IsByReference;
            ReWrite(type.Properties.First(Predicate).GetMethod, element);
        }

        private static FieldReference FillGenericParams(this FieldDefinition definition)
        {
            var declaringType = new GenericInstanceType(definition.DeclaringType);
            foreach (var param in definition.DeclaringType.GenericParameters)
                declaringType.GenericArguments.Add(param);
            return new FieldReference(definition.Name, definition.FieldType, declaringType);
        }

        private static void ReWrite(MethodDefinition method, FieldReference field)
        {
            var body = method.Body;
            var processor = body.GetILProcessor();
            var ldflda = processor.Create(OpCodes.Ldflda, field);
            var ret = processor.Create(OpCodes.Ret);
            var ldarg0 = processor.Create(OpCodes.Ldarg_0);
            for (var i = body.Instructions.Count; --i >= 0;)
            {
                var instruction = body.Instructions[i];
                if (instruction.OpCode == OpCodes.Throw)
                    processor.Replace(instruction, ret);
                else if (instruction.OpCode == OpCodes.Newobj)
                {
                    processor.InsertBefore(instruction, ldarg0);
                    processor.Replace(instruction, ldflda);
                }
            }
        }
    }
}
