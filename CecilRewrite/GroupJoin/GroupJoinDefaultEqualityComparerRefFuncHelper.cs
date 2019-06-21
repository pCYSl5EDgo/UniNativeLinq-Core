using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;

    static class GroupJoinDefaultEqualityComparerRefFuncHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(GroupJoinDefaultEqualityComparerRefFuncHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);

            foreach (var type0 in Enumerables)
            {
                GroupJoinOuter(@static, type0, "Array", CalcArrayType, AsRefEnumerableArray);
                GroupJoinInner(@static, type0, "Array", CalcArrayType, AsRefEnumerableArray);
                GroupJoinOuter(@static, type0, "Native", NativeArray.MakeGenericInstanceType, AsRefEnumerableNative);
                GroupJoinInner(@static, type0, "Native", NativeArray.MakeGenericInstanceType, AsRefEnumerableNative);
                foreach (var type1 in Enumerables)
                    GroupJoin(@static, type0, type1);
            }
        }

        private static void GroupJoinOuter(TypeDefinition @static, TypeDefinition type, string prefix, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodDefinition constructor)
        {
            var MainModule = Prologue(@static, out var method, out var TKey, out var T, out var DefaultTKeyEqualityComparer);

            var Element0 = new GenericParameter("T0", method) { HasNotNullableValueTypeConstraint = true };
            Element0.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(Element0);
            var Enumerable0 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(Element0);
            var Enumerator0 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element0);

            var TOuterKeySelectorFunc = InternalOuterRoutine(Element0, TKey, MainModule, out var TOuterKeySelector);

            var Enumerable1 = InnerRoutine(type, method, TKey, MainModule, DefaultTKeyEqualityComparer, out var Enumerator1, out var Element1, out var TInnerKeySelectorFunc, out var TInnerKeySelector, out var WhereIndexEnumerable);

            var TSelectorFunc = Epilogue(Element0, WhereIndexEnumerable, T, MainModule, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, TKey, TOuterKeySelector, TInnerKeySelector, DefaultTKeyEqualityComparer, method, out var TSelector, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.None, InputFunc(new[] { Element0 }));
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            var selector = DefineParameters(TOuterKeySelectorFunc, method, TInnerKeySelectorFunc, TSelectorFunc, out var allocator);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(Element0);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(TOuterKeySelector));
            variables.Add(new VariableDefinition(TInnerKeySelector));
            variables.Add(new VariableDefinition(DefaultTKeyEqualityComparer));
            variables.Add(new VariableDefinition(TSelector));
            variables.Add(new VariableDefinition(Enumerable0));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(_constructor);
            processor.Append(Instruction.Create(OpCodes.Stloc_S, variables[4]));
            processor.LdLocaS(4);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_3);
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.LdLocaS(2);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Do(OpCodes.Stloc_3);
            processor.LdLocaS(3);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void GroupJoinInner(TypeDefinition @static, TypeDefinition type, string prefix, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodDefinition constructor)
        {
            var MainModule = Prologue(@static, out var method, out var TKey, out var T, out var DefaultTKeyEqualityComparer);

            var Enumerable0 = OuterRoutine(type, method, TKey, MainModule, out var Enumerator0, out var Element0, out var TOuterKeySelectorFunc, out var TOuterKeySelector);

            var Element1 = new GenericParameter("T1", method) { HasNotNullableValueTypeConstraint = true };
            Element1.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(Element1);
            var Enumerable1 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(Element1);
            var Enumerator1 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element1);

            var TInnerKeySelectorFunc = InternalInnerRoutine(Element1, TKey, MainModule, DefaultTKeyEqualityComparer, out var TInnerKeySelector, out var WhereIndexEnumerable);

            var TSelectorFunc = Epilogue(Element0, WhereIndexEnumerable, T, MainModule, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, TKey, TOuterKeySelector, TInnerKeySelector, DefaultTKeyEqualityComparer, method, out var TSelector, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.None, InputFunc(new[] { Element1 }));
            method.Parameters.Add(inner);

            var selector = DefineParameters(TOuterKeySelectorFunc, method, TInnerKeySelectorFunc, TSelectorFunc, out var allocator);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(Element1);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(TOuterKeySelector));
            variables.Add(new VariableDefinition(TInnerKeySelector));
            variables.Add(new VariableDefinition(DefaultTKeyEqualityComparer));
            variables.Add(new VariableDefinition(TSelector));
            variables.Add(new VariableDefinition(Enumerable1));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Call(_constructor);
            processor.Append(Instruction.Create(OpCodes.Stloc_S, variables[4]));
            processor.LdLocaS(4);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_3);
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.LdLocaS(2);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Do(OpCodes.Stloc_3);
            processor.LdLocaS(3);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void GroupJoin(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1)
        {
            var MainModule = Prologue(@static, out var method, out var TKey, out var T, out var DefaultTKeyEqualityComparer);

            var Enumerable0 = OuterRoutine(type0, method, TKey, MainModule, out var Enumerator0, out var Element0, out var TOuterKeySelectorFunc, out var TOuterKeySelector);

            var Enumerable1 = InnerRoutine(type1, method, TKey, MainModule, DefaultTKeyEqualityComparer, out var Enumerator1, out var Element1, out var TInnerKeySelectorFunc, out var TInnerKeySelector, out var WhereIndexEnumerable);

            var TSelectorFunc = Epilogue(Element0, WhereIndexEnumerable, T, MainModule, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, TKey, TOuterKeySelector, TInnerKeySelector, DefaultTKeyEqualityComparer, method, out var TSelector, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            var selector = DefineParameters(TOuterKeySelectorFunc, method, TInnerKeySelectorFunc, TSelectorFunc, out var allocator);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(TOuterKeySelector));
            variables.Add(new VariableDefinition(TInnerKeySelector));
            variables.Add(new VariableDefinition(DefaultTKeyEqualityComparer));
            variables.Add(new VariableDefinition(TSelector));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_3);
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.LdLocaS(2);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Do(OpCodes.Stloc_3);
            processor.LdLocaS(3);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static ParameterDefinition DefineParameters(GenericInstanceType TOuterKeySelectorFunc, MethodDefinition method, GenericInstanceType TInnerKeySelectorFunc, GenericInstanceType TSelectorFunc, out ParameterDefinition allocator)
        {
            var outerSelector = new ParameterDefinition("outerSelector", ParameterAttributes.None, TOuterKeySelectorFunc);
            method.Parameters.Add(outerSelector);

            var innerSelector = new ParameterDefinition("innerSelector", ParameterAttributes.None, TInnerKeySelectorFunc);
            method.Parameters.Add(innerSelector);

            var selector = new ParameterDefinition("selector", ParameterAttributes.None, TSelectorFunc);
            method.Parameters.Add(selector);

            allocator = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
            return selector;
        }

        private static GenericInstanceType Epilogue(TypeReference Element0, GenericInstanceType WhereIndexEnumerable, GenericParameter T, ModuleDefinition MainModule, GenericInstanceType Enumerable0, TypeReference Enumerator0, GenericInstanceType Enumerable1, TypeReference Enumerator1, TypeReference Element1, GenericParameter TKey, GenericInstanceType TOuterKeySelector, GenericInstanceType TInnerKeySelector, GenericInstanceType DefaultTKeyEqualityComparer, MethodDefinition method, out GenericInstanceType TSelector, out GenericInstanceType @return)
        {
            var Tuple3 = new[]
            {
                Element0,
                WhereIndexEnumerable,
                T
            };
            var TSelectorFunc = MainModule.GetType(NameSpace, "RefFunc`3").MakeGenericInstanceType(Tuple3);
            TSelector = MainModule.GetType(NameSpace, "DelegateRefFuncToStructOperatorFunc`3").MakeGenericInstanceType(Tuple3);

            @return = MainModule.GetType(NameSpace, "GroupJoinEnumerable`12").MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Element0,
                Enumerable1,
                Enumerator1,
                Element1,
                TKey,
                TOuterKeySelector,
                TInnerKeySelector,
                DefaultTKeyEqualityComparer,
                T,
                TSelector,
            });
            method.ReturnType = @return;
            return TSelectorFunc;
        }

        private static GenericInstanceType InnerRoutine(TypeDefinition type1, MethodDefinition method, GenericParameter TKey, ModuleDefinition MainModule, GenericInstanceType DefaultTKeyEqualityComparer, out TypeReference Enumerator1, out TypeReference Element1, out GenericInstanceType TInnerKeySelectorFunc, out GenericInstanceType TInnerKeySelector, out GenericInstanceType WhereIndexEnumerable)
        {
            const string suffix1 = "1";
            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, suffix1);
            var Enumerable1 = type1.MakeGenericInstanceType(added1);
            Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, suffix1);
            Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, suffix1);

            TInnerKeySelectorFunc = InternalInnerRoutine(Element1, TKey, MainModule, DefaultTKeyEqualityComparer, out TInnerKeySelector, out WhereIndexEnumerable);
            return Enumerable1;
        }

        private static GenericInstanceType InternalInnerRoutine(TypeReference Element1, GenericParameter TKey, ModuleDefinition MainModule, GenericInstanceType DefaultTKeyEqualityComparer, out GenericInstanceType TInnerKeySelector, out GenericInstanceType WhereIndexEnumerable)
        {
            var Element1_TKey = new[]
            {
                Element1,
                TKey
            };
            var TInnerKeySelectorFunc = MainModule.GetType(NameSpace, "RefFunc`2").MakeGenericInstanceType(Element1_TKey);
            TInnerKeySelector = MainModule.GetType(NameSpace, "DelegateRefFuncToStructOperatorFunc`2").MakeGenericInstanceType(Element1_TKey);

            var NativeEnumerable = MainModule.GetType(NameSpace, "NativeEnumerable`1");
            var InnerNativeEnumerable = NativeEnumerable.MakeGenericInstanceType(Element1);
            var InnerNativeEnumerator = NativeEnumerable.NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element1);

            var GroupJoinPredicate = MainModule.GetType(NameSpace, "GroupJoinPredicate`3").MakeGenericInstanceType(new[]
            {
                Element1,
                TKey,
                DefaultTKeyEqualityComparer
            });

            WhereIndexEnumerable = MainModule.GetType(NameSpace, "WhereIndexEnumerable`4").MakeGenericInstanceType(new[]
            {
                InnerNativeEnumerable,
                InnerNativeEnumerator,
                Element1,
                GroupJoinPredicate,
            });
            return TInnerKeySelectorFunc;
        }

        private static GenericInstanceType OuterRoutine(TypeDefinition type0, MethodDefinition method, GenericParameter TKey, ModuleDefinition MainModule, out TypeReference Enumerator0, out TypeReference Element0, out GenericInstanceType TOuterKeySelectorFunc, out GenericInstanceType TOuterKeySelector)
        {
            const string suffix0 = "0";
            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, suffix0);
            var Enumerable0 = type0.MakeGenericInstanceType(added0);
            Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix0);
            Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, suffix0);

            TOuterKeySelectorFunc = InternalOuterRoutine(Element0, TKey, MainModule, out TOuterKeySelector);
            return Enumerable0;
        }

        private static GenericInstanceType InternalOuterRoutine(TypeReference Element0, GenericParameter TKey, ModuleDefinition MainModule, out GenericInstanceType TOuterKeySelector)
        {
            var Element0_TKey = new[]
            {
                Element0,
                TKey
            };
            var TOuterKeySelectorFunc = MainModule.GetType(NameSpace, "RefFunc`2").MakeGenericInstanceType(Element0_TKey);
            TOuterKeySelector = MainModule.GetType(NameSpace, "DelegateRefFuncToStructOperatorFunc`2").MakeGenericInstanceType(Element0_TKey);
            return TOuterKeySelectorFunc;
        }

        private static ModuleDefinition Prologue(TypeDefinition @static, out MethodDefinition method, out GenericParameter TKey, out GenericParameter T, out GenericInstanceType DefaultTKeyEqualityComparer)
        {
            var MainModule = @static.Module;
            method = new MethodDefinition(nameof(GroupJoin), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            TKey = new GenericParameter(nameof(TKey), method) { HasNotNullableValueTypeConstraint = true };
            TKey.CustomAttributes.Add(UnManagedAttribute);
            TKey.Constraints.Add(MainModule.ImportReference(SystemModule.GetType("System", "IEquatable`1")).MakeGenericInstanceType(TKey));
            method.GenericParameters.Add(TKey);

            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(T);
            DefaultTKeyEqualityComparer = MainModule.GetType(NameSpace, "DefaultEqualityComparer`1").MakeGenericInstanceType(TKey);
            return MainModule;
        }
    }
}
