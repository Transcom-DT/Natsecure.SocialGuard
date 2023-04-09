﻿using System.Net;
using System.Net.Http.Json;
using JetBrains.Annotations;
using SocialGuard.Common.Data.Models.Authentication;

namespace SocialGuard.Client.Http.Services;

public partial class SocialGuardHttpClient
{
	/// <summary>
	/// Registers a new user on the directory.
	/// </summary>
	/// <param name="registerDetails">The details of the user to register.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The response from the server.</returns>
	public async Task<Response?> RegisterNewUserAsync(RegisterModel registerDetails, CancellationToken ct = default)
	{
		using HttpRequestMessage request = new(HttpMethod.Post, "/api/v3/auth/register")
		{
			Content = JsonContent.Create(registerDetails)
		};

		using HttpResponseMessage response = await HttpClient.SendAsync(request, ct);
		return await response.Content.ReadFromJsonAsync<Response>(SerializerOptions, ct);
	}
	
	/// <summary>
	/// Logs in a user and returns the token.
	/// </summary>
	/// <param name="username">The username of the user to log in.</param>
	/// <param name="password">The password of the user to log in.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>The token for the user.</returns>
	/// <exception cref="HttpRequestException">Thrown when the authentication fails.</exception>
	public async Task<TokenResult?> LoginAsync(string username, string password, CancellationToken ct = default)
	{
		using HttpRequestMessage request = new(HttpMethod.Post, "/api/v3/auth/login")
		{
			Content = JsonContent.Create(
				new LoginModel
				{
					Username = username,
					Password = password
				},
				options: SerializerOptions
			)
		};

		using HttpResponseMessage response = await HttpClient.SendAsync(request, ct);

		if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
		{
			throw new HttpRequestException("Authentication Failed. Please check and update your credentials.", null, response.StatusCode);
		}

		return (await response.Content.ReadFromJsonAsync<Response<TokenResult>>(SerializerOptions, ct))?.Details;
	}
}
