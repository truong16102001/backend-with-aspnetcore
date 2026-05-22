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
    }
}
