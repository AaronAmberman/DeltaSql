namespace DeltaSql.Services
{
    internal interface ICryptographyService
    {
        string Decrypt(string encrypted);
        string Encrypt(string uncrypted);
    }
}
