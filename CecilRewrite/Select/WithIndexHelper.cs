using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;

    static class WithIndexHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(WithIndexHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                WithIndex(@static, type);
            }
        }

        private static void WithIndex(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(WithIndex), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);

            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(added);
            var Element = @this.GetElementTypeOfCollectionType().Replace(added);

            var Tuple = MainModule.ImportReference(SystemModule.GetType("System", "ValueTuple`2")).MakeGenericInstanceType(new[]
            {
                Element,
                MainModule.TypeSystem.Int64,
            });

            var Action = MainModule.GetType(NameSpace, "WithIndex`1").MakeGenericInstanceType(Element);

            var @return = MainModule.GetType(NameSpace, "SelectIndexEnumerable`5").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                Tuple,
                Action,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            method.Body.Variables.Add(new VariableDefinition(Action));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.LdLocaS(0);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
