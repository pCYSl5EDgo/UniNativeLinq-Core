using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CecilRewrite
{
    internal static class Helper
    {
        public static GenericInstanceType MakeGenericInstanceType(this TypeReference self, IEnumerable<TypeReference> arguments)
        {
            var instance = new GenericInstanceType(self);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);
            return instance;
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, IEnumerable<TypeReference> arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericInstanceType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };
            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            foreach (var genericParameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));
            return reference;
        }

        public static TypeDefinition ToDefinition(this TypeReference type)
        {
            return type is TypeDefinition definition ? definition : type is GenericInstanceType genericInstanceType ? genericInstanceType.ElementType.ToDefinition() : type.Resolve();
        }

        public static MethodReference FindMethod(this GenericInstanceType type, string name, Func<MethodDefinition, bool> predicate)
        {
            var methodDefinitions = type.ToDefinition().Methods;
            var methodDefinition = methodDefinitions.Single(x => x.Name == name && predicate(x));
            return methodDefinition.MakeHostInstanceGeneric(type.GenericArguments);
        }

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters, string specialName, TypeReference specialType)
        {
            var genericParameters = methodGenericParameters as GenericParameter[] ?? methodGenericParameters.ToArray();
            switch (constraint)
            {
                case GenericInstanceType genericConstraint:
                    {
                        var newConstraint = (GenericInstanceType)constraint.Module.ImportReference(new GenericInstanceType(constraint.Resolve()));
                        foreach (var argument in genericConstraint.GenericArguments)
                            newConstraint.GenericArguments.Add(argument.Replace(genericParameters, specialName, specialType));
                        return newConstraint;
                    }
                case GenericParameter genericParameter when genericParameter.Name == specialName:
                    return specialType;
                case GenericParameter genericParameter:
                    var singleOrDefault = genericParameters.SingleOrDefault(x => x.Name == genericParameter.Name);
                    switch (singleOrDefault)
                    {
                        case null:
                            return constraint;
                        default:
                            var constraints = singleOrDefault.Constraints;
                            for (var i = constraints.Count; --i >= 0;)
                            {
                                constraints[i] = constraints[i].Replace(genericParameters, specialName, specialType);
                            }
                            return singleOrDefault;
                    }
                default:
                    return constraint;
            }
        }

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters, string specialName, TypeReference specialType, string suffix)
        {
            switch (constraint)
            {
                case GenericInstanceType genericConstraint:
                    {
                        var newConstraint = (GenericInstanceType)constraint.Module.ImportReference(new GenericInstanceType(constraint.Resolve()));
                        foreach (var argument in genericConstraint.GenericArguments)
                            newConstraint.GenericArguments.Add(argument.Replace(methodGenericParameters, specialName, specialType, suffix));
                        return newConstraint;
                    }
                case GenericParameter genericParameter when genericParameter.Name == specialName:
                    return specialType;
                case GenericParameter genericParameter:
                    var singleOrDefault = methodGenericParameters.SingleOrDefault(x => x.Name == genericParameter.Name + suffix);
                    switch (singleOrDefault)
                    {
                        case null:
                            return constraint;
                        default:
                            var constraints = singleOrDefault.Constraints;
                            for (var i = constraints.Count; --i >= 0;)
                            {
                                constraints[i] = constraints[i].Replace(methodGenericParameters, specialName, specialType, suffix);
                            }
                            return singleOrDefault;
                    }
                default:
                    return constraint;
            }
        }

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters)
        {
            switch (constraint)
            {
                case GenericInstanceType genericConstraint:
                    {
                        var newConstraint = (GenericInstanceType)constraint.Module.ImportReference(new GenericInstanceType(constraint.Resolve()));
                        foreach (var argument in genericConstraint.GenericArguments)
                            newConstraint.GenericArguments.Add(argument.Replace(methodGenericParameters));
                        return newConstraint;
                    }
                case GenericParameter genericParameter:
                    var singleOrDefault = methodGenericParameters.SingleOrDefault(x => x.Name == genericParameter.Name);
                    switch (singleOrDefault)
                    {
                        case null:
                            return constraint;
                        default:
                            return singleOrDefault;
                    }
                default:
                    return constraint;
            }
        }

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters, string suffix)
        {
            switch (constraint)
            {
                case GenericInstanceType genericConstraint:
                    {
                        var newConstraint = (GenericInstanceType)constraint.Module.ImportReference(new GenericInstanceType(constraint.Resolve()));
                        foreach (var argument in genericConstraint.GenericArguments)
                            newConstraint.GenericArguments.Add(argument.Replace(methodGenericParameters, suffix));
                        return newConstraint;
                    }
                case GenericParameter genericParameter:
                    var singleOrDefault = methodGenericParameters.SingleOrDefault(x => x.Name == genericParameter.Name + suffix);
                    switch (singleOrDefault)
                    {
                        case null:
                            return constraint;
                        default:
                            var constraints = singleOrDefault.Constraints;
                            for (var i = constraints.Count; --i >= 0;)
                            {
                                constraints[i] = constraints[i].Replace(methodGenericParameters, suffix);
                            }
                            return singleOrDefault;
                    }
                default:
                    return constraint;
            }
        }

        public static void Call(this ILProcessor processor, MethodReference method)
            => processor.Append(Instruction.Create(OpCodes.Call, method));

        public static void GetEnumerator(this ILProcessor processor, GenericInstanceType @this)
            => processor.Call(@this.FindMethod(nameof(GetEnumerator), NoParameter));
        
        public static bool NoParameter(this MethodDefinition method)
            => !method.HasParameters;
    }
}