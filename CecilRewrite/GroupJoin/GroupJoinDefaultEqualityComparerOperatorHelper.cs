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
                foreach (var type1 in Enumerables)
                    GroupJoin(@static, type0, type1);
        }

        private static void GroupJoin(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(GroupJoin), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            const string suffix0 = "0";
            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, suffix0);
            var Enumerable0 = type0.MakeGenericInstanceType(added0);
            var Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix0);
            var Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, suffix0);

            const string suffix1 = "1";
            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, suffix1);
            var Enumerable1 = type1.MakeGenericInstanceType(added1);
            var Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, suffix1);
            var Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, suffix1);

            GenericParameter TKey = new GenericParameter(nameof(TKey), method) { HasNotNullableValueTypeConstraint = true };
            TKey.CustomAttributes.Add(UnManagedAttribute);
            TKey.Constraints.Add(MainModule.ImportReference(SystemModule.GetType("System", "IEquatable`1")).MakeGenericInstanceType(TKey));
            method.GenericParameters.Add(TKey);

            var IRefFunc2 = MainModule.GetType(NameSpace, "IRefFunc`2");
            GenericParameter TOuterKeySelector = new GenericParameter(nameof(TOuterKeySelector), method) { HasNotNullableValueTypeConstraint = true };
            TOuterKeySelector.Constraints.Add(IRefFunc2.MakeGenericInstanceType(new[]
            {
                Element0,
                TKey
            }));
            method.GenericParameters.Add(TOuterKeySelector);
            GenericParameter TInnerKeySelector = new GenericParameter(nameof(TInnerKeySelector), method) { HasNotNullableValueTypeConstraint = true };
            TInnerKeySelector.Constraints.Add(IRefFunc2.MakeGenericInstanceType(new[]
            {
                Element1,
                TKey
            }));
            method.GenericParameters.Add(TInnerKeySelector);

            GenericParameter T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(T);

            var NativeEnumerable = MainModule.GetType(NameSpace, "NativeEnumerable`1");
            var InnerNativeEnumerable = NativeEnumerable.MakeGenericInstanceType(Element1);
            var InnerNativeEnumerator = NativeEnumerable.NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(Element1);

            var DefaultTKeyEqualityComparer = MainModule.GetType(NameSpace, "DefaultEqualityComparer`1").MakeGenericInstanceType(TKey);
            var GroupJoinPredicate = MainModule.GetType(NameSpace, "GroupJoinPredicate`3").MakeGenericInstanceType(new[]
            {
                Element1,
                TKey,
                DefaultTKeyEqualityComparer
            });

            var WhereIndexEnumerable = MainModule.GetType(NameSpace, "WhereIndexEnumerable`4").MakeGenericInstanceType(new[]
            {
                InnerNativeEnumerable,
                InnerNativeEnumerator,
                Element1,
                GroupJoinPredicate,
            });

            GenericParameter TSelector = new GenericParameter(nameof(TSelector), method) { HasNotNullableValueTypeConstraint = true };
            TSelector.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`3").MakeGenericInstanceType(new[]
            {
                Element0,
                WhereIndexEnumerable,
                T
            }));
            method.GenericParameters.Add(TSelector);

            var @return = MainModule.GetType(NameSpace, "GroupJoinEnumerable`12").MakeGenericInstanceType(new[]
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

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            outer.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            inner.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            var outerSelector = new ParameterDefinition("outerSelector", ParameterAttributes.In, TOuterKeySelector.MakeByReferenceType());
            outerSelector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(outerSelector);

            var innerSelector = new ParameterDefinition("innerSelector", ParameterAttributes.In, TInnerKeySelector.MakeByReferenceType());
            innerSelector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(innerSelector);

            var selector = new ParameterDefinition("selector", ParameterAttributes.In, TSelector.MakeByReferenceType());
            selector.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(selector);

            var allocator = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);

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
    }
}
