using System;
using Steamworks;
using UnityEngine;

public class SteamTicketManager : MonoBehaviour 
{
    // //새로운 토큰 생성
    // public static byte[] NewToken() => Guid.NewGuid().ToByteArray();

    // /// <summary>
    // /// Token을 hash하는 함수
    // /// </summary>
    // /// <param name="token">hash할 token</param>
    // /// <returns>hash된 token</returns>
    // public static int HashToken(byte[] token) => new Guid(token).GetHashCode();

    // /// <summary>
    // /// Token을 string으로 변환하는 함수
    // /// </summary>
    // /// <param name="token">string으로 변환할 token</param>
    // /// <returns>string으로 변환된 token</returns>
    // public static string TokenToString(byte[] token) => new Guid(token).ToString();

    #region Singleton Pattern
    
    private static SteamTicketManager instance = null;
    
    /// <summary>
    /// UIManager 싱글톤 구현
    /// </summary>
    public static SteamTicketManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }
    
    #endregion
    
    private byte[] authTicketData;
    private HAuthTicket authTicketHandle;
    
    void Start()
    {
        // 싱글톤
        if (SteamTicketManager.Instance == null){
            
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steamworks is not initialized!");
            return;
        }

        // Steam 인증 티켓을 생성할 최대 크기 설정 (최대 1024 바이트)
        authTicketData = new byte[1024];

        // Steam 인증 티켓 생성
        uint ticketSize;

        SteamNetworkingIdentity steamNetworkingIdentity = new SteamNetworkingIdentity();
        steamNetworkingIdentity.SetSteamID(SteamUser.GetSteamID());

        authTicketHandle = SteamUser.GetAuthSessionTicket(authTicketData, authTicketData.Length, out ticketSize, ref steamNetworkingIdentity);

        if (authTicketHandle == HAuthTicket.Invalid)
        {
            Debug.LogError("Failed to create Steam Auth Session Ticket.");
            return;
        }

        Debug.Log($"Steam Auth Session Ticket created. Ticket size: {ticketSize}");
    }

    public byte[] GetToken()
    {
        return authTicketData;
    }
}
