using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming

namespace CecilRewrite.Contains
{
    using static Program;

    static class TryGetElementAtHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetElementAtHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
                TryGetElementAt(@static, type);
        }

        private static void TryGetElementAt(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var Boolean = MainModule.TypeSystem.Boolean;
            var method = new MethodDefinition(nameof(TryGetElementAt), StaticMethodAttributes, Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            var argumentsFromTypeToMethodParam = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(argumentsFromTypeToMethodParam);
            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var Enumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);

            ParameterDefinition index;
            index = new ParameterDefinition(nameof(index), ParameterAttributes.None, MainModule.TypeSystem.Int64);
            method.Parameters.Add(index);

            var body = method.Body;
            var variables = body.Variables;
            
        }
    }
}
