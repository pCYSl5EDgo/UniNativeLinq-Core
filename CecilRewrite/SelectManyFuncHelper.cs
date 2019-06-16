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
    static class SelectManyFuncHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(SelectManyFuncHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);

            var typeDefinitions = module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")).ToArray();
            foreach (var type in typeDefinitions)
            {
                SelectManyArray(@static, type);
                SelectManyNativeArray(@static, type);
                foreach (var type1 in typeDefinitions)
                    SelectMany(@static, type, type1);
            }
        }

        private static void SelectManyCommon(TypeDefinition @static, TypeDefinition type, string name, Func<GenericParameter, TypeReference> thisFunc, Func<MethodDefinition, bool> predicate)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(SelectMany), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            GenericParameter T;
            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(T);

            var @this = thisFunc(T);
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, @this));

            var Enumerable = MainModule.GetType(NameSpace, name + "Enumerable`1").MakeGenericInstanceType(T);
            var Enumerator = Enumerable.GetEnumeratorTypeOfCollectionType().Replace(new[] { T });

            const string suffix = "00";
            var added1 = method.FromTypeToMethodParam(type.GenericParameters, suffix);
            var TEnumerable0 = type.MakeGenericInstanceType(added1);

            var TEnumerator0 = TEnumerable0.GetEnumeratorTypeOfCollectionType().Replace(added1, suffix);
            var T0 = TEnumerable0.GetElementTypeOfCollectionType().Replace(added1, suffix);

            var Func = MainModule.ImportReference(SystemModule.GetType("System", "Func`2")).MakeGenericInstanceType(new TypeReference[]
            {
                T,
                TEnumerable0
            });

            var funcParam = new ParameterDefinition("func", ParameterAttributes.None, Func);
            method.Parameters.Add(funcParam);

            var TAction = MainModule.GetType(NameSpace, "DelegateFuncToStructOperatorAction`2").MakeGenericInstanceType(new TypeReference[]
            {
                T,
                TEnumerable0
            });

            var @return = MainModule.GetType(NameSpace, "SelectManyEnumerable`7").MakeGenericInstanceType(new[]
            {
                Enumerable,
                Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TAction,
            });
            method.ReturnType = @return;

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(Enumerable));
            variables.Add(new VariableDefinition(TAction));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            var AsRefEnumerable = new GenericInstanceMethod(MainModule.GetType(NameSpace, "NativeEnumerable").Methods.First(predicate));
            AsRefEnumerable.GenericArguments.Add(T);
            processor.Call(AsRefEnumerable);
            processor.Do(OpCodes.Stloc_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_1);
            processor.NewObj(TAction.FindMethod(".ctor"));
            processor.Do(OpCodes.Stloc_1);
            processor.LdLocaS(1);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void SelectManyArray(TypeDefinition @static, TypeDefinition type)
            => SelectManyCommon(@static, type, "Array", 
                x => x.MakeArrayType(),
                x => x.Parameters.Count == 1 && x.Parameters.First().ParameterType.IsArray);

        private static void SelectManyNativeArray(TypeDefinition @static, TypeDefinition type)
            => SelectManyCommon(@static, type, "Native", 
                x => NativeArray.MakeGenericInstanceType(x),
                x => x.Parameters.Count == 1 && !x.Parameters.First().ParameterType.IsArray);

        private static void SelectMany(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(SelectMany), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            const string suffix0 = "00";
            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, suffix0);
            var @this = type0.MakeGenericInstanceType(added0);

            var Element0 = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters, suffix0);
            var Enumerator0 = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters, suffix0);

            const string suffix1 = "01";
            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, suffix1);
            var TEnumerable = type1.MakeGenericInstanceType(added1);

            var TEnumerator = TEnumerable.GetEnumeratorTypeOfCollectionType().Replace(added1, suffix1);
            var T = TEnumerable.GetElementTypeOfCollectionType().Replace(added1, suffix1);

            var Func = MainModule.ImportReference(SystemModule.GetType("System", "Func`2")).MakeGenericInstanceType(new[]
            {
                Element0,
                TEnumerable
            });

            var TAction = MainModule.GetType(NameSpace, "DelegateFuncToStructOperatorAction`2").MakeGenericInstanceType(new[]
            {
                Element0,
                TEnumerable
            });

            var @return = MainModule.GetType(NameSpace, "SelectManyEnumerable`7").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator0,
                Element0,
                TEnumerable,
                TEnumerator,
                T,
                TAction,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var funcParam = new ParameterDefinition("func", ParameterAttributes.None, Func);
            method.Parameters.Add(funcParam);

            method.Body.Variables.Add(new VariableDefinition(TAction));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_1);
            processor.NewObj(TAction.FindMethod(".ctor"));
            processor.Append(Instruction.Create(OpCodes.Stloc_0));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
