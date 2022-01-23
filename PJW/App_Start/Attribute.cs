﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PJW.Web.Admin.Controllers;
using DbModel;
using Newtonsoft.Json;

namespace PJW.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class APIAttribute : Attribute
    {
        public APIAttribute()
        {
            Name = string.Empty;
        }
        public APIAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// API名稱
        /// </summary>
        public string Name { get; set; }
    }


    internal class APIMetaData
    {
        public string ClassName { get; set; }
        public Type Type { get; set; }
        public APIAttribute Attribute { get; set; }
    }

    public class APIAttibuteHelper
    {

        private static ConcurrentDictionary<Assembly, List<APIMetaData>> _cache = new ConcurrentDictionary<Assembly, List<APIMetaData>>();

        //獲取目前的程式集裡面所有繼承了BaseController的類
        //獲取目前class上打了API標籤的屬性 + className
        //在獲取目前class裡面的所有打了API標籤的method + methodName
        public static List<APIDescription> GetAllDescriptions(Assembly current = null, List<SystemPageAction> pageActions = null)
        {
            if (current == null) current = typeof(APIAttibuteHelper).Assembly;
            var result = new List<APIDescription>();
            if (!_cache.TryGetValue(current, out var maps))
            {
                var types = current.GetExportedTypes();
                maps = (from t in types
                        where t.IsClass && t.BaseType == typeof(BaseController) &&
                              !t.IsAbstract && !t.IsInterface
                        select new APIMetaData
                        {
                            ClassName = t.Name,
                            Type = t,
                            Attribute = t.GetCustomAttribute<APIAttribute>()
                        }).ToList();
                _cache.TryAdd(current, maps);
            }
            var div = new Dictionary<string, List<string>>();
            if (pageActions != null)
            {
                div = pageActions.GroupBy(r => r.ControlName)
                    .ToDictionary(r => r.Key, y => y.Select(r => r.ActionName).ToList());
            }
            foreach (var item in maps)
            {
                APIDescription classDescription = new APIDescription
                {
                    APIName = item.Attribute != null && !string.IsNullOrEmpty(item.Attribute.Name) ? item.Attribute.Name : string.Empty,
                    Name = item.ClassName,
                    MethodList = new List<APIDescription>()
                };
                var methodInfos = item.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(r => new
                {
                    MethodName = r.Name,
                    Attribute = r.GetCustomAttribute<APIAttribute>()
                }).Where(r => r.Attribute != null).ToList();

                var divAction = new List<string>();
                div.TryGetValue(item.ClassName, out divAction);
                foreach (var method in methodInfos)
                {
                    classDescription.MethodList.Add(new APIDescription
                    {
                        APIName = !string.IsNullOrEmpty(method.Attribute.Name) ? method.Attribute.Name : string.Empty,
                        Name = method.MethodName,
                        ParentName = item.ClassName,
                        Check = divAction != null && divAction.Contains(method.MethodName)
                    });
                }

                if (classDescription.MethodList.Count > 0)
                {
                    classDescription.MethodList = classDescription.MethodList.OrderBy(r => r.Name).ToList();
                    result.Add(classDescription);
                }
            }

            result = result.OrderBy(r => r.Name).ToList();

            return result;
        }



    }

    public class APIDescription
    {

        public string Name { get; set; }

        [JsonProperty("name")]
        public string DisplayName => this.APIName + "[" + this.Name + "]";

        public int id { get; set; }
        public int pid { get; set; }
        public string APIName { get; set; }
        public string Action { get; set; }
        public string ParentName { get; set; }

        [JsonProperty("checked")]
        public bool Check { get; set; }

        [JsonProperty("children")]
        public List<APIDescription> MethodList { get; set; }
    }
}