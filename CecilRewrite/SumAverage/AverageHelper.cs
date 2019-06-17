using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class AverageHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AverageHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
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
                    Average(@static, type, number);
            }
        }

        private static void Average(TypeDefinition @static, TypeDefinition type, TypeReference number)
        {
            var method = new MethodDefinition(nameof(Average), StaticMethodAttributes, number)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            var @this = new GenericInstanceType(type);
            var added = method.FromTypeToMethodParam(type.GenericParameters, "T", number);

            var index = 0;
            foreach (var genericParameter in type.GenericParameters)
                @this.GenericArguments.Add(genericParameter.Name == "T" ? number : added[index++]);

            var Element = @this.GetElementTypeOfCollectionType().Replace(added, "T", number);
            if (Element.Name != number.Name)
                return;

            var @thisParameter = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType())
            {
                IsIn = true,
            };
            @thisParameter.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(@thisParameter);

            var Enumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(added, "T", number);

            var body = method.Body;
            var variables = body.Variables;

            variables.Add(new VariableDefinition(Enumerator));
            switch (number.Name)
            {
                case "Byte":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                    variables.Add(new VariableDefinition(MainModule.TypeSystem.UInt64));
                    variables.Add(new VariableDefinition(MainModule.TypeSystem.UInt64));
                    break;
                case "SByte":
                case "Int16":
                case "Int32":
                case "Int64":
                    variables.Add(new VariableDefinition(MainModule.TypeSystem.Int64));
                    variables.Add(new VariableDefinition(MainModule.TypeSystem.Int64));
                    break;
                case "Single":
                    variables.Add(new VariableDefinition(MainModule.ImportReference(typeof(float))));
                    variables.Add(new VariableDefinition(MainModule.ImportReference(typeof(float))));
                    break;
                case "Double":
                    variables.Add(new VariableDefinition(MainModule.ImportReference(typeof(double))));
                    variables.Add(new VariableDefinition(MainModule.ImportReference(typeof(double))));
                    break;
                default:
                    throw new Exception();
            }
            variables.Add(new VariableDefinition(number));

            var il000D = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var il0024 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

            var processor = body.GetILProcessor();

            processor.Do(OpCodes.Ldarg_0);
            processor.GetEnumerator(@this);
            processor.Do(OpCodes.Stloc_0);
            switch (number.Name)
            {
                case "Byte":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "SByte":
                case "Int16":
                case "Int32":
                case "Int64":
                    processor.Do(OpCodes.Ldc_I4_0);
                    break;
                case "Single":
                    processor.Append(Instruction.Create(OpCodes.Ldc_R4, 0f));
                    break;
                case "Double":
                    processor.Append(Instruction.Create(OpCodes.Ldc_R8, 0.0));
                    break;
            }
            switch (number.Name)
            {
                case "Byte":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                    processor.Do(OpCodes.Conv_U8);
                    break;
                case "SByte":
                case "Int16":
                case "Int32":
                    processor.Do(OpCodes.Conv_I8);
                    break;
            }
            processor.Do(OpCodes.Stloc_2);
            processor.Append(il000D);
            processor.LdLocaS(3);
            processor.Call(Enumerator.FindMethod("TryMoveNext"));
            processor.False(il0024);
            processor.Do(OpCodes.Ldloc_2);
            switch (number.Name)
            {
                case "Byte":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "SByte":
                case "Int16":
                case "Int32":
                case "Int64":
                    processor.Do(OpCodes.Ldc_I4_1);
                    break;
                case "Single":
                    processor.Append(Instruction.Create(OpCodes.Ldc_R4, 1f));
                    break;
                case "Double":
                    processor.Append(Instruction.Create(OpCodes.Ldc_R8, 1.0));
                    break;
            }
            switch (number.Name)
            {
                case "Byte":
                case "UInt16":
                case "UInt32":
                    processor.Do(OpCodes.Conv_U8);
                    break;
                case "SByte":
                case "Int16":
                case "Int32":
                    processor.Do(OpCodes.Conv_I8);
                    break;
            }
            processor.Do(OpCodes.Add);
            processor.Do(OpCodes.Stloc_2);
            processor.Do(OpCodes.Ldloc_1);
            processor.Do(OpCodes.Ldloc_3);
            switch (number.Name)
            {
                case "Byte":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                    processor.Do(OpCodes.Conv_U8);
                    break;
                case "SByte":
                case "Int16":
                case "Int32":
                case "Int64":
                    processor.Do(OpCodes.Conv_I8);
                    break;
                case "Single":
                    processor.Do(OpCodes.Conv_R4);
                    break;
                case "Double":
                    processor.Do(OpCodes.Conv_R8);
                    break;
            }
            processor.Do(OpCodes.Add);
            processor.Do(OpCodes.Stloc_1);
            processor.Jump(il000D);
            processor.Append(il0024);
            processor.Call(Enumerator.FindMethod("Dispose"));
            processor.Do(OpCodes.Ldloc_1);
            processor.Do(OpCodes.Ldloc_2);
            switch (number.Name)
            {
                case "Byte":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                    processor.Do(OpCodes.Div_Un);
                    break;
                case "SByte":
                case "Int16":
                case "Int32":
                case "Int64":
                case "Single":
                case "Double":
                    processor.Do(OpCodes.Div);
                    break;
            }
            switch (number.Name)
            {
                case "Byte":
                    processor.Do(OpCodes.Conv_U1);
                    break;
                case "UInt16":
                    processor.Do(OpCodes.Conv_U2);
                    break;
                case "UInt32":
                    processor.Do(OpCodes.Conv_U4);
                    break;
                case "SByte":
                    processor.Do(OpCodes.Conv_I1);
                    break;
                case "Int16":
                    processor.Do(OpCodes.Conv_I2);
                    break;
                case "Int32":
                    processor.Do(OpCodes.Conv_I4);
                    break;
            }
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
