using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    using static Program;
    public static class TryGetMinHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetMinHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                if (type.Name == "GroupByEnumerable`8") continue;
                @static.TryGetMin(type);
            }
        }

        private static void TryGetMin(this TypeDefinition @static, TypeDefinition type)
        {
            static void WithType(TypeDefinition @static, TypeDefinition type, TypeReference fillType)
            {
                var method = new MethodDefinition(nameof(TryGetMin), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
                {
                    DeclaringType = @static,
                    AggressiveInlining = true,
                };
                method.CustomAttributes.Add(ExtensionAttribute);
                method.Parameters.Capacity = 2;
                method.GenericParameters.Capacity = type.GenericParameters.Count - 1;
                method.TryGetMinMethodFillTypeArgument(type, fillType);
                @static.Methods.Add(method);
            }
            WithType(@static, type, MainModule.TypeSystem.Byte);
            WithType(@static, type, MainModule.TypeSystem.SByte);
            WithType(@static, type, MainModule.TypeSystem.Int16);
            WithType(@static, type, MainModule.TypeSystem.UInt16);
            WithType(@static, type, MainModule.TypeSystem.Int32);
            WithType(@static, type, MainModule.TypeSystem.UInt32);
            WithType(@static, type, MainModule.TypeSystem.Int64);
            WithType(@static, type, MainModule.TypeSystem.UInt64);
            WithType(@static, type, MainModule.TypeSystem.Single);
            WithType(@static, type, MainModule.TypeSystem.Double);
        }

        private static void TryGetMinMethodFillTypeArgument(this MethodDefinition method, TypeDefinition collectionTypeDefinition, TypeReference fillTypeReference)
        {
            var @this = new GenericInstanceType(collectionTypeDefinition);
            var typeGenericParameters = collectionTypeDefinition.GenericParameters;

            var addedParams = method.FromTypeToMethodParam(typeGenericParameters, "T", fillTypeReference);
            var index = 0;
            foreach (var genericParameter in collectionTypeDefinition.GenericParameters)
                @this.GenericArguments.Add(genericParameter.Name == "T" ? fillTypeReference : addedParams[index++]);
            TryGetMaxHelper.FillParameter(@this, method, fillTypeReference);
            TryGetMaxHelper.FillBody(@this, method, fillTypeReference, OpCodes.Bge_S);
        }
    }
}
