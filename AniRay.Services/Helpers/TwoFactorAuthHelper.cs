using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AniRay.Services.Helpers
{
    public static class TwoFactorAuthHelper
    {
        public static string Hash2FA(string code, DateTime salt)
        {
            var algorithm = SHA256.Create();
            var saltString = salt.ToString();
            var hashCodeByte = algorithm.ComputeHash(Encoding.UTF8.GetBytes(saltString + code + saltString));
            return BitConverter.ToString(hashCodeByte).Replace("-", string.Empty);
        }

        public static string CodeGeneration()
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] code = new char[6];

            for (int i = 0; i < code.Length; i++)
            {
                code[i] = characters[random.Next(characters.Length)];
            }

            string result = new string(code);
            return result;
        }

        public static TwoWayAuth NewRecord(int UserID, string Code)
        {
            var Time = DateTime.Now;
            var HashedCode = Hash2FA(Code, Time);

            TwoWayAuth New = new TwoWayAuth();
            New.Code = HashedCode;
            New.UserId = UserID;
            New.CreatedAt = Time;

            return New;
        }

    }
}
