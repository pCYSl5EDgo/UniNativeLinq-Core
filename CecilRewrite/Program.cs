#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CecilRewriteAndAddExtensions
{
    static class Program
    {
        private const string NameSpace = "UniNativeLinq";

        private const TypeAttributes StaticExtensionClassTypeAttributes = TypeAttributes.AnsiClass | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Abstract;
        private const MethodAttributes StaticMethodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        private static ModuleDefinition MainModule;
        private static ModuleDefinition UnsafeModule;
        private static CustomAttribute ExtensionAttribute;
        private static CustomAttribute IsReadOnlyAttribute;
        private static ModuleDefinition UnityModule;

        private static void Main(string[] args)
        {
            var location = typeof(UniNativeLinq.Enumerable).Assembly.Location;
            Console.WriteLine(location);
            using var asm = AssemblyDefinition.ReadAssembly(location);
            MainModule = asm.MainModule;
            InitializeModule(MainModule);
            RewriteThrow(MainModule);
            MinMaxHelper(MainModule);
            asm.Write(@"C:\Users\conve\source\repos\pcysl5edgo\UniNativeLinq\bin\Release\UniNativeLinq.dll");
            var s = Console.ReadLine();
        }

        private static void InitializeModule(ModuleDefinition module)
        {
            var nativeEnumerable = module.Types.First(x => x.Name == "NativeEnumerable");
            ExtensionAttribute = nativeEnumerable.CustomAttributes.Single();
            var negateMethodDefinition = module.Types.First(x => x.Name == "NegatePredicate`2").GetConstructors().First();
            IsReadOnlyAttribute = negateMethodDefinition.Parameters.First().CustomAttributes.First();
            UnsafeModule = ExtensionAttribute.AttributeType.Module;
            UnityModule = nativeEnumerable.Methods.First(x => x.Parameters.First().ParameterType.IsValueType).Parameters.First().ParameterType.Module;
        }

        private static void MinMaxHelper(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(MinMaxHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                if (type.Name == "GroupByEnumerable`8") continue;
                @static.TryGetMin(type);
            }
        }

        private static void TryGetMin(this TypeDefinition @static, TypeDefinition type)
        {
            var instanceType = new GenericInstanceType(type);
            var systemByte = MainModule.TypeSystem.Byte;
            foreach (var parameter in type.GenericParameters)
            {
                if (parameter.Name == "T")
                {
                    instanceType.GenericArguments.Add(systemByte);
                    Console.WriteLine("\t" + instanceType.GenericArguments[instanceType.GenericArguments.Count - 1]);
                    continue;
                }
                if (!parameter.HasConstraints)
                {
                    instanceType.GenericArguments.Add(parameter);
                    Console.WriteLine("\t" + instanceType.GenericArguments[instanceType.GenericArguments.Count - 1]);
                    continue;
                }
                foreach (var constraint in parameter.Constraints.Select(x => x as GenericInstanceType))
                {
                    if (constraint is null) continue;
                    foreach (var (genericArgument, index) in constraint.GenericArguments.Select((genericArgument, index) => (genericArgument, index)))
                    {
                        if (genericArgument.Name == "T")
                            constraint.GenericArguments[index] = systemByte;
                        Console.WriteLine("\t\t" + constraint.GenericArguments[index]);
                    }
                }
                instanceType.GenericArguments.Add(parameter);
                Console.WriteLine("\t" + instanceType.GenericArguments[instanceType.GenericArguments.Count - 1].Name);
            }
            Console.WriteLine(instanceType.Name);
            var method = new MethodDefinition("TryGetMin", StaticMethodAttributes, MainModule.TypeSystem.Boolean);
            method.CustomAttributes.Add(ExtensionAttribute);
            var firstParameterDefinition = new ParameterDefinition("@this", ParameterAttributes.In, instanceType.MakeByReferenceType());
            firstParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(firstParameterDefinition);
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, MainModule.TypeSystem.Byte.MakeByReferenceType()));
            foreach (var argument in instanceType.GenericArguments)
            {
                if (argument.IsGenericParameter)
                    method.GenericParameters.Add(new GenericParameter(argument));
            }
            @static.Methods.Add(method);
        }

        private static void RewriteThrow(ModuleDefinition module)
        {
            var enumeratorTypes = module.GetTypes()
                .Where(x => x.IsPublic && x.IsValueType && x.HasNestedTypes)
                .SelectMany(x => x.NestedTypes)
                .Where(x => x.HasCustomAttributes && x.CustomAttributes.Any(y => y.AttributeType.Name == "LocalRefReturnAttribute"));
            Console.WriteLine("rewrite 'throw new NotImplemented' to return ref element;");
            foreach (var enumeratorType in enumeratorTypes)
            {
                //Console.WriteLine(enumeratorType.FullName);
                ReWriteRefReturn(module, enumeratorType);
            }
        }

        private static void ReWriteRefReturn(ModuleDefinition module, TypeDefinition type)
        {
            var element = type.Fields.First(x => x.Name == "element").FillGenericParams();
            ReWrite(type.Methods.First(x => x.Name == "TryGetNext"), element);
            static bool Predicate(PropertyDefinition x)
                => x.Name == "Current" && x.PropertyType.IsByReference;
            ReWrite(type.Properties.First(Predicate).GetMethod, element);
        }

        private static FieldReference FillGenericParams(this FieldDefinition definition)
        {
            var declaringType = new GenericInstanceType(definition.DeclaringType);
            foreach (var param in definition.DeclaringType.GenericParameters)
                declaringType.GenericArguments.Add(param);
            return new FieldReference(definition.Name, definition.FieldType, declaringType);
        }

        private static void ReWrite(MethodDefinition method, FieldReference field)
        {
            var body = method.Body;
            var processor = body.GetILProcessor();
            var ldflda = processor.Create(OpCodes.Ldflda, field);
            var ret = processor.Create(OpCodes.Ret);
            var ldarg0 = processor.Create(OpCodes.Ldarg_0);
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
