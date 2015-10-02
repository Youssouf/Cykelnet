using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;
using Cykelnet.Models.Events;

namespace Cykelnet.Models
{
    public class AccountHelper
    {
        private static List<SelectListItem> _cyclisttypes = new List<SelectListItem>();
        private static List<SelectListItem> _countries = new List<SelectListItem>();
        private static List<SelectListItem> _birthdayDates = new List<SelectListItem>();
        private static List<SelectListItem> _birthdayMonths = new List<SelectListItem>();
        private static List<SelectListItem> _birthdayYears = new List<SelectListItem>();

        public AccountHelper()
        {
        }

        
        public static List<SelectListItem> BirthdayDates
        {
            get
            {
                if (_birthdayDates.Count == 0)
                {
                    _birthdayDates.Add(new SelectListItem() { Text = "Day", Value = "0" });
                    for (int i = 1; i < 32; i++)
                    {
                        _birthdayDates.Add(new SelectListItem() { Text = Convert.ToString(i), Value = Convert.ToString(i) });
                    }
                }
                return _birthdayDates;
            }
        }

        public static List<SelectListItem> BirthdayMonths
        {
            get
            {
                if (_birthdayMonths.Count == 0)
                {
                    _birthdayMonths.Add(new SelectListItem() { Text = "Month", Value = "0" });
                    for (int i = 1; i < 13; i++)
                    {
                        _birthdayMonths.Add(new SelectListItem() { Text = Convert.ToString(i), Value = Convert.ToString(i) });
                    }
                }
                return _birthdayMonths;
            }
        }

        public static List<SelectListItem> BirthdayYears
        {
            get
            {
                if (_birthdayYears.Count == 0)
                {
                    _birthdayYears.Add(new SelectListItem() { Text = "Year", Value = "0" });
                    for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 130; i--)
                    {
                        _birthdayYears.Add(new SelectListItem() { Text = Convert.ToString(i), Value = Convert.ToString(i) });
                    }
                }
                return _birthdayYears;
            }
        }

        public static List<SelectListItem> CyclistTypes
        {
            get
            {
                if (_cyclisttypes.Count == 0)
                {
                    _cyclisttypes.Add(new SelectListItem() { Text = "Recreational", Value = "Recreational" });
                    _cyclisttypes.Add(new SelectListItem() { Text = "Elite", Value = "Elite" });
                }
                return _cyclisttypes;
            }

        }

