using OneOf;

namespace DataProvider.Models;

/// <summary>
///     This is the request result class.
/// </summary>
/// <typeparam name="T">Type of the result on successful execution</typeparam>
[GenerateOneOf]
public partial class RequestResult<T> : OneOfBase<T, Exception>
{
    public bool IsSuccessful => IsT0;
    public bool IsFailed => IsT1;
    public Exception Error => AsT1;

    /// <summary>
    ///     This is the actual value of the request result.
    /// </summary>
    public T ActualValue => AsT0;

    /// <summary>
    ///     Throws the exception if the request result is failed.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void EnsureSuccess()
    {
        if (IsFailed) throw Error;
    }
    
    
    
}