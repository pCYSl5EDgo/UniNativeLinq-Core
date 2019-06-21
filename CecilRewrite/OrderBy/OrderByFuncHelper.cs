using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class OrderByFuncHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(OrderByFuncHelper),
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


            var addedParams = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(addedParams);
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);

            var TPredicate0Tuple = new[]
            {
                Element,
                Element,
                MainModule.TypeSystem.Int32,
            };
            var TPredicate0Func = MainModule.ImportReference(SystemModule.GetType("System", "Func`3")).MakeGenericInstanceType(TPredicate0Tuple);
            var TPredicate0 = MainModule.GetType(NameSpace, "DelegateFuncToStructOperatorFunc`3").MakeGenericInstanceType(TPredicate0Tuple);

            var @return = MainModule.GetType(NameSpace, "OrderByEnumerable`4").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                TPredicate0,
            });
            method.ReturnType = @return;

            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);

            var TPredicate0ParameterDefinition = new ParameterDefinition("predicate", ParameterAttributes.None, TPredicate0Func);
            method.Parameters.Add(TPredicate0ParameterDefinition);

            var allocatorParameterDefinition = new ParameterDefinition("allocator", ParameterAttributes.Optional, Allocator) { Constant = 2 };
            method.Parameters.Add(allocatorParameterDefinition);

            method.Body.Variables.Add(new VariableDefinition(TPredicate0));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_2);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();
            @static.Methods.Add(method);
        }
    }
}
