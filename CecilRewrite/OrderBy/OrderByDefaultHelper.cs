using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class OrderByDefaultHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(OrderByDefaultHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            var numbers = new[]
            {
                MainModule.TypeSystem.Byte,
                MainModule.TypeSystem.SByte,
                MainModule.TypeSystem.Int16,
                MainModule.TypeSystem.UInt16,
                MainModule.TypeSystem.Int32,
                MainModule.TypeSystem.UInt32,
                MainModule.TypeSystem.Int64,
                MainModule.TypeSystem.UInt64,
                MainModule.TypeSystem.Single,
                MainModule.TypeSystem.Double,
            };
            var orders = new[]
            {
                "Ascending",
                "Descending",
            };
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                foreach (var order in orders)
                {
                    OrderBy(@static, type, order);
                    foreach (var number in numbers)
                    {
                        OrderBy(@static, type, number, order);
                    }
                }
            }
        }

        private static void OrderBy(TypeDefinition @static, TypeDefinition type, string order)
        {
            var MainModule = PreCommon(@static, order, out var method);

            var addedParams = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(addedParams);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var Predicate = MainModule.GetType(NameSpace, "DefaultOrderBy" + order + "`1").MakeGenericInstanceType(new[]
            {
                Element
            });
            var Enumerator = type.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            Common(@static, MainModule, @this, Enumerator, Element, Predicate, method);
        }

        private static void OrderBy(TypeDefinition @static, TypeDefinition type, TypeReference number, string order)
        {
            var MainModule = PreCommon(@static, order, out var method);

            var addedParams = method.FromTypeToMethodParam(type.GenericParameters, "T", number);
            var @this = new GenericInstanceType(type);
            var index = 0;
            foreach (var genericParameter in type.GenericParameters)
                @this.GenericArguments.Add(genericParameter.Name == "T" ? number : addedParams[index++]);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters, "T", number);
            if (Element.Name != number.Name)
                return;

            var Predicate = MainModule.GetType(NameSpace, "DefaultOrderBy" + order + number.Name);

            var Enumerator = type.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters, "T", number);

            Common(@static, MainModule, @this, Enumerator, Element, Predicate, method);
        }

        private static ModuleDefinition PreCommon(TypeDefinition @static, string order, out MethodDefinition method)
        {
            var MainModule = @static.Module;
            method = new MethodDefinition(nameof(OrderBy) + (order == "Ascending" ? "" : order), StaticMethodAttributes, MainModule.TypeSystem.Void)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            return MainModule;
        }

        private static void Common(TypeDefinition @static, ModuleDefinition MainModule, GenericInstanceType @this, TypeReference Enumerator, TypeReference Element, TypeReference Predicate, MethodDefinition method)
        {
            var @return = MainModule.GetType(NameSpace, "OrderByEnumerable`4").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                Predicate,
            });

            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var allocParam = new ParameterDefinition("allocator", ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
                // この一行をたさないとエラーになる。
                // なぜか0 == Invalidで初期化されたことになる。
                // バグの原因としてはAttributesに4096を設定しない限り既定値を持たない==0初期化されると解釈されるようである
                HasDefault = true
            };
            method.Parameters.Add(allocParam);

            var body = method.Body;

            body.Variables.Add(new VariableDefinition(Predicate));

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_1);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
