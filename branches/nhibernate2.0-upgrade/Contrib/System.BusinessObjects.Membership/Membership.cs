using System;
using System.Data;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace System.BusinessObjects.Membership
{
    /// <summary>
    /// Membership : BusinessObject
    /// </summary>
    public class Membership : User
    {
        public virtual String Password
        {
            get { return GetValue<String>("Password"); }
            set
            {
                BeginEdit();
                SetValue("Password", value);
            }
        }

        public virtual Int32 PasswordFormat
        {
            get { return GetValue<Int32>("PasswordFormat"); }
            set
            {
                BeginEdit();
                SetValue("PasswordFormat", value);
            }
        }

        public virtual String PasswordSalt
        {
            get { return GetValue<String>("PasswordSalt"); }
            set
            {
                BeginEdit();
                SetValue("PasswordSalt", value);
            }
        }

        public virtual String MobilePIN
        {
            get { return GetValue<String>("MobilePIN"); }
            set
            {
                BeginEdit();
                SetValue("MobilePIN", value);
            }
        }

        public virtual String Email
        {
            get { return GetValue<String>("Email"); }
            set
            {
                BeginEdit();
                SetValue("Email", value);
            }
        }

        public virtual String LoweredEmail
        {
            get { return GetValue<String>("LoweredEmail"); }
            set
            {
                BeginEdit();
                SetValue("LoweredEmail", value);
            }
        }

        public virtual String PasswordQuestion
        {
            get { return GetValue<String>("PasswordQuestion"); }
            set
            {
                BeginEdit();
                SetValue("PasswordQuestion", value);
            }
        }

        public virtual String PasswordAnswer
        {
            get { return GetValue<String>("PasswordAnswer"); }
            set
            {
                BeginEdit();
                SetValue("PasswordAnswer", value);
            }
        }

        public virtual Boolean IsApproved
        {
            get { return GetValue<Boolean>("IsApproved"); }
            set
            {
                BeginEdit();
                SetValue("IsApproved", value);
            }
        }

        public virtual Boolean IsLockedOut
        {
            get { return GetValue<Boolean>("IsLockedOut"); }
            set
            {
                BeginEdit();
                SetValue("IsLockedOut", value);
            }
        }

        public virtual DateTime CreateDate
        {
            get { return GetValue<DateTime>("CreateDate"); }
            set
            {
                BeginEdit();
                SetValue("CreateDate", value);
            }
        }

        public virtual DateTime LastLoginDate
        {
            get { return GetValue<DateTime>("LastLoginDate"); }
            set
            {
                BeginEdit();
                SetValue("LastLoginDate", value);
            }
        }

        public virtual DateTime LastPasswordChangedDate
        {
            get { return GetValue<DateTime>("LastPasswordChangedDate"); }
            set
            {
                BeginEdit();
                SetValue("LastPasswordChangedDate", value);
            }
        }

        public virtual DateTime LastLockoutDate
        {
            get { return GetValue<DateTime>("LastLockoutDate"); }
            set
            {
                BeginEdit();
                SetValue("LastLockoutDate", value);
            }
        }

        public virtual Int32 FailedPasswordAttemptCount
        {
            get { return GetValue<Int32>("FailedPasswordAttemptCount"); }
            set
            {
                BeginEdit();
                SetValue("FailedPasswordAttemptCount", value);
            }
        }

        public virtual DateTime FailedPasswordAttemptWindowStart
        {
            get { return GetValue<DateTime>("FailedPasswordAttemptWindowStart"); }
            set
            {
                BeginEdit();
                SetValue("FailedPasswordAttemptWindowStart", value);
            }
        }

        public virtual Int32 FailedPasswordAnswerAttemptCount
        {
            get { return GetValue<Int32>("FailedPasswordAnswerAttemptCount"); }
            set
            {
                BeginEdit();
                SetValue("FailedPasswordAnswerAttemptCount", value);
            }
        }

        public virtual DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get { return GetValue<DateTime>("FailedPasswordAnswerAttemptWindowStart"); }
            set
            {
                BeginEdit();
                SetValue("FailedPasswordAnswerAttemptWindowStart", value);
            }
        }

        public virtual String Comment
        {
            get { return GetValue<String>("Comment"); }
            set
            {
                BeginEdit();
                SetValue("Comment", value);
            }
        }

        public virtual System.Web.Security.MembershipUser ToMembershipUser(string providerName)
        {
            return (new System.Web.Security.MembershipUser(providerName, UserName, ID, Email, PasswordQuestion, Comment, IsApproved,
                                       IsLockedOut, CreateDate, LastLoginDate, LastActivityDate, LastPasswordChangedDate,
                                       LastLockoutDate));
        }
        public virtual Membership FromMembershipUser(System.Web.Security.MembershipUser mu)
        {
            ID = (Guid)mu.ProviderUserKey;
            Email = mu.Email;
            PasswordQuestion = mu.PasswordQuestion;
            Comment = mu.Comment;
            IsApproved = mu.IsApproved;
            IsLockedOut = mu.IsLockedOut;
            CreateDate = mu.CreationDate;
            UserName = mu.UserName;
            LastActivityDate = mu.LastActivityDate;
            LastLoginDate = mu.LastLoginDate;
            LastPasswordChangedDate = mu.LastPasswordChangedDate;
            LastLockoutDate = mu.LastLockoutDate;
            return this;
        }

    }
}