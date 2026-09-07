using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Recommendations.Source;

public class JWT
{
    public static string issuer = "serv";
    public static string audience = "serv";
    public static string key = "sekretkeysekretkeysekretkeysekretkey";
    public static SymmetricSecurityKey GetSymmetricSecurityKey()=> new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
}
