using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public static class RandomSessionNameGenerator
{
    private const string chars = "ABCDEFGHJKMNOPQRSTUVWXYZabcdefghjkmnopqrstuvwxyz0123456789";
    
    // 5글자의 랜덤 방 이름 생성 함수
    public static string GenerateRandomRoomName(int length = 5)
    {
        StringBuilder result = new StringBuilder(length);
        System.Random random = new System.Random();
        
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }
        
        return result.ToString();
    }
}
