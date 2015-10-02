using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Configuration.Provider;
using System.Configuration;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Data;

namespace Cykelnet
{
    public class VersionedMembershipProvider : MembershipProvider
    {
        //TODO: Some functions, such as locked users and password questions and answers are not yet supported.
        //TODO: Applicationname is not used in most selects. This should be fixed, but is not critical.
        #region props
        private string connectionString;

        private int maxInvalidPasswordAttempts;
        public override int MaxInvalidPasswordAttempts { get { return maxInvalidPasswordAttempts; } }

        private int minRequiredNonAlphanumericCharacters;
        public override int MinRequiredNonAlphanumericCharacters { get { return minRequiredNonAlphanumericCharacters; } }

        private int minRequiredPasswordLength;
        public override int MinRequiredPasswordLength { get { return minRequiredPasswordLength; } }

        private int passwordAttemptWindow;
        public override int PasswordAttemptWindow { get { return passwordAttemptWindow; } }

        private MembershipPasswordFormat passwordFormat;
        public override MembershipPasswordFormat PasswordFormat { get { return passwordFormat; } }

        private string passwordStrengthRegularExpression;
        public override string PasswordStrengthRegularExpression { get { return passwordStrengthRegularExpression; } }

        private bool requiresQuestionAndAnswer;
        public override bool RequiresQuestionAndAnswer { get { return requiresQuestionAndAnswer; } }

        private bool requiresUniqueEmail;
        public override bool RequiresUniqueEmail { get { return requiresUniqueEmail; } }

        public override string ApplicationName { get; set; }

        public bool WriteExceptionsToEventLog { get; set; }

        private bool enablePasswordReset;
        public override bool EnablePasswordReset { get { return enablePasswordReset; } }

        private bool enablePasswordRetrieval;
        public override bool EnablePasswordRetrieval { get { return enablePasswordReset; } }

