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
    /// <summary>
    /// I prefer to use method name instead of verb
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        static CDNContext cdnCtxt = new();

        public UserController()
        {
            cdnCtxt = new CDNContext();
        }
        /// <summary>
        /// List all user
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Return the profile of the current user, required user logged in
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public Profile GetProfile(Int32 UserID)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity.Name != null)
            {
                var CurrentUserID = identity.FindFirst("Id").Value; //var CurrentUserID = identity.FindFirst("Id").Value; For security purpose, we can compare the Id stored in claims, during user edit or delete
            }

            var user = cdnCtxt.Users.Where(user => user.UserId== UserID).FirstOrDefault();
            if (user == null) return null;
            var myProfile = new Profile { UserName = user.Username, Mail = user.Mail, PhoneNo = user.PhoneNo };
            var skills = cdnCtxt.Skills.AsNoTracking().Where(d => d.UserId == UserID).ToList();
            var hobbies = cdnCtxt.Hobbies.AsNoTracking().Where(d => d.UserId == UserID).ToList();
            if (skills != null)
            {
                System.Text.StringBuilder skillbuilder = new System.Text.StringBuilder(); //For the read only, I use StringBuilder to concat all the skills,hobbies
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
        /// <summary>
        /// For editing the profile
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public UserRead GetProfileEdit(Int32 UserID)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity.Name != null)
            {
                var CurrentUserID = identity.FindFirst("Id").Value; //var CurrentUserID = identity.FindFirst("Id").Value; For security purpose, we can compare the Id stored in claims, during user edit or delete
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

        /// <summary>
        /// Return the list of user. I planned to only display "followers"
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public List<User> GetUserList(Int32 UserID)
        {
            var listUser = new List<User>();
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
        /// <summary>
        /// For user registration. Use the hashing to secure the password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
            cdnCtxt.SaveChanges(); //SaveChanges here so that we can get the UserId
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

        /// <summary>
        /// Delete the user. Require username,password so user need confirm the process
        /// </summary>
        /// <param name="UserToDelete"></param>
        /// <returns></returns>
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
            //New feature in EF 7.0
            cdnCtxt.Skills.Where(u => u.UserId == user.UserId).ExecuteDelete();
            cdnCtxt.Hobbies.Where(u => u.UserId == user.UserId).ExecuteDelete();
            cdnCtxt.Users.Where(u => u.UserId == user.UserId).ExecuteDelete();
            return new rtnDeleteUser { Success = true, ErrorMsg = "" };
        }

        /// <summary>
        /// Update current user. Check wether the username,email conflict with existing user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
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
        public class UserRegStatus
        {
            public bool Success { get; set; }
            public string ErrMsg { get; set; }
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
            public string? Mail { get; set; }
            public string? PhoneNo { get; set; }

            public List<Skills> SkillsList { get; set; }
            public List<Hobbies> HobbiesList { get; set; }

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

/// <summary>
/// For verifying the user, should put in pubFunc
/// </summary>
/// <param name="Username"></param>
/// <param name="Mail"></param>
/// <returns></returns>
        internal bool CheckUserExistByUsernameEmail(string? Username, string? Mail)
        {

            var User = cdnCtxt.Users.AsNoTracking().Where(user => user.Username == Username || user.Mail == Mail).FirstOrDefault();
            if (User != null) { return true; }
            return false;

        }

        /// <summary>
        /// For verifying the CURRENT user, should put in pubFunc
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Username"></param>
        /// <param name="Mail"></param>
        /// <returns></returns>
        internal bool CheckUserExistByUsernameEmail(Int32 UserId,string? Username, string? Mail)
        {

            var User = cdnCtxt.Users.AsNoTracking().Where(user => user.UserId != UserId && (user.Username == Username || user.Mail == Mail)).FirstOrDefault();
            if (User != null) { return true; }
            return false;

        }

    }
}
