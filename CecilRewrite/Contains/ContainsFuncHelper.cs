using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming

namespace CecilRewrite
{
    using static Program;

    static class ContainsFuncHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(ContainsFuncHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
                Contains(@static, type);
        }

        private static void Contains(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var Boolean = MainModule.TypeSystem.Boolean;
            var method = new MethodDefinition(nameof(Contains), StaticMethodAttributes, Boolean)
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

            var elementParameter = new ParameterDefinition("value", ParameterAttributes.In, Element.MakeByReferenceType());
            elementParameter.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(elementParameter);

            var Func3 = typeof(Func<,,>).ImportGenericType(MainModule, new[] { Element, Element, Boolean });
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.None, Func3));

            var body = method.Body;
            var variables = body.Variables;
            variables.Add(new VariableDefinition(Enumerator));
            variables.Add(new VariableDefinition(Element.MakeByReferenceType()));
            variables.Add(new VariableDefinition(Boolean));

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
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_1);
            processor.LdObj(Element);
            processor.Do(OpCodes.Ldloc_1);
            processor.LdObj(Element);
            processor.CallVirtual(typeof(Func<,,>).FindMethodImportGenericType(MainModule, "Invoke", new[] { Element, Element, Boolean }));
            processor.False(il0007);
            processor.LdLocaS(0);
            var Dispose = Enumerator.FindMethod("Dispose", Helper.NoParameter);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Ret();
            processor.Append(il0031);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
