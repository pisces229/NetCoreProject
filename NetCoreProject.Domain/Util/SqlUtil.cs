using Dapper;
using NetCoreProject.Domain.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NetCoreProject.Domain.Util
{
    public class SqlUtil
    {
        public string CreateSelectCount(string sql)
        {
            return $"SELECT COUNT(1) FROM ({ sql }) PAGE";
        }
        public string CreateSelect<T>(List<string> select, string where, string orderby) where T : class
        {
            return $"SELECT { string.Join(",", select) } FROM { typeof(T).Name } { where } ORDER BY { orderby }";
        }
        public string CreateInsert<T>(DynamicParameters insertParameters) where T : class
        {
            DefaultUpdateInformation<T>(insertParameters);
            var insert = new List<string>();
            var values = new List<string>();
            insertParameters.ParameterNames.ToList().ForEach(f =>
            {
                insert.Add(f);
                values.Add($"@{ f }");
            });
            return $"INSERT INTO { typeof(T).Name } ({ string.Join(",", insert) }) VALUES ({ string.Join(",", values) })";
        }
        public string CreateUpdate<T>(DynamicParameters setParameters, string where) where T : class
        {
            DefaultUpdateInformation<T>(setParameters);
            var set = new List<string>();
            setParameters.ParameterNames.Where(w => !w.StartsWith("_")).ToList().ForEach(f =>
            {
                set.Add($"{ f } { GetOper(OperatorEnum.EQUAL) } @{ f }");
            });
            return $"UPDATE { typeof(T).Name } SET { string.Join(",", set) } { where } ";
        }
        public string CreateDelete<T>(string where) where T : class
        {
            return $"DELETE FROM { typeof(T).Name } { where } ";
        }
        private void DefaultUpdateInformation<T>(DynamicParameters parameters) where T : class
        {
            var propertyInfoArray = typeof(T).GetType().GetProperties();
            foreach (var propertyInfo in typeof(T).GetType().GetProperties())
            {
                if ("UPDATE_USER_ID".Equals(propertyInfo.Name))
                {
                    parameters.Add("UPDATE_USER_ID", "DEFAULT");
                }
                else if ("UPDATE_PROG_CD".Equals(propertyInfo.Name))
                {
                    parameters.Add("UPDATE_PROG_CD", "DEFAULT");
                }
                else if ("UPDATE_DATE_TIME".Equals(propertyInfo.Name))
                {
                    parameters.Add("UPDATE_DATE_TIME", DateTime.Now);
                }
            }
        }
        public void WhereAndCondition(StringBuilder sql, string condition)
        {
            sql.Append($" { (sql.Length == 0 ? "WHERE" : "AND") } { condition }");
        }
        public void WhereAndCondition(StringBuilder sql, DynamicParameters dynamicParameters, string condition, OperatorEnum oper, object value, DbType? dbType = null)
        {
            Condition(sql, dynamicParameters, $" { (sql.Length == 0 ? "WHERE" : "AND") } { condition }", oper, value, dbType);
        }
        public void WhereOrCondition(StringBuilder sql, string condition)
        {
            sql.Append($" { (sql.Length == 0 ? "WHERE" : "OR") } { condition }");
        }
        public void WhereOrCondition(StringBuilder sql, DynamicParameters dynamicParameters, string condition, OperatorEnum oper, object value, DbType? dbType = null)
        {
            Condition(sql, dynamicParameters, $" { (sql.Length == 0 ? "WHERE" : "OR") } { condition }", oper, value, dbType);
        }
        private void Condition(StringBuilder sql, DynamicParameters dynamicParameters, string condition, OperatorEnum oper, object value, DbType? dbType = null)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                var paramString = string.Empty;
                switch (oper)
                {
                    case OperatorEnum.EQUAL:
                    case OperatorEnum.NOT_EQUAL:
                    case OperatorEnum.LESS_THAN:
                    case OperatorEnum.GREATER_THAN:
                    case OperatorEnum.LESS_THAN_EQUAL:
                    case OperatorEnum.GREATER_THAN_EQUAL:
                        paramString = CreateParam(dynamicParameters, value, dbType);
                        break;
                    case OperatorEnum.LIKE_START_END:
                        paramString = CreateParam(dynamicParameters, "%" + value + "%", dbType);
                        break;
                    case OperatorEnum.LIKE_START:
                        paramString = CreateParam(dynamicParameters, "%" + value, dbType);
                        break;
                    case OperatorEnum.LIKE_END:
                        paramString = CreateParam(dynamicParameters, value + "%", dbType);
                        break;
                    case OperatorEnum.LIKE:
                        paramString = CreateParam(dynamicParameters, value, dbType);
                        break;
                    case OperatorEnum.NOT_LIKE_START_END:
                        paramString = CreateParam(dynamicParameters, "%" + value + "%", dbType);
                        break;
                    case OperatorEnum.NOT_LIKE_START:
                        paramString = CreateParam(dynamicParameters, "%" + value, dbType);
                        break;
                    case OperatorEnum.NOT_LIKE_END:
                        paramString = CreateParam(dynamicParameters, value + "%", dbType);
                        break;
                    case OperatorEnum.NOT_LIKE:
                        paramString = CreateParam(dynamicParameters, value, dbType);
                        break;
                    case OperatorEnum.IN:
                    case OperatorEnum.NOT_IN:
                        var hasCount = false;
                        if (value is Array)
                        {
                            hasCount = (value as Array).Length > 0;
                        }
                        else if (value is IList)
                        {
                            hasCount = (value as IList).Count > 0;
                        }
                        if (hasCount)
                        {
                            paramString = CreateParam(dynamicParameters, value, dbType);
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(paramString))
                {
                    sql.Append($"{ condition } {  GetOper(oper) } { paramString } ");
                }
            }
        }
        private string GetOper(OperatorEnum oper)
        {
            switch (oper)
            {
                case OperatorEnum.EQUAL:
                    return "=";
                case OperatorEnum.NOT_EQUAL:
                    return "<>";
                case OperatorEnum.LESS_THAN:
                    return "<";
                case OperatorEnum.GREATER_THAN:
                    return ">";
                case OperatorEnum.LESS_THAN_EQUAL:
                    return "<=";
                case OperatorEnum.GREATER_THAN_EQUAL:
                    return ">=";
                case OperatorEnum.LIKE_START_END:
                case OperatorEnum.LIKE_START:
                case OperatorEnum.LIKE_END:
                case OperatorEnum.LIKE:
                    return "LIKE";
                case OperatorEnum.NOT_LIKE_START_END:
                case OperatorEnum.NOT_LIKE_START:
                case OperatorEnum.NOT_LIKE_END:
                case OperatorEnum.NOT_LIKE:
                    return "NOT LIKE";
                case OperatorEnum.IN:
                    return "IN";
                case OperatorEnum.NOT_IN:
                    return "NOT IN";
                default:
                    return "";
            }
        }
        private string CreateParam(DynamicParameters dynamicParameters, object value, DbType? dbType = null)
        {
            var paramKey = $"_PARAM_{dynamicParameters.ParameterNames.Count()}";
            if (dbType == null)
            {
                dynamicParameters.Add(paramKey, value);
            }
            else
            {
                dynamicParameters.Add(paramKey, value, dbType.Value);
            }
            return "@" + paramKey;
        }
    }
}
