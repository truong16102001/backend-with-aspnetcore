namespace NetCore.DataAccess.Common
{
    public static class CONSTANT
    {
        public static class MESSAGE
        {
            public const string SUCCESS =
                "Success";

            public const string CREATE_SUCCESS =
                "Create successfully";

            public const string UPDATE_SUCCESS =
                "Update successfully";

            public const string DELETE_SUCCESS =
                "Delete successfully";

            public const string NOT_FOUND =
                "Data not found";

            public const string INTERNAL_ERROR = "Internal Error";
        }

        public static class PERMISSION
        {
            public static class ROOM
            {
                public const string READ = "ROOM.READ";

                public const string CREATE = "ROOM.CREATE";

                public const string UPDATE = "ROOM.UPDATE";

                public const string DELETE = "ROOM.DELETE";
            }
        }

        public static class REDIS
        {
            public static class AUTH
            {
                public static string SESSION(string sid) => $"auth:sessions:{sid}";
                public static string REFRESH_TOKENS(string hashRt) => $"auth:refresh_tokens:{hashRt}";
                public static string USER_SESSIONS(int userId) => $"auth:user_sessions:{userId}";
            }
        }
    }
}
