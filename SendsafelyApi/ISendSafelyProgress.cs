namespace SendsafelyApi
{
    /// <summary>
    /// Implement this interface to receive progress on how the encryption/signature/upload is progressing. 
    /// The double will be between 0 and 100.
    /// </summary>
    public interface ISendSafelyProgress
    {
        void UpdateProgress(string prefix, double progress);
    }
}
