// Igor Krupin
// Based on https://github.com/vavjeeva/AzureSignalRConsoleApp
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace SignalR.Azure.Serverless
{
    public class SignalRHubHelper
    {
        static readonly JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();

        public string Endpoint { get; private set; }

        public string Version { get; private set; }

        SigningCredentials _signingCredentials;

        public SignalRHubHelper(string connectionString)
        {
            ParseConnString(connectionString);
        }

        void ParseConnString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            var pattern = "Endpoint=(?<ep>.+);AccessKey=(?<ak>.+);Version=(?<v>.+);";

            var match = Regex.Match(connectionString, pattern);

            if (!match.Success)
                throw new ArgumentException($"Connection string does not match {pattern} pattern", nameof(connectionString));

            Endpoint = match.Groups["ep"].Value;
            Version = match.Groups["v"].Value;

            var accesKey = match.Groups["ak"].Value;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accesKey));

            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }


        /// <summary>
        /// As per https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-2.2 token is validated upon establishing the initial connection.
        /// Once the connection is established, token will not be checked for revocation
        /// </summary>
        public string GenerateAccessToken(string audience, string userId, TimeSpan? lifetime = null)
        {
            IEnumerable<Claim> claims = null;

            if (!string.IsNullOrEmpty(userId))
            {
                claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };
            }

            return GenerateAccessToken(audience, claims, lifetime ?? TimeSpan.FromMinutes(3)); // short lived token
        }

        /// <summary>
        /// As per https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-2.2 token is validated upon establishing the initial connection.
        /// Once the connection is established, token will not be checked for revocation
        /// </summary>
        public string GenerateAccessToken(string audience, IEnumerable<Claim> claims, TimeSpan lifetime)
        {
            var expire = DateTime.UtcNow.Add(lifetime);

            var token = JwtTokenHandler.CreateJwtSecurityToken(
                issuer: null,
                audience: audience,
                subject: claims == null ? null : new ClaimsIdentity(claims),
                expires: expire,
                signingCredentials: _signingCredentials);

            return JwtTokenHandler.WriteToken(token);
        }

        public string ClientUrl(string hubName) => $"{Endpoint}/client/?hub={hubName.ToLowerInvariant()}";
    }
}
