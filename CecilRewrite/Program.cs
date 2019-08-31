using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    public static class Program
    {
        internal const string NameSpace = "UniNativeLinq";

        internal static ModuleDefinition MainModule;
        internal static CustomAttribute IsReadOnlyAttribute;
        internal static ModuleDefinition SystemModule;
        internal static AssemblyDefinition Assembly;
        internal static TypeReference Allocator;
        internal static TypeDefinition[] Enumerables;

        static Program()
        {
            const string folderPath = @"C:\Users\conve\source\repos\pcysl5edgo\UniNativeLinq\bin\Release\netstandard2.0\";
            const string UniNativeLinqDll = folderPath + @"UniNativeLinq.dll";
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(folderPath);
            Assembly = AssemblyDefinition.ReadAssembly(UniNativeLinqDll, new ReaderParameters(readingMode: ReadingMode.Deferred) { AssemblyResolver = resolver });
            MainModule = Assembly.MainModule;

            IsReadOnlyAttribute = MainModule.GetType("UniNativeLinq", "ZipValueTuple`2").CustomAttributes[2];

            var nativeEnumerable1 = MainModule.GetType(NameSpace, "NativeEnumerable`1");
            MethodDefinition ToNativeArray = nativeEnumerable1.Methods.First(x => x.Name == nameof(ToNativeArray));
            Allocator = ToNativeArray.Parameters.First().ParameterType;
            SystemModule = ModuleDefinition.ReadModule(@"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll");
            Enumerables = MainModule.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")).ToArray();
        }

        public static void Main()
        {
            RewriteThrow(MainModule);
            ReWritePseudoIsReadOnly(MainModule);
            ReWritePseudoUtility(MainModule);

            Assembly.Write(@"C:\Users\conve\source\repos\pcysl5edgo\UniNativeLinq\bin\Release\UniNativeLinq.dll");
        }

        private static void ReWritePseudoUtility(ModuleDefinition module)
        {
            foreach (var type in module.GetTypes()
                .Where(x => x.IsValueType))
            {
                foreach (var method in type.Methods)
                {
                    using (ScopedProcessor processor = method.Body.GetILProcessor())
                    {
                        var instructions = method.Body.Instructions;
                        for (var i = instructions.Count; --i >= 0;)
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
            }
            var pseudo = module.GetType(NameSpace, "Pseudo");
            module.Types.Remove(pseudo);
        }

        private static void ReWritePseudoIsReadOnly(this ParameterDefinition param)
        {
            var memberCustomAttributes = param.CustomAttributes;
            var pseudo = memberCustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "PseudoIsReadOnlyAttribute");
            if (pseudo is null) return;
            memberCustomAttributes.Remove(pseudo);
            memberCustomAttributes.Add(IsReadOnlyAttribute);
            param.Attributes = ParameterAttributes.In;
        }

        private static void ReWritePseudoIsReadOnly(ModuleDefinition module)
        {
            foreach (var type in module.GetTypes().Where(x => x.HasNestedTypes))
            {
                ReWritePseudoIsReadOnly(type);
            }
            var pseudo = MainModule.GetType(NameSpace, "PseudoIsReadOnlyAttribute");
            MainModule.Types.Remove(pseudo);
        }

        private static void ReWritePseudoIsReadOnly(TypeDefinition type)
        {
            foreach (var method in type.Methods)
            {
                foreach (var parameter in method.Parameters)
                {
                    parameter.ReWritePseudoIsReadOnly();
                }
            }
            foreach (var nestedType in type.NestedTypes)
                ReWritePseudoIsReadOnly(nestedType);
        }

        internal static void RewriteThrow(ModuleDefinition module)
        {
            const string local = "LocalRefReturnAttribute";
            var enumeratorTypes = module.GetTypes()
                .Where(x => x.IsValueType && x.HasNestedTypes)
                .SelectMany(x => x.NestedTypes)
                .Concat(module.GetTypes().Where(x => x.IsValueType));
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
            var firstOrDefault = type.Fields.FirstOrDefault(x => x.Name == "element");
            if (firstOrDefault is null) return;
            var element = firstOrDefault.FillGenericParams();
            foreach (var methodDefinition in type.Methods)
            {
                switch (methodDefinition.Name)
                {
                    case "TryGetNext":
                    case "get_Item":
                        ReWrite(methodDefinition, element);
                        break;
                }
            }
            static bool Predicate(PropertyDefinition x)
                => x.Name == "Current" && x.PropertyType.IsByReference;

            var method = type.Properties.FirstOrDefault(Predicate)?.GetMethod;
            if (method is null) return;
            ReWrite(method, element);
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
            using (ScopedProcessor processor = body.GetILProcessor())
            {
                for (var i = body.Instructions.Count; --i >= 0;)
                {
                    var throwInstruction = body.Instructions[i];
                    if (throwInstruction.OpCode != OpCodes.Throw) continue;
                    var newObjInstruction = throwInstruction.Previous;
                    if (newObjInstruction.OpCode != OpCodes.Newobj) continue;
                    var NotImplementedExceptionConstructor = ((MethodReference)newObjInstruction.Operand);
                    Console.WriteLine(NotImplementedExceptionConstructor.DeclaringType.FullName);
                    if (NotImplementedExceptionConstructor.DeclaringType.FullName != "System.NotImplementedException") continue;

                    var ldarg0 = Instruction.Create(OpCodes.Ldarg_0);
                    var ldflda = Instruction.Create(OpCodes.Ldflda, field);
                    var ret = Instruction.Create(OpCodes.Ret);

                    processor
                        .Remove(newObjInstruction)
                        .Replace(throwInstruction, ldarg0)
                        .InsertAfter(ldarg0, ldflda)
                        .InsertAfter(ldflda, ret);
                }
            }
        }
    }
}
