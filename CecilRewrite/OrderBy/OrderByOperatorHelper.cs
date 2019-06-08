using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class OrderByOperatorHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(OrderByOperatorHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                OrderBy(@static, type);
            }
        }

        private static void OrderBy(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(OrderBy), StaticMethodAttributes, MainModule.TypeSystem.Void)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            GenericParameter TPredicate0;
            TPredicate0 = new GenericParameter(nameof(TPredicate0), method) { HasNotNullableValueTypeConstraint = true };
            method.GenericParameters.Add(TPredicate0);

            var addedParams = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(addedParams);
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);

            TPredicate0.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`3").MakeGenericInstanceType(new[]
            {
                Element,
                Element,
                MainModule.TypeSystem.Int32,
            }));

            var @return = MainModule.GetType(NameSpace, "OrderByEnumerable`4").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                TPredicate0,
            });

            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);

            var TPredicate0ParameterDefinition = new ParameterDefinition("predicate", ParameterAttributes.In, TPredicate0.MakeByReferenceType());
            TPredicate0ParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(TPredicate0ParameterDefinition);

            var allocatorParameterDefinition = new ParameterDefinition("allocator", ParameterAttributes.Optional, Allocator) { Constant = 2 };
            method.Parameters.Add(allocatorParameterDefinition);

            method.ReturnType = @return;

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();
            @static.Methods.Add(method);
        }
    }
}
