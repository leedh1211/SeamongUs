using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using _02_Scripts.Alert;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace _02_Scripts.SignUP
{
    public class SignUpManager : MonoBehaviour
    {
        [SerializeField] private SignUpUIManager signUpUIManager;
        [SerializeField] private TMP_InputField idInputField;
        [SerializeField] private TMP_InputField pwInputField;
        [SerializeField] private TMP_InputField pwcInputField;
        [SerializeField] private TMP_InputField nickNameInputField;
        [SerializeField] private TMP_InputField emailInputField;
        private bool IsDuplicateCheck = false;

        private string signUpUrl = "http://121.162.172.253:3000/api/signUp/signUp";
        private string duplicationCheckUrl = "http://121.162.172.253:3000/api/signUp/isDuplicateCheck";

        public void OnSignUpButtonPressed()
        {
            SoundManager.Instance.PlaySFX(SFXType.Click);

            string id = idInputField.text;
            string pw = pwInputField.text;
            string pwc = pwcInputField.text;
            string nickName = nickNameInputField.text;
            string email = emailInputField.text;

            ValidationResult SignUpValidResult = isCheckedSignUpData(id, pw, pwc, nickName, email);

            if (!SignUpValidResult.IsValid)
            {
                // UI매니저에 토스트 팝업 띄우기
                AlertUIManager.Instance.OnAlert(SignUpValidResult.Message);
            }
            else
            {
                StartCoroutine(SendSignUpRequest(id, pw, nickName, email));
            }
        }

        public void OnDuplicateCheckButtonPressed()
        {
            string id = idInputField.text;
            if (IsValidId(id))
            {
                StartCoroutine(SendDuplicateRequest(id));
            }
            else
            {
                AlertUIManager.Instance.OnAlert("아이디는 8~20자 영문,숫자로 입력해주세요");
            }
        }

        public ValidationResult isCheckedSignUpData(string id, string pw, string pwc, string nickName, string email)
        {
            bool result = true;
            string message = "";
            if (!IsDuplicateCheck)
            {
                result = false;
                message = "아이디 중복체크를 진행해주세요.";
            }
            else if (!IsValidId(id))
            {
                result = false;
                message = "아이디를 제대로 입력해주세요";
            }
            else if (pw != pwc || !IsValidPassword(pw))
            {
                result = false;
                message = "비밀번호를 다시 확인해주세요";
            }
            else if (!IsValidNickname(nickName))
            {
                result = false;
                message = "닉네임은 2자~20자 이내로 작성해주세요";
            }
            else if (!IsValidEmail(email))
            {
                result = false;
                message = "이메일 형식이 맞지 않습니다.";
            }

            ValidationResult resultData = new ValidationResult(result, message);
            return resultData;
        }

        private IEnumerator SendSignUpRequest(string userId, string password, string nickname, string email)
        {
            string HashPass = ConvertHash.StringToHash(password);
            // JSON 형식으로 전송할 데이터 생성
            string jsonData = JsonConvert.SerializeObject(new SignUpData(userId, HashPass, nickname, email));

            // 요청 객체 생성
            UnityWebRequest request = new UnityWebRequest(signUpUrl, "POST");
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 전송
            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                SignUpResponse response = JsonConvert.DeserializeObject<SignUpResponse>(request.downloadHandler.text);
                if (response.success)
                {
                    AlertUIManager.Instance.OnAlert("회원가입이 완료되었습니다.");
                    signUpUIManager.ResetSignUpPopup();
                    signUpUIManager.HideSignUpPopup();
                    IsDuplicateCheck = false;
                }
                else
                {
                    AlertUIManager.Instance.OnAlert("회원가입에 실패하였습니다. 다시 시도해주세요");
                }
            }
            else
            {
                Debug.LogError("서버 요청 실패: " + request.error);
            }
        }

        private IEnumerator SendDuplicateRequest(string userId)
        {
            // JSON 형식으로 전송할 데이터 생성
            string jsonData = JsonConvert.SerializeObject(new { userId = userId });

            // 요청 객체 생성
            UnityWebRequest request = new UnityWebRequest(duplicationCheckUrl, "POST");
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 전송
            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                duplicationData response = JsonConvert.DeserializeObject<duplicationData>(request.downloadHandler.text);
                if (response.result)
                {
                    IsDuplicateCheck = true;
                    idInputField.readOnly = true;
                    AlertUIManager.Instance.OnAlert("사용가능한 아이디 입니다.");
                }
                else
                {
                    AlertUIManager.Instance.OnAlert("아이디가 중복됩니다. 다른 아이디를 이용해주세요.");
                }
            }
            else
            {
                Debug.LogError("서버 요청 실패: " + request.error);
            }
        }

        bool IsValidId(string id) => Regex.IsMatch(id, "^[a-zA-Z0-9]{8,20}$");
        bool IsValidPassword(string pw) => Regex.IsMatch(pw, "^.{8,20}$");
        bool IsValidNickname(string nick) => Regex.IsMatch(nick, "^[a-zA-Z가-힣]{2,20}$");
        bool IsValidEmail(string email) => Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
    }
}

public class SignUpResponse
{
    public bool success { get; set; }
    public int insertId { get; set; }
}

public class duplicationData
{
    public bool success { get; set; }
    public bool result { get; set; }
}

public struct ValidationResult
{
    public bool IsValid;
    public string Message;

    public ValidationResult(bool isValid, string message)
    {
        IsValid = isValid;
        Message = message;
    }
}