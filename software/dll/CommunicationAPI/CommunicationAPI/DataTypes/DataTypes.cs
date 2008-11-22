﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace CommunicationAPI.DataTypes
{
    public struct Session
    {
        int id;
        User user;
    }

    /// <summary>
    /// Stores a password in MD5 format and offers encryption algorythms
    /// </summary>
    public struct Password
    {
        private string password;
        private bool isEncrypted;

        public string PasswordMD5
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }

        public string Password
        {
            set
            {
                password = Encrypt(value);
            }
        }

        public Password(string password)
        {
            this.Password = password;
        }

        public string Encrypt(string value)
        {
            MD5CryptoServiceProvider cryptMD5 = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(value);
            bs = x.ComputeHash(bs);
            return BitConverter.ToString(bs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password">the password to check</param>
        /// <param name="isEncrypted">is the password already encrypted?</param>
        /// <returns>Returns true if the password is matching</returns>
        public bool CheckPassword(string password)
        {
            return Encrypt(password) == this.password;
        }

        public bool CheckPassword(Password password)
        {
            return password.PasswordMD5 == this.PasswordMD5;
        }
    }

    public struct User
    {
        private string userName;
        private string password;
        

    }

    public struct Lamp
    {
        private int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public Lamp(int id)
        {
            this.id = id;
        }
    }

    public struct Region
    {
    }

    public struct Product
    {
    }

    public struct Sign
    {
    }

    public struct Trace
    {
    }

    public struct Advertisement
    {
    }
}
