// Decompiled with JetBrains decompiler
// Type: CharacterEditor.Reflect
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace CharacterEditor;

internal static class Reflect
{
    internal static string APP_VERISON_AND_DATE
    {
        get
        {
            var stringBuilder = new StringBuilder();
            var executingAssembly = Assembly.GetExecutingAssembly();
            var customAttribute = executingAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            var version = executingAssembly.GetName().Version;
            stringBuilder.Append("v");
            stringBuilder.Append(version.Major.ToString());
            stringBuilder.Append(".");
            stringBuilder.Append(version.Minor.ToString());
            stringBuilder.Append(".");
            stringBuilder.Append(version.Build.ToString());
            stringBuilder.Append(" " + customAttribute.Description);
            return stringBuilder.ToString();
        }
    }

    internal static int VERSION_BUILD => Assembly.GetExecutingAssembly().GetName().Version.Build;

    internal static string APP_NAME_AND_VERISON
    {
        get
        {
            var stringBuilder = new StringBuilder();
            var executingAssembly = Assembly.GetExecutingAssembly();
            executingAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            var version = executingAssembly.GetName().Version;
            stringBuilder.Append(APP_ATTRIBUTE_TITLE);
            stringBuilder.Append(" v");
            stringBuilder.Append(version.Major.ToString());
            stringBuilder.Append(".");
            stringBuilder.Append(version.Minor.ToString());
            stringBuilder.Append(".");
            stringBuilder.Append(version.Build.ToString());
            return stringBuilder.ToString();
        }
    }

    internal static string APP_ATTRIBUTE_TITLE => ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;

    internal static Type ByName(string name)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
        {
            var type = assembly.GetType(name);
            if (type != null)
                return type;
        }

        return null;
    }

    internal static object GetMemberValue(this Type type, string name)
    {
        var bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        return type?.GetField(name, bindingAttr)?.GetValue(null);
    }

    internal static void SetMemberValue(this Type type, string name, object value)
    {
        var bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        type?.GetField(name, bindingAttr)?.SetValue(null, value);
    }

    internal static object GetMemberConstValue(this object obj, string name)
    {
        return obj?.GetType().GetField(name)?.GetValue(null);
    }

    internal static T GetMemberValue<T>(this object obj, string name, T fallback)
    {
        var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var obj1 = obj?.GetType().GetField(name, bindingAttr)?.GetValue(obj);
        return obj1 != null ? (T)obj1 : fallback;
    }

    internal static string GetMemberValueAsString<T>(this object obj, string name, string fallback)
    {
        var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var obj1 = obj?.GetType().GetField(name, bindingAttr)?.GetValue(obj);
        return obj1 != null ? ((T)obj1).ToString() : fallback;
    }

    [Obsolete]
    internal static object GetMemberValueOld(this object obj, string name)
    {
        var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        return obj?.GetType().GetField(name, bindingAttr)?.GetValue(obj);
    }

    internal static void SetMemberValue(this object obj, string name, object value)
    {
        var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        obj?.GetType().GetField(name, bindingAttr)?.SetValue(obj, value);
    }

    internal static object CallMethod(this object obj, string name, object[] param)
    {
        var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        return obj?.GetType().GetMethod(name, bindingAttr)?.Invoke(obj, param);
    }

    internal static object CallMethodAmbiguous(this object obj, string name, object[] param)
    {
        var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (var method in obj?.GetType().GetMethods(bindingAttr))
            if (method.Name == name && method.GetParameters().Length == param.Length)
                return method?.Invoke(obj, param);
        return null;
    }

    internal static object CallMethod(this Type type, string name, object[] param)
    {
        var bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        return type?.GetMethod(name, bindingAttr)?.Invoke(null, param);
    }

    internal static object CallMethod(this MethodInfo mi, object[] param, object instance = null)
    {
        return mi?.Invoke(instance, param);
    }

    internal static MethodInfo GetMethodInfo(this Type type, string name)
    {
        var bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        return type?.GetMethod(name, bindingAttr);
    }

    internal static MethodInfo GetMethodInfo(this object obj, string name)
    {
        var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        return obj?.GetType().GetMethod(name, bindingAttr);
    }

    internal static Type GetAssemblyType(string name, string type)
    {
        return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name).GetType(type);
    }

    internal static Type GetAType(string nameSpace, string className)
    {
        return GenTypes.GetTypeInAnyAssembly(nameSpace + "." + className, nameSpace);
    }
}