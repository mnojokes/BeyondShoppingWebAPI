using BeyondShopping.Contracts.Objects;
using BeyondShopping.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BeyondShopping.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _userDataRepositoryAddress;

    public UserRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;

        string userDataRepoSection = "UserDataRepositoryAddress";
        _userDataRepositoryAddress = configuration[userDataRepoSection] ??
            throw new ArgumentNullException(userDataRepoSection);
    }

    public async Task<UserData?> Get(int id)
    {
        HttpClient client = _httpClientFactory.CreateClient("ClientWithExponentialBackoff");
        var response = await client.GetAsync($"{_userDataRepositoryAddress}/{id}");

        UserData? user = null;
        switch (response.IsSuccessStatusCode)
        {
            case true:
                user = JsonConvert.DeserializeObject<UserData>(await response.Content.ReadAsStringAsync());
                break;
            case false when response.StatusCode == System.Net.HttpStatusCode.NotFound:
                break;
            case false:
                response.EnsureSuccessStatusCode();
                break;
        }

        return user;
    }
}
