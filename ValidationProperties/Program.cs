using System.Reflection;

namespace ValidationProperties
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person
            {
                Name = "John",
                LastName = "Doe",
                Email = "john@doe.com",
                Phone = "123456789",
                Address = "123 Main St."
            };
            person = new Person
            {
                Name = "",
                LastName = "",
                Email = "",
                Phone = "",
                Address = ""
            };
            Console.Write(person.AreAllPropertiesNullOrEmpty());
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public static class ObjectExtensions
    {
        public static bool AreAllPropertiesNullOrEmpty<T>(this T obj)
        {
            var result = true;
            if (obj == null)
                return result;
            var props = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var t in props)
            {
                if (HasValue(t, obj))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private static bool HasValue(PropertyInfo prop, object obj)
        {
            var value = prop.GetValue(obj);
            if (value == null)
                return false;
            if (value is string str)
                return !string.IsNullOrEmpty(str.Trim());
            if (value is int i)
                return i != 0;
            return false; // si es un tipo distinto se asume que no tiene valor
        }
    }
}