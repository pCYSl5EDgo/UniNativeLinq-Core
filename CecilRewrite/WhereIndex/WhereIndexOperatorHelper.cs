using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class WhereIndexOperatorHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(WhereIndexOperatorHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);

            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                WhereIndex(@static, type);
            }
        }

        private static void WhereIndex(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(WhereIndex), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            GenericParameter TPredicate0;
            TPredicate0 = new GenericParameter(nameof(TPredicate0), method) { HasNotNullableValueTypeConstraint = true };
            TPredicate0.Constraints.Add(MainModule.GetType(NameSpace, "IWhereIndex`1").MakeGenericInstanceType(new[]
            {
                Element,
            }));
            method.GenericParameters.Add(TPredicate0);

            var @return = MainModule.GetType(NameSpace, "WhereIndexEnumerable`4").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                TPredicate0,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var funcParam = new ParameterDefinition("predicate", ParameterAttributes.In, TPredicate0.MakeByReferenceType());
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
