
namespace NetCoreProject.Domain.Enum
{
    public enum OperatorEnum : int
    {
        /// <summary>
        /// " = "
        /// </summary>
        EQUAL = 1,
        /// <summary>
        /// " &lt;&gt; "
        /// </summary>
        NOT_EQUAL = 2,
        /// <summary>
        /// " &lt; "
        /// </summary>
        LESS_THAN = 3,
        /// <summary>
        /// " &gt; "
        /// </summary>
        GREATER_THAN = 4,
        /// <summary>
        /// " &lt;= "
        /// </summary>
        LESS_THAN_EQUAL = 5,
        /// <summary>
        /// " &gt;= "
        /// </summary>
        GREATER_THAN_EQUAL = 6,
        /// <summary>
        /// " %like% "
        /// </summary>
        LIKE_START_END = 11,
        /// <summary>
        /// " %like "
        /// </summary>
        LIKE_START = 12,
        /// <summary>
        /// " like% "
        /// </summary>
        LIKE_END = 13,
        /// <summary>
        /// " like "
        /// </summary>
        LIKE = 14,
        /// <summary>
        /// " not %like% "
        /// </summary>
        NOT_LIKE_START_END = 21,
        /// <summary>
        /// " not %like "
        /// </summary>
        NOT_LIKE_START = 22,
        /// <summary>
        /// " not like% "
        /// </summary>
        NOT_LIKE_END = 23,
        /// <summary>
        /// " not like "
        /// </summary>
        NOT_LIKE = 24,
        /// <summary>
        /// " in "
        /// </summary>
        IN = 31,
        /// <summary>
        /// " not in "
        /// </summary>
        NOT_IN = 32
    }
}
