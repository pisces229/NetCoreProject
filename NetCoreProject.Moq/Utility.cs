using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NetCoreProject.Moq
{
    public class Utility
    {
        public static IConfigurationRoot CreateConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        public static void PrintSqlString(string value)
            => Console.WriteLine(new Regex("[ ]{2,}", RegexOptions.None).Replace(value.ToString().Replace(Environment.NewLine, " "), " ").Trim());
        public static void PrintDynamicParameters(DynamicParameters parameter)
        {
            if (parameter != null)
            {
                var parameterNames = parameter.ParameterNames.ToList();
                if (parameterNames.Any())
                {
                    parameterNames.ForEach(name =>
                    {
                        var dynamicParameter = parameter.Get<object>(name);
                        if (dynamicParameter != null)
                        {
                            var dynamicParameterType = dynamicParameter.GetType();
                            if (dynamicParameter is Array)
                            {
                                Console.WriteLine($"[{ name }]:[{ GetArrayString(dynamicParameter as Array) }]");
                            }
                            else if (dynamicParameter is IList)
                            {
                                Console.WriteLine($"[{ name }]:[{ GetListString(dynamicParameter as IList) }]");
                            }
                            else
                            {
                                Console.WriteLine($"[{ name }]:[{ dynamicParameter }]");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[{ name }]:[NULL]");
                        }
                    });
                }
            }
        }
        private static string GetArrayString(Array values)
        {
            var result = new StringBuilder();
            foreach (var value in values)
            {
                if (result.Length > 0)
                {
                    result.Append(",");
                }
                result.Append(value.ToString());
            }
            return result.ToString();
        }
        private static string GetListString(IList values)
        {
            var result = new StringBuilder();
            foreach (var value in values)
            {
                if (result.Length > 0)
                {
                    result.Append(",");
                }
                result.Append(value != null ? value.ToString() : "null");
            }
            return result.ToString();
        }
    }
}
