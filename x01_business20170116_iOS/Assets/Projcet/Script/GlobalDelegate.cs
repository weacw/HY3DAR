/******
创建人：NSWell
用途：全局委托
******/
using System.Collections;

public delegate bool OnWWWEndDelegate(byte[] bytes);
public delegate bool OnWWWEndWithPathDelegate(byte[] bytes);
public delegate void OnWWWUIHandler();
public delegate void OnWWWErrorHandler(string error_msg, SenderType sender);
public delegate void OnRequestOver(string data);

public delegate void OnBaseModulsOperatedHandler();
public delegate void OnBaseModulsOperationHandler();


public enum SenderType
{
    Text_Tips,
    Img_Tips,
    FLOAT_WINDOW
};

public class RequestErrorCode
{
    public const string _404ErrorCode = "404";
    public const string _NoFoundErrorCode = "No found";
}