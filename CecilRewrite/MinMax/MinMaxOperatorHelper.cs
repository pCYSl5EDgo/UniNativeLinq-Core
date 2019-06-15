using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;

    static class MinMaxOperatorHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition Int32;
            Int32 = new TypeDefinition(NameSpace,
                nameof(MinMaxOperatorHelper) + nameof(Int32),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Int32.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Int32);

            TypeDefinition Double;
            Double = new TypeDefinition(NameSpace,
                nameof(MinMaxOperatorHelper) + nameof(Double),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Double.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Double);

            TypeDefinition Single;
            Single = new TypeDefinition(NameSpace,
                nameof(MinMaxOperatorHelper) + nameof(Single),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Single.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Single);

            TypeDefinition UInt64;
            UInt64 = new TypeDefinition(NameSpace,
                nameof(MinMaxOperatorHelper) + nameof(UInt64),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            UInt64.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(UInt64);

            TypeDefinition UInt32;
            UInt32 = new TypeDefinition(NameSpace,
                nameof(MinMaxOperatorHelper) + nameof(UInt32),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            UInt32.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(UInt32);

            TypeDefinition Int64;
            Int64 = new TypeDefinition(NameSpace,
                nameof(MinMaxOperatorHelper) + nameof(Int64),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Int64.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Int64);

            var numbers = new[]
            {
                (Int32, module.TypeSystem.Int32),
                (Int64, module.TypeSystem.Int64),
                (UInt32, module.TypeSystem.UInt32),
                (UInt64, module.TypeSystem.UInt64),
                (Single, module.TypeSystem.Single),
                (Double, module.TypeSystem.Double),
            };
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                foreach (var (@static, number) in numbers)
                {
                    MinMax(@static, type, number, "MinBy");
                    MinMax(@static, type, number, "MaxBy");
                }
            }
        }

        private static void MinMax(TypeDefinition @static, TypeDefinition type, TypeReference number, string name)
        {
            var method = new MethodDefinition(name, StaticMethodAttributes, number)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            var addedParams = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(addedParams);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            GenericParameter TKeySelector0;
            TKeySelector0 = new GenericParameter(nameof(TKeySelector0), method) { HasNotNullableValueTypeConstraint = true };
            TKeySelector0.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[]
            {
                Element,
                number
            }));
            method.GenericParameters.Add(TKeySelector0);

            var @return = MainModule.GetType(NameSpace, name + "Enumerable" + number.Name + "`4").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                TKeySelector0,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var funcParam = new ParameterDefinition("func", ParameterAttributes.In, TKeySelector0.MakeByReferenceType());
            funcParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(funcParam);

            var allocatorParam = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocatorParam);

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
