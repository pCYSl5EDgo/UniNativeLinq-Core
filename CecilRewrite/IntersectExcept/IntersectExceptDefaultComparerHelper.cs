using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;

    static class IntersectExceptDefaultComparerHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition IntersectDefaultComparerHelper = new TypeDefinition(NameSpace,
                nameof(IntersectDefaultComparerHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            IntersectDefaultComparerHelper.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(IntersectDefaultComparerHelper);

            TypeDefinition ExceptDefaultComparerHelper = new TypeDefinition(NameSpace,
                nameof(ExceptDefaultComparerHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            ExceptDefaultComparerHelper.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(ExceptDefaultComparerHelper);

            var Intersect = module.GetType("UniNativeLinq", "IntersectOperation`6");
            var Except = module.GetType("UniNativeLinq", "ExceptOperation`6");
            foreach (var type0 in Enumerables)
            {
                foreach (var type1 in Enumerables)
                {
                    Make(IntersectDefaultComparerHelper, type0, type1, nameof(Intersect), Intersect);
                    Make(ExceptDefaultComparerHelper, type0, type1, nameof(Except), Except);
                }
            }
        }

        private static void Make(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1, string name, TypeDefinition operation)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(name, StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            GenericParameter T;
            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);
            T.Constraints.Add(MainModule.ImportReference(SystemModule.GetType("System", "IComparable`1")).MakeGenericInstanceType(T));

            var DefaultOrderByAscending = MainModule.GetType(NameSpace, "DefaultOrderByAscending`1").MakeGenericInstanceType(T);

            const string suffix0 = "0";

            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, nameof(T), T, suffix0);
            foreach (var parameter in added0)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var Enumerable0 = type0.MakeGenericInstanceType(added0.Append(T));

            var Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
            var Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);

            const string suffix1 = "1";

            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, nameof(T), T, suffix1);
            foreach (var parameter in added1)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var Enumerable1 = type1.MakeGenericInstanceType(added1.Append(T));

            var Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);
            var Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);

            if (!Element0.Equals(Element1))
                return;

            method.GenericParameters.Add(T);

            var Operation = operation.MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Enumerable1,
                Enumerator1,
                T,
                DefaultOrderByAscending,
            });

            var @return = MainModule.GetType(NameSpace, "SetOperationEnumerable`6").MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Enumerable1,
                Enumerator1,
                T,
                Operation,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var secondParam = new ParameterDefinition("second", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            secondParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(secondParam);

            var allocParam = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocParam);

            method.Body.Variables.Add(new VariableDefinition(Operation));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_2);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
