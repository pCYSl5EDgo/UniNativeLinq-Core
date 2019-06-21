using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;

    static class ZipHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition Adjusted = new TypeDefinition(NameSpace,
                nameof(Adjusted) + nameof(ZipHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Adjusted.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Adjusted);

            TypeDefinition Exceptional = new TypeDefinition(NameSpace,
                nameof(Exceptional) + nameof(ZipHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Exceptional.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Exceptional);
            foreach (var type0 in Enumerables)
            {
                Zip(Exceptional, type0, "Array", CalcArrayType, AsRefEnumerableArray, nameof(Exceptional), 0);
                Zip(Exceptional, type0, "Array", CalcArrayType, AsRefEnumerableArray, nameof(Exceptional), 1);
                Zip(Exceptional, type0, "Native", NativeArray.MakeGenericInstanceType, AsRefEnumerableNative, nameof(Exceptional), 0);
                Zip(Exceptional, type0, "Native", NativeArray.MakeGenericInstanceType, AsRefEnumerableNative, nameof(Exceptional), 1);
                Zip(Adjusted, type0, "Array", CalcArrayType, AsRefEnumerableArray, nameof(Adjusted), 0);
                Zip(Adjusted, type0, "Array", CalcArrayType, AsRefEnumerableArray, nameof(Adjusted), 1);
                Zip(Adjusted, type0, "Native", NativeArray.MakeGenericInstanceType, AsRefEnumerableNative, nameof(Adjusted), 0);
                Zip(Adjusted, type0, "Native", NativeArray.MakeGenericInstanceType, AsRefEnumerableNative, nameof(Adjusted), 1);
                foreach (var type1 in Enumerables)
                {
                    Zip(Exceptional, type0, type1, nameof(Exceptional));
                    Zip(Adjusted, type0, type1, nameof(Adjusted));
                }
            }
        }
        private static void Zip(TypeDefinition @static, TypeDefinition type, string prefix, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodDefinition constructor, string typePrefix, int index)
        {
            var method = ModuleDefinition(@static, out var MainModule);

            var ElementX = new GenericParameter("T" + index.ToString(), method) { HasNotNullableValueTypeConstraint = true };
            ElementX.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(ElementX);
            var EnumerableX = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(ElementX);
            var EnumeratorX = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(ElementX);

            Routine(type, method, (1 - index).ToString(), out var Enumerable, out var Enumerator, out var Element);

            TypeReference TAction;
            GenericInstanceType @return;
            if (index == 0)
                Epilogue(method, MainModule, typePrefix, EnumerableX, EnumeratorX, ElementX, Enumerable, Enumerator, Element, out @return, out _, out TAction);
            else
                Epilogue(method, MainModule, typePrefix, Enumerable, Enumerator, Element, EnumerableX, EnumeratorX, ElementX, out @return, out _, out TAction);

            ParameterDefinition first, second;
            var Input = InputFunc(new[] { ElementX });
            if (index == 0)
            {
                first = new ParameterDefinition(nameof(second), ParameterAttributes.None, Input);

                second = new ParameterDefinition(nameof(second), ParameterAttributes.In, Enumerable.MakeByReferenceType());
                second.CustomAttributes.Add(IsReadOnlyAttribute);
            }
            else
            {
                first = new ParameterDefinition(nameof(first), ParameterAttributes.In, Enumerable.MakeByReferenceType());
                first.CustomAttributes.Add(IsReadOnlyAttribute);

                second = new ParameterDefinition(nameof(second), ParameterAttributes.None, Input);
            }

            method.Parameters.Add(first);
            method.Parameters.Add(second);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(ElementX);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(EnumerableX));
            variables.Add(new VariableDefinition(TAction));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            if (index == 0)
            {
                processor.Call(_constructor);
                processor.Do(OpCodes.Stloc_0);
                processor.LdLocaS(0);
            }
            processor.Do(OpCodes.Ldarg_1);
            if (index != 0)
            {
                processor.Call(_constructor);
                processor.Do(OpCodes.Stloc_0);
                processor.LdLocaS(0);
            }
            processor.LdLocaS(1);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void Zip(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1, string typePrefix)
        {
            var method = ModuleDefinition(@static, out var MainModule);

            Routine(type0, method, "0", out var Enumerable0, out var Enumerator0, out var Element0);

            Routine(type1, method, "1", out var Enumerable1, out var Enumerator1, out var Element1);

            Epilogue(method, MainModule, typePrefix, Enumerable0, Enumerator0, Element0, Enumerable1, Enumerator1, Element1, out var @return, out var T, out var TAction);

            ParameterDefinition first = new ParameterDefinition(nameof(first), ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            first.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(first);

            ParameterDefinition second = new ParameterDefinition(nameof(second), ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            second.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(second);

            var variables = method.Body.Variables;
            variables.Add(new VariableDefinition(TAction));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.LdLocaS(0);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static MethodDefinition ModuleDefinition(TypeDefinition @static, out ModuleDefinition MainModule)
        {
            MainModule = @static.Module;
            var method = new MethodDefinition(nameof(Zip), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            return method;
        }

        private static void Epilogue(MethodDefinition method, ModuleDefinition MainModule, string typePrefix, GenericInstanceType enumerable0, TypeReference enumerator0, TypeReference element0, GenericInstanceType enumerable1, TypeReference enumerator1, TypeReference element1, out GenericInstanceType @return, out TypeReference T, out TypeReference TAction)
        {
            var Tuple2 = new[] { element0, element1 };
            T = MainModule.ImportReference(SystemModule.GetType("System", "ValueTuple`2")).MakeGenericInstanceType(Tuple2);
            TAction = MainModule.GetType(NameSpace, "ZipValueTuple`2").MakeGenericInstanceType(Tuple2);

            @return = MainModule.GetType(NameSpace, typePrefix + "ZipEnumerable`8").MakeGenericInstanceType(new[]
            {
                enumerable0,
                enumerator0,
                element0,
                enumerable1,
                enumerator1,
                element1,
                T,
                TAction,
            });
            method.ReturnType = @return;
        }

        private static void Routine(TypeDefinition type, MethodDefinition method, string suffix, out GenericInstanceType Enumerable, out TypeReference Enumerator, out TypeReference Element)
        {
            var added = method.FromTypeToMethodParam(type.GenericParameters, suffix);
            Enumerable = type.MakeGenericInstanceType(added);
            Enumerator = Enumerable.GetEnumeratorTypeOfCollectionType().Replace(added, suffix);
            Element = Enumerable.GetElementTypeOfCollectionType().Replace(added, suffix);
        }
    }
}
