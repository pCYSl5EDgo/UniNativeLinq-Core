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

    static class GroupJoinDefaultEqualityComparerOperatorHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(GroupJoinDefaultEqualityComparerOperatorHelper),
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

        private static void GroupJoinInner(TypeDefinition @static, TypeDefinition type, string prefix, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodDefinition constructor)
        {
            var MainModule = Prologue(@static, out var method, out var TKey, out var DefaultTKeyEqualityComparer, out var T, out var IRefFunc2);

            var Enumerable0 = OuterRoutine(type, method, IRefFunc2, TKey, out var Enumerator0, out var Element0, out var TOuterKeySelector);

            GenericParameter Element1 = new GenericParameter("T1", method) { HasNotNullableValueTypeConstraint = true };
            Element1.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(Element1);

            var Enumerable1 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(Element1);
            var Enumerator1 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element1);

            InnerRoutineDescendants(method, IRefFunc2, TKey, MainModule, DefaultTKeyEqualityComparer, Element1, out var TInnerKeySelector, out var WhereIndexEnumerable);

            var TSelector = Epilogue(method, MainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, TKey, TOuterKeySelector, TInnerKeySelector, DefaultTKeyEqualityComparer, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.None, InputFunc(new[] { Element1 }));
            method.Parameters.Add(inner);

            var selector = DefineParameters(TOuterKeySelector, method, TInnerKeySelector, TSelector, out var allocator);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(Element1);

            method.Body.Variables.Add(new VariableDefinition(DefaultTKeyEqualityComparer));
            method.Body.Variables.Add(new VariableDefinition(Enumerable1));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Call(_constructor);
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.LdLocaS(0);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }


        private static void GroupJoinOuter(TypeDefinition @static, TypeDefinition type, string prefix, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodDefinition constructor)
        {
            var MainModule = Prologue(@static, out var method, out var TKey, out var DefaultTKeyEqualityComparer, out var T, out var IRefFunc2);

            GenericParameter Element0 = new GenericParameter("T0", method) { HasNotNullableValueTypeConstraint = true };
            Element0.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(Element0);

            var Enumerable0 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(Element0);
            var Enumerator0 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element0);

            OuterRoutineDescendants(method, IRefFunc2, TKey, Element0, out var TOuterKeySelector);

            var Enumerable1 = InnerRoutine(type, method, IRefFunc2, TKey, MainModule, DefaultTKeyEqualityComparer, out var Enumerator1, out var Element1, out var TInnerKeySelector, out var WhereIndexEnumerable);

            var TSelector = Epilogue(method, MainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, TKey, TOuterKeySelector, TInnerKeySelector, DefaultTKeyEqualityComparer, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.None, InputFunc(new[] { Element0 }));
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            var selector = DefineParameters(TOuterKeySelector, method, TInnerKeySelector, TSelector, out var allocator);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(Element0);

            method.Body.Variables.Add(new VariableDefinition(DefaultTKeyEqualityComparer));
            method.Body.Variables.Add(new VariableDefinition(Enumerable0));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(_constructor);
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.LdLocaS(0);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void GroupJoin(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1)
        {
            var MainModule = Prologue(@static, out var method, out var TKey, out var DefaultTKeyEqualityComparer, out var T, out var IRefFunc2);

            var Enumerable0 = OuterRoutine(type0, method, IRefFunc2, TKey, out var Enumerator0, out var Element0, out var TOuterKeySelector);

            var Enumerable1 = InnerRoutine(type1, method, IRefFunc2, TKey, MainModule, DefaultTKeyEqualityComparer, out var Enumerator1, out var Element1, out var TInnerKeySelector, out var WhereIndexEnumerable);

            var TSelector = Epilogue(method, MainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, TKey, TOuterKeySelector, TInnerKeySelector, DefaultTKeyEqualityComparer, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            var selector = DefineParameters(TOuterKeySelector, method, TInnerKeySelector, TSelector, out var allocator);

            method.Body.Variables.Add(new VariableDefinition(DefaultTKeyEqualityComparer));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.LdLocaS(0);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static ParameterDefinition DefineParameters(GenericParameter TOuterKeySelector, MethodDefinition method, GenericParameter TInnerKeySelector, GenericParameter TSelector, out ParameterDefinition allocator)
        {
            var outerSelector = new ParameterDefinition("outerSelector", ParameterAttributes.In, TOuterKeySelector.MakeByReferenceType());
            outerSelector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outerSelector);

            var innerSelector = new ParameterDefinition("innerSelector", ParameterAttributes.In, TInnerKeySelector.MakeByReferenceType());
            innerSelector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(innerSelector);

            var selector = new ParameterDefinition("selector", ParameterAttributes.In, TSelector.MakeByReferenceType());
            selector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(selector);

            allocator = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
            return selector;
        }

        private static GenericParameter Epilogue(MethodDefinition method, ModuleDefinition MainModule, TypeReference Element0, GenericInstanceType WhereIndexEnumerable, GenericParameter T, GenericInstanceType Enumerable0, TypeReference Enumerator0, GenericInstanceType Enumerable1, TypeReference Enumerator1, TypeReference Element1, GenericParameter TKey, GenericParameter TOuterKeySelector, GenericParameter TInnerKeySelector, GenericInstanceType DefaultTKeyEqualityComparer, out GenericInstanceType @return)
        {
            GenericParameter TSelector = new GenericParameter(nameof(TSelector), method) { HasNotNullableValueTypeConstraint = true };
            TSelector.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`3").MakeGenericInstanceType(new[]
            {
                Element0,
                WhereIndexEnumerable,
                T
            }));
            method.GenericParameters.Add(TSelector);

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
            return TSelector;
        }

        private static GenericInstanceType InnerRoutine(TypeDefinition type1, MethodDefinition method, TypeDefinition IRefFunc2, GenericParameter TKey, ModuleDefinition MainModule, GenericInstanceType DefaultTKeyEqualityComparer, out TypeReference Enumerator1, out TypeReference Element1, out GenericParameter TInnerKeySelector, out GenericInstanceType WhereIndexEnumerable)
        {
            const string suffix1 = "1";
            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, suffix1);
            var Enumerable1 = type1.MakeGenericInstanceType(added1);
            Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, suffix1);
            Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, suffix1);

            InnerRoutineDescendants(method, IRefFunc2, TKey, MainModule, DefaultTKeyEqualityComparer, Element1, out TInnerKeySelector, out WhereIndexEnumerable);
            return Enumerable1;
        }

        private static void InnerRoutineDescendants(MethodDefinition method, TypeDefinition IRefFunc2, GenericParameter TKey, ModuleDefinition MainModule, GenericInstanceType DefaultTKeyEqualityComparer, TypeReference Element1, out GenericParameter TInnerKeySelector, out GenericInstanceType WhereIndexEnumerable)
        {
            TInnerKeySelector = new GenericParameter(nameof(TInnerKeySelector), method) { HasNotNullableValueTypeConstraint = true };
            TInnerKeySelector.Constraints.Add(IRefFunc2.MakeGenericInstanceType(new[]
            {
                Element1,
                TKey
            }));
            method.GenericParameters.Add(TInnerKeySelector);

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
        }

        private static GenericInstanceType OuterRoutine(TypeDefinition type0, MethodDefinition method, TypeDefinition IRefFunc2, GenericParameter TKey, out TypeReference Enumerator0, out TypeReference Element0, out GenericParameter TOuterKeySelector)
        {
            const string suffix0 = "0";
            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, suffix0);
            var Enumerable0 = type0.MakeGenericInstanceType(added0);
            Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix0);
            Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, suffix0);

            OuterRoutineDescendants(method, IRefFunc2, TKey, Element0, out TOuterKeySelector);
            return Enumerable0;
        }

        private static void OuterRoutineDescendants(MethodDefinition method, TypeDefinition IRefFunc2, GenericParameter TKey, TypeReference Element0, out GenericParameter TOuterKeySelector)
        {
            TOuterKeySelector = new GenericParameter(nameof(TOuterKeySelector), method) { HasNotNullableValueTypeConstraint = true };
            TOuterKeySelector.Constraints.Add(IRefFunc2.MakeGenericInstanceType(new[]
            {
                Element0,
                TKey
            }));
            method.GenericParameters.Add(TOuterKeySelector);
        }

        private static ModuleDefinition Prologue(TypeDefinition @static, out MethodDefinition method, out GenericParameter TKey, out GenericInstanceType DefaultTKeyEqualityComparer, out GenericParameter T, out TypeDefinition IRefFunc2)
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

            DefaultTKeyEqualityComparer = MainModule.GetType(NameSpace, "DefaultEqualityComparer`1").MakeGenericInstanceType(TKey);

            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(T);

            IRefFunc2 = MainModule.GetType(NameSpace, "IRefFunc`2");
            return MainModule;
        }
    }
}
