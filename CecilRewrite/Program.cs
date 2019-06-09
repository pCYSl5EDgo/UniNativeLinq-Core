using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    static class Program
    {
        internal const string NameSpace = "UniNativeLinq";

        internal const TypeAttributes StaticExtensionClassTypeAttributes = TypeAttributes.AnsiClass | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Abstract;
        internal const MethodAttributes StaticMethodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        internal static readonly ModuleDefinition MainModule;
        internal static readonly ModuleDefinition UnsafeModule;
        internal static readonly CustomAttribute ExtensionAttribute;
        // ReSharper disable once InconsistentNaming
        internal static readonly CustomAttribute IsReadOnlyAttribute;
        internal static readonly ModuleDefinition UnityModule;
        internal static readonly AssemblyDefinition Assembly;
        internal static readonly TypeReference Allocator;

        static Program()
        {
            var location = @"C:\Users\conve\source\repos\pcysl5edgo\CecilRewrite\bin\Release\UniNativeLinq.dll";
            Assembly = AssemblyDefinition.ReadAssembly(location);
            MainModule = Assembly.MainModule;
            var nativeEnumerable = MainModule.GetType("UniNativeLinq.NativeEnumerable");
            ExtensionAttribute = nativeEnumerable.CustomAttributes.Single();
            var negateMethodDefinition = MainModule.GetType("UniNativeLinq.NegatePredicate`2").GetConstructors().First();
            IsReadOnlyAttribute = negateMethodDefinition.Parameters.First().CustomAttributes.First();
            UnsafeModule = ExtensionAttribute.AttributeType.Module;
            UnityModule = nativeEnumerable.Methods.First(x => x.Parameters.First().ParameterType.IsValueType).Parameters.First().ParameterType.Module;
            Allocator = MainModule.GetType(NameSpace, "NativeEnumerable`1").Methods.First(x => x.Name == "ToNativeArray").Parameters.First().ParameterType;
        }

        internal static void Main(string[] args)
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
            //OrderByOperatorHelper.Create(MainModule);
            //OrderByDefaultHelper.Create(MainModule);
            //AppendPrependDefaultIfEmptyHelper.Create(MainModule);
            //ReverseHelper.Create(MainModule);
            SkipTakeLastHelper.Create(MainModule);
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
    }
}
