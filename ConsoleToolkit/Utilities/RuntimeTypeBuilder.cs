using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace ConsoleToolkit.Utilities
{
    /// <summary>
    /// Build .NET types from property sets, derived at runtime.
    /// </summary>
    internal class RunTimeTypeBuilder
    {
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>(); 
        
        public RunTimeTypeBuilder(string assemblyName)
        {
            _assemblyBuilder = Thread.GetDomain()
                              .DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName);
        }

        public Type MakeRuntimeType(IEnumerable<RuntimeTypeBuilderProperty> propertyTypes)
        {
            var properties = propertyTypes.ToList();
            var identity = MakeTypeIdentity(properties);
            Type existingType;
            if (_typeCache.TryGetValue(identity, out existingType))
                return existingType;

            var typeBuilder = _moduleBuilder.DefineType(string.Format("T{0}", _typeCache.Count));

            foreach (var property in properties)
            {
                MakeProperty(typeBuilder, property.Name, property.Type);
            }

            var type = typeBuilder.CreateType();
            _typeCache[identity] = type;
            return type;
        }

        private static void MakeProperty(TypeBuilder typeBuilder, string name, Type type)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + name, type, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, CallingConventions.HasThis, type,
                                                             null);
            var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            var getter = typeBuilder.DefineMethod("get_" + name, getSetAttr, type, Type.EmptyTypes);

            var getIL = getter.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            var setter = typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new[] {type});

            var setIL = setter.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getter);
            propertyBuilder.SetSetMethod(setter);
        }

        private string MakeTypeIdentity(IEnumerable<RuntimeTypeBuilderProperty> propertyTypes)
        {
            var sb = new StringBuilder();
            foreach (var propertyType in propertyTypes)
            {
                sb.AppendFormat("{0}|{1}0x1", propertyType.Name, propertyType.Type);
            }
            return sb.ToString();
        }
    }
}
