using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class AppendPrependDefaultIfEmptyHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition Append;
            Append = new TypeDefinition(NameSpace,
                nameof(Append) + "Helper",
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Append.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Append);

            TypeDefinition Prepend;
            Prepend = new TypeDefinition(NameSpace,
                nameof(Prepend) + "Helper",
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Prepend.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Prepend);

            TypeDefinition DefaultIfEmpty;
            DefaultIfEmpty = new TypeDefinition(NameSpace,
                nameof(DefaultIfEmpty) + "Helper",
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            DefaultIfEmpty.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(DefaultIfEmpty);

            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                MakeMethod(Append, type, nameof(Append));
                MakeMethod(Prepend, type, nameof(Prepend));
                MakeMethod(DefaultIfEmpty, type, nameof(DefaultIfEmpty));
            }
        }

        private static void MakeMethod(TypeDefinition @static, TypeDefinition type, string name)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(name, StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            var @return = MainModule.GetType(NameSpace, name + "Enumerable`3").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var valueParam = new ParameterDefinition("value", ParameterAttributes.In, Element.MakeByReferenceType());
            valueParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(valueParam);

            var processor = method.Body.GetILProcessor();

            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
