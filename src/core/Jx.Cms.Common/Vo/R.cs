namespace Jx.Cms.Common.Vo
{
    /// <summary>
    /// API统一回复
    /// </summary>
    public class R
    {
        /// <summary>
        /// 返回码，成功为20000
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static R Success()
        {
            return new R() {Code = 20000, Data = new { }, Message = "处理成功"};
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">返回内容</param>
        /// <returns></returns>
        public static R Success(object data)
        {
            return new R() {Code = 20000, Data = data, Message = "处理成功"};
        }
        
        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message">消息描述</param>
        /// <param name="data">返回内容</param>
        /// <returns></returns>
        public static R Success(string message, object data)
        {
            return new R() {Code = 20000, Data = data, Message = message};
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="code">返回码</param>
        /// <returns></returns>
        public static R Fail(int code)
        {
            return new R() {Code = code, Data = new { }, Message = "处理失败"};
        }
        
        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="code">返回码</param>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static R Fail(int code, string message)
        {
            return new R() {Code = code, Data = new { }, Message = message};
        }
        
        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="code">返回码</param>
        /// <param name="message">错误信息</param>
        /// <param name="data">内容</param>
        /// <returns></returns>
        public static R Fail(int code, string message, object data)
        {
            return new R() {Code = code, Data = data, Message = message};
        }
    }
}