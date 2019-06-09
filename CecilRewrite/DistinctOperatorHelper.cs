using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class DistinctOperatorHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition @static;
            @static = new TypeDefinition(NameSpace,
                nameof(DistinctOperatorHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);

            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                Distinct(@static, type);
            }
        }

        private static void Distinct(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(Distinct), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);

            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);

            GenericParameter TEqualityComparer0, TGetHashCodeFunc0;
            TEqualityComparer0 = new GenericParameter(nameof(TEqualityComparer0), method)
            {
                HasNotNullableValueTypeConstraint = true,
                IsValueType = true,
            };
            TEqualityComparer0.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`3").MakeGenericInstanceType(new[]
            {
                Element,
                Element,
                MainModule.TypeSystem.Boolean,
            }));
            method.GenericParameters.Add(TEqualityComparer0);

            TGetHashCodeFunc0 = new GenericParameter(nameof(TGetHashCodeFunc0), method)
            {
                HasNotNullableValueTypeConstraint = true,
                IsValueType = true,
            };
            TGetHashCodeFunc0.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericInstanceType(new[]
            {
                Element,
                MainModule.TypeSystem.Int32,
            }));
            method.GenericParameters.Add(TGetHashCodeFunc0);

            var @return = MainModule.GetType(NameSpace, nameof(Distinct) + "Enumerable`5").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                TEqualityComparer0,
                TGetHashCodeFunc0,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var comparerParam = new ParameterDefinition("comparer", ParameterAttributes.In, TEqualityComparer0.MakeByReferenceType());
            comparerParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(comparerParam);

            var hashCodeFuncParam = new ParameterDefinition("func", ParameterAttributes.In, TGetHashCodeFunc0.MakeByReferenceType());
            hashCodeFuncParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(hashCodeFuncParam);

            method.Parameters.Add(new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            });

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
