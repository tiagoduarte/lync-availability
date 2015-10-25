using LyncAvailability.Properties;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using System;
using System.Collections.Generic;

namespace LyncAvailability
{
    class LyncManagerStatus
    {
        private string _uri;
        private LyncClient _client;

        private bool _done = false;
        public bool Done
        {
            get { return _done; }
        }

        /// <summary>
        /// build up the asynchronous call to contact manager
        /// </summary>
        /// <param name="email"></param>
        public LyncManagerStatus(string email)
        {
            try
            {
                //Console.WriteLine("starting...");
                _uri = email;
                _client = Microsoft.Lync.Model.LyncClient.GetClient();
                _client.ContactManager.BeginSearch(
                    _uri,
                    SearchProviders.GlobalAddressList,
                    SearchFields.EmailAddresses,
                    SearchOptions.IncludeContactsWithoutSipOrTelUri,
                    2,
                    BeginSearchCallback,
                    new object[] { _client.ContactManager, _uri }
                );
            }
            catch(Microsoft.Lync.Model.NotSignedInException)
            {
                Console.WriteLine(Resources.NotSignedIn);
                _done = true;
            }
            catch(Microsoft.Lync.Model.ClientNotFoundException)
            {
                Console.WriteLine(Resources.NotRunning);
                _done = true;
            }
        }

        /// <summary>
        /// helper method to look for potential time information
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public string GetElapsedIdleTime(Contact contact)
        {
            try
            {
                DateTime idleSince = ((DateTime)contact.GetContactInformation(ContactInformationType.IdleStartTime)).ToLocalTime();
                return TimeHelper.DisplayTimeAgo(idleSince);
            }
            catch
            {
                return "N/A";
            }
        }

        /// <summary>
        /// lync search callback to parse the results of the query
        /// </summary>
        /// <param name="r"></param>
        private void BeginSearchCallback(IAsyncResult r)
        {
            //Console.WriteLine("callback here");
            object[] asyncState = (object[])r.AsyncState;
            ContactManager cm = (ContactManager)asyncState[0];
            try
            {
                SearchResults results = cm.EndSearch(r);
                if (results.AllResults.Count == 0)
                {
                    Console.WriteLine(Resources.NoResults);
                    _done = true;
                }
                else if (results.AllResults.Count == 1)
                {
                    Contact contact = results.Contacts[0];

                    ContactAvailability availability = (ContactAvailability)contact.GetContactInformation(ContactInformationType.Availability);

                    //todo: check idle time without hanging
                    string timeElapsed = GetElapsedIdleTime(contact);
                    string userStatus = availability.ToString();
                    string statusCode = contact.GetContactInformation(ContactInformationType.Availability).ToString();
                    Console.Write(String.Format("Status: {0}, Code: {1}, Last seen: {2}", userStatus, statusCode, timeElapsed));
                    
                    //http://rcosic.wordpress.com/2011/11/17/availability-presence-in-lync-client/
                    //Invalid (-1),
                    //None (0) – Do not use this enumerator. This flag indicates that the cotact state is unspecified.,
                    //Free (3500) – A flag indicating that the contact is available,
                    //FreeIdle (5000) – Contact is free but inactive,
                    //Busy (6500) – A flag indicating that the contact is busy and inactive,
                    //BusyIdle (7500) – Contact is busy but inactive,
                    //DoNotDisturb (9500) – A flag indicating that the contact does not want to be disturbed,
                    //TemporarilyAway (12500) – A flag indicating that the contact is temporarily away,
                    //Away (15500) – A flag indicating that the contact is away,
                    //Offline (18500) – A flag indicating that the contact is signed out.

                    _done = true;
                }
                else
                {
                    Console.WriteLine(Resources.MultipleResults);
                    _done = true;
                }
            }
            catch (SearchException se)
            {
                Console.WriteLine(Resources.SearchFailed + se.Reason.ToString());
                _done = true;
            }
            _client.ContactManager.EndSearch(r);
        }

    }
}