        #region countries
        public static List<SelectListItem> Countries
        {
            get
            {
                if (_countries.Count == 0)
                {
                    _countries.Add(new SelectListItem() { Text = "Afghanistan", Value = "Afghanistan" });
                    _countries.Add(new SelectListItem() { Text = "Albania", Value = "Albania" });
                    _countries.Add(new SelectListItem() { Text = "Algeria", Value = "Algeria" });
                    _countries.Add(new SelectListItem() { Text = "Andorra", Value = "Andorra" });
                    _countries.Add(new SelectListItem() { Text = "Angola", Value = "Angola" });
                    _countries.Add(new SelectListItem() { Text = "Antigua and Barbuda", Value = "Antigua and Barbuda" });
                    _countries.Add(new SelectListItem() { Text = "Argentina", Value = "Argentina" });
                    _countries.Add(new SelectListItem() { Text = "Armenia", Value = "Armenia" });
                    _countries.Add(new SelectListItem() { Text = "Australia", Value = "Australia" });
                    _countries.Add(new SelectListItem() { Text = "Austria", Value = "Austria" });
                    _countries.Add(new SelectListItem() { Text = "Azerbaijan", Value = "Azerbaijan" });
                    _countries.Add(new SelectListItem() { Text = "Bahamas, The", Value = "Bahamas, The" });
                    _countries.Add(new SelectListItem() { Text = "Bahrain", Value = "Bahrain" });
                    _countries.Add(new SelectListItem() { Text = "Bangladesh", Value = "Bangladesh" });
                    _countries.Add(new SelectListItem() { Text = "Barbados", Value = "Barbados" });
                    _countries.Add(new SelectListItem() { Text = "Belarus", Value = "Belarus" });
                    _countries.Add(new SelectListItem() { Text = "Belgium", Value = "Belgium" });
                    _countries.Add(new SelectListItem() { Text = "Belize", Value = "Belize" });
                    _countries.Add(new SelectListItem() { Text = "Benin", Value = "Benin" });
                    _countries.Add(new SelectListItem() { Text = "Bhutan", Value = "Bhutan" });
                    _countries.Add(new SelectListItem() { Text = "Bolivia", Value = "Bolivia" });
                    _countries.Add(new SelectListItem() { Text = "Bosnia and Herzegovina", Value = "Bosnia and Herzegovina" });
                    _countries.Add(new SelectListItem() { Text = "Botswana", Value = "Botswana" });
                    _countries.Add(new SelectListItem() { Text = "Brazil", Value = "Brazil" });
                    _countries.Add(new SelectListItem() { Text = "Brunei", Value = "Brunei" });
                    _countries.Add(new SelectListItem() { Text = "Bulgaria", Value = "Bulgaria" });
                    _countries.Add(new SelectListItem() { Text = "Burkina Faso", Value = "Burkina Faso" });
                    _countries.Add(new SelectListItem() { Text = "Burundi", Value = "Burundi" });
                    _countries.Add(new SelectListItem() { Text = "Cambodia", Value = "Cambodia" });
                    _countries.Add(new SelectListItem() { Text = "Cameroon", Value = "Cameroon" });
                    _countries.Add(new SelectListItem() { Text = "Canada", Value = "Canada" });
                    _countries.Add(new SelectListItem() { Text = "Cape Verde", Value = "Cape Verde" });
                    _countries.Add(new SelectListItem() { Text = "Central African Republic", Value = "Central African Republic" });
                    _countries.Add(new SelectListItem() { Text = "Chad", Value = "Chad" });
                    _countries.Add(new SelectListItem() { Text = "Chile", Value = "Chile" });
                    _countries.Add(new SelectListItem() { Text = "China, People's Republic of", Value = "China, People's Republic of" });
                    _countries.Add(new SelectListItem() { Text = "Colombia", Value = "Colombia" });
                    _countries.Add(new SelectListItem() { Text = "Comoros", Value = "Comoros" });
                    _countries.Add(new SelectListItem() { Text = "Congo, Democratic Republic of the (Congo – Kinshasa)", Value = "Congo, Democratic Republic of the (Congo – Kinshasa)" });
                    _countries.Add(new SelectListItem() { Text = "Congo, Republic of the (Congo – Brazzaville)", Value = "Congo, Republic of the (Congo – Brazzaville)" });
                    _countries.Add(new SelectListItem() { Text = "Costa Rica", Value = "Costa Rica" });
                    _countries.Add(new SelectListItem() { Text = "Cote d'Ivoire (Ivory Coast)", Value = "Cote d'Ivoire (Ivory Coast)" });
                    _countries.Add(new SelectListItem() { Text = "Croatia", Value = "Croatia" });
                    _countries.Add(new SelectListItem() { Text = "Cuba", Value = "Cuba" });
                    _countries.Add(new SelectListItem() { Text = "Cyprus", Value = "Cyprus" });
                    _countries.Add(new SelectListItem() { Text = "Czech Republic", Value = "Czech Republic" });
                    _countries.Add(new SelectListItem() { Text = "Denmark", Value = "Denmark" });
                    _countries.Add(new SelectListItem() { Text = "Djibouti", Value = "Djibouti" });
                    _countries.Add(new SelectListItem() { Text = "Dominica", Value = "Dominica" });
                    _countries.Add(new SelectListItem() { Text = "Dominican Republic", Value = "Dominican Republic" });
                    _countries.Add(new SelectListItem() { Text = "Ecuador", Value = "Ecuador" });
                    _countries.Add(new SelectListItem() { Text = "Egypt", Value = "Egypt" });
                    _countries.Add(new SelectListItem() { Text = "El Salvador", Value = "El Salvador" });
                    _countries.Add(new SelectListItem() { Text = "Equatorial Guinea", Value = "Equatorial Guinea" });
                    _countries.Add(new SelectListItem() { Text = "Eritrea", Value = "Eritrea" });
                    _countries.Add(new SelectListItem() { Text = "Estonia", Value = "Estonia" });
                    _countries.Add(new SelectListItem() { Text = "Ethiopia", Value = "Ethiopia" });
                    _countries.Add(new SelectListItem() { Text = "Fiji", Value = "Fiji" });
                    _countries.Add(new SelectListItem() { Text = "Finland", Value = "Finland" });
                    _countries.Add(new SelectListItem() { Text = "France", Value = "France" });
                    _countries.Add(new SelectListItem() { Text = "Gabon", Value = "Gabon" });
                    _countries.Add(new SelectListItem() { Text = "Gambia, The", Value = "Gambia, The" });
                    _countries.Add(new SelectListItem() { Text = "Georgia", Value = "Georgia" });
                    _countries.Add(new SelectListItem() { Text = "Germany", Value = "Germany" });
                    _countries.Add(new SelectListItem() { Text = "Ghana", Value = "Ghana" });
                    _countries.Add(new SelectListItem() { Text = "Greece", Value = "Greece" });
                    _countries.Add(new SelectListItem() { Text = "Grenada", Value = "Grenada" });
                    _countries.Add(new SelectListItem() { Text = "Guatemala", Value = "Guatemala" });
                    _countries.Add(new SelectListItem() { Text = "Guinea", Value = "Guinea" });
                    _countries.Add(new SelectListItem() { Text = "Guinea-Bissau", Value = "Guinea-Bissau" });
                    _countries.Add(new SelectListItem() { Text = "Guyana", Value = "Guyana" });
                    _countries.Add(new SelectListItem() { Text = "Haiti", Value = "Haiti" });
                    _countries.Add(new SelectListItem() { Text = "Honduras", Value = "Honduras" });
                    _countries.Add(new SelectListItem() { Text = "Hungary", Value = "Hungary" });
                    _countries.Add(new SelectListItem() { Text = "Iceland", Value = "Iceland" });
                    _countries.Add(new SelectListItem() { Text = "India", Value = "India" });
                    _countries.Add(new SelectListItem() { Text = "Indonesia", Value = "Indonesia" });
                    _countries.Add(new SelectListItem() { Text = "Iran", Value = "Iran" });
                    _countries.Add(new SelectListItem() { Text = "Iraq", Value = "Iraq" });
                    _countries.Add(new SelectListItem() { Text = "Ireland", Value = "Ireland" });
                    _countries.Add(new SelectListItem() { Text = "Israel", Value = "Israel" });
                    _countries.Add(new SelectListItem() { Text = "Italy", Value = "Italy" });
                    _countries.Add(new SelectListItem() { Text = "Jamaica", Value = "Jamaica" });
                    _countries.Add(new SelectListItem() { Text = "Japan", Value = "Japan" });
                    _countries.Add(new SelectListItem() { Text = "Jordan", Value = "Jordan" });
                    _countries.Add(new SelectListItem() { Text = "Kazakhstan", Value = "Kazakhstan" });
                    _countries.Add(new SelectListItem() { Text = "Kenya", Value = "Kenya" });
                    _countries.Add(new SelectListItem() { Text = "Kiribati", Value = "Kiribati" });
                    _countries.Add(new SelectListItem() { Text = "Korea, Democratic People's Republic of (North Korea)", Value = "Korea, Democratic People's Republic of (North Korea)" });
                    _countries.Add(new SelectListItem() { Text = "Korea, Republic of  (South Korea)", Value = "Korea, Republic of  (South Korea)" });
                    _countries.Add(new SelectListItem() { Text = "Kuwait", Value = "Kuwait" });
                    _countries.Add(new SelectListItem() { Text = "Kyrgyzstan", Value = "Kyrgyzstan" });
                    _countries.Add(new SelectListItem() { Text = "Laos", Value = "Laos" });
                    _countries.Add(new SelectListItem() { Text = "Latvia", Value = "Latvia" });
                    _countries.Add(new SelectListItem() { Text = "Lebanon", Value = "Lebanon" });
                    _countries.Add(new SelectListItem() { Text = "Lesotho", Value = "Lesotho" });
                    _countries.Add(new SelectListItem() { Text = "Liberia", Value = "Liberia" });
                    _countries.Add(new SelectListItem() { Text = "Libya", Value = "Libya" });
                    _countries.Add(new SelectListItem() { Text = "Liechtenstein", Value = "Liechtenstein" });
                    _countries.Add(new SelectListItem() { Text = "Lithuania", Value = "Lithuania" });
                    _countries.Add(new SelectListItem() { Text = "Luxembourg", Value = "Luxembourg" });
                    _countries.Add(new SelectListItem() { Text = "Macedonia", Value = "Macedonia" });
                    _countries.Add(new SelectListItem() { Text = "Madagascar", Value = "Madagascar" });
                    _countries.Add(new SelectListItem() { Text = "Malawi", Value = "Malawi" });
                    _countries.Add(new SelectListItem() { Text = "Malaysia", Value = "Malaysia" });
                    _countries.Add(new SelectListItem() { Text = "Maldives", Value = "Maldives" });
                    _countries.Add(new SelectListItem() { Text = "Mali", Value = "Mali" });
                    _countries.Add(new SelectListItem() { Text = "Malta", Value = "Malta" });
                    _countries.Add(new SelectListItem() { Text = "Marshall Islands", Value = "Marshall Islands" });
                    _countries.Add(new SelectListItem() { Text = "Mauritania", Value = "Mauritania" });
                    _countries.Add(new SelectListItem() { Text = "Mauritius", Value = "Mauritius" });
                    _countries.Add(new SelectListItem() { Text = "Mexico", Value = "Mexico" });
                    _countries.Add(new SelectListItem() { Text = "Micronesia", Value = "Micronesia" });
                    _countries.Add(new SelectListItem() { Text = "Moldova", Value = "Moldova" });
                    _countries.Add(new SelectListItem() { Text = "Monaco", Value = "Monaco" });
                    _countries.Add(new SelectListItem() { Text = "Mongolia", Value = "Mongolia" });
                    _countries.Add(new SelectListItem() { Text = "Montenegro", Value = "Montenegro" });
                    _countries.Add(new SelectListItem() { Text = "Morocco", Value = "Morocco" });
                    _countries.Add(new SelectListItem() { Text = "Mozambique", Value = "Mozambique" });
                    _countries.Add(new SelectListItem() { Text = "Myanmar (Burma)", Value = "Myanmar (Burma)" });
                    _countries.Add(new SelectListItem() { Text = "Namibia", Value = "Namibia" });
                    _countries.Add(new SelectListItem() { Text = "Nauru", Value = "Nauru" });
                    _countries.Add(new SelectListItem() { Text = "Nepal", Value = "Nepal" });
                    _countries.Add(new SelectListItem() { Text = "Netherlands", Value = "Netherlands" });
                    _countries.Add(new SelectListItem() { Text = "New Zealand", Value = "New Zealand" });
                    _countries.Add(new SelectListItem() { Text = "Nicaragua", Value = "Nicaragua" });
                    _countries.Add(new SelectListItem() { Text = "Niger", Value = "Niger" });
                    _countries.Add(new SelectListItem() { Text = "Nigeria", Value = "Nigeria" });
                    _countries.Add(new SelectListItem() { Text = "Norway", Value = "Norway" });
                    _countries.Add(new SelectListItem() { Text = "Oman", Value = "Oman" });
                    _countries.Add(new SelectListItem() { Text = "Pakistan", Value = "Pakistan" });
                    _countries.Add(new SelectListItem() { Text = "Palau", Value = "Palau" });
                    _countries.Add(new SelectListItem() { Text = "Panama", Value = "Panama" });
                    _countries.Add(new SelectListItem() { Text = "Papua New Guinea", Value = "Papua New Guinea" });
                    _countries.Add(new SelectListItem() { Text = "Paraguay", Value = "Paraguay" });
                    _countries.Add(new SelectListItem() { Text = "Peru", Value = "Peru" });
                    _countries.Add(new SelectListItem() { Text = "Philippines", Value = "Philippines" });
                    _countries.Add(new SelectListItem() { Text = "Poland", Value = "Poland" });
                    _countries.Add(new SelectListItem() { Text = "Portugal", Value = "Portugal" });
                    _countries.Add(new SelectListItem() { Text = "Qatar", Value = "Qatar" });
                    _countries.Add(new SelectListItem() { Text = "Romania", Value = "Romania" });
                    _countries.Add(new SelectListItem() { Text = "Russia", Value = "Russia" });
                    _countries.Add(new SelectListItem() { Text = "Rwanda", Value = "Rwanda" });
                    _countries.Add(new SelectListItem() { Text = "Saint Kitts and Nevis", Value = "Saint Kitts and Nevis" });
                    _countries.Add(new SelectListItem() { Text = "Saint Lucia", Value = "Saint Lucia" });
                    _countries.Add(new SelectListItem() { Text = "Saint Vincent and the Grenadines", Value = "Saint Vincent and the Grenadines" });
                    _countries.Add(new SelectListItem() { Text = "Samoa", Value = "Samoa" });
                    _countries.Add(new SelectListItem() { Text = "San Marino", Value = "San Marino" });
                    _countries.Add(new SelectListItem() { Text = "Sao Tome and Principe", Value = "Sao Tome and Principe" });
                    _countries.Add(new SelectListItem() { Text = "Saudi Arabia", Value = "Saudi Arabia" });
                    _countries.Add(new SelectListItem() { Text = "Senegal", Value = "Senegal" });
                    _countries.Add(new SelectListItem() { Text = "Serbia", Value = "Serbia" });
                    _countries.Add(new SelectListItem() { Text = "Seychelles", Value = "Seychelles" });
                    _countries.Add(new SelectListItem() { Text = "Sierra Leone", Value = "Sierra Leone" });
                    _countries.Add(new SelectListItem() { Text = "Singapore", Value = "Singapore" });
                    _countries.Add(new SelectListItem() { Text = "Slovakia", Value = "Slovakia" });
                    _countries.Add(new SelectListItem() { Text = "Slovenia", Value = "Slovenia" });
                    _countries.Add(new SelectListItem() { Text = "Solomon Islands", Value = "Solomon Islands" });
                    _countries.Add(new SelectListItem() { Text = "Somalia", Value = "Somalia" });
                    _countries.Add(new SelectListItem() { Text = "South Africa", Value = "South Africa" });
                    _countries.Add(new SelectListItem() { Text = "Spain", Value = "Spain" });
                    _countries.Add(new SelectListItem() { Text = "Sri Lanka", Value = "Sri Lanka" });
                    _countries.Add(new SelectListItem() { Text = "Sudan", Value = "Sudan" });
                    _countries.Add(new SelectListItem() { Text = "Suriname", Value = "Suriname" });
                    _countries.Add(new SelectListItem() { Text = "Swaziland", Value = "Swaziland" });
                    _countries.Add(new SelectListItem() { Text = "Sweden", Value = "Sweden" });
                    _countries.Add(new SelectListItem() { Text = "Switzerland", Value = "Switzerland" });
                    _countries.Add(new SelectListItem() { Text = "Syria", Value = "Syria" });
                    _countries.Add(new SelectListItem() { Text = "Tajikistan", Value = "Tajikistan" });
                    _countries.Add(new SelectListItem() { Text = "Tanzania", Value = "Tanzania" });
                    _countries.Add(new SelectListItem() { Text = "Thailand", Value = "Thailand" });
                    _countries.Add(new SelectListItem() { Text = "Timor-Leste (East Timor)", Value = "Timor-Leste (East Timor)" });
                    _countries.Add(new SelectListItem() { Text = "Togo", Value = "Togo" });
                    _countries.Add(new SelectListItem() { Text = "Tonga", Value = "Tonga" });
                    _countries.Add(new SelectListItem() { Text = "Trinidad and Tobago", Value = "Trinidad and Tobago" });
                    _countries.Add(new SelectListItem() { Text = "Tunisia", Value = "Tunisia" });
                    _countries.Add(new SelectListItem() { Text = "Turkey", Value = "Turkey" });
                    _countries.Add(new SelectListItem() { Text = "Turkmenistan", Value = "Turkmenistan" });
                    _countries.Add(new SelectListItem() { Text = "Tuvalu", Value = "Tuvalu" });
                    _countries.Add(new SelectListItem() { Text = "Uganda", Value = "Uganda" });
                    _countries.Add(new SelectListItem() { Text = "Ukraine", Value = "Ukraine" });
                    _countries.Add(new SelectListItem() { Text = "United Arab Emirates", Value = "United Arab Emirates" });
                    _countries.Add(new SelectListItem() { Text = "United Kingdom", Value = "United Kingdom" });
                    _countries.Add(new SelectListItem() { Text = "United States", Value = "United States" });
                    _countries.Add(new SelectListItem() { Text = "Uruguay", Value = "Uruguay" });
                    _countries.Add(new SelectListItem() { Text = "Uzbekistan", Value = "Uzbekistan" });
                    _countries.Add(new SelectListItem() { Text = "Vanuatu", Value = "Vanuatu" });
                    _countries.Add(new SelectListItem() { Text = "Vatican City", Value = "Vatican City" });
                    _countries.Add(new SelectListItem() { Text = "Venezuela", Value = "Venezuela" });
                    _countries.Add(new SelectListItem() { Text = "Vietnam", Value = "Vietnam" });
                    _countries.Add(new SelectListItem() { Text = "Yemen", Value = "Yemen" });
                    _countries.Add(new SelectListItem() { Text = "Zambia", Value = "Zambia" });
                    _countries.Add(new SelectListItem() { Text = "Zimbabwe", Value = "Zimbabwe" });
                    _countries.Add(new SelectListItem() { Text = "Abkhazia", Value = "Abkhazia" });
                    _countries.Add(new SelectListItem() { Text = "China, Republic of (Taiwan)", Value = "China, Republic of (Taiwan)" });
                    _countries.Add(new SelectListItem() { Text = "Nagorno-Karabakh", Value = "Nagorno-Karabakh" });
                    _countries.Add(new SelectListItem() { Text = "Northern Cyprus", Value = "Northern Cyprus" });
                    _countries.Add(new SelectListItem() { Text = "Pridnestrovie (Transnistria)", Value = "Pridnestrovie (Transnistria)" });
                    _countries.Add(new SelectListItem() { Text = "Somaliland", Value = "Somaliland" });
                    _countries.Add(new SelectListItem() { Text = "South Ossetia", Value = "South Ossetia" });
                    _countries.Add(new SelectListItem() { Text = "Ashmore and Cartier Islands", Value = "Ashmore and Cartier Islands" });
                    _countries.Add(new SelectListItem() { Text = "Christmas Island", Value = "Christmas Island" });
                    _countries.Add(new SelectListItem() { Text = "Cocos (Keeling) Islands", Value = "Cocos (Keeling) Islands" });
                    _countries.Add(new SelectListItem() { Text = "Coral Sea Islands", Value = "Coral Sea Islands" });
                    _countries.Add(new SelectListItem() { Text = "Heard Island and McDonald Islands", Value = "Heard Island and McDonald Islands" });
                    _countries.Add(new SelectListItem() { Text = "Norfolk Island", Value = "Norfolk Island" });
                    _countries.Add(new SelectListItem() { Text = "New Caledonia", Value = "New Caledonia" });
                    _countries.Add(new SelectListItem() { Text = "French Polynesia", Value = "French Polynesia" });
                    _countries.Add(new SelectListItem() { Text = "Mayotte", Value = "Mayotte" });
                    _countries.Add(new SelectListItem() { Text = "Saint Barthelemy", Value = "Saint Barthelemy" });
                    _countries.Add(new SelectListItem() { Text = "Saint Martin", Value = "Saint Martin" });
                    _countries.Add(new SelectListItem() { Text = "Saint Pierre and Miquelon", Value = "Saint Pierre and Miquelon" });
                    _countries.Add(new SelectListItem() { Text = "Wallis and Futuna", Value = "Wallis and Futuna" });
                    _countries.Add(new SelectListItem() { Text = "French Southern and Antarctic Lands", Value = "French Southern and Antarctic Lands" });
                    _countries.Add(new SelectListItem() { Text = "Clipperton Island", Value = "Clipperton Island" });
                    _countries.Add(new SelectListItem() { Text = "Bouvet Island", Value = "Bouvet Island" });
                    _countries.Add(new SelectListItem() { Text = "Cook Islands", Value = "Cook Islands" });
                    _countries.Add(new SelectListItem() { Text = "Niue", Value = "Niue" });
                    _countries.Add(new SelectListItem() { Text = "Tokelau", Value = "Tokelau" });
                    _countries.Add(new SelectListItem() { Text = "Guernsey", Value = "Guernsey" });
                    _countries.Add(new SelectListItem() { Text = "Isle of Man", Value = "Isle of Man" });
                    _countries.Add(new SelectListItem() { Text = "Jersey", Value = "Jersey" });
                    _countries.Add(new SelectListItem() { Text = "Anguilla", Value = "Anguilla" });
                    _countries.Add(new SelectListItem() { Text = "Bermuda", Value = "Bermuda" });
                    _countries.Add(new SelectListItem() { Text = "British Indian Ocean Territory", Value = "British Indian Ocean Territory" });
                    _countries.Add(new SelectListItem() { Text = "British Sovereign Base Areas", Value = "British Sovereign Base Areas" });
                    _countries.Add(new SelectListItem() { Text = "British Virgin Islands", Value = "British Virgin Islands" });
                    _countries.Add(new SelectListItem() { Text = "Cayman Islands", Value = "Cayman Islands" });
                    _countries.Add(new SelectListItem() { Text = "Falkland Islands (Islas Malvinas)", Value = "Falkland Islands (Islas Malvinas)" });
                    _countries.Add(new SelectListItem() { Text = "Gibraltar", Value = "Gibraltar" });
                    _countries.Add(new SelectListItem() { Text = "Montserrat", Value = "Montserrat" });
                    _countries.Add(new SelectListItem() { Text = "Pitcairn Islands", Value = "Pitcairn Islands" });
                    _countries.Add(new SelectListItem() { Text = "Saint Helena", Value = "Saint Helena" });
                    _countries.Add(new SelectListItem() { Text = "South Georgia and the South Sandwich Islands", Value = "South Georgia and the South Sandwich Islands" });
                    _countries.Add(new SelectListItem() { Text = "Turks and Caicos Islands", Value = "Turks and Caicos Islands" });
                    _countries.Add(new SelectListItem() { Text = "Northern Mariana Islands", Value = "Northern Mariana Islands" });
                    _countries.Add(new SelectListItem() { Text = "Puerto Rico", Value = "Puerto Rico" });
                    _countries.Add(new SelectListItem() { Text = "American Samoa", Value = "American Samoa" });
                    _countries.Add(new SelectListItem() { Text = "Baker Island", Value = "Baker Island" });
                    _countries.Add(new SelectListItem() { Text = "Guam", Value = "Guam" });
                    _countries.Add(new SelectListItem() { Text = "Howland Island", Value = "Howland Island" });
                    _countries.Add(new SelectListItem() { Text = "Jarvis Island", Value = "Jarvis Island" });
                    _countries.Add(new SelectListItem() { Text = "Johnston Atoll", Value = "Johnston Atoll" });
                    _countries.Add(new SelectListItem() { Text = "Kingman Reef", Value = "Kingman Reef" });
                    _countries.Add(new SelectListItem() { Text = "Midway Islands", Value = "Midway Islands" });
                    _countries.Add(new SelectListItem() { Text = "Navassa Island", Value = "Navassa Island" });
                    _countries.Add(new SelectListItem() { Text = "Palmyra Atoll", Value = "Palmyra Atoll" });
                    _countries.Add(new SelectListItem() { Text = "U.S. Virgin Islands", Value = "U.S. Virgin Islands" });
                    _countries.Add(new SelectListItem() { Text = "Wake Island", Value = "Wake Island" });
                    _countries.Add(new SelectListItem() { Text = "Hong Kong", Value = "Hong Kong" });
                    _countries.Add(new SelectListItem() { Text = "Macau", Value = "Macau" });
                    _countries.Add(new SelectListItem() { Text = "Faroe Islands", Value = "Faroe Islands" });
                    _countries.Add(new SelectListItem() { Text = "Greenland", Value = "Greenland" });
                    _countries.Add(new SelectListItem() { Text = "French Guiana", Value = "French Guiana" });
                    _countries.Add(new SelectListItem() { Text = "Guadeloupe", Value = "Guadeloupe" });
                    _countries.Add(new SelectListItem() { Text = "Martinique", Value = "Martinique" });
                    _countries.Add(new SelectListItem() { Text = "Reunion", Value = "Reunion" });
                    _countries.Add(new SelectListItem() { Text = "Aland", Value = "Aland" });
                    _countries.Add(new SelectListItem() { Text = "Aruba", Value = "Aruba" });
                    _countries.Add(new SelectListItem() { Text = "Netherlands Antilles", Value = "Netherlands Antilles" });
                    _countries.Add(new SelectListItem() { Text = "Svalbard", Value = "Svalbard" });
                    _countries.Add(new SelectListItem() { Text = "Ascension", Value = "Ascension" });
                    _countries.Add(new SelectListItem() { Text = "Tristan da Cunha", Value = "Tristan da Cunha" });
                    _countries.Add(new SelectListItem() { Text = "Antarctica", Value = "Antarctica" });
                    _countries.Add(new SelectListItem() { Text = "Kosovo", Value = "Kosovo" });
                    _countries.Add(new SelectListItem() { Text = "Palestinian Territories (Gaza Strip and West Bank)", Value = "Palestinian Territories (Gaza Strip and West Bank)" });
                    _countries.Add(new SelectListItem() { Text = "Western Sahara", Value = "Western Sahara" });
                    _countries.Add(new SelectListItem() { Text = "Australian Antarctic Territory", Value = "Australian Antarctic Territory" });
                    _countries.Add(new SelectListItem() { Text = "Ross Dependency", Value = "Ross Dependency" });
                    _countries.Add(new SelectListItem() { Text = "Peter I Island", Value = "Peter I Island" });
                    _countries.Add(new SelectListItem() { Text = "Queen Maud Land", Value = "Queen Maud Land" });
                    _countries.Add(new SelectListItem() { Text = "British Antarctic Territory", Value = "British Antarctic Territory" });
                }
                return _countries;
            }
        }

