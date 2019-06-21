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

    static class JoinDefaultEqualityComparerOperatorHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(JoinDefaultEqualityComparerOperatorHelper),
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
            var method = Prologue(@static, out var MainModule, out var TKey, out var DefaultTKeyEqualityComparer, out var T, out var IRefFunc2);

            GenericParameter Element0 = new GenericParameter("T0", method) { HasNotNullableValueTypeConstraint = true };
            Element0.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(Element0);

            var Enumerable0 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(Element0);
            var Enumerator0 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element0);

            InternalRoutine(method, "0", IRefFunc2, TKey, Element0, out var TKeySelector0);

            Routine(type, method, "1", IRefFunc2, TKey, out var Enumerable1, out var Enumerator1, out var Element1, out var TKeySelector1);

            var outer = new ParameterDefinition("outer", ParameterAttributes.None, InputFunc(new[] { Element0 }));
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            Epilogue(method, MainModule, Element0, Element1, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, TKey, TKeySelector0, TKeySelector1, DefaultTKeyEqualityComparer, out var @return, out var selector, out var allocator);

            var _consturctor = new GenericInstanceMethod(constructor);
            _consturctor.GenericArguments.Add(Element0);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(Enumerable0));
            variables.Add(new VariableDefinition(DefaultTKeyEqualityComparer));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(_consturctor);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.LdLocaS(1);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();
        }

        private static void JoinInner(TypeDefinition @static, TypeDefinition type, string prefix, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodDefinition constructor)
        {
            var method = Prologue(@static, out var MainModule, out var TKey, out var DefaultTKeyEqualityComparer, out var T, out var IRefFunc2);

            Routine(type, method, "0", IRefFunc2, TKey, out var Enumerable0, out var Enumerator0, out var Element0, out var TKeySelector0);

            GenericParameter Element1 = new GenericParameter("T1", method) { HasNotNullableValueTypeConstraint = true };
            Element1.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(Element1);

            var Enumerable1 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(Element1);
            var Enumerator1 = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element1);

            InternalRoutine(method, "1", IRefFunc2, TKey, Element1, out var TKeySelector1);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.None, InputFunc(new[] { Element1 }));
            method.Parameters.Add(inner);

            Epilogue(method, MainModule, Element0, Element1, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, TKey, TKeySelector0, TKeySelector1, DefaultTKeyEqualityComparer, out var @return, out var selector, out var allocator);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(Element1);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(Enumerable1));
            variables.Add(new VariableDefinition(DefaultTKeyEqualityComparer));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Call(_constructor);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.LdLocaS(1);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, selector));
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocator));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();
        }

        private static void Join(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1)
        {
            var method = Prologue(@static, out var MainModule, out var TKey, out var DefaultTKeyEqualityComparer, out var T, out var IRefFunc2);

            Routine(type0, method, "0", IRefFunc2, TKey, out var Enumerable0, out var Enumerator0, out var Element0, out var TKeySelector0);

            Routine(type1, method, "1", IRefFunc2, TKey, out var Enumerable1, out var Enumerator1, out var Element1, out var TKeySelector1);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            Epilogue(method, MainModule, Element0, Element1, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, TKey, TKeySelector0, TKeySelector1, DefaultTKeyEqualityComparer, out var @return, out var selector, out var allocator);

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
        }

        private static void Epilogue(MethodDefinition method, ModuleDefinition MainModule, TypeReference Element0, TypeReference Element1, GenericParameter T, GenericInstanceType Enumerable0, TypeReference Enumerator0, GenericInstanceType Enumerable1, TypeReference Enumerator1, GenericParameter TKey, GenericParameter TKeySelector0, GenericParameter TKeySelector1, GenericInstanceType DefaultTKeyEqualityComparer, out GenericInstanceType @return, out ParameterDefinition selector, out ParameterDefinition allocator)
        {
            GenericParameter TSelector = new GenericParameter(nameof(TSelector), method) { HasNotNullableValueTypeConstraint = true };
            TSelector.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`3").MakeGenericInstanceType(new[]
            {
                Element0,
                Element1,
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
                TKeySelector0,
                TKeySelector1,
                DefaultTKeyEqualityComparer,
                T,
                TSelector,
            });
            method.ReturnType = @return;

            var outerSelector = new ParameterDefinition("outerSelector", ParameterAttributes.In, TKeySelector0.MakeByReferenceType());
            outerSelector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outerSelector);

            var innerSelector = new ParameterDefinition("innerSelector", ParameterAttributes.In, TKeySelector1.MakeByReferenceType());
            innerSelector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(innerSelector);

            selector = new ParameterDefinition("selector", ParameterAttributes.In, TSelector.MakeByReferenceType());
            selector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(selector);

            allocator = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
        }

        private static MethodDefinition Prologue(TypeDefinition @static, out ModuleDefinition MainModule, out GenericParameter TKey, out GenericInstanceType DefaultTKeyEqualityComparer, out GenericParameter T, out TypeDefinition IRefFunc2)
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

            DefaultTKeyEqualityComparer = MainModule.GetType(NameSpace, "DefaultEqualityComparer`1").MakeGenericInstanceType(TKey);

            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(T);

            IRefFunc2 = MainModule.GetType(NameSpace, "IRefFunc`2");

            @static.Methods.Add(method);

            return method;
        }

        private static void Routine(TypeDefinition type, MethodDefinition method, string suffix, TypeDefinition IRefFunc2, GenericParameter TKey, out GenericInstanceType Enumerable, out TypeReference Enumerator, out TypeReference Element, out GenericParameter TKeySelector)
        {
            var added0 = method.FromTypeToMethodParam(type.GenericParameters, suffix);
            Enumerable = type.MakeGenericInstanceType(added0);
            Enumerator = Enumerable.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix);
            Element = Enumerable.GetElementTypeOfCollectionType().Replace(added0, suffix);

            InternalRoutine(method, suffix, IRefFunc2, TKey, Element, out TKeySelector);
        }

        private static void InternalRoutine(MethodDefinition method, string suffix, TypeDefinition IRefFunc2, GenericParameter TKey, TypeReference Element, out GenericParameter TKeySelector)
        {
            TKeySelector = new GenericParameter(nameof(TKeySelector) + suffix, method) { HasNotNullableValueTypeConstraint = true };
            TKeySelector.Constraints.Add(IRefFunc2.MakeGenericInstanceType(new[]
            {
                Element,
                TKey
            }));
            method.GenericParameters.Add(TKeySelector);
        }
    }
}
