﻿using System;
using System.Linq;
using System.Reflection;

namespace CMSSolutions.Reflection
{
    public static class ObjectExtensions
    {
        public static object GetPropertyValue<T>(this T item, string propertyName)
        {
            PropertyInfo property = item.GetType().GetProperty(propertyName);
            return GetPropertyValue(item, property);
        }

        public static object GetPropertyValue<T>(this T item, PropertyInfo property)
        {
            return property.GetValue(item, null);
        }

        public static bool HasProperty<T>(this T item, string propertyName)
        {
            return typeof(T).GetProperties().SingleOrDefault(p => p.Name.Equals(propertyName)) != null;
        }

        public static TOut InvokeMethod<TIn, TOut>(this TIn item, string methodName, params object[] parameters)
        {
            Type[] types = new Type[parameters.Length];

            for (int i = 0; i < types.Length; i++)
            {
                types[i] = parameters[i].GetType();
            }

            MethodInfo methodInfo = typeof(TIn).GetMethod(methodName, types);
            object value = methodInfo.Invoke(item, parameters);
            return (value is TOut ? (TOut)value : default(TOut));
        }

        public static TOut InvokeExtensionMethod<TIn, TOut>(this TIn item, Assembly extensionsAssembly, string methodName, params object[] parameters)
        {
            object[] newParameters = new object[parameters.Length + 1];
            newParameters[0] = item;

            for (byte b = 0; b < parameters.Length; b++)
            {
                newParameters[b + 1] = parameters[b];
            }

            Type[] types = new Type[parameters.Length];

            for (int i = 0; i < types.Length; i++)
            {
                types[i] = parameters[i].GetType();
            }

            MethodInfo methodInfo = typeof(TIn).GetExtensionMethod(extensionsAssembly, methodName, types);
            object value = methodInfo.Invoke(item, newParameters);
            return (value is TOut ? (TOut)value : default(TOut));
        }

        public static void SetPropertyValue<T>(this T item, string propertyName, object value)
        {
            SetPropertyValue(item, item.GetType().GetProperty(propertyName), value);
        }

        public static void SetPropertyValue<T>(this T item, PropertyInfo property, object value)
        {
            Type propertyType = property.PropertyType;

            if (propertyType == typeof(Boolean))
            {
                property.SetValue(item, Boolean.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Byte))
            {
                property.SetValue(item, Byte.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Char))
            {
                property.SetValue(item, Char.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Int16))
            {
                property.SetValue(item, Int16.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Int32))
            {
                property.SetValue(item, Int32.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Int64))
            {
                property.SetValue(item, Int64.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Decimal))
            {
                property.SetValue(item, Decimal.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Double))
            {
                property.SetValue(item, Double.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(DateTime))
            {
                property.SetValue(item, DateTime.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Guid))
            {
                property.SetValue(item, new Guid(value.ToString()), null);
            }
            else if (propertyType == typeof(Single))
            {
                property.SetValue(item, Single.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(String))
            {
                property.SetValue(item, value.ToString(), null);
            }
            else if (propertyType == typeof(SByte))
            {
                property.SetValue(item, SByte.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(TimeSpan))
            {
                property.SetValue(item, TimeSpan.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(UInt16))
            {
                property.SetValue(item, UInt16.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(UInt32))
            {
                property.SetValue(item, UInt32.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(UInt64))
            {
                property.SetValue(item, UInt64.Parse(value.ToString()), null);
            }
            else if (propertyType == typeof(Uri))
            {
                property.SetValue(item, new Uri(value.ToString()), null);
            }
            else
            {
                property.SetValue(item, value, null);
            }
        }

        /// <summary>
        /// <para>Returns a private Property Value from the specified Object.</para>
        /// <para>Throws an ArgumentOutOfRangeException if the Property is not found.</para>
        /// </summary>
        /// <typeparam name="T">The type of System.Object.</typeparam>
        /// <param name="item">The System.Object.</param>
        /// <param name="propertyName">The name of the property to get the value for.</param>
        /// <returns>The value of the specified property from the specified System.Object.</returns>
        public static object GetPrivatePropertyValue<T>(this T item, string propertyName)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            PropertyInfo property = typeof(T).GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (property == null)
            {
                throw new ArgumentOutOfRangeException(
                   "propertyName",
                   string.Format("Property {0} was not found in Type {1}", propertyName, typeof(T).FullName));
            }
            return property.GetValue(item, null);
        }

        /// <summary>
        /// <para>Returns a private Field Value from the specified Object.</para>
        /// <para>Throws an ArgumentOutOfRangeException if the Field is not found.</para>
        /// </summary>
        /// <typeparam name="T">The type of System.Object.</typeparam>
        /// <param name="item">The System.Object.</param>
        /// <param name="fieldName">The name of the field to get the value for.</param>
        /// <returns>The value of the specified field from the specified System.Object.</returns>
        public static object GetPrivateFieldValue<T>(this T item, string fieldName)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Type type = typeof(T);
            FieldInfo fieldInfo = null;
            while (fieldInfo == null && type != null)
            {
                fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                type = type.BaseType;
            }
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException(
                    "fieldName",
                    string.Format("Field {0} was not found in Type {1}", fieldName, typeof(T).FullName));
            }
            return fieldInfo.GetValue(item);
        }

        /// <summary>
        /// <para>Sets the value for the specified private Property on the specified Object.</para>
        /// <para>Throws an ArgumentOutOfRangeException if the Property is not found.</para>
        /// </summary>
        /// <typeparam name="T">The type of System.Object.</typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="item">The System.Object.</param>
        /// <param name="propertyName">The name of the property to set the value for.</param>
        /// <param name="value">The value to set for the specified property on the specified System.Object.</param>
        public static void SetPrivatePropertyValue<T, TValue>(this T item, string propertyName, TValue value)
        {
            Type type = typeof(T);
            if (type.GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            {
                throw new ArgumentOutOfRangeException(
                    "propertyName",
                    string.Format("Property {0} was not found in Type {1}", propertyName, typeof(T).FullName));
            }
            type.InvokeMember(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance,
                null, item,
                new object[] { value });
        }

        /// <summary>
        /// <para>Sets the value for the specified private Field on the specified Object.</para>
        /// <para>Throws an ArgumentOutOfRangeException if the Field is not found.</para>
        /// </summary>
        /// <typeparam name="T">The type of System.Object.</typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="item">The System.Object.</param>
        /// <param name="fieldName">The name of the field to set the value for.</param>
        /// <param name="value">The value to set for the specified field on the specified System.Object.</param>
        public static void SetPrivateFieldValue<T, TValue>(this T item, string fieldName, TValue value)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            Type type = typeof(T);
            FieldInfo fieldInfo = null;
            while (fieldInfo == null && type != null)
            {
                fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                type = type.BaseType;
            }
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException(
                   "fieldName",
                   string.Format("Field {0} was not found in Type {1}", fieldName, item.GetType().FullName));
            }
            fieldInfo.SetValue(item, value);
        }
    }
}