        #endregion
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        private List<SelectListItem> _cyclisttypes = AccountHelper.CyclistTypes;
        private List<SelectListItem> _countries = AccountHelper.Countries;
        private List<SelectListItem> _birthdayDates = AccountHelper.BirthdayDates;
        private List<SelectListItem> _birthdayMonths = AccountHelper.BirthdayMonths;
        private List<SelectListItem> _birthdayYears = AccountHelper.BirthdayYears;


        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Cyclist Type")]
        public string CyclistType { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Birthday")]
        public string BirthdayDay { get; set; }

        [Display(Name = "Birthday")]
        public string BirthdayMonth { get; set; }

        [Display(Name = "Birthday")]
        public string BirthdayYear { get; set; }

        //Only used for displaying label and validation error
        [Display(Name = "Select an avatar")]
        public string Avatar { get; set; }


        public List<SelectListItem> BirthdayDates
        {
            get
            {
                return _birthdayDates;
            }
        }

        public List<SelectListItem> BirthdayMonths
        {
            get
            {
                return _birthdayMonths;
            }
        }

        public List<SelectListItem> BirthdayYears
        {
            get
            {
                return _birthdayYears;
            }
        }

        public List<SelectListItem> CyclistTypes
        {
            get
            {
                return _cyclisttypes;
            }

        }

        #region countries
        public List<SelectListItem> Countries
        {
            get
            {
                return _countries;
            }
        }

        #endregion

    }

