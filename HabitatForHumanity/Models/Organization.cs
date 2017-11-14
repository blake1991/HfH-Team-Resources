﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    [Table("Organization")]
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }

        public Organization()
        {
            Id = -1;
            name = "";
        }


        #region Database Access Methods

        /// <summary>
        /// Get all organizations in the database.
        /// </summary>
        /// <returns>A list of all organizations.</returns>
        public static ReturnStatus GetAllOrganizations()
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.organizations.ToList();
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
        /// Get a single organization by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A single organization object with a matching id otherwise null.</returns>
        public static ReturnStatus GetOrganizationById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.organizations.Find(id);
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
        /// Get a single organization by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A single organization object with a matching name otherwise null.</returns>
        public static ReturnStatus GetOrganizationByName(string name)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = db.organizations.Where(x => x.name.Equals(name)).Single();
                return st;
            }
            catch (Exception e)
            {
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_CONNECT_TO_DATABASE;
                st.errorMessage = e.ToString();
                return st;
            }
        }

        /// <summary>
        /// Adds an organization to the database.
        /// </summary>
        /// <param name="org">The organization to be added</param>
        public static ReturnStatus AddOrganization(Organization org)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.organizations.Add(org);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = "Successfully added organization.";
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
        /// Edits the organization with new values.
        /// </summary>
        /// <param name="org">The organization object with new values.</param>
        public static ReturnStatus EditOrganization(Organization org)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.Entry(org).State = EntityState.Modified;
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = "Successfully edited organization.";
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
        /// Deletes an organization from the database.
        /// </summary>
        /// <param name="org">The organization object to delete</param>
        public static ReturnStatus DeleteOrganization(Organization org)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                db.organizations.Attach(org);
                db.organizations.Remove(org);
                db.SaveChanges();

                st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                st.data = "Successfully deleted the organization.";
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
        /// Deletes an organization from the database by id.
        /// </summary>
        /// <param name="id">The id of the organization to delete.</param>
        public static ReturnStatus DeleteOrganizationById(int id)
        {
            ReturnStatus st = new ReturnStatus();
            try
            {
                VolunteerDbContext db = new VolunteerDbContext();
                Organization org = db.organizations.Find(id);
                if (org != null)
                {
                    db.organizations.Remove(org);
                    db.SaveChanges();

                    st.errorCode = (int)ReturnStatus.ErrorCodes.All_CLEAR;
                    st.data = "Successfully deleted the organization.";
                    return st;
                }
                st.errorCode = (int)ReturnStatus.ErrorCodes.COULD_NOT_DELETE;
                st.data = "Could not delete the organization with the supplied id.";
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
        #endregion
    }
}