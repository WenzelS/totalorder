﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using to.contracts.data.domain;
using to.contracts.data.result;
using static to.contracts.UserQueryResult;

namespace to.contracts
{
    public interface IRequestHandler
    {
        Status HandleBacklogDeleteRequest(string id);
        Status HandleBacklogsShowRequest();
        BacklogEvalQueryResult HandleBacklogCreationRequest(BacklogCreationRequest request);
        BacklogEvalQueryResult HandleBacklogEvalQuery(string id);
        BacklogOrderQueryResult HandleBacklogOrderQuery(string id);
        BacklogEvalQueryResult HandleBacklogOrderSubmissionRequest(BacklogOrderRequest request);
        void HandleLoginQuery(LoginRequest request, Action<UserLoginQueryResult> OnSuccess, Action<string> OnFailure);
        void HandleUserUpdateRequest(UserUpdateRequest request, Action<UserListResult> OnSuccess, Action<string> OnFailure);
        void HandleUserEditRequest(UserEditRequest request, Action<UserQueryResult> OnSuccess, Action<string> OnFailure);
        void HandleUserListRequest(Action<UserListResult> OnSuccess, Action<string> OnFailure);
        void HandleUserCreateRequest(UserCreateRequest request, Action<UserListResult> OnSuccess, Action<string> OnFailure);
        void HandleUserDeleteRequest(UserDeleteRequest request, Action<UserListResult> OnSuccess, Action<string> OnFailure);
    }

    public class BacklogShowQueryResult
    {
        public BacklogShowQueryResult(IEnumerable<BacklogDisplayItem> backlogs)
        {
            Backlogs = backlogs.ToList();
        }
        public List<BacklogDisplayItem> Backlogs { get; set; }

        public class BacklogDisplayItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public int UserStoryCount { get; set; }
            public int VoteCount { get; set; }

            public static BacklogDisplayItem FromBacklog(Backlog backlog, Submission[] submissions) => new BacklogDisplayItem() {
                Id = backlog.Id,
                Title = backlog.Title,
                UserStoryCount = backlog.UserStories.Length,
                VoteCount = submissions.Length
            };
        }
    }

    public class UserUpdateRequest
    {
        public int Id { get; set; }
        public UserRole UserRole { get; set; }
    }

    public class UserListResult
    {
        public UserQueryResult[] Users{ get; set; }
    }

    public class UserEditRequest
    {
        public int Id { get; set; }
    }

    public class UserDeleteRequest
    {
        public int Id { get; set; }
    }

    public class UserQueryResult
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public UserRole UserRole { get; set; }

        public UserQueryResult()
        {
            
        }

        public UserQueryResult(User user)
        {
            Id = user.Id;
            UserRole = user.UserRole;
            Username = user.Username;
        }
    }

    public class UserLoginQueryResult : UserQueryResult
    {
        public UserLoginQueryResult(User user, List<Permission> permissions) : base(user)
        {
            Permissions = permissions;
        }

        public List<Permission> Permissions { get; set; }

    }

    public class BacklogOrderRequest
    {
        public string Id { get; set; }
        public int[] UserStoryIndexes { get; set; }
    }

    public class BacklogOrderQueryResult
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string[] UserStories { get; set; }
        public int[] UserStoryIndexes { get; set; }
    }

    public class BacklogCreationRequest
    {
        public string Title { get; set; }
        public string[] UserStories { get; set; }
    }

    public class BacklogEvalQueryResult
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string[] UserStories { get; set; }
        public int NumberOfSubmissions { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserCreateRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserRole UserRole { get; set; }
    }
}