        #endregion

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "VersionedMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Versioned Membership provider for Cykelnet");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            this.ApplicationName = GetConfigValue(config["applicationName"],
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "0"));
            minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));

            if (requiresQuestionAndAnswer == true)
                throw new ProviderException("Password questions and answers are not supported by this provider");

            requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            //
            // Get connectionstring
            //

            ConnectionStringSettings ConnectionStringSettings =
              ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;


            // Get encryption and decryption key information from the configuration.
            //Configuration cfg =
            //  WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            //machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");
            //
            //if (machineKey.ValidationKey.Contains("AutoGenerate"))
            //    if (PasswordFormat != MembershipPasswordFormat.Clear)
            //        throw new ProviderException("Hashed or Encrypted passwords " +
            //                                   "are not supported with auto-generated keys.");
        }

        //
        // A helper function to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword) && !passwordConformsWithPolicy(newPassword))
                return false;

            SqlConnection con = new SqlConnection(connectionString);

            byte[] password = hashPassword(newPassword);

            string query = "UPDATE Users SET UserPassword = @password WHERE UserName = @UserName AND ApplicationName = @ApplicationName AND UserDeleteDate IS NULL";

            var command = new SqlCommand(query, con);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@UserName", username);
            command.Parameters.AddWithValue("@ApplicationName", ApplicationName);

            con.Open();
            var result = command.ExecuteNonQuery();
            con.Close();

            if (result == 1)
                return true;
            else
                return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public MembershipUser CreateUser(string username,
                                         string password,
                                         string email,
                                         string FullName,
                                         string Address1,
                                         string Address2,
                                         string Country,
                                         string CyclistType,
                                         DateTime? Birthday,
                                     out MembershipCreateStatus status)
        {
            //TODO: Make a CreateUser function that adds the aditional userdata we require

            if (!passwordConformsWithPolicy(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            //TODO: Add email validation

            SqlConnection con = new SqlConnection(connectionString);
            
            try
            {
                con.Open();
                
                //Check duplicate username
                SqlCommand CheckUsernameCommand = new SqlCommand("SELECT COUNT(UserID) FROM Users WHERE UserName = @UserName AND ApplicationName = @ApplicationName AND UserDeleteDate IS NULL", con);
                CheckUsernameCommand.Parameters.AddWithValue("@UserName", username);
                CheckUsernameCommand.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

                Int32 ExistingUsersWithUsername = (Int32)CheckUsernameCommand.ExecuteScalar();

                if (ExistingUsersWithUsername != 0)
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }

                if (this.RequiresUniqueEmail)
                {
                    SqlCommand CheckEmailCommand = new SqlCommand("SELECT COUNT(UserID) FROM Users WHERE UserEmail = @UserEmail AND ApplicationName = @ApplicationName AND UserDeleteDate IS NULL", con);
                    CheckEmailCommand.Parameters.AddWithValue("@UserEmail", email);
                    CheckEmailCommand.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

                    Int32 ExistingUsersWithEmail = (Int32)CheckEmailCommand.ExecuteScalar();

                    if (ExistingUsersWithUsername != 0)
                    {
                        status = MembershipCreateStatus.DuplicateEmail;
                        return null;
                    }
                }

                SqlCommand InsertUserCommand = new SqlCommand("INSERT INTO Users (UserName,UserPassword,UserEmail,UserFullName,UserAddress1,UserAddress2,UserCountry,UserCyclistType,UserBirthday,ApplicationName) VALUES (@UserName,@UserPassword,@UserEmail,@UserFullName,@UserAddress1,@UserAddress2,@UserCountry,@UserCyclistType,@UserBirthday,@ApplicationName)", con);

                InsertUserCommand.Parameters.AddWithValue("@UserName", username);
                InsertUserCommand.Parameters.AddWithValue("@UserPassword", hashPassword(password));
                InsertUserCommand.Parameters.AddWithValue("@UserEmail", email);
                InsertUserCommand.Parameters.AddWithValue("@UserFullName", FullName);
                InsertUserCommand.Parameters.AddWithValue("@UserAddress1", Address1);
                InsertUserCommand.Parameters.AddWithValue("@UserAddress2", Address2);
                InsertUserCommand.Parameters.AddWithValue("@UserCountry", Country);
                InsertUserCommand.Parameters.AddWithValue("@UserCyclistType", CyclistType);
                InsertUserCommand.Parameters.AddWithValue("@UserBirthday", Birthday);
                InsertUserCommand.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

                //We must ensure that if any values are null, then it is the correct null!
                foreach(IDataParameter param in InsertUserCommand.Parameters){
                    if (param.Value == null) param.Value = DBNull.Value; 
                    }

                InsertUserCommand.ExecuteNonQuery();
            }
            catch
            {
                status = MembershipCreateStatus.ProviderError;

                return null;
            }
            finally
            {
                con.Close();
            }

            status = MembershipCreateStatus.Success;
            return GetUser(username, false);
        }

        public override MembershipUser CreateUser(string username, 
            string password, 
            string email, 
            string passwordQuestion, 
            string passwordAnswer, 
            bool isApproved, 
            object providerUserKey, 
            out MembershipCreateStatus status)
        {
            return CreateUser(username, password, email, null, null, null, null, null, null, out status);
        }

        /// <summary>
        /// Deletes a user. No userdate is ever actually removed, so this just updates the deleteDate column
        /// </summary>
        /// <param name="username">The username of the user to delete</param>
        /// <param name="deleteAllRelatedData">Completely ignored</param>
        /// <returns>True if the user was succesfully deleted, otherwise null</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                SqlCommand command = new SqlCommand("UPDATE Users SET UserDeleteDate = GETDATE() WHERE UserName = @username AND ApplicationName = @ApplicationName", con);

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

                command.ExecuteNonQuery();

                command = new SqlCommand("UPDATE OldUsers SET UserDeleteDate = GETDATE() WHERE UserName = @username AND ApplicationName = @ApplicationName", con);

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

                command.ExecuteNonQuery();

            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
            return true;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return getUsersLimited("UserEmail", emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return getUsersLimited("UserFullName", usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return getUsersLimited(null, null, pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfUsersOnline()
        {
            string query = "SELECT COUNT(UserID) FROM Users WHERE DATEDIFF(MI,LastActivityDate,GETDATE()) < @OnlineWindow AND ApplicationName = @ApplicationName AND UserDeleteDate IS NULL";

            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, con);

            command.Parameters.AddWithValue("@OnlineWindow", Membership.UserIsOnlineTimeWindow);
            command.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

            con.Open();

            int usersOnline = (Int32)command.ExecuteScalar();

            con.Close();

            return usersOnline;
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand command = new SqlCommand("SELECT UserID FROM Users WHERE UserName = @UserName AND ApplicationName = @ApplicationName AND UserDeleteDate IS NULL", con);

            command.Parameters.AddWithValue("@UserName", username);
            command.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

            con.Open();
            Guid userID = (Guid)command.ExecuteScalar();

            con.Close();

            return userID != null ? GetUser(userID, userIsOnline) : null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            SqlConnection con = new SqlConnection(connectionString);

	        SqlCommand SelectCommand = new SqlCommand("SELECT [UserID]" +       //0
                                                      ",[UserName]" +           //1
                                                      ",[UserPassword]" +       //2 Not needed, forgot to remove it
                                                      ",[UserEmail]" +          //3
                                                      ",[UserCyclistType]" +    //4    
                                                      ",[UserFullName]" +       //5
                                                      ",[UserAddress1]" +       //6
                                                      ",[UserAddress2]" +       //7
                                                      ",[UserCountry]" +        //8
                                                      ",[UserBirthday]" +       //9
                                                      ",[LastActivityDate]" +   //10
                                                      ",[CreationDate] " +      //11 
                                                      "FROM Users WHERE UserID = @UserID AND UserDeleteDate IS NULL AND ApplicationName = @ApplicationName", con);
			    SelectCommand.Parameters.AddWithValue("@UserID", providerUserKey);
            SelectCommand.Parameters.AddWithValue("@ApplicationName", ApplicationName);

            con.Open();

            var reader = SelectCommand.ExecuteReader();

            if (!reader.HasRows)
                return null;

            reader.Read(); //Only one user per key

            string providername = this.Name;
            string username = reader.IsDBNull(1) ? "Silly Hacker" : reader.GetString(1);
            Guid providerUserID = reader.IsDBNull(0) ? new Guid() : reader.GetGuid(0);
            string email = reader.IsDBNull(3) ? "hacker@badsite.com" : reader.GetString(3);
            string passwordQuestion = null;
            string comment = null;
            bool isApproved = true;
            bool isLockedOut = false;
            DateTime creationDate = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11);
            DateTime lastLoginDate = DateTime.MinValue;
            DateTime lastActivityDate = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10);
            DateTime lastPasswordChangedDate = DateTime.MinValue;
            DateTime lastLockedOutDate = DateTime.MinValue;
            string FullName = reader.IsDBNull(5) ? null : reader.GetString(5);
            string Address1 = reader.IsDBNull(6) ? null : reader.GetString(6);
            string Address2 = reader.IsDBNull(7) ? null : reader.GetString(7);
            string Country = reader.IsDBNull(8) ? null : reader.GetString(8);
            string CyclistType = reader.IsDBNull(4) ? null : reader.GetString(4);
            DateTime Birthday = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9);

            VersionedUser user = new VersionedUser(providername,
                                   username,
                                   providerUserKey,
                                   email,
                                   passwordQuestion,
                                   comment,
                                   isApproved,
                                   isLockedOut,
                                   creationDate,
                                   lastLoginDate,
                                   lastActivityDate,
                                   lastPasswordChangedDate,
                                   lastLockedOutDate,
                                   FullName,
                                   Address1,
                                   Address2,
                                   Country,
                                   CyclistType,
                                   Birthday);

            if (userIsOnline)
            {
                SqlCommand UpdateCommand = new SqlCommand("UPDATE Users SET LastActivityDate = getdate() WHERE UserID = @UserID", con);
                UpdateCommand.Parameters.AddWithValue("@UserID", providerUserID);

                UpdateCommand.ExecuteNonQuery();
            }

            con.Close();

            return user;
        }

        public override string GetUserNameByEmail(string email)
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand command = new SqlCommand("SELECT UserName FROM Users WHERE UserEmail = @UserEmail", con);

            command.Parameters.AddWithValue("@UserEmail", email);

            con.Open();
            string username = (string)command.ExecuteScalar();
            con.Close();

            return username;
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            VersionedUser vUser = (VersionedUser)user;

            //Get the creationdate and hashed password of the user
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand selectCommand = new SqlCommand("SELECT UserPassword,CreationDate From Users WHERE UserID = @UserID AND ApplicationName = @ApplicationName", con);

            selectCommand.Parameters.AddWithValue("@UserID", (Guid)user.ProviderUserKey);
            selectCommand.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

            con.Open();

            var reader = selectCommand.ExecuteReader();

            reader.Read();

            Byte[] password = new Byte[512]; 
            reader.GetBytes(0,0,password,0,512);
            DateTime creationDate = reader.GetDateTime(1);
            reader.Close();


            //Move the old userdata to the dump table
            string moveQuery = "INSERT INTO OldUsers (UserID, UserName, UserPassword, UserEmail, ApplicationName, " +
            "UserCyclistType, UserFullName, UserAddress1, UserAddress2, UserCountry, " +
            "UserBirthday, UserDeleteDate, ValidFrom, LastActivityDate, CreationDate) " +
            "SELECT [UserID],[UserName],[UserPassword],[UserEmail],[ApplicationName],[UserCyclistType],[UserFullName]," +
            "[UserAddress1],[UserAddress2],[UserCountry],[UserBirthday],[UserDeleteDate],[ValidFrom],[LastActivityDate],[CreationDate] " +
            "FROM Users WHERE UserID = @UserID AND ApplicationName = @ApplicationName";

            SqlCommand moveCommand = new SqlCommand(moveQuery, con);

            moveCommand.Parameters.AddWithValue("@UserID", vUser.ProviderUserKey);
            moveCommand.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

            moveCommand.ExecuteNonQuery();


            //Insert the updated info

            string updateQuery = "UPDATE [Cykelnet].[dbo].[Users] SET [UserEmail] = @Email," +
            "[UserCyclistType] = @CyclistType,[UserFullName] = @FullName,[UserAddress1] = @Address1,[UserAddress2] = @Address2," + 
            "[UserCountry] = @Country,[UserBirthday] = @Birthday,[CreationDate] = @CreationDate WHERE UserID = @UserID AND ApplicationName = @ApplicationName";

            SqlCommand updateCommand = new SqlCommand(updateQuery, con);

            updateCommand.Parameters.AddWithValue("@UserID", (Guid)user.ProviderUserKey);
            updateCommand.Parameters.AddWithValue("@Email", vUser.Email);
            updateCommand.Parameters.AddWithValue("@ApplicationName", ApplicationName);
            updateCommand.Parameters.AddWithValue("@CyclistType", vUser.CyclistType);
            updateCommand.Parameters.AddWithValue("@FullName", vUser.FullName);
            updateCommand.Parameters.AddWithValue("@Address1", vUser.Address1 != null ? (object)vUser.Address1 : (object)DBNull.Value);
            updateCommand.Parameters.AddWithValue("@Address2", vUser.Address2 != null ? (object)vUser.Address2 : (object)DBNull.Value);
            updateCommand.Parameters.AddWithValue("@Country", vUser.Country != null ? (object)vUser.Country : (object)DBNull.Value);
            updateCommand.Parameters.AddWithValue("@Birthday", vUser.Birthday);
            updateCommand.Parameters.AddWithValue("@CreationDate", creationDate);

            updateCommand.ExecuteNonQuery();

            con.Close();
        }

        public override bool ValidateUser(string username, string password)
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand command = new SqlCommand("SELECT COUNT(UserId) FROM Users WHERE UserName = @UserName AND UserPassword = @UserPassword AND ApplicationName = @ApplicationName", con);

            command.Parameters.AddWithValue("@UserName", username);
            command.Parameters.AddWithValue("@UserPassword", hashPassword(password));
            command.Parameters.AddWithValue("@ApplicationName", this.ApplicationName);

            con.Open();
            int usersThatMatch = (Int32)command.ExecuteScalar();
            con.Close();

            return usersThatMatch == 1 ? true : false;
        }

        private byte[] hashPassword(string password)
        {
            var crypt = SHA512.Create();

            return crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool passwordConformsWithPolicy(string password)
        {
            if (password.Length < this.MinRequiredPasswordLength)
            {
                return false;
            }

            int nonAlphaChars = 0;

            foreach (char a in password)
            {
                if (Char.IsDigit(a))
                    nonAlphaChars++;
            }

            if (nonAlphaChars < this.MinRequiredNonAlphanumericCharacters)
                return false;


            return true;
        }

        /// <summary>
        /// Selects users limited by some colums value. If null is provided as columnToMatch, all users are selected.
        /// </summary>
        /// <param name="columnToMatch">The column to which the value is matched. If this is null, all users are selected</param>
        /// <param name="valueToMatch">The value to match. the LIKE comparison is used, making room for wildcard use.</param>
        /// <param name="pageIndex">The page index where 0 is the first page</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="totalRecords">The total number of records selected</param>
        /// <returns></returns>
        private MembershipUserCollection getUsersLimited(string columnToMatch, string valueToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            int pageStartId = pageIndex == 0 ? 1 : pageIndex * pageSize;
            string query = "";

            if (pageStartId == 1)
            {
                query += "SELECT TOP ";
                query += pageSize;
                query += " UserID FROM Users";
                query += " WHERE ApplicationName = @ApplicationName AND UserDeleteDate IS NULL";
                
                if (columnToMatch != null)
                {
                    query += " AND ";
                    query += columnToMatch;
                    query += " LIKE ";
                    query += "'" + valueToMatch + "'";
                }
            }
            else
            {
                query += "SELECT UserID from (SELECT TOP ";
                query += pageSize;
                query += " UserID from (SELECT TOP ";
                query += pageStartId + pageSize;
                query += " UserID FROM Users ";
                query += "WHERE ApplicationName = @ApplicationName AND UserDeleteDate IS NULL";
                if (columnToMatch != null)
                {
                    query += " AND ";
                    query += columnToMatch;
                    query += " LIKE ";
                    query += "'" + valueToMatch + "'";
                }

                query += " ORDER BY ";
                query += columnToMatch != null ? columnToMatch : "UserID";
                query += " ASC) AS innerTable ORDER BY ";
                query += columnToMatch != null ? columnToMatch : "UserID";
                query += " DESC) AS outerTable ORDER BY ";
                query += columnToMatch != null ? columnToMatch : "UserID";
                query += " ASC";
            }

            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, con);
            List<Guid> userIDs = new List<Guid>();

            command.Parameters.AddWithValue("@ApplicationName",ApplicationName);

            con.Open();

            var result = command.ExecuteReader();

            while (result.Read())
            {
                userIDs.Add(result.GetGuid(0));
            }

            result.Close();
            con.Close();

            totalRecords = userIDs.Count;

            MembershipUserCollection users = new MembershipUserCollection();

            foreach (Guid userID in userIDs)
            {
                users.Add(GetUser(userID, false));
            }


            return users;
        }

    }
}