    public class EditProfileModel
    {
        private List<SelectListItem> _cyclisttypes = AccountHelper.CyclistTypes;
        private List<SelectListItem> _countries = AccountHelper.Countries;
        private List<SelectListItem> _birthdayDates = AccountHelper.BirthdayDates;
        private List<SelectListItem> _birthdayMonths = AccountHelper.BirthdayMonths;
        private List<SelectListItem> _birthdayYears = AccountHelper.BirthdayYears;

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Cyclist Type")]
        public string CyclistType { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Birthday")]
        public string BirthdayDay { get; set; }

        [Display(Name = "Birthday")]
        public string BirthdayMonth { get; set; }

        [Display(Name = "Birthday")]
        public string BirthdayYear { get; set; }

        //Only used for displaying label and validation error
        [Display(Name = "Select an avatar")]
        public string Avatar { get; set; }


        public List<SelectListItem> BirthdayDates
        {
            get
            {
                return _birthdayDates;
            }
        }

        public List<SelectListItem> BirthdayMonths
        {
            get
            {
                return _birthdayMonths;
            }
        }

        public List<SelectListItem> BirthdayYears
        {
            get
            {
                return _birthdayYears;
            }
        }

        public List<SelectListItem> CyclistTypes
        {
            get
            {
                return _cyclisttypes;
            }

        }

        #region countries
        public List<SelectListItem> Countries
        {
            get
            {
                return _countries;
            }
        }

        #endregion

    }

    public class PublicProfileModel
    {
        [Display]
        public string UserName { get; set; }

        public Guid id { get; set; }

        public bool hasAge { get; set; }
        
        [Display(Name = "Name:")]
        public string FullName { get; set; }

        [Display(Name = "Age:")]
        public string Age { get; set; }

        [Display(Name = "Cyclist Type:")]
        public string CyclistType { get; set; }

        [Display]
        public List<IEvent> Events { get; set; }
    }

    public class AccountModel
    {
        public static bool checkUserID(Guid? id)
        {
            if (!id.HasValue)
            {
                return false;
            }

            if (Membership.GetUser(id) == null)
            {
                return false;
            }

            return true;
        }
    }
}
