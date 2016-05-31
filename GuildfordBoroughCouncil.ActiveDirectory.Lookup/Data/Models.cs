using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using GuildfordBoroughCouncil.ActiveDirectory.Interfaces;

namespace GuildfordBoroughCouncil.Data.Models
{
    public enum AdObjectType
    {
        User,
        Group,
        Computer
    }

    [DisplayColumn("Name")]
    [ComplexType]
    public class AdObject
    {
        public AdObject()
        {
            Type = AdObjectType.User;
        }

        public AdObject(string sAMAccountName)
        {
            SamAccountName = sAMAccountName;

            if (ActiveDirectory != null)
            {
                Name = ActiveDirectory.Name;
            }
            else
            {
                Name = sAMAccountName;
            }
        }

        public AdObjectType Type { get; set; }

        private string _Name;

        [MaxLength(300)]
        [DisplayFormat(NullDisplayText = "-")]
        public string Name
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_Name) && !String.IsNullOrWhiteSpace(this.SamAccountName))
                {
                    _Name = ActiveDirectory.Name;
                }

                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        [MaxLength(300)]
        public string SamAccountName { get; set; }

        [ScaffoldColumn(false)]
        [DataType(DataType.Url)]
        public string ProfileUrl
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(SamAccountName))
                {
                    return @"http://my.loop.guildford.gov.uk/Person.aspx?accountname=" + SamAccountName;
                }
                else
                {
                    return null;
                }
            }
        }

        [IgnoreDataMember]
        public virtual IUser ActiveDirectory
        {
            get
            {
                return GuildfordBoroughCouncil.ActiveDirectory.Lookup.User(SamAccountName).Result;
            }
        }

        [ScaffoldColumn(false)]
        [IgnoreDataMember]
        public virtual bool HasValue
        {
            get
            {
                return (!String.IsNullOrWhiteSpace(Name) && !String.IsNullOrWhiteSpace(SamAccountName));
            }
        }

        [ScaffoldColumn(false)]
        [IgnoreDataMember]
        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }
    }

    [DisplayColumn("Name")]
    [ComplexType]
    public class User
    {
        public User() { }

        public User(string un)
        {
            Username = un;

            if (ActiveDirectory != null)
            {
                Name = ActiveDirectory.Name;
            }
            else
            {
                Name = un;
            }
        }

        private string _Name;

        [MaxLength(300)]
        [DisplayFormat(NullDisplayText = "-")]
        public string Name
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_Name) && !String.IsNullOrWhiteSpace(this.Username))
                {
                    _Name = ActiveDirectory.Name;
                }

                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        [MaxLength(300)]
        public string Username { get; set; }

        [ScaffoldColumn(false)]
        [DataType(DataType.Url)]
        public string ProfileUrl
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Username))
                {
                    return @"http://my.loop.guildford.gov.uk/Person.aspx?accountname=" + Username;
                }
                else
                {
                    return null;
                }
            }
        }

        [IgnoreDataMember]
        public virtual IUser ActiveDirectory
        {
            get
            {
                return GuildfordBoroughCouncil.ActiveDirectory.Lookup.User(Username).Result;
            }
        }

        [ScaffoldColumn(false)]
        [IgnoreDataMember]
        public virtual bool HasValue
        {
            get
            {
                return (!String.IsNullOrWhiteSpace(Name) && !String.IsNullOrWhiteSpace(Username));
            }
        }

        [ScaffoldColumn(false)]
        [IgnoreDataMember]
        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }
    }
}