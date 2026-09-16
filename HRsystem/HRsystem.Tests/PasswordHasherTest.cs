namespace HRsystem.Tests;

public class PasswordHasherTest
{
    [Fact]
    public void HashPassword_ReturnsAHashThatIsNotThePlainTextPassword()
    {
        const string password = "MySecretPassword123!";
        string hash = PasswordHasher.HashPassword(password);
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void VerifyPassword_ReturnsTrueForTheOriginalPassword()
    {
        const string password = "MySecretPassword123!";
        string storedHash = PasswordHasher.HashPassword(password);

        bool result = PasswordHasher.VerifyPassword(password, storedHash);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseForTheWrongPassword()
    {
        string storedHash = PasswordHasher.HashPassword("MySecretPassword123!");

        bool result = PasswordHasher.VerifyPassword("WrongPassword", storedHash);

        Assert.False(result);
    }
}
