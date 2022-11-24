using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class CommonEnums
    {
        public static class STATUS_CODE
        {
            public static readonly int OK = 200;
            public static readonly int AUTH_FAILED = 403;
            public static readonly int BAD_REQUEST = 400;
            public static readonly int NOT_FOUND = 404;
            public static readonly int SERVER_ERROR = 500;
        }

        public static class USER_ROLE_ID
        {
            public const string ADMINISTRATOR = "F54ED2C8-2FD6-44E0-AC2F-61FE5A466B0E";
            public const string USER = "09F7343C-67F9-474F-8186-1C9CDC78E1FF";
            public const string HR = "22FC848E-6B93-43D4-A911-05370D8A0DBF";
            public const string INTERVIEWER = "B98D2F1D-A8EA-4FAA-A9EE-267A6EE7F92B";
        }

        public static class USER_ROLE
        {
            public const string ADMINISTRATOR = "ADMINISTRATOR";
            public const string USER = "USER";
            public const string HR = "HR";
            public const string INTERVIEWER = "INTERVIEWER";
        }

        public static string API_PATH = "http://localhost:5000/api/";

        public static class POST_STATUS
        {
            public const int Available = 1; // open for applying
            public const int Deleted = 0; // Deleted
            public const int Closed = 2; // Finished post
            public const int Pending = 3; // stop receiving form
        }

        public static class APPLICATION_FORM_STATUS
        {
            public const int Open = 1;
            public const int Approved = 2;
            public const int Rejected = 3;
            public const int Revoked = 4; // revoked by user who applied
            public const int Pending = 5; // when this applied user has been having one form being approved by HR -> pending all remaing forms
            public const int BatchRejected = 6; // when this user interview's status is successfully -> all pending form  will be batch rejected
            public const int Successfully = 7; // passed interview
            public const int Failed = 8; // failed interview

        }
    }
}
