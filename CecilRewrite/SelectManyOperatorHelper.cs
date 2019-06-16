using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class SelectManyOperatorHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(SelectManyOperatorHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);

            var method = new MethodDefinition("SelectMany", StaticMethodAttributes, module.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            GenericParameter TEnumerable, TEnumerator, T, TEnumerableAfter, TEnumeratorAfter, TAfter, TAction;
            TEnumerable = new GenericParameter(nameof(TEnumerable), method) { HasNotNullableValueTypeConstraint = true };
            TEnumerator = new GenericParameter(nameof(TEnumerator), method) { HasNotNullableValueTypeConstraint = true };
            TEnumerableAfter = new GenericParameter(nameof(TEnumerableAfter), method) { HasNotNullableValueTypeConstraint = true };
            TEnumeratorAfter = new GenericParameter(nameof(TEnumeratorAfter), method) { HasNotNullableValueTypeConstraint = true };
            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            TAfter = new GenericParameter(nameof(TAfter), method) { HasNotNullableValueTypeConstraint = true };
            TAction = new GenericParameter(nameof(TAction), method) { HasNotNullableValueTypeConstraint = true };
            method.GenericParameters.Add(TEnumerable);
            method.GenericParameters.Add(TEnumerator);
            method.GenericParameters.Add(T);
            method.GenericParameters.Add(TEnumerableAfter);
            method.GenericParameters.Add(TEnumeratorAfter);
            method.GenericParameters.Add(TAfter);
            method.GenericParameters.Add(TAction);
            T.CustomAttributes.Add(UnManagedAttribute);
            TAfter.CustomAttributes.Add(UnManagedAttribute);
            TEnumerator.Constraints.Add(module.GetType(NameSpace, "IRefEnumerator`1").MakeGenericInstanceType(T));
            TEnumeratorAfter.Constraints.Add(module.GetType(NameSpace, "IRefEnumerator`1").MakeGenericInstanceType(TAfter));
            TEnumerable.Constraints.Add(module.GetType(NameSpace, "IRefEnumerable`2").MakeGenericInstanceType(new[]
            {
                TEnumerator,
                T
            }));
            TEnumerableAfter.Constraints.Add(module.GetType(NameSpace, "IRefEnumerable`2").MakeGenericInstanceType(new[]
            {
                TEnumeratorAfter,
                TAfter
            }));
            TAction.Constraints.Add(module.GetType(NameSpace, "IRefAction`2").MakeGenericInstanceType(new[]
            {
                T,
                TEnumerableAfter
            }));

            var @return = module.GetType(NameSpace, "SelectManyEnumerable`7").MakeGenericInstanceType(new[]
            {
                TEnumerable,
                TEnumerator,
                T,
                TEnumerableAfter,
                TEnumeratorAfter,
                TAfter,
                TAction,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, TEnumerable.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var funcParam = new ParameterDefinition("func", ParameterAttributes.In, TAction.MakeByReferenceType());
            funcParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(funcParam);

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
