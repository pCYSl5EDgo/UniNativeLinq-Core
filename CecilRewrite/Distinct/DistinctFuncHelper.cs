using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class DistinctFuncHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition @static;
            @static = new TypeDefinition(NameSpace,
                nameof(DistinctFuncHelper),
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

            GenericInstanceType TEqualityComparer0, TGetHashCodeFunc0;
            GenericInstanceType TEqualityComparer0Func, TGetHashCodeFunc0Func;
            var EqualityTuple = new[]
            {
                Element,
                Element,
                MainModule.TypeSystem.Boolean,
            };
            var GetHashCodeTuple = new[]
            {
                Element,
                MainModule.TypeSystem.Int32,
            };
            TEqualityComparer0 = MainModule.GetType(NameSpace, "DelegateFuncToStructOperatorFunc`3").MakeGenericInstanceType(EqualityTuple);
            TEqualityComparer0Func = MainModule.ImportReference(SystemModule.GetType("System", "Func`3")).MakeGenericInstanceType(EqualityTuple);
            TGetHashCodeFunc0 = MainModule.GetType(NameSpace, "DelegateFuncToStructOperatorFunc`2").MakeGenericInstanceType(GetHashCodeTuple);
            TGetHashCodeFunc0Func = MainModule.ImportReference(SystemModule.GetType("System", "Func`2")).MakeGenericInstanceType(GetHashCodeTuple);

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

            var comparerParam = new ParameterDefinition("comparer", ParameterAttributes.None, TEqualityComparer0Func);
            method.Parameters.Add(comparerParam);

            var hashCodeFuncParam = new ParameterDefinition("func", ParameterAttributes.None, TGetHashCodeFunc0Func);
            method.Parameters.Add(hashCodeFuncParam);

            method.Parameters.Add(new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            });

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(TEqualityComparer0));
            variables.Add(new VariableDefinition(TGetHashCodeFunc0));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.Do(OpCodes.Ldarg_3);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
