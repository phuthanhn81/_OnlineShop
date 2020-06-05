namespace OnlineShop.Common
{
    public static class CommonConstants
    {
        // save data tương tác của user tạm thời trên server chả liên quan gì đến biến local hay global
        public static string USER_SESSION = "USER_SESSION";
        public static string CartSession = "CartSession";
        public static string SESSION_CREDENTIALS = "SESSION_CREDENTIALS";

        public static string CurrentCulture { set; get; }
    }
}