﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using HabitatForHumanity.ViewModels;
using System.Data.Entity;
using System.Web.Helpers;
using System.Collections.Generic;

namespace HabitatForHumanity.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string homePhoneNumber { get; set; }
        public string workPhoneNumber { get; set; }
        public string emailAddress { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string password { get; set; }
        public DateTime birthDate { get; set; }
        public string gender { get; set; }
        /// <summary>
        /// 0 - volunteer, 1 - admin
        /// </summary>
        public int isAdmin { get; set; }
        public DateTime waiverSignDate { get; set; }
        public string emergencyFirstName { get; set; }
        public string emergencyLastName { get; set; }
        public string relation { get; set; }
        public string emergencyHomePhone { get; set; }
        public string emergencyWorkPhone { get; set; }
        public string emergencyStreetAddress { get; set; }
        public string emergencyCity { get; set; }
        public string emergencyZip { get; set; }



        #region Database Access Methods





        /// <summary>
        /// Checks whether the user entered a bad password for that log in email.
        /// </summary>
        /// <param name="loginVm">The viewmodel containing the users email and password.</param>
        /// <returns>ReturnStatus object that contains true if user entered a correct password.</returns>
        public static ReturnStatus AuthenticateUser(LoginVM loginVm)
        {
            ReturnStatus st = new ReturnStatus();

            try
            {
                bool exists = false;

                st = User.GetUserByEmail(loginVm.email);

                //check status to make sure error code and data are correct
                if (ReturnStatus.tryParseUser(st, out User user))
                {
                    if (user != null && Crypto.VerifyHashedPassword(user.password, loginVm.password))
                    {
                        exists = true;
                    }
                }

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = exists;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_AUTHENTICATE_USER;
                st.data = "Failed to authenticate user.";
                st.errorMessage = e.ToString();
                return st;
            }
        }


        /// <summary>
        /// Gets all the users with matching names. To be used when you know one name, but not the other. 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>ReturnStatus object containing a list of users</returns>
        public static ReturnStatus GetUsersByName(string firstName, string lastName)
        {
            ReturnStatus st = new ReturnStatus();

            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.data = db.users.Where(x => x.firstName.Equals(firstName) || x.lastName.Equals(lastName)).ToList();
                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database.";
                return st;
            }

        }


        /// <summary>
        /// Get a single user out of the database with a matching first and last name.
        /// Only to be used when you know the exact names
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>Id of the returned user</returns>
        public static ReturnStatus GetUserByName(string firstName, string lastName)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                var userCount = db.users.Count(x => x.firstName.Equals(firstName) && x.lastName.Equals(lastName));

                //if no users are found or if multiple users are found
                if (userCount != 1)
                {
                    st.errorCode = -1;
                    st.data = "More than one user found.";
                    return st;
                }

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.users.Where(x => x.firstName.Equals(firstName) && x.lastName.Equals(lastName)).Single();

                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_FIND_SINGLE_USER;
                st.data = "";
                st.errorMessage = e.ToString();
                return st;
            }
        }



        /// <summary>
        /// Finds email if it exists in the database.
        /// </summary>
        /// <param name="email">Email to search for.</param>
        /// <returns>True if email exists</returns>
        public static ReturnStatus EmailExists(string email)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.users.Any(u => u.emailAddress.Equals(email));
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_FIND_EMAIL;
                st.data = "Could not find user with that email address";
                st.errorMessage = e.ToString();
                return st;
            }
        }


        ///// <summary>
        ///// Finds email if it exists in the database.
        ///// </summary>
        ///// <param name="email">Email to search for.</param>
        ///// <returns>True if email exists</returns>
        //public static bool EmailExists(string email)
        //{
        //    VolunteerDbContext db = new VolunteerDbContext();
        //   return db.users.Any(u => u.emailAddress.Equals(email));
        //}

        /// <summary>
        /// Gets the user in the database with the matching email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>User with matching email address.</returns>
        public static ReturnStatus GetUserByEmail(string email)
        {
            ReturnStatus st = new ReturnStatus();

            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                var users = db.users.Where(u => u.emailAddress.Equals(email));

                st.errorCode = 0;
                st.data = users.FirstOrDefault();

                return st;

            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }


        /// <summary>
        /// Gets the user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ReturnStatus GetUser(int id)
        {
            ReturnStatus st = new ReturnStatus();

            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                //VolunteerDbContext db = null;

                st.data = db.users.Find(id);
                st.errorCode = 0;

                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }



        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="user">User to add.</param>
        /// <returns>The id of the user or 0 if no user could be added.</returns>
        public static ReturnStatus CreateUser(User user)
        {
            ReturnStatus st = new ReturnStatus();

            try
            {
                user.password = Crypto.HashPassword(user.password);
                user.waiverSignDate = DateTime.Today;

                VolunteerDbContext db = new VolunteerDbContext();
                db.users.Add(user);
                db.SaveChanges();

                //entity framework automagically populates a model with all database generated ids
                //so the passed in user object will have an id
                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = user.Id;



                //var users = db.users.Where(u => u.emailAddress.Equals(user.emailAddress));
                //User newUser = users.FirstOrDefault();
                //if (newUser != null)
                //{
                //    userId = newUser.Id;
                //}
                //return userId;

                return st;

            }
            catch (ArgumentNullException e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.USER_PASSWORD_CANNOT_BE_NULL;
                st.errorMessage = e.ToString();
                st.data = "Password is a required field.";
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                st.data = "Could not connect to database.";
                return st;
            }
        }

        ///// <summary>
        ///// Adds a user to the database.
        ///// </summary>
        ///// <param name="user">User to add.</param>
        ///// <returns>The id of the user or 0 if no user could be added.</returns>
        //public static int CreateUser(User user)
        //{
        //    int userId = 0;
        //    VolunteerDbContext db = new VolunteerDbContext();
        //    db.users.Add(user);
        //    db.SaveChanges();

        //    var users = db.users.Where(u => u.emailAddress.Equals(user.emailAddress));
        //    User newUser = users.FirstOrDefault();
        //    if (newUser != null)
        //    {
        //        userId = newUser.Id;
        //    }
        //    return userId;
        //}


        /// <summary>
        /// Updates the users information based on a new model.
        /// </summary>
        /// <param name="user">User object with new information.</param>
        public static ReturnStatus EditUser(User user)
        {
            ReturnStatus st = new ReturnStatus();

            try
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;

                VolunteerDbContext db = new VolunteerDbContext();
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }


        /// <summary>
        /// Deletes the user from the database.
        /// </summary>
        /// <param name="user">The user object to be deleted.</param>
        public static ReturnStatus DeleteUser(User user)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.users.Attach(user);
                db.users.Remove(user);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = "Successfully deleted user.";
                return st;

            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Deletes the user in the database with matching id.
        /// </summary>
        /// <param name="id"></param>
        public static ReturnStatus DeleteUserById(int id)
        {
            ReturnStatus st = new ReturnStatus();

            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                User user = db.users.Find(id);
                if (user != null)
                {
                    db.users.Remove(user);
                    db.SaveChanges();

                    st.data = "Successfully deleted user.";
                }
                else
                {
                    st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_DELETE;
                    st.data = "Could not delete user.";
                }
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.data = "Could not connect to database.";
                st.errorMessage = e.ToString();
                return st;
            }
        }

        public class Demog
        {
            public string ageBracket { get; set; }
            public int numPeople { get; set; }
        }
        public static List<Demog> GetDemographicsForPie(string gender)
        {
            VolunteerDbContext db = new VolunteerDbContext();
            List<Demog> demogs = new List<Demog>();
            List<User> users = new List<User>();
            if (gender.Equals("All"))
            {
                users = db.users.Where(u => u.birthDate != null).ToList();
            }
            else
            {
                users = db.users.Where(u => u.birthDate != null && u.gender.Equals(gender)).ToList();
            }
            Demog dunder18 = new Demog() { ageBracket = "Under 18", numPeople = 0 };
            Demog d18to27 = new Demog() { ageBracket = "18 to 27", numPeople = 0 };
            Demog d27to40 = new Demog() { ageBracket = "27 to 40", numPeople = 0 };
            Demog d40to55 = new Demog() { ageBracket = "40 to 55", numPeople = 0 };
            Demog dover55 = new Demog() { ageBracket = "Over 55", numPeople = 0 };
            foreach (User u in users)
            {
                DateTime present = DateTime.Now;
                if (present.AddYears(-18) < u.birthDate)
                {
                    dunder18.numPeople++;
                }
                else if (present.AddYears(-18) <= u.birthDate && present.AddYears(-27) > u.birthDate)
                {
                    d18to27.numPeople++;
                }
                else if (present.AddYears(-27) <= u.birthDate && present.AddYears(-40) > u.birthDate)
                {
                    d27to40.numPeople++;
                }
                else if (present.AddYears(-40) <= u.birthDate && present.AddYears(-55) > u.birthDate)
                {
                    d40to55.numPeople++;
                }
                else
                {
                    dover55.numPeople++;
                }
            }

            demogs.Add(dunder18);
            demogs.Add(d18to27);
            demogs.Add(d27to40);
            demogs.Add(d40to55);
            demogs.Add(dover55);
            return demogs;

        }
        /*TimeSheet temp = new TimeSheet();
            VolunteerDbContext db = new VolunteerDbContext();
            var sheets = from t in db.timeSheets
                         group t by t.user_Id into g
                         select g.OrderByDescending(t => t.clockInTime).FirstOrDefault();
            if (sheets.Count() > 0)
            {
                temp = sheets.First();
                // only if the clockout is midnight today(tomorrow really)
                if(temp.clockOutTime == DateTime.Today.AddDays(1))
                {
                    return temp;
                }
            }
            return new TimeSheet();*/
        #endregion
    }
}