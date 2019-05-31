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
        internal static readonly TypeDefinition SystemUnsafeType;
        internal static readonly CustomAttribute ExtensionAttribute;
        // ReSharper disable once InconsistentNaming
        internal static readonly CustomAttribute IsReadOnlyAttribute;
        internal static readonly ModuleDefinition UnityModule;
        internal static readonly AssemblyDefinition Assembly;

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
            SystemUnsafeType = MainModule.ImportReference(typeof(System.Runtime.CompilerServices.Unsafe)).Resolve();
            UnityModule = nativeEnumerable.Methods.First(x => x.Parameters.First().ParameterType.IsValueType).Parameters.First().ParameterType.Module;
        }

        internal static void Main(string[] args)
        {
            RewriteThrow(MainModule);
            TryGetMinHelper.Create(MainModule);
            TryGetMaxHelper.Create(MainModule);
            TryGetMinFuncHelper.Create(MainModule);
            TryGetMaxFuncHelper.Create(MainModule);
            TryGetMinOperatorHelper.Create(MainModule);
            TryGetMaxOperatorHelper.Create(MainModule);
            AnyOperatorHelper.Create(MainModule);
            AllOperatorHelper.Create(MainModule);
            AnyFuncHelper.Create(MainModule);
            AllFuncHelper.Create(MainModule);
            IsEmptyHelper.Create(MainModule);
            AggregateOperatorSmallHelper.Create(MainModule);
            AggregateFunctionSmallHelper.Create(MainModule);
            AggregateOperatorWithResultTypeHelper.Create(MainModule);
            AggregateFunctionWithResultTypeHelper.Create(MainModule);
            Assembly.Write(@"C:\Users\conve\source\repos\pcysl5edgo\UniNativeLinq\bin\Release\UniNativeLinq.dll");
        }

        internal static void RewriteThrow(ModuleDefinition module)
        {
            var enumeratorTypes = module.GetTypes()
                .Where(x => x.IsPublic && x.IsValueType && x.HasNestedTypes)
                .SelectMany(x => x.NestedTypes)
                .Where(x => x.HasCustomAttributes && x.CustomAttributes.Any(y => y.AttributeType.Name == "LocalRefReturnAttribute"));
            foreach (var enumeratorType in enumeratorTypes)
            {
                ReWriteRefReturn(module, enumeratorType);
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
