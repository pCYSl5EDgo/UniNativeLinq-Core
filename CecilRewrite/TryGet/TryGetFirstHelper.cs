using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming

namespace CecilRewrite
{
    using static Program;
    static class TryGetFirstHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetFirstHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
                TryGetFirst(@static, type);
        }

        private static void TryGetFirst(TypeDefinition @static, TypeDefinition type)
        {
            if (type.Methods.Any(x => x.Name == nameof(TryGetFirst)))
                return;
            var MainModule = @static.Module;
            var Boolean = MainModule.TypeSystem.Boolean;
            var method = new MethodDefinition(nameof(TryGetFirst), StaticMethodAttributes, Boolean)
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

            ParameterDefinition value;
            value = new ParameterDefinition(nameof(value), ParameterAttributes.Out, Element.MakeByReferenceType());
            method.Parameters.Add(value);

            var body = method.Body;

            var processor = body.GetILProcessor();

            MethodReference LongCount;
            LongCount = @this.FindMethod(nameof(LongCount), Helper.NoParameter);

            if (!(type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "FastCountAttribute") is null))
            {
                MethodReference get_Item;
                get_Item = @this.FindMethod(nameof(get_Item));

                var il0011 = Instruction.Create(OpCodes.Ldarg_1);

                processor.Do(OpCodes.Ldarg_0);
                processor.Call(LongCount);
                processor.True(il0011);
                processor.Do(OpCodes.Ldarg_1);
                processor.InitObj(Element);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il0011);
                processor.Do(OpCodes.Ldarg_0);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Do(OpCodes.Conv_I8);
                processor.Call(get_Item);
                if (get_Item.ReturnType.IsByReference)
                    processor.LdObj(Element);
                processor.StObj(Element);
                processor.Do(OpCodes.Ldc_I4_1);
                processor.Ret();
            }
            else
            {
                var variables = body.Variables;
                variables.Add(new VariableDefinition(Enumerator));
                variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));

                processor.Do(OpCodes.Ldarg_0);
                processor.Call(@this.FindMethod("GetEnumerator", Helper.NoParameter));
                processor.Do(OpCodes.Stloc_0);
                processor.Do(OpCodes.Ldarg_1);
                processor.LdLocaS(0);
                processor.LdLocaS(1);
                processor.Call(Enumerator.FindMethod("TryGetNext"));
                processor.LdObj(Element);
                processor.StObj(Element);
                processor.LdLocaS(0);
                processor.Call(Enumerator.FindMethod("Dispose", Helper.NoParameter));
                processor.Do(OpCodes.Ldloc_1);
                processor.Ret();
            }
            @static.Methods.Add(method);
        }
    }
}
