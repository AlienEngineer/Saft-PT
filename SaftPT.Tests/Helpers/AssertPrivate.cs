using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SaftPT.Tests.Helpers
{
    /// <summary>
    /// Assert private members.
    /// </summary>
    public static class AssertPrivate
    {
        private static readonly IDictionary<string, FieldInfo> FieldDictionary;

        static AssertPrivate()
        {
            FieldDictionary = new Dictionary<string, FieldInfo>();
        }

        private static FieldInfo GetField(string fieldName, Type objectType)
        {
            FieldInfo fieldInfo;

            if (FieldDictionary.TryGetValue(fieldName + objectType.FullName, out fieldInfo))
            {
                return fieldInfo;
            }

            fieldInfo = objectType.GetField(fieldName,BindingFlags.NonPublic | BindingFlags.Instance);
            FieldDictionary[fieldName + objectType.FullName] = fieldInfo;

            return fieldInfo;
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="subject">The subject.</param>
        /// <returns></returns>
        /// <exception cref="FieldNotFoundException"></exception>
        public static object GetFieldValue<T>(string fieldName, T subject)
        {
            var type = typeof(T);
            var field = GetField(fieldName, type);

            if (field == null)
            {
                throw new FieldNotFoundException(
                    $"The Field {fieldName} is not an instance private field on {type.Name}"
                );
            }

            return field.GetValue(subject);
        }

        public static void That<T>(T subject, string fieldName, IResolveConstraint constraint)
        {
            Assert.That(GetFieldValue(fieldName, subject), constraint);
        }
    }
}