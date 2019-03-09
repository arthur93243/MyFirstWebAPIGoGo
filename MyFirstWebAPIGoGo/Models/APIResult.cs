using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFirstWebAPIGoGo.Models
{
    /// <summary>
    /// API呼叫時，傳回的物件
    /// </summary>
    public class APIResult<T>
    {
        /// <summary>
        /// 執行成功與否
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 結果編碼 
        /// (預設 0000=成功，其餘為錯誤編碼)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 資料時間
        /// </summary>
        public DateTime DataTime { get; set; }

        /// <summary>
        /// 資料本體
        /// </summary>
        public T Data { get; set; }

        /*********************************************************************/
        //建構子
        public APIResult() { }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        /// <param name="data"></param>
        public APIResult(T data)
        {
            Code = "0000";
            Success = true;
            DataTime = DateTime.Now;
            Data = data;
        }
    }

    /// <summary>
    /// API呼叫時，失敗傳回物件。
    /// </summary>
    public class ApiError : APIResult<object>
    {
        /// <summary>
        /// 建立失敗結果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ApiError(ErrorCode code, string message)
        {
            Code = ErrorCodeConvert(code);
            Success = false;
            this.DataTime = DateTime.Now;
            Message = ErrorCodeConvertMessage(code) + " - " + message;
        }

        /// <summary>
        /// 錯誤編碼枚舉
        /// </summary>
        public enum ErrorCode
        {
            Err1000,
            Err2000,
            Err3000,
            Err4000
        }

        /// <summary>
        /// 錯誤編碼訊息
        /// </summary>
        public struct ErrorMessage
        {
            public static string Err1000 = "";
            public static string Err2000 = "";
            public static string Err3000 = "";
            public static string Err4000 = "";
        }

        /// <summary>
        /// 錯誤編碼轉換訊息方法
        /// </summary>
        public string ErrorCodeConvertMessage(ErrorCode code)
        {
            switch (code)
            {
                case ErrorCode.Err1000:
                    return ErrorMessage.Err1000;
                case ErrorCode.Err2000:
                    return ErrorMessage.Err2000;
                case ErrorCode.Err3000:
                    return ErrorMessage.Err3000;
                case ErrorCode.Err4000:
                    return ErrorMessage.Err4000;
                default:
                    return "";
            }
        }

        /// <summary>
        /// 錯誤編碼轉換字串方法
        /// </summary>
        public string ErrorCodeConvert(ErrorCode code)
        {
            switch (code)
            {
                case ErrorCode.Err1000:
                    return "1000";
                case ErrorCode.Err2000:
                    return "2000";
                case ErrorCode.Err3000:
                    return "3000";
                case ErrorCode.Err4000:
                    return "4000";
                default:
                    return "";
            }
        }
    }
}