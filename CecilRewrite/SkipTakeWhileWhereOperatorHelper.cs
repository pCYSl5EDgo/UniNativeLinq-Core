using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class SkipTakeWhileWhereOperatorHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition Where;
            const string suffix = "OperatorHelper";
            Where = new TypeDefinition(NameSpace,
                nameof(Where) + suffix,
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Where.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Where);

            TypeDefinition TakeWhile;
            TakeWhile = new TypeDefinition(NameSpace,
                nameof(TakeWhile) + suffix,
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            TakeWhile.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(TakeWhile);

            TypeDefinition SkipWhile;
            SkipWhile = new TypeDefinition(NameSpace,
                nameof(SkipWhile) + suffix,
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            SkipWhile.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(SkipWhile);

            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                Make(@static: Where, type, nameof(Where));
                Make(@static: SkipWhile, type, nameof(SkipWhile));
                Make(@static: TakeWhile, type, nameof(TakeWhile));
            }
        }

        private static void Make(TypeDefinition @static, TypeDefinition type, string name)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(name, StaticMethodAttributes, MainModule.TypeSystem.Void)
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
            TPredicate0.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[]
            {
                Element,
                MainModule.TypeSystem.Boolean,
            }));
            method.GenericParameters.Add(TPredicate0);

            var @return = MainModule.GetType(NameSpace, name + "Enumerable`4").MakeGenericInstanceType(new[]
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

            var predicateParam = new ParameterDefinition("predicate", ParameterAttributes.In, TPredicate0.MakeByReferenceType());
            predicateParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(predicateParam);

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
