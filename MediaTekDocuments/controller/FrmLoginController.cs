using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using System.Security.Cryptography;
using System.Text;
using MediaTekDocuments.view;
using System.Windows.Forms;

namespace MediaTekDocuments.controller
{
    class FrmLoginController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        private Utilisateur utilisateur = null;


        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmLoginController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// Lance la vue principale
        /// </summary>
        private void Init()
        {
            FrmMediatek mediatek = new FrmMediatek(utilisateur);
            mediatek.Show();
        }

        /// <summary>
        /// Retourne vrai si l'utilisateur existe dans la BDD
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool GetLogin(string mail, string password)
        {
            password = "Mediatek" + password;
            string hash = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                 hash = GetHash(sha256Hash, password);
            }
            utilisateur = access.GetLogin(mail, hash);
            if (utilisateur != null)
            {
                Init();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retourne le hash au format string
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
