using TracerDemo.Data;
using TracerDemo.Model;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TracerDemo.Helpers
{
  public class UserHelper
  {
    private SqliteContext db { get; set; }
    private HasherHelper Hasher { get; set; }
    public UserHelper(SqliteContext context, HasherHelper hasher)
    {
      db = context;
      this.Hasher = hasher;
    }

    /// <summary>
    /// Retrieve user by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public User GetUserById(string id)
    {
      User user = db.Users.Where(u => u.Id == id).FirstOrDefault();
      if (user != null)
        return user;
      else
        return default(User);
    }

    /// <summary>
    /// Retrieve user by email address
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public User GetUserByEmail(string email)
    {
      User user = db.Users.Where(u => u.Email == email).FirstOrDefault();
      if (user != null)
        return user;
      else
        return default(User);
    }

    /// <summary>
    /// Validate user and save to DB
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool CreateUser(User user)
    {
      if (GetUserByEmail(user.Email) != null) //User email address must be unique
        return false;
      else if (!IsValidPassword(user.Password)) //User password must meet complexity requirements
        return false;

      db.Users.Add(user);

      return true;
    }

    public UserSlim UserToUserSlim(User user)
    {
      return new UserSlim()
      {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        LastSignin = user.LastSignin,
        Created = user.Created,
        Updated = user.Updated,
        Roles = user.Roles
      };
    }

    /// <summary>
    /// Validate usernamd and password
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public UserValidationResponse ValidateUserIdentity(string username, string password, ref User userResult, string facebookToken = null)
    {
      if (username != null)
        username = username.ToLower();
      User user = db.Users.Where(u => u.Email.ToLower() == username).FirstOrDefault();

      if (facebookToken != null)
      {
        user = FacebookAuthentication(facebookToken).Result;
        userResult = user;
      }


      if (user == null)
      {
        return UserValidationResponse.Invalid;
      }
      else if (user.EmailValidated == false)
      {
        return UserValidationResponse.Invalidated;
      }
      else if (PasswordMatch(user, password) == false && facebookToken == null)
      {
        user.LockoutCount++;
        db.Users.Update(user);

        if (user.LockoutCount >= 10)
        {
          user.LockoutDateTime = DateTime.Now.ToUniversalTime();
          db.Users.Update(user);

          return UserValidationResponse.LockedOut;
        }

        return UserValidationResponse.Invalid;
      }
      else
      {
        if (user.LockoutCount >= 10 && user.LockoutDateTime <= DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToUniversalTime())
        {
          user.LockoutCount = 0;
          db.Users.Update(user);
          userResult = user;
          return UserValidationResponse.Validated;
        }

        else if (user.LockoutCount >= 10 && user.LockoutDateTime > DateTime.Now.Subtract(new TimeSpan(0, 30, 0)))
        {
          return UserValidationResponse.LockedOut;
        }
        else
        {
          if(user.LockoutCount >= 0)
          {
            user.LockoutCount = 0;
            db.Users.Update(user);
          }
          userResult = user;
          return UserValidationResponse.Validated;
        }
      }
    }

    /// <summary>
    /// Access token is provided from the authentication process - the client application will do the authenication, and provide the accessToken to the API.
    /// </summary>
    /// <param name="facebookAccessToken"></param>
    /// <returns></returns>
    async public Task<User> FacebookAuthentication(string facebookAccessToken)
    {
      try
      {
        //This link was helpful in making the graph.facebook.com api call with the facebook access token.
        //https://blogs.msdn.microsoft.com/nickpinheiro/2015/02/28/facebook-login-with-asp-net-web-forms/


        // Request the Facebook user information, using the accessToken
        Uri targetUserUri = new Uri("https://graph.facebook.com/me?fields=id,first_name,last_name,email,locale&access_token=" + facebookAccessToken);
        HttpClient client = new HttpClient();
        var facebookResult = await client.GetAsync(targetUserUri);
        var response = await facebookResult.Content.ReadAsStringAsync();
        FacebookApiResult result = JsonConvert.DeserializeObject<FacebookApiResult>(response);

        //Does the user already exist - facebook will be added to it.
        User user = GetUserByEmail(result.email);
        if (user != null)
        {
            user.FacebookId = result.id;
            user.ActivationToken = null;
            user.ActivationTokenExpiration = null;
            user.EmailValidated = true;
            db.Users.Update(user);
        }
        else if (user == null)
        {
          //If no facebook user registered, create a new one
          if (result.id != null)//Do this to ensure that a response was found from the facebook api

            user = db.Users.Where(u => u.FacebookId == result.id).FirstOrDefault();
          if (user == null)
          {
            //Facebook profile must have an email address associated with it.
            if (result.email == null)
            {
              return default(User);
            }

            //Is the user already registered?
            User existingUser = GetUserByEmail(result.email);
            if (existingUser != null)
            {
              existingUser.FacebookId = result.id;
              user = existingUser;
            }
            else
            {
              //Create a new user
              user = new User()
              {
                Email = result.email,
                FacebookId = result.id,
                EmailValidated = true
              };
              Rol r = new Rol();
              r.value = "Trial";
              user.Roles.Add(r);
              db.Users.Add(user);
            }


            if (existingUser != null)
            {
               return default(User);
            }
          }
        }

        user = db.Users.Where(u => u.FacebookId == result.id).FirstOrDefault();
        if (user == null)
          return default(User);

        if (user.EmailValidated == false)
        {
            user.FacebookId = result.id;
            user.ActivationToken = null;
            user.ActivationTokenExpiration = null;
            user.EmailValidated = true;
            db.Users.Update(user);
        }

        return user;
      }
      catch
      {
        return default(User);
      }
    }

    /// <summary>
    /// Test password for complexity 
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool IsValidPassword(string password)
    {
      return true;
    }

    /// <summary>
    /// Compare password hashes
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool PasswordMatch(User user, string password)
    {
      string hash = Hasher.GetHash(password + user.Salt);

      if (user.Password == hash)
        return true;
      else
        return false;
    }

    public string CreatUserSalt()
    {
      var random = RandomNumberGenerator.Create();// new RNGCryptoServiceProvider();

      // Maximum length of salt
      int max_length = 128;

      // Empty salt array
      byte[] salt = new byte[max_length];

      // Build the random bytes
      random.GetBytes(salt);//.GetNonZeroBytes(salt);

      // Return the string encoded salt
      return Convert.ToBase64String(salt); ;
    }




  }
}
