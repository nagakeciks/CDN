using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Security.Claims;
using HafizDemoAPI.ModelCDN;
using static HafizDemoAPI.Controllers.UserController;

namespace HafizDemoAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        static CDNContext cdnCtxt = new();

        public UserController()
        {
            cdnCtxt = new CDNContext();
        }
        [HttpGet]
        public List<User> GetUser()
        {
            var users = cdnCtxt.Users.AsNoTracking().Select(d => new User { UserID = d.UserId, UserName = d.Username, Mail = d.Mail, PhoneNo = d.PhoneNo }).ToList();
            users.ForEach(user =>
            {
                var skills = cdnCtxt.Skills.AsNoTracking().Where(d => d.UserId == user.UserID).ToList();
                var hobbies = cdnCtxt.Hobbies.AsNoTracking().Where(d => d.UserId == user.UserID).ToList();
                skills.ForEach(d =>
                {
                    user.SkillsList.Add(
                        new Skills { SkillName = d.SkillName, SkillRatings = d.SkillRating }
                    );
                });

                hobbies.ForEach(d =>
                {
                    user.HobbiesList.Add(
                        new Hobbies { HobbyName = d.HobbyName }
                    );
                });
            });
            return users;

        }

        public class User 
            {
            public User()
            {
                SkillsList = new List<Skills>();
                HobbiesList = new List<Hobbies>();
            }
            public Int32 UserID { get; set; }
            public string UserName { get; set; }
            //public string Password { get; set; }
            public string Mail { get; set; }
            public string PhoneNo { get; set; }
            public string Status { get; set; }
            public string FollowedStatus { get; set; }
            public List<Skills> SkillsList { get; set; }
            public List<Hobbies> HobbiesList { get; set; }
        }

        public class Profile
        {
            public string UserName { get; set; }
            //public string Password { get; set; }
            public string Mail { get; set; }
            public string PhoneNo { get; set; }
            public string SkillList { get; set; }
            public string HobbyList { get; set; }
        }
        [Authorize]
        [HttpGet]
        public Profile GetProfile(Int32 UserID)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var CurrentUserID = identity.FindFirst("Id").Value;
            CDNContext cdnCtxt = new();
            var user = cdnCtxt.Users.Where(user => user.UserId== UserID).FirstOrDefault();
            if (user == null) return null;
            var myProfile = new Profile { UserName = user.Username, Mail = user.Mail, PhoneNo = user.PhoneNo };
            var skills = cdnCtxt.Skills.AsNoTracking().Where(d => d.UserId == UserID).ToList();
            var hobbies = cdnCtxt.Hobbies.AsNoTracking().Where(d => d.UserId == UserID).ToList();
            if (skills != null)
            {
                System.Text.StringBuilder skillbuilder = new System.Text.StringBuilder();
                skills.ForEach(skill =>
                {
                    skillbuilder.Append($"{skill.SkillName} ({skill.SkillRating}) ");
                });
                myProfile.SkillList = skillbuilder.ToString();
            }
            if (hobbies != null)
            {
                System.Text.StringBuilder hobbybuilder = new System.Text.StringBuilder();
                hobbies.ForEach(hobby =>
                {
                    hobbybuilder.Append($"{hobby.HobbyName} ");
                });
                myProfile.HobbyList = hobbybuilder.ToString();
            }
            return myProfile;
        }

        [HttpGet]
        public UserRead GetProfileEdit(Int32 UserID)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity.Name != null)
            {
                var CurrentUserID = identity.FindFirst("Id").Value;
            }
            CDNContext cdnCtxt = new();
            var user = cdnCtxt.Users.AsNoTracking().Where(user => user.UserId == UserID).FirstOrDefault();
            if (user == null) return null;
            var myProfileEdit = new UserRead {UserID = user.UserId, UserName = user.Username, Mail = user.Mail, PhoneNo = user.PhoneNo};
            var skills = cdnCtxt.Skills.AsNoTracking().Where(d => d.UserId == UserID).ToList();
            var hobbies = cdnCtxt.Hobbies.AsNoTracking().Where(d => d.UserId == UserID).ToList();
            if (skills != null)
            {
                skills.ForEach(skill =>
                {
                    myProfileEdit.SkillsList.Add(new Skills { SkillName = skill.SkillName , SkillRatings = skill.SkillRating});
                });
            }
            if (hobbies != null)
            {

                hobbies.ForEach(hobby =>
                {
                    myProfileEdit.HobbiesList.Add(new Hobbies { HobbyName = hobby.HobbyName });
                });

            }
            return myProfileEdit;
        }

        [Authorize]
        [HttpGet]
        public List<User> GetUserList(Int32 UserID)
        {
            //    { userId: 1, username: 'User1', email: 'user1@example.com', phoneNumber: '555-111-1111', status: 'Live', followedStatus: 'No' },
            //{ userId: 2, username: 'User2', email: 'user2@example.com', phoneNumber: '555-222-2222', status: 'N/A', followedStatus: 'Yes' },
            //{ userId: 3, username: 'User3', email: 'user3@example.com', phoneNumber: '555-333-3333', status: 'Live', followedStatus: 'Yes' },
            var listUser = new List<User>();
            //listUser.Add(new User { UserID = 2, UserName = "nagakeciks", Mail = "nagakeciks@gmail.com", PhoneNo = "0122026609", Status = "Online", FollowedStatus = "Yes" });
            //listUser.Add(new User { UserID = 21, UserName = "hafiz", Mail = "nagakeciks@gmail.com", PhoneNo = "0122026609", Status = "Online", FollowedStatus = "Yes" });
            //listUser.Add(new User { UserID = 22, UserName = "din", Mail = "nagakeciks@gmail.com", PhoneNo = "0122026609", Status = "Offline", FollowedStatus = "No" });
            CDNContext cdnCtxt = new();
            var Users = cdnCtxt.Users.Where(user => user.UserId != UserID).ToList();

            foreach (var user in Users)
            {
                var userConn = cdnCtxt.CDNUserConn.Where(u => u.UserID == user.UserId).AsNoTracking().OrderByDescending(i => i.Id).FirstOrDefault();
                listUser.Add(new User
                {
                    UserID = user.UserId, UserName = user.Username, Mail = user.Mail, PhoneNo = user.PhoneNo,
                    Status = userConn != null && userConn.Connected ? "Online" : "Offline", FollowedStatus = "Yes"

                });
            }
           
            listUser.ForEach(user =>
            {
                List<Skills> skillList = new List<Skills>();
                var skills = cdnCtxt.Skills.AsNoTracking().Where(d => d.UserId == user.UserID).ToList();
                var hobbies = cdnCtxt.Hobbies.AsNoTracking().Where(d => d.UserId == user.UserID).ToList();
                skills.ForEach(d =>
                {

                    skillList.Add(
                        new Skills { SkillName = d.SkillName,SkillRatings = d.SkillRating }
                    );
                });
                user.SkillsList.AddRange(skillList);

            });
            return listUser;
        }

        [HttpPost]
        public UserRegStatus CreateUser(Users user)
        {
            if (CheckUserExistByUsernameEmail(user.UserName.ToLower(),user.Mail.ToLower()))
            {
                return new UserRegStatus { Success = false, ErrMsg = "Please use different username or email" };
            }
            PubFunc pubFunc = new();
            var returnHash = pubFunc.HashPassword(user.Password);
            var User = new ModelCDN.User { Username = user.UserName.ToLower(), Password = returnHash.HashedPassword, Salt = returnHash.Salt, Mail = user.Mail.ToLower(), PhoneNo = user.PhoneNo };
            cdnCtxt.Users.Add(User);
            cdnCtxt.SaveChanges();
            var Skills = new List<ModelCDN.Skill>();
            user.SkillsList.ForEach(skill =>
            {
                Skills.Add(new ModelCDN.Skill { UserId = User.UserId, SkillName = skill.SkillName, SkillRating = skill.SkillRatings });
            });
            cdnCtxt.Skills.AddRange(Skills);
            var hobbies = new List<ModelCDN.Hobby>();
            user.HobbiesList.ForEach(hobby =>
            {
                hobbies.Add(new Hobby {UserId = User.UserId, HobbyName = hobby.HobbyName });
            });
            cdnCtxt.Hobbies.AddRange(hobbies);
            cdnCtxt.SaveChanges();
            return new UserRegStatus { Success = true };
        }

        public class UserDelete
        {
            public Int32 UserID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }    
        }
        
        public class rtnDeleteUser
        {
            public bool Success { get; set; }
            public string ErrorMsg { get; set; }
        }
        [Authorize]
        [HttpPost]
        public rtnDeleteUser DeleteUser(UserDelete UserToDelete)
        {
            PubFunc pubFunc = new();
            var user = cdnCtxt.Users.AsNoTracking().Where(u => u.UserId == UserToDelete.UserID).FirstOrDefault();

            if (user == null)
            {
                return new rtnDeleteUser { Success = false , ErrorMsg = "User not found" };
            }
            if (String.Compare(UserToDelete.Username, user.Username, comparisonType: StringComparison.OrdinalIgnoreCase) > 0)
            {
                return new rtnDeleteUser { Success = false, ErrorMsg = "Invalid username" };
            }
            if (!pubFunc.VerifyPassword(UserToDelete.Password, user.Password, Convert.FromBase64String(user.Salt))) 
            {
                return new rtnDeleteUser { Success = false, ErrorMsg = "Invalid password" };
            }
            cdnCtxt.Skills.Where(u => u.UserId == user.UserId).ExecuteDelete();
            cdnCtxt.Hobbies.Where(u => u.UserId == user.UserId).ExecuteDelete();
            cdnCtxt.Users.Where(u => u.UserId == user.UserId).ExecuteDelete();
            return new rtnDeleteUser { Success = true, ErrorMsg = "" };
        }
        [HttpPost]
        public UserRegStatus UpdateUser(UserRead user)
        {
            if (CheckUserExistByUsernameEmail(user.UserID, user.UserName.ToLower(), user.Mail.ToLower()))
            {
                return new UserRegStatus { Success = false, ErrMsg = "Please use different username or email" };
            }
            PubFunc pubFunc = new();
            var returnHash = pubFunc.HashPassword(user.Password);
            var userUpdate = cdnCtxt.Users.Where(u => u.UserId == user.UserID).FirstOrDefault();
            userUpdate.Username = user.UserName;
            userUpdate.Password = returnHash.HashedPassword;
            userUpdate.Salt = returnHash.Salt;
            userUpdate.Mail = user.Mail;
            userUpdate.PhoneNo= user.PhoneNo;
            //Reset Skills, Hobbies
            var SkillRemove = cdnCtxt.Skills.Where(u => u.UserId == user.UserID).ToList();
            cdnCtxt.Skills.RemoveRange(SkillRemove);
            var hobbyRemove = cdnCtxt.Hobbies.Where(u => u.UserId == user.UserID).ToList();
            cdnCtxt.Hobbies.RemoveRange(hobbyRemove);

            var Skills = new List<ModelCDN.Skill>();
            user.SkillsList.ForEach(skill =>
            {
                Skills.Add(new ModelCDN.Skill { UserId = user.UserID, SkillName = skill.SkillName, SkillRating = skill.SkillRatings });
            });
            cdnCtxt.Skills.AddRange(Skills);
            var hobbies = new List<ModelCDN.Hobby>();
            user.HobbiesList.ForEach(hobby =>
            {
                hobbies.Add(new Hobby { UserId = user.UserID, HobbyName = hobby.HobbyName });
            });
            cdnCtxt.Hobbies.AddRange(hobbies);
            cdnCtxt.SaveChanges();
            return new UserRegStatus { Success = true };
        }

        public class UserRegStatus
        {
            public bool Success { get; set; }
            public string ErrMsg { get; set; }
        }
        internal bool CheckUserExistByEmail(string? Mail)
        {

            var User = cdnCtxt.Users.AsNoTracking().Where(d => d.Mail == Mail).FirstOrDefault();
            if (User != null) { return true; }
            return false;

        }

        internal bool CheckUserExistByUsernameEmail(string? Username, string? Mail)
        {

            var User = cdnCtxt.Users.AsNoTracking().Where(user => user.Username == Username || user.Mail == Mail).FirstOrDefault();
            if (User != null) { return true; }
            return false;

        }

        internal bool CheckUserExistByUsernameEmail(Int32 UserId,string? Username, string? Mail)
        {

            var User = cdnCtxt.Users.AsNoTracking().Where(user => user.UserId != UserId && (user.Username == Username || user.Mail == Mail)).FirstOrDefault();
            if (User != null) { return true; }
            return false;

        }
        public class UserRead : Users
        {
            public Int32 UserID { get; set; }
        }
        public class Users
        {
            public Users()
            {
                SkillsList = new List<Skills>();
                HobbiesList = new List<Hobbies>();
            }
            public string? UserName { get; set; }

            public string? Password { get; set; }
            public string? Mail { get; set;}
            public string? PhoneNo { get; set; }

            public List<Skills> SkillsList { get; set; }
            public List<Hobbies> HobbiesList { get;set; }

        }


        public class Skills
        {
            public string? SkillName { get; set; }
            public Int32? SkillRatings { get; set; }
        }

        public class Hobbies
        {
            public string? HobbyName { get; set; }
        }
    }
}
