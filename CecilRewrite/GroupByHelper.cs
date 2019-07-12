using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class GroupByHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(GroupByHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);

            foreach (var type in Enumerables)
            {
                GroupBy(@static, type);
            }
        }

        private static void GroupBy(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(GroupBy), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            GenericParameter TKey, TKeyFunc, TElement, TElementFunc, TEqualityComparer;
            TKey = new GenericParameter(nameof(TKey) + "0", method) { HasNotNullableValueTypeConstraint = true };
            TKey.CustomAttributes.Add(UnManagedAttribute);
            TKeyFunc = new GenericParameter(nameof(TKeyFunc) + "0", method) { HasNotNullableValueTypeConstraint = true };
            TKeyFunc.Constraints.Add(MainModule.GetType(NameSpace, "IRefAction`2").MakeGenericInstanceType(new[]
            {
                Element,
                TKey,
            }));
            TElement = new GenericParameter(nameof(TElement) + "0", method) { HasNotNullableValueTypeConstraint = true };
            TElement.CustomAttributes.Add(UnManagedAttribute);
            TElementFunc = new GenericParameter(nameof(TElementFunc) + "0", method) { HasNotNullableValueTypeConstraint = true };
            TElementFunc.Constraints.Add(MainModule.GetType(NameSpace, "IRefAction`2").MakeGenericInstanceType(new[]
            {
                Element,
                TElement,
            }));
            TEqualityComparer = new GenericParameter(nameof(TEqualityComparer) + "0", method) { HasNotNullableValueTypeConstraint = true };
            TEqualityComparer.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`3").MakeGenericInstanceType(new[]
            {
                TKey,
                TKey,
                MainModule.TypeSystem.Boolean,
            }));
            method.GenericParameters.Add(TKey);
            method.GenericParameters.Add(TKeyFunc);
            method.GenericParameters.Add(TElement);
            method.GenericParameters.Add(TElementFunc);
            method.GenericParameters.Add(TEqualityComparer);

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var keyFuncParam = new ParameterDefinition("keyFunc", ParameterAttributes.In, TKeyFunc.MakeByReferenceType());
            keyFuncParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(keyFuncParam);

            var elementFuncParam = new ParameterDefinition("elementFunc", ParameterAttributes.In, TElementFunc.MakeByReferenceType());
            elementFuncParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(elementFuncParam);

            var comparerParam = new ParameterDefinition("comparer", ParameterAttributes.In, TEqualityComparer.MakeByReferenceType());
            comparerParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(comparerParam);

            var @return = MainModule.GetType(NameSpace, "GroupByEnumerable`8").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                TKey,
                TKeyFunc,
                TElement,
                TElementFunc,
                TEqualityComparer,
            });
            method.ReturnType = @return;

            var allocatorParam = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2
            };
            method.Parameters.Add(allocatorParam);
            
            var optionParam = new ParameterDefinition("option", ParameterAttributes.HasDefault | ParameterAttributes.Optional, MainModule.GetType(NameSpace, "GroupByDisposeOptions"))
            {
                Constant = 1
            };
            method.Parameters.Add(optionParam);

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, allocatorParam));
            processor.Append(Instruction.Create(OpCodes.Ldarg_S, optionParam));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();
            @static.Methods.Add(method);
        }
    }
}
