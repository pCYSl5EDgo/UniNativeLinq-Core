using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable InconsistentNaming

namespace CecilRewrite
{
    using static Program;
    static class ContainsDefaultEqualityComparerHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(ContainsDefaultEqualityComparerHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
                Contains(@static, type);
        }

        private static void Contains(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(Contains), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            var argumentsFromTypeToMethodParam = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(argumentsFromTypeToMethodParam);
            GenericParameter Element;
            {
                var _element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
                var findIndex = argumentsFromTypeToMethodParam.FindIndex(x => x.Name == _element.Name);
                if (findIndex == -1)
                    return;
                Element = argumentsFromTypeToMethodParam[findIndex];
            }
            Element.Constraints.Add(MainModule.ImportReference(typeof(IEquatable<>)).MakeGenericInstanceType(Element));

            var Enumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);

            var elementParameter = new ParameterDefinition("value", ParameterAttributes.In, Element.MakeByReferenceType());
            elementParameter.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(elementParameter);

            var body = method.Body;
            var variables = body.Variables;
            variables.Add(new VariableDefinition(Enumerator));
            variables.Add(new VariableDefinition(Element.MakeByReferenceType()));
            variables.Add(new VariableDefinition(@this.Module.TypeSystem.Boolean));

            var il0007 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var il0031 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.GetEnumerator(@this);
            processor.Do(OpCodes.Stloc_0);
            processor.Append(il0007);
            processor.LdLocaS(2);
            processor.Call(Enumerator.FindMethod("TryGetNext"));
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.False(il0031);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldloc_1);
            processor.LdObj(Element);
            processor.Constrained(Element);
            var IEquatable = MainModule.ImportReference(SystemModule.GetType("System", "IEquatable`1")).MakeGenericInstanceType(Element);
            processor.CallVirtual(IEquatable.FindMethod("Equals"));
            processor.False(il0007);
            processor.LdLocaS(0);
            var Dispose = Enumerator.FindMethod("Dispose", Helper.NoParameter);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Ret();
            processor.Append(il0031);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
