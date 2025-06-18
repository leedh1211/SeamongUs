namespace _02_Scripts.SignUP
{
    public class SignUpData
    {
        public string userId;
        public string password;
        public string nickname;
        public string email;

        public SignUpData(string id, string password, string nickname, string email)
        {
            this.userId = id;
            this.password = password;
            this.nickname = nickname;
            this.email = email;
        }
    }
}