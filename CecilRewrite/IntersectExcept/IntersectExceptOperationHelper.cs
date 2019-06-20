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

    static class IntersectExceptOperationHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition IntersectOperatorHelper = new TypeDefinition(NameSpace,
                nameof(IntersectOperatorHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            IntersectOperatorHelper.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(IntersectOperatorHelper);

            TypeDefinition ExceptOperatorHelper = new TypeDefinition(NameSpace,
                nameof(ExceptOperatorHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            ExceptOperatorHelper.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(ExceptOperatorHelper);

            var Intersect = module.GetType("UniNativeLinq", "IntersectOperation`6");
            var Except = module.GetType("UniNativeLinq", "ExceptOperation`6");
            foreach (var type0 in Enumerables)
            {
                Make(IntersectOperatorHelper, type0, "Array", 0, CalcArrayType, AsRefEnumerableArray, nameof(Intersect), Intersect);
                Make(IntersectOperatorHelper, type0, "Array", 1, CalcArrayType, AsRefEnumerableArray, nameof(Intersect), Intersect);
                Make(ExceptOperatorHelper, type0, "Native", 0, NativeArray.MakeGenericInstanceType, AsRefEnumerableNative, nameof(Except), Except);
                Make(ExceptOperatorHelper, type0, "Native", 1, NativeArray.MakeGenericInstanceType, AsRefEnumerableNative, nameof(Except), Except);
                foreach (var type1 in Enumerables)
                {
                    Make(IntersectOperatorHelper, type0, type1, nameof(Intersect), Intersect);
                    Make(ExceptOperatorHelper, type0, type1, nameof(Except), Except);
                }
            }
        }

        private static void Make(TypeDefinition @static, TypeDefinition type, string prefix, int index, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodReference constructor, string name, TypeDefinition operation)
        {
            var MainModule = Prologue(@static, type, name, out var method, out var T, out var Enumerable0, out var Enumerator0, out var Element0, out var TComparer);
            if (Element0.Name != nameof(T)) return;

            var Input = InputFunc(new[] { T });

            var Enumerable = MainModule.GetType(NameSpace, prefix + "Enumerable`1").MakeGenericInstanceType(T);
            var Enumerator = MainModule.GetType(NameSpace, prefix + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(T);

            var OperationGenericArguments = new[]
            {
                Enumerable0,
                Enumerator0,
                Enumerable0,
                Enumerator0,
                T,
                TComparer,
            };
            OperationGenericArguments[index << 1] = Enumerable;
            OperationGenericArguments[1 + (index << 1)] = Enumerator;
            var Operation = operation.MakeGenericInstanceType(OperationGenericArguments);

            var ReturnGenericArguments = new[]
            {
                Enumerable0,
                Enumerator0,
                Enumerable0,
                Enumerator0,
                T,
                Operation,
            };
            ReturnGenericArguments[index << 1] = Enumerable;
            ReturnGenericArguments[(index << 1) + 1] = Enumerator;
            var @return = MainModule.GetType(NameSpace, "SetOperationEnumerable`6").MakeGenericInstanceType(ReturnGenericArguments);
            method.ReturnType = @return;

            ParameterDefinition thisParam, secondParam;
            if (index == 0)
            {
                thisParam = new ParameterDefinition("@this", ParameterAttributes.None, Input);
                method.Parameters.Add(thisParam);

                secondParam = new ParameterDefinition("second", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
                secondParam.CustomAttributes.Add(IsReadOnlyAttribute);
                method.Parameters.Add(secondParam);
            }
            else
            {
                thisParam = new ParameterDefinition("@this", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
                thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
                method.Parameters.Add(thisParam);

                secondParam = new ParameterDefinition("second", ParameterAttributes.None, Input);
                method.Parameters.Add(secondParam);
            }

            var comparerParam = new ParameterDefinition("comparer", ParameterAttributes.In, TComparer.MakeByReferenceType());
            comparerParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(comparerParam);

            var allocParam = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocParam);

            var _constructor = new GenericInstanceMethod(constructor);
            _constructor.GenericArguments.Add(T);

            method.Body.Variables.Add(new VariableDefinition(Enumerable));

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
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void Make(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1, string name, TypeDefinition operation)
        {
            var MainModule = Prologue(@static, type0, name, out var method, out var T, out var Enumerable0, out var Enumerator0, out var Element0, out var TComparer);

            const string suffix1 = "1";

            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, nameof(T), T, suffix1);
            foreach (var parameter in added1)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var Enumerable1 = type1.MakeGenericInstanceType(added1.Append(T));

            var Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);
            var Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);

            if (!Element0.Equals(Element1))
                return;

            var Operation = operation.MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Enumerable1,
                Enumerator1,
                T,
                TComparer,
            });

            var @return = MainModule.GetType(NameSpace, "SetOperationEnumerable`6").MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Enumerable1,
                Enumerator1,
                T,
                Operation,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var secondParam = new ParameterDefinition("second", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            secondParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(secondParam);

            var comparerParam = new ParameterDefinition("comparer", ParameterAttributes.In, TComparer.MakeByReferenceType());
            comparerParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(comparerParam);

            var allocParam = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocParam);

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_3);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static ModuleDefinition Prologue(TypeDefinition @static, TypeDefinition type0, string name, out MethodDefinition method, out GenericParameter T, out GenericInstanceType Enumerable0, out TypeReference Enumerator0, out TypeReference Element0, out GenericParameter TComparer)
        {
            var MainModule = @static.Module;
            method = new MethodDefinition(name, StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            T = new GenericParameter(nameof(T), method) {HasNotNullableValueTypeConstraint = true};
            T.CustomAttributes.Add(UnManagedAttribute);
            T.Constraints.Add(MainModule.ImportReference(SystemModule.GetType("System", "IComparable`1")).MakeGenericInstanceType(T));

            const string suffix0 = "0";

            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, nameof(T), T, suffix0);
            foreach (var parameter in added0)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            Enumerable0 = type0.MakeGenericInstanceType(added0.Append(T));

            Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
            Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);

            method.GenericParameters.Add(T);

            TComparer = new GenericParameter(nameof(TComparer), method) {HasNotNullableValueTypeConstraint = true};
            TComparer.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`3").MakeGenericInstanceType(new[]
            {
                T,
                T,
                MainModule.TypeSystem.Int32,
            }));
            method.GenericParameters.Add(TComparer);
            return MainModule;
        }
    }
}
