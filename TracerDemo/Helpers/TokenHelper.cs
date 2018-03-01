using TracerDemo.Model;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TracerDemo.Helpers
{
	public class TokenHelper
	{
		private ApplicationSettings ApplicationSettings { get; set; }

		public TokenHelper(ApplicationSettings applicationSettings)
		{
			ApplicationSettings = applicationSettings;
		}

		public string EncodeStandardJwtToken(IToken payload)
		{
			IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
			IJsonSerializer serializer = new JsonNetSerializer();
			IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
			IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

			var jwt = encoder.Encode(payload, ApplicationSettings.SigningKey);

			return jwt;
		}

		public T DecodeStandardJwtToken<T>(string token) where T : IToken
		{
			IJsonSerializer serializer = new JsonNetSerializer();
			IDateTimeProvider provider = new UtcDateTimeProvider();
			IJwtValidator validator = new JwtValidator(serializer, provider);
			IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
			IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

			string json = decoder.Decode(token, ApplicationSettings.SigningKey, verify: true);
			T recoveryToken = JsonConvert.DeserializeObject<T>(json);

			return recoveryToken;
		}



		async public Task<string> BuildJwtAuthorizationToken(User user, TokenProviderOptions options)
		{
			var now = DateTime.UtcNow;
			// Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
			// You can add other claims here, if you want:
			var claims = new List<Claim>()
			{

				new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(ClaimTypes.Name, user.Id),
				new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, await options.NonceGenerator()),
                //new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
            };

			foreach (Rol role in user.Roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role.value));
			}

			var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
				issuer: options.Issuer,
				claims: claims,
				notBefore: now,
				signingCredentials: options.SigningCredentials);
			var encodedJwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(jwt);

			return encodedJwt;
		}


		public void BuildResponseCookie(HttpContext context, string encodedJwt)
		{
			CookieOptions options = new CookieOptions()
			{
				Expires = new DateTimeOffset(DateTime.Now.AddDays(1000)),
				HttpOnly = false,
				SameSite = SameSiteMode.None,
				Domain = "tracer-lol-user"
			};
			context.Response.Cookies.Append("access_token", encodedJwt, options);
		}

	}

	public class TokenProviderOptions
	{
		/// <summary>
		///  The Issuer (iss) claim for generated tokens.
		/// </summary>
		public string Issuer { get; set; }

		/// <summary>
		/// The signing key to use when generating tokens.
		/// </summary>
		public SigningCredentials SigningCredentials { get; set; }

		/// <summary>
		/// Generates a random value (nonce) for each generated token. Default is a guid.
		/// </summary>
		public Func<Task<string>> NonceGenerator { get; set; } = new Func<Task<string>>(() => Task.FromResult(Guid.NewGuid().ToString()));

	}


	public class CustomJwtDataFormat : ISecureDataFormat<AuthenticationTicket>
	{
		private readonly string algorithm;
		private readonly TokenValidationParameters validationParameters;

		public CustomJwtDataFormat(string algorithm, TokenValidationParameters validationParameters)
		{
			this.algorithm = algorithm;
			this.validationParameters = validationParameters;
		}

		public AuthenticationTicket Unprotect(string protectedText)
		=> Unprotect(protectedText, null);

		public AuthenticationTicket Unprotect(string protectedText, string purpose)
		{
			var handler = new JwtSecurityTokenHandler();
			ClaimsPrincipal principal = null;
			SecurityToken validToken = null;

			try
			{
				principal = handler.ValidateToken(protectedText, this.validationParameters, out validToken);

				var validJwt = validToken as JwtSecurityToken;

				if (validJwt == null)
				{
					throw new ArgumentException("Invalid JWT");
				}

				if (!validJwt.Header.Alg.Equals(algorithm, StringComparison.Ordinal))
				{
					throw new ArgumentException($"Algorithm must be '{algorithm}'");
				}

				// Additional custom validation of JWT claims here (if any)
			}
			catch (SecurityTokenValidationException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}

			// Validation passed. Return a valid AuthenticationTicket:
			return new AuthenticationTicket(principal, new Microsoft.AspNetCore.Authentication.AuthenticationProperties(), "Cookie");
		}

		// This ISecureDataFormat implementation is decode-only
		public string Protect(AuthenticationTicket data)
		{
			throw new NotImplementedException();
		}

		public string Protect(AuthenticationTicket data, string purpose)
		{
			throw new NotImplementedException();
		}
	}
}
