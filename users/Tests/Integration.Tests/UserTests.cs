using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Users.Api.Application.Commands.Create;
using Users.Api.Application.Commands.Update;
using Users.Api.Application.Commands.UpdateAddress;
using Users.Api.Application.Queries.Dto;
using Users.Domain.Aggregates.User;
using Xunit;

namespace Integration.Tests;

public class UserTests(UsersWebApplicationFactory factory)
    : IClassFixture<UsersWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _configClient = new();

    [Fact]
    public async Task Get_Should_ReturnUnauthorized_WhenTokenIsNotProvided()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();
        
        // Act
        var response = await client.GetAsync("users", tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_ReturnForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetTestUserAccessToken();
        SetBearerToken(client, token);
        
        // Act
        var response = await client.GetAsync("users", tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_ReturnOk_WhenUserIsAdmin()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);
        
        // Act
        var response = await client.GetAsync("users", tokenSrc.Token);
        var page = await response.Content.ReadFromJsonAsync<PageDto<UserReadDto>>(tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(page);
        Assert.True(page.TotalItems > 3);
    }

    [Fact]
    public async Task GetById_Should_ReturnUnauthorized_WhenTokenIsNotProvided()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();
        
        // Act
        var url = $"users/{UsersWebApplicationFactory.TestId}";
        var response = await client.GetAsync(url, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_Should_ReturnForbidden_WhenUserIsDifferent()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetTestUserAccessToken();
        SetBearerToken(client, token);
        
        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}";
        var response = await client.GetAsync(url, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetById_Should_ReturnOk_WhenUserIsTheSame()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetTestUserAccessToken();
        SetBearerToken(client, token);
        
        // Act
        var url = $"users/{UsersWebApplicationFactory.TestId}";
        var response = await client.GetAsync(url, tokenSrc.Token);
        var dto = await response.Content.ReadFromJsonAsync<UserReadDto>(tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(dto);
        Assert.Equal(UsersWebApplicationFactory.TestId, dto.Id);
    }

    [Fact]
    public async Task GetById_Should_ReturnOk_WhenUserIsAdmin()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);
        
        // Act
        var url = $"users/{UsersWebApplicationFactory.TestId}";
        var response = await client.GetAsync(url, tokenSrc.Token);
        var dto = await response.Content.ReadFromJsonAsync<UserReadDto>(tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(dto);
        Assert.Equal(UsersWebApplicationFactory.TestId, dto.Id);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);
        
        // Act
        var url = $"users/{Guid.CreateVersion7()}";
        var response = await client.GetAsync(url, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Should_ReturnCreated_WhenUserIsValid()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        Address address = new("USA", "California", "Torrance", "90501");

        CreateUserCommand command = new(
            "invisigal@sdn.org",
            "Courtney",
            "Visi",
            "5558322341",
            new DateOnly(1995, 5, 27),
            "robbieIII",
            address);

        // Act
        var response = await client.PostAsJsonAsync("users", command, tokenSrc.Token);
        var dto = await response.Content.ReadFromJsonAsync<UserReadDto>(tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal(dto.Email, command.Email);
        Assert.Equal(dto.Address.City, command.Address.City);
    }

    [Theory]
    [MemberData(nameof(GetInvalidCreateUserData))]
    public async Task Create_Should_ReturnBadRequest_WhenUserIsInValid(CreateUserCommand command)
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        // Act
        var response = await client.PostAsJsonAsync("users", command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_ReturnUnauthorized_WhenTokenIsNotProvided()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateUserCommand command = new(
            "Test",
            "Edited",
            "9876543210",
            new DateOnly(1987, 2, 3));

        // Act
        var url = $"users/{UsersWebApplicationFactory.TestId}";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_ReturnForbidden_WhenUserIsDifferent()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateUserCommand command = new(
            "Akira",
            "Kurusu",
            "0992384795",
            new DateOnly(1987, 2, 3));

        var token =  await GetTestUserAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_ReturnNoContent_WhenUserIsTheSame()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateUserCommand command = new(
            "Akira",
            "Kurusu",
            "0992384795",
            new DateOnly(1987, 2, 3));

        var token =  await GetJokerAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_ReturnNoContent_WhenUserIsAdmin()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateUserCommand command = new(
            "Akira",
            "Kurusu",
            "0992384795",
            new DateOnly(1987, 2, 3));

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_ReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateUserCommand command = new(
            "Akira",
            "Kurusu",
            "0992384795",
            new DateOnly(1987, 2, 3));

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{Guid.CreateVersion7()}";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateUserData))]
    public async Task Update_Should_ReturnBadRequest_WhenUserIsInValid(UpdateUserCommand command)
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetJokerAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAddress_Should_ReturnUnauthorized_WhenTokenIsNotProvided()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateAddressCommand command = new(
            "Ukraine",
            "Ivano-Frankivsk Oblast",
            "Ivano-Frankivsk",
            "76000",
            "Nezalezhnosti Street");

        // Act
        var url = $"users/{UsersWebApplicationFactory.TestId}/address";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAddress_Should_ReturnForbidden_WhenUserIsDifferent()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateAddressCommand command = new(
            "Ukraine",
            "Ivano-Frankivsk Oblast",
            "Ivano-Frankivsk",
            "76000",
            "Nezalezhnosti Street");

        var token =  await GetTestUserAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}/address";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAddress_Should_ReturnNoContent_WhenUserIsTheSame()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateAddressCommand command = new(
            "Ukraine",
            "Ivano-Frankivsk Oblast",
            "Ivano-Frankivsk",
            "76000",
            "Nezalezhnosti Street");

        var token =  await GetJokerAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}/address";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAddress_Should_ReturnNoContent_WhenUserIsAdmin()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateAddressCommand command = new(
            "Ukraine",
            "Ivano-Frankivsk Oblast",
            "Ivano-Frankivsk",
            "76000",
            "Nezalezhnosti Street");

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}/address";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAddress_Should_ReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        UpdateAddressCommand command = new(
            "Ukraine",
            "Ivano-Frankivsk Oblast",
            "Ivano-Frankivsk",
            "76000",
            "Nezalezhnosti Street");

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{Guid.CreateVersion7()}/address";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateAddressData))]
    public async Task UpdateAddress_Should_ReturnBadRequest_WhenUserIsInValid(UpdateAddressCommand command)
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetJokerAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.JokerId}/address";
        var response = await client.PutAsJsonAsync(url, command, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_ReturnUnauthorized_WhenTokenIsNotProvided()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        // Act
        var url = $"users/{UsersWebApplicationFactory.CrowId}";
        var response = await client.DeleteAsync(url, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_ReturnForbidden_WhenUserIsDifferent()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetTestUserAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.CrowId}";
        var response = await client.DeleteAsync(url, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_ReturnNoContent_WhenUserIsTheSame()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetCrowAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.CrowId}";
        var response = await client.DeleteAsync(url, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_ReturnNoContent_WhenUserIsAdmin()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{UsersWebApplicationFactory.FoxId}";
        var response = await client.DeleteAsync(url, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();
        CancellationTokenSource tokenSrc = new();

        var token =  await GetAdminAccessToken();
        SetBearerToken(client, token);

        // Act
        var url = $"users/{Guid.CreateVersion7()}";
        var response = await client.DeleteAsync(url, tokenSrc.Token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public static IEnumerable<object[]> GetInvalidCreateUserData() =>
    [
        [
            new CreateUserCommand(
                "skull",
                "Ryuji",
                "Sakamoto",
                "0672384292",
                new DateOnly(2000, 8, 02),
                "phantoms",
                new("Japan", "Tokyo", "Harajuku", "150-0001"))
        ],
        [
            new CreateUserCommand(
                "skull@phantoms.org",
                string.Empty,
                "Sakamoto",
                "0672384292",
                new DateOnly(2000, 8, 02),
                "phantoms",
                new("Japan", "Tokyo", "Harajuku", "150-0001"))
        ],
        [
            new CreateUserCommand(
                "skull@phantoms.org",
                "Ryuji",
                string.Empty,
                "0672384292",
                new DateOnly(2000, 8, 02),
                "phantoms",
                new("Japan", "Tokyo", "Harajuku", "150-0001"))
        ],
        [
            new CreateUserCommand(
                "skull@phantoms.org",
                "Ryuji",
                "Sakamoto",
                "01234567890123456789",
                new DateOnly(2000, 8, 02),
                "phantoms",
                new("Japan", "Tokyo", "Harajuku", "150-0001"))
        ],
        [
            new CreateUserCommand(
                "skull@phantoms.org",
                "Ryuji",
                "Sakamoto",
                "01234567890123456789",
                new DateOnly(2000, 8, 02),
                "0672384292",
                new(string.Empty, "Tokyo", "Harajuku", "150-0001"))
        ],
        [
            new CreateUserCommand(
                "skull@phantoms.org",
                "Ryuji",
                "Sakamoto",
                "01234567890123456789",
                new DateOnly(2000, 8, 02),
                "0672384292",
                new("Japan", string.Empty, "Harajuku", "150-0001"))
        ],
        [
            new CreateUserCommand(
                "skull@phantoms.org",
                "Ryuji",
                "Sakamoto",
                "01234567890123456789",
                new DateOnly(2000, 8, 02),
                "0672384292",
                new("Japan", "Tokyo", string.Empty, "150-0001"))
        ],
        [
            new CreateUserCommand(
                "skull@phantoms.org",
                "Ryuji",
                "Sakamoto",
                "01234567890123456789",
                new DateOnly(2000, 8, 02),
                "0672384292",
                new("Japan", "Tokyo", "Harajuku", string.Empty))
        ]
    ];
    
    public static IEnumerable<object[]> GetInvalidUpdateUserData() =>
    [
        [
            new UpdateUserCommand(
                string.Empty,
                "Sakamoto",
                "0672384292",
                new DateOnly(2000, 8, 02))
        ],
        [
            new UpdateUserCommand(
                "Ryuji",
                string.Empty,
                "0672384292",
                new DateOnly(2000, 8, 02))
        ],
        [
            new UpdateUserCommand(
                "Ryuji",
                "Sakamoto",
                string.Empty,
                new DateOnly(2000, 8, 02))
        ]
    ];

    public static IEnumerable<object[]> GetInvalidUpdateAddressData() =>
    [
        [
            new UpdateAddressCommand(
                string.Empty,
                "Ivano-Frankivsk Oblast",
                "Ivano-Frankivsk",
                "76000",
                "Nezalezhnosti Street")
        ],
        [
            new UpdateAddressCommand(
                "Ukraine",
                string.Empty,
                "Ivano-Frankivsk",
                "76000",
                "Nezalezhnosti Street")
        ],
        [
            new UpdateAddressCommand(
                "Ukraine",
                "Ivano-Frankivsk Oblast",
                string.Empty,
                "76000",
                "Nezalezhnosti Street")
        ],
        [
            new UpdateAddressCommand(
                "Ukraine",
                "Ivano-Frankivsk Oblast",
                "Ivano-Frankivsk",
                string.Empty,
                "Nezalezhnosti Street")
        ],
        [
            new UpdateAddressCommand(
                "Ukraine",
                "Ivano-Frankivsk Oblast",
                "Ivano-Frankivsk",
                "76000",
                string.Empty)
        ],
    ];

    public void Dispose() => _configClient.Dispose();

    private static void SetBearerToken(HttpClient client, string? token) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    private async Task<string?> GetAccessToken(string username, string password)
    {
        var body = new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "user"),
            new KeyValuePair<string, string>("client_secret", "vGFERwcZYrCJdnWebH9JB9EaSr8AQf8C"),
            new KeyValuePair<string, string>("scope", "roles"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        };

        var response = await _configClient.PostAsync(factory.KeycloakTokenUrl, new FormUrlEncodedContent(body));
        var content = await response.Content.ReadAsStringAsync();

        var token = JsonDocument.Parse(content)
            .RootElement
            .GetProperty("access_token")
            .GetString();

        return token;
    }

    private Task<string?> GetAdminAccessToken() => GetAccessToken("admin@mail.com", "adminsecret");

    private Task<string?> GetTestUserAccessToken() => GetAccessToken("test@mail.com", "test5678");

    private Task<string?> GetJokerAccessToken() => GetAccessToken("joker@mail.com", "phantoms12");

    private Task<string?> GetCrowAccessToken() => GetAccessToken("crow@pubsec.com", "ju5t1c3");
}