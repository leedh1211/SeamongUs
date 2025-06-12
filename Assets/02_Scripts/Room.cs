using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    [Header("Player Slots (Max 8)")]
    public List<PlayerSlot> playerSlots; // 플레이어 슬롯 UI (빈 슬롯 포함)

    [Header("Chat")]
    public InputField chatInputField;
    public Text chatDisplay;

    [Header("Character Selection")]
    public Dropdown characterDropdown;

    [Header("Room Settings")]
    public Dropdown impostorCountDropdown;
    public Dropdown missionCountDropdown;

    [Header("Controls")]
    public Button readyButton;
    public Button startButton;

    private bool isHost = false;
    private string localPlayerID;

    void Start()
    {
        //플레이어 ID와 호스트 여부 체크 기능 추가 요망

        // UI 초기화


        // 이벤트 리스너 등록

    }

    void SetupUIByHost()
    {
        // 호스트 여부에 따라 UI 설정(호스트는 시작 버튼, 일반 플레이어는 준비 버튼)
        startButton.gameObject.SetActive(isHost);
        readyButton.gameObject.SetActive(!isHost);

        // 방장 여부에 따라 드롭다운 활성화
        impostorCountDropdown.interactable = isHost;
        missionCountDropdown.interactable = isHost;
    }

    void RefreshPlayerSlots()
    {
        // 플레이어 슬롯을 현재 플레이어 목록으로 업데이트해주는 로직
    }

    void OnClickReady()
    {
        // (NetworkManager)로컬 플레이어가 준비 상태로 전환
    }

    void OnClickStart()
    {
        // (NetworkManager)호스트가 게임 시작 요청
    }

    void OnChatSubmit(string message)
    {
        // 채팅 메시지 전송 로직
    }

    // 외부에서 호출
    public void AddChatMessage(string sender, string message)
    {
        // 채팅 메시지를 UI에 추가하는 로직
    }

    public void UpdateCharacterSelection(string selectedCharacter)
    {
        // 선택된 캐릭터를 네트워크 매니저에 전달
    }


}
