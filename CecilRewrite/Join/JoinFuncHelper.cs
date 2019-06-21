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

    static class JoinFuncHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(JoinFuncHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type0 in Enumerables)
            {
                JoinOuter(@static, type0, "Array", CalcArrayType, AsRefEnumerableArray);
                JoinInner(@static, type0, "Array", CalcArrayType, AsRefEnumerableArray);
                JoinOuter(@static, type0, "Native", NativeArray.MakeGenericInstanceType, AsRefEnumerableNative);
                JoinInner(@static, type0, "Native", NativeArray.MakeGenericInstanceType, AsRefEnumerableNative);
                foreach (var type1 in Enumerables)
                    Join(@static, type0, type1);
            }
        }

        private static void JoinOuter(TypeDefinition @static, TypeDefinition type, string prefix, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodDefinition constructor)
        {
            var method = Prologue(@static, out var MainModule, out var TKey, out var TKeyEqualityComparer, out var TKeyEqualityComparerFunc, out var T, out var IRefFunc2);

            GenericParameter Element0 = new GenericParameter("T0", method) { HasNotNullableValueTypeConstraint = true };
            Element0.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(Element0);

            var Enumerable0 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(Element0);
            var Enumerator0 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element0);

            InternalRoutine(TKey, Element0, out var TKeySelector0, out var TKeySelectorFunc0);

            Routine(type, method, "1", IRefFunc2, TKey, out var Enumerable1, out var Enumerator1, out var Element1, out var TKeySelector1, out var TKeySelectorFunc1);

            var outer = new ParameterDefinition("outer", ParameterAttributes.None, InputFunc(new[] { Element0 }));
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            Epilogue(method, MainModule, Element0, Element1, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, TKey, TKeySelector0, TKeySelectorFunc0, TKeySelector1, TKeySelectorFunc1, TKeyEqualityComparer, TKeyEqualityComparerFunc, out var @return, out var comparer, out var selector, out var allocator, out var TSelector);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(Element0);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(Enumerable0));
            variables.Add(new VariableDefinition(TKeySelector0));
            variables.Add(new VariableDefinition(TKeySelector1));
            variables.Add(new VariableDefinition(TKeyEqualityComparer));
            variables.Add(new VariableDefinition(TSelector));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(_constructor);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.Do(OpCodes.Ldarg_3);
            processor.Do(OpCodes.Stloc_2);
            processor.LdLocaS(2);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, comparer));
            processor.Do(OpCodes.Stloc_3);
            processor.LdLocaS(3);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Append(Instruction.Create(OpCodes.Stloc_S, variables[4]));
            processor.LdLocaS(4);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void JoinInner(TypeDefinition @static, TypeDefinition type, string prefix, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodDefinition constructor)
        {
            var method = Prologue(@static, out var MainModule, out var TKey, out var TKeyEqualityComparer, out var TKeyEqualityComparerFunc, out var T, out var IRefFunc2);

            Routine(type, method, "0", IRefFunc2, TKey, out var Enumerable0, out var Enumerator0, out var Element0, out var TKeySelector0, out var TKeySelectorFunc0);

            GenericParameter Element1 = new GenericParameter("T1", method) { HasNotNullableValueTypeConstraint = true };
            Element1.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(Element1);

            var Enumerable1 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(Element1);
            var Enumerator1 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element1);

            InternalRoutine(TKey, Element1, out var TKeySelector1, out var TKeySelectorFunc1);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.None, InputFunc(new[] { Element1 }));
            method.Parameters.Add(inner);

            Epilogue(method, MainModule, Element0, Element1, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, TKey, TKeySelector0, TKeySelectorFunc0, TKeySelector1, TKeySelectorFunc1, TKeyEqualityComparer, TKeyEqualityComparerFunc, out var @return, out var comparer, out var selector, out var allocator, out var TSelector);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(Element1);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(Enumerable1));
            variables.Add(new VariableDefinition(TKeySelector0));
            variables.Add(new VariableDefinition(TKeySelector1));
            variables.Add(new VariableDefinition(TKeyEqualityComparer));
            variables.Add(new VariableDefinition(TSelector));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Call(_constructor);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.Do(OpCodes.Ldarg_3);
            processor.Do(OpCodes.Stloc_2);
            processor.LdLocaS(2);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, comparer));
            processor.Do(OpCodes.Stloc_3);
            processor.LdLocaS(3);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Append(Instruction.Create(OpCodes.Stloc_S, variables[4]));
            processor.LdLocaS(4);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void Join(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1)
        {
            var method = Prologue(@static, out var MainModule, out var TKey, out var TKeyEqualityComparer, out var TKeyEqualityComparerFunc, out var T, out var IRefFunc2);

            Routine(type0, method, "0", IRefFunc2, TKey, out var Enumerable0, out var Enumerator0, out var Element0, out var TKeySelector0, out var TKeySelectorFunc0);

            Routine(type1, method, "1", IRefFunc2, TKey, out var Enumerable1, out var Enumerator1, out var Element1, out var TKeySelector1, out var TKeySelectorFunc1);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            Epilogue(method, MainModule, Element0, Element1, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, TKey, TKeySelector0, TKeySelectorFunc0, TKeySelector1, TKeySelectorFunc1, TKeyEqualityComparer, TKeyEqualityComparerFunc, out var @return, out var comparer, out var selector, out var allocator, out var TSelector);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(TKeySelector0));
            variables.Add(new VariableDefinition(TKeySelector1));
            variables.Add(new VariableDefinition(TKeyEqualityComparer));
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
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, comparer));
            processor.Do(OpCodes.Stloc_2);
            processor.LdLocaS(2);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Do(OpCodes.Stloc_3);
            processor.LdLocaS(3);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void Epilogue(MethodDefinition method, ModuleDefinition MainModule, TypeReference Element0, TypeReference Element1, GenericParameter T, GenericInstanceType Enumerable0, TypeReference Enumerator0, GenericInstanceType Enumerable1, TypeReference Enumerator1, GenericParameter TKey, TypeReference TKeySelector0, TypeReference TKeySelectorFunc0, TypeReference TKeySelector1, TypeReference TKeySelectorFunc1, TypeReference TKeyEqualityComparer, TypeReference TKeyEqualityComparerFunc, out GenericInstanceType @return, out ParameterDefinition comparer, out ParameterDefinition selector, out ParameterDefinition allocator, out GenericInstanceType TSelector)
        {
            var SelectorTuple3 = new[]
            {
                Element0,
                Element1,
                T
            };
            TSelector = MainModule.GetType(NameSpace, "DelegateFuncToStructOperatorFunc`3").MakeGenericInstanceType(SelectorTuple3);
            var TSelectorFunc = MainModule.ImportReference(SystemModule.GetType("System", "Func`3")).MakeGenericInstanceType(SelectorTuple3);

            @return = MainModule.GetType(NameSpace, "JoinEnumerable`12").MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Element0,
                Enumerable1,
                Enumerator1,
                Element1,
                TKey,
                TKeySelector0,
                TKeySelector1,
                TKeyEqualityComparer,
                T,
                TSelector,
            });
            method.ReturnType = @return;

            var outerSelector = new ParameterDefinition("outerSelector", ParameterAttributes.None, TKeySelectorFunc0);
            method.Parameters.Add(outerSelector);

            var innerSelector = new ParameterDefinition("innerSelector", ParameterAttributes.None, TKeySelectorFunc1);
            method.Parameters.Add(innerSelector);

            comparer = new ParameterDefinition(nameof(comparer), ParameterAttributes.None, TKeyEqualityComparerFunc);
            method.Parameters.Add(comparer);

            selector = new ParameterDefinition(nameof(selector), ParameterAttributes.None, TSelectorFunc);
            method.Parameters.Add(selector);

            allocator = new ParameterDefinition(nameof(allocator), ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
        }

        private static MethodDefinition Prologue(TypeDefinition @static, out ModuleDefinition MainModule, out GenericParameter TKey, out TypeReference TKeyEqualityComparer, out TypeReference TKeyEqualityComparerFunc, out GenericParameter T, out TypeDefinition IRefFunc2)
        {
            MainModule = @static.Module;
            var method = new MethodDefinition(nameof(Join), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            TKey = new GenericParameter(nameof(TKey), method) { HasNotNullableValueTypeConstraint = true };
            TKey.CustomAttributes.Add(UnManagedAttribute);
            TKey.Constraints.Add(MainModule.ImportReference(SystemModule.GetType("System", "IEquatable`1")).MakeGenericInstanceType(TKey));
            method.GenericParameters.Add(TKey);

            var TKeyComparerTuple3 = new[]
            {
                TKey,
                TKey,
                MainModule.TypeSystem.Boolean,
            };
            TKeyEqualityComparer = MainModule.GetType(NameSpace, "DelegateFuncToStructOperatorFunc`3").MakeGenericInstanceType(TKeyComparerTuple3);
            TKeyEqualityComparerFunc = MainModule.ImportReference(SystemModule.GetType("System", "Func`3")).MakeGenericInstanceType(TKeyComparerTuple3);

            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(T);

            IRefFunc2 = MainModule.GetType(NameSpace, "IRefFunc`2");

            return method;
        }

        private static void Routine(TypeDefinition type, MethodDefinition method, string suffix, TypeDefinition IRefFunc2, GenericParameter TKey, out GenericInstanceType Enumerable, out TypeReference Enumerator, out TypeReference Element, out TypeReference TKeySelector, out GenericInstanceType TKeySelectorFunc)
        {
            var added0 = method.FromTypeToMethodParam(type.GenericParameters, suffix);
            Enumerable = type.MakeGenericInstanceType(added0);
            Enumerator = Enumerable.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix);
            Element = Enumerable.GetElementTypeOfCollectionType().Replace(added0, suffix);

            InternalRoutine(TKey, Element, out TKeySelector, out TKeySelectorFunc);
        }

        private static void InternalRoutine(GenericParameter TKey, TypeReference Element, out TypeReference TKeySelector, out GenericInstanceType TKeySelectorFunc)
        {
            var typeReferences = new[]
            {
                Element,
                TKey
            };
            TKeySelector = MainModule.GetType(NameSpace, "DelegateFuncToStructOperatorFunc`2").MakeGenericInstanceType(typeReferences);
            TKeySelectorFunc = MainModule.ImportReference(SystemModule.GetType("System", "Func`2")).MakeGenericInstanceType(typeReferences);
        }
    }
}
