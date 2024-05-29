public interface IJwtProvider
{
    string Generate(string login, string guid);
}