using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using GuildfordBoroughCouncil.ActiveDirectory.Interfaces;

namespace GuildfordBoroughCouncil.ActiveDirectory
{
    #region Concrete classes for JSON deserialisation

    internal class User : IUser
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string UserName { get; set; }
        public string UserNameX { get; set; }

        public string Department { get; set; }
        public int DirectReports { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ManagerUserName { get; set; }
        public string ManagerUserNameX { get; set; }
        public string Office { get; set; }
        public string Telephone { get; set; }
        public string TelephoneXtn { get; set; }
        public string Title { get; set; }
    }

    internal class Group : IGroup
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string UserName { get; set; }
        public string UserNameX { get; set; }

        public string Description { get; set; }
    }
    #endregion

    public class Lookup
    {
        private static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = Properties.Settings.Default.ActiveDirectoryServiceUri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public static async Task<IEnumerable<IUser>> Councillors(string query = null)
        {
            using (var client = GetClient())
            {
                var response = client.GetAsync("v1/users/councillors?q=" + query).Result;

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<User>>();
                }
                else
                {
                    return new List<IUser>();
                }
            } 
        }

        public static async Task<IUser> User(string SamAccountName)
        {
            if (!String.IsNullOrWhiteSpace(SamAccountName))
            {
                using (var client = GetClient())
                {
                    var response = client.GetAsync("v1/users/" + SamAccountName).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var Users = await response.Content.ReadAsAsync<IEnumerable<User>>();
                        return Users.FirstOrDefault() ?? new User();
                    }
                }
            }

            return null;
        }

        public static async Task<IEnumerable<IUser>> GroupMembers(string SamAccountName)
        {
            if (!String.IsNullOrWhiteSpace(SamAccountName))
            {
                using (var client = GetClient())
                {
                    var response = client.GetAsync("v1/groups/" + SamAccountName + "/members").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<IEnumerable<User>>();
                    }
                }
            }

            return null;
        }

        public static async Task<IEnumerable<string>> GroupSamAccountNamesForUser(string SamAccountName, bool Follow = true)
        {
            if (!String.IsNullOrWhiteSpace(SamAccountName))
            {
                using (var client = GetClient())
                {
                    var response = client.GetAsync("v1/users/" + SamAccountName + "/groups?follow=" + Follow.ToString()).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<IEnumerable<string>>();
                    }
                }
            }

            return new List<string>();
        }

        public static async Task<IEnumerable<IUser>> Users(string query, bool returnQuery = false)
        {
            if (!String.IsNullOrWhiteSpace(query))
            {
                using (var client = GetClient())
                {
                    var response = client.GetAsync("v1/users?q=" + query + "&rq=" + returnQuery.ToString()).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<IEnumerable<User>>();
                    }
                }
            }

            return new List<User>();
        }

        public static async Task<IEnumerable<IUser>> Users(IEnumerable<string> SamAccountNames)
        {
            if (SamAccountNames != null && SamAccountNames.Count() > 0)
            {
                var Users = new List<IUser>();

                foreach (var user in SamAccountNames)
                {
                    var u = await User(user);

                    if (u != null)
                        Users.Add(u);
                }
                return Users;
            }

            return new List<IUser>();
        }
    }
}
