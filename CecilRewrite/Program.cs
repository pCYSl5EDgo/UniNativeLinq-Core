using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    public static class Program
    {
        internal const string NameSpace = "UniNativeLinq";

        internal const TypeAttributes StaticExtensionClassTypeAttributes = TypeAttributes.AnsiClass | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Abstract;
        internal const MethodAttributes StaticMethodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        internal static readonly ModuleDefinition MainModule;
        internal static readonly CustomAttribute ExtensionAttribute;
        internal static readonly CustomAttribute IsReadOnlyAttribute;
        internal static readonly CustomAttribute UnManagedAttribute;
        internal static readonly ModuleDefinition SystemModule;
        internal static readonly AssemblyDefinition Assembly;
        internal static readonly TypeReference Allocator;
        internal static readonly TypeReference NativeArray;
        internal static readonly TypeDefinition[] Enumerables;
        internal static readonly MethodDefinition AsRefEnumerableArray;
        internal static readonly MethodDefinition AsRefEnumerableNative;

        static Program()
        {
            const string folderPath = @"C:\Users\conve\source\repos\pcysl5edgo\UniNativeLinq\bin\Release\netstandard2.0\";
            const string UniNativeLinqDll = folderPath + @"UniNativeLinq.dll";
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(folderPath);
            Assembly = AssemblyDefinition.ReadAssembly(UniNativeLinqDll, new ReaderParameters(readingMode: ReadingMode.Deferred) { AssemblyResolver = resolver });
            MainModule = Assembly.MainModule;
            var nativeEnumerable = MainModule.GetType("UniNativeLinq.NativeEnumerable");
            AsRefEnumerableArray = nativeEnumerable.Methods.First(x => x.Name == "AsRefEnumerable" && x.Parameters.Count == 1 && x.Parameters.First().ParameterType.IsArray);
            AsRefEnumerableNative = nativeEnumerable.Methods.First(x => x.Name == "AsRefEnumerable" && x.Parameters.Count == 1 && !x.Parameters.First().ParameterType.IsArray);
            ExtensionAttribute = nativeEnumerable.CustomAttributes.Single();
            var negateMethodDefinition = MainModule.GetType("UniNativeLinq.NegatePredicate`2").GetConstructors().First();
            IsReadOnlyAttribute = negateMethodDefinition.Parameters.First().CustomAttributes.First();
            var nativeEnumerable1 = MainModule.GetType(NameSpace, "NativeEnumerable`1");
            MethodDefinition ToNativeArray;
            ToNativeArray = nativeEnumerable1.Methods.First(x => x.Name == nameof(ToNativeArray));
            NativeArray = MainModule.ImportReference(ToNativeArray.ReturnType.Resolve());
            Allocator = ToNativeArray.Parameters.First().ParameterType;
            var t = nativeEnumerable1.GenericParameters.First();
            UnManagedAttribute = t.CustomAttributes[0];
            SystemModule = ModuleDefinition.ReadModule(@"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll");
            Enumerables = MainModule.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")).ToArray();
        }

        public static void Main(string[] args)
        {
            RewriteThrow(MainModule);
            ReWritePseudoIsReadOnly(MainModule);
            ReWritePseudoUtility(MainModule);

            //TryGetMinHelper.Create(MainModule);
            //TryGetMaxHelper.Create(MainModule);
            //TryGetMinFuncHelper.Create(MainModule);
            //TryGetMaxFuncHelper.Create(MainModule);
            //TryGetMinOperatorHelper.Create(MainModule);
            //TryGetMaxOperatorHelper.Create(MainModule);
            //AnyOperatorHelper.Create(MainModule);
            //AllOperatorHelper.Create(MainModule);
            //AnyFuncHelper.Create(MainModule);
            //AllFuncHelper.Create(MainModule);
            //IsEmptyHelper.Create(MainModule);
            //AggregateOperatorSmallHelper.Create(MainModule);
            //AggregateFunctionSmallHelper.Create(MainModule);
            //AggregateOperatorWithResultTypeHelper.Create(MainModule);
            //AggregateFunctionWithResultTypeHelper.Create(MainModule);
            //ContainsDefaultEqualityComparerHelper.Create(MainModule);
            //ContainsFuncHelper.Create(MainModule);
            //TryGetElementAtHelper.Create(MainModule);
            //TryGetFirstHelper.Create(MainModule);
            //TryGetLastHelper.Create(MainModule);
            //TryGetSingleHelper.Create(MainModule);
            //SumHelper.Create(MainModule);
            //AverageHelper.Create(MainModule);
            //AppendPrependDefaultIfEmptyHelper.Create(MainModule);
            //SelectOperatorHelper.Create(MainModule);
            //SelectFuncHelper.Create(MainModule);
            //SkipTakeWhileWhereOperatorHelper.Create(MainModule);
            //SkipTakeWhileFuncWhereHelper.Create(MainModule);
            //OrderByOperatorHelper.Create(MainModule);
            //OrderByDefaultHelper.Create(MainModule);
            //ReverseHelper.Create(MainModule);
            //SkipTakeLastHelper.Create(MainModule);
            //DistinctDefaultHelper.Create(MainModule);
            //DistinctOperatorHelper.Create(MainModule);
            //GroupByHelper.Create(MainModule);
            //SelectIndexFuncHelper.Create(MainModule);
            //SelectIndexOperatorHelper.Create(MainModule);
            //WhereIndexOperatorHelper.Create(MainModule);
            //WhereIndexFunctionHelper.Create(MainModule);
            //MinMaxOperatorHelper.Create(MainModule);
            //MinMaxFuncHelper.Create(MainModule);
            //SelectManyOperatorHelper.Create(MainModule);
            //SelectManyFuncHelper.Create(MainModule);
            //RepeatHelper.Create(MainModule);
            //WithIndexHelper.Create(MainModule);
            //ConcatHelper.Create(MainModule);
            //IntersectExceptDefaultComparerHelper.Create(MainModule);
            //IntersectExceptOperationHelper.Create(MainModule);
            //IntersectExceptFuncHelper.Create(MainModule);
            //GroupJoinDefaultEqualityComparerOperatorHelper.Create(MainModule);
            //GroupJoinDefaultEqualityComparerFuncHelper.Create(MainModule);
            //GroupJoinOperatorHelper.Create(MainModule);
            //GroupJoinFuncHelper.Create(MainModule);
            //JoinDefaultEqualityComparerOperatorHelper.Create(MainModule);
            //JoinDefaultEqualityComparerFuncHelper.Create(MainModule);
            //JoinOperatorHelper.Create(MainModule);
            //JoinFuncHelper.Create(MainModule);

            Assembly.Write(@"C:\Users\conve\source\repos\pcysl5edgo\UniNativeLinq\bin\Release\UniNativeLinq.dll");
        }

        private static void ReWritePseudoUtility(ModuleDefinition module)
        {
            foreach (var type in module.GetTypes()
                .Where(x => x.IsValueType))
            {
                foreach (var method in type.Methods)
                {
                    var processor = method.Body.GetILProcessor();
                    var instructions = method.Body.Instructions;
                    for (int i = instructions.Count; --i >= 0;)
                    {
                        var instruction = instructions[i];
                        if (instruction.OpCode != OpCodes.Call || !(instruction.Operand is MethodReference methodReference) || methodReference.DeclaringType.Name != "Pseudo") continue;
                        if (methodReference.Name == "AsRefNull")
                        {
                            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldc_I4_0));
                            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Conv_U));
                        }
                        processor.Remove(instruction);
                    }
                }
            }
            var pseudo = module.GetType(NameSpace, "Pseudo");
            module.Types.Remove(pseudo);
        }

        private static void ReWritePseudoIsReadOnlyGeneric<T>(this T member)
            where T : ICustomAttributeProvider
        {
            var memberCustomAttributes = member.CustomAttributes;
            var pseudo = memberCustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "PseudoIsReadOnlyAttribute");
            if (!(pseudo is null))
            {
                memberCustomAttributes.Remove(pseudo);
                memberCustomAttributes.Add(IsReadOnlyAttribute);
            }
        }

        private static void ReWritePseudoIsReadOnly(ModuleDefinition module)
        {
            foreach (var type in module.GetTypes()
                .Where(x => x.IsValueType && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                ReWritePseudoIsReadOnly(type);
            }
            var pseudo = MainModule.GetType(NameSpace, "PseudoIsReadOnlyAttribute");
            MainModule.Types.Remove(pseudo);
        }

        private static void ReWritePseudoIsReadOnly(TypeDefinition type)
        {
            type.ReWritePseudoIsReadOnlyGeneric();
            foreach (var field in type.Fields)
            {
                var fieldCustomAttributes = field.CustomAttributes;
                var pseudo = fieldCustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "PseudoIsReadOnlyAttribute");
                if (!(pseudo is null))
                {
                    fieldCustomAttributes.Remove(pseudo);
                    field.IsInitOnly = true;
                }
            }
            foreach (var method in type.Methods)
            {
                method.ReWritePseudoIsReadOnlyGeneric();
                foreach (var parameter in method.Parameters)
                {
                    parameter.ReWritePseudoIsReadOnlyGeneric();
                }
            }
            foreach (var nestedType in type.NestedTypes)
                ReWritePseudoIsReadOnly(nestedType);
        }

        internal static void RewriteThrow(ModuleDefinition module)
        {
            const string local = "LocalRefReturnAttribute";
            var enumeratorTypes = module.GetTypes()
                .Where(x => x.IsPublic && x.IsValueType && x.HasNestedTypes)
                .SelectMany(x => x.NestedTypes)
                .Where(x => x.HasCustomAttributes && x.CustomAttributes.Any(y => y.AttributeType.Name == local));
            foreach (var enumeratorType in enumeratorTypes)
            {
                ReWriteRefReturn(module, enumeratorType);
                var remove = enumeratorType.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == local);
                if (remove is null) continue;
                enumeratorType.CustomAttributes.Remove(remove);
            }
            {
                var remove = module.GetType(NameSpace, local);
                module.Types.Remove(remove);
            }
        }

        internal static void ReWriteRefReturn(ModuleDefinition module, TypeDefinition type)
        {
            var element = type.Fields.First(x => x.Name == "element").FillGenericParams();
            ReWrite(type.Methods.First(x => x.Name == "TryGetNext"), element);
            static bool Predicate(PropertyDefinition x)
                => x.Name == "Current" && x.PropertyType.IsByReference;
            ReWrite(type.Properties.First(Predicate).GetMethod, element);
        }

        internal static FieldReference FillGenericParams(this FieldDefinition definition)
        {
            var declaringType = new GenericInstanceType(definition.DeclaringType);
            foreach (var param in definition.DeclaringType.GenericParameters)
                declaringType.GenericArguments.Add(param);
            return new FieldReference(definition.Name, definition.FieldType, declaringType);
        }

        internal static void ReWrite(MethodDefinition method, FieldReference field)
        {
            var body = method.Body;
            var processor = body.GetILProcessor();
            var ldflda = Instruction.Create(OpCodes.Ldflda, field);
            var ret = Instruction.Create(OpCodes.Ret);
            var ldarg0 = Instruction.Create(OpCodes.Ldarg_0);
            for (var i = body.Instructions.Count; --i >= 0;)
            {
                var instruction = body.Instructions[i];
                if (instruction.OpCode == OpCodes.Throw)
                    processor.Replace(instruction, ret);
                else if (instruction.OpCode == OpCodes.Newobj)
                {
                    processor.InsertBefore(instruction, ldarg0);
                    processor.Replace(instruction, ldflda);
                }
            }
        }

        internal static TypeReference CalcArrayType(IEnumerable<TypeReference> xs) => xs.First().MakeArrayType();
    }
}
