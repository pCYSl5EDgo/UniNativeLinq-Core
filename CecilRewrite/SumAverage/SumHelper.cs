using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class SumHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(SumHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            var numbers = new TypeReference[]
            {
                module.TypeSystem.Byte,
                module.TypeSystem.SByte,
                module.TypeSystem.Int16,
                module.TypeSystem.UInt16,
                module.TypeSystem.Int32,
                module.TypeSystem.UInt32,
                module.TypeSystem.Int64,
                module.TypeSystem.UInt64,
                module.TypeSystem.Single,
                module.TypeSystem.Double,
            };
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                foreach (var number in numbers)
                    Sum(@static, type, number);
            }
        }

        private static void Sum(TypeDefinition @static, TypeDefinition type, TypeReference number)
        {
            var method = new MethodDefinition(nameof(Sum), StaticMethodAttributes, number)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            var @this = new GenericInstanceType(type);
            var typeGenericParameters = type.GenericParameters;
            var addedParams = method.FromTypeToMethodParam(typeGenericParameters, "T", number);

            var index = 0;
            foreach (var genericParameter in type.GenericParameters)
                @this.GenericArguments.Add(genericParameter.Name == "T" ? number : addedParams[index++]);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters, "T", number);
            if (Element.Name != number.Name)
                return;

            var @thisParameter = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType())
            {
                IsIn = true,
            };
            @thisParameter.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(@thisParameter);

            var Enumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters, "T", number);

            var body = method.Body;
            var variables = body.Variables;

            variables.Add(new VariableDefinition(Enumerator));
            variables.Add(new VariableDefinition(number));
            variables.Add(new VariableDefinition(number));

            var il0009 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var il001B = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.GetEnumerator(@this);
            processor.Do(OpCodes.Stloc_0);
            processor.Do(OpCodes.Ldc_I4_0);
            if (number.Name == "Int64" || number.Name == "Uint64")
                processor.Do(OpCodes.Conv_I8);
            else if (number.Name == "Single")
                processor.Do(OpCodes.Conv_R4);
            else if (number.Name == "Double")
                processor.Do(OpCodes.Conv_R8);
            processor.Do(OpCodes.Stloc_1);
            processor.Append(il0009);
            processor.LdLocaS(2);
            processor.Call(Enumerator.FindMethod("TryMoveNext"));
            processor.False(il001B);
            processor.Do(OpCodes.Ldloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.Do(OpCodes.Add);
            if (number.Name == "Byte")
                processor.Do(OpCodes.Conv_U1);
            else if (number.Name == "SByte")
                processor.Do(OpCodes.Conv_I1);
            else if (number.Name == "UInt16")
                processor.Do(OpCodes.Conv_U2);
            else if (number.Name == "Int16")
                processor.Do(OpCodes.Conv_I2);
            processor.Do(OpCodes.Stloc_1);
            processor.Jump(il0009);
            processor.Append(il001B);
            processor.Call(Enumerator.FindMethod("Dispose", Helper.NoParameter));
            processor.Do(OpCodes.Ldloc_1);
            processor.Ret();
            @static.Methods.Add(method);
        }
    }
}
